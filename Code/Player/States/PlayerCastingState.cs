using _01.Scripts.Core.EventSystems;
using _01.Scripts.Player.FSM;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.Player.States
{
    public class PlayerCastingState : PlayerState
    {
        private bool _isCastingEnd;
        private bool _isHitFish;

        public PlayerCastingState(Player player, int animationHash) : base(player, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _isCastingEnd = false;
            _isHitFish = false;
            _player.PlayerAnimatorTrigger.OnCastingAnimationTrigger += HandleCastingAnimation;
            _player.PlayerInput.OnCastingEvent += HandleCancelCast;
            _player.PlayerInput.OnCastingEvent += HandleHitCast;
            _player.AddFishingSignalListener(HandleFishingSignal);
        }

        public override void Exit()
        {
            _player.PlayerAnimatorTrigger.OnCastingAnimationTrigger -= HandleCastingAnimation;
            _player.PlayerInput.OnCastingEvent -= HandleCancelCast;
            _player.PlayerInput.OnCastingEvent -= HandleHitCast;
            _player.RemoveFishingSignalListener(HandleFishingSignal);
            base.Exit();
        }

        private void HandleHitCast(Vector3 inputValue)
        {
            if (_isHitFish)
            {
                _player.ChangeState(PlayerStateType.Fishing);
            }
        }

        private void HandleFishingSignal(FishingSignalEvent evt)
        {
            switch (evt.signalType)
            {
                case FishingSignalType.HitFish:
                    _isHitFish = true;
                    _player.Marker.HitFishMarker();
                    break;
                case FishingSignalType.MissFish:
                    _isHitFish = false;
                    _player.ChangeState(PlayerStateType.CancelCast);
                    break;
            }
        }

        private void HandleCancelCast(Vector3 inputValue)
        {
            if (_isCastingEnd && !_isHitFish)
            {
                _player.RaiseFishingCommand(FishingCommandType.StopWaiting);
                _player.ChangeState(PlayerStateType.CancelCast);
            }
        }

        private void HandleCastingAnimation()
        {
            _player.Marker.ActiveMarker(true);

            _player.Marker.HookingMarker(_player.CastingDirection, 3f, 1.5f)
                .OnComplete(() =>
                {
                    _isCastingEnd = true;
                    _player.RaiseFishingCommand(FishingCommandType.StartWaiting);
                });
        }
    }
}




