using UnityEngine;
public class Golf_CameraManager : SingletonCustom<Golf_CameraManager>
{
	[SerializeField]
	[Header("カメラクラス")]
	private Golf_Camera camera;
	[SerializeField]
	[Header("カメラを回転させるかどうかの角度")]
	private float CAMERA_VIEW_CUP_LINE_ROT_ANGLE;
	[SerializeField]
	[Header("カメラを回転させる時の速度")]
	private float CAMERA_VIEW_CUP_LINE_ROT_SPEED;
	[SerializeField]
	[Header("カメラを回転させるまでの待機時間")]
	private float CAMERA_VIEW_CUP_ROT_WAIT_TIME;
	[SerializeField]
	[Header("カメラを回転させる時の時間")]
	private float CAMERA_VIEW_CUP_ROT_TIME;
	[SerializeField]
	[Header("カメラの方向とカップとボ\u30fcルのベクトルが逆方向にある時のカップとカメラとの距離")]
	private float CAMERA_REVERSE_ROT_DISTANCE;
	[SerializeField]
	[Header("カメラの方向とカップとボ\u30fcルのベクトルが逆方向にある時の回転角度")]
	private float CAMERA_REVERSE_ROT_ANGLE;
	[SerializeField]
	[Header("カメラの移動速度")]
	private float CAMERA_MOVE_SPEED;
	[SerializeField]
	[Header("ボ\u30fcルを見る時の補正座標")]
	private Vector3 LOOK_BALL_DIFF_POS;
	public void Init()
	{
		camera.Init();
		InitPlay();
	}
	public void InitPlay()
	{
		camera.InitPlay();
	}
	public void ShotReadyRot(Vector3 _rot, float _diffRotSpeed = 1f)
	{
		camera.ShotReadyRot(_rot, _diffRotSpeed);
	}
	public void FixedUpdateMethod()
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() < Golf_GameManager.State.VIEW_CUP_ROT_CAMERA)
		{
			camera.FixedUpdateMethod();
		}
	}
	public bool CheckViewCupRot()
	{
		return camera.CheckViewCupRot();
	}
	public void SetViewCupRotCamera()
	{
		camera.SetViewCupRotCamera();
	}
	public float GetCameraMoveSpeed()
	{
		return CAMERA_MOVE_SPEED;
	}
	public Vector3 GetLookBallDiffPos()
	{
		return LOOK_BALL_DIFF_POS;
	}
	public float GetCameraViewCupLineRotAngle()
	{
		return CAMERA_VIEW_CUP_LINE_ROT_ANGLE;
	}
	public float GetCameraViewCupLineRotSpeed()
	{
		return CAMERA_VIEW_CUP_LINE_ROT_SPEED;
	}
	public float GetCameraViewCupRotWaitTime()
	{
		return CAMERA_VIEW_CUP_ROT_WAIT_TIME;
	}
	public float GetCameraViewCupRotTime()
	{
		return CAMERA_VIEW_CUP_ROT_TIME;
	}
	public float GetCameraReverseRotDistance()
	{
		return CAMERA_REVERSE_ROT_DISTANCE;
	}
	public float GetCameraReverseRotAngle()
	{
		return CAMERA_REVERSE_ROT_ANGLE;
	}
	public Golf_Camera GetCamera()
	{
		return camera;
	}
}
