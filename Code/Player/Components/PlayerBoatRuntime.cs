using System.Collections.Generic;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerBoatRuntime
    {
        private readonly PlayerBoatService _playerBoatService;
        private readonly PlayerMovement _playerMovement;

        public PlayerBoatRuntime(List<GameObject> boats, PlayerMovement playerMovement)
        {
            _playerBoatService = new PlayerBoatService(boats);
            _playerMovement = playerMovement;
        }

        public void ApplySavedBoat()
        {
            int savedBoatLevel = Mathf.Max(0, SaveDataSystem.Load().boatLevel);
            if (_playerBoatService.TrySetActiveBoat(savedBoatLevel, out int appliedLevel))
            {
                _playerMovement?.ApplyBoatLevel(appliedLevel);
                SaveBoatLevel(appliedLevel);
                return;
            }

            if (_playerBoatService.TrySetActiveBoat(0, out int fallbackLevel))
            {
                _playerMovement?.ApplyBoatLevel(fallbackLevel);
                SaveBoatLevel(fallbackLevel);
            }
        }

        public bool TryApplyBoatChange(int index)
        {
            if (!_playerBoatService.TrySetActiveBoat(index, out int appliedLevel))
            {
                return false;
            }

            _playerMovement?.ApplyBoatLevel(appliedLevel);
            SaveBoatLevel(appliedLevel);
            return true;
        }

        private void SaveBoatLevel(int boatLevel)
        {
            GameSaveData data = SaveDataSystem.Load();
            data.boatLevel = boatLevel;
            SaveDataSystem.Save(data);
        }
    }
}
