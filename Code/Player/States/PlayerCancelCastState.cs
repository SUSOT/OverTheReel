using DG.Tweening;
using _01.Scripts.Player.FSM;

namespace _01.Scripts.Player.States
{
    public class PlayerCancelCastState : PlayerState
    {
        public PlayerCancelCastState(Player player, int animationHash) : base(player, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.Marker.HookingMarker(_player.Marker.MarkerPos.position, 2f, 0.5f).SetEase(Ease.InQuart);
            _player.PlayerAnimatorTrigger.OnAnimationEndTrigger += HandleAnimationEnd;
        }

        public override void Exit()
        {
            _player.PlayerAnimatorTrigger.OnAnimationEndTrigger -= HandleAnimationEnd;
            base.Exit();
        }

        private void HandleAnimationEnd()
        {
            _player.Marker.ActiveMarker(false);
            _player.ChangeState(PlayerStateType.WaitCast);
        }
    }
}




