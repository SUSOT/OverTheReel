using _01.Scripts.Core.EventSystems.UIEvent;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public readonly struct CastingValidationResult
    {
        public readonly bool isValid;
        public readonly CastingFailEventType failType;

        public CastingValidationResult(bool isValid, CastingFailEventType failType)
        {
            this.isValid = isValid;
            this.failType = failType;
        }
    }

    public class PlayerCastingValidator
    {
        private readonly Transform _avatarTransform;
        private readonly LayerMask _waterLayerMask;
        private readonly float _castingDistance;

        public PlayerCastingValidator(Transform avatarTransform, LayerMask waterLayerMask, float castingDistance)
        {
            _avatarTransform = avatarTransform;
            _waterLayerMask = waterLayerMask;
            _castingDistance = castingDistance;
        }

        public CastingValidationResult Validate(Vector3 castingPos)
        {
            if (!Physics.CheckSphere(castingPos, 0.1f, _waterLayerMask))
            {
                return new CastingValidationResult(false, CastingFailEventType.NotWater);
            }

            if (!IsWithinCastingRange(castingPos))
            {
                return new CastingValidationResult(false, CastingFailEventType.NearRange);
            }

            return new CastingValidationResult(true, default);
        }

        private bool IsWithinCastingRange(Vector3 castingPos)
        {
            if (_avatarTransform == null) return false;

            Vector3 playerPos = _avatarTransform.position;
            float horizontalDistance = Vector2.Distance(
                new Vector2(playerPos.x, playerPos.z),
                new Vector2(castingPos.x, castingPos.z)
            );

            return horizontalDistance > _castingDistance;
        }
    }
}



