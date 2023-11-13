using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
public class MakingPotion_PlayerScript : MonoBehaviour
{
	public enum SpinSpeedType
	{
		Stay,
		TooSlow,
		Slow,
		Normal,
		Fast,
		TooFast,
		Max
	}
	public enum SugarColorType
	{
		White,
		Red,
		Yellow,
		Green,
		Blue,
		Max
	}
	public enum TurnType
	{
		Stay,
		LeftTurn,
		RightTurn,
		Max
	}
	private const float ROD_POS_RADIUS = 0.15f;
	private const float SPIN_ADD_INTERVAL = 1f;
	private const float SPIN_CHECK_INTERVAL = 1f;
	private const float SPIN_MOVE_SPEED = 10f;
	private const float SPIN_ADD_PER = 14.3f;
	private const float SPIN_BEST_BASE_ANGLE = 345f;
	private const float SPIN_BEST_RANGE_ANGLE = 105f;
	private const float SPIN_GOOD_RANGE_ANGLE = 180f;
	private static readonly Color[] WATER_COLORS = new Color[5]
	{
		new Color(0.6f, 0.6f, 0.6f, 1f),
		new Color(0.8f, 0.2f, 0.1f, 1f),
		new Color(0.8f, 0.8f, 0f, 1f),
		new Color(0f, 0.8f, 0f, 1f),
		new Color(0f, 0.6f, 0.8f, 1f)
	};
	[SerializeField]
	private MakingPotion_Rod rod;
	[SerializeField]
	private MakingPotion_Machine machine;
	[SerializeField]
	private Transform ghostRodTrans;
	[SerializeField]
	[Header("左回転の矢印")]
	private GameObject leftArrow;
	[SerializeField]
	[Header("右回転の矢印")]
	private GameObject rightArrow;
	[SerializeField]
	[Header("指示のテキスト")]
	private TextMeshPro speedDisplay;
	[SerializeField]
	[Header("判定のテキスト")]
	private TextMeshPro judgementDisplay;
	[SerializeField]
	private GameObject moveObj;
	private bool isPlayer;
	private int playerNo;
	private int charaNo;
	private int teamNo;
	private int createCount;
	private SpinSpeedType spinSpeedType;
	private SpinSpeedType definitionSpinSpeedType;
	private SpinSpeedType nowSpinSpeedType;
	private SugarColorType sugarColorType;
	private TurnType turnType;
	private TurnType nowTurnType;
	private MakingPotion_TargetManager.TargetData targetData;
	private bool isSpinning;
	private float spinningAddTime;
	private float spinningCheckTime;
	private float spinningNowAngle;
	private float spinningAddAngle;
	private float spinningMinAngle;
	private float spinningMaxAngle;
	private int spinningFastCnt;
	private int spinningSlowCnt;
	private float ghostAngle;
	private int spinSpeedMissCount;
	private float spinDirMatchTime;
	private int sugarMatchCount;
	private bool[] isSugarSuccessArray = new bool[5];
	private float sugarDelayTime;
	private float[] sugarDelayLerpArray = new float[5];
	private bool isSugarWait;
	private SugarColorType targetSugarType;
	private float targetSugarTime;
	private SugarColorType[] targetSugarTypeArray;
	private bool[] isTargetSugarTypeSuccessArray;
	private float targetSugarFailedTime;
	private bool isTargetTurnRight;
	private bool isNowCreate;
	private MaterialPropertyBlock pbA;
	private MaterialPropertyBlock pbB;
	[SerializeField]
	private MeshRenderer waterRendererA;
	[SerializeField]
	private MeshRenderer waterRendererB;
	private float nowRotAngle;
	private bool isNowChangeWaterColor;
	[SerializeField]
	private MeshRenderer[] redRenderers;
	[SerializeField]
	private MeshRenderer[] yellowRenderers;
	[SerializeField]
	private MeshRenderer[] greenRenderers;
	[SerializeField]
	private MeshRenderer[] blueRenderers;
	[SerializeField]
	private MeshRenderer[] redFadeRenderers;
	[SerializeField]
	private MeshRenderer[] yellowFadeRenderers;
	[SerializeField]
	private MeshRenderer[] greenFadeRenderers;
	[SerializeField]
	private MeshRenderer[] blueFadeRenderers;
	private MaterialPropertyBlock colorPb;
	private bool isUsedRed;
	private bool isUsedYellow;
	private bool isUsedGreen;
	private bool isUsedBlue;
	private bool isUsedRedTwo;
	private bool isUsedYellowTwo;
	private bool isUsedGreenTwo;
	private bool isUsedBlueTwo;
	private const float AI_ANGLE_HALF_RANGE_WEAK = 330f;
	private const float AI_ANGLE_HALF_RANGE_NORMAL = 300f;
	private const float AI_ANGLE_HALF_RANGE_STRONG = 270f;
	private const float AI_COLOR_WAIT_TIME_WEAK = 1f;
	private const float AI_COLOR_WAIT_TIME_NORMAL = 0.75f;
	private const float AI_COLOR_WAIT_TIME_STRONG = 0.5f;
	private const int AI_COLOR_MISS_PER_WEAK = 90;
	private const int AI_COLOR_MISS_PER_NORMAL = 70;
	private const int AI_COLOR_MISS_PER_STRONG = 50;
	private const float AI_DIR_CHANGE_DELAY_MIN_WEAK = 0.5f;
	private const float AI_DIR_CHANGE_DELAY_MAX_WEAK = 1.5f;
	private const float AI_DIR_CHANGE_DELAY_MIN_NORMAL = 0.3f;
	private const float AI_DIR_CHANGE_DELAY_MAX_NORMAL = 1f;
	private const float AI_DIR_CHANGE_DELAY_MIN_STRONG = 0.1f;
	private const float AI_DIR_CHANGE_DELAY_MAX_STRONG = 0.5f;
	private const float AI_COLOR_DELAY_LERP_MIN_WEAK = 0.4f;
	private const float AI_COLOR_DELAY_LERP_MAX_WEAK = 0.8f;
	private const float AI_COLOR_DELAY_LERP_MIN_NORMAL = 0.2f;
	private const float AI_COLOR_DELAY_LERP_MAX_NORMAL = 0.8f;
	private const float AI_COLOR_DELAY_LERP_MIN_STRONG = 0.1f;
	private const float AI_COLOR_DELAY_LERP_MAX_STRONG = 0.6f;
	private float aiAngle;
	private UnityEngine.Vector2 aiPerlinVec;
	private float aiAngleHalfRange;
	private bool isAiColorWait;
	private float aiColorWaitTime;
	private float aiColorTimer;
	private int aiColorMissPer;
	private int aiStrength;
	private int aiColorSameNum;
	private float aiDirChangeDelayMin;
	private float aiDirChangeDelayMax;
	private float aiColorDelayLerpMin;
	private float aiColorDelayLerpMax;
	private float aiDirChangeDelay;
	private float aiColorDelayLerp;
	private float aiRotDir;
	public bool IsPlayer => isPlayer;
	public int PlayerNo => playerNo;
	public int CharaNo => charaNo;
	public int TeamNo => teamNo;
	public int CreateCount => createCount;
	public SugarColorType NowSugarColorType => sugarColorType;
	public void Init(int _charaNo)
	{
		charaNo = _charaNo;
		playerNo = SingletonCustom<MakingPotion_GameManager>.Instance.GetCharaPlayerNo(charaNo);
		teamNo = SingletonCustom<MakingPotion_GameManager>.Instance.GetCharaTeamNo(charaNo);
		isPlayer = (playerNo < 4);
		rod.Init(_charaNo);
		machine.Init(_charaNo);
		DataInit();
	}
	public void SecondGroupInit()
	{
		playerNo = SingletonCustom<MakingPotion_GameManager>.Instance.GetCharaPlayerNo(charaNo);
		teamNo = SingletonCustom<MakingPotion_GameManager>.Instance.GetCharaTeamNo(charaNo);
		isPlayer = (playerNo < 4);
		createCount = 0;
		rod.SecondGroupInit();
		DataInit();
	}
	private void DataInit()
	{
		isSpinning = false;
		spinningAddTime = 0f;
		spinningAddAngle = 0f;
		spinningCheckTime = 0f;
		spinningFastCnt = 0;
		spinningSlowCnt = 0;
		ghostRodTrans.gameObject.SetActive(value: true);
		ghostAngle = -90f;
		ghostRodTrans.SetLocalPosition(0.15f, ghostRodTrans.localPosition.y, 0f);
		rod.SetPos(new UnityEngine.Vector3(0.15f, 0f, 0f));
		isNowCreate = false;
		if (pbA == null)
		{
			pbA = new MaterialPropertyBlock();
		}
		if (pbB == null)
		{
			pbB = new MaterialPropertyBlock();
		}
		if (colorPb == null)
		{
			colorPb = new MaterialPropertyBlock();
		}
		ChangeWaterColor(SugarColorType.White, _immediate: true);
		AiInit();
	}
	public void UpdateMethod()
	{
		if (isTargetTurnRight)
		{
			ghostAngle -= 360f * Time.deltaTime * SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetNowRotSpeed;
		}
		else
		{
			ghostAngle += 360f * Time.deltaTime * SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetNowRotSpeed;
		}
		UnityEngine.Vector2 vector = Rotate(UnityEngine.Vector2.up, ghostAngle);
		ghostRodTrans.SetLocalPosition(vector.x * 0.15f, ghostRodTrans.localPosition.y, vector.y * 0.15f);
		if (isNowCreate)
		{
			if (isTargetTurnRight && nowRotAngle < -1f)
			{
				spinDirMatchTime += Time.deltaTime;
			}
			else if (!isTargetTurnRight && nowRotAngle > 1f)
			{
				spinDirMatchTime += Time.deltaTime;
			}
			if (isSugarWait && isPlayer)
			{
				SugarButtonOperation();
			}
			waterRendererA.transform.AddLocalEulerAnglesY(nowRotAngle * -4f * Time.deltaTime);
			waterRendererB.transform.AddLocalEulerAnglesY(nowRotAngle * -4f * Time.deltaTime);
		}
	}
	private void TurnDisplay()
	{
		leftArrow.SetActive(!isTargetTurnRight);
		rightArrow.SetActive(isTargetTurnRight);
	}
	private void TurnSpeedChange()
	{
	}
	private void TurnChange()
	{
	}
	private void SugarButtonOperation()
	{
		bool flag = false;
		SugarColorType colorType = SugarColorType.White;
		if (MakingPotion_Controller.GetRedButtonDown(playerNo))
		{
			flag = true;
			colorType = SugarColorType.Red;
		}
		else if (MakingPotion_Controller.GetYellowButtonDown(playerNo))
		{
			flag = true;
			colorType = SugarColorType.Yellow;
		}
		else if (MakingPotion_Controller.GetGreenButtonDown(playerNo))
		{
			flag = true;
			colorType = SugarColorType.Green;
		}
		else if (MakingPotion_Controller.GetBlueButtonDown(playerNo))
		{
			flag = true;
			colorType = SugarColorType.Blue;
		}
		if (flag && Time.time - targetSugarFailedTime > 0.5f)
		{
			ChangeWaterColor(colorType, _immediate: false);
			SetSugarColorType(colorType);
			SugarMatchCheck(colorType);
		}
	}
	private void SugarMatchCheck(SugarColorType _colorType)
	{
		for (int i = 0; i < targetSugarTypeArray.Length; i++)
		{
			if (isTargetSugarTypeSuccessArray[i] || targetSugarTypeArray[i] != _colorType)
			{
				continue;
			}
			isTargetSugarTypeSuccessArray[i] = true;
			List<SugarColorType> list = new List<SugarColorType>();
			for (int j = 0; j < isTargetSugarTypeSuccessArray.Length; j++)
			{
				if (!isTargetSugarTypeSuccessArray[j])
				{
					list.Add(targetSugarTypeArray[j]);
				}
			}
			if (list.Count == 2)
			{
				SingletonCustom<MakingPotion_UiManager>.Instance.ShowSugar(charaNo, list[0], list[1]);
				if (isPlayer)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
				}
			}
			else if (list.Count == 1)
			{
				SingletonCustom<MakingPotion_UiManager>.Instance.ShowSugar(charaNo, list[0]);
				if (isPlayer)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
				}
			}
			else
			{
				SugarSuccess();
			}
			return;
		}
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
		}
		targetSugarFailedTime = Time.time;
	}
	private void SugarSuccess()
	{
		isSugarWait = false;
		sugarMatchCount++;
		isSugarSuccessArray[createCount] = true;
		sugarDelayLerpArray[createCount] = (SingletonCustom<MakingPotion_PlayerManager>.Instance.CreateTimer - targetSugarTime) / 3f;
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.HideSugar(charaNo);
	}
	private void SugarFailed()
	{
		isSugarWait = false;
		isSugarSuccessArray[createCount] = false;
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.HideSugar(charaNo);
	}
	public void SugarOperationEnd()
	{
		if (isSugarWait)
		{
			isSugarWait = false;
			isSugarSuccessArray[createCount] = false;
			SingletonCustom<MakingPotion_UiManager>.Instance.HideSugar(charaNo);
		}
	}
	private void ChangeWaterColor(SugarColorType _colorType, bool _immediate)
	{
		if (_immediate)
		{
			LeanTween.cancel(waterRendererA.gameObject);
			pbA.SetColor("_BaseColor", WATER_COLORS[(int)_colorType]);
			waterRendererA.SetPropertyBlock(pbA);
			waterRendererB.gameObject.SetActive(value: false);
			machine.SetFastEffectColor(WATER_COLORS[(int)_colorType]);
			isNowChangeWaterColor = false;
		}
		else if (!isNowChangeWaterColor)
		{
			isNowChangeWaterColor = true;
			waterRendererB.gameObject.SetActive(value: true);
			Color bColor = WATER_COLORS[(int)_colorType];
			bColor.a = 0f;
			pbB.SetColor("_BaseColor", bColor);
			waterRendererB.SetPropertyBlock(pbB);
			LeanTween.value(waterRendererA.gameObject, 0f, 1f, 0.25f).setDelay(0.25f).setOnUpdate(delegate(float _value)
			{
				bColor.a = _value;
				pbB.SetColor("_BaseColor", bColor);
				waterRendererB.SetPropertyBlock(pbB);
			})
				.setOnComplete((Action)delegate
				{
					LeanTween.value(waterRendererA.gameObject, 1f, 0f, 0.25f).setDelay(0.25f).setOnUpdate(delegate(float _value)
					{
						bColor.a = _value;
						pbB.SetColor("_BaseColor", bColor);
						waterRendererB.SetPropertyBlock(pbB);
					})
						.setOnComplete((Action)delegate
						{
							waterRendererB.gameObject.SetActive(value: false);
							isNowChangeWaterColor = false;
						});
				});
			Color nowColor = pbA.GetColor("_BaseColor");
			Color changeColor = WATER_COLORS[(int)_colorType];
			LeanTween.delayedCall(waterRendererA.gameObject, 0.5f, (Action)delegate
			{
				LeanTween.value(waterRendererA.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
				{
					pbA.SetColor("_BaseColor", Color.Lerp(nowColor, changeColor, _value));
					waterRendererA.SetPropertyBlock(pbA);
					machine.SetFastEffectColor(pbA.GetColor("_BaseColor"));
				});
			});
		}
		else
		{
			LeanTween.cancel(waterRendererA.gameObject);
			waterRendererB.gameObject.SetActive(value: true);
			Color nowBColor = pbB.GetColor("_BaseColor");
			Color bColor2 = WATER_COLORS[(int)_colorType];
			LeanTween.value(waterRendererA.gameObject, 0f, 1f, 0.25f).setOnUpdate(delegate(float _value)
			{
				pbB.SetColor("_BaseColor", Color.Lerp(nowBColor, bColor2, _value));
				waterRendererB.SetPropertyBlock(pbB);
			}).setOnComplete((Action)delegate
			{
				LeanTween.value(waterRendererA.gameObject, 1f, 0f, 0.25f).setDelay(0.25f).setOnUpdate(delegate(float _value)
				{
					bColor2.a = _value;
					pbB.SetColor("_BaseColor", bColor2);
					waterRendererB.SetPropertyBlock(pbB);
				})
					.setOnComplete((Action)delegate
					{
						waterRendererB.gameObject.SetActive(value: false);
						isNowChangeWaterColor = false;
					});
			});
			Color nowColor2 = pbA.GetColor("_BaseColor");
			Color changeColor2 = WATER_COLORS[(int)_colorType];
			LeanTween.delayedCall(waterRendererA.gameObject, 0.25f, (Action)delegate
			{
				LeanTween.value(waterRendererA.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
				{
					pbA.SetColor("_BaseColor", Color.Lerp(nowColor2, changeColor2, _value));
					waterRendererA.SetPropertyBlock(pbA);
					machine.SetFastEffectColor(pbA.GetColor("_BaseColor"));
				});
			});
		}
	}
	public void CreateStart()
	{
		isNowCreate = true;
		nowRotAngle = 0f;
	}
	public void CreateEnd()
	{
		isNowCreate = false;
		createCount++;
		int score = CalcScore();
		spinSpeedMissCount = 0;
		spinDirMatchTime = 0f;
		sugarMatchCount = 0;
		LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
		{
			SingletonCustom<MakingPotion_GameManager>.Instance.AddScore(charaNo, score);
			SingletonCustom<MakingPotion_UiManager>.Instance.ScoreUpdate(charaNo);
			UnityEngine.Vector3 position = machine.transform.position;
			position.y += 0.2f;
			SingletonCustom<MakingPotion_UiManager>.Instance.ViewGetPoint(charaNo, score, position);
			if (SingletonCustom<MakingPotion_PlayerManager>.Instance.GetIsPlayer(charaNo))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
			}
			if (createCount < 5)
			{
				LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
				{
					LeanTween.moveLocalZ(moveObj, -2f, 0.4f).setOnComplete((Action)delegate
					{
						ChangeWaterColor(SugarColorType.White, _immediate: true);
						machine.SpeedEffectSetting(SpinSpeedType.Stay);
						LeanTween.moveLocalZ(moveObj, 0f, 0.4f);
						SingletonCustom<MakingPotion_UiManager>.Instance.SetPotionSprite(charaNo, createCount - 1, sugarColorType);
						SingletonCustom<MakingPotion_UiManager>.Instance.SetPotionPoint(charaNo, createCount - 1, score);
						SingletonCustom<MakingPotion_UiManager>.Instance.SetPotionOrder(charaNo, createCount);
					});
				});
			}
		});
		spinningCheckTime = 0f;
		spinningMinAngle = (spinningMaxAngle = spinningNowAngle);
		spinningFastCnt = 0;
		spinningSlowCnt = 0;
		nowTurnType = TurnType.Stay;
	}
	public void GameStart()
	{
		machine.MachineEffectPlay();
		machine.SetMachineEffectColor(SingletonCustom<MakingPotion_PlayerManager>.Instance.GetThreadColor(SugarColorType.White));
	}
	public void GameEnd()
	{
		machine.MachineEffectStop();
	}
	public UnityEngine.Vector3 GetMachineCenterPos()
	{
		return machine.GetCenterPos();
	}
	public MakingPotion_Machine GetMachine()
	{
		return machine;
	}
	public void SetSugarColorType(SugarColorType _colorType)
	{
		sugarColorType = _colorType;
		machine.DropSugarEffectPlay(_colorType);
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_makingpotion_water_in", _loop: false, 0f, 1f, 1f, 0.3f);
		}
		switch (_colorType)
		{
		case SugarColorType.Red:
			if (!isUsedRed)
			{
				isUsedRed = true;
				redRenderers[0].gameObject.SetActive(value: false);
				LeanTween.delayedCall(redRenderers[0].transform.parent.gameObject, 1f, (Action)delegate
				{
					redFadeRenderers[0].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 0, 0f);
					LeanTween.value(redRenderers[0].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 0, _value);
					}).setOnComplete((Action)delegate
					{
						redRenderers[0].gameObject.SetActive(value: true);
						redFadeRenderers[0].gameObject.SetActive(value: false);
						isUsedRed = false;
					});
				});
			}
			else if (!isUsedRedTwo)
			{
				isUsedRedTwo = true;
				redRenderers[1].gameObject.SetActive(value: false);
				LeanTween.delayedCall(redRenderers[1].transform.parent.gameObject, 1f, (Action)delegate
				{
					redFadeRenderers[1].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 1, 0f);
					LeanTween.value(redRenderers[1].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 1, _value);
					}).setOnComplete((Action)delegate
					{
						redRenderers[1].gameObject.SetActive(value: true);
						redFadeRenderers[1].gameObject.SetActive(value: false);
						isUsedRedTwo = false;
					});
				});
			}
			break;
		case SugarColorType.Yellow:
			if (!isUsedYellow)
			{
				isUsedYellow = true;
				yellowRenderers[0].gameObject.SetActive(value: false);
				LeanTween.delayedCall(yellowRenderers[0].transform.parent.gameObject, 1f, (Action)delegate
				{
					yellowFadeRenderers[0].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 0, 0f);
					LeanTween.value(yellowRenderers[0].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 0, _value);
					}).setOnComplete((Action)delegate
					{
						yellowRenderers[0].gameObject.SetActive(value: true);
						yellowFadeRenderers[0].gameObject.SetActive(value: false);
						isUsedYellow = false;
					});
				});
			}
			else if (!isUsedYellowTwo)
			{
				isUsedYellowTwo = true;
				yellowRenderers[1].gameObject.SetActive(value: false);
				LeanTween.delayedCall(yellowRenderers[1].transform.parent.gameObject, 1f, (Action)delegate
				{
					yellowFadeRenderers[1].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 1, 0f);
					LeanTween.value(yellowRenderers[1].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 1, _value);
					}).setOnComplete((Action)delegate
					{
						yellowRenderers[1].gameObject.SetActive(value: true);
						yellowFadeRenderers[1].gameObject.SetActive(value: false);
						isUsedYellowTwo = false;
					});
				});
			}
			break;
		case SugarColorType.Green:
			if (!isUsedGreen)
			{
				isUsedGreen = true;
				greenRenderers[0].gameObject.SetActive(value: false);
				LeanTween.delayedCall(greenRenderers[0].transform.parent.gameObject, 1f, (Action)delegate
				{
					greenFadeRenderers[0].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 0, 0f);
					LeanTween.value(greenRenderers[0].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 0, _value);
					}).setOnComplete((Action)delegate
					{
						greenRenderers[0].gameObject.SetActive(value: true);
						greenFadeRenderers[0].gameObject.SetActive(value: false);
						isUsedGreen = false;
					});
				});
			}
			else if (!isUsedGreenTwo)
			{
				isUsedGreenTwo = true;
				greenRenderers[1].gameObject.SetActive(value: false);
				LeanTween.delayedCall(greenRenderers[1].transform.parent.gameObject, 1f, (Action)delegate
				{
					greenFadeRenderers[1].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 1, 0f);
					LeanTween.value(greenRenderers[1].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 1, _value);
					}).setOnComplete((Action)delegate
					{
						greenRenderers[1].gameObject.SetActive(value: true);
						greenFadeRenderers[1].gameObject.SetActive(value: false);
						isUsedGreenTwo = false;
					});
				});
			}
			break;
		case SugarColorType.Blue:
			if (!isUsedBlue)
			{
				isUsedBlue = true;
				blueRenderers[0].gameObject.SetActive(value: false);
				LeanTween.delayedCall(blueRenderers[0].transform.parent.gameObject, 1f, (Action)delegate
				{
					blueFadeRenderers[0].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 0, 0f);
					LeanTween.value(blueRenderers[0].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 0, _value);
					}).setOnComplete((Action)delegate
					{
						blueRenderers[0].gameObject.SetActive(value: true);
						blueFadeRenderers[0].gameObject.SetActive(value: false);
						isUsedBlue = false;
					});
				});
			}
			else if (!isUsedBlueTwo)
			{
				isUsedBlueTwo = true;
				blueRenderers[1].gameObject.SetActive(value: false);
				LeanTween.delayedCall(blueRenderers[1].transform.parent.gameObject, 1f, (Action)delegate
				{
					blueFadeRenderers[1].gameObject.SetActive(value: true);
					SetColorItemFade(_colorType, 1, 0f);
					LeanTween.value(blueRenderers[1].transform.parent.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						SetColorItemFade(_colorType, 1, _value);
					}).setOnComplete((Action)delegate
					{
						blueRenderers[1].gameObject.SetActive(value: true);
						blueFadeRenderers[1].gameObject.SetActive(value: false);
						isUsedBlueTwo = false;
					});
				});
			}
			break;
		}
	}
	private void SetColorItemFade(SugarColorType _colorType, int _idx, float _alpha)
	{
		Color white = Color.white;
		white.a = _alpha;
		colorPb.SetColor("_BaseColor", white);
		switch (_colorType)
		{
		case SugarColorType.Red:
			redFadeRenderers[_idx].SetPropertyBlock(colorPb);
			break;
		case SugarColorType.Yellow:
			yellowFadeRenderers[_idx].SetPropertyBlock(colorPb);
			break;
		case SugarColorType.Green:
			greenFadeRenderers[_idx].SetPropertyBlock(colorPb);
			break;
		case SugarColorType.Blue:
			blueFadeRenderers[_idx].SetPropertyBlock(colorPb);
			break;
		}
	}
	public void SetTargetData(MakingPotion_TargetManager.TargetData _targetData)
	{
		targetData = _targetData;
	}
	public void SetTargetSugarTimeData(MakingPotion_TargetManager.SugarTimeData _data)
	{
		isSugarWait = true;
		targetSugarType = _data.sugarType;
		targetSugarTime = _data.time;
		isAiColorWait = true;
		aiColorSameNum = _data.sameNum;
		if (_data.sameNum == 1)
		{
			targetSugarTypeArray = new SugarColorType[1]
			{
				_data.sugarType
			};
			isTargetSugarTypeSuccessArray = new bool[1];
			SingletonCustom<MakingPotion_UiManager>.Instance.ShowSugar(_data.sugarType);
		}
		else if (_data.sameNum == 2)
		{
			targetSugarTypeArray = new SugarColorType[2]
			{
				_data.sugarType,
				_data.sugarType_Two
			};
			isTargetSugarTypeSuccessArray = new bool[2];
			SingletonCustom<MakingPotion_UiManager>.Instance.ShowSugar(_data.sugarType, _data.sugarType_Two);
		}
		else if (_data.sameNum == 3)
		{
			targetSugarTypeArray = new SugarColorType[3]
			{
				_data.sugarType,
				_data.sugarType_Two,
				_data.sugarType_Three
			};
			isTargetSugarTypeSuccessArray = new bool[3];
			SingletonCustom<MakingPotion_UiManager>.Instance.ShowSugar(_data.sugarType, _data.sugarType_Two, _data.sugarType_Three);
		}
	}
	public void SetTargetTurnDir(bool _isRight)
	{
		isTargetTurnRight = _isRight;
		if (!isPlayer)
		{
			float from = aiRotDir;
			float to = _isRight ? 1 : (-1);
			LeanTween.value(base.gameObject, from, to, 1f).setDelay(aiDirChangeDelay).setOnUpdate(delegate(float _value)
			{
				aiRotDir = _value;
			})
				.setOnComplete((Action)delegate
				{
					RandomResetAiDirChangeDelay();
				});
		}
	}
	public int CalcScore()
	{
		int num = 400;
		int num2 = 300;
		int num3 = 300;
		num -= Mathf.Max(0, spinSpeedMissCount - 2) / 3 * 50;
		if (num < 0)
		{
			num = 0;
		}
		float num4 = spinDirMatchTime / 15f;
		num2 = ((num4 > 0.8f) ? 300 : ((num4 > 0.75f) ? 250 : ((num4 > 0.7f) ? 200 : ((num4 > 0.65f) ? 150 : ((num4 > 0.6f) ? 100 : ((num4 > 0.5f) ? 50 : 0))))));
		int num5 = SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetData.sugarTimeDataArray.Length;
		int[] sugarTimePoints = GetSugarTimePoints(num5);
		num3 = 0;
		for (int i = 0; i < num5; i++)
		{
			if (isSugarSuccessArray[i])
			{
				int num6 = 0;
				num6 = ((!(sugarDelayLerpArray[i] < 0.6f)) ? ((sugarDelayLerpArray[i] < 0.7f) ? 1 : ((sugarDelayLerpArray[i] < 0.8f) ? 2 : ((!(sugarDelayLerpArray[i] < 0.9f)) ? 4 : 3))) : 0);
				num3 += sugarTimePoints[num6];
			}
		}
		if (num3 % 10 != 0)
		{
			num3 = (num3 / 10 + 1) * 10;
		}
		return num + num2 + num3;
	}
	public int CalcScore(SpinSpeedType[] _speedTypes, SugarColorType[] _colorTypes)
	{
		int num = 250;
		int num2 = 250;
		int num3 = 250;
		for (int i = 0; i < _speedTypes.Length; i++)
		{
			if (_speedTypes[i] == SpinSpeedType.Fast || _speedTypes[i] == SpinSpeedType.Slow)
			{
				num2 -= 20;
			}
			else if (_speedTypes[i] == SpinSpeedType.TooFast || _speedTypes[i] == SpinSpeedType.TooSlow)
			{
				num2 -= 50;
			}
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		CalcMatchCountTypeNum(_colorTypes);
		int num4 = 0;
		num3 -= Mathf.Min(100, (7 - num4) * 20);
		turnType = TurnType.Stay;
		definitionSpinSpeedType = SpinSpeedType.Stay;
		return Mathf.Max(50, num2 + num3 + num);
	}
	public int CalcMatchCountTypeNum(SugarColorType[] _rodColorTypes)
	{
		return 0;
	}
	public int[] GetSugarTimePoints(int _sugarNum)
	{
		switch (_sugarNum)
		{
		case 3:
			return new int[5]
			{
				100,
				80,
				60,
				40,
				20
			};
		case 4:
			return new int[5]
			{
				75,
				60,
				45,
				30,
				20
			};
		case 5:
			return new int[5]
			{
				60,
				50,
				40,
				30,
				20
			};
		default:
			return null;
		}
	}
	public void RodControl(UnityEngine.Vector2 _stickDir)
	{
		if (_stickDir.sqrMagnitude > 0.01f)
		{
			float num = Mathf.Atan2(_stickDir.y, _stickDir.x) * 57.29578f;
			if (!isSpinning)
			{
				isSpinning = true;
				nowTurnType = TurnType.Stay;
				nowSpinSpeedType = SpinSpeedType.Stay;
				spinningMinAngle = (spinningMaxAngle = (spinningNowAngle = num));
				rod.InitTrans();
				rod.SetPos(new UnityEngine.Vector3(_stickDir.x * 0.15f, 0f, _stickDir.y * 0.15f));
				rod.SetCreateState(MakingPotion_Rod.CreateState.Create);
			}
			else
			{
				while (num - spinningNowAngle > 180f)
				{
					num -= 360f;
				}
				for (; num - spinningNowAngle < -180f; num += 360f)
				{
				}
				if (num < 0f)
				{
					nowTurnType = TurnType.RightTurn;
				}
				else
				{
					nowTurnType = TurnType.LeftTurn;
				}
				nowRotAngle = Mathf.Lerp(nowRotAngle, num - spinningNowAngle, Time.deltaTime);
				num = Mathf.Lerp(spinningNowAngle, num, 10f * Time.deltaTime);
				if (num > spinningMaxAngle)
				{
					spinningAddAngle += num - spinningMaxAngle;
					spinningMaxAngle = num;
				}
				else if (num < spinningMinAngle)
				{
					spinningAddAngle += spinningMinAngle - num;
					spinningMinAngle = num;
				}
				spinningAddTime += Time.deltaTime;
				spinningNowAngle = num;
				UnityEngine.Vector2 vector = Rotate(UnityEngine.Vector2.up, num - 90f);
				rod.SetPos(new UnityEngine.Vector3(vector.x * 0.15f, 0f, vector.y * 0.15f));
				spinningCheckTime += Time.deltaTime;
			}
			if (isPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak);
			}
		}
		else
		{
			if (isSpinning)
			{
				isSpinning = false;
			}
			nowRotAngle -= nowRotAngle * Time.deltaTime;
		}
	}
	public void SpinCheck()
	{
		float num = 345f * SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetAverageRotSpeed;
		if (num - 105f < spinningAddAngle && num + 105f > spinningAddAngle)
		{
			spinSpeedType = SpinSpeedType.Normal;
		}
		else if (num < spinningAddAngle)
		{
			if (num + 180f > spinningAddAngle)
			{
				spinSpeedType = SpinSpeedType.Fast;
			}
			else
			{
				spinSpeedType = SpinSpeedType.TooFast;
			}
			spinningFastCnt++;
		}
		else
		{
			if (num - 180f < spinningAddAngle)
			{
				spinSpeedType = SpinSpeedType.Slow;
			}
			else
			{
				spinSpeedType = SpinSpeedType.TooSlow;
			}
			spinningSlowCnt++;
		}
		spinningAddTime = 0f;
		spinningAddAngle = 0f;
		spinningMinAngle = (spinningMaxAngle = spinningNowAngle);
		switch (spinSpeedType)
		{
		case SpinSpeedType.TooSlow:
		case SpinSpeedType.TooFast:
			spinSpeedMissCount += 2;
			break;
		case SpinSpeedType.Slow:
		case SpinSpeedType.Fast:
			spinSpeedMissCount++;
			break;
		}
		machine.SpeedEffectSetting(spinSpeedType);
	}
	private void AiInit()
	{
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		aiPerlinVec = new UnityEngine.Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
		switch (aiStrength)
		{
		case 0:
			aiAngleHalfRange = 330f;
			aiColorWaitTime = 1f;
			aiColorMissPer = 90;
			aiDirChangeDelayMin = 0.5f;
			aiDirChangeDelayMax = 1.5f;
			aiColorDelayLerpMin = 0.4f;
			aiColorDelayLerpMax = 0.8f;
			break;
		case 1:
			aiAngleHalfRange = 300f;
			aiColorWaitTime = 0.75f;
			aiColorMissPer = 70;
			aiDirChangeDelayMin = 0.3f;
			aiDirChangeDelayMax = 1f;
			aiColorDelayLerpMin = 0.2f;
			aiColorDelayLerpMax = 0.8f;
			break;
		case 2:
			aiAngleHalfRange = 270f;
			aiColorWaitTime = 0.5f;
			aiColorMissPer = 50;
			aiDirChangeDelayMin = 0.1f;
			aiDirChangeDelayMax = 0.5f;
			aiColorDelayLerpMin = 0.1f;
			aiColorDelayLerpMax = 0.6f;
			break;
		}
		RandomResetAiDirChangeDelay();
		RandomResetAiColorDelayLerp();
	}
	public void AiControl()
	{
		if (rod.CheckCreateState(MakingPotion_Rod.CreateState.Interval))
		{
			return;
		}
		if (isAiColorWait)
		{
			aiColorTimer += Time.deltaTime;
			if (aiColorTimer > 3f * aiColorDelayLerp)
			{
				aiColorSameNum--;
				if (aiColorSameNum == 0)
				{
					aiColorTimer = 0f;
					RandomResetAiColorDelayLerp();
					isAiColorWait = false;
				}
				else
				{
					aiColorTimer -= 0.25f;
				}
				for (int i = 0; i < targetSugarTypeArray.Length; i++)
				{
					if (!isTargetSugarTypeSuccessArray[i])
					{
						AiChangeSugarColor(targetSugarTypeArray[i]);
						break;
					}
				}
			}
		}
		aiPerlinVec.x += Time.deltaTime;
		aiAngle -= (345f + Mathf.Lerp(0f - aiAngleHalfRange, aiAngleHalfRange, Mathf.PerlinNoise(aiPerlinVec.x, aiPerlinVec.y))) * Time.deltaTime * aiRotDir * SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetNowRotSpeed;
		RodControl(Rotate(UnityEngine.Vector2.up, aiAngle));
	}
	private void RandomResetAiDirChangeDelay()
	{
		aiDirChangeDelay = UnityEngine.Random.Range(aiDirChangeDelayMin, aiDirChangeDelayMax);
	}
	private void RandomResetAiColorDelayLerp()
	{
		aiColorDelayLerp = UnityEngine.Random.Range(aiColorDelayLerpMin, aiColorDelayLerpMax);
	}
	private void AiChangeSugarColor(SugarColorType _colorType)
	{
		if (isSugarWait)
		{
			if (UnityEngine.Random.Range(0, 100) < aiColorMissPer)
			{
				_colorType = (SugarColorType)UnityEngine.Random.Range(1, 5);
			}
			ChangeWaterColor(_colorType, _immediate: false);
			SetSugarColorType(_colorType);
			SugarMatchCheck(_colorType);
		}
	}
	public static UnityEngine.Vector2 Rotate(UnityEngine.Vector2 from, float angle)
	{
		Complex left = new Complex(from.x, from.y);
		float num = Mathf.Cos((float)Math.PI / 180f * angle);
		float num2 = Mathf.Sin((float)Math.PI / 180f * angle);
		Complex right = new Complex(num, num2);
		Complex complex = left * right;
		return new UnityEngine.Vector2((float)complex.Real, (float)complex.Imaginary);
	}
}
