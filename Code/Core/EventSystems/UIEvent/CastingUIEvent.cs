namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class CastingUIEvent
    {
        public static CastingFailEvent CreateCastingFailEvent(CastingFailEventType castingFailEventType) =>
            new() { castingFailEventType = castingFailEventType };
    }

    public class CastingFailEvent : GameEvent
    {
        public CastingFailEventType castingFailEventType;
    }

    public enum CastingFailEventType
    {
        NotWater,
        NearRange
    }
}
