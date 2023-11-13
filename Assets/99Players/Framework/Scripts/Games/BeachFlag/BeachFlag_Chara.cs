using System.Collections;
using UnityEngine;
public class BeachFlag_Chara : MonoBehaviour
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
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	[SerializeField]
	[Header("CharacterBase")]
	private GameObject base_obj;
	[SerializeField]
	[Header("対象のBeachFlag_Player")]
	public BeachFlag_Player player;
	[SerializeField]
	[Header("対象のBeachFlag_AI")]
	private BeachFlag_AI ai;
	[SerializeField]
	[Header("キャラのX軸回転/高さ変更用")]
	public GameObject characterAnchorXrot;
	[SerializeField]
	[Header("キャラのY軸回転用")]
	public GameObject characterAnchorYrot;
	[SerializeField]
	[Header("走行に使用するDollyCart")]
	private MyCinemachineDollyCart cart;
	[SerializeField]
	[Header("加速度")]
	public float acceleration = 0.07f;
	[SerializeField]
	[Header("プレイヤ\u30fcの最高速度")]
	private float MAX_SPEED = 3f;
	[SerializeField]
	[Header("ダッシュエフェクト")]
	private ParticleSystem psDashRoll;
	[SerializeField]
	[Header("ダッシュ炎")]
	private ParticleSystem psDashFire;
	[SerializeField]
	[Header("ダッシュ地面")]
	private ParticleSystem psGround;
	[SerializeField]
	[Header("ダッシュスピ\u30fcド")]
	private ParticleSystem psSpeed;
	[SerializeField]
	[Header("ダッシュ移行エフェクト")]
	private ParticleSystem psDashChange;
	[SerializeField]
	[Header("汗エフェクト")]
	private ParticleSystem sweatEffect;
	[SerializeField]
	[Header("地面煙")]
	private ParticleSystem psSmoke;
	[SerializeField]
	[Header("BeachFlag_Animation")]
	private BeachFlag_Animation anime;
	public float CART_MAX = 19f;
	public float DIVE_POINT = 19.8f;
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
	private BeachFlag_Define.UserType userType;
	private float startSpeed = 1.5f;
	public ProcessType processType;
	private float sePitch;
	public GameObject Base_obj => base_obj;
	public MyCinemachineDollyCart Cart
	{
		get
		{
			return cart;
		}
		set
		{
			cart = value;
		}
	}
	public ParticleSystem PsDashRoll => psDashRoll;
	public ParticleSystem PsDashFire => psDashFire;
	public ParticleSystem PsGround => psGround;
	public ParticleSystem PsSpeed => psSpeed;
	public ParticleSystem PsDashChange => psDashChange;
	public ParticleSystem SweatEffect => sweatEffect;
	public ParticleSystem PsSmoke => psSmoke;
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
		runSeTime += Time.deltaTime * Mathf.Abs(cart.m_Speed / 0.7f);
		if (!anime.dive_done)
		{
			cart.m_Position = Mathf.Clamp(cart.m_Position, 0.26f, CART_MAX + 1f);
		}
		else
		{
			cart.m_Speed = 0.5f;
		}
		if (BeachFlag_Define.GM.IsGoalCharacter() && !player.goal)
		{
			EffectReset();
			SetGoal();
		}
		if (processType >= ProcessType.START && BeachFlag_Define.GM.IsStopTime)
		{
			canctrl = false;
			int num = (playerNo == 0) ? 1 : 0;
			if (!player.goal && !BeachFlag_Define.PM.Players[num].goal && !goaleffect)
			{
				EffectReset();
				UnityEngine.Debug.Log("ゴ\u30fcル！" + player.goal.ToString() + BeachFlag_Define.PM.Players[num].goal.ToString());
				SetGoal();
				goaleffect = true;
				runningSE = false;
			}
		}
		if (cart.m_Position >= CART_MAX)
		{
			if (anime.animType != BeachFlag_Animation.AnimType.DIVE)
			{
				canctrl = false;
				EffectReset();
			}
			if (cart.m_Position < DIVE_POINT)
			{
				PsGround.Emit(1);
				psSmoke.Emit(2);
			}
			else
			{
				cart.m_Speed = 0f;
				EffectReset();
			}
			if (!BeachFlag_Define.GM.signboard.activeSelf)
			{
				BeachFlag_Define.GM.signboard.SetActive(value: true);
			}
			if (BeachFlag_Define.CaM.GetState() != BeachFlag_CameraManager.CameraState.DIVE)
			{
				BeachFlag_Define.CaM.SetState(BeachFlag_CameraManager.CameraState.DIVE);
			}
			if (anime.animType < BeachFlag_Animation.AnimType.DIVE && anime.animType != BeachFlag_Animation.AnimType.DIVE)
			{
				anime.SetAnim(BeachFlag_Animation.AnimType.DIVE);
			}
			for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
			{
				BeachFlag_Define.PM.Players[i].Chara.EffectReset();
			}
			SetGoal();
			animestart = true;
			int num2 = 0;
			for (int j = 0; j < BeachFlag_Define.PM.Players.Length; j++)
			{
				if (BeachFlag_Define.PM.Players[j].isPlay && j != playerNo)
				{
					num2 = j;
				}
			}
			if (!player.goal && !BeachFlag_Define.PM.Players[num2].goal && !goaleffect)
			{
				UnityEngine.Debug.Log("ゴ\u30fcル！" + player.goal.ToString() + BeachFlag_Define.PM.Players[num2].goal.ToString());
				SetGoal();
				goaleffect = true;
			}
		}
		else
		{
			canctrl = true;
		}
	}
	private void AddEffect()
	{
		if (processType == ProcessType.START)
		{
			if (start)
			{
				psDashChange.Emit(1);
				SingletonCustom<AudioManager>.Instance.SePlay("se_speed");
				psDashChange.Stop();
				start = false;
			}
			if (cart.m_Speed > 2f)
			{
				if (Time.frameCount % UnityEngine.Random.Range(8, 10) == 0)
				{
					psSmoke.Emit(1);
				}
				if (!psSpeed.isPlaying)
				{
					psSpeed.Play();
				}
			}
			else if (psSpeed.isPlaying)
			{
				psSpeed.Stop();
			}
			if (cart.m_Speed >= MAX_SPEED)
			{
				if (!psDashRoll.isPlaying)
				{
					if (!isChangeEffect)
					{
						psDashChange.Emit(1);
						isChangeEffect = true;
						SingletonCustom<AudioManager>.Instance.SePlay("se_speed");
					}
					psDashRoll.Play();
				}
				psDashFire.Emit(3);
				psGround.Emit(3);
			}
			else if (psDashRoll.isPlaying)
			{
				psDashRoll.Stop();
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
		psDashRoll.Stop();
		psGround.Stop();
		psSpeed.Stop();
		psDashFire.Stop();
		psDashChange.Stop();
		psSmoke.Clear();
		psDashRoll.Clear();
		psGround.Clear();
		psSpeed.Clear();
		psDashFire.Clear();
		psDashChange.Clear();
		isChangeEffect = false;
		runningSE = false;
	}
	public void PlayerInit(BeachFlag_Player _player)
	{
		userType = _player.UserType;
		player.goal = false;
		animestart = false;
		goaleffect = false;
		start = false;
		EffectReset();
		maxspeed = false;
		ProcessTypeChange(ProcessType.STANDBY);
		psDashRoll.Stop();
		psDashRoll.Clear();
		psSmoke.Stop();
		psSmoke.Clear();
		if (_player.isPlay)
		{
			anime.SetAnim(BeachFlag_Animation.AnimType.STANDBY);
		}
		else if (player == BeachFlag_Define.PM.UnPlayers[0])
		{
			anime.SetAnim(BeachFlag_Animation.AnimType.CHEER);
		}
		else
		{
			anime.SetAnim(BeachFlag_Animation.AnimType.CHEER_TWO_HAND);
		}
		cart.m_Position = 0.26f;
		cart.m_Speed = 0f;
		if (BeachFlag_Define.CaM.GetState() != 0)
		{
			BeachFlag_Define.CaM.SetState(BeachFlag_CameraManager.CameraState.STANDBY);
		}
	}
	public void StartMethod()
	{
		ProcessTypeChange(ProcessType.START);
	}
	public void UpdateMethod()
	{
	}
	private IEnumerator WaitForSeconds(float interval)
	{
		yield return new WaitForSeconds(interval);
	}
	public void AccelInput()
	{
		int num = (playerNo == 0) ? 1 : 0;
		if (!animestart && !BeachFlag_Define.PM.Players[num].Chara.animestart)
		{
			if (processType != ProcessType.START || !canctrl)
			{
				return;
			}
			anime.SetAnimSpeed(Mathf.Abs(cart.m_Speed / 0.7f));
			if (cart.m_Speed <= 0f)
			{
				EffectReset();
			}
			if (cart.m_Position < CART_MAX && !runningSE)
			{
				runningSE = true;
			}
			if (cart.m_Speed < MAX_SPEED)
			{
				if (anime.animType != BeachFlag_Animation.AnimType.GETSET && anime.animType < BeachFlag_Animation.AnimType.GETSET)
				{
					anime.SetAnim(BeachFlag_Animation.AnimType.GETSET);
				}
				AddEffect();
				if (cart.m_Speed == 0f)
				{
					cart.m_Speed += 1f;
					start = true;
				}
				else
				{
					cart.m_Speed += acceleration;
				}
				if (player.UserType <= BeachFlag_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)player.UserType);
				}
				if (BeachFlag_Define.CaM.GetState() != BeachFlag_CameraManager.CameraState.DASH && BeachFlag_Define.PM.Players[num].Chara.canctrl)
				{
					BeachFlag_Define.CaM.SetState(BeachFlag_CameraManager.CameraState.DASH);
				}
				else
				{
					canctrl = false;
				}
			}
			else
			{
				cart.m_Speed = MAX_SPEED;
				AddEffect();
			}
		}
		else
		{
			EffectReset();
		}
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
			}
			break;
		case ProcessType.GOAL:
			if (processType == ProcessType.START)
			{
				processType = ProcessType.GOAL;
			}
			break;
		}
	}
	public void SetGoal()
	{
		if (processType != ProcessType.GOAL)
		{
			if (!BeachFlag_Define.GM.IsGoalCharacter())
			{
				ProcessTypeChange(ProcessType.GOAL);
				player.goal = true;
			}
			else if (player.isPlay)
			{
				UnityEngine.Debug.Log(PlayerNo.ToString() + "番のプレイヤ\u30fcの負け");
			}
			player.isover = true;
			if (!BeachFlag_Define.GM.InSkip && BeachFlag_Define.GM.GetGoalCharacterNo() != playerNo && anime.animType != BeachFlag_Animation.AnimType.GOAL_SAD && player.isPlay)
			{
				anime.SetAnim(BeachFlag_Animation.AnimType.GOAL_SAD);
				cart.m_Speed = 0f;
				EffectReset();
				UnityEngine.Debug.Log("敗北アニメ\u30fcション開始");
			}
		}
	}
}
