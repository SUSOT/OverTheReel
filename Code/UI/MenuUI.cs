using _01.Scripts.Core.EventSystems;
using _01.Scripts.Core.EventSystems.UIEvent;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class MenuUI : MonoBehaviour
    {
        private static readonly Vector3 OpenPanelPosition = new Vector3(600f, -170f, 0f);
        private static readonly Vector3 ClosePanelPosition = new Vector3(600f, -890f, 0f);
        private static readonly Vector3 OpenButtonRotation = new Vector3(0f, 0f, 180f);
        private static readonly Vector3 CloseButtonRotation = Vector3.zero;

        [SerializeField] private RectTransform panel;
        [SerializeField] private RectTransform button;
        private bool _isOpen;


        [SerializeField] private GameEventChannelSO menuUiChannel;
        [SerializeField] private GameEventChannelSO playerChannel;


        private void Awake()
        {
            menuUiChannel.AddListener<MenuUIOpenEvent>(ChangeOpenValue);
        }

        private void OnDestroy()
        {
            menuUiChannel.RemoveListener<MenuUIOpenEvent>(ChangeOpenValue);
        }

        private void ChangeOpenValue(MenuUIOpenEvent evt)
        {
            _isOpen = evt.isOpen;
            if (!evt.isOpen)
            {
                CloseUI();
            }
        }

        public void OpenAndCloseUI()
        {
            if (_isOpen)
            {
                AnimateUI(OpenPanelPosition, OpenButtonRotation, 1f);
            }
            else
            {
                AnimateUI(ClosePanelPosition, CloseButtonRotation, 1f);
            }

            _isOpen = !_isOpen;
            SendEvent(_isOpen);
        }

        private void CloseUI()
        {
            AnimateUI(ClosePanelPosition, CloseButtonRotation, 0.3f);
            _isOpen = false;
        }

        private void AnimateUI(Vector3 panelPosition, Vector3 buttonRotation, float duration)
        {
            panel.DOAnchorPos3D(panelPosition, duration).SetEase(Ease.InSine);
            button.DOLocalRotate(buttonRotation, duration).SetEase(Ease.InSine);
        }

        private void SendEvent(bool isCanInput)
        {
            playerChannel.RaiseEvent(PlayerEvent.CreatePlayerInputEvent(isCanInput));
        }
    }
}

