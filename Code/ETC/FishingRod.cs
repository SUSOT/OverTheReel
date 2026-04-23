using UnityEngine;

namespace _01.Scripts.ETC
{
    [RequireComponent(typeof(LineRenderer))]
    public class FishingRod : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform fishingRod;
        [SerializeField] private Transform marker;
        
        [Header("Line Settings")]
        [SerializeField, Min(0f)] private float lineWidth = 0.02f;
        
        
        private LineRenderer _lineRenderer;
        private bool _wasMarkerActive;
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (fishingRod == null)
                Debug.LogError($"[{nameof(FishingRod)}] fishingRod Transform이 할당되지 않았습니다.");
            if (marker == null)
                Debug.LogError($"[{nameof(FishingRod)}] marker Transform이 할당되지 않았습니다.");

            _lineRenderer.startWidth = lineWidth;
            _lineRenderer.endWidth = lineWidth;
            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }

        private void Update()
        {
            bool isActive = marker != null && marker.gameObject.activeInHierarchy;

            if (isActive != _wasMarkerActive)
            {
                _lineRenderer.enabled = isActive;
                _wasMarkerActive = isActive;
            }

            if (isActive)
            {
                _lineRenderer.SetPosition(0, fishingRod.position);
                _lineRenderer.SetPosition(1, marker.position);
            }
        }
    }
}
