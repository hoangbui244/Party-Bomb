using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
public class Skijump_CameraWorkManager : SingletonCustom<Skijump_CameraWorkManager>
{
	public enum State
	{
		STANDBY,
		SLIDE,
		JUMP,
		AIR_BALANCE,
		LANDING,
		END
	}
	[SerializeField]
	[Header("対象カメラ")]
	private Camera targetCamera;
	[SerializeField]
	[Header("集中線エフェクト")]
	private ParticleSystem psSpeedEffect;
	[SerializeField]
	[Header("対空エフェクト")]
	private ParticleSystem psAirEffect;
	[SerializeField]
	[Header("ブラ\u30fc")]
	private MotionBlur blur;
	private State currentState;
	private readonly Vector3 OFFSET_DEFAULT = new Vector3(0f, 0.325f, -0.475f);
	private readonly Vector3 ANGLE = new Vector3(23.36f, 0f, 0f);
	private Vector3 offset;
	private Vector3 angle;
	private Skijump_Character jumpChara;
	private int anglePattern;
	public void Init()
	{
		offset = OFFSET_DEFAULT;
		angle = ANGLE;
		SetState(State.STANDBY);
	}
	public void SetState(State _state)
	{
		currentState = _state;
		switch (currentState)
		{
		case State.SLIDE:
			break;
		case State.STANDBY:
			anglePattern = UnityEngine.Random.Range(0, 3);
			offset = OFFSET_DEFAULT;
			angle = ANGLE;
			blur.enabled = true;
			break;
		case State.JUMP:
			LeanTween.delayedCall(base.gameObject, 1.25f, (Action)delegate
			{
				psAirEffect.Play();
			});
			break;
		case State.AIR_BALANCE:
			LeanTween.delayedCall(base.gameObject, 0.15f, (Action)delegate
			{
				psAirEffect.Stop();
			});
			break;
		case State.LANDING:
			blur.enabled = false;
			break;
		}
	}
	public void SetSpeedEffect(bool _isEnable)
	{
		if (_isEnable)
		{
			psSpeedEffect.Play();
		}
		else
		{
			psSpeedEffect.Stop();
		}
	}
	private void Update()
	{
		switch (currentState)
		{
		case State.STANDBY:
			switch (anglePattern)
			{
			case 0:
				break;
			case 1:
				offset.x = 0.075f;
				offset.y = OFFSET_DEFAULT.y - 0.05f;
				offset.z = OFFSET_DEFAULT.z + 0.05f;
				break;
			case 2:
				offset.x = -0.075f;
				offset.y = OFFSET_DEFAULT.y - 0.05f;
				offset.z = OFFSET_DEFAULT.z + 0.05f;
				break;
			}
			break;
		case State.SLIDE:
			if (jumpChara != null && jumpChara.Rigid.velocity.magnitude >= 3.5f)
			{
				targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 35.5f, 0.05f);
				offset.y = Mathf.SmoothStep(offset.y, 0.3f, 0.05f);
				offset.z = Mathf.SmoothStep(offset.z, -0.55f, 0.05f);
			}
			break;
		case State.JUMP:
			switch (anglePattern)
			{
			case 0:
				targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 55f, 0.1f);
				offset.y = Mathf.SmoothStep(offset.y, 0.6f, 0.033f);
				offset.z = Mathf.SmoothStep(offset.z, -0.8f, 0.175f);
				angle.x = Mathf.SmoothStep(angle.x, ANGLE.x + 7.5f, 0.05f);
				break;
			case 1:
				targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 55f, 0.1f);
				offset.x = Mathf.SmoothStep(offset.x, 0.2f, 0.033f);
				offset.y = Mathf.SmoothStep(offset.y, 0.8f, 0.033f);
				offset.z = Mathf.SmoothStep(offset.z, -0.8f, 0.175f);
				angle.x = Mathf.SmoothStep(angle.x, ANGLE.x + 15f, 0.05f);
				angle.y = Mathf.SmoothStep(angle.y, -10f, 0.05f);
				break;
			case 2:
				targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 55f, 0.1f);
				offset.x = Mathf.SmoothStep(offset.x, -0.2f, 0.033f);
				offset.y = Mathf.SmoothStep(offset.y, 0.8f, 0.033f);
				offset.z = Mathf.SmoothStep(offset.z, -0.8f, 0.175f);
				angle.x = Mathf.SmoothStep(angle.x, ANGLE.x + 15f, 0.05f);
				angle.y = Mathf.SmoothStep(angle.y, 10f, 0.05f);
				break;
			}
			break;
		case State.AIR_BALANCE:
			targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 57.5f, 0.035f);
			offset.y = Mathf.SmoothStep(offset.y, 0.7f, 0.065f);
			offset.z = Mathf.SmoothStep(offset.z, -0.351f, 0.075f);
			angle.x = Mathf.SmoothStep(angle.x, 60f, 0.07f);
			break;
		case State.LANDING:
			targetCamera.fieldOfView = Mathf.SmoothStep(targetCamera.fieldOfView, 60f, 0.1f);
			offset.y = Mathf.SmoothStep(offset.y, 0.75f, 0.1f);
			offset.z = Mathf.SmoothStep(offset.z, OFFSET_DEFAULT.z - 0.3f, 0.1f);
			angle.x = Mathf.SmoothStep(angle.x, 40f, 0.1f);
			break;
		case State.END:
			offset.y = Mathf.SmoothStep(offset.y, OFFSET_DEFAULT.y, 0.15f);
			offset.z = Mathf.SmoothStep(offset.z, OFFSET_DEFAULT.z - 0.075f, 0.1f);
			angle.x = Mathf.SmoothStep(angle.x, ANGLE.x, 0.15f);
			break;
		}
	}
	public void LateUpdateMethod()
	{
		jumpChara = Skijump_Define.MCM.GetNowJumpChara();
		if (jumpChara.GetPos().z >= Skijump_Define.MSM.GetSlopeEndAnchor().position.z && currentState != State.END)
		{
			SetState(State.END);
		}
		MoveCamera(jumpChara.GetPos(), 0f, angle, offset, _lerp: false);
	}
	private void MoveCamera(Vector3 _targetPos, float _moveSpeed, Vector3 _rot, Vector3 _offset, bool _lerp = true)
	{
		_targetPos += _offset;
		_targetPos.z = Mathf.Min(_targetPos.z, Skijump_Define.MSM.GetSlopeEndAnchor().position.z + 4f);
		CalcManager.mCalcVector3 = targetCamera.transform.localEulerAngles;
		CalcManager.mCalcVector3 = _rot;
		if (_lerp)
		{
			targetCamera.transform.localEulerAngles = Vector3.Lerp(targetCamera.transform.localEulerAngles, CalcManager.mCalcVector3, Mathf.Min(_moveSpeed * Time.deltaTime, 1f));
		}
		else
		{
			targetCamera.transform.localEulerAngles = CalcManager.mCalcVector3;
		}
		CalcManager.mCalcVector3 = _targetPos;
		if (_lerp)
		{
			targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.localPosition, CalcManager.mCalcVector3, Mathf.Min(_moveSpeed * Time.deltaTime, 1f));
		}
		else
		{
			targetCamera.transform.position = CalcManager.mCalcVector3;
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
