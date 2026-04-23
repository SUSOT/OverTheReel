using System.Collections.Generic;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;

namespace _01.Scripts.Manager.MapFlow
{
    public readonly struct MapStateSnapshot
    {
        public readonly int currentMapIndex;
        public readonly List<int> unlockedMapIndices;

        public MapStateSnapshot(int currentMapIndex, List<int> unlockedMapIndices)
        {
            this.currentMapIndex = currentMapIndex;
            this.unlockedMapIndices = unlockedMapIndices;
        }
    }

    public class MapStateService
    {
        private readonly List<Map> _unlockedMaps = new();
        private int _currentMapIndex;

        public MapStateSnapshot Load()
        {
            GameSaveData data = SaveDataSystem.Load();
            data.unlockedMaps ??= new List<Map>();

            _unlockedMaps.Clear();
            _unlockedMaps.AddRange(data.unlockedMaps);
            _currentMapIndex = data.mapIndex > 0 ? data.mapIndex : 1;

            if (_unlockedMaps.Count == 0)
            {
                _unlockedMaps.Add(Map.FirstMap);
                _currentMapIndex = 1;
                Save();
            }

            return CreateSnapshot();
        }

        public bool TryUnlock(int mapIndex)
        {
            Map targetMap = (Map)mapIndex;
            if (_unlockedMaps.Contains(targetMap)) return false;

            _unlockedMaps.Add(targetMap);
            Save();
            return true;
        }

        public bool TryChangeMap(int mapIndex, bool firstLoading, out int changedMapIndex)
        {
            changedMapIndex = _currentMapIndex;

            if (!_unlockedMaps.Contains((Map)mapIndex))
            {
                return false;
            }

            if (!firstLoading && _currentMapIndex == mapIndex)
            {
                return false;
            }

            _currentMapIndex = mapIndex;
            changedMapIndex = _currentMapIndex;
            Save();
            return true;
        }

        public MapStateSnapshot GetSnapshot()
        {
            return CreateSnapshot();
        }

        private MapStateSnapshot CreateSnapshot()
        {
            var unlockedMapIndices = new List<int>(_unlockedMaps.Count);
            foreach (Map unlockedMap in _unlockedMaps)
            {
                unlockedMapIndices.Add((int)unlockedMap);
            }

            return new MapStateSnapshot(_currentMapIndex, unlockedMapIndices);
        }

        private void Save()
        {
            GameSaveData data = SaveDataSystem.Load();
            data.mapIndex = _currentMapIndex;
            data.unlockedMaps = new List<Map>(_unlockedMaps);
            SaveDataSystem.Save(data);
        }
    }
}
