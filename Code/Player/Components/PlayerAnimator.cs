using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);

        public void SetAnimatorOff()
        {
            animator.enabled = false;
        }
    }
}



