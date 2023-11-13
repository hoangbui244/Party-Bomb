using System;
using System.Collections.Generic;
using UnityEngine;
public class SwordFight_MainCharacterManager : SingletonCustom<SwordFight_MainCharacterManager>
{
	[Serializable]
	public struct UnifomrTexNoArrayStruct
	{
		public int texNo;
		public StyleTextureManager.GenderType genderType;
	}
	[SerializeField]
	[Header("キャラプレハブ")]
	private GameObject prefabChara;
	[SerializeField]
	[Header("エリアの中心アンカ\u30fc")]
	private Transform centerAreaAnchor;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private SwordFight_CharacterCursor[] cursor;
	private SwordFight_CharacterScript[] playerControlCharaList;
	private SwordFight_CharacterScript[] cpuCharaList;
	private SwordFight_MetaArtificialIntelligence metaAi = new SwordFight_MetaArtificialIntelligence();
	private SwordFight_CpuArtificialIntelligence cpuAi = new SwordFight_CpuArtificialIntelligence();
	private const float SHOOT_NEED_TIME = 1f;
	private float[] controlChangeInterval = new float[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	private bool[] isPlayer = new bool[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	private bool[] isPlayerAutoControl = new bool[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	private bool isTempAiMove;
	private const float PLAYER_ATTACK_INTERVAL = 0.5f;
	private List<int>[] teamMemberNoList;
	private float currentDeffenceTime;
	private UnifomrTexNoArrayStruct[] uniformTexNoArray = new UnifomrTexNoArrayStruct[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	public SwordFight_CharacterScript[] PlayerControlCharaList => playerControlCharaList;
	public SwordFight_CharacterScript[] CPUCharaList => cpuCharaList;
	public SwordFight_MetaArtificialIntelligence MetaAi => metaAi;
	public SwordFight_CpuArtificialIntelligence CpuAi => cpuAi;
	public UnifomrTexNoArrayStruct[] UniformTexNoArray => uniformTexNoArray;
	public void Init()
	{
		for (int i = 0; i < isPlayer.Length; i++)
		{
			UnityEngine.Debug.Log(i.ToString() + " : " + isPlayer[i].ToString());
		}
		for (int j = 0; j < cursor.Length; j++)
		{
			cursor[j].Show(_show: false);
		}
		UnityEngine.Debug.Log("プレイヤ\u30fc人数：" + SingletonCustom<GameSettingManager>.Instance.PlayerNum.ToString());
		if (!isPlayer[0])
		{
			isPlayer[0] = true;
		}
		for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++)
		{
			isPlayer[k] = true;
		}
		playerControlCharaList = new SwordFight_CharacterScript[SwordFight_Define.CHAMBARA_CHARACTER_MAX];
		cpuCharaList = new SwordFight_CharacterScript[0];
		List<UnifomrTexNoArrayStruct> list = new List<UnifomrTexNoArrayStruct>();
		List<int> list2 = new List<int>();
		for (int l = 0; l < playerControlCharaList.Length + CPUCharaList.Length; l++)
		{
			int num = 0;
			UnifomrTexNoArrayStruct unifomrTexNoArrayStruct = default(UnifomrTexNoArrayStruct);
			unifomrTexNoArrayStruct.genderType = ((UnityEngine.Random.Range(0, 2) != 0) ? StyleTextureManager.GenderType.GIRL : StyleTextureManager.GenderType.BOY);
			if (SingletonCustom<StyleTextureManager>.Instance.GetUseTexType() == StyleTextureManager.TexType.PLAIN_CLOTH)
			{
				while (list2.IndexOf(num) != -1)
				{
					num = UnityEngine.Random.Range(0, SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(unifomrTexNoArrayStruct.genderType));
				}
			}
			else
			{
				num = UnityEngine.Random.Range(0, SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(unifomrTexNoArrayStruct.genderType));
			}
			list2.Add(num);
			unifomrTexNoArrayStruct.texNo = num;
			list.Add(unifomrTexNoArrayStruct);
		}
		uniformTexNoArray = list.ToArray();
		new List<Transform>(SingletonCustom<SwordFight_FieldManager>.Instance.OriginAnchorList);
		cpuAi.Init();
		metaAi.Init();
	}
	public void CreatePlayer(bool _isInstantiate)
	{
		for (int i = 0; i < cursor.Length; i++)
		{
			cursor[i].Show(_show: false);
		}
		List<Transform> list = new List<Transform>(SingletonCustom<SwordFight_FieldManager>.Instance.OriginAnchorList);
		for (int j = 0; j < playerControlCharaList.Length; j++)
		{
			if (_isInstantiate)
			{
				playerControlCharaList[j] = UnityEngine.Object.Instantiate(prefabChara, base.transform.position, Quaternion.identity, list[0]).GetComponent<SwordFight_CharacterScript>();
			}
			playerControlCharaList[j].gameObject.name = "PLAYER_" + (j + 1).ToString();
			playerControlCharaList[j].transform.localPosition = Vector3.zero;
			switch (j)
			{
			case 0:
				playerControlCharaList[j].GameStartInit(j, SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0], -1, list[0], 0.5f);
				SingletonCustom<SwordFight_CharacterUIManager>.Instance.SetGauge(j, playerControlCharaList[j]);
				break;
			case 1:
				playerControlCharaList[j].GameStartInit(j, SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0], -1, list[0], 0.5f);
				SingletonCustom<SwordFight_CharacterUIManager>.Instance.SetGauge(j, playerControlCharaList[j]);
				break;
			}
			if (playerControlCharaList[j].IsCpu)
			{
				playerControlCharaList[j].AiInit(cpuAi.GetAiStrength(), cpuAi.GetRunSpeed(), cpuAi.GetAttackInterval(), cpuAi.GetDeffencePer(), cpuAi.GetDeffenceCounterPer());
			}
			list.RemoveAt(0);
		}
		for (int k = 0; k < playerControlCharaList.Length; k++)
		{
			playerControlCharaList[k].SetOpponent();
		}
	}
	public void SettingGameStart()
	{
		UnityEngine.Debug.Log("試合開始設定");
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (IsPlayer(i))
			{
				SettingPlayerControlChara(i, playerControlCharaList[i]);
			}
		}
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			cpuCharaList[j].AiStandby();
		}
		ChangeActionStateAllChara(SwordFight_CharacterScript.ActionState.STANDARD);
	}
	public void SettingRoundStart()
	{
		UnityEngine.Debug.Log("ラウンド開始設定");
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (IsPlayer(i))
			{
				playerControlCharaList[i].SetActionState(SwordFight_CharacterScript.ActionState.STANDARD);
				playerControlCharaList[i].RoundStartInit();
				playerControlCharaList[i].ChangeCharacterDisplayActive(_isHide: true);
			}
		}
		cpuAi.Init();
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			cpuCharaList[j].SetActionState(SwordFight_CharacterScript.ActionState.STANDARD);
			cpuCharaList[j].AiStandby();
			cpuCharaList[j].RoundStartInit();
			cpuCharaList[j].AiInit(cpuAi.GetAiStrength(), cpuAi.GetRunSpeed(), cpuAi.GetAttackInterval(), cpuAi.GetDeffencePer(), cpuAi.GetDeffenceCounterPer());
			cpuCharaList[j].ChangeCharacterDisplayActive(_isHide: true);
		}
	}
	public void SettingContinueRoundStart()
	{
		UnityEngine.Debug.Log("ラウンド続行設定");
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (IsPlayer(i))
			{
				if (playerControlCharaList[i].GetActionState() == SwordFight_CharacterScript.ActionState.DEATH)
				{
					playerControlCharaList[i].ChangeCharacterDisplayActive(_isHide: false);
					cursor[i].Show(_show: false);
				}
				else
				{
					playerControlCharaList[i].SetActionState(SwordFight_CharacterScript.ActionState.STANDARD);
					playerControlCharaList[i].RoundContinueStartInit();
				}
			}
		}
		cpuAi.Init();
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			if (cpuCharaList[j].GetActionState() == SwordFight_CharacterScript.ActionState.DEATH)
			{
				cpuCharaList[j].ChangeCharacterDisplayActive(_isHide: false);
				cursor[j].Show(_show: false);
				continue;
			}
			cpuCharaList[j].SetActionState(SwordFight_CharacterScript.ActionState.STANDARD);
			cpuCharaList[j].AiStandby();
			cpuCharaList[j].RoundContinueStartInit();
			cpuCharaList[j].AiInit(cpuAi.GetAiStrength(), cpuAi.GetRunSpeed(), cpuAi.GetAttackInterval(), cpuAi.GetDeffencePer(), cpuAi.GetDeffenceCounterPer());
		}
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState() == SwordFight_MainGameManager.GameState.ROUND_END_WAIT || SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState() == SwordFight_MainGameManager.GameState.GAME_END_WAIT || SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState() == SwordFight_MainGameManager.GameState.GAME_END || SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState() == SwordFight_MainGameManager.GameState.ANIMATION_WAIT)
		{
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++)
			{
				cursor[i].UpdateMethod();
			}
			for (int j = 0; j < controlChangeInterval.Length; j++)
			{
				controlChangeInterval[j] -= Time.deltaTime;
			}
			for (int k = 0; k < SwordFight_Define.CHAMBARA_CHARACTER_MAX; k++)
			{
				playerControlCharaList[k].EndUpdate();
			}
			SingletonCustom<SwordFight_CharacterUIManager>.Instance.UpdateCharacterUI();
			return;
		}
		UnityEngine.Debug.Log("update:" + SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState().ToString());
		metaAi.UpdateMethod();
		for (int l = 0; l < SwordFight_Define.CHAMBARA_CHARACTER_MAX; l++)
		{
			if (IsPlayer(l))
			{
				playerControlCharaList[l].AddGravity();
				if (!SingletonCustom<SwordFight_MainGameManager>.Instance.IsStopChara())
				{
					if (!SingletonCustom<SwordFight_MainGameManager>.Instance.IsAutoMove() && !CheckInRestrictedArea(playerControlCharaList[l]) && !playerControlCharaList[l].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
					{
						PlayerOperation(l);
						isPlayerAutoControl[l] = false;
					}
					else
					{
						isPlayerAutoControl[l] = true;
						cursor[l].Show(_show: false);
					}
				}
				isTempAiMove = false;
				playerControlCharaList[l].UpdateMethod();
			}
			else
			{
				playerControlCharaList[l].AddGravity();
				if (!SingletonCustom<SwordFight_MainGameManager>.Instance.IsStopChara())
				{
					cpuAi.UpdateAutoAction(l);
					metaAi.UpdateRestrictedAreaData();
					isTempAiMove = true;
				}
				playerControlCharaList[l].UpdateMethod();
			}
		}
		for (int m = 0; m < cpuCharaList.Length; m++)
		{
			cpuCharaList[m].AddGravity();
			if (!SingletonCustom<SwordFight_MainGameManager>.Instance.IsStopChara())
			{
				cpuAi.UpdateAutoAction(m);
				metaAi.UpdateRestrictedAreaData();
				isTempAiMove = true;
			}
			cpuCharaList[m].UpdateMethod();
		}
		for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerNum; n++)
		{
			cursor[n].UpdateMethod();
		}
		for (int num = 0; num < controlChangeInterval.Length; num++)
		{
			controlChangeInterval[num] -= Time.deltaTime;
		}
		SingletonCustom<SwordFight_CharacterUIManager>.Instance.UpdateCharacterUI();
	}
	private void SettingCursor(SwordFight_CharacterScript _chara)
	{
		controlChangeInterval[_chara.PlayerNo] = 1f;
		switch (_chara.PlayerNo)
		{
		case 0:
		{
			int num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0];
			break;
		}
		case 1:
		{
			int num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0];
			break;
		}
		}
	}
	public void ShowCursor(bool _show)
	{
		for (int i = 0; i < cursor.Length; i++)
		{
			cursor[i].Show(_show: false);
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerNum; j++)
		{
			switch (j)
			{
			case 0:
				if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0] < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
				{
					cursor[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0]].Show(_show: true);
				}
				break;
			case 1:
				if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0] < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
				{
					cursor[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0]].Show(_show: true);
				}
				break;
			}
		}
	}
	private void SettingPlayerControlChara(int _playerNo, SwordFight_CharacterScript _chara)
	{
		if (_playerNo != -1)
		{
			playerControlCharaList[_playerNo] = _chara;
			switch (_playerNo)
			{
			case 0:
				playerControlCharaList[_playerNo].PlayerNo = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0];
				break;
			case 1:
				playerControlCharaList[_playerNo].PlayerNo = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0];
				break;
			}
			cursor[_playerNo].ResetGauge();
			SettingCursor(playerControlCharaList[_playerNo]);
			playerControlCharaList[_playerNo].ResetControlInterval();
		}
	}
	public void ChangeActionStateAllChara(SwordFight_CharacterScript.ActionState _state)
	{
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (!(playerControlCharaList[i] == null))
			{
				playerControlCharaList[i].SetActionState(_state);
			}
		}
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			cpuCharaList[j].SetActionState(_state);
		}
	}
	private void PlayerOperation(int _playerNo)
	{
		if (SingletonCustom<SwordFight_ControllerManager>.Instance.GetAttackButtonPressTime(_playerNo) > 0f)
		{
			cursor[_playerNo].SetGauge(Mathf.Min(SingletonCustom<SwordFight_ControllerManager>.Instance.GetAttackButtonPressTime(_playerNo) / 1f, 1f));
		}
		else
		{
			cursor[_playerNo].ResetGauge();
		}
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.CheckGameState(SwordFight_MainGameManager.GameState.GAME_START_STANDBY))
		{
			return;
		}
		if (playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.STANDARD) || playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.MOVE))
		{
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsMove(_playerNo))
			{
				CharaMove(_playerNo);
			}
			else
			{
				CharaWait(_playerNo);
			}
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsJump_BtnDown(_playerNo))
			{
				Jump(_playerNo);
			}
			else if (!playerControlCharaList[_playerNo].IsJumpInput && SingletonCustom<SwordFight_ControllerManager>.Instance.IsDeffence_BtnDown(_playerNo))
			{
				CharaDeffence(_playerNo);
			}
			else if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsVerticalSlash(_playerNo))
			{
				CharaVerticalAttack(_playerNo);
			}
			else if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsHorizontalRightSlash(_playerNo))
			{
				CharaRightHorizontalAttack(_playerNo);
			}
		}
		else if (playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.ATTACK))
		{
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsMove(_playerNo))
			{
				CharaMove(_playerNo);
			}
			else
			{
				CharaWait(_playerNo);
			}
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsVerticalSlash(_playerNo))
			{
				CharaVerticalAttack(_playerNo);
			}
			else if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsHorizontalRightSlash(_playerNo))
			{
				CharaRightHorizontalAttack(_playerNo);
			}
		}
		else if (playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.DEFENCE))
		{
			CharaRotate(_playerNo);
			currentDeffenceTime += Time.deltaTime;
			if (currentDeffenceTime > SwordFight_Define.DEFFENCE_TIME)
			{
				CharaDeffenceReset(_playerNo);
			}
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsVerticalSlash(_playerNo))
			{
				CharaDeffenceReset(_playerNo);
				CharaVerticalAttack(_playerNo);
			}
			else if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsHorizontalRightSlash(_playerNo))
			{
				CharaDeffenceReset(_playerNo);
				CharaRightHorizontalAttack(_playerNo);
			}
		}
		else if (playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.FREEZE))
		{
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsDeffence_BtnDown(_playerNo))
			{
				currentDeffenceTime = 0f;
				CharaDeffence(_playerNo);
			}
		}
		else if (playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.REPEL))
		{
			if (SingletonCustom<SwordFight_ControllerManager>.Instance.IsDeffence_BtnDown(_playerNo))
			{
				currentDeffenceTime = 0f;
				CharaDeffence(_playerNo);
			}
		}
		else
		{
			playerControlCharaList[_playerNo].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH);
		}
	}
	private void CharaMove(int _no)
	{
		playerControlCharaList[_no].Move(SingletonCustom<SwordFight_ControllerManager>.Instance.GetMoveDir(_no), SingletonCustom<SwordFight_ControllerManager>.Instance.GetMoveLength(_no));
	}
	private void Jump(int _no)
	{
		playerControlCharaList[_no].Jump();
	}
	private void CharaRotate(int _no)
	{
		playerControlCharaList[_no].DeffenceRotateAnimation(SingletonCustom<SwordFight_ControllerManager>.Instance.GetMoveDir(_no));
	}
	private void CharaWait(int _no)
	{
		playerControlCharaList[_no].Wait();
	}
	private void CharaVerticalAttack(int _no)
	{
		playerControlCharaList[_no].VerticalSlashAnimation();
	}
	private void CharaRightHorizontalAttack(int _no)
	{
		playerControlCharaList[_no].HorizontalRightSlashAnimation();
	}
	private void CharaDeffence(int _no)
	{
		if (playerControlCharaList[_no].CheckUseDeffence() && playerControlCharaList[_no].GetActionState() != SwordFight_CharacterScript.ActionState.DEFENCE)
		{
			playerControlCharaList[_no].Dodge(SingletonCustom<SwordFight_ControllerManager>.Instance.GetMoveDir(_no));
		}
	}
	private void CharaDeffenceReset(int _no)
	{
		playerControlCharaList[_no].ResetDeffenceMotion();
	}
	public SwordFight_CharacterScript SearchMeNearest(SwordFight_CharacterScript _chara)
	{
		float num = 100f;
		int num2 = -1;
		for (int i = 0; i < cpuCharaList.Length; i++)
		{
			if (!(cpuCharaList[i] == _chara))
			{
				CalcManager.mCalcFloat = CalcManager.Length(cpuCharaList[i].GetPos(), _chara.GetPos());
				if (CalcManager.mCalcFloat < num)
				{
					num = CalcManager.mCalcFloat;
					num2 = i;
				}
			}
		}
		return cpuCharaList[num2];
	}
	public SwordFight_CharacterScript SearchMeNearestPlayerChara(SwordFight_CharacterScript _chara)
	{
		for (int i = 0; i < cpuCharaList.Length; i++)
		{
			SwordFight_CharacterScript swordFight_CharacterScript = playerControlCharaList[i];
			if (swordFight_CharacterScript.PlayerNo != -1)
			{
				return swordFight_CharacterScript;
			}
		}
		return cpuCharaList[0];
	}
	public SwordFight_CharacterScript SearchPosNearest(Vector3 _pos)
	{
		float num = 100f;
		int num2 = -1;
		for (int i = 0; i < cpuCharaList.Length; i++)
		{
			CalcManager.mCalcFloat = CalcManager.Length(cpuCharaList[i].GetPos(), _pos);
			if (CalcManager.mCalcFloat < num)
			{
				num = CalcManager.mCalcFloat;
				num2 = i;
			}
		}
		return cpuCharaList[num2];
	}
	public SwordFight_CharacterScript SearchPosNearestFormation(Vector3 _pos)
	{
		float num = 100f;
		int num2 = -1;
		for (int i = 0; i < cpuCharaList.Length; i++)
		{
			CalcManager.mCalcFloat = CalcManager.Length(cpuCharaList[i].GetFormationPos(_local: false, _half: true), _pos);
			if (CalcManager.mCalcFloat < num)
			{
				num = CalcManager.mCalcFloat;
				num2 = i;
			}
		}
		return cpuCharaList[num2];
	}
	public Vector3 SearchSpacePos(Vector3 _centerPos, int _xRangeMin, int _xRangeMax, int _yRnageMin, int _yRnageMax, int _teamNo)
	{
		return metaAi.GetSpacePos(_centerPos, _xRangeMin, _xRangeMax, _yRnageMin, _yRnageMax, _teamNo);
	}
	public bool CheckControlChara(SwordFight_CharacterScript _chara)
	{
		if (_chara.PlayerNo == -1 || _chara.PlayerNo == -2)
		{
			return false;
		}
		if (_chara.PlayerNo >= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
			return false;
		}
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0] == _chara.PlayerNo)
		{
			return playerControlCharaList[0] == _chara;
		}
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0] == _chara.PlayerNo)
		{
			return playerControlCharaList[1] == _chara;
		}
		return playerControlCharaList[_chara.PlayerNo] == _chara;
	}
	public bool CheckPlayerControl(SwordFight_CharacterScript _chara)
	{
		if (isPlayer[_chara.PlayerNo] && playerControlCharaList[_chara.PlayerNo] == _chara)
		{
			return true;
		}
		return false;
	}
	public bool CheckInRestrictedArea(SwordFight_CharacterScript _chara)
	{
		return false;
	}
	public SwordFight_CharacterCursor GetCursor(int _playerNo)
	{
		return cursor[_playerNo];
	}
	public List<SwordFight_CharacterScript> GetPositionCenterNearOrderChara()
	{
		List<SwordFight_CharacterScript> list = new List<SwordFight_CharacterScript>();
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (IsPlayer(i))
			{
				list.Add(playerControlCharaList[i]);
			}
		}
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			list.Add(cpuCharaList[j]);
		}
		float[] array = new float[list.Count];
		for (int k = 0; k < list.Count; k++)
		{
			list[k] = cpuCharaList[k];
			array[k] = CalcManager.Length(list[k].GetFormationPos(_local: false, _half: true), SingletonCustom<SwordFight_FieldManager>.Instance.GetAnchors().CenterAnchor.position);
		}
		for (int l = 0; l < list.Count - 1; l++)
		{
			for (int m = l; m < list.Count; m++)
			{
				if (array[m] < array[l])
				{
					float num = array[l];
					array[l] = array[m];
					array[m] = num;
					SwordFight_CharacterScript value = list[l];
					list[l] = list[m];
					list[m] = value;
				}
			}
		}
		return list;
	}
	public SwordFight_CharacterScript GetChara(GameObject _obj)
	{
		for (int i = 0; i < playerControlCharaList.Length; i++)
		{
			if (IsPlayer(i) && playerControlCharaList[i].CheckObj(_obj))
			{
				return playerControlCharaList[i];
			}
		}
		for (int j = 0; j < cpuCharaList.Length; j++)
		{
			for (int k = 0; k < cpuCharaList.Length; k++)
			{
				if (cpuCharaList[j].CheckObj(_obj))
				{
					return cpuCharaList[j];
				}
			}
		}
		UnityEngine.Debug.LogError("存在しないキャラです : " + _obj.name);
		return null;
	}
	public SwordFight_CharacterScript GetControlChara(int _playerNo)
	{
		return playerControlCharaList[_playerNo];
	}
	public SwordFight_CharacterScript GetCPUChara(int _no)
	{
		int num = _no - SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if (num < 0)
		{
			num = 0;
		}
		if (cpuCharaList[num] != null)
		{
			return cpuCharaList[num];
		}
		return null;
	}
	public SwordFight_MetaArtificialIntelligence.InRestrictedArea GetRestrictedArea(int _teamNo)
	{
		return metaAi.GetRestrictedArea(_teamNo);
	}
	public bool IsPlayer(int _playerNo)
	{
		switch (_playerNo)
		{
		case 0:
			return SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0] < SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		case 1:
			return SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0] < SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		case -1:
			return false;
		default:
			return isPlayer[_playerNo];
		}
	}
	public Transform GetCenterAreaAnchor()
	{
		return centerAreaAnchor;
	}
	public Vector3 ConvertOptimalPositioning(Vector3 _pos, int _no, bool _attack)
	{
		Vector3 vector = SingletonCustom<SwordFight_FieldManager>.Instance.ConvertLocalPosPer(_pos, _no);
		vector.x += 0.5f;
		Vector3 posPer = default(Vector3);
		if (_attack)
		{
			posPer.x = vector.x;
			posPer.z = vector.z * 0.4f + 0.6f;
			posPer.y = 0f;
		}
		else
		{
			posPer.x = vector.x;
			posPer.z = vector.z * 0.4f;
			posPer.y = 0f;
		}
		posPer.x -= 0.5f;
		return SingletonCustom<SwordFight_FieldManager>.Instance.ConvertPosPerToWorld(posPer, _no);
	}
	public Vector3 ConvertRestrictedArea(SwordFight_CharacterScript _chara, Vector3 _pos)
	{
		return _pos;
	}
	public UnifomrTexNoArrayStruct GetCharacterUniformData(int _playerNo)
	{
		int num = _playerNo;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (num == -1)
			{
				num = 1;
			}
		}
		else
		{
			switch (num)
			{
			case -1:
				num = 2;
				break;
			case -2:
				num = 3;
				break;
			}
		}
		return uniformTexNoArray[num];
	}
	public void SetCharacterUniformData(int _playerNo, int _texNo, StyleTextureManager.GenderType _genderType)
	{
		int num = _playerNo;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (num == -1)
			{
				num = 1;
			}
		}
		else
		{
			switch (num)
			{
			case -1:
				num = 2;
				break;
			case -2:
				num = 3;
				break;
			}
		}
		uniformTexNoArray[num].texNo = _texNo;
		uniformTexNoArray[num].genderType = _genderType;
	}
}
