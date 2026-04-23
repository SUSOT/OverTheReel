using _01.Scripts.Core.EventSystems;
using _01.Scripts.Player.FSM;

namespace _01.Scripts.Player.States
{
    public class PlayerFishingState : PlayerState
    {
        public PlayerFishingState(Player player, int animationHash) : base(player, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.Marker.StartContinuousMovement();

            _player.PlayerInput.OnMousePressEvent += HandleMousePress;
            _player.PlayerInput.OnMouseCancelEvent += HandleMouseCancel;
            _player.AddFishingSignalListener(HandleFishingSignal);
            _player.RaiseFishingCommand(FishingCommandType.StartFishing);
        }

        public override void Exit()
        {
            _player.Marker.StopContinuousMovement();

            _player.PlayerInput.OnMousePressEvent -= HandleMousePress;
            _player.PlayerInput.OnMouseCancelEvent -= HandleMouseCancel;
            _player.RemoveFishingSignalListener(HandleFishingSignal);
            _player.RaiseFishingCommand(FishingCommandType.StopReeling);

            base.Exit();
        }

        private void HandleMousePress()
        {
            _player.RaiseFishingCommand(FishingCommandType.ReelGaugeUp);
        }

        private void HandleMouseCancel()
        {
            _player.RaiseFishingCommand(FishingCommandType.ReelGaugeDown);
        }

        private void HandleFishingSignal(FishingSignalEvent evt)
        {
            if (evt.signalType != FishingSignalType.RoundEnded) return;
            _player.ChangeState(PlayerStateType.CancelCast);
        }
    }
}


