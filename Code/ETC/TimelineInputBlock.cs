using _01.Scripts.Core.EventSystems;
using UnityEngine;
using UnityEngine.Playables;

namespace _01.Scripts.ETC
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineInputBlock : MonoBehaviour
    {
        private PlayableDirector _director;

        [SerializeField] private GameEventChannelSO playerChannel;
        
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _director.stopped += OnTimelineStopped;
        }

        private void OnTimelineStopped(PlayableDirector pd)
        {
            SendEvent(true);
        }

        private void SendEvent(bool isCanInput)
        {
            playerChannel.RaiseEvent(PlayerEvent.CreatePlayerInputEvent(isCanInput));
        }
    }
}
