using System.Collections.Generic;

namespace _01.Scripts.Core.EventSystems
{
    public static class MapEvent
    {
        public static UnlockMapEvent CreateUnlockMapEvent(int mapIndex) =>
            new() { mapIndex = mapIndex };

        public static ChangeMapEvent CreateChangeMapEvent(int mapIndex, bool firstLoading) =>
            new() { mapIndex = mapIndex, firstLoading = firstLoading };

        public static RequestMapStatesEvent CreateRequestMapStatesEvent() => new();

        public static MapStatesUpdatedEvent CreateMapStatesUpdatedEvent(List<int> unlockedMapIndices) =>
            new() { unlockedMapIndices = unlockedMapIndices };
    }

    public class UnlockMapEvent : GameEvent
    {
        public int mapIndex;
    }

    public class ChangeMapEvent : GameEvent
    {
        public int mapIndex;
        public bool firstLoading;
    }

    public class RequestMapStatesEvent : GameEvent
    {
    }

    public class MapStatesUpdatedEvent : GameEvent
    {
        public List<int> unlockedMapIndices;
    }
}
