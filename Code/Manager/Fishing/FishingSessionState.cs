using _01.Scripts.Fish;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Scripts.Manager.Fishing
{
    public class FishingSessionState
    {
        private const float ReelGaugeMin = 0f;
        private const float ReelGaugeMax = 100f;
        private const float InitialReelGauge = 30f;
        private const float DefaultPerfectZoneMin = 80f;
        private const float PerfectZoneWidth = 15f;
        private const float PerfectZoneMinLimit = 15f;
        private const float PerfectZoneMaxLimit = 95f;
        private const float LowGaugeSlowThreshold = 10f;

        private float _reelGauge = InitialReelGauge;
        private float _fishingElapsedTime;
        private float _fishHp;

        private float _minSpeed;
        private float _maxSpeed;
        private Vector2 _speedChangeIntervalRange;
        private float _currentSpeed;
        private float _speedTimer;
        private float _speedChangeInterval;
        private bool _isIncreasing;
        private bool _isDecreasing;

        private float _perfectZoneMoveSpeed;
        private float _perfectZoneMoveDirection = -1f;
        private float _perfectZoneChangeTimer;
        private float _perfectZoneChangeInterval;
        private Vector2 _perfectZoneSpeedRange;
        private Vector2 _perfectZoneChangeIntervalRange;
        private bool _wasInPerfectZone;

        public float ReelGauge => _reelGauge;
        public float PerfectZoneMin { get; private set; } = DefaultPerfectZoneMin;
        public float PerfectZoneMax { get; private set; } = DefaultPerfectZoneMin + PerfectZoneWidth;
        public bool IsFishingStarted { get; private set; }
        public bool IsReeling => _isIncreasing || _isDecreasing;

        public void StartFishing(FishDataSO fishData)
        {
            _minSpeed = fishData.minReelSpeed;
            _maxSpeed = fishData.maxReelSpeed;
            _speedChangeIntervalRange = fishData.speedChangeIntervalRange;
            _fishHp = fishData.hp;

            _fishingElapsedTime = 0f;
            _reelGauge = InitialReelGauge;
            IsFishingStarted = true;
            _wasInPerfectZone = false;
            _speedTimer = 0f;
            RefreshRandomSpeed();

            InitializePerfectZoneByTier(fishData.tier);
        }

        public void Reset()
        {
            _reelGauge = InitialReelGauge;
            _fishingElapsedTime = 0f;
            _fishHp = 0f;
            _isIncreasing = false;
            _isDecreasing = false;
            IsFishingStarted = false;
            _wasInPerfectZone = false;
        }

        public void ReelGaugeUp()
        {
            _isIncreasing = true;
            _isDecreasing = false;
            RefreshRandomSpeed();
        }

        public void ReelGaugeDown()
        {
            _isIncreasing = false;
            _isDecreasing = true;
            RefreshRandomSpeed();
        }

        public void StopReeling()
        {
            _isIncreasing = false;
            _isDecreasing = false;
        }

        public void UpdateReel(float deltaTime)
        {
            UpdateSpeed(deltaTime);

            if (_isIncreasing)
            {
                _reelGauge = Mathf.Min(_reelGauge + _currentSpeed * deltaTime, ReelGaugeMax);
                return;
            }

            if (_isDecreasing)
            {
                float speed = _reelGauge < LowGaugeSlowThreshold ? 10f : _currentSpeed;
                _reelGauge = Mathf.Max(_reelGauge - speed * deltaTime, ReelGaugeMin);
            }
        }

        public void UpdateFishingTime(float deltaTime)
        {
            _fishingElapsedTime += deltaTime;
        }

        public void UpdatePerfectZone(float deltaTime)
        {
            _perfectZoneChangeTimer += deltaTime;

            if (_perfectZoneChangeTimer >= _perfectZoneChangeInterval)
            {
                _perfectZoneChangeTimer = 0f;
                RefreshPerfectZoneMovementProfile();
                if (Random.value < 0.5f)
                {
                    _perfectZoneMoveDirection *= -1f;
                }
            }

            PerfectZoneMin += _perfectZoneMoveDirection * _perfectZoneMoveSpeed * deltaTime;

            float maxZoneMin = PerfectZoneMaxLimit - PerfectZoneWidth;
            if (PerfectZoneMin <= PerfectZoneMinLimit)
            {
                PerfectZoneMin = PerfectZoneMinLimit;
                _perfectZoneMoveDirection = 1f;
            }
            else if (PerfectZoneMin >= maxZoneMin)
            {
                PerfectZoneMin = maxZoneMin;
                _perfectZoneMoveDirection = -1f;
            }

            PerfectZoneMax = PerfectZoneMin + PerfectZoneWidth;
        }

        public bool TryConsumePerfectZoneEntryChanged(bool isInPerfectZone, out bool shouldShowPerfectText)
        {
            shouldShowPerfectText = false;
            if (isInPerfectZone == _wasInPerfectZone) return false;

            _wasInPerfectZone = isInPerfectZone;
            shouldShowPerfectText = isInPerfectZone;
            return true;
        }

        public bool TryApplyPerfectZoneDamage(float deltaTime, int damagePerSecond, float maxHp, out float hpPercent)
        {
            hpPercent = 0f;

            if (!IsReelInPerfectZone()) return false;
            if (_fishHp <= 0f) return false;

            _fishHp = Mathf.Max(_fishHp - damagePerSecond * deltaTime, 0f);
            float safeMaxHp = Mathf.Max(maxHp, 0.0001f);
            hpPercent = (_fishHp / safeMaxHp) * 100f;
            return true;
        }

        public bool IsFailed(float fishingTimeLimit)
        {
            return _reelGauge is >= 99.5f or <= 0.5f || _fishingElapsedTime >= fishingTimeLimit;
        }

        public bool IsFishCaught() => _fishHp <= 0f;

        public bool IsReelInPerfectZone() => _reelGauge >= PerfectZoneMin && _reelGauge < PerfectZoneMax;

        private void UpdateSpeed(float deltaTime)
        {
            _speedTimer += deltaTime;
            if (_speedTimer < _speedChangeInterval) return;

            _speedTimer = 0f;
            RefreshRandomSpeed();
        }

        private void RefreshRandomSpeed()
        {
            _speedChangeInterval = Random.Range(_speedChangeIntervalRange.x, _speedChangeIntervalRange.y);
            _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
        }

        private void InitializePerfectZoneByTier(Tier tier)
        {
            PerfectZoneMin = DefaultPerfectZoneMin;
            PerfectZoneMax = PerfectZoneMin + PerfectZoneWidth;
            _perfectZoneMoveDirection = Random.value < 0.5f ? -1f : 1f;
            _perfectZoneChangeTimer = 0f;

            (_perfectZoneSpeedRange, _perfectZoneChangeIntervalRange) = tier switch
            {
                Tier.Common => (new Vector2(6f, 9f), new Vector2(1.8f, 2.5f)),
                Tier.Rare => (new Vector2(8f, 12f), new Vector2(1.3f, 2f)),
                Tier.Epic => (new Vector2(11f, 16f), new Vector2(0.9f, 1.5f)),
                Tier.Legendary => (new Vector2(14f, 20f), new Vector2(0.6f, 1.1f)),
                _ => (new Vector2(6f, 9f), new Vector2(1.8f, 2.5f))
            };

            RefreshPerfectZoneMovementProfile();
        }

        private void RefreshPerfectZoneMovementProfile()
        {
            _perfectZoneMoveSpeed = Random.Range(_perfectZoneSpeedRange.x, _perfectZoneSpeedRange.y);
            _perfectZoneChangeInterval =
                Random.Range(_perfectZoneChangeIntervalRange.x, _perfectZoneChangeIntervalRange.y);
        }
    }
}