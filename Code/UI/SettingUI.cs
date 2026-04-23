using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class SettingUI : MenuPanel
    {
        private const float MinVolume = 0.0001f;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider bgmslider;
        [SerializeField] private Image uiPanel;
        

        private void Awake()
        {
            uiPanel.transform.localScale = Vector3.zero;

            if (masterSlider != null)
            {
                masterSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            }

            if (bgmslider != null)
            {
                bgmslider.onValueChanged.AddListener(SetMusicVolume);
            }
        }

        private void OnDestroy()
        {
            if (masterSlider != null)
            {
                masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            }

            if (bgmslider != null)
            {
                bgmslider.onValueChanged.RemoveListener(SetMusicVolume);
            }
        }

        public override void ShowUI()
        {
            base.ShowUI();
            
            uiPanel.gameObject.SetActive(true);
            uiPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        public override void CloseUI()
        {
            base.CloseUI();
            
            uiPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                uiPanel.gameObject.SetActive(false);
            });
        }
        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20f);
        }
        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20f);
        }
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20f);
        }
    }
}
