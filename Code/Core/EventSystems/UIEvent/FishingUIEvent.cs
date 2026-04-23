namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class FishingUIEvent
    {
        public static StartFishingEvent CreateStartFishingEvent() => new();
        public static EndFishingEvent CreateEndFishingEvent() => new();

        public static ChangeReelGaugeEvent CreateChangeReelGaugeEvent(float reelGauge) =>
            new() { reelGauge = reelGauge };

        public static ChangePerfectZoneEvent CreateChangePerfectZoneEvent(float minGauge, float maxGauge) =>
            new() { minGauge = minGauge, maxGauge = maxGauge };

        public static TogglePerfectTextEvent CreateTogglePerfectTextEvent(bool isActive) =>
            new() { isActive = isActive };

        public static ChangeFishHpEvent CreateChangeFishHpEvent(float hp) =>
            new() { hp = hp };

        public static StopReelingEvent CreateStopReelingEvent() => new();

        public static RollingReelEvent CreateRollingReelEvent(bool isRolling) =>
            new() { isRolling = isRolling };

        public static TimerDownEvent CreateTimerDownEvent(float time) =>
            new() { time = time };
    }

    public class StartFishingEvent : GameEvent
    {
    }

    public class EndFishingEvent : GameEvent
    {
    }

    public class ChangeReelGaugeEvent : GameEvent
    {
        public float reelGauge;
    }

    public class ChangePerfectZoneEvent : GameEvent
    {
        public float minGauge;
        public float maxGauge;
    }

    public class TogglePerfectTextEvent : GameEvent
    {
        public bool isActive;
    }
    
    public class ChangeFishHpEvent : GameEvent
    {
        public float hp;
    }
    
    public class StopReelingEvent : GameEvent
    {
    }
    
    public class RollingReelEvent : GameEvent
    {
        public bool isRolling;
    }

    public class TimerDownEvent : GameEvent
    {
        public float time;
    }
}
