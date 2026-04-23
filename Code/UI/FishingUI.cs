using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class FishingUI : MonoBehaviour
    {
        private const float GaugePercentDivisor = 100f;
        private const string PerfectZoneObjectName = "Zone";
        private const string PerfectTextObjectName = "PerfectText";
        private const float DefaultPerfectZoneMinGauge = 80f;
        private const float DefaultPerfectZoneMaxGauge = 95f;

        [SerializeField] private GameObject fishingUI;
        [SerializeField] private float scaleUpDuration = 0.3f;

        private Vector3 _originalScale = Vector3.zero;
        [SerializeField] private Image fishingGauge;
        [SerializeField] private GameObject reel;

        [SerializeField] private RectTransform timerIcon;
        [SerializeField] private RectTransform endTimerTrm;
        private Vector2 _initialTimerIconPos;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private RectTransform perfectZoneRect;
        [SerializeField] private TextMeshProUGUI perfectText;

        private RectTransform _gaugeRect;
        private RectTransform _perfectZoneParentRect;
        private float _gaugeLeftInParentX;
        private float _gaugeRightInParentX;
        private bool _hasGaugeBounds;

        [SerializeField] private GameEventChannelSO fishingUiChannel;

        private void Awake()
        {
            _originalScale = fishingUI.transform.localScale;
            fishingUI.transform.localScale = Vector3.zero;
            _initialTimerIconPos = timerIcon.anchoredPosition;
            fishingUI.SetActive(false);
            TryInitializePerfectZoneReference();
            TryInitializePerfectTextReference();

            fishingUiChannel.AddListener<TimerDownEvent>(TimerDown);
            fishingUiChannel.AddListener<RollingReelEvent>(RollingReel);
            fishingUiChannel.AddListener<StopReelingEvent>(StopReeling);
            fishingUiChannel.AddListener<TogglePerfectTextEvent>(TogglePerfectText);
            fishingUiChannel.AddListener<ChangeFishHpEvent>(ChangeFishHp);
            fishingUiChannel.AddListener<ChangePerfectZoneEvent>(ChangePerfectZone);
            fishingUiChannel.AddListener<ChangeReelGaugeEvent>(ChangeReelGauge);
            fishingUiChannel.AddListener<StartFishingEvent>(ShowFishingUI);
            fishingUiChannel.AddListener<EndFishingEvent>(HideFishingUI);
        }

        private void OnDestroy()
        {
            fishingUiChannel.RemoveListener<TimerDownEvent>(TimerDown);
            fishingUiChannel.RemoveListener<RollingReelEvent>(RollingReel);
            fishingUiChannel.RemoveListener<StopReelingEvent>(StopReeling);
            fishingUiChannel.RemoveListener<TogglePerfectTextEvent>(TogglePerfectText);
            fishingUiChannel.RemoveListener<ChangeFishHpEvent>(ChangeFishHp);
            fishingUiChannel.RemoveListener<ChangePerfectZoneEvent>(ChangePerfectZone);
            fishingUiChannel.RemoveListener<ChangeReelGaugeEvent>(ChangeReelGauge);
            fishingUiChannel.RemoveListener<StartFishingEvent>(ShowFishingUI);
            fishingUiChannel.RemoveListener<EndFishingEvent>(HideFishingUI);
        }

        private void ShowFishingUI(StartFishingEvent evt)
        {
            fishingUI.transform.DOKill();
            fishingUI.SetActive(true);
            timerIcon.anchoredPosition = _initialTimerIconPos;
            SetPerfectTextVisible(false);
            fishingUI.transform.DOScale(_originalScale, scaleUpDuration)
                .SetEase(Ease.OutBack);
        }

        private void HideFishingUI(EndFishingEvent evt)
        {
            timerIcon.DOKill();
            fishingUI.transform.DOKill();
            hpSlider.value = hpSlider.maxValue;
            ResetPerfectZoneUI();
            SetPerfectTextVisible(false);
            fishingUI.transform.DOScale(Vector3.zero, scaleUpDuration)
                .SetEase(Ease.OutBack).OnComplete(() => { fishingUI.SetActive(false); });
        }

        private void ChangeReelGauge(ChangeReelGaugeEvent evt)
        {
            fishingGauge.fillAmount = evt.reelGauge / GaugePercentDivisor;
        }

        private void TimerDown(TimerDownEvent evt)
        {
            timerIcon.DOKill();
            timerIcon.DOAnchorPosX(endTimerTrm.anchoredPosition.x, evt.time).SetEase(Ease.Linear);
        }

        private void ChangeFishHp(ChangeFishHpEvent evt)
        {
            hpSlider.value = evt.hp;
        }

        private void TogglePerfectText(TogglePerfectTextEvent evt)
        {
            SetPerfectTextVisible(evt.isActive);
        }

        private void ChangePerfectZone(ChangePerfectZoneEvent evt)
        {
            if (!TryEnsurePerfectZoneMappingReady()) return;

            ApplyPerfectZone(evt.minGauge, evt.maxGauge);
        }

        private void RollingReel(RollingReelEvent evt)
        {
            float rotationSpeed = evt.isRolling ? 0.7f : 0.3f;
            float rotationAngle = evt.isRolling ? -360f : 360f;

            reel.transform.DOKill();
            reel.transform.DOLocalRotate(
                    new Vector3(0, 0, rotationAngle),
                    rotationSpeed,
                    RotateMode.FastBeyond360
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        private void StopReeling(StopReelingEvent evt)
        {
            reel.transform.DOKill();
        }

        private void TryInitializePerfectZoneReference()
        {
            if (perfectZoneRect == null)
            {
                perfectZoneRect = FindRectTransformByName(fishingUI.transform, PerfectZoneObjectName);
            }

            _gaugeRect = fishingGauge != null ? fishingGauge.rectTransform : null;
            _perfectZoneParentRect = perfectZoneRect != null ? perfectZoneRect.parent as RectTransform : null;
            _hasGaugeBounds = TryCacheGaugeBoundsInZoneParent();
        }

        private void ResetPerfectZoneUI()
        {
            if (!TryEnsurePerfectZoneMappingReady()) return;

            ApplyPerfectZone(DefaultPerfectZoneMinGauge, DefaultPerfectZoneMaxGauge);
        }

        private void TryInitializePerfectTextReference()
        {
            if (perfectText == null)
            {
                RectTransform perfectTextRect = FindRectTransformByName(fishingUI.transform, PerfectTextObjectName);
                if (perfectTextRect != null)
                {
                    perfectText = perfectTextRect.GetComponent<TextMeshProUGUI>();
                }
            }

            SetPerfectTextVisible(false);
        }

        private void SetPerfectTextVisible(bool isVisible)
        {
            if (perfectText == null) return;
            if (perfectText.gameObject.activeSelf == isVisible) return;

            perfectText.gameObject.SetActive(isVisible);
        }

        private bool TryEnsurePerfectZoneMappingReady()
        {
            if (perfectZoneRect == null) return false;

            if (_gaugeRect == null && fishingGauge != null)
            {
                _gaugeRect = fishingGauge.rectTransform;
            }

            if (_perfectZoneParentRect == null)
            {
                _perfectZoneParentRect = perfectZoneRect.parent as RectTransform;
            }

            if (_hasGaugeBounds) return true;
            _hasGaugeBounds = TryCacheGaugeBoundsInZoneParent();
            return _hasGaugeBounds;
        }

        private bool TryCacheGaugeBoundsInZoneParent()
        {
            if (perfectZoneRect == null || _gaugeRect == null || _perfectZoneParentRect == null) return false;

            Vector3[] gaugeWorldCorners = new Vector3[4];
            _gaugeRect.GetWorldCorners(gaugeWorldCorners);

            Vector3 leftPoint = _perfectZoneParentRect.InverseTransformPoint(gaugeWorldCorners[0]);
            Vector3 rightPoint = _perfectZoneParentRect.InverseTransformPoint(gaugeWorldCorners[3]);
            if (rightPoint.x <= leftPoint.x) return false;

            _gaugeLeftInParentX = leftPoint.x;
            _gaugeRightInParentX = rightPoint.x;
            return true;
        }

        private void ApplyPerfectZone(float minGauge, float maxGauge)
        {
            float clampedMin = Mathf.Clamp(minGauge, 0f, GaugePercentDivisor);
            float clampedMax = Mathf.Clamp(maxGauge, clampedMin, GaugePercentDivisor);
            float zoneWidthGauge = clampedMax - clampedMin;
            float zoneCenterGauge = clampedMin + zoneWidthGauge * 0.5f;

            float gaugeWidthInParent = _gaugeRightInParentX - _gaugeLeftInParentX;
            float centerNormalized = zoneCenterGauge / GaugePercentDivisor;
            float widthNormalized = zoneWidthGauge / GaugePercentDivisor;
            float targetX = Mathf.Lerp(_gaugeLeftInParentX, _gaugeRightInParentX, centerNormalized);

            Vector2 anchoredPosition = perfectZoneRect.anchoredPosition;
            anchoredPosition.x = targetX;
            perfectZoneRect.anchoredPosition = anchoredPosition;

            Vector2 sizeDelta = perfectZoneRect.sizeDelta;
            sizeDelta.x = gaugeWidthInParent * widthNormalized;
            perfectZoneRect.sizeDelta = sizeDelta;
        }

        private static RectTransform FindRectTransformByName(Transform root, string targetName)
        {
            if (root == null || string.IsNullOrEmpty(targetName)) return null;

            RectTransform[] allRectTransforms = root.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rectTransform in allRectTransforms)
            {
                if (rectTransform.name == targetName)
                {
                    return rectTransform;
                }
            }

            return null;
        }
    }
}
