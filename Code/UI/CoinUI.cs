using _01.Scripts.Core.EventSystems;
using TMPro;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class CoinUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        
        [SerializeField] private GameEventChannelSO coinChannel;
        
        private void Awake()
        {
            coinChannel.AddListener<CoinChangeEvent>(SetCoinText);
        }

        private void OnDestroy()
        {
            coinChannel.RemoveListener<CoinChangeEvent>(SetCoinText);
        }

        private void SetCoinText(CoinChangeEvent evt)
        {
            coinText.SetText(evt.currentAmount.ToString());
        }
    }
}

