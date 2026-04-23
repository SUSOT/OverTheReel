using UnityEngine;

namespace _01.Scripts.Player.FSM
{
    [CreateAssetMenu(fileName = "StateData", menuName = "SO/FSM/StateData", order = 0)]
    public class StateDataSO : ScriptableObject
    {
        public string stateName;
        public string animParamName;
        
        [HideInInspector]
        public int animationHash;

        private void OnValidate()
        {
            animationHash = Animator.StringToHash(animParamName);
        }
    }
}