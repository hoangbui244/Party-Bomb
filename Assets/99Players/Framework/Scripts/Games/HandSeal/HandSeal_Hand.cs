using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
public class HandSeal_Hand : MonoBehaviour
{
	public enum MoveCursorDirection
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum InputButton
	{
		A,
		B,
		Y,
		X,
		R,
		L
	}
	public enum Kuji
	{
		Rin,
		Hyou,
		Tou,
		Sya,
		Kai,
		Jin,
		Retsu,
		Zai,
		Zen,
		Hold
	}
	public enum Ninjutsu
	{
		FireStyle,
		FireStyle2,
		WaterStyle,
		WaterStyle2,
		DirtStyle,
		DirtStyle2,
		WindStyle,
		WindStyle2,
		BindStyle,
		BindStyle2,
		SecretStyle
	}
	public enum EnemyLevel
	{
		Easy,
		Normal,
		Hard
	}
	public enum GameProcessType
	{
		STANDBY,
		START,
		END
	}
	[SerializeField]
	[Header("対象のHandSeal_Player")]
	public HandSeal_Player player;
	[SerializeField]
	[Header("対象のHandSeal_Enemy")]
	public HandSeal_Enemy[] enemys = new HandSeal_Enemy[3];
	[SerializeField]
	[Header("各Enemyのオブジェクト位置")]
	public Transform[] enemysPos = new Transform[3];
	[SerializeField]
	[Header("術エフェクトの親アンカ\u30fc")]
	private Transform gunAnchor;
	[SerializeField]
	[Header("術エフェクト_火遁")]
	private ParticleSystem psFireStyle;
	[SerializeField]
	[Header("術エフェクト_火遁2")]
	private ParticleSystem psFireStyle2;
	[SerializeField]
	[Header("術エフェクト_水遁")]
	private ParticleSystem psWaterStyle;
	[SerializeField]
	[Header("術エフェクト_水遁2")]
	private ParticleSystem psWaterStyle2;
	[SerializeField]
	[Header("術エフェクト_土遁")]
	private ParticleSystem psDirtStyle;
	[SerializeField]
	[Header("術エフェクト_土遁2")]
	private ParticleSystem psDirtStyle2;
	[SerializeField]
	[Header("術エフェクト_風遁")]
	private ParticleSystem psWindStyle;
	[SerializeField]
	[Header("術エフェクト_風遁2")]
	private ParticleSystem psWindStyle2;
	[SerializeField]
	[Header("術エフェクト_封印")]
	private ParticleSystem psBindStyle;
	[SerializeField]
	[Header("術エフェクト_封印2")]
	private ParticleSystem psBindStyle2;
	[SerializeField]
	[Header("術エフェクト_必殺技")]
	private HandSeal_SecretStyleEffect[] secretStyleEffect;
	[SerializeField]
	[Header("印エフェクト")]
	private HandSeal_SealEffectPlayer sealEffectPlayer;
	[SerializeField]
	[Header("手のアニメ\u30fcション")]
	private Animator handAnime;
	[SerializeField]
	[Header("次の入力が可能になるまでの待ち時間")]
	private float pushInterval = 0.1f;
	[SerializeField]
	[Header("プレイヤ\u30fc移動用のMyCinemachineDollyCart")]
	private MyCinemachineDollyCart dollyCart;
	[SerializeField]
	[Header("ビネットの色(ダメ\u30fcジ)")]
	private Color vignetteColor;
	[SerializeField]
	[Header("ビネットの色(必殺技)")]
	private Color vignetteColor_SecretStyle;
	[SerializeField]
	[Header("Cinemachine Impulse Source")]
	private CinemachineImpulseSource impulseSource;
	[SerializeField]
	[Header("CinemachineVirtualCamera(歩行時振動用)")]
	private CinemachineVirtualCamera cameraFP;
	[SerializeField]
	[Header("HandSeal_Animation")]
	private HandSeal_Animation anime;
	[SerializeField]
	[Header("奥義中に動きを一時停止するマテリアル(柳の木)")]
	private MeshRenderer[] mrTree;
	[SerializeField]
	[Header("奥義中に動きを一時停止するパ\u30fcティクル(篝火)")]
	private ParticleSystem[] psFire;
	[SerializeField]
	[Header("タイムボ\u30fcナスUIの親アンカ\u30fc")]
	public GameObject timeBonusUIAnchor;
	[SerializeField]
	[Header("タイムボ\u30fcナスのSpriteNumbersスクリプト")]
	private SpriteNumbers timeBonusSN;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	private bool isAttack;
	private HandSeal_Define.UserType userType;
	public GameProcessType processType;
	private int selectEnemy;
	private bool isLockOn;
	private int enemyExistRail;
	private bool isEnemyExistCol;
	private IEnumerator routine;
	private IEnumerator routineRun;
	private int railRandom;
	private bool isPushButtom;
	private EnemyLevel enemyLevel;
	private bool isPlayerMove;
	private int totalKillCount;
	private int existEnemyCount;
	private int normalLevelCount = 6;
	private int hardLevelCount = 12;
	private int vignetteID;
	public float secretGauge;
	private bool isSecretStylePlay;
	private CinemachineBasicMultiChannelPerlin perlin;
	private float sePitch;
	public int PlayerNo => playerNo;
	public HandSeal_Define.UserType UserType => userType;
	public int SelectEnemy => selectEnemy;
	public EnemyLevel EnemyLevelCheck => enemyLevel;
	public int TotalKillCount => totalKillCount;
	public bool IsSecretStylePlay => isSecretStylePlay;
	public void PlayerInit(HandSeal_Player _player)
	{
		userType = _player.UserType;
		ProcessTypeChange(GameProcessType.STANDBY);
		isLockOn = false;
		isAttack = false;
		HandSeal_Define.PM.SetPoint(userType, 0);
		dollyCart.m_Position = 0f;
		if (HandSeal_Define.PLAYER_NUM == 2)
		{
			handAnime.gameObject.transform.localPosition -= new Vector3(0f, 0.3f, 0f);
		}
		perlin = cameraFP.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		anime.SetAnim(HandSeal_Animation.AnimType.STANDBY);
		timeBonusUIAnchor.SetActive(value: false);
		timeBonusSN.Set(0);
	}
	public void StartMethod()
	{
		ProcessTypeChange(GameProcessType.START);
		enemys[1].AwakeEnemy();
	}
	private void Update()
	{
		if (processType == GameProcessType.STANDBY && HandSeal_Define.GM.IsStartCountDown && !handAnime.GetCurrentAnimatorStateInfo(0).IsName("ToHold"))
		{
			handAnime.SetTrigger("StandbyToHold");
		}
	}
	public void UpdateMethod()
	{
		if (isPlayerMove)
		{
			return;
		}
		secretGauge = Mathf.Clamp(secretGauge + Time.fixedDeltaTime * 0.05f, 0f, 1f);
		if (!isEnemyExistCol)
		{
			routine = null;
			routine = EnemyExist();
			StartCoroutine(routine);
			StageClearCheck();
		}
		if (isLockOn || isAttack)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < enemys.Length)
			{
				if (enemys[num].IsExist && !enemys[num].IsDeath)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		isLockOn = true;
		selectEnemy = num;
		enemys[num].LockOnEnemy(_set: true);
	}
	public void EnemyKillMethod()
	{
		totalKillCount++;
		if (totalKillCount == normalLevelCount)
		{
			StartCoroutine(EnemyLevelUpNormal());
		}
		if (totalKillCount == hardLevelCount)
		{
			StartCoroutine(EnemyLevelUpHard());
		}
		if (enemys[selectEnemy].IsExist)
		{
			return;
		}
		for (int i = 0; i < enemys.Length; i++)
		{
			if (enemys[i].IsExist && !enemys[i].IsDeath)
			{
				isLockOn = true;
				selectEnemy = i;
				enemys[i].LockOnEnemy(_set: true);
				return;
			}
		}
		isLockOn = false;
	}
	public void MoveCursor(MoveCursorDirection _dir)
	{
		switch (_dir)
		{
		case MoveCursorDirection.UP:
		case MoveCursorDirection.DOWN:
			break;
		case MoveCursorDirection.RIGHT:
			TargetMove(_dir);
			break;
		case MoveCursorDirection.LEFT:
			TargetMove(_dir);
			break;
		}
	}
	public void InputSeal(InputButton _input)
	{
		if (!isPushButtom)
		{
			enemys[selectEnemy].pushButton(_input);
		}
	}
	public void InputSealMiss()
	{
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_miss", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
		StartCoroutine(IsPushButtomWait());
	}
	public void HandSeal(Kuji _input)
	{
		switch (_input)
		{
		case Kuji.Rin:
			handAnime.SetTrigger("ToSeal_0");
			break;
		case Kuji.Hyou:
			handAnime.SetTrigger("ToSeal_1");
			break;
		case Kuji.Tou:
			handAnime.SetTrigger("ToSeal_2");
			break;
		case Kuji.Sya:
			handAnime.SetTrigger("ToSeal_3");
			break;
		case Kuji.Kai:
			handAnime.SetTrigger("ToSeal_4");
			break;
		case Kuji.Jin:
			handAnime.SetTrigger("ToSeal_5");
			break;
		case Kuji.Retsu:
			handAnime.SetTrigger("ToSeal_6");
			break;
		case Kuji.Zai:
			handAnime.SetTrigger("ToSeal_7");
			break;
		case Kuji.Zen:
			handAnime.SetTrigger("ToSeal_8");
			break;
		case Kuji.Hold:
			if (!handAnime.GetCurrentAnimatorStateInfo(0).IsName("Hold") && !handAnime.GetCurrentAnimatorStateInfo(0).IsName("ToHold"))
			{
				handAnime.SetTrigger("ToHold");
			}
			break;
		}
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4 && _input != Kuji.Hold)
		{
			sePitch = UnityEngine.Random.Range(0.8f, 1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_result_scroll", _loop: false, 0f, 1f, sePitch, 0f, _overlap: true);
			SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_sealeffectcorrect", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
	}
	public void ProcessTypeChange(GameProcessType _type)
	{
		switch (_type)
		{
		case GameProcessType.STANDBY:
			processType = GameProcessType.STANDBY;
			break;
		case GameProcessType.START:
			processType = GameProcessType.START;
			break;
		case GameProcessType.END:
		{
			if (processType == GameProcessType.END)
			{
				break;
			}
			HandSeal(Kuji.Hold);
			processType = GameProcessType.END;
			StopAllCoroutines();
			LeanTween.cancel(base.gameObject);
			int num = 0;
			for (int i = 0; i < HandSeal_Define.PM.UserData_Group1.Length; i++)
			{
				if (HandSeal_Define.PM.UserData_Group1[playerNo].point < HandSeal_Define.PM.UserData_Group1[i].point)
				{
					num++;
				}
			}
			switch (num)
			{
			case 0:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.HAPPY);
				anime.SetAnim(HandSeal_Animation.AnimType.WINNER);
				break;
			case 1:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SMILE);
				anime.SetAnim(HandSeal_Animation.AnimType.JOY);
				break;
			case 2:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
				anime.SetAnim(HandSeal_Animation.AnimType.FIGHT);
				break;
			case 3:
				player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
				anime.SetAnim(HandSeal_Animation.AnimType.SAD);
				break;
			}
			break;
		}
		}
	}
	private void TargetMove(MoveCursorDirection _dir)
	{
		if (!isLockOn || isSecretStylePlay)
		{
			return;
		}
		switch (_dir)
		{
		case MoveCursorDirection.RIGHT:
			switch (selectEnemy)
			{
			case 2:
				break;
			case 0:
				if (enemys[1].IsExist && !enemys[1].IsDeath)
				{
					enemys[0].LockOnEnemy(_set: false);
					enemys[1].LockOnEnemy(_set: true);
					selectEnemy = 1;
				}
				else if (enemys[2].IsExist && !enemys[2].IsDeath)
				{
					enemys[0].LockOnEnemy(_set: false);
					enemys[2].LockOnEnemy(_set: true);
					selectEnemy = 2;
				}
				break;
			case 1:
				if (enemys[2].IsExist && !enemys[2].IsDeath)
				{
					enemys[1].LockOnEnemy(_set: false);
					enemys[2].LockOnEnemy(_set: true);
					selectEnemy = 2;
				}
				break;
			}
			break;
		case MoveCursorDirection.LEFT:
			switch (selectEnemy)
			{
			case 0:
				break;
			case 1:
				if (enemys[0].IsExist && !enemys[0].IsDeath)
				{
					enemys[1].LockOnEnemy(_set: false);
					enemys[0].LockOnEnemy(_set: true);
					selectEnemy = 0;
				}
				break;
			case 2:
				if (enemys[1].IsExist && !enemys[1].IsDeath)
				{
					enemys[2].LockOnEnemy(_set: false);
					enemys[1].LockOnEnemy(_set: true);
					selectEnemy = 1;
				}
				else if (enemys[0].IsExist && !enemys[0].IsDeath)
				{
					enemys[2].LockOnEnemy(_set: false);
					enemys[0].LockOnEnemy(_set: true);
					selectEnemy = 0;
				}
				break;
			}
			break;
		}
	}
	public void GunTargetLookOn()
	{
		switch (selectEnemy)
		{
		case 0:
			gunAnchor.LookAt(enemysPos[0]);
			break;
		case 1:
			gunAnchor.SetLocalEulerAnglesY(0f);
			break;
		case 2:
			gunAnchor.LookAt(enemysPos[2]);
			break;
		}
	}
	public void ShotNinjutsu(Ninjutsu _set)
	{
		switch (_set)
		{
		case Ninjutsu.FireStyle:
			psFireStyle.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_firethrow", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_firehit", _loop: false, 0f, 1f, 1f, 0.5f, _overlap: true);
			}
			break;
		case Ninjutsu.FireStyle2:
			psFireStyle2.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_firethrow", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_firehit", _loop: false, 0f, 1f, 1f, 0.5f, _overlap: true);
			}
			break;
		case Ninjutsu.WaterStyle:
			psWaterStyle.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_waterthrow", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_waterhit", _loop: false, 0f, 1f, 1f, 0.4f, _overlap: true);
			}
			break;
		case Ninjutsu.WaterStyle2:
			psWaterStyle2.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_waterhit", _loop: false, 0f, 1f, 1f, 0.3f, _overlap: true);
			}
			break;
		case Ninjutsu.DirtStyle:
			psDirtStyle.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_earthappear", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_earthcrack", _loop: false, 0f, 1f, 1f, 0.3f, _overlap: true);
			}
			break;
		case Ninjutsu.DirtStyle2:
			psDirtStyle2.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_earthinstantiate", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_earthcrack", _loop: false, 0f, 1f, 1f, 0.5f, _overlap: true);
			}
			break;
		case Ninjutsu.WindStyle:
			psWindStyle.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_windthrow", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_windhit_1", _loop: false, 0f, 1f, 1f, 0.5f, _overlap: true);
			}
			break;
		case Ninjutsu.WindStyle2:
			psWindStyle2.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_windhit_0", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			}
			break;
		case Ninjutsu.BindStyle:
			psBindStyle.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_sealingbind", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			}
			break;
		case Ninjutsu.BindStyle2:
			psBindStyle2.Play();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_sealingmagic", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			}
			break;
		case Ninjutsu.SecretStyle:
			switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
			{
			case 0:
				secretStyleEffect[0].Play();
				break;
			case 1:
				secretStyleEffect[1].Play();
				break;
			case 2:
				secretStyleEffect[2].Play();
				break;
			case 3:
				secretStyleEffect[3].Play();
				break;
			case 4:
				secretStyleEffect[4].Play();
				break;
			case 5:
				secretStyleEffect[5].Play();
				break;
			case 6:
				secretStyleEffect[6].Play();
				break;
			case 7:
				secretStyleEffect[7].Play();
				break;
			}
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_secretexplode2", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, 1f);
			}
			break;
		}
	}
	public void PlaySealEffect(Kuji _set)
	{
		switch (_set)
		{
		case Kuji.Rin:
			sealEffectPlayer.PlaySealEffect(Kuji.Rin);
			break;
		case Kuji.Hyou:
			sealEffectPlayer.PlaySealEffect(Kuji.Hyou);
			break;
		case Kuji.Tou:
			sealEffectPlayer.PlaySealEffect(Kuji.Tou);
			break;
		case Kuji.Sya:
			sealEffectPlayer.PlaySealEffect(Kuji.Sya);
			break;
		case Kuji.Kai:
			sealEffectPlayer.PlaySealEffect(Kuji.Kai);
			break;
		case Kuji.Jin:
			sealEffectPlayer.PlaySealEffect(Kuji.Jin);
			break;
		case Kuji.Retsu:
			sealEffectPlayer.PlaySealEffect(Kuji.Retsu);
			break;
		case Kuji.Zai:
			sealEffectPlayer.PlaySealEffect(Kuji.Zai);
			break;
		case Kuji.Zen:
			sealEffectPlayer.PlaySealEffect(Kuji.Zen);
			break;
		}
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_kuji", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
	}
	public void DamageVignette()
	{
		if (HandSeal_Define.GM.IsDuringGame())
		{
			LeanTween.cancel(vignetteID);
			vignetteID = LeanTween.value(base.gameObject, 0.6f, 0f, 1f).setOnUpdate((Action<float>)delegate
			{
			}).id;
			impulseSource.GenerateImpulse();
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
			}
		}
	}
	public void SecretStyleAction()
	{
		if (!isLockOn || enemys[selectEnemy].IsDeath || isSecretStylePlay || secretGauge < 1f)
		{
			return;
		}
		isSecretStylePlay = true;
		enemys[0].SecretStyleAttackReady();
		enemys[1].SecretStyleAttackReady();
		enemys[2].SecretStyleAttackReady();
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_ougi", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
		if (mrTree != null)
		{
			for (int i = 0; i < mrTree.Length; i++)
			{
				mrTree[i].materials[0].SetFloat("_Big_Windspeed", 0f);
				mrTree[i].materials[0].SetFloat("_Small_WindSpeed", 0f);
				mrTree[i].materials[1].SetFloat("_Big_Windspeed", 0f);
				mrTree[i].materials[1].SetFloat("_Small_WindSpeed", 0f);
			}
		}
		if (psFire != null)
		{
			for (int j = 0; j < psFire.Length; j++)
			{
				psFire[j].playbackSpeed = 0f;
			}
		}
	}
	public void SecretStylePlay()
	{
		if (!isSecretStylePlay)
		{
			return;
		}
		gunAnchor.SetLocalEulerAnglesY(0f);
		ShotNinjutsu(Ninjutsu.SecretStyle);
		StartCoroutine(SecretStyleImpulse());
		enemys[0].SecretStyleAttack();
		enemys[1].SecretStyleAttack();
		enemys[2].SecretStyleAttack();
		isSecretStylePlay = false;
		isLockOn = false;
		LeanTween.value(base.gameObject, 0.7f, 0f, 0.5f).setOnUpdate((Action<float>)delegate
		{
		}).setOnComplete((Action)delegate
		{
		});
		secretGauge = 0f;
		if (mrTree != null)
		{
			for (int i = 0; i < mrTree.Length; i++)
			{
				mrTree[i].materials[0].SetFloat("_Big_Windspeed", 0.75f);
				mrTree[i].materials[0].SetFloat("_Small_WindSpeed", 0f);
				mrTree[i].materials[1].SetFloat("_Big_Windspeed", 0.75f);
				mrTree[i].materials[1].SetFloat("_Small_WindSpeed", 0.1f);
			}
		}
		if (psFire != null)
		{
			for (int j = 0; j < psFire.Length; j++)
			{
				psFire[j].playbackSpeed = 1f;
			}
		}
	}
	public void SecretStyleCounter()
	{
		existEnemyCount = 0;
		for (int i = 0; i < enemys.Length; i++)
		{
			if (enemys[i].IsExist && !enemys[i].IsDeath)
			{
				existEnemyCount++;
			}
		}
		if (existEnemyCount == 3 && secretGauge == 1f)
		{
			SecretStyleAction();
		}
	}
	private void StageClearCheck()
	{
		if (processType != GameProcessType.END && enemys[0].IsAllKill && enemys[1].IsAllKill && enemys[2].IsAllKill)
		{
			HandSeal_Define.PM.SetPoint(userType, HandSeal_Define.GM.GetTimeBonusPoint());
			timeBonusSN.Set(HandSeal_Define.GM.GetTimeBonusPoint());
			timeBonusUIAnchor.SetActive(value: true);
			ProcessTypeChange(GameProcessType.END);
			if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cracker", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
			}
			if (HandSeal_Define.PM.PlayerGoalCheck())
			{
				HandSeal_Define.GM.GameEnd();
			}
		}
	}
	private bool LastBossCheck()
	{
		if (enemys[1].KillCount < 7)
		{
			return true;
		}
		if (enemys[1].KillCount == 7)
		{
			if (totalKillCount < 24)
			{
				return false;
			}
			int totalKillCount2 = totalKillCount;
			return true;
		}
		return true;
	}
	private IEnumerator EnemyExist()
	{
		isEnemyExistCol = true;
		existEnemyCount = 0;
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < enemys.Length; i++)
		{
			if (enemys[i].IsExist || enemys[i].IsExistAnime || enemys[i].IsBossExist)
			{
				existEnemyCount++;
			}
		}
		switch (enemyLevel)
		{
		case EnemyLevel.Easy:
			if (existEnemyCount < 1 && totalKillCount + existEnemyCount < normalLevelCount)
			{
				switch (selectEnemy)
				{
				case 0:
					enemys[1].AwakeEnemy();
					break;
				case 1:
					enemys[2].AwakeEnemy();
					break;
				case 2:
					enemys[0].AwakeEnemy();
					break;
				}
			}
			break;
		case EnemyLevel.Normal:
			if (existEnemyCount < 2 && totalKillCount + existEnemyCount < hardLevelCount)
			{
				switch (selectEnemy)
				{
				case 0:
					enemys[1].AwakeEnemy();
					break;
				case 1:
					enemys[2].AwakeEnemy();
					break;
				case 2:
					enemys[0].AwakeEnemy();
					break;
				}
			}
			break;
		case EnemyLevel.Hard:
			if (existEnemyCount >= 3)
			{
				break;
			}
			switch (selectEnemy)
			{
			case 0:
				if (!enemys[1].IsExist && !enemys[1].IsAllKill && LastBossCheck())
				{
					enemys[1].AwakeEnemy();
				}
				else if (!enemys[2].IsExist && !enemys[2].IsAllKill)
				{
					enemys[2].AwakeEnemy();
				}
				else if (!enemys[0].IsExist && !enemys[0].IsAllKill)
				{
					enemys[0].AwakeEnemy();
				}
				break;
			case 1:
				if (!enemys[2].IsExist && !enemys[2].IsAllKill)
				{
					enemys[2].AwakeEnemy();
				}
				else if (!enemys[0].IsExist && !enemys[0].IsAllKill)
				{
					enemys[0].AwakeEnemy();
				}
				else if (!enemys[1].IsExist && !enemys[1].IsAllKill && LastBossCheck())
				{
					enemys[1].AwakeEnemy();
				}
				break;
			case 2:
				if (!enemys[0].IsExist && !enemys[0].IsAllKill)
				{
					enemys[0].AwakeEnemy();
				}
				else if (!enemys[1].IsExist && !enemys[1].IsAllKill && LastBossCheck())
				{
					enemys[1].AwakeEnemy();
				}
				else if (!enemys[2].IsExist && !enemys[2].IsAllKill)
				{
					enemys[2].AwakeEnemy();
				}
				break;
			}
			break;
		}
		isEnemyExistCol = false;
	}
	private IEnumerator IsPushButtomWait()
	{
		isPushButtom = true;
		yield return new WaitForSeconds(pushInterval);
		isPushButtom = false;
	}
	private IEnumerator EnemyLevelUpNormal()
	{
		isPlayerMove = true;
		StopCoroutine(routine);
		enemyLevel = EnemyLevel.Normal;
		yield return new WaitForSeconds(1f);
		handAnime.SetTrigger("ToRun");
		LeanTween.value(base.gameObject, 0f, 2f, 1f).setOnUpdate(delegate(float val)
		{
			perlin.m_AmplitudeGain = val;
		});
		LeanTween.value(base.gameObject, 0f, 40f, 4f).setOnUpdate(delegate(float val)
		{
			dollyCart.m_Position = val;
		});
		LeanTween.value(base.gameObject, 2f, 0f, 1f).setOnUpdate(delegate(float val)
		{
			perlin.m_AmplitudeGain = val;
		}).setDelay(3f);
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
		{
			routineRun = null;
			routineRun = RunSePlay();
			StartCoroutine(routineRun);
		}
		yield return new WaitForSeconds(4f);
		handAnime.SetTrigger("ToHold");
		isPlayerMove = false;
		enemys[0].AwakeEnemy();
		enemys[2].AwakeEnemy();
		isEnemyExistCol = false;
	}
	private IEnumerator EnemyLevelUpHard()
	{
		isPlayerMove = true;
		StopCoroutine(routine);
		enemyLevel = EnemyLevel.Hard;
		yield return new WaitForSeconds(1f);
		handAnime.SetTrigger("ToRun");
		LeanTween.value(base.gameObject, 0f, 2f, 1f).setOnUpdate(delegate(float val)
		{
			perlin.m_AmplitudeGain = val;
		});
		LeanTween.value(base.gameObject, 40f, 80f, 4f).setOnUpdate(delegate(float val)
		{
			dollyCart.m_Position = val;
		});
		LeanTween.value(base.gameObject, 2f, 0f, 1f).setOnUpdate(delegate(float val)
		{
			perlin.m_AmplitudeGain = val;
		}).setDelay(3f);
		if (HandSeal_Define.GM.IsDuringGame() && player.UserType <= HandSeal_Define.UserType.PLAYER_4)
		{
			routineRun = null;
			routineRun = RunSePlay();
			StartCoroutine(routineRun);
		}
		yield return new WaitForSeconds(4f);
		handAnime.SetTrigger("ToHold");
		isPlayerMove = false;
		enemys[0].AwakeEnemy();
		enemys[1].AwakeEnemy();
		enemys[2].AwakeEnemy();
		isEnemyExistCol = false;
	}
	private IEnumerator RunSePlay()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 0.9f, 0f, _overlap: true);
		yield return new WaitForSeconds(0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
	}
	private IEnumerator SecretStyleImpulse()
	{
		yield return new WaitForSeconds(0.6f);
		impulseSource.GenerateImpulse();
		yield return new WaitForSeconds(0.25f);
		impulseSource.GenerateImpulse();
		yield return new WaitForSeconds(0.25f);
		impulseSource.GenerateImpulse();
	}
	private void OnDestroy()
	{
		if (routineRun != null)
		{
			StopCoroutine(routineRun);
		}
	}
}
