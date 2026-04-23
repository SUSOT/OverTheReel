namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class MenuUIEvent
    {
        public static MenuUIOpenEvent CreateMenuUIOpenEvent(bool isOpen) =>
            new() { isOpen = isOpen };
    }

    public class MenuUIOpenEvent : GameEvent
    {
        public bool isOpen;
    }
}
