using DG.Tweening;
using _01.Scripts.Player.FSM;
using UnityEngine;

namespace _01.Scripts.Player.States
{
    public class PlayerWaitCastState : PlayerState
    {
        public PlayerWaitCastState(Player player, int animationHash) : base(player, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnCastModeEvent += HandleCastMode;
            _player.OnCastingSuccess += HandleCasting;

            _player.PlayerInput.SetCanMove(false);
            _player.CameraController.ChangeCamera(false);
            _player.MovementController.StopImmediately();
            _player.IsCastingClickRequired = true;
        }


        public override void Exit()
        {
            _player.PlayerInput.OnCastModeEvent -= HandleCastMode;
            _player.OnCastingSuccess -= HandleCasting;
            _player.IsCastingClickRequired = false;
            base.Exit();
        }


        private void HandleCasting(Vector3 castingDirection)
        {
            _player.CastingDirection = castingDirection;

            _player.AvatarTransform.DOLookAt(castingDirection, 0.5f);
            _player.ChangeState(PlayerStateType.Casting);
        }

        private void HandleCastMode()
        {
            _player.CameraController.ChangeCamera(true);
            _player.PlayerInput.SetCanMove(true);
            _player.ChangeState(PlayerStateType.Idle);
        }
    }
}


