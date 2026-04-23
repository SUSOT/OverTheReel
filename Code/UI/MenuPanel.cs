using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Sounds;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class MenuPanel : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] protected GameEventChannelSO menuUiChannel;
        [SerializeField] protected GameEventChannelSO playerChannel;
        [SerializeField] protected GameEventChannelSO soundChannel;
        
        [SerializeField] protected SoundSO uiSound;

        public virtual void ShowUI()
        {
            PlayUiSound();
            SendMenuEvent(false);
        }

        public virtual void CloseUI()
        {
            PlayUiSound();
            SendMenuEvent(true);
        }

        private void PlayUiSound()
        {
            if (uiSound == null) return;

            soundChannel.RaiseEvent(SoundEvent.CreatePlaySfxEvent(transform.position, uiSound));
        }

        private void SendMenuEvent(bool isOpen)
        {
            menuUiChannel.RaiseEvent(MenuUIEvent.CreateMenuUIOpenEvent(isOpen));
            playerChannel.RaiseEvent(PlayerEvent.CreatePlayerInputEvent(isOpen));
        }
    }
}
