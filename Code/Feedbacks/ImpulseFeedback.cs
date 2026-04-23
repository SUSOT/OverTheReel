using Unity.Cinemachine;
using UnityEngine;

namespace _01.Scripts.Feedbacks
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class ImpulseFeedback : Feedback
    {
        private CinemachineImpulseSource _source;

        [SerializeField] private float _impulsePower = 0.3f;

        private void Awake()
        {
            _source = GetComponent<CinemachineImpulseSource>();
        }

        public override void PlayFeedback()
        {
            _source.GenerateImpulse(_impulsePower);
        }

        public override void StopFeedback()
        {
        }
    }
}