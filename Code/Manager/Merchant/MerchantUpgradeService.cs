using System.Collections.Generic;
using _01.Scripts.SO.Coin;

namespace _01.Scripts.Manager.Merchant
{
    public readonly struct BoatUpgradeResult
    {
        public readonly bool isSuccess;
        public readonly int newBoatLevel;
        public readonly int unlockedMapIndex;
        public readonly string nextBoatCostText;

        public BoatUpgradeResult(bool isSuccess, int newBoatLevel, int unlockedMapIndex, string nextBoatCostText)
        {
            this.isSuccess = isSuccess;
            this.newBoatLevel = newBoatLevel;
            this.unlockedMapIndex = unlockedMapIndex;
            this.nextBoatCostText = nextBoatCostText;
        }
    }

    public class MerchantUpgradeService
    {
        private readonly CoinDataMono _coinDataMono;
        private readonly List<int> _boatCosts;

        public MerchantUpgradeService(CoinDataMono coinDataMono, List<int> boatCosts)
        {
            _coinDataMono = coinDataMono;
            _boatCosts = boatCosts;
        }

        public bool TryUpgradeFishingRod(int requiredCoin)
        {
            if (_coinDataMono == null) return false;
            if (!_coinDataMono.CheckCanChangeCoinAmount(requiredCoin)) return false;

            _coinDataMono.ChangeCoinAmount(requiredCoin, false);
            return true;
        }

        public BoatUpgradeResult TryUpgradeBoat(int currentBoatLevel)
        {
            if (_coinDataMono == null)
            {
                return new BoatUpgradeResult(false, currentBoatLevel, -1, GetBoatCostText(currentBoatLevel));
            }

            if (!TryGetCurrentBoatCost(currentBoatLevel, out int currentBoatCost))
            {
                return new BoatUpgradeResult(false, currentBoatLevel, -1, "MAX");
            }

            if (!_coinDataMono.CheckCanChangeCoinAmount(currentBoatCost))
            {
                return new BoatUpgradeResult(false, currentBoatLevel, -1, GetBoatCostText(currentBoatLevel));
            }

            _coinDataMono.ChangeCoinAmount(currentBoatCost, false);

            int nextBoatLevel = currentBoatLevel + 1;
            return new BoatUpgradeResult(true, nextBoatLevel, nextBoatLevel + 1, GetBoatCostText(nextBoatLevel));
        }

        public string GetBoatCostText(int boatLevel)
        {
            return TryGetCurrentBoatCost(boatLevel, out int cost) ? cost.ToString() : "MAX";
        }

        private bool TryGetCurrentBoatCost(int boatLevel, out int boatCost)
        {
            boatCost = 0;

            if (_boatCosts == null || _boatCosts.Count == 0) return false;
            if (boatLevel < 0 || boatLevel >= _boatCosts.Count) return false;

            boatCost = _boatCosts[boatLevel];
            return boatLevel != _boatCosts.Count - 1;
        }
    }
}
