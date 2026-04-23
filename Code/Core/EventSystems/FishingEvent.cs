namespace _01.Scripts.Core.EventSystems
{
    public static class FishingEvent
    {
        public static LevelUpEvent CreateLevelUpEvent() => new();
        public static FishingCommandEvent CreateCommandEvent(FishingCommandType commandType) => new(commandType);
        public static FishingSignalEvent CreateSignalEvent(FishingSignalType signalType) => new(signalType);
    }

    public enum FishingCommandType
    {
        StartWaiting,
        StopWaiting,
        StartFishing,
        ReelGaugeUp,
        ReelGaugeDown,
        StopReeling
    }

    public enum FishingSignalType
    {
        HitFish,
        MissFish,
        RoundEnded
    }

    public class LevelUpEvent : GameEvent
    {
    }

    public class FishingCommandEvent : GameEvent
    {
        public readonly FishingCommandType commandType;

        public FishingCommandEvent(FishingCommandType commandType)
        {
            this.commandType = commandType;
        }
    }

    public class FishingSignalEvent : GameEvent
    {
        public readonly FishingSignalType signalType;

        public FishingSignalEvent(FishingSignalType signalType)
        {
            this.signalType = signalType;
        }
    }
}
