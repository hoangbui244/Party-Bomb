using System;
using System.Collections;
using UnityEngine;
public class LegendarySword_Chara : MonoBehaviour
{
	public enum MoveCursorDirection
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum ProcessType
	{
		STANDBY,
		SLOW_SPEED,
		START,
		GOAL,
		SKIP
	}
	public enum CameraPosType
	{
		NEAR,
		NORMAL,
		DISTANT
	}
	[SerializeField]
	[Header("観客アニメ\u30fcション")]
	private AudienceAnimation audienceAnimation;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	private int swordFieldIdx;
	[SerializeField]
	[Header("CharacterBase")]
	private GameObject base_obj;
	[SerializeField]
	[Header("対象のLegendarySword_Player")]
	public LegendarySword_Player player;
	[SerializeField]
	[Header("対象のLegendarySword_AI")]
	private LegendarySword_AI ai;
	[SerializeField]
	[Header("アニメ\u30fcション管理クラス")]
	private LegendarySword_AnimationManagement animationManagement;
	[SerializeField]
	[Header("キャラのX軸回転/高さ変更用")]
	public GameObject characterAnchorXrot;
	[SerializeField]
	[Header("キャラのY軸回転用")]
	public GameObject characterAnchorYrot;
	[SerializeField]
	[Header("加速度")]
	public float acceleration = 0.07f;
	[SerializeField]
	[Header("プレイヤ\u30fcの最高速度")]
	private float MAX_SPEED = 3f;
	[SerializeField]
	[Header("ダッシュ地面")]
	private ParticleSystem psGround;
	[SerializeField]
	[Header("汗エフェクト")]
	private ParticleSystem[] sweatEffect;
	[SerializeField]
	[Header("地面煙")]
	private ParticleSystem psSmoke;
	private int pushCnt;
	[SerializeField]
	[Header("試合別の剣オブジェクト配列")]
	private LegendarySword_Sword[] arraySword;
	private LegendarySword_Sword currentSword;
	private float m_speed;
	private Vector3 speed = new Vector3(0f, 0f, -0.5f);
	private bool maxspeed;
	private bool canctrl;
	private bool goaleffect;
	private bool runningSE;
	private bool animestart;
	private bool isChangeEffect;
	private bool isInit;
	private bool start;
	private float runSeTime;
	private LegendarySword_Define.UserType userType;
	private float startSpeed = 1.5f;
	public ProcessType processType;
	private float sePitch;
	private bool isSkip;
	public GameObject Base_obj => base_obj;
	public bool Dive
	{
		get
		{
			return Dive;
		}
		set
		{
			Dive = value;
		}
	}
	public int PlayerNo => playerNo;
	private void Start()
	{
		animationManagement.Init(player);
	}
	private void LateUpdate()
	{
		if (processType == ProcessType.GOAL || animestart)
		{
			EffectReset();
		}
		else if (runSeTime >= 0.7f && runningSE)
		{
			float pitch = UnityEngine.Random.Range(0.5f, 1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.35f, pitch, 0f, _overlap: true);
			runSeTime = 0f;
		}
		AddEffect();
		if (processType == ProcessType.START && !player.isover)
		{
			if (pushCnt == SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt() / 2)
			{
				LegendarySword_Define.GM.GetSwordField(swordFieldIdx).PlayHalfPullOutSparkleEffect();
			}
			if (pushCnt >= SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt())
			{
				SetGoal();
				SingletonCustom<AudioManager>.Instance.SePlay("se_legendary_sword_sparkle", _loop: false, 0f, 0.5f);
				LegendarySword_Define.GM.GetSwordField(swordFieldIdx).PlayPullOutSparkleEffect();
				player.CharacterShake(25);
				LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
				{
					if (player.isPlay)
					{
						player.PlayerFaceJoy();
					}
					LegendarySword_Define.GM.GetSwordField(swordFieldIdx).PlayPullOutSparkleRainbowEffect();
					SetRaiseAnimation();
					SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult");
					SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult");
					LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
					{
						currentSword.PlayShineEffect();
					});
				});
			}
			else if (LegendarySword_Define.GM.IsGoalCharacter() && !player.goal)
			{
				SetGoal();
				UnityEngine.Debug.Log("負けた場合 ");
				LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
				{
					if (player.isPlay)
					{
						player.PlayerFaceSorrow();
					}
					EffectReset();
				});
			}
		}
		if (processType >= ProcessType.START && LegendarySword_Define.GM.IsStopTime)
		{
			canctrl = false;
			int num = (playerNo == 0) ? 1 : 0;
			if (!player.goal && !LegendarySword_Define.PM.Players[num].goal && !goaleffect)
			{
				EffectReset();
				UnityEngine.Debug.Log("ゴ\u30fcル！" + player.goal.ToString() + LegendarySword_Define.PM.Players[num].goal.ToString());
				SetGoal();
				goaleffect = true;
				runningSE = false;
			}
		}
	}
	private void AddEffect()
	{
		if (processType == ProcessType.START)
		{
			if (start)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_speed");
				start = false;
			}
		}
		else
		{
			EffectReset();
		}
	}
	public void EffectReset()
	{
		psSmoke.Stop();
		psGround.Stop();
		psSmoke.Clear();
		psGround.Clear();
		isChangeEffect = false;
		runningSE = false;
	}
	public void StopSwordShineEffect()
	{
		currentSword.StopShineEffect();
	}
	public void PlayerInit(LegendarySword_Player _player)
	{
		userType = _player.UserType;
		player.goal = false;
		animestart = false;
		goaleffect = false;
		start = false;
		EffectReset();
		maxspeed = false;
		pushCnt = 0;
		ProcessTypeChange(ProcessType.STANDBY);
		psSmoke.Stop();
		psSmoke.Clear();
		bool isPlay = _player.isPlay;
		for (int i = 0; i < arraySword.Length; i++)
		{
			arraySword[i].gameObject.SetActive(value: false);
		}
		if (player.isPlay)
		{
			currentSword = arraySword[(int)LegendarySword_Define.GM.CurrentTournamentType];
			currentSword.gameObject.SetActive(value: true);
		}
		audienceAnimation.enabled = !player.isPlay;
		isSkip = false;
	}
	public void StartMethod()
	{
		ProcessTypeChange(ProcessType.START);
		ai.BattleStartControl();
	}
	public void UpdateMethod()
	{
	}
	private IEnumerator WaitForSeconds(float interval)
	{
		yield return new WaitForSeconds(interval);
	}
	public bool AccelInput()
	{
		int num = (playerNo == 0) ? 1 : 0;
		if (!animestart && !LegendarySword_Define.PM.Players[num].Chara.animestart)
		{
			if (processType == ProcessType.START && canctrl)
			{
				pushCnt++;
				if (pushCnt > SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt())
				{
					pushCnt = SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt();
				}
				if (IsMaxPushCnt() && LegendarySword_Define.PM.Players[num].Chara.IsMaxPushCnt())
				{
					pushCnt--;
					return false;
				}
				LegendarySword_Define.GM.GetSwordField(swordFieldIdx).SwordUp();
				if (!isSkip)
				{
					SetPulloutAnimationValue((float)pushCnt / (float)SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt());
				}
				sweatEffect[0].Play();
				sweatEffect[1].Play();
				if (player.UserType <= LegendarySword_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)player.UserType);
					SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
				}
				return true;
			}
			return false;
		}
		EffectReset();
		return false;
	}
	public void ProcessTypeChange(ProcessType _type)
	{
		switch (_type)
		{
		case ProcessType.SLOW_SPEED:
			break;
		case ProcessType.STANDBY:
			processType = ProcessType.STANDBY;
			break;
		case ProcessType.START:
			if (processType == ProcessType.STANDBY)
			{
				processType = ProcessType.START;
				canctrl = true;
			}
			break;
		case ProcessType.GOAL:
			if (processType == ProcessType.START)
			{
				processType = ProcessType.GOAL;
				canctrl = false;
			}
			break;
		}
	}
	public void SetGoal()
	{
		if (processType != ProcessType.GOAL)
		{
			if (!LegendarySword_Define.GM.IsGoalCharacter())
			{
				ProcessTypeChange(ProcessType.GOAL);
				player.goal = true;
			}
			else if (player.isPlay)
			{
				UnityEngine.Debug.Log(PlayerNo.ToString() + "番のプレイヤ\u30fcの負け");
			}
			player.isover = true;
		}
	}
	public void SetStandbyAnimation()
	{
		animationManagement.SetStandbyAnimation();
	}
	public void SetPulloutAnimationValue(float _value)
	{
		animationManagement.SetPulloutAnimationValue(_value);
	}
	public void SetRaiseAnimation()
	{
		animationManagement.SetRaiseAnimation();
	}
	public void SetResetAnimation()
	{
		animationManagement.SetResetAnimation();
	}
	public void SetSwordFieldIdx(int _idx)
	{
		swordFieldIdx = _idx;
	}
	public void SetSkipPushCnt()
	{
		isSkip = true;
		pushCnt = SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt();
	}
	public bool IsMaxPushCnt()
	{
		return pushCnt == SingletonCustom<LegendarySword_GameManager>.Instance.GetMaxPushCnt();
	}
}
