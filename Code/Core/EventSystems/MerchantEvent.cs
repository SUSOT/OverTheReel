namespace _01.Scripts.Core.EventSystems
{
    public static class MerchantEvent
    {
        public static LevelUpFishingRod CreateLevelUpFishingRodEvent(int coin, string panelName) =>
            new() { coin = coin, panelName = panelName };

        public static LevelUpBoat CreateLevelUpBoatEvent() => new();
    }

    public class LevelUpFishingRod : GameEvent
    {
        public int coin;
        public string panelName;
    }

    public class LevelUpBoat : GameEvent
    {
    }
}
