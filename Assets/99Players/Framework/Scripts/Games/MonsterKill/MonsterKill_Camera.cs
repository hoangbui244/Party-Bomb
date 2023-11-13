using System;
using UnityEngine;
public class MonsterKill_Camera : MonoBehaviour
{
	private MonsterKill_Player player;
	private Vector3 diffPos;
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	private bool isActive;
	private bool isCameraRotReset;
	[SerializeField]
	[Header("カメラ分割時のカメラの座標")]
	private Vector3 multiCameraPos;
	public void Init(int _playerNo)
	{
		player = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo);
		diffPos = base.transform.position - player.transform.position;
		isActive = true;
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			camera.transform.localPosition = multiCameraPos;
		}
	}
	public void FixedUpdateMethod()
	{
		float num = 0f;
		Vector3 b = player.transform.position + diffPos;
		b.y = base.transform.position.y;
		base.transform.position = Vector3.Lerp(base.transform.position, b, (SingletonCustom<MonsterKill_CameraManager>.Instance.GetFollowSpeed() + num) * Time.deltaTime);
		if (!player.GetIsJump())
		{
			float b2 = player.transform.position.y + diffPos.y;
			base.transform.SetPositionY(Mathf.Lerp(base.transform.position.y, b2, (SingletonCustom<MonsterKill_CameraManager>.Instance.GetFollowSpeed() + num) * Time.deltaTime * 0.25f));
		}
	}
	public void SetCameraRect(Rect _rect)
	{
		camera.rect = _rect;
	}
	public bool GetIsActive()
	{
		return isActive;
	}
	public Camera GetCamera()
	{
		return camera;
	}
	public bool GetIsCameraRotReset()
	{
		return isCameraRotReset;
	}
	public void SetCameraRotReset()
	{
		isCameraRotReset = true;
		float time = Vector3.Angle(base.transform.forward, Vector3.forward) / 180f * SingletonCustom<MonsterKill_CameraManager>.Instance.GetCameraRotResetTime();
		LeanTween.rotateLocal(base.gameObject, Vector3.zero, time).setEaseOutSine().setOnComplete((Action)delegate
		{
			isCameraRotReset = false;
		});
	}
	public void SetCameraRot(Vector3 _vec, bool _isDirect = false)
	{
		if (_isDirect)
		{
			base.transform.localEulerAngles = _vec;
		}
		else
		{
			base.transform.RotateAround(player.transform.position, _vec, SingletonCustom<MonsterKill_CameraManager>.Instance.GetRotSpeed() * Time.deltaTime);
		}
	}
	public Quaternion GetCameraDir()
	{
		return Quaternion.Euler(0f, base.transform.localEulerAngles.y, 0f);
	}
}
