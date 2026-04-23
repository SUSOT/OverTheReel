namespace _01.Scripts.Player.States
{
    public class PlayerCanCastState : PlayerState
    {
        public PlayerCanCastState(Player player, int animationHash) : base(player, animationHash)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnCastModeEvent += HandleCastMode;
        }

        public override void Exit()
        {
            _player.PlayerInput.OnCastModeEvent -= HandleCastMode;
            base.Exit();
        }

        private void HandleCastMode()
        {
            _player.ChangeState(PlayerStateType.WaitCast);
        }
    }
}

