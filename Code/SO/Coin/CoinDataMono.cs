using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using UnityEngine;

namespace _01.Scripts.SO.Coin
{
    public class CoinDataMono : MonoBehaviour
    {
        [SerializeField] private CoinDataSO coinDataSO;

        private void Start()
        {
            GameSaveData data = SaveDataSystem.Load();
            coinDataSO.ChangeCoinAmount(data.coin, true, true);
        }

        public void ChangeCoinAmount(int amount, bool isPlus)
        {
            coinDataSO.ChangeCoinAmount(amount, isPlus);
            GameSaveData data = SaveDataSystem.Load();
            data.coin = coinDataSO.CoinAmount;
            SaveDataSystem.Save(data);
        }

        public bool CheckCanChangeCoinAmount(int amount)
        {
            return coinDataSO.CheckCanChangeCoinAmount(amount);
        }
    }
}
