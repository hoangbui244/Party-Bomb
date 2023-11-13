using GamepadInput;
using System;
using TMPro;
using UnityEngine;
public class GS_Collection : MonoBehaviourExtension
{
	public enum BoardType
	{
		BRONZE,
		SILVER,
		GOLD,
		COMPLETE,
		DLC_1
	}
	[SerializeField]
	[Header("ボ\u30fcドオブジェクトのル\u30fcト")]
	private GameObject rootBoard;
	[SerializeField]
	[Header("各コインボ\u30fcド")]
	private GameObject[] arrayCoinBoard;
	[SerializeField]
	[Header("銅コイン")]
	private SpriteRenderer[] arrayBronzeCoin;
	[SerializeField]
	[Header("銀コイン")]
	private SpriteRenderer[] arraySilverCoin;
	[SerializeField]
	[Header("金コイン")]
	private SpriteRenderer[] arrayGoldCoin;
	[SerializeField]
	[Header("金コインエフェクト")]
	private GameObject[] arrayGoldCoinEffect;
	[SerializeField]
	[Header("コンプリ\u30fcトコイン")]
	private SpriteRenderer[] arrayCompleteCoin;
	[SerializeField]
	[Header("条件説明文")]
	private TextMeshPro[] arrayAchieveText;
	[SerializeField]
	[Header("キャラ表示管理クラス")]
	private GS_Teacher character;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private CursorManager[] arrayCursor;
	[SerializeField]
	[Header("コントロ\u30fcラ\u30fcボタン表示")]
	private GameObject objController;
	[SerializeField]
	[Header("キャラクタ\u30fc画像")]
	private SpriteRenderer characterSp;
	[SerializeField]
	[Header("キャラクタ\u30fc画像差分")]
	private Sprite[] arrayCharacterDiffSp;
	[SerializeField]
	[Header("ペ\u30fcジ")]
	private TextMeshPro page;
	[SerializeField]
	[Header("(DLC1)銅コイン")]
	[Header("--- DLC1 --------------------------")]
	private SpriteRenderer[] arrayBronzeCoin_DLC1;
	[SerializeField]
	[Header("(DLC1)銀コイン")]
	private SpriteRenderer[] arraySilverCoin_DLC1;
	[SerializeField]
	[Header("(DLC1)金コイン")]
	private SpriteRenderer[] arrayGoldCoin_DLC1;
	[SerializeField]
	[Header("(DLC1)コンプリ\u30fcトコイン")]
	private SpriteRenderer completeCoin_DLC1;
	private readonly float POS_DEFAULT_CONTROLLER = 525f;
	private readonly float POS_OUT_CONTROLLER = 1300f;
	private float POS_LEFT = -2000f;
	private float POS_CENTER;
	private float POS_RIGHT = 2000f;
	private BoardType currentBoardType;
	private bool isSliding;
	[SerializeField]
	[Header("DLCレイアウトのペ\u30fcジ番号名")]
	private string[] arrayPageSPNameDLC;
	private bool isOperation;
	private bool isInOutDLCFrame;
	private float captionShake;
	public void Open()
	{
		currentBoardType = BoardType.BRONZE;
		base.gameObject.SetActive(value: true);
		LeanTween.cancel(rootBoard);
		rootBoard.transform.SetLocalScale(0.001f, 0.001f, 1f);
		LeanTween.scale(rootBoard.gameObject, Vector3.one, 0.375f).setEaseOutQuart();
		LeanTween.cancel(base.gameObject);
		captionShake = ((UnityEngine.Random.Range(0, 2) == 0) ? (-7f) : 7f);
		LeanTween.value(base.gameObject, captionShake, 0f, 3.3f).setOnUpdate(delegate(float _value)
		{
			captionShake = _value;
		}).setEaseOutQuart();
		character.Set(GS_Define.GameType.MAX);
		if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX))
		{
			SetTeacherRenderer(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpenDLC1Complete());
		}
		else
		{
			SetTeacherRenderer(isTrophyCheck: true);
		}
		objController.transform.SetLocalPositionX(POS_OUT_CONTROLLER);
		LeanTween.moveLocalX(objController, POS_DEFAULT_CONTROLLER, 0.5f).setEaseOutQuint();
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			arrayCursor[i].SetCursorPos(0, 0);
			arrayCursor[i].IsStop = (i != (int)currentBoardType);
		}
		arrayCoinBoard[0].transform.SetLocalPositionX(POS_CENTER);
		arrayCoinBoard[1].transform.SetLocalPositionX(POS_RIGHT);
		arrayCoinBoard[2].transform.SetLocalPositionX(POS_LEFT);
		arrayCoinBoard[3].transform.SetLocalPositionX(POS_LEFT);
		arrayCoinBoard[4].transform.SetLocalPositionX(POS_LEFT);
		arrayCoinBoard[0].SetActive(value: true);
		arrayCoinBoard[1].SetActive(value: false);
		arrayCoinBoard[2].SetActive(value: false);
		arrayCoinBoard[3].SetActive(value: false);
		arrayCoinBoard[4].SetActive(value: false);
		isOperation = false;
		LeanTween.delayedCall(rootBoard.gameObject, 0.375f, (Action)delegate
		{
			arrayCoinBoard[1].SetActive(value: true);
			arrayCoinBoard[2].SetActive(value: true);
			arrayCoinBoard[3].SetActive(value: true);
			arrayCoinBoard[4].SetActive(value: true);
			isOperation = true;
		});
		PageUpdate();
		page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
		for (int j = 0; j < arrayAchieveText.Length; j++)
		{
			if (j == 4)
			{
				arrayAchieveText[j].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 178 + arrayCursor[j].GetSelectNo()));
				AchievementLabelCentering(arrayAchieveText[j]);
				continue;
			}
			UnityEngine.Debug.Log("_currentNo " + arrayCursor[j].GetSelectNo().ToString());
			int num = TrophyData.ConversionGameTypeNo(arrayCursor[j].GetSelectNo());
			UnityEngine.Debug.Log("返還後\u3000conversionGameTypeNo " + num.ToString());
			arrayAchieveText[j].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + j));
			AchievementLabelCentering(arrayAchieveText[j]);
		}
	}
	private void SetTeacherRenderer(bool isTrophyCheck)
	{
		if (!isTrophyCheck)
		{
			characterSp.sprite = arrayCharacterDiffSp[0];
			return;
		}
		int num = 0;
		for (int i = 0; i < GS_Define.FIRST_GAME_NUM; i++)
		{
			if (SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.BRONZE, TrophyData.ConversionGameTypeNo(i)))
			{
				num++;
			}
			if (SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.SILVER, TrophyData.ConversionGameTypeNo(i)))
			{
				num++;
			}
			if (SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.GOLD, TrophyData.ConversionGameTypeNo(i)))
			{
				num++;
			}
		}
		if (num >= 30)
		{
			characterSp.sprite = arrayCharacterDiffSp[1];
		}
		else
		{
			characterSp.sprite = arrayCharacterDiffSp[0];
		}
	}
	public void SetCursorEnable(bool _value)
	{
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			arrayCursor[i].IsStop = _value;
		}
	}
	private void Close()
	{
		character.Close();
		base.gameObject.SetActive(value: false);
		SingletonCustom<GS_GameSelectManager>.Instance.OnDetailBack();
		SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
	}
	private void PageUpdate()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
		bool flag = false;
		for (int i = 0; i < arrayBronzeCoin.Length; i++)
		{
			flag = SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.BRONZE, TrophyData.ConversionGameTypeNo(i));
			arrayBronzeCoin[i].SetAlpha(flag ? 1f : 0f);
		}
		for (int j = 0; j < arraySilverCoin.Length; j++)
		{
			flag = SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.SILVER, TrophyData.ConversionGameTypeNo(j));
			arraySilverCoin[j].SetAlpha(flag ? 1f : 0f);
		}
		for (int k = 0; k < arrayGoldCoin.Length; k++)
		{
			flag = SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.GOLD, TrophyData.ConversionGameTypeNo(k));
			arrayGoldCoin[k].SetAlpha(flag ? 1f : 0f);
			arrayGoldCoinEffect[k].SetActive(flag);
		}
		arrayBronzeCoin_DLC1[0].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.BRONZE, 2) ? 1f : 0f);
		arrayBronzeCoin_DLC1[1].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.BRONZE, 2) ? 1f : 0f);
		arraySilverCoin_DLC1[0].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.SILVER, 2) ? 1f : 0f);
		arraySilverCoin_DLC1[1].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.SILVER, 2) ? 1f : 0f);
		arrayGoldCoin_DLC1[0].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.GOLD, 2) ? 1f : 0f);
		arrayGoldCoin_DLC1[1].SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen(TrophyData.Type.GOLD, 2) ? 1f : 0f);
		completeCoin_DLC1.SetAlpha(SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpenDLC1Complete() ? 1f : 0f);
		for (int l = 0; l < arrayCursor.Length; l++)
		{
			if (isInOutDLCFrame && l != (int)currentBoardType)
			{
				arrayCursor[l].SetCursorPos(0, 0);
			}
			else if (l != 3 && l != 4)
			{
				arrayCursor[l].SetCursorPos(0, arrayCursor[(int)currentBoardType].GetSelectNo());
			}
		}
	}
	private void Update()
	{
		if (SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<CommonNotificationManager>.Instance.IsOpen || isSliding || !isOperation)
		{
			return;
		}
		int num = 0;
		if (arrayCursor[(int)currentBoardType].IsPushMovedButtonMoment())
		{
			for (int i = 0; i < arrayAchieveText.Length; i++)
			{
				if (i == 4)
				{
					arrayAchieveText[i].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 178 + arrayCursor[i].GetSelectNo()));
					AchievementLabelCentering(arrayAchieveText[i]);
				}
				else if (currentBoardType == BoardType.COMPLETE)
				{
					UnityEngine.Debug.Log("_currentNo " + arrayCursor[i].GetSelectNo().ToString());
					arrayAchieveText[i].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 108 + arrayCursor[i].GetSelectNo()));
					AchievementLabelCentering(arrayAchieveText[i]);
				}
				else
				{
					num = TrophyData.ConversionGameTypeNo(arrayCursor[i].GetSelectNo());
					UnityEngine.Debug.Log("返還後\u3000conversionGameTypeNo " + num.ToString());
					arrayAchieveText[i].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + i));
					AchievementLabelCentering(arrayAchieveText[i]);
				}
			}
		}
		if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B))
		{
			Close();
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.LeftShoulder))
		{
			if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX))
			{
				PageLeftMove_DLC1();
			}
			else
			{
				PageLeftMove();
			}
			PageUpdate();
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder))
		{
			if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX))
			{
				PageRightMove_DLC1();
			}
			else
			{
				PageRightMove();
			}
			PageUpdate();
		}
	}
	private void PageLeftMove()
	{
		int num = 0;
		switch (currentBoardType)
		{
		case BoardType.BRONZE:
			num = TrophyData.ConversionGameTypeNo(arrayCursor[0].GetSelectNo());
			arrayCursor[2].SetCursorPos(0, 0);
			arrayCoinBoard[1].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.GOLD;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int i = 0; i < arrayCursor.Length; i++)
				{
					arrayCursor[i].IsStop = (i != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			UnityEngine.Debug.Log("ばんごう：" + (69 + num * 3).ToString());
			arrayAchieveText[2].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 2));
			AchievementLabelCentering(arrayAchieveText[2]);
			break;
		case BoardType.SILVER:
			num = TrophyData.ConversionGameTypeNo(arrayCursor[1].GetSelectNo());
			arrayCursor[0].SetCursorPos(0, arrayCursor[1].GetSelectNo());
			arrayCoinBoard[2].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.BRONZE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int j = 0; j < arrayCursor.Length; j++)
				{
					arrayCursor[j].IsStop = (j != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[1].GetSelectNo());
			arrayAchieveText[0].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3));
			AchievementLabelCentering(arrayAchieveText[0]);
			break;
		case BoardType.GOLD:
			num = TrophyData.ConversionGameTypeNo(arrayCursor[2].GetSelectNo());
			arrayCursor[1].SetCursorPos(0, arrayCursor[2].GetSelectNo());
			arrayCoinBoard[0].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.SILVER;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int k = 0; k < arrayCursor.Length; k++)
				{
					arrayCursor[k].IsStop = (k != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[2].GetSelectNo());
			arrayAchieveText[1].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 1));
			AchievementLabelCentering(arrayAchieveText[1]);
			break;
		case BoardType.COMPLETE:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[2].SetCursorPos(0, 0);
			});
			arrayCoinBoard[0].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.GOLD;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int l = 0; l < arrayCursor.Length; l++)
				{
					arrayCursor[l].IsStop = (l != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[2].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 2));
			AchievementLabelCentering(arrayAchieveText[2]);
			break;
		}
	}
	private void PageRightMove()
	{
		int num = 0;
		switch (currentBoardType)
		{
		case BoardType.BRONZE:
			num = TrophyData.ConversionGameTypeNo(arrayCursor[0].GetSelectNo());
			arrayCursor[1].SetCursorPos(0, arrayCursor[0].GetSelectNo());
			arrayCoinBoard[2].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.SILVER;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int i = 0; i < arrayCursor.Length; i++)
				{
					arrayCursor[i].IsStop = (i != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			arrayAchieveText[1].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 1));
			AchievementLabelCentering(arrayAchieveText[1]);
			break;
		case BoardType.SILVER:
			arrayCursor[2].SetCursorPos(0, arrayCursor[1].GetSelectNo());
			arrayCoinBoard[0].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.GOLD;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int j = 0; j < arrayCursor.Length; j++)
				{
					arrayCursor[j].IsStop = (j != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[1].GetSelectNo());
			arrayAchieveText[2].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 2));
			AchievementLabelCentering(arrayAchieveText[2]);
			break;
		case BoardType.GOLD:
			arrayCursor[0].SetCursorPos(0, 0);
			arrayCoinBoard[1].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.BRONZE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int k = 0; k < arrayCursor.Length; k++)
				{
					arrayCursor[k].IsStop = (k != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[2].GetSelectNo());
			arrayAchieveText[0].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3));
			AchievementLabelCentering(arrayAchieveText[0]);
			break;
		case BoardType.COMPLETE:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[0].SetCursorPos(0, 0);
			});
			arrayCoinBoard[1].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.BRONZE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int l = 0; l < arrayCursor.Length; l++)
				{
					arrayCursor[l].IsStop = (l != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[0].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3));
			AchievementLabelCentering(arrayAchieveText[0]);
			break;
		}
	}
	private void PageLeftMove_DLC1()
	{
		isInOutDLCFrame = false;
		int num = 0;
		switch (currentBoardType)
		{
		case BoardType.BRONZE:
			arrayCursor[4].SetCursorPos(0, 0);
			arrayCoinBoard[1].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[4], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.DLC_1;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int i = 0; i < arrayCursor.Length; i++)
				{
					arrayCursor[i].IsStop = (i != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			arrayAchieveText[4].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 178));
			AchievementLabelCentering(arrayAchieveText[4]);
			break;
		case BoardType.SILVER:
			arrayCursor[0].SetCursorPos(0, arrayCursor[1].GetSelectNo());
			arrayCoinBoard[2].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.BRONZE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int j = 0; j < arrayCursor.Length; j++)
				{
					arrayCursor[j].IsStop = (j != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[1].GetSelectNo());
			arrayAchieveText[0].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3));
			AchievementLabelCentering(arrayAchieveText[0]);
			break;
		case BoardType.GOLD:
			arrayCursor[1].SetCursorPos(0, arrayCursor[2].GetSelectNo());
			arrayCoinBoard[3].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.SILVER;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int k = 0; k < arrayCursor.Length; k++)
				{
					arrayCursor[k].IsStop = (k != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[2].GetSelectNo());
			arrayAchieveText[1].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 1));
			AchievementLabelCentering(arrayAchieveText[1]);
			break;
		case BoardType.COMPLETE:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[2].SetCursorPos(0, 0);
			});
			arrayCoinBoard[4].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.GOLD;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int l = 0; l < arrayCursor.Length; l++)
				{
					arrayCursor[l].IsStop = (l != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[2].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 2));
			AchievementLabelCentering(arrayAchieveText[2]);
			break;
		case BoardType.DLC_1:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[3].SetCursorPos(0, 0);
			});
			arrayCoinBoard[0].transform.SetLocalPositionX(POS_LEFT);
			LeanTween.moveLocalX(arrayCoinBoard[4], POS_RIGHT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.COMPLETE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int m = 0; m < arrayCursor.Length; m++)
				{
					arrayCursor[m].IsStop = (m != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[3].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 108));
			AchievementLabelCentering(arrayAchieveText[3]);
			break;
		}
	}
	private void PageRightMove_DLC1()
	{
		isInOutDLCFrame = false;
		int num = 0;
		switch (currentBoardType)
		{
		case BoardType.BRONZE:
			arrayCursor[1].SetCursorPos(0, arrayCursor[0].GetSelectNo());
			arrayCoinBoard[2].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.SILVER;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int i = 0; i < arrayCursor.Length; i++)
				{
					arrayCursor[i].IsStop = (i != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[0].GetSelectNo());
			arrayAchieveText[1].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 1));
			AchievementLabelCentering(arrayAchieveText[1]);
			break;
		case BoardType.SILVER:
			arrayCursor[2].SetCursorPos(0, arrayCursor[1].GetSelectNo());
			arrayCoinBoard[3].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[1], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.GOLD;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int j = 0; j < arrayCursor.Length; j++)
				{
					arrayCursor[j].IsStop = (j != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(arrayCursor[1].GetSelectNo());
			arrayAchieveText[2].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3 + 2));
			AchievementLabelCentering(arrayAchieveText[2]);
			break;
		case BoardType.GOLD:
			arrayCursor[3].SetCursorPos(0, 0);
			arrayCoinBoard[4].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[2], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.COMPLETE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int k = 0; k < arrayCursor.Length; k++)
				{
					arrayCursor[k].IsStop = (k != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			arrayAchieveText[3].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 108));
			AchievementLabelCentering(arrayAchieveText[3]);
			break;
		case BoardType.COMPLETE:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[4].SetCursorPos(0, 0);
			});
			arrayCoinBoard[0].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[3], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[4], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.DLC_1;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int l = 0; l < arrayCursor.Length; l++)
				{
					arrayCursor[l].IsStop = (l != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[4].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 178));
			AchievementLabelCentering(arrayAchieveText[4]);
			break;
		case BoardType.DLC_1:
			WaitAfterExec(0.01f, delegate
			{
				arrayCursor[0].SetCursorPos(0, 0);
			});
			arrayCoinBoard[1].transform.SetLocalPositionX(POS_RIGHT);
			LeanTween.moveLocalX(arrayCoinBoard[4], POS_LEFT, 0.25f).setEaseInOutQuart();
			LeanTween.moveLocalX(arrayCoinBoard[0], POS_CENTER, 0.25f).setEaseInOutQuart().setOnComplete((Action)delegate
			{
				isSliding = false;
				currentBoardType = BoardType.BRONZE;
				page.SetText(((int)(currentBoardType + 1)).ToString() + "/3");
				for (int m = 0; m < arrayCursor.Length; m++)
				{
					arrayCursor[m].IsStop = (m != (int)currentBoardType);
				}
			});
			isSliding = true;
			arrayCursor[(int)currentBoardType].IsStop = true;
			num = TrophyData.ConversionGameTypeNo(0);
			arrayAchieveText[0].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 69 + num * 3));
			AchievementLabelCentering(arrayAchieveText[0]);
			break;
		}
	}
	private void AchievementLabelCentering(TextMeshPro label)
	{
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objController);
		LeanTween.cancel(rootBoard);
		LeanTween.cancel(base.gameObject);
		for (int i = 0; i < arrayCoinBoard.Length; i++)
		{
			LeanTween.cancel(arrayCoinBoard[i]);
		}
	}
}
