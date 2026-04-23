using System;
using System.Collections.Generic;
using System.Linq;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class CastingUI : MonoBehaviour
    {
        [SerializeField] private GameObject castingFailUI;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float scaleUpDuration = 0.3f;
        [SerializeField] private float scaleDownDuration = 0.2f;
        [SerializeField] private float showDuration = 1f;
        [SerializeField] private List<CustomDict> _texts;
        private Dictionary<CastingFailEventType, string> _textDict;

        private Vector3 _originalScale = Vector3.zero;
        private Sequence _currentSequence;
        
        [SerializeField] private GameEventChannelSO castingUIChannel;
        
        private void Awake()
        {
            BuildTextDictionary();

            _originalScale = castingFailUI.transform.localScale;
            castingFailUI.transform.localScale = Vector3.zero;
            castingFailUI.SetActive(false);
            
            castingUIChannel.AddListener<CastingFailEvent>(ShowCastingFailUI);
        }

        private void OnDestroy()
        {
            castingUIChannel.RemoveListener<CastingFailEvent>(ShowCastingFailUI);
            _currentSequence?.Kill();
        }
        
        private void OnValidate()
        {
            BuildTextDictionary();
        }

        private void ShowCastingFailUI(CastingFailEvent evt)
        {
            if (_currentSequence != null && _currentSequence.IsActive())
                _currentSequence.Kill();

            if (!_textDict.TryGetValue(evt.castingFailEventType, out string failText))
            {
                failText = evt.castingFailEventType.ToString();
            }

            text.SetText(failText);

            castingFailUI.SetActive(true);

            _currentSequence = DOTween.Sequence()
                .Append(castingFailUI.transform.DOScale(_originalScale, scaleUpDuration)
                    .SetEase(Ease.OutBack))
                .AppendInterval(showDuration)
                .Append(castingFailUI.transform.DOScale(Vector3.zero, scaleDownDuration)
                    .SetEase(Ease.InBack))
                .OnComplete(() => castingFailUI.SetActive(false));
        }
        
        [Serializable]
        public struct CustomDict
        {
            public CastingFailEventType Key;
            public string Value;
        }

        private void BuildTextDictionary()
        {
            _textDict = _texts == null
                ? new Dictionary<CastingFailEventType, string>()
                : _texts.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}

