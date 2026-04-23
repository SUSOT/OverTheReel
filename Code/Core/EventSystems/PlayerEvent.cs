namespace _01.Scripts.Core.EventSystems
{
    public static class PlayerEvent
    {
        public static PlayerInputEvent CreatePlayerInputEvent(bool isCanInput) =>
            new() { isCanInput = isCanInput };

        public static ChangeBoatEvent CreateChangeBoatEvent(int index) =>
            new() { index = index };
    }

    public class PlayerInputEvent : GameEvent
    {
        public bool isCanInput;
    }

    public class ChangeBoatEvent : GameEvent
    {
        public int index;
    }
}
