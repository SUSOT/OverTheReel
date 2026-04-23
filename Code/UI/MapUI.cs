using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class MapUI : MenuPanel
    {
        [SerializeField] private GameObject mapUiPanel;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Button[] mapButtons;
        [SerializeField] private GameObject[] lockImages;
        [SerializeField] private GameEventChannelSO mapChannel;

        private readonly HashSet<int> _unlockedMapIndices = new();

        private void Awake()
        {
            mapUiPanel.transform.localScale = Vector3.zero;
            mapUiPanel.SetActive(false);

            mapChannel.AddListener<MapStatesUpdatedEvent>(HandleMapStatesUpdated);
        }

        private void OnDestroy()
        {
            mapChannel.RemoveListener<MapStatesUpdatedEvent>(HandleMapStatesUpdated);
        }

        public override void ShowUI()
        {
            base.ShowUI();

            mapUiPanel.SetActive(true);
            mapUiPanel.transform.DOKill();
            mapUiPanel.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);

            mapChannel.RaiseEvent(MapEvent.CreateRequestMapStatesEvent());
        }

        public override void CloseUI()
        {
            base.CloseUI();

            mapUiPanel.transform.DOKill();
            mapUiPanel.transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                mapUiPanel.SetActive(false);
            });
        }

        public void SelectMap(int mapIndex)
        {
            mapChannel.RaiseEvent(MapEvent.CreateChangeMapEvent(mapIndex, false));
            CloseUI();
        }

        private void HandleMapStatesUpdated(MapStatesUpdatedEvent evt)
        {
            _unlockedMapIndices.Clear();
            if (evt.unlockedMapIndices != null)
            {
                foreach (int index in evt.unlockedMapIndices)
                {
                    _unlockedMapIndices.Add(index);
                }
            }

            UpdateMapButtonStates();
        }

        private void UpdateMapButtonStates()
        {
            if (mapButtons == null) return;

            for (int i = 0; i < mapButtons.Length; i++)
            {
                bool isUnlocked = _unlockedMapIndices.Contains(i + 1);
                mapButtons[i].interactable = isUnlocked;

                if (i < lockImages.Length && lockImages[i] != null)
                {
                    mapButtons[i].gameObject.SetActive(isUnlocked);
                    lockImages[i].SetActive(!isUnlocked);
                }
            }
        }
    }
}

