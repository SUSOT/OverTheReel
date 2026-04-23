using System.Collections.Generic;
using _01.Scripts.Manager;
using UnityEngine;

namespace _01.Scripts.Fish
{
    [CreateAssetMenu(fileName = "FishDataSO", menuName = "SO/FishDataSO")]
    public class FishDataSO : ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("물고기 식별용 이름")]
        public string fishName;
        
        [Header("Visuals")]
        [Tooltip("물고기 스프라이트")]
        public Sprite fishSprite;
        [Tooltip("인벤토리 또는 UI에서 표시할 스프라이트 크기")]
        public int spriteSize;
        [Tooltip("스프라이트 좌우 반전 여부")]
        public bool isFlip;
        
        
        
        [Header("Stats")]
        [Tooltip("물고기 체력(HP)")]  
        public float hp;
        [Tooltip("낚시 가능한 시간(초)")]
        public float fishingTime;
        [Tooltip("스폰될 맵")]
        public Map spawnMap;

        [Header("Reeling Speed")]
        [Tooltip("최소 릴 감기 속도")]   
        public float minReelSpeed;
        [Tooltip("최대 릴 감기 속도")]   
        public float maxReelSpeed;
        [Tooltip("속도 변경 간격 범위")]
        public Vector2 speedChangeIntervalRange;

        [Header("Rewards")]
        [Tooltip("획득 코인 수")]
        public int coin;

        [Header("Size & Tier")]
        [Tooltip("물고기 차지 크기(최소, 최대)")]
        public Vector2Int size;
        [Tooltip("희귀도")]
        public Tier tier;
        public Color backTierImageColor;


        private static readonly Dictionary<Tier, Color> TierColors = new Dictionary<Tier, Color>
        {
            { Tier.Common,    new Color(0.7f, 0.7f, 0.7f,1f) },
            { Tier.Rare,      new Color(0.2f, 0.6f, 1f,1f)   },
            { Tier.Epic,      new Color(0.6f, 0.2f, 1f,1f)   }, 
            { Tier.Legendary, new Color(1f, 0.8f, 0.2f,1f)   } 
        };
        private void OnValidate()
        {
            if (TierColors.TryGetValue(tier, out var color))
            {
                backTierImageColor = color;
            }
        }
    }

    public enum Tier
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
}