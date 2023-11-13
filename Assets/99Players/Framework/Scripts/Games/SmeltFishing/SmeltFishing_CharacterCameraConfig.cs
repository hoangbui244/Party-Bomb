using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CharacterCameraConfig : ScriptableObject
{
	[Header("移動中の視点設定")]
	[SerializeField]
	[DisplayName("カメラの相対位置")]
	private Vector3 perspectiveCameraPosition;
	[SerializeField]
	[DisplayName("カメラの注視角度")]
	private Vector3 perspectiveCameraEulerAngles;
	[SerializeField]
	[DisplayName("カメラの追従速度")]
	private float followDistanceDelta = 10f;
	[Header("釣り中の視点設定")]
	[SerializeField]
	[DisplayName("カメラの相対位置")]
	private Vector3 contactCameraPosition;
	[SerializeField]
	[DisplayName("カメラの注視角度")]
	private Vector3 contactCameraEulerAngles;
	[Header("カメラの遷移設定")]
	[SerializeField]
	[DisplayName("カメラの遷移時間")]
	private float transitionDuration = 0.1f;
	public Vector3 PerspectiveCameraPosition => perspectiveCameraPosition;
	public Vector3 PerspectiveCameraEulerAngles => perspectiveCameraEulerAngles;
	public float FollowDistanceDelta => followDistanceDelta;
	public Vector3 ContactCameraPosition => contactCameraPosition;
	public Vector3 ContactCameraEulerAngles => contactCameraEulerAngles;
	public float TransitionDuration => transitionDuration;
}
