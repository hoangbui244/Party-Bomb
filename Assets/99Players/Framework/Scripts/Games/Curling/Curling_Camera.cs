using UnityEngine;
public class Curling_Camera : MonoBehaviour
{
	private enum CameraInfoType
	{
		Thorw,
		Sweep
	}
	private Camera camera;
	private GameObject stone;
	[SerializeField]
	[Header("カメラの状況に合わせた角度")]
	private float[] arrayRotX;
	private Transform originParent;
	private Vector3 originMovePos;
	private readonly float MAX_TEA_DISTANCE_Z = 3.25f;
	public void Init()
	{
		originParent = base.transform.parent;
		originMovePos = base.transform.position;
		camera = GetComponent<Camera>();
	}
	public void InitPlay()
	{
		SetThrowCamera();
	}
	public void LateUpdateMethod()
	{
		Curling_GameManager.State state = SingletonCustom<Curling_GameManager>.Instance.GetState();
		if ((uint)(state - 3) <= 2u)
		{
			base.transform.SetPositionX(originMovePos.x);
			if (base.transform.position.z >= SingletonCustom<Curling_GameManager>.Instance.GetCameraMoveLimitAnchor().position.z)
			{
				base.transform.SetPositionZ(SingletonCustom<Curling_GameManager>.Instance.GetCameraMoveLimitAnchor().position.z);
			}
		}
	}
	public void SetStone(GameObject _stone)
	{
		stone = _stone;
	}
	public void SetNearHouse()
	{
		base.transform.parent = originParent;
		Vector3 position = base.transform.position;
		position.y = base.transform.position.y - 1f;
		position.z = SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos().z;
		LeanTween.move(base.gameObject, position, SingletonCustom<Curling_GameManager>.Instance.GetCameraNearHouseTime()).setEaseOutQuart();
	}
	public void SetThrowCamera()
	{
		base.transform.position = originMovePos;
		base.transform.localEulerAngles = new Vector3(arrayRotX[0], 0f, 0f);
	}
	public void SetSweepCamera(float _posZ, float _changeTime)
	{
		UnityEngine.Debug.Log("SetSweepCamera");
		LeanTween.moveLocalZ(base.gameObject, 0f, _changeTime);
		LeanTween.rotateLocal(base.gameObject, new Vector3(arrayRotX[1], base.transform.localEulerAngles.y, 0f), _changeTime);
	}
	public void SetParent()
	{
		base.transform.parent = stone.transform;
	}
	public Camera GetCamera()
	{
		return camera;
	}
}
