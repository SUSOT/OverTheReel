using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Manager.Merchant;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using _01.Scripts.SO.Coin;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class MerchantManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO merchantChannel;
        [SerializeField] private GameEventChannelSO merchantUiChannel;
        [SerializeField] private GameEventChannelSO mapChannel;
        [SerializeField] private GameEventChannelSO playerChannel;
        [SerializeField] private GameEventChannelSO fishingChannel;
        [SerializeField] private CoinDataMono coinDataMono;
        [SerializeField] private List<int> boatCosts = new();

        private MerchantUpgradeService _merchantUpgradeService;
        private int _boatLevel;

        private void Awake()
        {
            _merchantUpgradeService = new MerchantUpgradeService(coinDataMono, boatCosts);

            merchantChannel.AddListener<LevelUpFishingRod>(HandleLevelUpFishingRod);
            merchantChannel.AddListener<LevelUpBoat>(HandleLevelUpBoat);
        }

        private void Start()
        {
            GameSaveData data = SaveDataSystem.Load();
            _boatLevel = Mathf.Max(0, data.boatLevel);

            merchantUiChannel.RaiseEvent(
                MerchantUIEvent.CreateChangeBoatCostEvent(_merchantUpgradeService.GetBoatCostText(_boatLevel))
            );

            if (data.uiPanelStates == null) return;
            foreach (KeyValuePair<string, bool> panelState in data.uiPanelStates)
            {
                merchantUiChannel.RaiseEvent(
                    MerchantUIEvent.CreateSetUpgradePanelStateEvent(panelState.Key, panelState.Value)
                );
            }
        }

        private void OnDestroy()
        {
            merchantChannel.RemoveListener<LevelUpFishingRod>(HandleLevelUpFishingRod);
            merchantChannel.RemoveListener<LevelUpBoat>(HandleLevelUpBoat);
        }

        private void HandleLevelUpFishingRod(LevelUpFishingRod evt)
        {
            if (!_merchantUpgradeService.TryUpgradeFishingRod(evt.coin)) return;

            fishingChannel.RaiseEvent(FishingEvent.CreateLevelUpEvent());

            if (string.IsNullOrEmpty(evt.panelName)) return;

            GameSaveData data = SaveDataSystem.Load();
            data.uiPanelStates ??= new Dictionary<string, bool>();
            data.uiPanelStates[evt.panelName] = false;
            SaveDataSystem.Save(data);

            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateSetUpgradePanelStateEvent(evt.panelName, false));
        }

        private void HandleLevelUpBoat(LevelUpBoat evt)
        {
            BoatUpgradeResult result = _merchantUpgradeService.TryUpgradeBoat(_boatLevel);
            if (!result.isSuccess) return;

            _boatLevel = result.newBoatLevel;
            GameSaveData data = SaveDataSystem.Load();
            data.boatLevel = _boatLevel;
            SaveDataSystem.Save(data);

            playerChannel.RaiseEvent(PlayerEvent.CreateChangeBoatEvent(_boatLevel));
            mapChannel.RaiseEvent(MapEvent.CreateUnlockMapEvent(result.unlockedMapIndex));
            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateChangeBoatCostEvent(result.nextBoatCostText));
        }

    }
}
