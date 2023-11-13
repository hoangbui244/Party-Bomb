using GamepadInput;
using UnityEngine;
public class Golf_Player : MonoBehaviour
{
	private Golf_Character character;
	[SerializeField]
	[Header("AnimationEventor")]
	private Golf_AnimationEventor animationEventor;
	private int playerNo;
	private Golf_Define.UserType userType;
	private int npadId;
	private float originRotY;
	private int point;
	private Vector3 stickDir;
	private Golf_AI cpuAI;
	public void Init(int _playerNo, int _userType)
	{
		playerNo = _playerNo;
		userType = (Golf_Define.UserType)_userType;
		character = GetComponent<Golf_Character>();
		character.Init(this);
		character.SetStyle(_userType);
		animationEventor.Init(this);
		if (GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<Golf_AI>();
			cpuAI.Init(this);
		}
	}
	public void SetClubMaterial(Material _mat)
	{
		character.SetClubMaterial(_mat);
	}
	public void InitPlay()
	{
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		Vector3 readyCharaPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyCharaPos();
		base.transform.position = readyCharaPos;
		Vector3 forward = readyBallPos - readyCharaPos;
		forward.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(forward);
		base.transform.rotation = rotation;
		originRotY = base.transform.localEulerAngles.y;
		base.gameObject.SetActive(value: true);
	}
	public void InitPlayCPU()
	{
		cpuAI.InitPlay();
	}
	public void SetAudience()
	{
		base.gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		if (!GetIsCpu())
		{
			npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
			switch (SingletonCustom<Golf_GameManager>.Instance.GetState())
			{
			case Golf_GameManager.State.SHOT_READY:
				ShotReady();
				break;
			case Golf_GameManager.State.SHOT_POWER:
				ShotPower();
				break;
			case Golf_GameManager.State.SHOT_IMPACT:
				ShotVec();
				break;
			}
		}
		else
		{
			cpuAI.UpdateMethod();
		}
	}
	private void ShotReady()
	{
		stickDir = GetStickDir();
		if (stickDir != Vector3.zero)
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
			Vector3 vector = new Vector3(0f, stickDir.x, 0f);
			base.transform.RotateAround(readyBallPos, vector, SingletonCustom<Golf_PlayerManager>.Instance.GetShotReadyRotSpeed() * Time.deltaTime);
			if (base.transform.localEulerAngles.y > originRotY + SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle())
			{
				base.transform.localPosition = localPosition;
				base.transform.SetLocalEulerAnglesY(originRotY + SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle());
				vector = Vector3.zero;
			}
			else if (base.transform.localEulerAngles.y < originRotY - SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle())
			{
				base.transform.localPosition = localPosition;
				base.transform.SetLocalEulerAnglesY(originRotY - SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle());
				vector = Vector3.zero;
			}
			if (vector != Vector3.zero)
			{
				SingletonCustom<Golf_CameraManager>.Instance.ShotReadyRot(vector);
			}
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			SetShotReady();
		}
	}
	public void SetShotReady()
	{
		SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.SHOT_POWER);
		SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
		animationEventor.PlaySwingIdleAnimation();
	}
	public void ResetShotReady()
	{
		SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.RESET_SHOT_READY);
		SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
		animationEventor.PlayIdleAnimation();
	}
	private void ShotPower()
	{
		stickDir = GetStickDir();
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			SetShotPower();
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.B))
		{
			ResetShotReady();
		}
	}
	public void SetShotPower(bool _isPowerSkip = false)
	{
		SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.SHOT_IMPACT);
		float value = (!_isPowerSkip) ? SingletonCustom<Golf_UIManager>.Instance.GetShotPowerLerp(_isInput: true) : cpuAI.GetShotPowerLerp();
		value = Mathf.Clamp(value, 0.2f, 1f);
		float shotPower = SingletonCustom<Golf_PlayerManager>.Instance.GetBaseShotPower() * value;
		SingletonCustom<Golf_BallManager>.Instance.SetShotPower(shotPower);
	}
	private void ShotVec()
	{
		stickDir = GetStickDir();
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			SetShotVec();
			SetSwingAnimation();
		}
	}
	public void SetShotVec(bool _isShotReadySkip = false, bool _isImpactSkip = false)
	{
		float value = (!_isImpactSkip) ? SingletonCustom<Golf_UIManager>.Instance.GetImpactDiff(_isInput: true) : cpuAI.GetShotImpctLerp();
		value = Mathf.Clamp(value, 0f, 1f);
		Vector3 a = UnityEngine.Random.insideUnitCircle;
		Vector3 b = a * (SingletonCustom<Golf_PlayerManager>.Instance.GetMaxShotImpactDiff() * value);
		Vector3 vector = Quaternion.Euler(new Vector3(SingletonCustom<Golf_PlayerManager>.Instance.GetShotAngle(), 0f, 0f) + b) * ((!_isShotReadySkip) ? (-base.transform.right) : cpuAI.GetShotVec());
		SingletonCustom<Golf_BallManager>.Instance.SetShotVec(vector.normalized);
		SingletonCustom<Golf_BallManager>.Instance.SetRotDir(a * value + ((!_isShotReadySkip) ? stickDir : cpuAI.GetHitPointDir()));
	}
	public void Skip()
	{
		switch (SingletonCustom<Golf_GameManager>.Instance.GetState())
		{
		case Golf_GameManager.State.SHOT_READY:
			SetShotPower(_isPowerSkip: true);
			SetShotVec(_isShotReadySkip: true, _isImpactSkip: true);
			Shot(_isSkip: true);
			break;
		case Golf_GameManager.State.SHOT_POWER:
			SetShotPower(_isPowerSkip: true);
			SetShotVec(_isShotReadySkip: false, _isImpactSkip: true);
			Shot(_isSkip: true);
			break;
		case Golf_GameManager.State.SHOT_IMPACT:
			SetShotVec(_isShotReadySkip: false, _isImpactSkip: true);
			Shot(_isSkip: true);
			break;
		}
	}
	public void SetSwingAnimation()
	{
		SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.SWING_ANIMATION);
		animationEventor.PlaySwingAnimation();
	}
	public void Shot(bool _isSkip = false)
	{
		SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.BALL_FLY);
		if (!_isSkip)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_golf_shot");
		}
		SingletonCustom<Golf_BallManager>.Instance.Shot();
	}
	public void ResetAnimation()
	{
		animationEventor.PlayIdleAnimation();
	}
	public int GetPlayerNo()
	{
		return playerNo;
	}
	public Golf_Define.UserType GetUserType()
	{
		return userType;
	}
	public bool GetIsCpu()
	{
		return userType >= Golf_Define.UserType.CPU_1;
	}
	public float GetOriginRotY()
	{
		return originRotY;
	}
	public void SetStickDir(Vector3 _stickDir)
	{
		stickDir = _stickDir;
	}
	public int GetPoint()
	{
		return point;
	}
	public void AddPoint(int _point)
	{
		point += _point;
	}
	public void SetPoint(int _point)
	{
		point = _point;
	}
	private Vector3 GetStickDir()
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		mVector3Zero = new Vector3(num, num2, 0f);
		Vector3 vec = mVector3Zero;
		if (mVector3Zero.sqrMagnitude < 0.0400000028f)
		{
			mVector3Zero = Vector3.zero;
			vec = mVector3Zero;
		}
		else
		{
			mVector3Zero = mVector3Zero.normalized;
		}
		Golf_GameManager.State state = SingletonCustom<Golf_GameManager>.Instance.GetState();
		if ((uint)(state - 3) <= 1u)
		{
			SingletonCustom<Golf_UIManager>.Instance.MoveHitPoint(vec);
		}
		return mVector3Zero;
	}
}
