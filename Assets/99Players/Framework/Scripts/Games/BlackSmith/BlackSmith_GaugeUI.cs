using System;
using UnityEngine;
public class BlackSmith_GaugeUI : MonoBehaviour
{
	public enum LineType
	{
		Six,
		Five,
		Four,
		Three,
		Max
	}
	public enum PosType
	{
		Center,
		Left,
		Right,
		Max
	}
	private int playerNo;
	[SerializeField]
	[Header("ゲ\u30fcジパタ\u30fcン")]
	private BlackSmith_GaugeUI_Pattern[] arrayGaugePattern;
	private int prevActivePatternIdx = -1;
	[SerializeField]
	[Header("バ\u30fc")]
	private GameObject bar;
	[SerializeField]
	[Header("ゲ\u30fcジのフェ\u30fcド")]
	private GameObject gaugeFade;
	private int bar_MoveDir;
	private int currentGaugePatternIdx;
	[SerializeField]
	[Header("バ\u30fcの移動制限 Min")]
	private Transform bar_MoveLimit_Min;
	[SerializeField]
	[Header("バ\u30fcの移動制限 Max")]
	private Transform bar_MoveLimit_Max;
	[SerializeField]
	[Header("エフェクトを格納するアンカ\u30fc")]
	private Transform evaluationEffectAnchor;
	private float originPosY;
	private float diffSpeedUp;
	public void Init(int _playerNo)
	{
		playerNo = _playerNo;
		for (int i = 0; i < arrayGaugePattern.Length; i++)
		{
			arrayGaugePattern[i].gameObject.SetActive(value: false);
		}
		currentGaugePatternIdx = 0;
		SetGaugePatternActive();
		SetGaugeFadeActive(_isActive: false);
		bar_MoveDir = 1;
		bar.transform.localPosition = bar_MoveLimit_Min.localPosition;
		originPosY = base.transform.localPosition.y;
	}
	public void UpdateMethod()
	{
		Vector3 localPosition = bar.transform.localPosition;
		float num = 1f;
		if (SingletonCustom<BlackSmith_GameManager>.Instance.GetIsTimeGaugeSpeedUP())
		{
			if (SingletonCustom<BlackSmith_GameManager>.Instance.GetIsGaugeBarSpeedUp())
			{
				if (diffSpeedUp < 1f)
				{
					diffSpeedUp += Time.deltaTime * SingletonCustom<BlackSmith_UIManager>.Instance.GetBarDiffSpeedUp();
					if (diffSpeedUp > 1f)
					{
						diffSpeedUp = 1f;
					}
				}
				num += diffSpeedUp;
			}
		}
		else
		{
			num += SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).GetGaugeSpeedUpValue();
		}
		float num2 = SingletonCustom<BlackSmith_UIManager>.Instance.GetBarMoveSpeed() * num;
		localPosition.x += Time.deltaTime * num2 * (float)bar_MoveDir;
		localPosition.x = Mathf.Clamp(localPosition.x, bar_MoveLimit_Min.localPosition.x, bar_MoveLimit_Max.localPosition.x);
		bar.transform.localPosition = localPosition;
		if (localPosition.x == bar_MoveLimit_Min.localPosition.x || localPosition.x == bar_MoveLimit_Max.localPosition.x)
		{
			bar_MoveDir *= -1;
			currentGaugePatternIdx++;
			SetGaugePatternActive();
		}
	}
	public void SetGaugePatternActive()
	{
		if (prevActivePatternIdx != -1)
		{
			arrayGaugePattern[prevActivePatternIdx].gameObject.SetActive(value: false);
		}
		int arraySortGaugePatternIdx = SingletonCustom<BlackSmith_UIManager>.Instance.GetArraySortGaugePatternIdx(currentGaugePatternIdx % arrayGaugePattern.Length);
		arrayGaugePattern[arraySortGaugePatternIdx].gameObject.SetActive(value: true);
		prevActivePatternIdx = arraySortGaugePatternIdx;
		SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).SetHammerStrileInputCnt(arrayGaugePattern[arraySortGaugePatternIdx].GetPerfectCnt());
	}
	public int GetGaugePattenLength()
	{
		return arrayGaugePattern.Length;
	}
	public void SetGaugeFadeActive(bool _isActive)
	{
		gaugeFade.SetActive(_isActive);
	}
	public void SetGaugeFade(bool _fadeIn)
	{
		if (_fadeIn)
		{
			LeanTween.moveLocalY(base.gameObject, originPosY, SingletonCustom<BlackSmith_UIManager>.Instance.GetGaugeFadeTime());
		}
		else
		{
			LeanTween.moveLocalY(base.gameObject, originPosY - 250f, SingletonCustom<BlackSmith_UIManager>.Instance.GetGaugeFadeTime()).setOnComplete((Action)delegate
			{
				bar_MoveDir = 1;
				bar.transform.localPosition = bar_MoveLimit_Min.localPosition;
				currentGaugePatternIdx++;
				SetGaugePatternActive();
			});
		}
	}
	public int GetBarMoveDir()
	{
		return bar_MoveDir;
	}
	public int GetBarIdx()
	{
		return arrayGaugePattern[prevActivePatternIdx].GetBarIdx(bar.transform.position, bar_MoveDir);
	}
	public bool IsCanPerfectInput()
	{
		return arrayGaugePattern[prevActivePatternIdx].IsCanPerfectInput(bar.transform.position);
	}
	public bool IsPerfectBetweenMinMax()
	{
		return arrayGaugePattern[prevActivePatternIdx].IsPerfectBetweenMinMax(bar.transform.position);
	}
	public bool IsNiceBetweenMinMax()
	{
		return arrayGaugePattern[prevActivePatternIdx].IsNiceBetweenMinMax(bar.transform.position);
	}
	public bool IsCanGoodInput()
	{
		return arrayGaugePattern[prevActivePatternIdx].IsCanGoodInput(bar.transform.position);
	}
	public bool IsGoodBetweenMinMax()
	{
		return arrayGaugePattern[prevActivePatternIdx].IsGoodBetweenMinMax(bar.transform.position);
	}
	public bool IsInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType, float _inputTiming)
	{
		return arrayGaugePattern[prevActivePatternIdx].IsInputTiming(_idx, _evaluationType, _inputTiming, bar.transform.position);
	}
	public float GetInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		return arrayGaugePattern[prevActivePatternIdx].GetInputTiming(_idx, _evaluationType, bar_MoveDir);
	}
	public void PlayEvaluationEffect(int _playerNo, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		BlackSmith_EvaluationEffect blackSmith_EvaluationEffect = null;
		switch (_evaluationType)
		{
		case BlackSmith_PlayerManager.EvaluationType.Bad:
			blackSmith_EvaluationEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetEvaluationBadEffect(playerNo);
			if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
			}
			break;
		case BlackSmith_PlayerManager.EvaluationType.Good:
			blackSmith_EvaluationEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetEvaluationGoodEffect(playerNo);
			if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
			break;
		case BlackSmith_PlayerManager.EvaluationType.Nice:
			blackSmith_EvaluationEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetEvaluationNiceEffect(playerNo);
			if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_nice");
			}
			break;
		case BlackSmith_PlayerManager.EvaluationType.Perfect:
			blackSmith_EvaluationEffect = SingletonCustom<BlackSmith_EffectManager>.Instance.GetEvaluationPerfectEffect(playerNo);
			if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(playerNo).GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
			}
			break;
		}
		blackSmith_EvaluationEffect.transform.parent = evaluationEffectAnchor;
		blackSmith_EvaluationEffect.transform.localPosition = bar.transform.localPosition;
		blackSmith_EvaluationEffect.transform.localEulerAngles = Vector3.zero;
		blackSmith_EvaluationEffect.SetSize(_playerNo);
		blackSmith_EvaluationEffect.Play();
	}
}
