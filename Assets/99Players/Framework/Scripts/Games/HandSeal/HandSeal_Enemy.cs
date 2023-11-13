using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
public class HandSeal_Enemy : MonoBehaviour
{
	[Serializable]
	public struct ButtonData
	{
		public HandSeal_Hand.InputButton inputButton;
		public GameObject buttonQuestion;
		public GameObject buttonAnswer;
	}
	public enum EnemyExistAnimeType
	{
		WALK,
		JUMP
	}
	[SerializeField]
	[Header("対応するHandSeal_Hand")]
	private HandSeal_Hand hand;
	[SerializeField]
	[Header("敵オブジェクトの親アンカ\u30fc")]
	private GameObject enemyAnchor;
	[SerializeField]
	[Header("敵(通常)のオブジェクト")]
	[Header("《敵(通常)の設定》")]
	private GameObject enemyCommon;
	[SerializeField]
	[Header("敵(通常)のアニメ\u30fcタ\u30fc")]
	private Animator animatorCommon;
	[SerializeField]
	[Header("敵(通常)の表情設定用MeshRenderer")]
	private MeshRenderer mrCommon;
	[SerializeField]
	[Header("敵(通常)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataCommonAnchor;
	[SerializeField]
	[Header("敵(通常)のボタン指示")]
	private ButtonData[] buttonDataCommon;
	[SerializeField]
	[Header("敵(通常2)のオブジェクト")]
	[Header("《敵(通常2)の設定》")]
	private GameObject enemyCommon2;
	[SerializeField]
	[Header("敵(通常2)のアニメ\u30fcタ\u30fc")]
	private Animator animatorCommon2;
	[SerializeField]
	[Header("敵(通常2)の表情設定用MeshRenderer")]
	private MeshRenderer mrCommon2;
	[SerializeField]
	[Header("敵(通常2)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataCommonAnchor2;
	[SerializeField]
	[Header("敵(通常2)のボタン指示")]
	private ButtonData[] buttonDataCommon2;
	[SerializeField]
	[Header("敵(速い)のオブジェクト")]
	[Header("《敵(速い)の設定》")]
	private GameObject enemySpeed;
	[SerializeField]
	[Header("敵(速い)のアニメ\u30fcタ\u30fc")]
	private Animator animatorSpeed;
	[SerializeField]
	[Header("敵(速い)の表情設定用MeshRenderer")]
	private MeshRenderer mrSpeed;
	[SerializeField]
	[Header("敵(速い)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataSpeedAnchor;
	[SerializeField]
	[Header("敵(速い)のボタン指示")]
	private ButtonData[] buttonDataSpeed;
	[SerializeField]
	[Header("敵(速い2)のオブジェクト")]
	[Header("《敵(速い2)の設定》")]
	private GameObject enemySpeed2;
	[SerializeField]
	[Header("敵(速い2)のアニメ\u30fcタ\u30fc")]
	private Animator animatorSpeed2;
	[SerializeField]
	[Header("敵(速い2)の表情設定用MeshRenderer")]
	private MeshRenderer mrSpeed2;
	[SerializeField]
	[Header("敵(速い2)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataSpeedAnchor2;
	[SerializeField]
	[Header("敵(速い2)のボタン指示")]
	private ButtonData[] buttonDataSpeed2;
	[SerializeField]
	[Header("敵(強い)のオブジェクト")]
	[Header("《敵(強い)の設定》")]
	private GameObject enemyStrong;
	[SerializeField]
	[Header("敵(強い)のアニメ\u30fcタ\u30fc")]
	private Animator animatorStrong;
	[SerializeField]
	[Header("敵(強い)の表情設定用MeshRenderer")]
	private MeshRenderer mrStrong;
	[SerializeField]
	[Header("敵(強い)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataStrongAnchor;
	[SerializeField]
	[Header("敵(強い)のボタン指示")]
	private ButtonData[] buttonDataStrong;
	[SerializeField]
	[Header("敵(強い2)のオブジェクト")]
	[Header("《敵(強い2)の設定》")]
	private GameObject enemyStrong2;
	[SerializeField]
	[Header("敵(強い2)のアニメ\u30fcタ\u30fc")]
	private Animator animatorStrong2;
	[SerializeField]
	[Header("敵(強い2)の表情設定用MeshRenderer")]
	private MeshRenderer mrStrong2;
	[SerializeField]
	[Header("敵(強い2)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataStrongAnchor2;
	[SerializeField]
	[Header("敵(強い2)のボタン指示")]
	private ButtonData[] buttonDataStrong2;
	[SerializeField]
	[Header("敵(タフ)のオブジェクト")]
	[Header("《敵(タフ)の設定》")]
	private GameObject enemyTough;
	[SerializeField]
	[Header("敵(タフ)のアニメ\u30fcタ\u30fc")]
	private Animator animatorTough;
	[SerializeField]
	[Header("敵(タフ)の表情設定用MeshRenderer")]
	private MeshRenderer mrTough;
	[SerializeField]
	[Header("敵(タフ)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataToughAnchor;
	[SerializeField]
	[Header("敵(タフ)のボタン指示")]
	private ButtonData[] buttonDataTough;
	[SerializeField]
	[Header("敵(タフ2)のオブジェクト")]
	[Header("《敵(タフ2)の設定》")]
	private GameObject enemyTough2;
	[SerializeField]
	[Header("敵(タフ2)のアニメ\u30fcタ\u30fc")]
	private Animator animatorTough2;
	[SerializeField]
	[Header("敵(タフ2)の表情設定用MeshRenderer")]
	private MeshRenderer mrTough2;
	[SerializeField]
	[Header("敵(タフ2)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataToughAnchor2;
	[SerializeField]
	[Header("敵(タフ2)のボタン指示")]
	private ButtonData[] buttonDataTough2;
	[SerializeField]
	[Header("敵(ボス)のオブジェクト")]
	[Header("《敵(ボス)の設定》")]
	private GameObject enemyBoss;
	[SerializeField]
	[Header("敵(ボス)のアニメ\u30fcタ\u30fc")]
	private Animator animatorBoss;
	[SerializeField]
	[Header("敵(ボス)の表情設定用MeshRenderer")]
	private MeshRenderer mrBoss;
	[SerializeField]
	[Header("敵(ボス)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataBossAnchor;
	[SerializeField]
	[Header("敵(ボス)のボタン指示")]
	private ButtonData[] buttonDataBoss;
	[SerializeField]
	[Header("敵(ボス2)のオブジェクト")]
	[Header("《敵(ボス2)の設定》")]
	private GameObject enemyBoss2;
	[SerializeField]
	[Header("敵(ボス2)のアニメ\u30fcタ\u30fc")]
	private Animator animatorBoss2;
	[SerializeField]
	[Header("敵(ボス2)の表情設定用MeshRenderer")]
	private MeshRenderer mrBoss2;
	[SerializeField]
	[Header("敵(ボス2)のボタンUIの親アンカ\u30fc")]
	private GameObject enemyButtonDataBossAnchor2;
	[SerializeField]
	[Header("敵(ボス2)のボタン指示")]
	private ButtonData[] buttonDataBoss2;
	[SerializeField]
	[Header("敵ラインの番号(0~2)")]
	[Space(10f)]
	private int enemyLineNo;
	[SerializeField]
	[Header("CinemachineSmoothPath(前進レ\u30fcン)")]
	private CinemachineSmoothPath smoothPath;
	[SerializeField]
	[Header("CinemachineSmoothPath(Easy_左側)")]
	private CinemachineSmoothPath[] smoothPath_Easy_Left;
	[SerializeField]
	[Header("CinemachineSmoothPath(Easy_正面)")]
	private CinemachineSmoothPath[] smoothPath_Easy_Front;
	[SerializeField]
	[Header("CinemachineSmoothPath(Easy_右側)")]
	private CinemachineSmoothPath[] smoothPath_Easy_Right;
	[SerializeField]
	[Header("CinemachineSmoothPath(Normal_左側)")]
	private CinemachineSmoothPath[] smoothPath_Normal_Left;
	[SerializeField]
	[Header("CinemachineSmoothPath(Normal_正面)")]
	private CinemachineSmoothPath[] smoothPath_Normal_Front;
	[SerializeField]
	[Header("CinemachineSmoothPath(Normal_右側)")]
	private CinemachineSmoothPath[] smoothPath_Normal_Right;
	[SerializeField]
	[Header("CinemachineSmoothPath(Hard_左側)")]
	private CinemachineSmoothPath[] smoothPath_Hard_Left;
	[SerializeField]
	[Header("CinemachineSmoothPath(Hard_正面)")]
	private CinemachineSmoothPath[] smoothPath_Hard_Front;
	[SerializeField]
	[Header("CinemachineSmoothPath(Hard_右側)")]
	private CinemachineSmoothPath[] smoothPath_Hard_Right;
	[SerializeField]
	[Header("MyCinemachineDollyCart")]
	private MyCinemachineDollyCart dollyCart;
	[SerializeField]
	[Header("ロックオン表示用のエフェクト")]
	public ParticleSystem psLockOn;
	[SerializeField]
	[Header("接触判定用のコライダ\u30fc")]
	private BoxCollider boxCol;
	[SerializeField]
	[Header("必殺技ボタンUIの親アンカ\u30fc")]
	private GameObject secretStyleButtonAnchor;
	[SerializeField]
	[Header("必殺技のボタン指示")]
	private ButtonData[] buttonSecretStyle;
	[SerializeField]
	[Header("表情変更用テクスチャ(男_通常)")]
	private Texture[] texBoyFaceNormal;
	[SerializeField]
	[Header("表情変更用テクスチャ(女_通常)")]
	private Texture[] texGirlFaceNormal;
	[SerializeField]
	[Header("表情変更用テクスチャ(男_やられ)")]
	private Texture texBoyFaceSorrow;
	[SerializeField]
	[Header("表情変更用テクスチャ(女_やられ)")]
	private Texture texGirlFaceSorrow;
	private HandSeal_EnemyManager.EnemyData enemyData;
	private int killCount;
	private int needInputCount;
	private int pushCorrectCount;
	private float pathLength;
	private float animePathLength;
	private bool isExist;
	private bool isExistAnime;
	private bool isBossExist;
	private bool isDeath;
	private bool isAttack;
	private bool isAttackPlay;
	private bool isDeathAnime;
	private IEnumerator routine;
	private IEnumerator routineBoss;
	private IEnumerator routineAttack;
	private bool isLocked;
	private bool isSecretStyleAttack;
	private bool isAllKill;
	private int faceNum;
	private int animeNum;
	private int idAnime;
	private EnemyExistAnimeType enemyExistAnime;
	public ButtonData[] ButtonDataCommon => buttonDataCommon;
	public ButtonData[] ButtonDataCommon2 => buttonDataCommon2;
	public ButtonData[] ButtonDataSpeed => buttonDataSpeed;
	public ButtonData[] ButtonDataSpeed2 => buttonDataSpeed2;
	public ButtonData[] ButtonDataStrong => buttonDataStrong;
	public ButtonData[] ButtonDataStrong2 => buttonDataStrong2;
	public ButtonData[] ButtonDataTough => buttonDataTough;
	public ButtonData[] ButtonDataTough2 => buttonDataTough2;
	public ButtonData[] ButtonDataBoss => buttonDataBoss;
	public ButtonData[] ButtonDataBoss2 => buttonDataBoss2;
	public ButtonData[] ButtonSecretStyle => buttonSecretStyle;
	public HandSeal_EnemyManager.EnemyData EnemyData => enemyData;
	public int KillCount => killCount;
	public int PushCorrectCount => pushCorrectCount;
	public bool IsExist => isExist;
	public bool IsExistAnime => isExistAnime;
	public bool IsBossExist => isBossExist;
	public bool IsDeath => isDeath;
	public bool IsAttack => isAttack;
	public bool IsAllKill => isAllKill;
	private void Start()
	{
		killCount = 0;
		isExist = false;
		isExistAnime = false;
		isBossExist = false;
		isAttack = false;
		isDeathAnime = false;
		pathLength = smoothPath.PathLength;
		pushCorrectCount = 0;
		isAllKill = false;
		SetEnemy();
	}
	private void Update()
	{
		if (!isAttack && isExist && dollyCart.m_Position == pathLength)
		{
			isAttack = true;
			animeNum = UnityEngine.Random.Range(0, 3);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.COMMON:
				switch (animeNum)
				{
				case 0:
					animatorCommon.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorCommon.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorCommon.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.COMMON_2:
				switch (animeNum)
				{
				case 0:
					animatorCommon2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorCommon2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorCommon2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED:
				switch (animeNum)
				{
				case 0:
					animatorSpeed.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorSpeed.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorSpeed.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED_2:
				switch (animeNum)
				{
				case 0:
					animatorSpeed2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorSpeed2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorSpeed2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG:
				switch (animeNum)
				{
				case 0:
					animatorStrong.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorStrong.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorStrong.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG_2:
				switch (animeNum)
				{
				case 0:
					animatorStrong2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorStrong2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorStrong2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH:
				switch (animeNum)
				{
				case 0:
					animatorTough.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorTough.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorTough.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH_2:
				switch (animeNum)
				{
				case 0:
					animatorTough2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorTough2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorTough2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS:
				switch (animeNum)
				{
				case 0:
					animatorBoss.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorBoss.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorBoss.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				switch (animeNum)
				{
				case 0:
					animatorBoss2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorBoss2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorBoss2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			}
		}
		else if (isAttack && !isAttackPlay && !isSecretStyleAttack)
		{
			routineAttack = EnemyAttack();
			StartCoroutine(routineAttack);
		}
	}
	public void SetEnemy()
	{
		if (boxCol != null)
		{
			boxCol.enabled = false;
		}
		enemyAnchor.SetActive(value: false);
		enemyCommon.SetActive(value: false);
		enemyCommon2.SetActive(value: false);
		enemySpeed.SetActive(value: false);
		enemySpeed2.SetActive(value: false);
		enemyStrong.SetActive(value: false);
		enemyStrong2.SetActive(value: false);
		enemyTough.SetActive(value: false);
		enemyTough2.SetActive(value: false);
		enemyBoss.SetActive(value: false);
		enemyBoss2.SetActive(value: false);
		enemyData = SingletonCustom<HandSeal_EnemyManager>.Instance.GetEnemyData(enemyLineNo, killCount);
		faceNum = UnityEngine.Random.Range(0, 5);
		switch (enemyData.enemyType)
		{
		case HandSeal_EnemyManager.EnemyType.COMMON:
			enemyCommon.SetActive(value: true);
			needInputCount = buttonDataCommon.Length;
			mrCommon.material.SetTexture("_FaceTex", texBoyFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.COMMON_2:
			enemyCommon2.SetActive(value: true);
			needInputCount = buttonDataCommon2.Length;
			mrCommon2.material.SetTexture("_FaceTex", texGirlFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.SPEED:
			enemySpeed.SetActive(value: true);
			needInputCount = buttonDataSpeed.Length;
			mrSpeed.material.SetTexture("_FaceTex", texBoyFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.SPEED_2:
			enemySpeed2.SetActive(value: true);
			needInputCount = buttonDataSpeed2.Length;
			mrSpeed2.material.SetTexture("_FaceTex", texGirlFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.STRONG:
			enemyStrong.SetActive(value: true);
			needInputCount = buttonDataStrong.Length;
			mrStrong.material.SetTexture("_FaceTex", texBoyFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.STRONG_2:
			enemyStrong2.SetActive(value: true);
			needInputCount = buttonDataStrong2.Length;
			mrStrong2.material.SetTexture("_FaceTex", texGirlFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.TOUGH:
			enemyTough.SetActive(value: true);
			needInputCount = buttonDataTough.Length;
			mrTough.material.SetTexture("_FaceTex", texBoyFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.TOUGH_2:
			enemyTough2.SetActive(value: true);
			needInputCount = buttonDataTough2.Length;
			mrTough2.material.SetTexture("_FaceTex", texGirlFaceNormal[faceNum]);
			break;
		case HandSeal_EnemyManager.EnemyType.BOSS:
			enemyBoss.SetActive(value: true);
			needInputCount = buttonDataBoss.Length;
			mrBoss.material.SetTexture("_FaceTex", texBoyFaceNormal[0]);
			break;
		case HandSeal_EnemyManager.EnemyType.BOSS_2:
			enemyBoss2.SetActive(value: true);
			needInputCount = buttonDataBoss2.Length;
			mrBoss2.material.SetTexture("_FaceTex", texBoyFaceNormal[0]);
			break;
		case HandSeal_EnemyManager.EnemyType.MAX:
			isAllKill = true;
			break;
		}
		pushCorrectCount = 0;
		isExist = false;
		isExistAnime = false;
		isBossExist = false;
		isAttack = false;
		dollyCart.m_Speed = 0f;
		dollyCart.m_Position = 0f;
		isDeath = false;
	}
	public void AwakeEnemy()
	{
		if (isExist || isExistAnime || isBossExist || isSecretStyleAttack || isAttackPlay || isAllKill)
		{
			return;
		}
		switch (enemyData.enemyType)
		{
		case HandSeal_EnemyManager.EnemyType.COMMON:
		case HandSeal_EnemyManager.EnemyType.COMMON_2:
		case HandSeal_EnemyManager.EnemyType.SPEED:
		case HandSeal_EnemyManager.EnemyType.SPEED_2:
		case HandSeal_EnemyManager.EnemyType.STRONG:
		case HandSeal_EnemyManager.EnemyType.STRONG_2:
		case HandSeal_EnemyManager.EnemyType.TOUGH:
		case HandSeal_EnemyManager.EnemyType.TOUGH_2:
			switch (hand.EnemyLevelCheck)
			{
			case HandSeal_Hand.EnemyLevel.Easy:
				switch (enemyLineNo)
				{
				case 0:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Easy_Left.Length);
					dollyCart.m_Path = smoothPath_Easy_Left[animeNum];
					animePathLength = smoothPath_Easy_Left[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 1:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Easy_Front.Length);
					dollyCart.m_Path = smoothPath_Easy_Front[animeNum];
					animePathLength = smoothPath_Easy_Front[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 2:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Easy_Right.Length);
					dollyCart.m_Path = smoothPath_Easy_Right[animeNum];
					animePathLength = smoothPath_Easy_Right[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				}
				break;
			case HandSeal_Hand.EnemyLevel.Normal:
				switch (enemyLineNo)
				{
				case 0:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Normal_Left.Length);
					dollyCart.m_Path = smoothPath_Normal_Left[animeNum];
					animePathLength = smoothPath_Normal_Left[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 1:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Normal_Front.Length);
					dollyCart.m_Path = smoothPath_Normal_Front[animeNum];
					animePathLength = smoothPath_Normal_Front[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 2:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Normal_Right.Length);
					dollyCart.m_Path = smoothPath_Normal_Right[animeNum];
					animePathLength = smoothPath_Normal_Right[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.WALK;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				}
				break;
			case HandSeal_Hand.EnemyLevel.Hard:
				switch (enemyLineNo)
				{
				case 0:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Hard_Left.Length);
					dollyCart.m_Path = smoothPath_Hard_Left[animeNum];
					animePathLength = smoothPath_Hard_Left[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 1:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Hard_Front.Length);
					dollyCart.m_Path = smoothPath_Hard_Front[animeNum];
					animePathLength = smoothPath_Hard_Front[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				case 2:
					animeNum = UnityEngine.Random.Range(0, smoothPath_Hard_Right.Length);
					dollyCart.m_Path = smoothPath_Hard_Right[animeNum];
					animePathLength = smoothPath_Hard_Right[animeNum].PathLength;
					switch (animeNum)
					{
					case 0:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					case 1:
						enemyExistAnime = EnemyExistAnimeType.JUMP;
						break;
					}
					break;
				}
				break;
			}
			isExistAnime = true;
			idAnime = LeanTween.value(base.gameObject, 0f, animePathLength, 1.5f).setOnUpdate(delegate(float val)
			{
				dollyCart.m_Position = val;
			}).setOnComplete((Action)delegate
			{
				EnemyMoveStart();
			})
				.id;
				StartCoroutine(EnemyExistWait());
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS:
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				dollyCart.m_Position = pathLength;
				routineBoss = BossExistAnime();
				StartCoroutine(routineBoss);
				break;
			}
		}
		private void EnemyMoveStart()
		{
			isExistAnime = false;
			dollyCart.m_Path = smoothPath;
			dollyCart.m_Position = 0f;
			dollyCart.m_Speed = enemyData.speed;
			isExist = true;
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.COMMON:
				animatorCommon.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.COMMON_2:
				animatorCommon2.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED:
				animatorSpeed.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED_2:
				animatorSpeed2.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG:
				animatorStrong.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG_2:
				animatorStrong2.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH:
				animatorTough.Play("Base Layer.Run_0");
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH_2:
				animatorTough2.Play("Base Layer.Run_0");
				break;
			}
		}
		public void LockOnEnemy(bool _set)
		{
			if (_set)
			{
				isLocked = true;
				pushCorrectCount = 0;
				hand.HandSeal(HandSeal_Hand.Kuji.Hold);
				if (boxCol != null)
				{
					boxCol.enabled = true;
				}
				if (psLockOn != null)
				{
					psLockOn.Play();
				}
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					if (enemyButtonDataCommonAnchor != null)
					{
						enemyButtonDataCommonAnchor.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataCommonAnchor.SetActive(value: true);
					}
					for (int num3 = 0; num3 < buttonDataCommon.Length; num3++)
					{
						if (buttonDataCommon[num3].buttonQuestion != null)
						{
							buttonDataCommon[num3].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataCommon[num3].buttonAnswer != null)
						{
							buttonDataCommon[num3].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataCommon[0].buttonQuestion != null)
					{
						buttonDataCommon[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					if (enemyButtonDataCommonAnchor2 != null)
					{
						enemyButtonDataCommonAnchor2.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataCommonAnchor2.SetActive(value: true);
					}
					for (int num2 = 0; num2 < buttonDataCommon2.Length; num2++)
					{
						if (buttonDataCommon2[num2].buttonQuestion != null)
						{
							buttonDataCommon2[num2].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataCommon2[num2].buttonAnswer != null)
						{
							buttonDataCommon2[num2].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataCommon2[0].buttonQuestion != null)
					{
						buttonDataCommon2[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					if (enemyButtonDataSpeedAnchor != null)
					{
						enemyButtonDataSpeedAnchor.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataSpeedAnchor.SetActive(value: true);
					}
					for (int m = 0; m < buttonDataSpeed.Length; m++)
					{
						if (buttonDataSpeed[m].buttonQuestion != null)
						{
							buttonDataSpeed[m].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataSpeed[m].buttonAnswer != null)
						{
							buttonDataSpeed[m].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataSpeed[0].buttonQuestion != null)
					{
						buttonDataSpeed[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					if (enemyButtonDataSpeedAnchor2 != null)
					{
						enemyButtonDataSpeedAnchor2.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataSpeedAnchor2.SetActive(value: true);
					}
					for (int n = 0; n < buttonDataSpeed2.Length; n++)
					{
						if (buttonDataSpeed2[n].buttonQuestion != null)
						{
							buttonDataSpeed2[n].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataSpeed2[n].buttonAnswer != null)
						{
							buttonDataSpeed2[n].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataSpeed2[0].buttonQuestion != null)
					{
						buttonDataSpeed2[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					if (enemyButtonDataStrongAnchor != null)
					{
						enemyButtonDataStrongAnchor.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataStrongAnchor.SetActive(value: true);
					}
					for (int j = 0; j < buttonDataStrong.Length; j++)
					{
						if (buttonDataStrong[j].buttonQuestion != null)
						{
							buttonDataStrong[j].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataStrong[j].buttonAnswer != null)
						{
							buttonDataStrong[j].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataStrong[0].buttonQuestion != null)
					{
						buttonDataStrong[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					if (enemyButtonDataStrongAnchor2 != null)
					{
						enemyButtonDataStrongAnchor2.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataStrongAnchor2.SetActive(value: true);
					}
					for (int num4 = 0; num4 < buttonDataStrong2.Length; num4++)
					{
						if (buttonDataStrong2[num4].buttonQuestion != null)
						{
							buttonDataStrong2[num4].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataStrong2[num4].buttonAnswer != null)
						{
							buttonDataStrong2[num4].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataStrong2[0].buttonQuestion != null)
					{
						buttonDataStrong2[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					if (enemyButtonDataToughAnchor != null)
					{
						enemyButtonDataToughAnchor.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataToughAnchor.SetActive(value: true);
					}
					for (int k = 0; k < buttonDataTough.Length; k++)
					{
						if (buttonDataTough[k].buttonQuestion != null)
						{
							buttonDataTough[k].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataTough[k].buttonAnswer != null)
						{
							buttonDataTough[k].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataTough[0].buttonQuestion != null)
					{
						buttonDataTough[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					if (enemyButtonDataToughAnchor2 != null)
					{
						enemyButtonDataToughAnchor2.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataToughAnchor2.SetActive(value: true);
					}
					for (int l = 0; l < buttonDataTough2.Length; l++)
					{
						if (buttonDataTough2[l].buttonQuestion != null)
						{
							buttonDataTough2[l].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataTough2[l].buttonAnswer != null)
						{
							buttonDataTough2[l].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataTough2[0].buttonQuestion != null)
					{
						buttonDataTough2[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					if (enemyButtonDataBossAnchor != null)
					{
						enemyButtonDataBossAnchor.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataBossAnchor.SetActive(value: true);
					}
					for (int num = 0; num < buttonDataBoss.Length; num++)
					{
						if (buttonDataBoss[num].buttonQuestion != null)
						{
							buttonDataBoss[num].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataBoss[num].buttonAnswer != null)
						{
							buttonDataBoss[num].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataBoss[0].buttonQuestion != null)
					{
						buttonDataBoss[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					if (enemyButtonDataBossAnchor2 != null)
					{
						enemyButtonDataBossAnchor2.GetComponent<HandSeal_UIAnime>().Init();
						enemyButtonDataBossAnchor2.SetActive(value: true);
					}
					for (int i = 0; i < buttonDataBoss2.Length; i++)
					{
						if (buttonDataBoss2[i].buttonQuestion != null)
						{
							buttonDataBoss2[i].buttonQuestion.SetActive(value: true);
						}
						if (buttonDataBoss2[i].buttonAnswer != null)
						{
							buttonDataBoss2[i].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonDataBoss2[0].buttonQuestion != null)
					{
						buttonDataBoss2[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					break;
				}
			}
			else
			{
				if (!isLocked)
				{
					return;
				}
				isLocked = false;
				if (boxCol != null && !isDeath)
				{
					boxCol.enabled = false;
				}
				if (psLockOn != null)
				{
					psLockOn.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				}
				if (enemyButtonDataCommonAnchor != null)
				{
					enemyButtonDataCommonAnchor.SetActive(value: false);
				}
				if (enemyButtonDataCommonAnchor2 != null)
				{
					enemyButtonDataCommonAnchor2.SetActive(value: false);
				}
				if (enemyButtonDataSpeedAnchor != null)
				{
					enemyButtonDataSpeedAnchor.SetActive(value: false);
				}
				if (enemyButtonDataSpeedAnchor2 != null)
				{
					enemyButtonDataSpeedAnchor2.SetActive(value: false);
				}
				if (enemyButtonDataStrongAnchor != null)
				{
					enemyButtonDataStrongAnchor.SetActive(value: false);
				}
				if (enemyButtonDataStrongAnchor2 != null)
				{
					enemyButtonDataStrongAnchor2.SetActive(value: false);
				}
				if (enemyButtonDataToughAnchor != null)
				{
					enemyButtonDataToughAnchor.SetActive(value: false);
				}
				if (enemyButtonDataToughAnchor2 != null)
				{
					enemyButtonDataToughAnchor2.SetActive(value: false);
				}
				if (enemyButtonDataBossAnchor != null)
				{
					enemyButtonDataBossAnchor.SetActive(value: false);
				}
				if (enemyButtonDataBossAnchor2 != null)
				{
					enemyButtonDataBossAnchor2.SetActive(value: false);
				}
				for (int num5 = 0; num5 < buttonDataCommon.Length; num5++)
				{
					if (buttonDataCommon[num5].buttonQuestion != null)
					{
						buttonDataCommon[num5].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num6 = 0; num6 < buttonDataCommon2.Length; num6++)
				{
					if (buttonDataCommon2[num6].buttonQuestion != null)
					{
						buttonDataCommon2[num6].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num7 = 0; num7 < buttonDataSpeed.Length; num7++)
				{
					if (buttonDataSpeed[num7].buttonQuestion != null)
					{
						buttonDataSpeed[num7].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num8 = 0; num8 < buttonDataSpeed2.Length; num8++)
				{
					if (buttonDataSpeed2[num8].buttonQuestion != null)
					{
						buttonDataSpeed2[num8].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num9 = 0; num9 < buttonDataStrong.Length; num9++)
				{
					if (buttonDataStrong[num9].buttonQuestion != null)
					{
						buttonDataStrong[num9].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num10 = 0; num10 < buttonDataStrong2.Length; num10++)
				{
					if (buttonDataStrong2[num10].buttonQuestion != null)
					{
						buttonDataStrong2[num10].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num11 = 0; num11 < buttonDataTough.Length; num11++)
				{
					if (buttonDataTough[num11].buttonQuestion != null)
					{
						buttonDataTough[num11].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num12 = 0; num12 < buttonDataTough2.Length; num12++)
				{
					if (buttonDataTough2[num12].buttonQuestion != null)
					{
						buttonDataTough2[num12].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num13 = 0; num13 < buttonDataBoss.Length; num13++)
				{
					if (buttonDataBoss[num13].buttonQuestion != null)
					{
						buttonDataBoss[num13].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
				for (int num14 = 0; num14 < buttonDataBoss2.Length; num14++)
				{
					if (buttonDataBoss2[num14].buttonQuestion != null)
					{
						buttonDataBoss2[num14].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
			}
		}
		public void pushButton(HandSeal_Hand.InputButton _input)
		{
			if (!isExist || isDeathAnime)
			{
				return;
			}
			if (!isSecretStyleAttack)
			{
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					if (_input == buttonDataCommon[pushCorrectCount].inputButton)
					{
						if (buttonDataCommon[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataCommon[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataCommon[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataCommon[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataCommon[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataCommon[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataCommon[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataCommonAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					if (_input == buttonDataCommon2[pushCorrectCount].inputButton)
					{
						if (buttonDataCommon2[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataCommon2[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataCommon2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataCommon2[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataCommon2[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataCommon2[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataCommon2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataCommonAnchor2.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					if (_input == buttonDataSpeed[pushCorrectCount].inputButton)
					{
						if (buttonDataSpeed[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataSpeed[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataSpeed[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataSpeed[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataSpeed[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Zen);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataSpeed[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataSpeed[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataSpeedAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					if (_input == buttonDataSpeed2[pushCorrectCount].inputButton)
					{
						if (buttonDataSpeed2[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataSpeed2[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataSpeed2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataSpeed2[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataSpeed2[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Zai);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Zen);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataSpeed2[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataSpeed2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataSpeedAnchor2.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					if (_input == buttonDataStrong[pushCorrectCount].inputButton)
					{
						if (buttonDataStrong[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataStrong[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataStrong[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataStrong[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataStrong[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Sya);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Hyou);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataStrong[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataStrong[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataStrongAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					if (_input == buttonDataStrong2[pushCorrectCount].inputButton)
					{
						if (buttonDataStrong2[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataStrong2[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataStrong2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataStrong2[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataStrong2[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Jin);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Sya);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Hyou);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataStrong2[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataStrong2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataStrongAnchor2.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					if (_input == buttonDataTough[pushCorrectCount].inputButton)
					{
						if (buttonDataTough[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataTough[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataTough[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataTough[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataTough[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Jin);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Zen);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Zai);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Sya);
							break;
						case 5:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataTough[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataTough[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataToughAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					if (_input == buttonDataTough2[pushCorrectCount].inputButton)
					{
						if (buttonDataTough2[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataTough2[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataTough2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataTough2[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataTough2[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Jin);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Hyou);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Rin);
							break;
						case 5:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataTough2[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataTough2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataToughAnchor2.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					if (_input == buttonDataBoss[pushCorrectCount].inputButton)
					{
						if (buttonDataBoss[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataBoss[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataBoss[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataBoss[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataBoss[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Zen);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Jin);
							break;
						case 5:
							hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
							break;
						case 6:
							hand.HandSeal(HandSeal_Hand.Kuji.Zai);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataBoss[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataBoss[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataBossAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					if (_input == buttonDataBoss2[pushCorrectCount].inputButton)
					{
						if (buttonDataBoss2[pushCorrectCount].buttonQuestion != null)
						{
							buttonDataBoss2[pushCorrectCount].buttonQuestion.SetActive(value: false);
							buttonDataBoss2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
						}
						if (buttonDataBoss2[pushCorrectCount].buttonAnswer != null)
						{
							buttonDataBoss2[pushCorrectCount].buttonAnswer.SetActive(value: true);
						}
						switch (pushCorrectCount)
						{
						case 0:
							hand.HandSeal(HandSeal_Hand.Kuji.Jin);
							break;
						case 1:
							hand.HandSeal(HandSeal_Hand.Kuji.Tou);
							break;
						case 2:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 3:
							hand.HandSeal(HandSeal_Hand.Kuji.Hyou);
							break;
						case 4:
							hand.HandSeal(HandSeal_Hand.Kuji.Zai);
							break;
						case 5:
							hand.HandSeal(HandSeal_Hand.Kuji.Kai);
							break;
						case 6:
							hand.HandSeal(HandSeal_Hand.Kuji.Zai);
							break;
						}
						pushCorrectCount++;
						if (needInputCount > pushCorrectCount)
						{
							if (buttonDataBoss2[pushCorrectCount].buttonQuestion != null)
							{
								buttonDataBoss2[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
							}
							enemyButtonDataBossAnchor2.GetComponent<HandSeal_UIAnime>().Revolver();
						}
					}
					else
					{
						hand.InputSealMiss();
					}
					break;
				}
				if (needInputCount == pushCorrectCount)
				{
					dollyCart.m_Speed = 0f;
					hand.GunTargetLookOn();
					isDeath = true;
					StartCoroutine(EnemyKillAnime());
					if (HandSeal_Define.GM.IsDuringGame() && hand.player.UserType <= HandSeal_Define.UserType.PLAYER_4)
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_sealeffectdone", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
					}
					if (enemyButtonDataCommonAnchor != null)
					{
						enemyButtonDataCommonAnchor.SetActive(value: false);
					}
					if (enemyButtonDataCommonAnchor2 != null)
					{
						enemyButtonDataCommonAnchor2.SetActive(value: false);
					}
					if (enemyButtonDataSpeedAnchor != null)
					{
						enemyButtonDataSpeedAnchor.SetActive(value: false);
					}
					if (enemyButtonDataSpeedAnchor2 != null)
					{
						enemyButtonDataSpeedAnchor2.SetActive(value: false);
					}
					if (enemyButtonDataStrongAnchor != null)
					{
						enemyButtonDataStrongAnchor.SetActive(value: false);
					}
					if (enemyButtonDataStrongAnchor2 != null)
					{
						enemyButtonDataStrongAnchor2.SetActive(value: false);
					}
					if (enemyButtonDataToughAnchor != null)
					{
						enemyButtonDataToughAnchor.SetActive(value: false);
					}
					if (enemyButtonDataToughAnchor2 != null)
					{
						enemyButtonDataToughAnchor2.SetActive(value: false);
					}
					if (enemyButtonDataBossAnchor != null)
					{
						enemyButtonDataBossAnchor.SetActive(value: false);
					}
					if (enemyButtonDataBossAnchor2 != null)
					{
						enemyButtonDataBossAnchor2.SetActive(value: false);
					}
				}
				return;
			}
			if (_input == buttonSecretStyle[pushCorrectCount].inputButton)
			{
				if (buttonSecretStyle[pushCorrectCount].buttonQuestion != null)
				{
					buttonSecretStyle[pushCorrectCount].buttonQuestion.SetActive(value: false);
					buttonSecretStyle[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
				}
				if (buttonSecretStyle[pushCorrectCount].buttonAnswer != null)
				{
					buttonSecretStyle[pushCorrectCount].buttonAnswer.SetActive(value: true);
				}
				switch (pushCorrectCount)
				{
				case 0:
					hand.HandSeal(HandSeal_Hand.Kuji.Rin);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Rin);
					break;
				case 1:
					hand.HandSeal(HandSeal_Hand.Kuji.Hyou);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Hyou);
					break;
				case 2:
					hand.HandSeal(HandSeal_Hand.Kuji.Tou);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Tou);
					break;
				case 3:
					hand.HandSeal(HandSeal_Hand.Kuji.Sya);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Sya);
					break;
				case 4:
					hand.HandSeal(HandSeal_Hand.Kuji.Kai);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Kai);
					break;
				case 5:
					hand.HandSeal(HandSeal_Hand.Kuji.Jin);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Jin);
					break;
				case 6:
					hand.HandSeal(HandSeal_Hand.Kuji.Retsu);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Retsu);
					break;
				case 7:
					hand.HandSeal(HandSeal_Hand.Kuji.Zai);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Zai);
					break;
				case 8:
					hand.HandSeal(HandSeal_Hand.Kuji.Zen);
					hand.PlaySealEffect(HandSeal_Hand.Kuji.Zen);
					break;
				}
				pushCorrectCount++;
				if (needInputCount > pushCorrectCount)
				{
					if (buttonSecretStyle[pushCorrectCount].buttonQuestion != null)
					{
						buttonSecretStyle[pushCorrectCount].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
					secretStyleButtonAnchor.GetComponent<HandSeal_UIAnime>().Revolver();
				}
			}
			else
			{
				hand.InputSealMiss();
			}
			if (needInputCount == pushCorrectCount)
			{
				hand.SecretStylePlay();
				if (HandSeal_Define.GM.IsDuringGame() && hand.player.UserType <= HandSeal_Define.UserType.PLAYER_4)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_sealeffectdone", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				}
			}
		}
		public void SecretStyleAttackReady()
		{
			isSecretStyleAttack = true;
			if (isExist)
			{
				if (isLocked)
				{
					LockOnEnemy(_set: false);
					isLocked = true;
					pushCorrectCount = 0;
					hand.HandSeal(HandSeal_Hand.Kuji.Hold);
					needInputCount = buttonSecretStyle.Length;
					if (secretStyleButtonAnchor != null)
					{
						secretStyleButtonAnchor.GetComponent<HandSeal_UIAnime>().Init();
						secretStyleButtonAnchor.SetActive(value: true);
					}
					for (int i = 0; i < buttonSecretStyle.Length; i++)
					{
						if (buttonSecretStyle[i].buttonQuestion != null)
						{
							buttonSecretStyle[i].buttonQuestion.SetActive(value: true);
						}
						if (buttonSecretStyle[i].buttonAnswer != null)
						{
							buttonSecretStyle[i].buttonAnswer.SetActive(value: false);
						}
					}
					if (buttonSecretStyle[0].buttonQuestion != null)
					{
						buttonSecretStyle[0].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStart();
					}
				}
				if (psLockOn != null)
				{
					psLockOn.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
					psLockOn.Play();
				}
				dollyCart.m_Speed = 0f;
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					animatorCommon.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					animatorCommon2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					animatorSpeed.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					animatorSpeed2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					animatorStrong.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					animatorStrong2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					animatorTough.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					animatorTough2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					animatorBoss.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					animatorBoss2.speed = 0f;
					break;
				}
			}
			else if (isBossExist)
			{
				StopCoroutine(routineBoss);
				routineBoss = null;
				isExist = true;
				if (psLockOn != null)
				{
					psLockOn.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
					psLockOn.Play();
				}
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.BOSS:
					animatorBoss.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					animatorBoss2.speed = 0f;
					break;
				}
			}
			else if (isExistAnime)
			{
				LeanTween.pause(idAnime);
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					animatorCommon.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					animatorCommon2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					animatorSpeed.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					animatorSpeed2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					animatorStrong.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					animatorStrong2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					animatorTough.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					animatorTough2.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					animatorBoss.speed = 0f;
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					animatorBoss2.speed = 0f;
					break;
				}
			}
		}
		public void SecretStyleAttack()
		{
			isDeath = true;
			StartCoroutine(SecretStyleAttackAnime());
		}
		public HandSeal_Hand.InputButton GetInputButton()
		{
			if (!isSecretStyleAttack)
			{
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					if (pushCorrectCount < buttonDataCommon.Length)
					{
						return buttonDataCommon[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					if (pushCorrectCount < buttonDataCommon2.Length)
					{
						return buttonDataCommon2[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					if (pushCorrectCount < buttonDataSpeed.Length)
					{
						return buttonDataSpeed[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					if (pushCorrectCount < buttonDataSpeed2.Length)
					{
						return buttonDataSpeed2[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					if (pushCorrectCount < buttonDataStrong.Length)
					{
						return buttonDataStrong[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					if (pushCorrectCount < buttonDataStrong2.Length)
					{
						return buttonDataStrong2[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					if (pushCorrectCount < buttonDataTough.Length)
					{
						return buttonDataTough[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					if (pushCorrectCount < buttonDataTough2.Length)
					{
						return buttonDataTough2[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					if (pushCorrectCount < buttonDataBoss.Length)
					{
						return buttonDataBoss[pushCorrectCount].inputButton;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					if (pushCorrectCount < buttonDataBoss2.Length)
					{
						return buttonDataBoss2[pushCorrectCount].inputButton;
					}
					break;
				}
			}
			else if (pushCorrectCount < buttonSecretStyle.Length)
			{
				return buttonSecretStyle[pushCorrectCount].inputButton;
			}
			return HandSeal_Hand.InputButton.A;
		}
		private IEnumerator AutoEnemyExist()
		{
			yield return new WaitForSeconds(15f);
			AwakeEnemy();
		}
		private IEnumerator EnemyAttack()
		{
			isAttackPlay = true;
			yield return new WaitForSeconds(3f);
			yield return new WaitForSeconds(1f);
			if (isExist && !isDeath && !isSecretStyleAttack && HandSeal_Define.GM.IsDuringGame())
			{
				animeNum = UnityEngine.Random.Range(0, 3);
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					switch (animeNum)
					{
					case 0:
						animatorCommon.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorCommon.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorCommon.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					switch (animeNum)
					{
					case 0:
						animatorCommon2.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorCommon2.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorCommon2.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					switch (animeNum)
					{
					case 0:
						animatorSpeed.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorSpeed.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorSpeed.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					switch (animeNum)
					{
					case 0:
						animatorSpeed2.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorSpeed2.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorSpeed2.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					switch (animeNum)
					{
					case 0:
						animatorStrong.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorStrong.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorStrong.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					switch (animeNum)
					{
					case 0:
						animatorStrong2.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorStrong2.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorStrong2.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					switch (animeNum)
					{
					case 0:
						animatorTough.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorTough.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorTough.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					switch (animeNum)
					{
					case 0:
						animatorTough2.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorTough2.SetTrigger("ToAttack_3");
						break;
					case 2:
						animatorTough2.SetTrigger("ToAttack_4");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					switch (animeNum)
					{
					case 0:
						animatorBoss.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorBoss.SetTrigger("ToAttack_5");
						break;
					case 2:
						animatorBoss.SetTrigger("ToAttack_6");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					switch (animeNum)
					{
					case 0:
						animatorBoss2.SetTrigger("ToAttack_0");
						break;
					case 1:
						animatorBoss2.SetTrigger("ToAttack_5");
						break;
					case 2:
						animatorBoss2.SetTrigger("ToAttack_6");
						break;
					}
					break;
				}
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					switch (animeNum)
					{
					case 0:
						animatorCommon.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorCommon.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorCommon.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					switch (animeNum)
					{
					case 0:
						animatorCommon2.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorCommon2.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorCommon2.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					switch (animeNum)
					{
					case 0:
						animatorSpeed.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorSpeed.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorSpeed.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					switch (animeNum)
					{
					case 0:
						animatorSpeed2.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorSpeed2.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorSpeed2.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					switch (animeNum)
					{
					case 0:
						animatorStrong.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorStrong.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorStrong.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					switch (animeNum)
					{
					case 0:
						animatorStrong2.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorStrong2.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorStrong2.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					switch (animeNum)
					{
					case 0:
						animatorTough.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorTough.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorTough.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					switch (animeNum)
					{
					case 0:
						animatorTough2.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorTough2.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorTough2.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					switch (animeNum)
					{
					case 0:
						animatorBoss.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorBoss.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorBoss.SetTrigger("ToIdle_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					switch (animeNum)
					{
					case 0:
						animatorBoss2.SetTrigger("ToIdle_0");
						break;
					case 1:
						animatorBoss2.SetTrigger("ToIdle_1");
						break;
					case 2:
						animatorBoss2.SetTrigger("ToIdle_2");
						break;
					}
					break;
				}
			}
			isAttackPlay = false;
		}
		public void EnemyAttackDamage()
		{
			if (isExist)
			{
				hand.secretGauge = Mathf.Clamp(hand.secretGauge - 0.1f, 0f, 1f);
				hand.DamageVignette();
				if (HandSeal_Define.GM.IsDuringGame() && hand.player.UserType <= HandSeal_Define.UserType.PLAYER_4)
				{
					float pitch = UnityEngine.Random.Range(0.8f, 1f);
					SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_hit", _loop: false, 0f, 1f, pitch, 0f, _overlap: true);
				}
			}
		}
		private IEnumerator EnemyKillAnime()
		{
			isDeathAnime = true;
			if (HandSeal_Define.GM.IsDuringGame())
			{
				HandSeal_Define.UserType userType = hand.player.UserType;
			}
			yield return new WaitForSeconds(0.3f);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.COMMON:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.DirtStyle);
				break;
			case HandSeal_EnemyManager.EnemyType.COMMON_2:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.DirtStyle2);
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.WindStyle);
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED_2:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.WindStyle2);
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.WaterStyle);
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG_2:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.WaterStyle2);
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.BindStyle);
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH_2:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.BindStyle2);
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.FireStyle);
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				hand.ShotNinjutsu(HandSeal_Hand.Ninjutsu.FireStyle2);
				break;
			}
			yield return new WaitForSeconds(0.5f);
			animeNum = UnityEngine.Random.Range(0, 3);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.COMMON:
				mrCommon.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorCommon.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorCommon.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorCommon.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.COMMON_2:
				mrCommon2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorCommon2.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorCommon2.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorCommon2.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED:
				mrSpeed.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorSpeed.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorSpeed.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorSpeed.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED_2:
				mrSpeed2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorSpeed2.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorSpeed2.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorSpeed2.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG:
				mrStrong.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorStrong.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorStrong.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorStrong.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG_2:
				mrStrong2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorStrong2.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorStrong2.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorStrong2.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH:
				mrTough.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorTough.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorTough.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorTough.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH_2:
				mrTough2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorTough2.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorTough2.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorTough2.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS:
				mrBoss.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorBoss.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorBoss.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorBoss.SetTrigger("ToDie_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				mrBoss2.material.SetTexture("_FaceTex", texBoyFaceSorrow);
				switch (animeNum)
				{
				case 0:
					animatorBoss2.SetTrigger("ToDie_0");
					break;
				case 1:
					animatorBoss2.SetTrigger("ToDie_1");
					break;
				case 2:
					animatorBoss2.SetTrigger("ToDie_2");
					break;
				}
				break;
			}
			HandSeal_Define.PM.SetPoint(hand.UserType, enemyData.point);
			killCount++;
			LockOnEnemy(_set: false);
			yield return new WaitForSeconds(1f);
			SetEnemy();
			isDeathAnime = false;
			hand.EnemyKillMethod();
		}
		private IEnumerator SecretStyleAttackAnime()
		{
			isDeathAnime = true;
			yield return new WaitForSeconds(0.1f);
			if (isLocked)
			{
				if (secretStyleButtonAnchor != null)
				{
					secretStyleButtonAnchor.SetActive(value: false);
				}
				for (int i = 0; i < buttonSecretStyle.Length; i++)
				{
					if (buttonSecretStyle[i].buttonQuestion != null)
					{
						buttonSecretStyle[i].buttonQuestion.GetComponent<HandSeal_ButtonBlink>().BlinkStop();
					}
				}
			}
			yield return new WaitForSeconds(0.7f);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.COMMON:
				animatorCommon.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.COMMON_2:
				animatorCommon2.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED:
				animatorSpeed.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.SPEED_2:
				animatorSpeed2.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG:
				animatorStrong.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.STRONG_2:
				animatorStrong2.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH:
				animatorTough.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.TOUGH_2:
				animatorTough2.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS:
				animatorBoss.speed = 1f;
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				animatorBoss2.speed = 1f;
				break;
			}
			LeanTween.resume(idAnime);
			if (isExist)
			{
				animeNum = UnityEngine.Random.Range(0, 3);
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					mrCommon.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorCommon.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorCommon.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorCommon.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					mrCommon2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorCommon2.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorCommon2.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorCommon2.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					mrSpeed.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorSpeed.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorSpeed.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorSpeed.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					mrSpeed2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorSpeed2.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorSpeed2.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorSpeed2.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					mrStrong.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorStrong.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorStrong.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorStrong.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					mrStrong.material.SetTexture("_FaceTex", texGirlFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorStrong2.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorStrong2.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorStrong2.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					mrTough.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorTough.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorTough.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorTough.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					mrTough2.material.SetTexture("_FaceTex", texGirlFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorTough2.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorTough2.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorTough2.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS:
					mrBoss.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorBoss.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorBoss.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorBoss.SetTrigger("ToDie_2");
						break;
					}
					break;
				case HandSeal_EnemyManager.EnemyType.BOSS_2:
					mrBoss2.material.SetTexture("_FaceTex", texBoyFaceSorrow);
					switch (animeNum)
					{
					case 0:
						animatorBoss2.SetTrigger("ToDie_0");
						break;
					case 1:
						animatorBoss2.SetTrigger("ToDie_1");
						break;
					case 2:
						animatorBoss2.SetTrigger("ToDie_2");
						break;
					}
					break;
				}
				HandSeal_Define.PM.SetPoint(hand.UserType, enemyData.point);
				killCount++;
				hand.EnemyKillMethod();
				yield return new WaitForSeconds(1f);
				SetEnemy();
				isDeathAnime = false;
				isSecretStyleAttack = false;
			}
			isDeathAnime = false;
			isSecretStyleAttack = false;
			isDeath = false;
		}
		private IEnumerator BossExistAnime()
		{
			enemyAnchor.SetActive(value: true);
			isBossExist = true;
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.BOSS:
				animatorBoss.Play("Base Layer.BossIdle");
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				animatorBoss2.Play("Base Layer.BossIdle");
				break;
			}
			yield return new WaitForSeconds(1f);
			animeNum = UnityEngine.Random.Range(0, 3);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.BOSS:
				switch (animeNum)
				{
				case 0:
					animatorBoss.SetTrigger("ToAttack_0");
					break;
				case 1:
					animatorBoss.SetTrigger("ToAttack_1");
					break;
				case 2:
					animatorBoss.SetTrigger("ToAttack_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				switch (animeNum)
				{
				case 0:
					animatorBoss2.SetTrigger("ToAttack_0");
					break;
				case 1:
					animatorBoss2.SetTrigger("ToAttack_1");
					break;
				case 2:
					animatorBoss2.SetTrigger("ToAttack_2");
					break;
				}
				break;
			}
			yield return new WaitForSeconds(1f);
			animeNum = UnityEngine.Random.Range(0, 3);
			switch (enemyData.enemyType)
			{
			case HandSeal_EnemyManager.EnemyType.BOSS:
				switch (animeNum)
				{
				case 0:
					animatorBoss.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorBoss.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorBoss.SetTrigger("ToIdle_2");
					break;
				}
				break;
			case HandSeal_EnemyManager.EnemyType.BOSS_2:
				switch (animeNum)
				{
				case 0:
					animatorBoss2.SetTrigger("ToIdle_0");
					break;
				case 1:
					animatorBoss2.SetTrigger("ToIdle_1");
					break;
				case 2:
					animatorBoss2.SetTrigger("ToIdle_2");
					break;
				}
				break;
			}
			dollyCart.m_Speed = enemyData.speed;
			isExist = true;
		}
		private IEnumerator EnemyExistWait()
		{
			yield return null;
			enemyAnchor.SetActive(value: true);
			yield return null;
			switch (enemyExistAnime)
			{
			case EnemyExistAnimeType.WALK:
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					animatorCommon.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					animatorCommon2.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					animatorSpeed.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					animatorSpeed2.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					animatorStrong.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					animatorStrong2.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					animatorTough.Play("Base Layer.Run_0");
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					animatorTough2.Play("Base Layer.Run_0");
					break;
				}
				break;
			case EnemyExistAnimeType.JUMP:
				switch (enemyData.enemyType)
				{
				case HandSeal_EnemyManager.EnemyType.COMMON:
					animatorCommon.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.COMMON_2:
					animatorCommon2.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED:
					animatorSpeed.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.SPEED_2:
					animatorSpeed2.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG:
					animatorStrong.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.STRONG_2:
					animatorStrong2.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH:
					animatorTough.Play("Base Layer.Jump");
					break;
				case HandSeal_EnemyManager.EnemyType.TOUGH_2:
					animatorTough2.Play("Base Layer.Jump");
					break;
				}
				break;
			}
		}
	}
