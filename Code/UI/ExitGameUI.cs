using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class ExitGameUI : MenuPanel
    {
        private const float TweenDuration = 0.5f;

        [SerializeField] private Image uiPanel;

        private void Awake()
        {
            uiPanel.transform.localScale = Vector3.zero;
        }

        public override void ShowUI()
        {
            base.ShowUI();
            uiPanel.gameObject.SetActive(true);
            uiPanel.transform.DOKill();
            uiPanel.transform.DOScale(Vector3.one, TweenDuration).SetEase(Ease.OutBack);
        }

        public override void CloseUI()
        {
            base.CloseUI();
            uiPanel.transform.DOKill();
            uiPanel.transform.DOScale(Vector3.zero, TweenDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                uiPanel.gameObject.SetActive(false);
            });
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
