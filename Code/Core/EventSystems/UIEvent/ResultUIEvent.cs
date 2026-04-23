using UnityEngine;

namespace _01.Scripts.Core.EventSystems.UIEvent
{
    public static class ResultUIEvent
    {
        public static ShowResultUIEvent CreateShowResultUIEvent(
            Sprite newSprite,
            string fishName,
            string tierTxt,
            Color backTierImageColor,
            float size,
            bool isBestRecord,
            float spriteSize,
            bool isFlip
        ) =>
            new()
            {
                newSprite = newSprite,
                fishName = fishName,
                tierTxt = tierTxt,
                backTierImageColor = backTierImageColor,
                size = size,
                isBestRecord = isBestRecord,
                spriteSize = spriteSize,
                isFlip = isFlip
            };
    }

    public class ShowResultUIEvent : GameEvent
    {
        public Sprite newSprite;
        public string fishName;
        public string tierTxt;
        public Color backTierImageColor;
        public float size;
        public bool isBestRecord;
        public float spriteSize;
        public bool isFlip;
    }
}
