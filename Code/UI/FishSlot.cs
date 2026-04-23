using System;
using _01.Scripts.Fish;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _01.Scripts.UI
{
    public class FishSlot : MonoBehaviour
    {
        [SerializeField] private Image fishImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image checkmarkImage;

        private FishDataSO _fishData;
        private bool _isSelected;
        private GameObject _spawnedPreviewObject;
        private RectTransform _previewSpawnArea;
        private Action<int, FishDataSO, bool> _onSelectionChanged;

        public void SetupFishSlot(
            FishDataSO fishData,
            RectTransform previewSpawnArea,
            Action<int, FishDataSO, bool> onSelectionChanged
        )
        {
            if (fishData == null || fishImage == null || backgroundImage == null) return;

            _fishData = fishData;
            _previewSpawnArea = previewSpawnArea;
            _onSelectionChanged = onSelectionChanged;
            _isSelected = false;

            fishImage.sprite = fishData.fishSprite;
            backgroundImage.color = fishData.backTierImageColor;
            SetCheckmarkVisible(false);
            AdjustFishSize();
            DestroyPreviewIfExists();
        }

        public void CheckSlot()
        {
            if (_fishData == null) return;

            _isSelected = !_isSelected;
            SetCheckmarkVisible(_isSelected);

            if (_isSelected)
            {
                SpawnPreview();
            }
            else
            {
                DestroyPreviewIfExists();
            }

            _onSelectionChanged?.Invoke(GetInstanceID(), _fishData, _isSelected);
        }

        private void OnDisable()
        {
            DestroyPreviewIfExists();
        }

        private void AdjustFishSize()
        {
            if (fishImage.sprite == null || _fishData == null) return;

            float spriteWidth = fishImage.sprite.rect.width;
            float spriteHeight = fishImage.sprite.rect.height;
            float maxSize = _fishData.spriteSize / 2f;
            float scaleFactor = Mathf.Min(maxSize / spriteWidth, maxSize / spriteHeight);

            fishImage.rectTransform.sizeDelta = new Vector2(spriteWidth * scaleFactor, spriteHeight * scaleFactor);
            fishImage.rectTransform.anchoredPosition = new Vector2(maxSize / 2.3f, 0f);
            fishImage.rectTransform.localScale = new Vector3(_fishData.isFlip ? -1f : 1f, 1f, 1f);
        }

        private void SetCheckmarkVisible(bool isVisible)
        {
            if (checkmarkImage != null)
            {
                checkmarkImage.gameObject.SetActive(isVisible);
            }
        }

        private void SpawnPreview()
        {
            if (_previewSpawnArea == null || _fishData == null) return;

            DestroyPreviewIfExists();

            _spawnedPreviewObject = new GameObject(_fishData.fishName);
            _spawnedPreviewObject.transform.SetParent(_previewSpawnArea, false);

            Image previewImage = _spawnedPreviewObject.AddComponent<Image>();
            previewImage.sprite = _fishData.fishSprite;
            previewImage.rectTransform.sizeDelta = fishImage.rectTransform.sizeDelta;
            previewImage.rectTransform.localScale = fishImage.rectTransform.localScale;

            Vector2 imageHalfSize = previewImage.rectTransform.sizeDelta * 0.5f;
            Vector2 spawnHalfSize = _previewSpawnArea.rect.size * 0.5f;

            float minX = -spawnHalfSize.x + imageHalfSize.x;
            float maxX = spawnHalfSize.x - imageHalfSize.x;
            float minY = -spawnHalfSize.y + imageHalfSize.y;
            float maxY = spawnHalfSize.y - imageHalfSize.y;

            if (minX > maxX) minX = maxX = 0f;
            if (minY > maxY) minY = maxY = 0f;

            previewImage.rectTransform.anchoredPosition = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );
        }

        private void DestroyPreviewIfExists()
        {
            if (_spawnedPreviewObject != null)
            {
                Destroy(_spawnedPreviewObject);
                _spawnedPreviewObject = null;
            }
        }
    }
}
