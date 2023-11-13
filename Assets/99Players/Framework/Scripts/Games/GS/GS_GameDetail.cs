using GamepadInput;
using System.Collections.Generic;
using UnityEngine;
public class GS_GameDetail : SingletonCustom<GS_GameDetail>
{
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("ボ\u30fcドオブジェクトのル\u30fcト")]
	private GameObject rootBoard;
	[SerializeField]
	[Header("コントロ\u30fcラ\u30fc表示のル\u30fcト")]
	private GameObject rootController;
	[SerializeField]
	[Header("操作説明呼び出し表示のル\u30fcト")]
	private GameObject rootOperationInfo;
	[SerializeField]
	[Header("貼り付け内容のル\u30fcト")]
	private GameObject rootPastedContents;
	[SerializeField]
	[Header("サムネイル")]
	private SpriteRenderer sprThumb;
	[SerializeField]
	[Header("サムネイル画像")]
	private Sprite[] arraySpThumb;
	[SerializeField]
	[Header("最高記録の表示管理クラス")]
	private GS_BestRecord bestRecord;
	[SerializeField]
	[Header("簡易ゲ\u30fcム内容表示管理クラス")]
	private GS_GameInfo gameInfo;
	[SerializeField]
	[Header("チ\u30fcム分け管理クラス")]
	private GS_TeamAssignment teamAssignment;
	[SerializeField]
	[Header("ゲ\u30fcムモ\u30fcド管理クラス")]
	private GS_GameMode gameMode;
	[SerializeField]
	[Header("イニング数管理クラス")]
	private GS_Inning inning;
	[SerializeField]
	[Header("試合時間管理クラス")]
	private GS_MatchTime matchTime;
	[SerializeField]
	[Header("チ\u30fcムモ\u30fcド管理クラス")]
	private GS_TeamMode teamMode;
	[SerializeField]
	[Header("水泳ゲ\u30fcムモ\u30fcド管理クラス")]
	private GS_SwimmingMode swimmingMode;
	[SerializeField]
	[Header("コ\u30fcス選択管理クラス")]
	private GS_CourseSelect courseSelect;
	[SerializeField]
	[Header("ステ\u30fcジ選択管理クラス")]
	private GS_StageSelect stageSelect;
	[SerializeField]
	[Header("ハイスコア表示管理クラス")]
	private GS_Hiscore hiscore;
	[SerializeField]
	[Header("チ\u30fcム分け")]
	private GameObject popTeamAssignment;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private GameObject[] teamPlayerIcon;
	[SerializeField]
	[Header("鬼ごっこチ\u30fcム分け")]
	private GS_OniAssignment oniAssignment;
	[SerializeField]
	[Header("先生キャラ表示管理クラス")]
	private GS_Teacher teacher;
	[SerializeField]
	[Header("ボタン操作表示管理クラス")]
	private GS_Controller controller;
	[SerializeField]
	[Header("チ\u30fcム分けフレ\u30fcム画像")]
	private SpriteRenderer backFrame;
	private readonly float[] POS_TEAM_ICON_X = new float[8]
	{
		-420f,
		-303f,
		-186f,
		-68f,
		68f,
		186f,
		303f,
		420f
	};
	private readonly float BOARD_ROOT_SHORT_Y = -118f;
	private readonly int TEAM_PLAYER_NUM = 4;
	private GS_Define.GameType currentType;
	private bool isTeamAssignment;
	private List<int>[] playerGroupList;
	private float easeBoardRootY;
	private float easePastedPootY;
	private bool isShortLayout;
	private int frameLockCnt;
	public void Open(GS_Define.GameType _idx, bool _isFade = true)
	{
		currentType = _idx;
		base.gameObject.SetActive(value: true);
		SetHiscore();
		if (_isFade)
		{
			LeanTween.cancel(fade.gameObject);
			fade.SetAlpha(0f);
			LeanTween.value(fade.gameObject, 0f, 0.7f, 0.25f).setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			}).setEaseOutExpo();
		}
		SetLayout(_idx);
		if (CheckPlayerGroupSetting(_isShowDialog: false))
		{
			SetPlayerGroup();
			SetLayout(_idx);
		}
		sprThumb.sprite = arraySpThumb[(int)currentType];
		SetBackFrame(_idx);
		SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = _idx;
	}
	private void SetLayout(GS_Define.GameType _type)
	{
		SingletonCustom<GameSettingManager>.Instance.ResetDetailSetting();
		easeBoardRootY = 55f;
		easePastedPootY = 56f;
		isShortLayout = false;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			switch (_type)
			{
			case GS_Define.GameType.GET_BALL:
				gameInfo.SetShort(_type, _messageFrameShort: true);
				matchTime.Set(_type);
				isShortLayout = true;
				break;
			case GS_Define.GameType.CANNON_SHOT:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				gameMode.Set(_type);
				bestRecord.Set(_type);
				courseSelect.SetThreeCourse(_type);
				break;
			case GS_Define.GameType.BLOCK_WIPER:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				oniAssignment.Set(_type);
				break;
			case GS_Define.GameType.MOLE_HAMMER:
				gameInfo.SetShort(_type, _messageFrameShort: true);
				inning.Set(_type);
				isShortLayout = true;
				break;
			case GS_Define.GameType.BOMB_ROULETTE:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				hiscore.Set(_type);
				break;
			case GS_Define.GameType.RECEIVE_PON:
				gameInfo.SetShort(_type, _messageFrameShort: false);
				isShortLayout = true;
				break;
			case GS_Define.GameType.DELIVERY_ORDER:
				gameInfo.SetShort(_type, _messageFrameShort: false);
				isShortLayout = true;
				break;
			case GS_Define.GameType.ARCHER_BATTLE:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				hiscore.Set(_type);
				break;
			case GS_Define.GameType.ATTACK_BALL:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				hiscore.Set(_type);
				break;
			case GS_Define.GameType.BLOW_AWAY_TANK:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				hiscore.Set(_type);
				break;
			}
		}
		else
		{
			switch (_type)
			{
			case GS_Define.GameType.GET_BALL:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				matchTime.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			case GS_Define.GameType.CANNON_SHOT:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				courseSelect.SetThreeCourse(_type);
				break;
			case GS_Define.GameType.BLOCK_WIPER:
				gameInfo.SetLong(_type, _messageFrameShort: false);
				oniAssignment.Set(_type);
				break;
			case GS_Define.GameType.MOLE_HAMMER:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				inning.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			case GS_Define.GameType.BOMB_ROULETTE:
				gameInfo.SetShort(_type, _messageFrameShort: false);
				isShortLayout = true;
				break;
			case GS_Define.GameType.RECEIVE_PON:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				teamMode.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			case GS_Define.GameType.DELIVERY_ORDER:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				teamMode.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			case GS_Define.GameType.ARCHER_BATTLE:
				gameInfo.SetShort(_type, _messageFrameShort: true);
				isShortLayout = true;
				break;
			case GS_Define.GameType.ATTACK_BALL:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				teamMode.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			case GS_Define.GameType.BLOW_AWAY_TANK:
				gameInfo.SetLong(_type, _messageFrameShort: true);
				teamMode.Set(_type);
				popTeamAssignment.SetActive(value: true);
				break;
			}
		}
		if (isShortLayout)
		{
			easeBoardRootY = 57f;
			easePastedPootY = 176f;
		}
		if ((bool)popTeamAssignment)
		{
			StartTeamAssignment();
			if (_type == GS_Define.GameType.GET_BALL || _type == GS_Define.GameType.MOLE_HAMMER)
			{
				teamAssignment.SetEnable();
			}
			else
			{
				teamAssignment.SetDisable();
			}
		}
		teacher.Set(_type);
		controller.Set(_type);
		rootBoard.transform.SetLocalPositionY(easeBoardRootY);
		rootPastedContents.transform.SetLocalPositionY(0f - easePastedPootY);
		LeanTween.cancel(rootBoard);
		rootBoard.transform.SetLocalScale(0.001f, 0.001f, 1f);
		LeanTween.scale(rootBoard.gameObject, Vector3.one, 0.375f).setEaseOutQuart();
		frameLockCnt = 23;
	}
	public void OnCloseOperationInfo()
	{
		rootBoard.gameObject.SetActive(value: true);
		rootController.gameObject.SetActive(value: true);
		rootOperationInfo.gameObject.SetActive(value: true);
		frameLockCnt = 1;
		Open(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, _isFade: false);
	}
	public void UpdateData()
	{
		if (bestRecord.gameObject.activeSelf)
		{
			bestRecord.Set(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType);
		}
		if (hiscore.gameObject.activeSelf)
		{
			hiscore.Set(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType);
		}
	}
	public void Close(bool _isSe = true)
	{
		inning.Close();
		gameMode.Close();
		matchTime.Close();
		teamMode.Close();
		bestRecord.Close();
		hiscore.Close();
		courseSelect.Close();
		stageSelect.Close();
		oniAssignment.Close();
		swimmingMode.Close();
		controller.Close();
		popTeamAssignment.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		SingletonCustom<GS_GameSelectManager>.Instance.OnDetailBack();
		if (_isSe)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
		}
	}
	private void SetHiscore()
	{
		switch (currentType)
		{
		}
	}
	private bool IsTeamAssignmentDisable()
	{
		switch (currentType)
		{
		case GS_Define.GameType.CANNON_SHOT:
		case GS_Define.GameType.BLOCK_WIPER:
		case GS_Define.GameType.BOMB_ROULETTE:
		case GS_Define.GameType.RECEIVE_PON:
		case GS_Define.GameType.ARCHER_BATTLE:
		case GS_Define.GameType.BLOW_AWAY_TANK:
			return true;
		default:
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				return true;
			}
			return false;
		}
	}
	private void StartGame()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
		UnityEngine.Debug.Log("select:" + SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx.ToString());
		switch (currentType)
		{
		case GS_Define.GameType.GET_BALL:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.GET_BALL);
			break;
		case GS_Define.GameType.CANNON_SHOT:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.ARCHER_BATTLE);
			break;
		case GS_Define.GameType.BLOCK_WIPER:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.BLOCK_WIPER);
			break;
		case GS_Define.GameType.MOLE_HAMMER:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MOLE_HAMMER);
			break;
		case GS_Define.GameType.BOMB_ROULETTE:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.BOMB_ROULETTE);
			break;
		case GS_Define.GameType.RECEIVE_PON:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.RECEIVE_PON);
			break;
		case GS_Define.GameType.DELIVERY_ORDER:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.BLACKSMITH);
			break;
		case GS_Define.GameType.ARCHER_BATTLE:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.CANNON_SHOT);
			break;
		case GS_Define.GameType.ATTACK_BALL:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.ATTACK_BALL);
			break;
		case GS_Define.GameType.BLOW_AWAY_TANK:
			SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.BLOW_AWAY_TANK);
			break;
		}
	}
	private void StartTeamAssignment()
	{
		playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			teamPlayerIcon[i].SetActive(i < SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		}
		for (int j = 0; j < playerGroupList.Length; j++)
		{
			for (int k = 0; k < playerGroupList[j].Count; k++)
			{
				UnityEngine.Debug.Log("i:" + j.ToString() + " j:" + k.ToString());
				teamPlayerIcon[playerGroupList[j][k]].transform.SetLocalPositionX(POS_TEAM_ICON_X[j * TEAM_PLAYER_NUM + playerGroupList[j][k]]);
			}
		}
	}
	private void SetPlayerGroup()
	{
		UnityEngine.Debug.Log("set:" + SingletonCustom<GameSettingManager>.Instance.PlayerNum.ToString());
		playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		for (int i = 0; i < playerGroupList.Length; i++)
		{
			playerGroupList[i].Clear();
		}
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			playerGroupList[0].Add(0);
			break;
		case 2:
			playerGroupList[0].Add(0);
			playerGroupList[1].Add(1);
			break;
		case 3:
			playerGroupList[0].Add(0);
			playerGroupList[1].Add(1);
			playerGroupList[0].Add(2);
			break;
		case 4:
			playerGroupList[0].Add(0);
			playerGroupList[1].Add(1);
			playerGroupList[0].Add(2);
			playerGroupList[1].Add(3);
			break;
		}
	}
	private bool CheckPlayerGroupSetting(bool _isShowDialog)
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			switch (currentType)
			{
			case GS_Define.GameType.MOLE_HAMMER:
			case GS_Define.GameType.RECEIVE_PON:
			case GS_Define.GameType.BLOW_AWAY_TANK:
				if (GetLeftTeamNum() <= 2 && GetRightTeamNum() <= 2)
				{
					break;
				}
				if (_isShowDialog)
				{
					if (currentType != GS_Define.GameType.MOLE_HAMMER && SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
					{
						return false;
					}
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
					SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 61), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate
					{
					}), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
				}
				return true;
			case GS_Define.GameType.BLOCK_WIPER:
				if (GetLeftTeamNum() > 2 || GetRightTeamNum() > 2)
				{
					if (_isShowDialog)
					{
						if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
						{
							return false;
						}
						SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
						SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 61), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate
						{
						}), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
					}
					return true;
				}
				if (GetLeftTeamNum() > 0 && GetRightTeamNum() > 0)
				{
					break;
				}
				if (_isShowDialog)
				{
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
					{
						return false;
					}
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
					SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 62), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate
					{
					}), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
				}
				return true;
			}
		}
		return false;
	}
	private int GetRightTeamNum()
	{
		CalcManager.mCalcInt = 0;
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x > 0f)
			{
				CalcManager.mCalcInt++;
			}
		}
		return CalcManager.mCalcInt;
	}
	private int GetLeftTeamNum()
	{
		CalcManager.mCalcInt = 0;
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x < 0f)
			{
				CalcManager.mCalcInt++;
			}
		}
		return CalcManager.mCalcInt;
	}
	private void Update()
	{
		if (SingletonCustom<CommonNotificationManager>.Instance.IsOpen || SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<SceneManager>.Instance.IsLoading || SingletonCustom<DM>.Instance.IsActive())
		{
			return;
		}
		if (frameLockCnt > 0)
		{
			frameLockCnt--;
		}
		else if (popTeamAssignment.activeSelf)
		{
			if (teamAssignment.IsEnable)
			{
				switch (currentType)
				{
				case GS_Define.GameType.RECEIVE_PON:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
					{
						teamAssignment.SetDisable();
					}
					break;
				case GS_Define.GameType.BLOCK_WIPER:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
					{
						teamAssignment.SetDisable();
					}
					break;
				case GS_Define.GameType.BLOW_AWAY_TANK:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode != 1)
					{
						teamAssignment.SetDisable();
					}
					break;
				}
			}
			else
			{
				switch (currentType)
				{
				case GS_Define.GameType.RECEIVE_PON:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode == 1)
					{
						teamAssignment.SetEnable();
					}
					break;
				case GS_Define.GameType.BLOCK_WIPER:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode == 1)
					{
						teamAssignment.SetEnable();
					}
					break;
				case GS_Define.GameType.BLOW_AWAY_TANK:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameMode == 1)
					{
						teamAssignment.SetEnable();
					}
					break;
				}
			}
			if (teamAssignment.IsEnable)
			{
				for (int i = 0; i < teamPlayerIcon.Length; i++)
				{
					if (teamPlayerIcon[i].activeSelf)
					{
						if (teamPlayerIcon[i].transform.localPosition.x > 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Left))
						{
							teamPlayerIcon[i].transform.SetLocalPositionX(POS_TEAM_ICON_X[i]);
							SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
						}
						else if (teamPlayerIcon[i].transform.localPosition.x < 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Right))
						{
							teamPlayerIcon[i].transform.SetLocalPositionX(POS_TEAM_ICON_X[TEAM_PLAYER_NUM + i]);
							SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
						}
					}
				}
			}
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				if (CheckPlayerGroupSetting(_isShowDialog: true))
				{
					return;
				}
				for (int j = 0; j < playerGroupList.Length; j++)
				{
					playerGroupList[j].Clear();
				}
				for (int k = 0; k < teamPlayerIcon.Length; k++)
				{
					if (teamPlayerIcon[k].activeSelf)
					{
						playerGroupList[(teamPlayerIcon[k].transform.localPosition.x > 0f) ? 1 : 0].Add(k);
					}
				}
				SingletonCustom<GameSettingManager>.Instance.PlayerGroupList = playerGroupList;
				StartGame();
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B))
			{
				Close();
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				rootBoard.gameObject.SetActive(value: false);
				rootController.gameObject.SetActive(value: false);
				rootOperationInfo.gameObject.SetActive(value: false);
				teacher.Close();
				SingletonCustom<CommonNotificationManager>.Instance.OpenOperationInfoAtGameSelect();
			}
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
		{
			if (!oniAssignment.gameObject.activeSelf || oniAssignment.IsAssignment())
			{
				StartGame();
			}
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B))
		{
			Close();
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
			rootBoard.gameObject.SetActive(value: false);
			rootController.gameObject.SetActive(value: false);
			rootOperationInfo.gameObject.SetActive(value: false);
			teacher.Close();
			SingletonCustom<CommonNotificationManager>.Instance.OpenOperationInfoAtGameSelect();
		}
	}
	private void SetBackFrame(GS_Define.GameType _type)
	{
		switch (_type)
		{
		case GS_Define.GameType.GET_BALL:
		case GS_Define.GameType.CANNON_SHOT:
		case GS_Define.GameType.BLOCK_WIPER:
		case GS_Define.GameType.MOLE_HAMMER:
		case GS_Define.GameType.BOMB_ROULETTE:
		case GS_Define.GameType.RECEIVE_PON:
			return;
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(fade.gameObject);
		LeanTween.cancel(rootBoard);
	}
}
