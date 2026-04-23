using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class PictorialBookUI : MenuPanel
    {
        private const float TweenDuration = 0.5f;

        [SerializeField] private Image uiPanel;

        [SerializeField] private List<GameObject> pictorials;

        private int _pictorialIndex;


        private void Awake()
        {
            uiPanel.transform.localScale = Vector3.zero;
        }

        public override void ShowUI()
        {
            base.ShowUI();
            if (pictorials == null || pictorials.Count == 0) return;

            uiPanel.gameObject.SetActive(true);
            _pictorialIndex = 0;
            SetActivePictorial(_pictorialIndex);
            uiPanel.transform.DOScale(Vector3.one, TweenDuration).SetEase(Ease.OutBack);
        }

        public override void CloseUI()
        {
            base.CloseUI();

            uiPanel.transform.DOScale(Vector3.zero, TweenDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                foreach (var pictorial in pictorials)
                {
                    pictorial.SetActive(false);
                }
                uiPanel.gameObject.SetActive(false);
            });
        }

        public void ChangePictorialUI(bool isRight)
        {
            if (pictorials == null || pictorials.Count == 0) return;

            SetActivePictorial(_pictorialIndex, false);
            _pictorialIndex += isRight ? 1 : -1;

            if (_pictorialIndex >= pictorials.Count)
            {
                _pictorialIndex = 0;
            }
            else if (_pictorialIndex < 0)
            {
                _pictorialIndex = pictorials.Count - 1;
            }

            SetActivePictorial(_pictorialIndex);
        }

        private void SetActivePictorial(int index, bool isActive = true)
        {
            if (index < 0 || index >= pictorials.Count || pictorials[index] == null) return;
            pictorials[index].SetActive(isActive);
        }
    }
}
