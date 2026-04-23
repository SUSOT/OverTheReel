using _01.Scripts.Core.EventSystems;
using _01.Scripts.Sounds;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float VelocityLerpMultiplier = 2f;
        private const float StopMoveThreshold = 2f;
        private const float FullStopThreshold = 0.1f;
        private const float LookForwardOffsetY = 90f;

        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float upgradeSpeed = 2f;

        private Player _player;
        private Rigidbody _rigidbody;
        private Vector3 _movementDirection;

        private Vector3 _velocity;
        public Vector3 Velocity => _velocity;

        private PlayerVFX _boatVfx;
        private const string BoatWaveEffectName = "BoatWave";
        private bool _isStartMove;

        private float _baseMoveSpeed;
        private int _boatLevel;

        [SerializeField] private GameEventChannelSO soundChannel;
        [SerializeField] private SoundSO boatSfx;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
            _rigidbody = GetComponentInParent<Rigidbody>();
            _boatVfx = _player != null
                ? _player.GetComponentInChildren<PlayerVFX>(true)
                : GetComponentInChildren<PlayerVFX>(true);
            _baseMoveSpeed = moveSpeed;
        }

        public void SetMovementDirection(Vector2 movementInput)
        {
            _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (_player == null || _rigidbody == null) return;

            if (_movementDirection != Vector3.zero)
            {
                if (!_isStartMove)
                {
                    Vector3 vfxPosition = _player.transform.position;
                    Quaternion vfxRotation = _player.transform.rotation;
                    _boatVfx?.PlayVfx(BoatWaveEffectName, vfxPosition, vfxRotation);

                    if (soundChannel != null && boatSfx != null)
                    {
                        soundChannel.RaiseEvent(
                            SoundEvent.CreatePlayLongSfxEvent(transform.position, boatSfx, boatSfx.name)
                        );
                    }
                }

                Vector3 targetVelocity = -_player.transform.right * moveSpeed;

                _rigidbody.linearVelocity = Vector3.Lerp(
                    _rigidbody.linearVelocity,
                    targetVelocity,
                    Time.fixedDeltaTime * VelocityLerpMultiplier
                );
                _isStartMove = true;
            }
            else
            {
                _rigidbody.linearVelocity = Vector3.Lerp(
                    _rigidbody.linearVelocity,
                    Vector3.zero,
                    Time.fixedDeltaTime * VelocityLerpMultiplier
                );

                if (_rigidbody.linearVelocity.magnitude < StopMoveThreshold)
                {
                    if (_isStartMove)
                    {
                        _boatVfx?.StopVfx(BoatWaveEffectName);
                        if (soundChannel != null && boatSfx != null)
                        {
                            soundChannel.RaiseEvent(SoundEvent.CreateStopLongSfxEvent(boatSfx.name));
                        }
                        _isStartMove = false;
                    }
                }

                if (_rigidbody.linearVelocity.magnitude < FullStopThreshold)
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                }
            }

            _velocity = Quaternion.Euler(0f, LookForwardOffsetY, 0f) * _movementDirection;
            _velocity *= moveSpeed * Time.fixedDeltaTime;

            if (_velocity.sqrMagnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                Transform parent = _player.transform;
                parent.rotation = Quaternion.Lerp(parent.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }

        public void StopImmediately()
        {
            _movementDirection = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
        }

        public void ApplyBoatLevel(int boatLevel)
        {
            _boatLevel = Mathf.Max(0, boatLevel);
            moveSpeed = _baseMoveSpeed + _boatLevel * upgradeSpeed;
        }

        public void MovementSpeedUp()
        {
            ApplyBoatLevel(_boatLevel + 1);
        }
    }
}
