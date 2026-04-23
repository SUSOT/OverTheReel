using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Sounds;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class ResultUI : MonoBehaviour
    {
        private const float ShowDuration = 0.5f;
        private const float SlideDelay = 1f;
        private const float FishSlideOffset = 450f;
        private const float PanelSlideOffset = 1100f;

        [SerializeField] private GameObject blackBoard;
        [SerializeField] private Image fishImage;
        [SerializeField] private Image panel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image backTierImage;
        [SerializeField] private TextMeshProUGUI tierText;
        [SerializeField] private TextMeshProUGUI sizeText;
        [SerializeField] private GameObject bestRecord;

        private float _referenceSize = 200f;

        private RectTransform _rectTransform;
        private bool _isFlip;

        private Vector3 _originalFishPos;
        private Vector3 _originalPanelPos;
        
        [SerializeField] private GameEventChannelSO resultUiChannel;
        [SerializeField] private GameEventChannelSO playerChannel;
        [SerializeField] private GameEventChannelSO soundChannel;
        
        [SerializeField] private SoundSO resultSfx;

        private void Awake()
        {
            _rectTransform = fishImage.GetComponent<RectTransform>();
            resultUiChannel.AddListener<ShowResultUIEvent>(ShowResultUI);
        }

        private void OnDestroy()
        {
            resultUiChannel.RemoveListener<ShowResultUIEvent>(ShowResultUI);
        }

        private void ShowResultUI(ShowResultUIEvent evt)
        {
            soundChannel.RaiseEvent(SoundEvent.CreatePlaySfxEvent(transform.position, resultSfx));

            SetPlayerInput(false);

            blackBoard.SetActive(true);
            _referenceSize = evt.spriteSize;

            _originalFishPos = _rectTransform.anchoredPosition;
            _originalPanelPos = panel.rectTransform.anchoredPosition;

            _rectTransform.DOKill();
            panel.rectTransform.DOKill();
            _rectTransform.localScale = Vector3.zero;
            bestRecord.SetActive(evt.isBestRecord);

            ChangeInfo(evt.newSprite, evt.fishName, evt.tierTxt, evt.backTierImageColor, evt.size, evt.isFlip);

            _rectTransform.DOScale(_isFlip ? new Vector3(-1f, 1f, 1f) : Vector3.one, ShowDuration).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(SlideDelay, () =>
                    {
                        _rectTransform.DOAnchorPosX(_originalFishPos.x - FishSlideOffset, ShowDuration).SetEase(Ease.OutBack);
                        panel.rectTransform.DOAnchorPosX(_originalPanelPos.x - PanelSlideOffset, ShowDuration)
                            .SetEase(Ease.OutBack);
                    });
                });
        }

        #region ChangeFishInfo

        private void ChangeInfo(Sprite newSprite, string fishName, string tierTxt, Color backTierImageColor, float size, bool flip)
        {
            if (newSprite == null) return;

            fishImage.sprite = newSprite;

            nameText.SetText(fishName);
            tierText.SetText(tierTxt);
            backTierImage.color = backTierImageColor;
            sizeText.SetText(size + "cm");
            _isFlip = flip;

            AdjustSize();
        }

        private void AdjustSize()
        {
            float spriteWidth = fishImage.sprite.rect.width;
            float spriteHeight = fishImage.sprite.rect.height;
            float aspectRatio = spriteWidth / spriteHeight;

            if (spriteWidth > spriteHeight)
            {
                _rectTransform.sizeDelta = new Vector2(
                    _referenceSize,
                    _referenceSize / aspectRatio
                );
            }
            else
            {
                _rectTransform.sizeDelta = new Vector2(
                    _referenceSize * aspectRatio,
                    _referenceSize
                );
            }
        }

        #endregion

        public void CloseResultUI()
        {
            _rectTransform.DOKill();
            panel.rectTransform.DOKill();

            _rectTransform.DOScale(Vector3.zero, ShowDuration).SetEase(Ease.InBack);
            panel.transform.DOScale(Vector3.zero, ShowDuration).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _rectTransform.anchoredPosition = _originalFishPos;
                    panel.rectTransform.anchoredPosition = _originalPanelPos;

                    _rectTransform.localScale = Vector3.one;
                    panel.transform.localScale = Vector3.one;
                    blackBoard.SetActive(false);
                });

            SetPlayerInput(true);
        }

        private void SetPlayerInput(bool canInput)
        {
            playerChannel.RaiseEvent(PlayerEvent.CreatePlayerInputEvent(canInput));
        }
    }
}

