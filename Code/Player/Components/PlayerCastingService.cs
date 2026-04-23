using System;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerCastingService
    {
        private readonly PlayerCastingValidator _castingValidator;
        private readonly GameEventChannelSO _castingUiChannel;

        public PlayerCastingService(
            Transform avatarTransform,
            LayerMask waterLayerMask,
            float castingDistance,
            GameEventChannelSO castingUiChannel
        )
        {
            _castingValidator = new PlayerCastingValidator(avatarTransform, waterLayerMask, castingDistance);
            _castingUiChannel = castingUiChannel;
        }

        public void TryCast(Vector3 castingPos, bool isCastingClickRequired, Action<Vector3> onCastingSuccess)
        {
            if (!isCastingClickRequired) return;

            CastingValidationResult validationResult = _castingValidator.Validate(castingPos);
            if (!validationResult.isValid)
            {
                _castingUiChannel?.RaiseEvent(CastingUIEvent.CreateCastingFailEvent(validationResult.failType));
                return;
            }

            onCastingSuccess?.Invoke(castingPos);
        }
    }
}



