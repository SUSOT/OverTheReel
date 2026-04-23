using System.Collections.Generic;
using System.Linq;
using _01.Scripts.Fish;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class LoadAllFishPictorial : MonoBehaviour
    {
        [SerializeField] private List<FishPictorialBookUI> fishPictorialBooks = new();
        [SerializeField] private FishDataListSO fishDataListSO;

        private void OnEnable()
        {
            fishPictorialBooks = GetComponentsInChildren<FishPictorialBookUI>(true).ToList();

            GameSaveData data = SaveDataSystem.Load();
            data.fishCaughtStates ??= new Dictionary<string, bool>();
            data.fishMaxSizes ??= new Dictionary<string, int>();

            bool changed = EnsureFishCaughtStateEntries(data);
            changed |= RestoreCaughtStateFromSavedInventory(data);
            if (changed)
            {
                SaveDataSystem.Save(data);
            }

            foreach (FishPictorialBookUI fishView in fishPictorialBooks)
            {
                if (fishView == null || fishView.fishData == null) continue;

                string fishName = fishView.fishData.fishName;
                bool isCaught = data.fishCaughtStates.TryGetValue(fishName, out bool caught) && caught;
                int maxSize = data.fishMaxSizes.TryGetValue(fishName, out int size) ? size : 0;

                fishView.Render(isCaught, maxSize);
            }
        }

        private bool EnsureFishCaughtStateEntries(GameSaveData data)
        {
            if (fishDataListSO == null || fishDataListSO.allFishDatas == null)
            {
                return false;
            }

            bool changed = false;
            foreach (FishDataSO fish in fishDataListSO.allFishDatas)
            {
                if (fish == null) continue;
                if (data.fishCaughtStates.ContainsKey(fish.fishName)) continue;

                data.fishCaughtStates[fish.fishName] = false;
                changed = true;
            }

            return changed;
        }

        private bool RestoreCaughtStateFromSavedInventory(GameSaveData data)
        {
            if (fishDataListSO == null || fishDataListSO.allFishDatas == null) return false;
            if (data.savedFishIDs == null || data.savedFishIDs.Count == 0) return false;

            bool changed = false;
            foreach (string fishId in data.savedFishIDs)
            {
                if (string.IsNullOrWhiteSpace(fishId)) continue;

                FishDataSO fish = fishDataListSO.allFishDatas.Find(item => item != null && item.name == fishId);
                if (fish == null || string.IsNullOrWhiteSpace(fish.fishName)) continue;

                bool hasCaughtState = data.fishCaughtStates.TryGetValue(fish.fishName, out bool isCaught);
                if (!hasCaughtState || !isCaught)
                {
                    data.fishCaughtStates[fish.fishName] = true;
                    changed = true;
                }
            }

            return changed;
        }
    }
}
