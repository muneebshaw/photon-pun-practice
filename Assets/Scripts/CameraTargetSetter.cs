using Cinemachine;
using UnityEngine;

public class CameraTargetSetter : MonoBehaviour
{
    public static CameraTargetSetter Instance;
    private CinemachineVirtualCamera _camera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetTarget(Transform target)
    {
        _camera.Follow = target;
    }
}
