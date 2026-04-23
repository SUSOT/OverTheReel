using _01.Scripts.Fish;
using System.Collections.Generic;

namespace _01.Scripts.Core.EventSystems
{
    public static class InventoryEvent
    {
        public static ChangeSlotEvent CreateChangeSlotEvent(int slotInstanceId, FishDataSO fishData, bool isAdd) =>
            new() { slotInstanceId = slotInstanceId, fishData = fishData, isAdd = isAdd };

        public static SellFishEvent CreateSellFishEvent() => new();

        public static StackFishEvent CreateStackFishEvent(FishDataSO fishData) =>
            new() { fishData = fishData };

        public static RequestInventoryRefreshEvent CreateRequestInventoryRefreshEvent() => new();

        public static InventoryUpdatedEvent CreateInventoryUpdatedEvent(List<FishDataSO> fishList) =>
            new() { fishList = fishList };
    }

    public class ChangeSlotEvent : GameEvent
    {
        public int slotInstanceId;
        public FishDataSO fishData;
        public bool isAdd;
    }

    public class SellFishEvent : GameEvent
    {
    }

    public class StackFishEvent : GameEvent
    {
        public FishDataSO fishData;
    }

    public class RequestInventoryRefreshEvent : GameEvent
    {
    }

    public class InventoryUpdatedEvent : GameEvent
    {
        public List<FishDataSO> fishList;
    }
}
