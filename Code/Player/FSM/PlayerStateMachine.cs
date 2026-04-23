using _01.Scripts.Player.States;
using UnityEngine;

namespace _01.Scripts.Player.FSM
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }

        private readonly PlayerState _idleState;
        private readonly PlayerState _moveState;
        private readonly PlayerState _waitCastState;
        private readonly PlayerState _castingState;
        private readonly PlayerState _fishingState;
        private readonly PlayerState _cancelCastState;

        public PlayerStateMachine(Player player, StateDataSO[] stateList)
        {
            _idleState = new PlayerIdleState(player, GetAnimationHash(stateList, PlayerStateType.Idle));
            _moveState = new PlayerMoveState(player, GetAnimationHash(stateList, PlayerStateType.Move));
            _waitCastState = new PlayerWaitCastState(player, GetAnimationHash(stateList, PlayerStateType.WaitCast));
            _castingState = new PlayerCastingState(player, GetAnimationHash(stateList, PlayerStateType.Casting));
            _fishingState = new PlayerFishingState(player, GetAnimationHash(stateList, PlayerStateType.Fishing));
            _cancelCastState =
                new PlayerCancelCastState(player, GetAnimationHash(stateList, PlayerStateType.CancelCast));
        }

        public void ChangeState(PlayerStateType newStateType)
        {
            var newState = GetState(newStateType);
            if (newState == null)
            {
                Debug.LogError($"PlayerStateMachine: State is null '{newStateType}'.");
                return;
            }

            if (CurrentState == newState) return;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        public void EndStateMachine()
        {
            if (CurrentState == null) return;

            CurrentState.Exit();
            CurrentState = null;
        }

        private PlayerState GetState(PlayerStateType stateType)
        {
            switch (stateType)
            {
                case PlayerStateType.Idle:
                    return _idleState;
                case PlayerStateType.Move:
                    return _moveState;
                case PlayerStateType.WaitCast:
                    return _waitCastState;
                case PlayerStateType.Casting:
                    return _castingState;
                case PlayerStateType.Fishing:
                    return _fishingState;
                case PlayerStateType.CancelCast:
                    return _cancelCastState;
                default:
                    return null;
            }
        }

        private int GetAnimationHash(StateDataSO[] stateList, PlayerStateType stateType)
        {
            string stateName = GetStateName(stateType);
            int fallbackHash = Animator.StringToHash(stateName);

            if (stateList == null || stateList.Length == 0) return fallbackHash;

            foreach (var stateData in stateList)
            {
                if (stateData == null || string.IsNullOrWhiteSpace(stateData.stateName)) continue;
                if (!string.Equals(stateData.stateName.Trim(), stateName,
                        System.StringComparison.OrdinalIgnoreCase)) continue;

                return stateData.animationHash;
            }

            Debug.LogWarning($"PlayerStateMachine: animation hash not found for '{stateName}'.");
            return fallbackHash;
        }

        private string GetStateName(PlayerStateType stateType)
        {
            switch (stateType)
            {
                case PlayerStateType.Idle:
                    return "IDLE";
                case PlayerStateType.Move:
                    return "MOVE";
                case PlayerStateType.WaitCast:
                    return "WAITCAST";
                case PlayerStateType.Casting:
                    return "CASTING";
                case PlayerStateType.Fishing:
                    return "FISHING";
                case PlayerStateType.CancelCast:
                    return "CANCELCAST";
                default:
                    return string.Empty;
            }
        }
    }
}
