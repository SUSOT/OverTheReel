using Unity.Cinemachine;
using UnityEngine;

namespace _01.Scripts.Player.Components
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera movingCamera;
        [SerializeField] private CinemachineCamera castingCamera;

        public void ChangeCamera(bool isMovingCam)
        {
            if (isMovingCam)
            {
                castingCamera.Priority = 0;
                movingCamera.Priority = 10;
            }
            else
            {
                castingCamera.Priority = 10;
                movingCamera.Priority = 0;
            }
        }
    }
}
