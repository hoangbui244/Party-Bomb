using System;
using UnityEngine;
public class Skijump_BalanceController : MonoBehaviour
{
	public enum State
	{
		NONE,
		SHOW,
		HIDE,
		MOVE_MARK,
		SHOW_DELAY,
		HIDE_DELAY,
		MOVE_MARK_DELAY,
		MAX
	}
	public enum ArrowAniType
	{
		NEUTRAL,
		EXPAND,
		REVERSE
	}
	[SerializeField]
	[Header("オブジェクト")]
	private GameObject obj;
	[SerializeField]
	[Header("背景")]
	private SpriteRenderer[] back;
	[SerializeField]
	[Header("マ\u30fcク")]
	private SpriteRenderer mark;
	[SerializeField]
	[Header("半径アンカ\u30fc")]
	private Transform radiusAnchor;
	[SerializeField]
	[Header("中心アンカ\u30fc")]
	private Transform centerAnchor;
	[SerializeField]
	[Header("文字")]
	private SpriteRenderer[] text;
	[SerializeField]
	[Header("矢印")]
	private SpriteRenderer[] arrow;
	[SerializeField]
	[Header("説明表示")]
	private GameObject objInfo;
	[SerializeField]
	[Header("エフェクトアンカ\u30fc")]
	private GameObject effectAnchor;
	[SerializeField]
	[Header("エフェクト")]
	private ParticleSystem[] effect;
	private float[] backAlpha;
	private float markAlpha;
	private float radius;
	private int state;
	private float[] DirInterval = new float[2]
	{
		0.5f,
		0.75f
	};
	private float dirInterval;
	private Vector3 moveDir;
	private float moveSpeed = 100f;
	private float controllSpeed;
	private float controllSpeedMag = 1.33f;
	private float moveInterval;
	private float alpha;
	private float fadeTime = 0.5f;
	private float[] stateTime = new float[7];
	private float[] delayTime = new float[7];
	private Vector3 rot;
	private ArrowAniType[] arrowAniType;
	private float arrowExpand = 0.5f;
	private float[] arrowAniTime;
	private float descriptionScalingTime;
	private float descriptionScalingSpeed = 3f;
	public void Init()
	{
		state = 0;
		radius = radiusAnchor.localPosition.magnitude;
		controllSpeed = moveSpeed * controllSpeedMag;
		backAlpha = new float[back.Length];
		for (int i = 0; i < backAlpha.Length; i++)
		{
			backAlpha[i] = back[i].color.a;
		}
		markAlpha = mark.color.a;
		alpha = 0f;
		for (int j = 0; j < back.Length; j++)
		{
			back[j].SetAlpha(alpha);
		}
		mark.SetAlpha(alpha);
		for (int k = 0; k < arrow.Length; k++)
		{
			arrow[k].SetAlpha(alpha);
		}
		for (int l = 0; l < text.Length; l++)
		{
			text[l].SetAlpha(alpha);
		}
		for (int m = 0; m < stateTime.Length; m++)
		{
			stateTime[m] = 0f;
		}
		for (int n = 0; n < delayTime.Length; n++)
		{
			delayTime[n] = 0f;
		}
		arrowAniType = new ArrowAniType[arrow.Length];
		arrowAniTime = new float[arrow.Length];
		descriptionScalingTime = 0f;
		objInfo.SetActive(value: false);
	}
	public void ShowEffect(Skijump_Define.TimingResult _type)
	{
		if (effectAnchor.activeSelf)
		{
			effectAnchor.transform.SetPositionX(mark.transform.position.x);
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
	public void UpdateMethod()
	{
		if (CheckState(State.SHOW_DELAY))
		{
			StateShowDelay();
		}
		if (CheckState(State.SHOW))
		{
			StateShow();
		}
		if (CheckState(State.HIDE_DELAY))
		{
			StateHideDelay();
		}
		if (CheckState(State.HIDE))
		{
			StateHide();
		}
		if (CheckState(State.MOVE_MARK_DELAY))
		{
			StateMoveMarkDelay();
		}
		if (CheckState(State.MOVE_MARK))
		{
			StateMoveMark();
		}
		for (int i = 0; i < arrow.Length; i++)
		{
			ArrowAnimation(i);
		}
	}
	private void DescriptionScaling()
	{
		descriptionScalingTime += descriptionScalingSpeed * Time.deltaTime;
		text[0].transform.SetLocalScaleX(1f + (1f + Mathf.Sin(-(float)Math.PI / 2f + descriptionScalingTime)) * 0.5f * 0.1f);
		text[0].transform.SetLocalScaleY(1f + (1f + Mathf.Sin(-(float)Math.PI / 2f + descriptionScalingTime)) * 0.5f * 0.1f);
	}
	public void Show(float _delay = 0f)
	{
		StartState(State.SHOW_DELAY);
		delayTime[4] = _delay;
	}
	private void StateShowDelay()
	{
		delayTime[4] -= Time.deltaTime;
		if (delayTime[4] <= 0f)
		{
			StopState(State.SHOW_DELAY);
			CngShow();
		}
	}
	private void CngShow()
	{
		StartState(State.SHOW);
		alpha = 0f;
		for (int i = 0; i < back.Length; i++)
		{
			back[i].SetAlpha(alpha);
		}
		mark.SetAlpha(alpha);
		for (int j = 0; j < arrow.Length; j++)
		{
			arrow[j].SetAlpha(alpha);
		}
		for (int k = 0; k < text.Length; k++)
		{
			text[k].SetAlpha(alpha);
		}
		mark.transform.SetLocalPosition(0f, 0f, 0f);
		stateTime[1] = 0f;
		objInfo.SetActive((!Skijump_Define.MCM.GetNowJumpChara().IsCpu) ? true : false);
		LeanTween.cancel(objInfo);
		objInfo.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(objInfo, Vector3.one, 0.25f).setEaseOutQuart();
	}
	public void Hide(float _delay = 0f)
	{
		StartState(State.HIDE_DELAY);
		delayTime[5] = _delay;
		StopState(State.MOVE_MARK);
	}
	private void StateHideDelay()
	{
		delayTime[5] -= Time.deltaTime;
		if (delayTime[5] <= 0f)
		{
			StopState(State.HIDE_DELAY);
			CntHide();
		}
	}
	private void CntHide()
	{
		StartState(State.HIDE);
		alpha = 1f;
		for (int i = 0; i < back.Length; i++)
		{
			back[i].SetAlpha(alpha);
		}
		mark.SetAlpha(alpha);
		for (int j = 0; j < arrow.Length; j++)
		{
			arrow[j].SetAlpha(alpha);
		}
		for (int k = 0; k < text.Length; k++)
		{
			text[k].SetAlpha(alpha);
		}
		stateTime[2] = 0f;
		LeanTween.cancel(objInfo);
		objInfo.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(objInfo, Vector3.zero, 0.25f).setEaseOutQuart().setOnComplete((Action)delegate
		{
			objInfo.SetActive(value: false);
		});
	}
	public void MoveMark(float _delay = 0f)
	{
		StartState(State.MOVE_MARK_DELAY);
		delayTime[6] = _delay;
	}
	private void StateMoveMarkDelay()
	{
		delayTime[6] -= Time.deltaTime;
		if (delayTime[6] <= 0f)
		{
			StopState(State.MOVE_MARK_DELAY);
			CngMoveMark();
		}
	}
	private void CngMoveMark()
	{
		StartState(State.MOVE_MARK);
		moveInterval = 0.5f;
	}
	private void StateShow()
	{
		stateTime[1] += Time.deltaTime;
		if (stateTime[1] > fadeTime)
		{
			stateTime[1] = fadeTime;
			StopState(State.SHOW);
		}
		alpha = stateTime[1] / fadeTime;
		SetAlpha(alpha);
	}
	private void StateHide()
	{
		stateTime[2] += Time.deltaTime;
		if (stateTime[2] > fadeTime)
		{
			stateTime[2] = fadeTime;
			StopState(State.HIDE);
		}
		alpha = 1f - stateTime[2] / fadeTime;
		SetAlpha(alpha);
	}
	public void SetAlpha(float _alpha)
	{
		for (int i = 0; i < back.Length; i++)
		{
			back[i].SetAlpha(backAlpha[i] * _alpha);
		}
		mark.SetAlpha(markAlpha * _alpha);
		for (int j = 0; j < arrow.Length; j++)
		{
			arrow[j].SetAlpha(_alpha);
		}
		for (int k = 0; k < text.Length; k++)
		{
			text[k].SetAlpha(_alpha);
		}
	}
	private void StateMoveMark()
	{
		moveInterval -= Time.deltaTime;
		if (moveInterval <= 0f)
		{
			dirInterval -= Time.deltaTime;
			if (dirInterval <= 0f)
			{
				UpdateDir();
			}
			mark.transform.AddLocalPositionX(moveDir.x * moveSpeed * Time.deltaTime);
			if (Mathf.Abs(mark.transform.localPosition.x) > radius)
			{
				mark.transform.SetLocalPositionX(Mathf.Sign(mark.transform.localPosition.x) * radius);
			}
		}
	}
	private void UpdateDir()
	{
		dirInterval = UnityEngine.Random.Range(DirInterval[0], DirInterval[1]);
		if (CalcManager.IsPerCheck(50))
		{
			moveDir.x = 1f;
		}
		else
		{
			moveDir.x = -1f;
		}
	}
	public void MarkControll(Vector3 _dir)
	{
		mark.transform.AddLocalPositionX(_dir.x * controllSpeed * Time.deltaTime);
	}
	public void SetMarkPos(float _per)
	{
		mark.transform.SetLocalPositionX(_per * radius);
	}
	public float GetControllValue()
	{
		return Mathf.Abs(mark.transform.localPosition.x) / radius;
	}
	public float GetMarkPosPer()
	{
		return mark.transform.localPosition.x / radius;
	}
	private void ArrowAnimation(int _no)
	{
		switch (arrowAniType[_no])
		{
		case ArrowAniType.NEUTRAL:
			arrowAniTime[_no] -= Time.deltaTime;
			if (arrowAniTime[_no] <= 0f)
			{
				arrowAniTime[_no] = 0f;
			}
			break;
		case ArrowAniType.EXPAND:
			arrowAniTime[_no] += Time.deltaTime;
			if (arrowAniTime[_no] >= 1f)
			{
				arrowAniTime[_no] = 1f;
				arrowAniType[_no] = ArrowAniType.REVERSE;
			}
			break;
		case ArrowAniType.REVERSE:
			arrowAniTime[_no] -= Time.deltaTime;
			if (arrowAniTime[_no] <= 0f)
			{
				arrowAniTime[_no] = 0f;
				arrowAniType[_no] = ArrowAniType.EXPAND;
			}
			break;
		}
		arrow[_no].transform.SetLocalScaleX(1f + arrowExpand * arrowAniTime[_no]);
		arrow[_no].transform.SetLocalScaleY(1f + arrowExpand * arrowAniTime[_no]);
	}
	public void SetArrowAnimation(int _no, ArrowAniType _type)
	{
		arrowAniType[_no] = _type;
	}
	public bool IsArrowNeutral(int _no)
	{
		return arrowAniType[_no] == ArrowAniType.NEUTRAL;
	}
	public Vector3 GetBalanceRot()
	{
		rot.x = 0f;
		rot.y = 0f;
		rot.z = GetMarkPosPer();
		return rot;
	}
	public Vector3 GetCenterPos()
	{
		return centerAnchor.position;
	}
	public Vector3 GetMoveDir()
	{
		return moveDir;
	}
	public bool IsControll()
	{
		return CheckState(State.MOVE_MARK);
	}
	public void SetShowUi(bool _show)
	{
		obj.SetActive(_show);
	}
	public bool CheckState(State _state)
	{
		if (_state == State.NONE)
		{
			return state == 0;
		}
		return (state & (1 << (int)_state)) != 0;
	}
	public void StartState(State _state)
	{
		state |= 1 << (int)_state;
	}
	public void StopState(State _state)
	{
		state &= ~(1 << (int)_state);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objInfo);
	}
}
