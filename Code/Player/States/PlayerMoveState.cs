using _01.Scripts.Player.FSM;
using UnityEngine;

namespace _01.Scripts.Player.States
{
    public class PlayerMoveState : PlayerCanCastState
    {
        public PlayerMoveState(Player player, int animationHash) : base(player, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            _player.MovementController.SetMovementDirection(movementKey);
            if (movementKey.magnitude < _inputThreshold)
            {
                _player.ChangeState(PlayerStateType.Idle);
            }
        }
    }
}


