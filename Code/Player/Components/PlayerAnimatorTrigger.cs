using System;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerAnimatorTrigger : MonoBehaviour
    {
        public Action OnAnimationEndTrigger;
        public Action OnCastingAnimationTrigger;

        private void AnimationEnd() => OnAnimationEndTrigger?.Invoke();
        private void CastingAnimation() => OnCastingAnimationTrigger?.Invoke();
    }
}


