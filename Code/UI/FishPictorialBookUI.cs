using _01.Scripts.Fish;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI
{
    public class FishPictorialBookUI : MonoBehaviour
    {
        [field: SerializeField] public FishDataSO fishData { get; private set; }

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI sizeText;

        public void Render(bool isCaught, int maxSize)
        {
            EnsureBindings();
            if (fishData == null || image == null || nameText == null || sizeText == null) return;

            image.sprite = fishData.fishSprite;
            image.color = isCaught ? Color.white : Color.black;
            nameText.SetText(isCaught ? fishData.fishName : "???");
            sizeText.SetText(isCaught ? $"{maxSize}cm" : "-cm");
        }

        private void EnsureBindings()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            if (nameText == null)
            {
                Transform nameTextTransform = transform.Find("NameText");
                if (nameTextTransform != null)
                {
                    nameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (sizeText == null)
            {
                Transform sizeTextTransform = transform.Find("NameText/SizeText");
                if (sizeTextTransform != null)
                {
                    sizeText = sizeTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }
        }
    }
}
