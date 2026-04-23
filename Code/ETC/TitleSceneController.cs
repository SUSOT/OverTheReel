using System.Collections;
using System.Collections.Generic;
using _01.Scripts.Core.EventSystems;
using _01.Scripts.SaveSystem;
using SaveDataSystem = _01.Scripts.SaveSystem.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01.Scripts.ETC
{
    public class TitleSceneController : MonoBehaviour
    {
        private const float InitialDelay = 1f;
        private const float FillStartPercent = 95f;
        private const float FillCompletePercent = 100f;
        private const float FillDelay = 1f;
        private const float FillDuration = 0.5f;

        [SerializeField] private Slider loadingSlider;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private List<Sprite> sprites;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private float minimumLoadTime;
        [SerializeField] private float fadeDuration;

        [SerializeField] private GameEventChannelSO mapChannel;

        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField, TextArea] private List<string> tips;
        private bool _isLoadingComplete;

        private void Start()
        {
            if (tips != null && tips.Count > 0)
            {
                tipText.SetText(tips[Random.Range(0, tips.Count)]);
            }

            loadingSlider.value = 0;
            versionText.SetText("version : " + Application.version);

            if (sprites != null && sprites.Count > 0)
            {
                backgroundImage.sprite = sprites[Random.Range(0, sprites.Count)];
            }

            StartCoroutine(SceneLoading());
        }

        private IEnumerator SceneLoading()
        {
            yield return new WaitForSeconds(InitialDelay);

            float startTime = Time.time;
            int mapIndex = GetSavedMapIndex();

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mapIndex);

            if (asyncOperation == null) yield break;
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                float realProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                float timeProgress = Mathf.Clamp01((Time.time - startTime) / minimumLoadTime);
                float displayProgress = Mathf.Min(realProgress, timeProgress) * 100f;

                loadingSlider.value = displayProgress;
                loadingText.text = $"{Mathf.RoundToInt(displayProgress)}%";

                if (!_isLoadingComplete && displayProgress >= FillStartPercent)
                {
                    yield return StartCoroutine(FillToComplete());
                }

                yield return null;
            }
        }

        private IEnumerator FillToComplete()
        {
            _isLoadingComplete = true;
            yield return new WaitForSeconds(FillDelay);

            float fillElapsed = 0f;

            while (fillElapsed < FillDuration)
            {
                fillElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(fillElapsed / FillDuration);
                float value = Mathf.Lerp(FillStartPercent, FillCompletePercent, t);

                loadingSlider.value = value;
                loadingText.text = $"{Mathf.RoundToInt(value)}%";

                yield return null;
            }

            loadingSlider.value = FillCompletePercent;
            loadingText.text = $"{FillCompletePercent}%";

            mapChannel?.RaiseEvent(MapEvent.CreateChangeMapEvent(GetSavedMapIndex(), true));

            yield return new WaitForSeconds(fadeDuration + 1f);
        }

        private int GetSavedMapIndex()
        {
            GameSaveData data = SaveDataSystem.Load();
            return data.mapIndex > 0 ? data.mapIndex : 1;
        }
    }
}
