using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using _01.Scripts.Sounds;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01.Scripts.ETC
{
    public class ChangeSceneComponent : MonoBehaviour
    {
        private Image _darkImage;
        [SerializeField] private GameEventChannelSO changeSceneChannel;
        [SerializeField] private GameEventChannelSO playerChannel;
        [SerializeField] private GameEventChannelSO soundChannel;

        [SerializeField] private SoundSO mainBgm;
        [SerializeField] private SoundSO waveSfx;

        private void Awake()
        {
            _darkImage = GetComponentInChildren<Image>();
            changeSceneChannel.AddListener<FadeAndChangeEvent>(FadeDarkPanel);
        }

        private void OnDestroy()
        {
            changeSceneChannel.RemoveListener<FadeAndChangeEvent>(FadeDarkPanel);
        }

        private void Start()
        {
            playerChannel.RaiseEvent(PlayerEvent.CreatePlayerInputEvent(false));

            soundChannel.RaiseEvent(SoundEvent.CreatePlayBgmEvent(mainBgm));

            soundChannel.RaiseEvent(SoundEvent.CreatePlaySfxEvent(Vector3.zero, waveSfx));
            
            
            var color = _darkImage.color;
            color.a = 1f;
            _darkImage.color = color;
            _darkImage.DOKill();
            _darkImage.DOFade(0f, 4f).SetEase(Ease.Linear);
        }

        private void FadeDarkPanel(FadeAndChangeEvent evt)
        {
            _darkImage.DOKill();
            _darkImage.DOFade(evt.alpha, evt.duration).SetEase(Ease.InSine);
            
            DOVirtual.DelayedCall(evt.duration, () =>
            {
                SceneManager.LoadScene(evt.mapIndex);
            });
        }
    }
}

