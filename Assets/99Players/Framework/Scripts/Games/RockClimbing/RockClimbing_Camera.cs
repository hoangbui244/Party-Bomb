using UnityEngine;
public class RockClimbing_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("開始アンカ\u30fc")]
	private Transform startAnchor;
	[SerializeField]
	[Header("左側の制限アンカ\u30fc")]
	private Transform leftLimitAnchor;
	[SerializeField]
	[Header("右側の制限アンカ\u30fc")]
	private Transform rightLimitAnchor;
	private Vector3 diffVec;
	private GameObject target;
	private bool isStop;
	public void SetDiffVec(int _playerNo)
	{
		target = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(_playerNo).gameObject;
		diffVec = target.transform.position - startAnchor.position;
	}
	public void SetStartAnchorPositionZ(float _posZ)
	{
		startAnchor.SetLocalPositionZ(_posZ);
	}
	public void SetRect(Rect _rect)
	{
		camera.rect = _rect;
	}
	public Camera GetCamera()
	{
		return camera;
	}
	public void SetIsStop()
	{
		isStop = true;
	}
	public bool GetIsStop()
	{
		return isStop;
	}
	public void GameStartAnimation()
	{
		LeanTween.moveLocal(base.gameObject, startAnchor.localPosition, 0.5f);
	}
	public void GoalAnimation(Vector3 _pos)
	{
		LeanTween.moveLocal(base.gameObject, _pos, 1f).setEaseOutQuad();
	}
	public void LateUpdateMethod()
	{
		Vector3 b = target.transform.position - diffVec;
		base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * SingletonCustom<RockClimbing_CameraManager>.Instance.GetCameraSpeed());
		if (base.transform.position.x < leftLimitAnchor.position.x)
		{
			base.transform.SetPositionX(leftLimitAnchor.position.x);
		}
		else if (base.transform.position.x > rightLimitAnchor.position.x)
		{
			base.transform.SetPositionX(rightLimitAnchor.position.x);
		}
	}
}
