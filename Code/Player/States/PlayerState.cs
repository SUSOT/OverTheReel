using _01.Scripts.Player.Components;
using UnityEngine;

namespace _01.Scripts.Player.States
{
    public class PlayerState
    {
        protected Player _player;
        protected int _animationHash;
        protected bool _isTriggerCall;
        protected readonly float _inputThreshold = 0.1f;

        public PlayerState(Player player, int animationHash)
        {
            Debug.Assert(player != null, "Player state using only in player");
            _player = player;
            _animationHash = animationHash;
        }

        public virtual void Enter()
        {
            _player.PlayerAnimator?.SetParam(_animationHash, true);
            _isTriggerCall = false;
            if (_player.PlayerAnimatorTrigger != null)
            {
                _player.PlayerAnimatorTrigger.OnAnimationEndTrigger += AnimationEndTrigger;
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
            _player.PlayerAnimator?.SetParam(_animationHash, false);
            if (_player.PlayerAnimatorTrigger != null)
            {
                _player.PlayerAnimatorTrigger.OnAnimationEndTrigger -= AnimationEndTrigger;
            }
        }

        protected virtual void AnimationEndTrigger() => _isTriggerCall = true;
    }

    public enum PlayerStateType
    {
        Idle,
        Move,
        WaitCast,
        Casting,
        Fishing,
        CancelCast
    }
}