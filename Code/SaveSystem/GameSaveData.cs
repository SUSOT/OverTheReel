using System;
using System.Collections.Generic;
using _01.Scripts.Manager;

namespace _01.Scripts.SaveSystem
{
    [Serializable]
    public class GameSaveData
    {
        public Dictionary<string, int> fishMaxSizes = new Dictionary<string, int>();
        public int mapIndex;
        public int coin;
        public List<Map> unlockedMaps = new List<Map>();
        public int boatLevel;
        public int damageTime;
        public Dictionary<string, bool> uiPanelStates = new Dictionary<string, bool>();
        public Dictionary<string, bool> fishCaughtStates = new Dictionary<string, bool>();
        public List<string> savedFishIDs = new List<string>();
    }
}
