using GamepadInput;
using System;
using UnityEngine;
public class BlackSmith_Player : MonoBehaviour
{
	private BlackSmith_PlayerManager.State state;
	[SerializeField]
	[Header("ハンマ\u30fcRoot")]
	private GameObject hammerRoot;
	[SerializeField]
	[Header("ハンマ\u30fcPivot")]
	private GameObject hammerPivot;
	[SerializeField]
	[Header("ハンマ\u30fcメッシュ")]
	private MeshRenderer hammerMesh;
	[SerializeField]
	[Header("ハンマ\u30fcを叩くアンカ\u30fc")]
	private Transform[] arrayHammerStrikeAnchor;
	private int hammerStrikeAnchorIdx;
	private int playerNo;
	private BlackSmith_Define.UserType userType;
	private int npadId;
	private int createWeaponCnt;
	private int hammerStrikeInputCnt;
	private bool isHammerStrileAnim;
	[SerializeField]
	[Header("ハンマ\u30fcで叩いた時のエフェクトを格納するアンカ\u30fc")]
	private Transform hammerStrikeEffectAnchor;
	[SerializeField]
	[Header("武器の作成が終了した時のエフェクトを格納するアンカ\u30fc")]
	private Transform completeEffectAnchor;
	private BlackSmith_Weapon currentCreateWeapon;
	private bool isInitCreate;
	private float gaugeSpeedUpValue;
	private bool isCpu;
	private BlackSmith_AI cpuAI;
	public void Init(int _playerNo, int _userType)
	{
		playerNo = _playerNo;
		userType = (BlackSmith_Define.UserType)_userType;
		isCpu = (userType >= BlackSmith_Define.UserType.CPU_1);
		hammerPivot.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		createWeaponCnt = 0;
		SetState(BlackSmith_PlayerManager.State.CreateWeapon);
		if (GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<BlackSmith_AI>();
			cpuAI.Init(this);
		}
	}
	public void SetHammerMaterial(Material _mat)
	{
		hammerMesh.material = _mat;
	}
	public void UpdateMethod()
	{
		if (state != BlackSmith_PlayerManager.State.HammerStrike || hammerStrikeInputCnt <= 0 || isHammerStrileAnim)
		{
			return;
		}
		if (!GetIsCpu())
		{
			npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
			if (IsHammerStrikeInput())
			{
				HammerStrike();
			}
		}
		else
		{
			cpuAI.UpdateMethod();
		}
	}
	private bool IsHammerStrikeInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A);
	}
	public void HammerStrike()
	{
		hammerStrikeInputCnt--;
		hammerStrikeAnchorIdx = currentCreateWeapon.GetHighWeightIdx();
		PlayHammerStrikeAnimation();
		JudgeEvaluation();
	}
	private void PlayHammerStrikeAnimation()
	{
		LeanTween.cancel(hammerRoot);
		LeanTween.moveLocal(hammerRoot, arrayHammerStrikeAnchor[hammerStrikeAnchorIdx].localPosition, SingletonCustom<BlackSmith_PlayerManager>.Instance.GetHammerStrikeTime());
		LeanTween.cancel(hammerPivot);
		isHammerStrileAnim = true;
		float hammerStrikeTime = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetHammerStrikeTime();
		LeanTween.rotateLocal(hammerPivot, new Vector3(60f, 0f, 0f), hammerStrikeTime).setEaseInBack().setOnComplete((Action)delegate
		{
			PlayHammerStrikeEffect();
		});
		LeanTween.rotateLocal(hammerPivot, new Vector3(0f, 0f, 0f), hammerStrikeTime).setDelay(hammerStrikeTime);
		float hammerStrikeAnimationTime = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetHammerStrikeAnimationTime();
		LeanTween.delayedCall(base.gameObject, hammerStrikeAnimationTime, (Action)delegate
		{
			isHammerStrileAnim = false;
		});
	}
	private void PlayHammerStrikeEffect()
	{
		ParticleSystem hammerStrikeEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetHammerStrikeEffect(playerNo);
		hammerStrikeEffect.transform.parent = hammerStrikeEffectAnchor;
		hammerStrikeEffect.transform.localPosition = Vector3.zero;
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_blacksmith_hammer_strike");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
		hammerStrikeEffect.Play();
	}
	private void JudgeEvaluation()
	{
		BlackSmith_PlayerManager.EvaluationType evaluationType = BlackSmith_PlayerManager.EvaluationType.Max;
		if (SingletonCustom<BlackSmith_UIManager>.Instance.IsPerfectBetweenMinMax(playerNo))
		{
			evaluationType = BlackSmith_PlayerManager.EvaluationType.Perfect;
		}
		else if (SingletonCustom<BlackSmith_UIManager>.Instance.IsNiceBetweenMinMax(playerNo))
		{
			evaluationType = BlackSmith_PlayerManager.EvaluationType.Nice;
		}
		else if (SingletonCustom<BlackSmith_UIManager>.Instance.IsGoodBetweenMinMax(playerNo))
		{
			evaluationType = BlackSmith_PlayerManager.EvaluationType.Good;
		}
		else
		{
			evaluationType = BlackSmith_PlayerManager.EvaluationType.Bad;
		}
		if (!SingletonCustom<BlackSmith_GameManager>.Instance.GetIsTimeGaugeSpeedUP())
		{
			switch (evaluationType)
			{
			case BlackSmith_PlayerManager.EvaluationType.Bad:
			case BlackSmith_PlayerManager.EvaluationType.Good:
				gaugeSpeedUpValue -= 0.1f;
				if (gaugeSpeedUpValue < 0f)
				{
					gaugeSpeedUpValue = 0f;
				}
				break;
			case BlackSmith_PlayerManager.EvaluationType.Nice:
			case BlackSmith_PlayerManager.EvaluationType.Perfect:
				gaugeSpeedUpValue += 0.05f;
				if (gaugeSpeedUpValue > 1f)
				{
					gaugeSpeedUpValue = 1f;
				}
				break;
			}
		}
		SingletonCustom<BlackSmith_UIManager>.Instance.PlayEvaluationEffect(playerNo, evaluationType);
		float hammerStrikeTime = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetHammerStrikeTime();
		LeanTween.delayedCall(base.gameObject, hammerStrikeTime, (Action)delegate
		{
			currentCreateWeapon.AddCreateNeedPoint(evaluationType, hammerStrikeAnchorIdx);
			if (currentCreateWeapon.IsCreateWeapon())
			{
				SetState(BlackSmith_PlayerManager.State.CreateWeaponComplete);
			}
		});
	}
	public BlackSmith_PlayerManager.State GetState()
	{
		return state;
	}
	private void SetState(BlackSmith_PlayerManager.State _state)
	{
		state = _state;
		switch (state)
		{
		case BlackSmith_PlayerManager.State.HammerStrike:
			break;
		case BlackSmith_PlayerManager.State.CreateWeapon:
			CreateWeapon();
			break;
		case BlackSmith_PlayerManager.State.CreateWeaponComplete:
			CreateWeaponComplete();
			break;
		}
	}
	public void CreateWeapon()
	{
		currentCreateWeapon = UnityEngine.Object.Instantiate(SingletonCustom<BlackSmith_WeaponManager>.Instance.GetWeapon(createWeaponCnt));
		currentCreateWeapon.Init(playerNo);
		if (!isInitCreate)
		{
			currentCreateWeapon.transform.parent = SingletonCustom<BlackSmith_WeaponManager>.Instance.GetCreateWeaponAnchor(playerNo);
			currentCreateWeapon.transform.localPosition = Vector3.zero;
			SetState(BlackSmith_PlayerManager.State.HammerStrike);
			isInitCreate = true;
		}
		else
		{
			currentCreateWeapon.transform.parent = SingletonCustom<BlackSmith_WeaponManager>.Instance.GetFadeInAnchor(playerNo);
			currentCreateWeapon.transform.localPosition = Vector3.zero;
			SingletonCustom<BlackSmith_WeaponManager>.Instance.PlayFadeInAnimation(playerNo, delegate
			{
				SingletonCustom<BlackSmith_UIManager>.Instance.SetGaugeFade(playerNo, _fadeIn: true);
			}, delegate
			{
				currentCreateWeapon.transform.parent = SingletonCustom<BlackSmith_WeaponManager>.Instance.GetCreateWeaponAnchor(playerNo);
				SingletonCustom<BlackSmith_UIManager>.Instance.SetGaugeFadeActive(playerNo, _isActive: false);
				SingletonCustom<BlackSmith_UIManager>.Instance.SetCreatePercent(playerNo, 0);
				SetState(BlackSmith_PlayerManager.State.HammerStrike);
			});
		}
	}
	public void CreateWeaponComplete()
	{
		SingletonCustom<BlackSmith_UIManager>.Instance.SetGaugeFadeActive(playerNo, _isActive: true);
		LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
		{
			ParticleSystem createWeaponCompleteEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetCreateWeaponCompleteEffect(playerNo);
			createWeaponCompleteEffect.transform.parent = completeEffectAnchor;
			createWeaponCompleteEffect.transform.localPosition = Vector3.zero;
			if (!GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult");
			}
			createWeaponCompleteEffect.Play();
			SingletonCustom<BlackSmith_UIManager>.Instance.SetGaugeFade(playerNo, _fadeIn: false);
		});
		createWeaponCnt++;
		SingletonCustom<BlackSmith_UIManager>.Instance.SetCreateWeaponCnt(playerNo, createWeaponCnt);
		GameObject prevCreateWeaponObj = currentCreateWeapon.gameObject;
		prevCreateWeaponObj.transform.parent = SingletonCustom<BlackSmith_WeaponManager>.Instance.GetFadeOutAnchor(playerNo);
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<BlackSmith_WeaponManager>.Instance.PlayFadeOutAnimation(playerNo, delegate
			{
				UnityEngine.Object.Destroy(prevCreateWeaponObj);
			});
			SetState(BlackSmith_PlayerManager.State.CreateWeapon);
		});
	}
	public int GetPlayerNo()
	{
		return playerNo;
	}
	public BlackSmith_Define.UserType GetUserType()
	{
		return userType;
	}
	public bool GetIsCpu()
	{
		return isCpu;
	}
	public int GetCreateWeaponCnt()
	{
		return createWeaponCnt;
	}
	public void SetCreateWeaponCnt(int _createWeaponCnt)
	{
		createWeaponCnt = _createWeaponCnt;
	}
	public int GetHammerStrileInputCnt()
	{
		return hammerStrikeInputCnt;
	}
	public float GetGaugeSpeedUpValue()
	{
		return gaugeSpeedUpValue;
	}
	public void SetHammerStrileInputCnt(int _inputCnt)
	{
		hammerStrikeInputCnt = _inputCnt;
		if (GetIsCpu())
		{
			cpuAI.SetInputEvaluationList();
		}
	}
	private void OnDrawGizmos()
	{
		for (int i = 0; i < arrayHammerStrikeAnchor.Length; i++)
		{
			switch (i)
			{
			case 0:
				Gizmos.color = Color.white;
				break;
			case 1:
				Gizmos.color = Color.green;
				break;
			case 2:
				Gizmos.color = Color.yellow;
				break;
			}
			Gizmos.DrawWireSphere(arrayHammerStrikeAnchor[i].transform.position, 0.1f);
		}
	}
}
