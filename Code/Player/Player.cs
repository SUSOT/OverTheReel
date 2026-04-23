using System;
using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Player.Components;
using _01.Scripts.Player.FSM;
using _01.Scripts.Player.States;
using Settings.Input;
using UnityEngine;
using Marker = _01.Scripts.ETC.Marker;

namespace _01.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Input")]
        [field: SerializeField]
        public InputSO PlayerInput { get; private set; }

        [Header("State")] [SerializeField] private StateDataSO[] stateDataList;
        private PlayerStateMachine _stateMachine;

        [Header("Components")] public PlayerCameraController CameraController { get; private set; }
        public PlayerMovement MovementController { get; private set; }
        public PlayerAnimator PlayerAnimator { get; set; }
        public PlayerAnimatorTrigger PlayerAnimatorTrigger { get; set; }
        public Marker Marker { get; private set; }

        [Header("Boat")] [SerializeField] private List<GameObject> boats;

        [Header("Casting")]
        [field: SerializeField]
        public Transform AvatarTransform { get; private set; }

        [SerializeField] private LayerMask waterLayerMask;
        [SerializeField] private float castingDistance;

        [Header("Events")] [SerializeField] private GameEventChannelSO playerChannel;
        [SerializeField] private GameEventChannelSO castingUIChannel;
        [SerializeField] private GameEventChannelSO fishingChannel;

        private PlayerBoatRuntime _playerBoatRuntime;
        private PlayerCastingService _playerCastingService;

        public Vector3 CastingDirection { get; set; }
        public bool IsCastingClickRequired { get; set; }
        public event Action<Vector3> OnCastingSuccess;

        private void Awake()
        {
            CameraController = GetComponentInChildren<PlayerCameraController>(true);
            MovementController = GetComponentInChildren<PlayerMovement>();
            PlayerAnimator = GetComponentInChildren<PlayerAnimator>();
            PlayerAnimatorTrigger = GetComponentInChildren<PlayerAnimatorTrigger>(true);
            Marker = GetComponentInChildren<Marker>(true);

            _stateMachine = new PlayerStateMachine(this, stateDataList);
            _playerBoatRuntime = new PlayerBoatRuntime(boats, MovementController);
            _playerCastingService =
                new PlayerCastingService(AvatarTransform, waterLayerMask, castingDistance, castingUIChannel);

            PlayerInput.OnCastingEvent += HandleCastingInput;

            playerChannel.AddListener<PlayerInputEvent>(HandlePlayerInput);
            playerChannel.AddListener<ChangeBoatEvent>(HandleChangeBoat);
        }

        private void Start()
        {
            _stateMachine.ChangeState(PlayerStateType.Idle);
            _playerBoatRuntime.ApplySavedBoat();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void OnDestroy()
        {
            _stateMachine?.EndStateMachine();

            if (PlayerInput != null)
            {
                PlayerInput.OnCastingEvent -= HandleCastingInput;
            }

            playerChannel.RemoveListener<PlayerInputEvent>(HandlePlayerInput);
            playerChannel.RemoveListener<ChangeBoatEvent>(HandleChangeBoat);
        }

        public void ChangeState(PlayerStateType newState)
        {
            _stateMachine.ChangeState(newState);
        }

        public void AddFishingSignalListener(Action<FishingSignalEvent> listener)
        {
            fishingChannel?.AddListener(listener);
        }

        public void RemoveFishingSignalListener(Action<FishingSignalEvent> listener)
        {
            fishingChannel?.RemoveListener(listener);
        }

        public void RaiseFishingCommand(FishingCommandType commandType)
        {
            fishingChannel?.RaiseEvent(FishingEvent.CreateCommandEvent(commandType));
        }

        private void HandleChangeBoat(ChangeBoatEvent evt)
        {
            if (_playerBoatRuntime.TryApplyBoatChange(evt.index)) return;
            Debug.LogWarning($"Player: Invalid boat index '{evt.index}'.");
        }

        private void HandlePlayerInput(PlayerInputEvent evt)
        {
            if (PlayerInput == null) return;
            PlayerInput.SetPlayerInput(evt.isCanInput);
        }

        private void HandleCastingInput(Vector3 castingPos)
        {
            _playerCastingService.TryCast(castingPos, IsCastingClickRequired, RaiseCastingSuccess);
        }

        private void RaiseCastingSuccess(Vector3 castingPos)
        {
            OnCastingSuccess?.Invoke(castingPos);
        }
    }
}