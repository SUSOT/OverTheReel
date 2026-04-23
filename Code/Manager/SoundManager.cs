using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.ObjectPool.Runtime;
using _01.Scripts.Sounds;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO soundChannel;
        [SerializeField] private PoolingItemSO soundPlayer;

        [SerializeField] private PoolManagerMono poolManager;
        private SoundPlayer _bgmPlayer;

        private Dictionary<string, SoundPlayer> _longSoundDict;

        private void Awake()
        {
            if (poolManager == null)
            {
                Debug.LogError("SoundManager: PoolManagerMono reference is missing.");
            }

            _longSoundDict = new Dictionary<string, SoundPlayer>();

            soundChannel.AddListener<PlaySFXEvent>(HandlePlaySFX);
            soundChannel.AddListener<PlayBGMEvent>(HandlePlayBGM);
            soundChannel.AddListener<StopBGMEvent>(HandleStopBGM);
            soundChannel.AddListener<PlayLongSFXEvent>(HandlePlayLongSFX);
            soundChannel.AddListener<StopLongSFXEvent>(HandleStopLongSFX);
        }

        private void OnDestroy()
        {
            soundChannel.RemoveListener<PlaySFXEvent>(HandlePlaySFX);
            soundChannel.RemoveListener<PlayBGMEvent>(HandlePlayBGM);
            soundChannel.RemoveListener<StopBGMEvent>(HandleStopBGM);
            soundChannel.RemoveListener<PlayLongSFXEvent>(HandlePlayLongSFX);
            soundChannel.RemoveListener<StopLongSFXEvent>(HandleStopLongSFX);
        }

        private void HandlePlayLongSFX(PlayLongSFXEvent evt)
        {
            if (poolManager == null || evt.soundClip == null) return;

            if (_longSoundDict.TryGetValue(evt.soundName, out SoundPlayer player))
            {
                player.StopAndReturnToPool();
                _longSoundDict.Remove(evt.soundName);
            }

            SoundPlayer sfxPlayer = poolManager.Pop<SoundPlayer>(soundPlayer);
            sfxPlayer.transform.position = evt.position;
            sfxPlayer.PlaySound(evt.soundClip);

            _longSoundDict.Add(evt.soundName, sfxPlayer);
        }

        private void HandleStopLongSFX(StopLongSFXEvent evt)
        {
            if (_longSoundDict.TryGetValue(evt.soundName, out SoundPlayer player))
            {
                player.StopAndReturnToPool();
                _longSoundDict.Remove(evt.soundName);
            }
        }

        private void HandlePlaySFX(PlaySFXEvent evt)
        {
            if (poolManager == null || evt.soundClip == null) return;

            SoundPlayer sfxPlayer = poolManager.Pop<SoundPlayer>(soundPlayer);
            sfxPlayer.transform.position = evt.position;
            sfxPlayer.PlaySound(evt.soundClip);
        }

        private void HandlePlayBGM(PlayBGMEvent evt)
        {
            if (poolManager == null || evt.bgmClip == null) return;

            _bgmPlayer?.StopAndReturnToPool();
            _bgmPlayer = poolManager.Pop<SoundPlayer>(soundPlayer);
            _bgmPlayer.PlaySound(evt.bgmClip);
        }

        private void HandleStopBGM(StopBGMEvent evt)
        {
            _bgmPlayer?.StopAndReturnToPool();
        }
    }
}

