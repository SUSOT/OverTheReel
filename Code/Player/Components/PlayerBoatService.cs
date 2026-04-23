using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerBoatService
    {
        private readonly List<GameObject> _boats;

        public PlayerBoatService(List<GameObject> boats)
        {
            _boats = boats;
        }

        public bool TrySetActiveBoat(int index, out int appliedLevel)
        {
            appliedLevel = 0;

            if (_boats == null || _boats.Count == 0) return false;
            if (index < 0 || index >= _boats.Count) return false;

            foreach (GameObject boat in _boats)
            {
                if (boat != null)
                {
                    boat.SetActive(false);
                }
            }

            if (_boats[index] == null) return false;

            _boats[index].SetActive(true);
            appliedLevel = index;
            return true;
        }
    }
}


