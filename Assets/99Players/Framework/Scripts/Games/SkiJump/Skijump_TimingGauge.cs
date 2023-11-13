using System;
using System.Collections;
using UnityEngine;
public class Skijump_TimingGauge : MonoBehaviour
{
	public enum State
	{
		NONE,
		SHOW,
		MOVE,
		MOVE_STOP,
		HIDE,
		GAUGE_SHOW,
		GAUGE_HIDE,
		MAX
	}
	[SerializeField]
	[Header("ゲ\u30fcジアンカ\u30fc")]
	private GameObject gaugeAnchor;
	[SerializeField]
	[Header("ゲ\u30fcジ背景")]
	private SpriteRenderer gaugeBack;
	[SerializeField]
	[Header("ハ\u30fcフェクト範囲")]
	private SpriteRenderer perfectArea;
	[SerializeField]
	[Header("グッド範囲")]
	private SpriteRenderer goodArea;
	[SerializeField]
	[Header("エフェクトアンカ\u30fc")]
	private GameObject effectAnchor;
	[SerializeField]
	[Header("エフェクト")]
	private ParticleSystem[] effect;
	[SerializeField]
	[Header("マ\u30fcク")]
	private SpriteRenderer[] mark;
	[SerializeField]
	[Header("マ\u30fcクアンカ\u30fc")]
	private Transform[] markAnchor;
	[SerializeField]
	[Header("ベスト文字")]
	private SpriteRenderer bestText;
	[SerializeField]
	[Header("説明テキスト")]
	private SpriteRenderer[] description;
	[SerializeField]
	[Header("説明表示")]
	private GameObject objInfo;
	private Skijump_Define.TimingResult timingResult;
	private float areaSizeDef;
	private float showTime = 0.2f;
	private float moveTime = 0.75f;
	private float moveTimeMax = 1.35f;
	private float[] nowTime;
	private int state;
	private float localPosXDef;
	private float badBoarder = 0.25f;
	private float gaugeFadeTime = 0.25f;
	private bool isShow;
	private bool[] isShowDescription;
	private Coroutine[] fadeInDescription;
	private Coroutine[] fadeOutDescription;
	private float descriptionScalingTime;
	private float descriptionScalingSpeed = 3f;
	public float MoveTimeMax => moveTimeMax;
	public bool IsShow => isShow;
	public void Init()
	{
		localPosXDef = markAnchor[0].transform.localPosition.x;
		UnityEngine.Debug.Log("Init_LocalPosXDef:" + localPosXDef.ToString());
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(0f);
		}
		areaSizeDef = perfectArea.size.x;
		for (int j = 0; j < mark.Length; j++)
		{
			mark[j].SetAlpha(0f);
		}
		gaugeBack.SetAlpha(0f);
		perfectArea.SetAlpha(0f);
		goodArea.SetAlpha(0f);
		bestText.SetAlpha(0f);
		for (int k = 0; k < description.Length; k++)
		{
			description[k].SetAlpha(0f);
		}
		nowTime = new float[7];
		isShow = false;
		fadeInDescription = new Coroutine[description.Length];
		fadeOutDescription = new Coroutine[description.Length];
		isShowDescription = new bool[description.Length];
		descriptionScalingTime = 0f;
		objInfo.SetActive(value: false);
	}
	public void SetShowUI(bool _show)
	{
		gaugeAnchor.SetActive(_show);
		effectAnchor.SetActive(_show);
		if (!_show)
		{
			isShow = false;
		}
	}
	public void UpdateMethod()
	{
		if (CheckState(State.SHOW))
		{
			StateShow();
		}
		if (CheckState(State.MOVE))
		{
			StateMove();
		}
		if (CheckState(State.HIDE))
		{
			StateHide();
		}
		if (CheckState(State.GAUGE_SHOW))
		{
			StateGaugeShow();
		}
		if (CheckState(State.GAUGE_HIDE))
		{
			StateGaugeHide();
		}
		DescriptionScaling();
	}
	public void CngStateShow(float _delayTime = 0f)
	{
		if (_delayTime <= 0f)
		{
			SettingStateShow();
		}
		else
		{
			StartCoroutine(_CngStateShow(_delayTime));
		}
	}
	private IEnumerator _CngStateShow(float _delayTime = 0f)
	{
		yield return new WaitForSeconds(_delayTime);
		SettingStateShow();
	}
	public void StateShow()
	{
		nowTime[1] += Time.deltaTime;
		if (nowTime[1] >= showTime)
		{
			nowTime[1] = showTime;
			StopState(State.SHOW);
		}
		float a = nowTime[1] / showTime;
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(a);
		}
	}
	public void CngStateHide(float _delayTime = 0f)
	{
		if (_delayTime <= 0f)
		{
			SettingStateHide();
		}
		else
		{
			StartCoroutine(_CngStateHide(_delayTime));
		}
	}
	private IEnumerator _CngStateHide(float _delayTime = 0f)
	{
		yield return new WaitForSeconds(_delayTime);
		SettingStateHide();
	}
	public void StateHide()
	{
		nowTime[4] += Time.deltaTime;
		if (nowTime[4] >= showTime)
		{
			nowTime[4] = showTime;
			StopState(State.HIDE);
		}
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(1f - nowTime[4] / showTime);
		}
	}
	public void CngStateMove(float _delay = 0f)
	{
		if (_delay <= 0f)
		{
			SettingStateMove();
		}
		else
		{
			StartCoroutine(_CngStateMove(_delay));
		}
	}
	private IEnumerator _CngStateMove(float _delay)
	{
		yield return new WaitForSeconds(_delay);
		SettingStateMove();
	}
	public void StateMove()
	{
		nowTime[2] += Time.deltaTime;
		if (nowTime[2] >= moveTimeMax)
		{
			nowTime[2] = moveTimeMax;
			StopState(State.MOVE);
		}
		SetMarkPos(nowTime[2] / moveTime);
	}
	public void CngStateMoveStop(bool _success = true, bool _isSkip = false)
	{
		timingResult = CheckTimingResult(GetTimiingLocalPosX(), _success);
		if (!_isSkip)
		{
			ShowEffect(timingResult);
		}
		StopState(State.MOVE);
		nowTime[3] = 0f;
		StartState(State.MOVE_STOP);
	}
	public void CngStateGaugeShow(bool _show, float _delay = 0f)
	{
		if (_delay <= 0f)
		{
			SettingCngStateGaugeShow(_show);
		}
		else
		{
			StartCoroutine(_CngStateGaugeShow(_show, _delay));
		}
	}
	private IEnumerator _CngStateGaugeShow(bool _show, float _delay)
	{
		yield return new WaitForSeconds(_delay);
		SettingCngStateGaugeShow(_show);
	}
	private void StateGaugeShow()
	{
		nowTime[5] += Time.deltaTime;
		if (nowTime[5] >= gaugeFadeTime)
		{
			nowTime[5] = gaugeFadeTime;
			StopState(State.GAUGE_SHOW);
		}
		float alpha = nowTime[5] / gaugeFadeTime;
		SetAlpha(alpha);
	}
	public void StateGaugeHide()
	{
		nowTime[6] += Time.deltaTime;
		if (nowTime[6] >= gaugeFadeTime)
		{
			nowTime[6] = gaugeFadeTime;
			StopState(State.GAUGE_HIDE);
			isShow = false;
			StopFadeDescription();
		}
		float alpha = 1f - nowTime[6] / gaugeFadeTime;
		SetAlpha(alpha);
	}
	public void ShowEffect(Skijump_Define.TimingResult _type)
	{
		if (effectAnchor.activeSelf)
		{
			effect[(int)_type].Play();
			switch (_type)
			{
			case Skijump_Define.TimingResult.PERFECT:
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
				break;
			case Skijump_Define.TimingResult.GOOD:
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
				break;
			case Skijump_Define.TimingResult.BAD:
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
				break;
			}
		}
	}
	public void ShowDescription(bool _show, int _no, float _delay = 0f)
	{
	}
	private IEnumerator _FadeInDescription(int _no, float _delay)
	{
		yield return new WaitForSeconds(_delay);
		float time = 0f;
		do
		{
			time += Time.deltaTime;
			if (time >= gaugeFadeTime)
			{
				time = gaugeFadeTime;
			}
			description[_no].SetAlpha(time / gaugeFadeTime);
			yield return null;
		}
		while (time < gaugeFadeTime);
		fadeInDescription[_no] = null;
	}
	private IEnumerator _FadeOutDescription(int _no, float _delay)
	{
		yield return new WaitForSeconds(_delay);
		float time = 0f;
		do
		{
			time += Time.deltaTime;
			if (time >= gaugeFadeTime)
			{
				time = gaugeFadeTime;
			}
			description[_no].SetAlpha(1f - time / gaugeFadeTime);
			yield return null;
		}
		while (time < gaugeFadeTime);
		fadeOutDescription[_no] = null;
	}
	public void StopFadeDescription()
	{
		for (int i = 0; i < fadeInDescription.Length; i++)
		{
			if (fadeInDescription[i] != null)
			{
				StopCoroutine(fadeInDescription[i]);
				fadeInDescription[i] = null;
			}
			isShowDescription[i] = false;
		}
		for (int j = 0; j < description.Length; j++)
		{
			description[j].SetAlpha(0f);
		}
	}
	public void ResetState()
	{
		state = 0;
	}
	public void StartState(State _state)
	{
		state |= 1 << (int)_state;
	}
	public void StopState(State _state)
	{
		state &= ~(1 << (int)_state);
	}
	private Skijump_Define.TimingResult CheckTimingResult(float _pos, bool _success)
	{
		if (_success)
		{
			if (_pos <= perfectArea.size.x * 0.5f)
			{
				return Skijump_Define.TimingResult.PERFECT;
			}
			if (_pos <= goodArea.size.x * 0.75f)
			{
				return Skijump_Define.TimingResult.GOOD;
			}
			return Skijump_Define.TimingResult.BAD;
		}
		return Skijump_Define.TimingResult.BAD;
	}
	public bool CheckState(State _state)
	{
		if (_state == State.NONE)
		{
			return state == 0;
		}
		return (state & (1 << (int)_state)) != 0;
	}
	public void SetGaugeAreaMag(float _mag)
	{
		Vector2 size = perfectArea.size;
		size.x = areaSizeDef * _mag;
		perfectArea.size = size;
		size.x = perfectArea.size.x * 3f;
		goodArea.size = size;
	}
	private void DescriptionScaling()
	{
		descriptionScalingTime += descriptionScalingSpeed * Time.deltaTime;
		for (int i = 0; i < description.Length; i++)
		{
			description[i].transform.SetLocalScaleX(1f + (1f + Mathf.Sin(-(float)Math.PI / 2f + descriptionScalingTime)) * 0.5f * 0.1f);
			description[i].transform.SetLocalScaleY(1f + (1f + Mathf.Sin(-(float)Math.PI / 2f + descriptionScalingTime)) * 0.5f * 0.1f);
		}
	}
	private void SettingStateShow()
	{
		nowTime[1] = 0f;
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(0f);
			mark[i].transform.SetLocalPositionX(0f);
		}
		nowTime[1] = 0f;
		StartState(State.SHOW);
	}
	private void SettingStateHide()
	{
		nowTime[4] = 0f;
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(1f);
		}
		nowTime[4] = 0f;
		StartState(State.HIDE);
	}
	public void SetMarkPos(float _per)
	{
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].transform.SetLocalPositionX(localPosXDef * _per);
		}
	}
	public void SettingStateMove()
	{
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].transform.SetLocalPositionX(0f);
		}
		nowTime[2] = 0f;
		StartState(State.MOVE);
	}
	private void SettingCngStateGaugeShow(bool _show)
	{
		if (_show)
		{
			StartState(State.GAUGE_SHOW);
			for (int i = 0; i < mark.Length; i++)
			{
				mark[i].transform.SetLocalPositionX(0f);
			}
			nowTime[5] = 0f;
			objInfo.SetActive((!Skijump_Define.MCM.GetNowJumpChara().IsCpu) ? true : false);
			LeanTween.cancel(objInfo);
			objInfo.transform.SetLocalScale(0f, 0f, 1f);
			LeanTween.scale(objInfo, Vector3.one, 0.25f).setEaseOutQuart();
		}
		else
		{
			StartState(State.GAUGE_HIDE);
			nowTime[6] = 0f;
			LeanTween.cancel(objInfo);
			objInfo.transform.SetLocalScale(0f, 0f, 1f);
			LeanTween.scale(objInfo, Vector3.zero, 0.25f).setEaseOutQuart().setOnComplete((Action)delegate
			{
				objInfo.SetActive(value: false);
			});
		}
		isShow = true;
	}
	public void SetAlpha(float _alpha)
	{
		for (int i = 0; i < mark.Length; i++)
		{
			mark[i].SetAlpha(_alpha);
		}
		gaugeBack.SetAlpha(_alpha);
		perfectArea.SetAlpha(_alpha);
		goodArea.SetAlpha(_alpha);
		bestText.SetAlpha(_alpha);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objInfo);
	}
	public float GetTimiingLocalPosXPer()
	{
		return 1f - Mathf.Min((localPosXDef - mark[0].transform.localPosition.x) / (goodArea.size.x * 0.5f), 1f);
	}
	public float GetTimiingLocalPosX()
	{
		return Mathf.Abs(mark[0].transform.localPosition.x - localPosXDef);
	}
	public float GetTimingValue(float _value = 0f, bool _resultCorr = true)
	{
		float num = (!(_value <= 0f)) ? _value : (1f - Mathf.Min(GetTimiingLocalPosX() / (goodArea.size.x * 0.5f), 1f - badBoarder));
		if (_resultCorr)
		{
			num = Mathf.Pow(num, 2f);
			if (timingResult == Skijump_Define.TimingResult.GOOD)
			{
				num *= 0.75f;
			}
			else if (timingResult == Skijump_Define.TimingResult.BAD)
			{
				num *= 0.5f;
			}
		}
		return num;
	}
	public float GetCpuStopTiming(Skijump_Define.TimingResult _result, float _value)
	{
		float num = 0f;
		switch (_result)
		{
		case Skijump_Define.TimingResult.PERFECT:
			num = perfectArea.size.x * 0.5f * _value;
			break;
		case Skijump_Define.TimingResult.GOOD:
			num = perfectArea.size.x * 0.5f + (goodArea.size.x * 0.5f - perfectArea.size.x * 0.5f) * _value;
			break;
		case Skijump_Define.TimingResult.BAD:
			num = goodArea.size.x * 0.5f + areaSizeDef * _value;
			break;
		}
		num /= localPosXDef;
		int num2 = (UnityEngine.Random.Range(0, 100) <= 50) ? 1 : (-1);
		num *= (float)num2;
		return Mathf.Abs(num);
	}
	public Skijump_Define.TimingResult GetTimingResult()
	{
		return timingResult;
	}
}
