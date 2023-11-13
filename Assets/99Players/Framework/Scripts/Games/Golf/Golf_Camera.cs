using System;
using UnityEngine;
public class Golf_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("カメラ回転アンカ\u30fc")]
	private Transform cameraRotAnchor;
	[SerializeField]
	[Header("カメラ")]
	private Camera cameraObj;
	private Vector3 originPos;
	private Quaternion originRot;
	private Transform originParent;
	private Vector3 originDiffBallToCameraPos;
	private Vector3 diffBallToCameraPos;
	private float diffSpeed;
	private float viewCupRotDir;
	public void Init()
	{
		originParent = cameraObj.transform.parent;
		cameraRotAnchor.localPosition = Vector3.zero;
		cameraRotAnchor.localEulerAngles = Vector3.zero;
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		float d = CalcManager.Length(readyBallPos, cameraObj.transform.position);
		Vector3 position = readyBallPos + SingletonCustom<Golf_FieldManager>.Instance.GetTeeMarkCenterToBallVec() * d;
		position.y = cameraObj.transform.position.y;
		cameraObj.transform.position = position;
		diffBallToCameraPos = readyBallPos - cameraObj.transform.position;
		originDiffBallToCameraPos = diffBallToCameraPos;
		Quaternion rotation = Quaternion.LookRotation(readyBallPos + SingletonCustom<Golf_CameraManager>.Instance.GetLookBallDiffPos() - cameraObj.transform.position);
		cameraObj.transform.rotation = rotation;
		originPos = cameraObj.transform.position;
		originRot = cameraObj.transform.rotation;
	}
	public void InitPlay()
	{
		cameraObj.transform.parent = originParent;
		cameraRotAnchor.localPosition = Vector3.zero;
		cameraRotAnchor.localEulerAngles = Vector3.zero;
		cameraObj.transform.position = originPos;
		cameraObj.transform.rotation = originRot;
		diffBallToCameraPos = originDiffBallToCameraPos;
		diffSpeed = 1f;
	}
	public void ShotReadyRot(Vector3 _rot, float _diffRotSpeed = 1f)
	{
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		cameraObj.transform.RotateAround(readyBallPos, _rot, SingletonCustom<Golf_PlayerManager>.Instance.GetShotReadyRotSpeed() * Time.deltaTime * _diffRotSpeed);
		Quaternion rotation = Quaternion.LookRotation(readyBallPos + SingletonCustom<Golf_CameraManager>.Instance.GetLookBallDiffPos() - cameraObj.transform.position);
		cameraObj.transform.rotation = rotation;
		diffBallToCameraPos = readyBallPos - cameraObj.transform.position;
	}
	public void FixedUpdateMethod()
	{
		Golf_Ball turnPlayerBall = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		if (turnPlayerBall.GetIsOB() && SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.CALC_POINT && diffSpeed > 0f)
		{
			diffSpeed -= Time.fixedDeltaTime * 5f;
			if (diffSpeed < 0f)
			{
				diffSpeed = 0f;
			}
		}
		Vector3 b = turnPlayerBall.transform.position - diffBallToCameraPos;
		cameraObj.transform.position = Vector3.Lerp(cameraObj.transform.position, b, Time.fixedDeltaTime * SingletonCustom<Golf_CameraManager>.Instance.GetCameraMoveSpeed() * diffSpeed);
	}
	public bool CheckViewCupRot()
	{
		Vector3 forward = cameraObj.transform.forward;
		forward.y = 0f;
		Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
		Vector3 to = cupPos - cameraObj.transform.position;
		to.y = 0f;
		Vector3 rhs = cupPos - SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
		rhs.y = 0f;
		float num = Vector3.Angle(forward, to);
		UnityEngine.Debug.Log("angle " + num.ToString());
		float num2 = Vector3.Dot(forward, rhs);
		UnityEngine.Debug.Log("dot " + num2.ToString());
		if (num2 >= 0f)
		{
			UnityEngine.Debug.Log("カメラの方向とカップとボ\u30fcルのベクトルが同じ方向");
			UnityEngine.Debug.Log("角度が一定角度以内かどうか (true : 回転させない false : 回転させる) " + (num < SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupLineRotAngle()).ToString());
			return num < SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupLineRotAngle();
		}
		UnityEngine.Debug.Log("カメラの方向とカップとボ\u30fcルのベクトルが違う方向");
		float num3 = CalcManager.Length(cupPos, cameraObj.transform.position);
		UnityEngine.Debug.Log("cupToCameraDistance " + num3.ToString());
		if (num3 >= SingletonCustom<Golf_CameraManager>.Instance.GetCameraReverseRotDistance())
		{
			UnityEngine.Debug.Log("カップとカメラの距離が一定距離以上");
			UnityEngine.Debug.Log("角度が一定角度以内かどうか (true : 回転させない false : 回転させる) " + (num < SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupLineRotAngle()).ToString());
			return num < SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupLineRotAngle();
		}
		UnityEngine.Debug.Log("カップとカメラの距離が一定距離以内");
		UnityEngine.Debug.Log("false : 回転させる");
		return false;
	}
	public void SetViewCupRotCamera()
	{
		LeanTween.delayedCall(base.gameObject, SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupRotWaitTime(), (Action)delegate
		{
			Golf_Camera golf_Camera = this;
			cameraRotAnchor.transform.position = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
			cameraObj.transform.parent = cameraRotAnchor;
			Vector3 forward = cameraObj.transform.forward;
			forward.y = 0f;
			Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
			Vector3 vector = cupPos - cameraObj.transform.position;
			vector.y = 0f;
			Vector3 rhs = cupPos - SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall().transform.position;
			rhs.y = 0f;
			float num = CalcManager.Length(cupPos, cameraObj.transform.position);
			UnityEngine.Debug.Log("cupToCameraDistance " + num.ToString());
			float diffAngle = Mathf.Abs(SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupLineRotAngle() - Vector3.Angle(forward, vector));
			if (Vector3.Dot(forward, rhs) < 0f && num < SingletonCustom<Golf_CameraManager>.Instance.GetCameraReverseRotDistance())
			{
				UnityEngine.Debug.Log("角度を補正\u3000前" + diffAngle.ToString());
				diffAngle = SingletonCustom<Golf_CameraManager>.Instance.GetCameraReverseRotAngle();
				UnityEngine.Debug.Log("角度を補正\u3000後" + diffAngle.ToString());
			}
			if (Vector3.Cross(forward, vector).y < 0f)
			{
				diffAngle *= -1f;
			}
			UnityEngine.Debug.Log("diffAngle " + diffAngle.ToString());
			LeanTween.value(base.gameObject, 0f, 1f, SingletonCustom<Golf_CameraManager>.Instance.GetCameraViewCupRotTime()).setOnUpdate(delegate(float _value)
			{
				golf_Camera.cameraRotAnchor.SetLocalEulerAnglesY(_value * diffAngle);
			}).setOnComplete((Action)delegate
			{
				SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.VIEW_CUP_LINE);
			});
		});
	}
	public Camera GetCameraObj()
	{
		return cameraObj;
	}
}
