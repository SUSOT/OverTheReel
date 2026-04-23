namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class ChangeSceneEvent
    {
        public static FadeAndChangeEvent CreateFadeAndChangeEvent(int alpha, float duration, int mapIndex) =>
            new() { alpha = alpha, duration = duration, mapIndex = mapIndex };
    }

    public class FadeAndChangeEvent : GameEvent
    {
        public int alpha;
        public float duration;
        public int mapIndex;
    }
}
