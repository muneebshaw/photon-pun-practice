using Cinemachine;
using UnityEngine;

public class CameraTargetSetter : MonoBehaviour
{
    private CinemachineVirtualCamera _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        if (Time.frameCount % 50 == 0)
        {
            if (!_camera.Follow)
            {
                _camera.Follow = RFMPlayerX.LocalPlayerInstance.GetComponent<RFMPlayerX>().cameraTarget;
            }
        }
    }
}
