using System;
using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Fish;
using _01.Scripts.Sounds;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class MerchantUI : MenuPanel
    {
        private const string TotalCostLabel = "총 가격 : {0}Coin";

        [Header("Default")]
        [SerializeField] private Image panel;
        [SerializeField] private Image buyPanel;
        [SerializeField] private Image sellPanel;
        [SerializeField] private Image defaultPanel;
        [SerializeField] private float showDuration = 0.5f;

        [Header("Inventory View")]
        [field: SerializeField] public Transform InventoryContent { get; private set; }
        [field: SerializeField] public RectTransform SpawnArea { get; private set; }
        [SerializeField] private GameObject fishSlotPrefab;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private TextMeshProUGUI totalCostText;

        [Header("Buy View")]
        [SerializeField] private GameObject buyFishingRodPanel;
        [SerializeField] private Transform buyFishingRodContent;
        [SerializeField] private GameObject buyBoatPanel;
        [SerializeField] private TextMeshProUGUI boatCostText;
        [SerializeField] private List<ButtonPanelNamePair> panelNameByButton = new();

        [Header("Channels")]
        [SerializeField] private GameEventChannelSO merchantUiChannel;
        [SerializeField] private GameEventChannelSO inventoryChannel;
        [SerializeField] private GameEventChannelSO merchantChannel;

        [Header("Sounds")]
        [SerializeField] private SoundSO coinSound;

        private bool _usesSceneTemplateSlot;
        private readonly Dictionary<GameObject, string> _panelNameLookup = new();

        [Serializable]
        private struct ButtonPanelNamePair
        {
            public GameObject buttonObject;
            public string panelName;
        }

        private void Awake()
        {
            TryResolveFishSlotPrefab();
            RebuildPanelNameLookup();

            merchantUiChannel.AddListener<UpdateTotalCostEvent>(HandleUpdateTotalCost);
            merchantUiChannel.AddListener<ChangeBoatCostEvent>(HandleChangeBoatCost);
            merchantUiChannel.AddListener<SetUpgradePanelStateEvent>(HandleSetUpgradePanelState);
            inventoryChannel.AddListener<InventoryUpdatedEvent>(HandleInventoryUpdated);
        }

        private void OnDestroy()
        {
            merchantUiChannel.RemoveListener<UpdateTotalCostEvent>(HandleUpdateTotalCost);
            merchantUiChannel.RemoveListener<ChangeBoatCostEvent>(HandleChangeBoatCost);
            merchantUiChannel.RemoveListener<SetUpgradePanelStateEvent>(HandleSetUpgradePanelState);
            inventoryChannel.RemoveListener<InventoryUpdatedEvent>(HandleInventoryUpdated);
        }

        public override void ShowUI()
        {
            base.ShowUI();

            panel.gameObject.SetActive(true);
            inventoryPanel.gameObject.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            inventoryPanel.transform.localScale = Vector3.zero;
            panel.transform.DOKill();
            inventoryPanel.transform.DOKill();
            inventoryPanel.transform.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);
            panel.transform.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);

            inventoryChannel.RaiseEvent(InventoryEvent.CreateRequestInventoryRefreshEvent());
        }

        public override void CloseUI()
        {
            base.CloseUI();

            panel.transform.DOKill();
            inventoryPanel.transform.DOKill();
            inventoryPanel.transform.DOScale(Vector3.zero, showDuration).SetEase(Ease.InBack);
            panel.transform.DOScale(Vector3.zero, showDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    SetMainPanelState(true, false, false);
                    panel.gameObject.SetActive(false);
                });
        }

        public void ShowBuyPanel()
        {
            PlayUiSound(uiSound);
            SetMainPanelState(false, true, false);
        }

        public void ShowSellPanel()
        {
            PlayUiSound(uiSound);
            SetMainPanelState(false, false, true);
        }

        public void ShowFishingRodPanel(bool isFishingRod)
        {
            PlayUiSound(uiSound);
            buyFishingRodPanel.SetActive(isFishingRod);
            buyBoatPanel.SetActive(!isFishingRod);
        }

        public void SellFish()
        {
            PlayUiSound(coinSound);
            inventoryChannel.RaiseEvent(InventoryEvent.CreateSellFishEvent());
        }

        public void LevelUpFishingRod(int coin)
        {
            string panelName = ResolveSelectedUpgradePanelName();
            merchantChannel.RaiseEvent(MerchantEvent.CreateLevelUpFishingRodEvent(coin, panelName));
        }

        public void LevelUpBoat()
        {
            merchantChannel.RaiseEvent(MerchantEvent.CreateLevelUpBoatEvent());
        }

        public void RenderTotalCost(int totalCost)
        {
            if (totalCostText == null) return;
            totalCostText.SetText(string.Format(TotalCostLabel, totalCost));
        }

        public void RenderBoatCost(string text)
        {
            if (boatCostText == null) return;
            boatCostText.SetText(text);
        }

        public void ApplyUpgradePanelState(string panelName, bool isActive)
        {
            if (buyFishingRodContent == null || string.IsNullOrEmpty(panelName)) return;

            Transform targetPanel = buyFishingRodContent.Find(panelName);
            if (targetPanel != null)
            {
                targetPanel.gameObject.SetActive(isActive);
            }
        }

        public void RenderInventory(List<FishDataSO> fishList, Action<int, FishDataSO, bool> onSelectionChanged)
        {
            if (InventoryContent == null || fishSlotPrefab == null) return;

            foreach (Transform child in InventoryContent)
            {
                if (_usesSceneTemplateSlot && child.gameObject == fishSlotPrefab)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }

                Destroy(child.gameObject);
            }

            if (fishList == null) return;

            foreach (FishDataSO fishData in fishList)
            {
                if (fishData == null) continue;

                GameObject slotObject = Instantiate(fishSlotPrefab, InventoryContent);
                slotObject.SetActive(true);
                FishSlot fishSlot = slotObject.GetComponent<FishSlot>();
                if (fishSlot != null)
                {
                    fishSlot.SetupFishSlot(fishData, SpawnArea, onSelectionChanged);
                }
            }
        }

        private void HandleUpdateTotalCost(UpdateTotalCostEvent evt)
        {
            RenderTotalCost(evt.totalCost);
        }

        private void HandleChangeBoatCost(ChangeBoatCostEvent evt)
        {
            RenderBoatCost(evt.boatCostText);
        }

        private void HandleSetUpgradePanelState(SetUpgradePanelStateEvent evt)
        {
            ApplyUpgradePanelState(evt.panelName, evt.isActive);
        }

        private void HandleInventoryUpdated(InventoryUpdatedEvent evt)
        {
            RenderInventory(evt.fishList, HandleSlotSelectionChanged);
        }

        private void HandleSlotSelectionChanged(int slotInstanceId, FishDataSO fishData, bool isAdd)
        {
            inventoryChannel.RaiseEvent(InventoryEvent.CreateChangeSlotEvent(slotInstanceId, fishData, isAdd));
        }

        private string ResolveSelectedUpgradePanelName()
        {
            if (EventSystem.current == null) return string.Empty;
            if (EventSystem.current.currentSelectedGameObject == null) return string.Empty;

            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (_panelNameLookup.TryGetValue(selectedObject, out string panelName) && !string.IsNullOrEmpty(panelName))
            {
                return panelName;
            }

            Transform parent = selectedObject.transform.parent;
            return parent != null ? parent.name : string.Empty;
        }

        private void RebuildPanelNameLookup()
        {
            _panelNameLookup.Clear();
            if (panelNameByButton == null) return;

            foreach (ButtonPanelNamePair entry in panelNameByButton)
            {
                if (entry.buttonObject == null || string.IsNullOrEmpty(entry.panelName)) continue;
                _panelNameLookup[entry.buttonObject] = entry.panelName;
            }
        }

        private void TryResolveFishSlotPrefab()
        {
            if (fishSlotPrefab != null) return;
            if (InventoryContent == null) return;

            FishSlot existingSlot = InventoryContent.GetComponentInChildren<FishSlot>(true);
            if (existingSlot != null)
            {
                fishSlotPrefab = existingSlot.gameObject;
                _usesSceneTemplateSlot = true;
                fishSlotPrefab.SetActive(false);
            }
        }

        private void PlayUiSound(SoundSO clip)
        {
            if (clip == null) return;
            soundChannel.RaiseEvent(SoundEvent.CreatePlaySfxEvent(transform.position, clip));
        }

        private void SetMainPanelState(bool showDefault, bool showBuy, bool showSell)
        {
            defaultPanel.gameObject.SetActive(showDefault);
            buyPanel.gameObject.SetActive(showBuy);
            sellPanel.gameObject.SetActive(showSell);
        }
    }
}

