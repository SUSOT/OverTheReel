using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _01.Scripts.ETC
{
    [RequireComponent(typeof(PlayableDirector))]
    public class AutoPlayerBindingTimeline : MonoBehaviour
    {
        [SerializeField] private int trackIndex = 0;
        [SerializeField] private Animator playerAnimator;

        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            if (playerAnimator == null)
                return;

            if (_director.playableAsset is not TimelineAsset timeline)
            {
                Debug.LogWarning($"[{nameof(AutoPlayerBindingTimeline)}] PlayableDirector에 TimelineAsset이 설정되지 않았습니다.");
                return;
            }

            var outputs = timeline.outputs.ToList();
            if (trackIndex < 0 || trackIndex >= outputs.Count)
            {
                Debug.LogError($"[{nameof(AutoPlayerBindingTimeline)}] trackIndex {trackIndex}가 범위를 벗어났습니다 (0~{outputs.Count - 1}).");
                return;
            }

            var track = outputs[trackIndex].sourceObject;
            _director.SetGenericBinding(track, playerAnimator);
        }
    }
}
