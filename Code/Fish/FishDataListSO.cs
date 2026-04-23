using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Scripts.Fish
{
    [CreateAssetMenu(fileName = "FishDataListSO", menuName = "SO/FishDataListSO")]
    public class FishDataListSO : ScriptableObject
    {
        public List<FishDataSO> allFishDatas;

        public FishDataSO GetRandomFishByMap(int mapIndex, List<int> fishingRandomPercent)
        {
            if (allFishDatas == null)
            {
                return null;
            }

            List<FishDataSO> availableFish = allFishDatas
                .Where(fish => fish != null && (int)fish.spawnMap == mapIndex)
                .ToList();

            if (availableFish.Count == 0) return null;

            if (fishingRandomPercent == null || fishingRandomPercent.Count == 0)
            {
                return availableFish[Random.Range(0, availableFish.Count)];
            }

            int randomValue = Random.Range(1, 101);
            int tierIndex = 0;
            while (tierIndex < fishingRandomPercent.Count - 1 && randomValue > fishingRandomPercent[tierIndex])
            {
                randomValue -= fishingRandomPercent[tierIndex];
                tierIndex++;
            }

            List<FishDataSO> tierMatchedFish = availableFish
                .Where(fish => (int)fish.tier == tierIndex)
                .ToList();

            if (tierMatchedFish.Count == 0)
            {
                tierMatchedFish = availableFish;
            }

            return tierMatchedFish[Random.Range(0, tierMatchedFish.Count)];
        }
    }
}
