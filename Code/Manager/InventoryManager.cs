using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Fish;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using _01.Scripts.SO.Coin;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private FishDataListSO fishDataList;
        [SerializeField] private CoinDataMono coinDataMono;
        [SerializeField] private GameEventChannelSO inventoryChannel;
        [SerializeField] private GameEventChannelSO merchantUiChannel;

        private readonly List<FishDataSO> _fishInventory = new();
        private readonly Dictionary<int, FishDataSO> _selectedSlots = new();

        private void Awake()
        {
            LoadInventory();

            inventoryChannel.AddListener<ChangeSlotEvent>(HandleChangeSlot);
            inventoryChannel.AddListener<SellFishEvent>(HandleSellFish);
            inventoryChannel.AddListener<StackFishEvent>(HandleStackFish);
            inventoryChannel.AddListener<RequestInventoryRefreshEvent>(HandleRequestInventoryRefresh);

            PublishInventoryChanged();
            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(0));
        }

        private void OnDestroy()
        {
            inventoryChannel.RemoveListener<ChangeSlotEvent>(HandleChangeSlot);
            inventoryChannel.RemoveListener<SellFishEvent>(HandleSellFish);
            inventoryChannel.RemoveListener<StackFishEvent>(HandleStackFish);
            inventoryChannel.RemoveListener<RequestInventoryRefreshEvent>(HandleRequestInventoryRefresh);
        }

        private void HandleRequestInventoryRefresh(RequestInventoryRefreshEvent evt)
        {
            PublishInventoryChanged();
            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(0));
        }

        private void HandleStackFish(StackFishEvent evt)
        {
            if (evt.fishData == null) return;

            _fishInventory.Add(evt.fishData);
            SaveInventory();

            PublishInventoryChanged();
            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(0));
        }

        private void HandleChangeSlot(ChangeSlotEvent evt)
        {
            if (evt.fishData == null) return;

            if (evt.isAdd)
            {
                _selectedSlots[evt.slotInstanceId] = evt.fishData;
            }
            else
            {
                _selectedSlots.Remove(evt.slotInstanceId);
            }

            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(CalculateTotalCost()));
        }

        private void HandleSellFish(SellFishEvent evt)
        {
            List<FishDataSO> selectedFish = new List<FishDataSO>(_selectedSlots.Values);
            if (selectedFish.Count == 0)
            {
                merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(0));
                return;
            }

            int totalCost = CalculateTotalCost();

            foreach (FishDataSO fishData in selectedFish)
            {
                int fishIndex = _fishInventory.FindIndex(fish => fish == fishData);
                if (fishIndex >= 0)
                {
                    _fishInventory.RemoveAt(fishIndex);
                }
            }

            _selectedSlots.Clear();
            SaveInventory();

            if (coinDataMono != null)
            {
                coinDataMono.ChangeCoinAmount(totalCost, true);
            }

            PublishInventoryChanged();
            merchantUiChannel.RaiseEvent(MerchantUIEvent.CreateUpdateTotalCostEvent(0));
        }

        private int CalculateTotalCost()
        {
            int totalCost = 0;
            foreach (FishDataSO fishData in _selectedSlots.Values)
            {
                if (fishData == null) continue;
                totalCost += fishData.coin;
            }

            return totalCost;
        }

        private void PublishInventoryChanged()
        {
            _selectedSlots.Clear();
            inventoryChannel.RaiseEvent(InventoryEvent.CreateInventoryUpdatedEvent(new List<FishDataSO>(_fishInventory)));
        }

        private void LoadInventory()
        {
            _fishInventory.Clear();

            if (fishDataList == null || fishDataList.allFishDatas == null)
            {
                return;
            }

            GameSaveData data = SaveDataSystem.Load();
            if (data.savedFishIDs == null)
            {
                return;
            }

            foreach (string fishId in data.savedFishIDs)
            {
                FishDataSO fish = fishDataList.allFishDatas.Find(item => item != null && item.name == fishId);
                if (fish != null)
                {
                    _fishInventory.Add(fish);
                }
            }
        }

        private void SaveInventory()
        {
            GameSaveData data = SaveDataSystem.Load();

            List<string> savedIds = new List<string>();
            for (int i = 0; i < _fishInventory.Count; i++)
            {
                FishDataSO fish = _fishInventory[i];
                if (fish == null) continue;
                savedIds.Add(fish.name);
            }

            data.savedFishIDs = savedIds;
            SaveDataSystem.Save(data);
        }
    }
}
