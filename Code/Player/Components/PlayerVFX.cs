using System.Collections.Generic;
using _01.Scripts.Effects;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerVFX : MonoBehaviour
    {
        private Dictionary<string, IPlayableVFX> _playableDictionary;

        private void Awake()
        {
            _playableDictionary = new Dictionary<string, IPlayableVFX>(System.StringComparer.OrdinalIgnoreCase);

            IPlayableVFX[] playables = GetComponentsInChildren<IPlayableVFX>(true);
            foreach (IPlayableVFX playable in playables)
            {
                if (playable == null) continue;
                if (string.IsNullOrWhiteSpace(playable.VfxName)) continue;
                _playableDictionary[playable.VfxName] = playable;
            }
        }

        public void PlayVfx(string vfxName, Vector3 position, Quaternion rotation)
        {
            if (_playableDictionary == null) return;
            if (!_playableDictionary.TryGetValue(vfxName, out IPlayableVFX vfx)) return;

            vfx.PlayVfx(position, rotation);
        }

        public void StopVfx(string vfxName)
        {
            if (_playableDictionary == null) return;
            if (!_playableDictionary.TryGetValue(vfxName, out IPlayableVFX vfx)) return;

            vfx.StopVfx();
        }
    }
}
