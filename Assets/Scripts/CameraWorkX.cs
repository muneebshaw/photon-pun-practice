using UnityEngine;

public class CameraWorkX : MonoBehaviour
{
	public void OnStartFollowing(Transform target)
	{
		CameraTargetSetter.Instance.SetTarget(target);
	}
}