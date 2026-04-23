namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class MerchantUIEvent
    {
        public static UpdateTotalCostEvent CreateUpdateTotalCostEvent(int totalCost) =>
            new() { totalCost = totalCost };

        public static ChangeBoatCostEvent CreateChangeBoatCostEvent(string boatCostText) =>
            new() { boatCostText = boatCostText };

        public static SetUpgradePanelStateEvent CreateSetUpgradePanelStateEvent(string panelName, bool isActive) =>
            new() { panelName = panelName, isActive = isActive };
    }

    public class UpdateTotalCostEvent : GameEvent
    {
        public int totalCost;
    }

    public class ChangeBoatCostEvent : GameEvent
    {
        public string boatCostText;
    }

    public class SetUpgradePanelStateEvent : GameEvent
    {
        public string panelName;
        public bool isActive;
    }
}
