using UnityEngine;
public class BlackSmith_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("カメラ（エフェクト用）")]
	private Camera camera_Effect;
	public void SetCameraRect(Rect _rect)
	{
		camera.rect = _rect;
		camera_Effect.rect = _rect;
	}
	public Vector3 GetCameraRot()
	{
		return camera.transform.localEulerAngles;
	}
}
