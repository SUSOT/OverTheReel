namespace _01.Scripts.Core.EventSystems
{
    public static class CoinEvent
    {
        public static CoinChangeEvent CreateCoinChangeEvent(int currentAmount, int changeAmount) =>
            new() { currentAmount = currentAmount, changeAmount = changeAmount };
    }

    public class CoinChangeEvent : GameEvent
    {
        public int currentAmount;
        public int changeAmount;
    }
}
