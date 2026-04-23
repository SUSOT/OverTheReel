using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Manager.MapFlow;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private float waitFadeTime = 3f;
        [SerializeField] private GameEventChannelSO mapChannel;
        [SerializeField] private GameEventChannelSO changeSceneChannel;

        private MapStateService _mapStateService;

        private void Awake()
        {
            _mapStateService = new MapStateService();
            _mapStateService.Load();

            mapChannel.AddListener<UnlockMapEvent>(HandleUnlockMap);
            mapChannel.AddListener<ChangeMapEvent>(HandleChangeMap);
            mapChannel.AddListener<RequestMapStatesEvent>(HandleRequestMapStates);
        }

        private void Start()
        {
            PublishMapStates();
        }

        private void OnDestroy()
        {
            mapChannel.RemoveListener<UnlockMapEvent>(HandleUnlockMap);
            mapChannel.RemoveListener<ChangeMapEvent>(HandleChangeMap);
            mapChannel.RemoveListener<RequestMapStatesEvent>(HandleRequestMapStates);
        }

        private void HandleUnlockMap(UnlockMapEvent evt)
        {
            if (!_mapStateService.TryUnlock(evt.mapIndex)) return;
            PublishMapStates();
        }

        private void HandleChangeMap(ChangeMapEvent evt)
        {
            if (!_mapStateService.TryChangeMap(evt.mapIndex, evt.firstLoading, out int changedMapIndex)) return;

            changeSceneChannel.RaiseEvent(ChangeSceneEvent.CreateFadeAndChangeEvent(1, waitFadeTime, changedMapIndex));
        }

        private void HandleRequestMapStates(RequestMapStatesEvent evt)
        {
            PublishMapStates();
        }

        private void PublishMapStates()
        {
            MapStateSnapshot snapshot = _mapStateService.GetSnapshot();
            mapChannel.RaiseEvent(MapEvent.CreateMapStatesUpdatedEvent(snapshot.unlockedMapIndices));
        }
    }
}
