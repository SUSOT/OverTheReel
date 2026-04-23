using _01.Scripts.Core.EventSystems;
using UnityEngine;

namespace _01.Scripts.SO.Coin
{
    [CreateAssetMenu(fileName = "CoinData", menuName = "SO/GameData/CoinData", order = 0)]
    public class CoinDataSO : ScriptableObject
    {
        [Header("Coin Settings")]
        [SerializeField] private int coinAmount;

        [Header("Event Channel")]
        [SerializeField] private GameEventChannelSO CoinChannel;

        public int CoinAmount => coinAmount;

        public void ChangeCoinAmount(int amount, bool isPlus, bool isInitialize = false)
        {
            if (isInitialize)
            {
                coinAmount = amount;
                SendCoinEvent(coinAmount);
                return;
            }

            coinAmount = isPlus ? coinAmount + amount : coinAmount - amount;
            SendCoinEvent(coinAmount);
        }

        public bool CheckCanChangeCoinAmount(int amount)
        {
            return coinAmount >= amount;
        }

        private void SendCoinEvent(int changeAmount)
        {
            CoinChannel.RaiseEvent(CoinEvent.CreateCoinChangeEvent(coinAmount, changeAmount));
        }
    }
}
