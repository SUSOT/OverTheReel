using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Scripts.ETC
{
    public class Marker : MonoBehaviour
    {
        private _01.Scripts.Player.Player _player;

        [Header("Main")]
        [field: SerializeField] public Transform CastingMarker { get; private set; }
        [field: SerializeField] public Transform MarkerPos { get; private set; }

        [Header("Fishing")]
        [SerializeField] private float movementRange = 2.0f;
        [SerializeField] private float movementDuration = 0.5f;
        private Tweener _moveTweener;
        private Vector3 _initialPosition;
        private Vector3 _forwardDirection;
        private Vector3 _rightDirection;

        [Header("Particle")]
        [SerializeField] private ParticleSystem _bubbleParticle;

        private void Awake()
        {
            _player = GetComponentInParent<_01.Scripts.Player.Player>();
            if (_player == null)
            {
                Debug.LogError($"{nameof(Marker)}: Player reference is missing.");
            }
        }

        private void Start()
        {
            ActiveMarker(false);
            Debug.Assert(CastingMarker != null, "CastingMarker is not assigned.");
            Debug.Assert(_bubbleParticle != null, "Bubble particle is not assigned.");
        }

        public Tween HookingMarker(Vector3 movingPos, float jumpPower, float duration)
        {
            return CastingMarker.transform.DOJump(movingPos, jumpPower, 1, duration);
        }

        public void ActiveMarker(bool isActive) => CastingMarker.gameObject.SetActive(isActive);

        public void HitFishMarker()
        {
            CastingMarker.DOScale(0.1f, 0.2f).SetEase(Ease.OutBounce);
            CastingMarker.DOMoveY(CastingMarker.position.y - 0.5f, 0.2f).SetEase(Ease.OutBounce);
        }

        public void StartContinuousMovement()
        {
            _moveTweener?.Kill();

            _initialPosition = CastingMarker.position;
            _bubbleParticle.Play();
            CalculateDirections();
            InitializeContinuousMovement();
        }

        public void StopContinuousMovement()
        {
            _moveTweener?.Kill();
            CastingMarker.position = _initialPosition;

            _bubbleParticle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        private void CalculateDirections()
        {
            if (_player == null || _player.AvatarTransform == null) return;
            _forwardDirection = _player.AvatarTransform.forward;
            _rightDirection = Vector3.Cross(_forwardDirection, Vector3.up).normalized;
        }

        private void InitializeContinuousMovement()
        {
            _moveTweener = CastingMarker.DOMove(CalculateRandomTarget(), movementDuration)
                .SetEase(Ease.InOutQuad)
                .SetAutoKill(false)
                .OnComplete(OnMovementComplete);
        }

        private Vector3 CalculateRandomTarget()
        {
            float signX = Random.value < 0.5f ? -1f : 1f;
            float signZ = Random.value < 0.5f ? -1f : 1f;
            float offsetX = Random.Range(movementRange * 0.3f, movementRange) * signX;
            float offsetZ = Random.Range(movementRange * 0.3f, movementRange) * signZ;

            return _initialPosition + _rightDirection * offsetX + _forwardDirection * offsetZ;
        }

        private void OnMovementComplete()
        {
            _moveTweener.ChangeEndValue(CalculateRandomTarget(), movementDuration, true)
                .Restart();
        }
    }
}

