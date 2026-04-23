using _01.Scripts.Sounds;
using UnityEngine;

namespace _01.Scripts.Core.EventSystems
{
    public static class SoundEvent
    {
        public static PlaySFXEvent CreatePlaySfxEvent(Vector3 position, SoundSO soundClip) =>
            new() { position = position, soundClip = soundClip };

        public static PlayBGMEvent CreatePlayBgmEvent(SoundSO bgmClip) =>
            new() { bgmClip = bgmClip };

        public static StopBGMEvent CreateStopBgmEvent() => new();

        public static PlayLongSFXEvent CreatePlayLongSfxEvent(Vector3 position, SoundSO soundClip, string soundName) =>
            new() { position = position, soundClip = soundClip, soundName = soundName };

        public static StopLongSFXEvent CreateStopLongSfxEvent(string soundName) =>
            new() { soundName = soundName };
    }
    
    public class PlaySFXEvent : GameEvent
    {
        public Vector3 position;
        public SoundSO soundClip;
    }

    public class PlayBGMEvent : GameEvent
    {
        public SoundSO bgmClip;
    }

    public class StopBGMEvent : GameEvent
    {
    }

    public class PlayLongSFXEvent : GameEvent
    {
        public Vector3 position;
        public SoundSO soundClip;
        public string soundName;
    }

    public class StopLongSFXEvent : GameEvent
    {
        public string soundName;
    }
}
