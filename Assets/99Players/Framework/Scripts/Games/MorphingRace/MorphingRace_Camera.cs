using Cinemachine;
using Cinemachine.PostFX;
using System;
using UnityEngine;
public class MorphingRace_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("VirtualCamera")]
	private CinemachineVirtualCamera virtualCamera;
	private CinemachineTransposer transposer;
	private CinemachinePostProcessing postProcessing;
	public void Init()
	{
		transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		postProcessing = virtualCamera.GetComponent<CinemachinePostProcessing>();
		postProcessing.enabled = false;
	}
	public void SetCameraLayer(int _playerNo)
	{
		camera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (_playerNo + 1).ToString());
		virtualCamera.gameObject.layer = LayerMask.NameToLayer("Collision_Obj_" + (_playerNo + 1).ToString());
	}
	public void SetRect(Rect _rect)
	{
		camera.rect = _rect;
	}
	public void UpdateMethod()
	{
		bool enabled = camera.transform.position.y < 0f;
		postProcessing.enabled = enabled;
	}
	public void SetFollowOffset(float _angle, Vector3 _followOffset)
	{
		Vector3 localEulerAngles = virtualCamera.transform.localEulerAngles;
		localEulerAngles.x = _angle;
		LeanTween.rotateLocal(virtualCamera.gameObject, localEulerAngles, 0.5f).setEaseOutCubic();
		LeanTween.value(base.gameObject, transposer.m_FollowOffset, _followOffset, 0.5f).setOnUpdate(delegate(Vector3 _value)
		{
			transposer.m_FollowOffset = _value;
		}).setEaseOutCubic();
	}
	public void AfterGoalAnimation(Vector3 _rot, Vector3 _followOffset)
	{
		LeanTween.value(base.gameObject, transposer.m_FollowOffset, _followOffset, 0.75f).setOnUpdate(delegate(Vector3 _value)
		{
			transposer.m_FollowOffset = _value;
		}).setEaseOutCubic();
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			LeanTween.value(base.gameObject, virtualCamera.transform.localEulerAngles, _rot, 0.5f).setOnUpdate(delegate(Vector3 _value)
			{
				virtualCamera.transform.localEulerAngles = _value;
			}).setEaseOutCubic();
		});
	}
}
