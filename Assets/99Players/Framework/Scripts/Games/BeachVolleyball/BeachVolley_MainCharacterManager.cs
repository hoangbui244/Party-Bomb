using System;
using System.Collections.Generic;
using UnityEngine;
public class BeachVolley_MainCharacterManager : SingletonCustom<BeachVolley_MainCharacterManager>
{
	[Serializable]
	public struct CharacterList
	{
		public BeachVolley_Character[] charas;
	}
	public struct RotationChangeData
	{
		public BeachVolley_Character charaTemp;
		public Transform formationAnchor;
		public BeachVolley_Define.PositionType positionType;
		public BeachVolley_Character.PositionState positionState;
	}
	private bool AUTOPLAYFlag = true;
	private float PLAYER_RESTRICTED_AREA_MAG = 1.5f;
	public readonly int BALL_TOUCH_LIMIT = 3;
	public float GAUGE_MAX_NEED_TIME = 0.8f;
	public float GAUGE_OUT_ADD_VALUE = 0.2f;
	public float GAUGE_SAFE_ADD_VALUE = 0.2f;
	public int SUPER_JUMP_PER = 50;
	private CharacterList[] teamList;
	private BeachVolley_Character[] zoneCharaList;
	private BeachVolley_Character haveBallCharaPrev;
	private BeachVolley_Character haveBallChara;
	private BeachVolley_Character[] controlChara = new BeachVolley_Character[5];
	private RotationChangeData rotationChangeData;
	[SerializeField]
	[Header("キャラクタ\u30fc")]
	private BeachVolley_Character charaPrefab;
	[SerializeField]
	[Header("カ\u30fcソルの親")]
	private Transform cursorParent;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private BeachVolley_CharacterCursor[] cursor;
	[SerializeField]
	[Header("パス先サ\u30fcクル")]
	private BeachVolley_CharacterCursor[] passCircle;
	private float[] controlChangeInterval = new float[5];
	private List<int>[] playerList = new List<int>[2];
	public List<int>[] teamUserList = new List<int>[2];
	private int[] nextChangeNo = new int[2];
	private bool isOptionalControl;
	private BeachVolley_MetaAI metaAi = new BeachVolley_MetaAI();
	private BeachVolley_CpuAI cpuAi = new BeachVolley_CpuAI();
	private float[] ballCharacterDistance = new float[4];
	private int ballControllTeam;
	private int ballTouchCnt;
	private int checkOutOfBoundsCnt;
	public readonly BeachVolley_Character.PositionState[] positionTypeTable = new BeachVolley_Character.PositionState[6]
	{
		BeachVolley_Character.PositionState.BACK_ZONE,
		BeachVolley_Character.PositionState.FRONT_ZONE,
		BeachVolley_Character.PositionState.FRONT_ZONE,
		BeachVolley_Character.PositionState.FRONT_ZONE,
		BeachVolley_Character.PositionState.BACK_ZONE,
		BeachVolley_Character.PositionState.BACK_ZONE
	};
	private BeachVolley_Character[] controllChara = new BeachVolley_Character[2];
	private BeachVolley_Character[] lastHitChara = new BeachVolley_Character[2];
	private Vector3[] dropPosPrev = new Vector3[2]
	{
		Vector3.zero,
		Vector3.zero
	};
	private int[] rotateAutoPlayTeam1;
	private int[] rotateAutoPlayTeam2;
	private int[] rotateAutoPlayer4Front = new int[2];
	private int[] rotateAutoPlayer4Back = new int[2];
	private int rotatePlayer4;
	private int[] rotatePlayerNow23 = new int[2];
	private int[] rotatePlayerNow4FB = new int[2]
	{
		0,
		2
	};
	private int rotatePlayerNow4;
	private BeachVolley_Character[] cPrev = new BeachVolley_Character[5];
	private BeachVolley_Character lasthitCharaPrev;
	private BeachVolley_Character[] cPrevAttackStay = new BeachVolley_Character[5];
	public CharacterList[] TeamList => teamList;
	public BeachVolley_Character[] ControlChara => controlChara;
	public bool IsOptionalControl
	{
		get
		{
			return true;
		}
		set
		{
			isOptionalControl = value;
		}
	}
	public BeachVolley_MetaAI MetaAi => metaAi;
	public BeachVolley_CpuAI CpuAi => cpuAi;
	public int BallControllTeam
	{
		get
		{
			return ballControllTeam;
		}
		set
		{
			ballControllTeam = value;
		}
	}
	public int BallTouchCnt
	{
		get
		{
			return ballTouchCnt;
		}
		set
		{
			ballTouchCnt = value;
		}
	}
	public BeachVolley_Character.ActionState EnemyActionState(int _team)
	{
		_team = 1 - _team;
		for (int i = 0; i < teamList[_team].charas.Length; i++)
		{
			if (teamList[_team].charas[i].playerNo >= 0)
			{
				UnityEngine.Debug.Log("プレイヤ\u30fcナンバ\u30fc：" + teamList[_team].charas[i].playerNo.ToString());
				UnityEngine.Debug.Log("キャラナンバ\u30fc：" + i.ToString());
				return teamList[_team].charas[i].GetActionState();
			}
		}
		return BeachVolley_Character.ActionState.STANDARD;
	}
	public Transform GetCursorParent()
	{
		return cursorParent;
	}
	public static bool IsGameWatchingMode()
	{
		return false;
	}
	public static void SetGameWatchingMode(bool _flg)
	{
	}
	public void SetLayer1(int layerN)
	{
	}
	public void Init()
	{
		AUTOPLAYFlag = true;
		for (int i = 0; i < 2; i++)
		{
			playerList[i] = new List<int>();
			teamUserList[i] = new List<int>();
		}
		List<int> list = new List<int>();
		switch (BeachVolley_Define.PLAYER_NUM)
		{
		case 1:
		{
			playerList[0].Add(0);
			teamUserList[0].Add(playerList[0][0]);
			list.Add(4);
			list.Add(5);
			list.Add(6);
			int index = UnityEngine.Random.Range(0, list.Count);
			teamUserList[0].Add(list[index]);
			list.RemoveAt(index);
			index = UnityEngine.Random.Range(0, list.Count);
			teamUserList[1].Add(list[index]);
			list.RemoveAt(index);
			teamUserList[1].Add(list[0]);
			break;
		}
		case 2:
		{
			playerList[0].Add(UnityEngine.Random.Range(0, 2));
			playerList[1].Add((playerList[0][0] == 0) ? 1 : 0);
			teamUserList[0].Add(playerList[0][0]);
			teamUserList[1].Add(playerList[1][0]);
			list.Clear();
			list.Add(4);
			list.Add(5);
			int index = UnityEngine.Random.Range(0, list.Count);
			teamUserList[0].Add(list[index]);
			list.RemoveAt(index);
			teamUserList[1].Add(list[0]);
			break;
		}
		case 3:
		{
			list.Add(0);
			list.Add(1);
			list.Add(2);
			int index = UnityEngine.Random.Range(0, list.Count);
			playerList[0].Add(list[index]);
			teamUserList[0].Add(list[index]);
			list.RemoveAt(index);
			index = UnityEngine.Random.Range(0, list.Count);
			playerList[1].Add(list[index]);
			teamUserList[1].Add(list[index]);
			list.RemoveAt(index);
			int num = UnityEngine.Random.Range(0, 2);
			playerList[num].Add(list[0]);
			teamUserList[num].Add(list[0]);
			teamUserList[1 - num].Add(4);
			break;
		}
		case 4:
			list.Add(0);
			list.Add(1);
			list.Add(2);
			list.Add(3);
			for (int j = 0; j < 4; j++)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				playerList[(j < 2) ? j : (j - 2)].Add(list[index]);
				teamUserList[(j < 2) ? j : (j - 2)].Add(list[index]);
				list.RemoveAt(index);
			}
			break;
		}
		UnityEngine.Debug.Log("playerList[0].Count :" + playerList[0].Count.ToString());
		UnityEngine.Debug.Log("playerList[1].Count :" + playerList[1].Count.ToString());
		for (int k = 0; k < playerList.Length; k++)
		{
			for (int l = 0; l < playerList[k].Count; l++)
			{
				UnityEngine.Debug.Log("playerList[" + k.ToString() + "][" + l.ToString() + "] :" + playerList[k][l].ToString());
			}
		}
		teamList = new CharacterList[2];
		zoneCharaList = new BeachVolley_Character[BeachVolley_Define.Return_team_infield_num() / 2];
		cpuAi.Init();
		for (int m = 0; m < teamList.Length; m++)
		{
			teamList[m].charas = new BeachVolley_Character[BeachVolley_Define.TEAM_MEMBER_NUM_MAX];
			for (int n = 0; n < teamList[m].charas.Length; n++)
			{
				teamList[m].charas[n] = UnityEngine.Object.Instantiate(charaPrefab, base.transform.position, Quaternion.identity, BeachVolley_Define.FM.GetTeamAnchor(m));
				if (n < BeachVolley_Define.Return_team_infield_num())
				{
					teamList[m].charas[n].Init(m, n, n, BeachVolley_Define.FM.GetFormationAnchor(m, n), positionTypeTable[n], (BeachVolley_Define.PositionType)n, teamUserList[m][n]);
				}
				else
				{
					teamList[m].charas[n].Init(m, n, n, BeachVolley_Define.FM.GetBenchAnchor(m), BeachVolley_Character.PositionState.BENCH, (BeachVolley_Define.PositionType)n, 0);
				}
			}
			ExchangeCharaPosition(m, 5, 6);
			teamList[m].charas[5].ShowCharacter();
			teamList[m].charas[6].ShowCharacter(_show: false);
			teamList[m].charas[5].transform.position = teamList[m].charas[5].GetFormationPos();
			teamList[m].charas[6].transform.position = teamList[m].charas[6].GetFormationPos();
		}
		metaAi.Init();
		ballTouchCnt = 0;
		checkOutOfBoundsCnt = -1;
	}
	public int GetPlayerTeam(int _playerId)
	{
		if (playerList[0].IndexOf(_playerId) < 0)
		{
			if (playerList[1].IndexOf(_playerId) < 0)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}
	public void RotatePlayer(int _team)
	{
		nextChangeNo[_team]++;
		if (nextChangeNo[_team] >= playerList[_team].Count)
		{
			nextChangeNo[_team] = 0;
		}
	}
	public int GetNextPlayer(int _team)
	{
		return playerList[_team][nextChangeNo[_team]];
	}
	public int GetNextNo(int _team)
	{
		return nextChangeNo[_team];
	}
	public void SettingGameStart()
	{
		if (IsGameWatchingMode())
		{
			playerList[0].Add(0);
			ChangeControlChara(0, 0, teamList[0].charas[0]);
			playerList[1].Add(1);
			ChangeControlChara(1, 1, teamList[1].charas[0]);
		}
		else if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			for (int i = 0; i < playerList[0].Count; i++)
			{
				ChangeControlChara(0, playerList[0][i], teamList[0].charas[i]);
				cursor[i].SetPosition(teamList[0].charas[i]);
			}
			playerList[1].Clear();
			playerList[1].Add(1);
			ChangeControlChara(1, 1, teamList[1].charas[0]);
		}
		else
		{
			int num = 0;
			for (int j = 0; j < playerList[0].Count; j++)
			{
				ChangeControlChara(0, playerList[0][j], teamList[0].charas[j]);
				num++;
			}
			for (int k = 0; k < playerList[1].Count; k++)
			{
				ChangeControlChara(1, playerList[1][k], teamList[1].charas[k]);
				num++;
			}
			UnityEngine.Debug.Log("playerList[0].Count:" + playerList[0].Count.ToString() + " aaa " + playerList[1].Count.ToString());
			if (playerList[0].Count == 0)
			{
				playerList[0].Add(num);
				ChangeControlChara(0, num, teamList[0].charas[0]);
			}
			else if (playerList[1].Count == 0)
			{
				playerList[1].Add(num);
				ChangeControlChara(1, num, teamList[1].charas[0]);
			}
		}
		UnityEngine.Debug.Log("6人目オフに入った");
		teamList[0].charas[5].gameObject.SetActive(value: false);
		teamList[1].charas[5].gameObject.SetActive(value: false);
		InitRotateAutoPlay();
	}
	public BeachVolley_Character SettingServe(int _teamNo)
	{
		ResetHaveBallChara();
		int num = 0;
		teamList[_teamNo].charas[num].ServeStandby();
		int nextPlayer = GetNextPlayer(_teamNo);
		if (playerList[_teamNo].Count == 2)
		{
			UnityEngine.Debug.Log("キャラ変更しない");
		}
		else
		{
			ChangeControlChara(_teamNo, nextPlayer, teamList[_teamNo].charas[num], _forcibly: true, _isParentFlag: true);
			RotatePlayer(_teamNo);
		}
		List<BeachVolley_Character> list = BeachVolley_Define.MCM.TeamCharacterList(_teamNo, 7);
		list.Remove(teamList[_teamNo].charas[BeachVolley_Define.TEAM_MEMBER_NUM_MAX - 1]);
		list.Remove(teamList[_teamNo].charas[num]);
		if (!BeachVolley_Define.MGM.CheckTutorialServe())
		{
			BeachVolley_Define.BM.MoveServePos(_teamNo);
		}
		for (int i = 0; i < teamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				if (teamList[i].charas[j] != teamList[_teamNo].charas[num])
				{
					teamList[i].charas[j].SetActionState(BeachVolley_Character.ActionState.STANDARD);
				}
			}
		}
		if (!BeachVolley_Define.MGM.CheckTutorialServe())
		{
			BeachVolley_Define.BM.SetBallState(BeachVolley_BallManager.BallState.SERVE);
			BeachVolley_Define.Ball.IsServeBall = true;
			int num3 = checkOutOfBoundsCnt = (BallControllTeam = _teamNo);
			BeachVolley_Define.GUM.HideLimitPassCnt(BallControllTeam);
		}
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			BeachVolley_Define.GUM.SetLimitPassCnt(1 - BallControllTeam, BALL_TOUCH_LIMIT);
		}
		return teamList[_teamNo].charas[0];
	}
	public void TeamRotation(int _teamNo)
	{
		if (teamList[_teamNo].charas[GetBenchCharaIndex() - 1].CheckPositionType(BeachVolley_Define.PositionType.LIBERO))
		{
			ChangeLibero(_teamNo, 5);
		}
		else
		{
			ChangeLibero(_teamNo, 0);
		}
		teamList[_teamNo].charas[GetBenchCharaIndex()].SettingReturnBench(BeachVolley_Define.FM.GetBenchPos(_teamNo, BeachVolley_Define.MGM.SetNo));
		int num = 0;
		for (int i = 0; i < GetBenchCharaIndex() - 1; i++)
		{
			num = (i + 1) % BeachVolley_Define.Return_team_infield_num();
			ExchangeCharaPosition(_teamNo, i, num);
		}
	}
	public void TeamRotationBeach(int _teamNo)
	{
		int num = 0;
		for (int i = 0; i < GetBenchCharaIndex() - 1; i++)
		{
			num = (i + 1) % BeachVolley_Define.Return_team_infield_num();
			ExchangeCharaPosition(_teamNo, i, num);
		}
	}
	public void ChangeLibero(int _teamNo, int _changeCharaNo)
	{
		ExchangeCharaPosition(_teamNo, _changeCharaNo, GetBenchCharaIndex());
		teamList[_teamNo].charas[_changeCharaNo].ShowCharacter();
		teamList[_teamNo].charas[_changeCharaNo].SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
		teamList[_teamNo].charas[GetBenchCharaIndex()].SettingReturnBench(BeachVolley_Define.FM.GetBenchPos(_teamNo, BeachVolley_Define.MGM.SetNo));
	}
	public void ResetRotation(int _teamNo, int _setNo)
	{
		BeachVolley_Character beachVolley_Character = null;
		for (int i = 0; i <= BeachVolley_Define.Return_team_infield_num(); i++)
		{
			for (int j = 0; j <= BeachVolley_Define.Return_team_infield_num(); j++)
			{
				if (i == teamList[_teamNo].charas[j].FormationNo)
				{
					beachVolley_Character = teamList[_teamNo].charas[j];
					teamList[_teamNo].charas[j] = teamList[_teamNo].charas[i];
					break;
				}
			}
			teamList[_teamNo].charas[i] = beachVolley_Character;
			teamList[_teamNo].charas[i].SetFormationAnchor((i < BeachVolley_Define.Return_team_infield_num()) ? BeachVolley_Define.FM.GetFormationAnchor(_teamNo, i) : BeachVolley_Define.FM.GetBenchAnchor(_teamNo));
			teamList[_teamNo].charas[i].SetPositionState((i < BeachVolley_Define.Return_team_infield_num()) ? positionTypeTable[i] : BeachVolley_Character.PositionState.BENCH);
			teamList[_teamNo].charas[i].SetPositionType((BeachVolley_Define.PositionType)i);
		}
	}
	private void ExchangeCharaPosition(int _teamNo, int _dataOneIndex, int _dataTwoIndex)
	{
		rotationChangeData.charaTemp = teamList[_teamNo].charas[_dataOneIndex];
		rotationChangeData.formationAnchor = teamList[_teamNo].charas[_dataTwoIndex].GetFormationAnchor();
		rotationChangeData.positionState = teamList[_teamNo].charas[_dataTwoIndex].GetPositionState();
		rotationChangeData.positionType = teamList[_teamNo].charas[_dataTwoIndex].GetPositionType();
		teamList[_teamNo].charas[_dataTwoIndex].SetFormationAnchor(teamList[_teamNo].charas[_dataOneIndex].GetFormationAnchor());
		teamList[_teamNo].charas[_dataTwoIndex].SetPositionState(teamList[_teamNo].charas[_dataOneIndex].GetPositionState());
		if (!teamList[_teamNo].charas[_dataTwoIndex].CheckPositionType(BeachVolley_Define.PositionType.LIBERO) && _dataOneIndex != 6)
		{
			teamList[_teamNo].charas[_dataTwoIndex].SetPositionType((BeachVolley_Define.PositionType)_dataOneIndex);
		}
		teamList[_teamNo].charas[_dataOneIndex] = teamList[_teamNo].charas[_dataTwoIndex];
		rotationChangeData.charaTemp.SetFormationAnchor(rotationChangeData.formationAnchor);
		rotationChangeData.charaTemp.SetPositionState(rotationChangeData.positionState);
		if (!rotationChangeData.charaTemp.CheckPositionType(BeachVolley_Define.PositionType.LIBERO) && _dataTwoIndex != 6)
		{
			rotationChangeData.charaTemp.SetPositionType((BeachVolley_Define.PositionType)_dataTwoIndex);
		}
		teamList[_teamNo].charas[_dataTwoIndex] = rotationChangeData.charaTemp;
	}
	public bool CheckCharacterNotMoveLists()
	{
		bool result = true;
		for (int i = 0; i < teamList.Length; i++)
		{
			if (teamList[0].charas[i].GetMoveListCount() > 0)
			{
				result = false;
			}
			if (teamList[1].charas[i].GetMoveListCount() > 0)
			{
				result = false;
			}
		}
		return result;
	}
	public void AutoPlayChangeControlleChara(BeachVolley_Character _actionChara)
	{
		if (!AUTOPLAYFlag)
		{
			return;
		}
		if (BeachVolley_Define.BM.GetBallDropPrediPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
		{
			if (BallControllTeam == 0)
			{
				BeachVolley_Define.BM.ResetBallControlTime();
			}
			BallControllTeam = 1;
		}
		else
		{
			if (BallControllTeam == 1)
			{
				BeachVolley_Define.BM.ResetBallControlTime();
			}
			BallControllTeam = 0;
		}
		for (int i = 0; i < 2; i++)
		{
			int num = NextPlayerAutoPlay(i, _actionChara.playerNo);
			if (i == BallControllTeam)
			{
				BeachVolley_Character chara = SearchCharaHandleBall(i, BeachVolley_Define.BM.GetBallDropPrediPosGround(), _actionChara, num);
				ChangeControlChara(i, num, chara);
				RotatePlayer(i);
				continue;
			}
			List<int> list = new List<int>();
			switch (playerList[i].Count)
			{
			case 1:
				ChangeControlChara(i, num, SearchBlockChara(i, num));
				break;
			case 2:
				for (int k = 0; k < playerList[i].Count; k++)
				{
					if (playerList[i][k] != num)
					{
						list.Add(playerList[i][k]);
					}
				}
				ChangeControlChara(i, num, SearchBlockChara(i, num));
				ChangeControlChara(i, list[0], SearchReceiveChara(i, list[0]));
				break;
			case 3:
				for (int j = 0; j < playerList[i].Count; j++)
				{
					if (playerList[i][j] != num)
					{
						list.Add(playerList[i][j]);
					}
				}
				ChangeControlChara(i, num, SearchBlockChara(i, num));
				ChangeControlChara(i, list[0], SearchReceiveChara(i, list[0]));
				ChangeControlChara(i, list[1], SearchReceiveChara(i, list[1]));
				break;
			case 4:
				ChangeControlChara(i, rotateAutoPlayer4Front[0], SearchBlockChara(i, rotateAutoPlayer4Front[0]));
				ChangeControlChara(i, rotateAutoPlayer4Front[1], SearchBlockChara(i, rotateAutoPlayer4Front[1]));
				ChangeControlChara(i, rotateAutoPlayer4Back[0], SearchReceiveChara(i, rotateAutoPlayer4Back[0]));
				ChangeControlChara(i, rotateAutoPlayer4Back[1], SearchReceiveChara(i, rotateAutoPlayer4Back[1]));
				break;
			}
			RotatePlayer(i);
		}
	}
	public void UpdateMethod()
	{
		if (BeachVolley_Define.MGM.CheckInPlay() && BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
		{
			BeachVolley_Define.BM.UpdateDropPrediPos();
			BeachVolley_Define.BM.ShowDropPrediPos(BeachVolley_Define.FM.CheckOnDesk(BeachVolley_Define.BM.GetBallDropPrediPosGround()));
		}
		else
		{
			BeachVolley_Define.BM.ShowDropPrediPos(_show: false);
		}
		metaAi.UpdateMethod();
		if (BeachVolley_Define.BM.GetBallDropPrediPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
		{
			if (BallControllTeam == 0)
			{
				BeachVolley_Define.BM.ResetBallControlTime();
			}
			BallControllTeam = 1;
		}
		else
		{
			if (BallControllTeam == 1)
			{
				BeachVolley_Define.BM.ResetBallControlTime();
			}
			BallControllTeam = 0;
		}
		if (!(BeachVolley_Define.Ball.GetLastHitChara() == null))
		{
			if (BeachVolley_Define.BM.GetBallPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
			{
				if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo == 0)
				{
					ballTouchCnt = 0;
					BeachVolley_Define.GUM.SetLimitPassCnt(BallControllTeam, BALL_TOUCH_LIMIT);
				}
				if (checkOutOfBoundsCnt == 0)
				{
					if (!BeachVolley_Define.FM.CheckAntennaInsideArea(BeachVolley_Define.BM.GetBallPos(_offset: false)))
					{
						BeachVolley_Define.Ball.IsPassingOutAntenna = true;
					}
					else
					{
						BeachVolley_Define.Ball.IsPassingOutAntenna = false;
					}
					checkOutOfBoundsCnt = 1;
				}
			}
			else
			{
				if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo == 1)
				{
					ballTouchCnt = 0;
					BeachVolley_Define.GUM.SetLimitPassCnt(BallControllTeam, BALL_TOUCH_LIMIT);
				}
				if (checkOutOfBoundsCnt == 1)
				{
					if (!BeachVolley_Define.FM.CheckAntennaInsideArea(BeachVolley_Define.BM.GetBallPos(_offset: false)))
					{
						BeachVolley_Define.Ball.IsPassingOutAntenna = true;
					}
					else
					{
						BeachVolley_Define.Ball.IsPassingOutAntenna = false;
					}
					checkOutOfBoundsCnt = 0;
				}
			}
		}
		ShowPassCircle(_show: false);
		for (int i = 0; i < playerList.Length; i++)
		{
			int num = i;
			int nextPlayer = GetNextPlayer(num);
			if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE) && BeachVolley_Define.Ball.GetLastHitChara() != null && playerList[num].Count != 2 && !AUTOPLAYFlag)
			{
				int[] array = playerList[0].ToArray();
				int[] array2 = playerList[1].ToArray();
				int[] array3 = new int[2]
				{
					array.Length,
					array2.Length
				};
				if (num == BallControllTeam)
				{
					if (ChangeControlChara(num, nextPlayer, SearchCharaHandleBall(num, BeachVolley_Define.BM.GetBallDropPrediPosGround(), BeachVolley_Define.Ball.GetLastHitChara(), nextPlayer)))
					{
						RotatePlayer(num);
					}
					else if (array3[0] == 1)
					{
						RotatePlayer(num);
					}
				}
				else if (ChangeControlChara(num, nextPlayer, SearchBlockChara(num, nextPlayer)))
				{
					RotatePlayer(num);
				}
				else if (array3[1] == 1)
				{
					RotatePlayer(num);
				}
			}
			for (int j = 0; j < playerList[i].Count; j++)
			{
				if (controlChara[playerList[i][j]] != null && controlChara[playerList[i][j]].IsPlayer)
				{
					UpdateCursorDir(controlChara[playerList[i][j]].transform.forward, playerList[i][j]);
				}
			}
			controllChara[num] = SearchCharaAttackStay(num, BeachVolley_Define.BM.GetBallDropPrediPosGround(), BeachVolley_Define.Ball.GetLastHitChara(), nextPlayer);
			for (int k = 0; k < teamList[num].charas.Length; k++)
			{
				if (controllChara[num] == teamList[num].charas[k])
				{
					teamList[num].charas[k].SetAttackStayFlg(_value: true);
				}
				else
				{
					teamList[num].charas[k].SetAttackStayFlg(_value: false);
				}
				if (BeachVolley_Define.MGM.IsStopChara())
				{
					teamList[num].charas[k].AddGravity();
				}
				else
				{
					int playerNo = teamList[num].charas[k].playerNo;
					cpuAi.UpdateAutoAction(num, k);
					metaAi.UpdateRestrictedAreaData();
					bool flag = false;
					if (playerNo >= 0 && teamList[num].charas[k].IsPlayer)
					{
						flag = true;
					}
					if (BeachVolley_Define.Ball.GetLastHitChara() != null)
					{
						int teamNo = BeachVolley_Define.Ball.GetLastHitChara().TeamNo;
					}
					bool flag2 = false;
					if ((BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE || BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE_STANDBY) && BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.KEEP) && teamList[num].charas[k].GetActionState() == BeachVolley_Character.ActionState.STANDARD)
					{
						flag2 = true;
					}
					if ((!BeachVolley_Define.MGM.IsAutoAction() | flag2) && flag)
					{
						PlayerOperation(teamList[num].charas[k], playerNo, flag2);
					}
					else
					{
						teamList[num].charas[k].AiAction();
					}
				}
				teamList[num].charas[k].UpdateMethod();
			}
		}
		for (int l = 0; l < cursor.Length; l++)
		{
			if ((bool)controlChara[l])
			{
				cursor[l].SetArrowPos(_haveBall: true);
				cursor[l].ShowCircle(_show: true);
				cursor[l].ShowCircleAlpha(_haveBall: true);
			}
			cursor[l].UpdateMethod();
		}
		for (int m = 0; m < passCircle.Length; m++)
		{
			passCircle[m].UpdateMethod();
		}
		for (int n = 0; n < controlChangeInterval.Length; n++)
		{
			controlChangeInterval[n] -= Time.deltaTime;
		}
		SingletonCustom<BeachVolley_CharacterNameManager>.Instance.UpdateNameObj();
	}
	public void ChangeCursor(int _playerNo, bool _isParentFlag = false)
	{
		controlChangeInterval[_playerNo] = 0.5f;
		cursor[_playerNo].SetCharacter(controlChara[_playerNo], _isParentFlag);
	}
	public void ShowCursor(bool _show)
	{
		for (int i = 0; i < cursor.Length; i++)
		{
			if ((bool)cursor[i].GetChara())
			{
				cursor[i].Show(_show && cursor[i].GetChara().IsPlayer);
			}
		}
	}
	public void UpdateCursorDir(Vector3 _dirVec, int _playerNo)
	{
		_dirVec.y = 0f;
		cursor[_playerNo].SetCursorDir(_dirVec);
	}
	public void ShowPassCircle(bool _show, int _teamNo = -1)
	{
		if (_teamNo == -1)
		{
			for (int i = 0; i < passCircle.Length; i++)
			{
				passCircle[i].gameObject.SetActive(_show);
			}
		}
		else
		{
			passCircle[_teamNo].gameObject.SetActive(_show);
		}
	}
	public void ResetHaveBallChara()
	{
		if (haveBallChara != null)
		{
			cursor[haveBallChara.playerNo].ResetGauge();
		}
		haveBallCharaPrev = null;
		haveBallChara = null;
	}
	public void HaveBallCharaBallRelease()
	{
		if (haveBallChara != null)
		{
			cursor[haveBallChara.TeamNo].ResetGauge();
			haveBallCharaPrev = haveBallChara;
		}
		haveBallChara = null;
	}
	private void ResetTeamCharaPlayer(int _teamNo, int _playerNo)
	{
		for (int i = 0; i < teamList[_teamNo].charas.Length; i++)
		{
			if (teamList[_teamNo].charas[i].playerNo == _playerNo)
			{
				teamList[_teamNo].charas[i].playerNo = -1;
			}
		}
	}
	public bool ChangeControlChara(int _teamNo, int _playerNo = 0, BeachVolley_Character _chara = null, bool _forcibly = false, bool _isParentFlag = false)
	{
		UnityEngine.Debug.Log("操作キャラ変更処理：" + _playerNo.ToString());
		_teamNo = BeachVolley_Define.MCM.GetPlayerTeam(_playerNo);
		if ((bool)_chara)
		{
			if (!_forcibly && _chara.playerNo >= 0)
			{
				return false;
			}
			if (controlChara[_playerNo] != null && _chara == controlChara[_playerNo])
			{
				return false;
			}
			_chara.SetChangeCharaMoveTime(0.5f);
			controlChara[_playerNo] = _chara;
			ResetTeamCharaPlayer(_teamNo, _playerNo);
			UnityEngine.Debug.Log("_teamNo:" + _teamNo.ToString());
			UnityEngine.Debug.Log("_playerNo:" + _playerNo.ToString());
			controlChara[_playerNo].playerNo = _playerNo;
			cursor[_playerNo].ResetGauge();
			ChangeCursor(_playerNo, _isParentFlag);
			if (BeachVolley_Define.Ball.GetLastHitChara() != null)
			{
				controlChara[_playerNo].ResetControlInterval();
			}
			if (BeachVolley_Define.Ball.GetLastHitChara() != null)
			{
				if (IsOptionalControl)
				{
					controlChara[_playerNo].ResetMoveInterval(0.1f);
				}
				else if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo != _teamNo)
				{
					controlChara[_playerNo].ResetMoveInterval();
				}
			}
			return true;
		}
		return false;
	}
	public void GameStart()
	{
		UnityEngine.Debug.Log("試合開始");
		ChangeActionStateAllChara(BeachVolley_Character.ActionState.STANDARD);
		BeachVolley_Define.BM.SetBallState(BeachVolley_BallManager.BallState.FREE);
		BeachVolley_Define.MGM.ChangeStateInPlay(_resumeTime: true);
	}
	public void ChangeActionStateAllChara(BeachVolley_Character.ActionState _state, int _teamNo = -1)
	{
		if (_teamNo != -1)
		{
			for (int i = 0; i < teamList[_teamNo].charas.Length; i++)
			{
				teamList[_teamNo].charas[i].SetActionState(_state);
			}
			return;
		}
		for (int j = 0; j < teamList.Length; j++)
		{
			for (int k = 0; k < BeachVolley_Define.Return_team_infield_num(); k++)
			{
				teamList[j].charas[k].SetActionState(_state);
			}
		}
	}
	private void PlayerOperation(BeachVolley_Character _chara, int _playerNo, bool notcheckAutoFlag = false)
	{
		int playerTeam = BeachVolley_Define.MCM.GetPlayerTeam(_playerNo);
		if (BeachVolley_Define.CM.GetTapTime(_playerNo) > 0f)
		{
			cursor[_playerNo].SetGauge(BeachVolley_Define.GAUGE_POWER);
		}
		else
		{
			cursor[_playerNo].ResetGauge();
		}
		if (_chara.CheckActionState(BeachVolley_Character.ActionState.SERVE_WAIT))
		{
			if (BeachVolley_Define.GUM.IsTimeLimitFinish(_playerNo))
			{
				ServeToss(_chara, 0f);
			}
			else
			{
				CharaMove(_playerNo);
			}
			UpdateCursorDir(_chara.transform.forward, _playerNo);
			if (BeachVolley_Define.CM.IsTap(_playerNo) >= 0 && !BeachVolley_Define.MGM.CheckTutorialServeMove())
			{
				ServeToss(_chara, cursor[_playerNo].GetGaugeValue());
				BeachVolley_Define.CM.ResetTapData(_playerNo);
			}
		}
		else
		{
			if (_chara.CheckActionState(BeachVolley_Character.ActionState.TOSS) || _chara.CheckActionState(BeachVolley_Character.ActionState.DIVING_TOSS))
			{
				return;
			}
			if (_chara.CheckActionState(BeachVolley_Character.ActionState.SPIKE) || _chara.CheckActionState(BeachVolley_Character.ActionState.DIVING_ATTACK))
			{
				Vector3 a = BeachVolley_Define.BM.CalcSpikeTargetPos(_chara, cursor[_playerNo].GetGaugeValue(), BeachVolley_Define.CM.GetMoveDir(_playerNo) * BeachVolley_Define.CM.GetMoveLength(_playerNo));
				UpdateCursorDir((a - _chara.GetPos()).normalized, _playerNo);
			}
			else if (_chara.CheckActionState(BeachVolley_Character.ActionState.STANDARD))
			{
				if (BeachVolley_Define.MGM.CheckInPlay() && (!AUTOPLAYFlag | notcheckAutoFlag) && controlChara[_playerNo].GetMoveInterval() <= 0f)
				{
					Vector3 ballDropPrediPosGround = SingletonCustom<BeachVolley_BallManager>.Instance.GetBallDropPrediPosGround();
					Vector3 pos = GetControlChara(_playerNo).GetPos();
					float num = Vector3.Distance(ballDropPrediPosGround, pos);
					if (BeachVolley_Define.CM.IsMove(_playerNo))
					{
						float num2 = Vector3.Distance(ballDropPrediPosGround, pos + BeachVolley_Define.CM.GetMoveDir(_playerNo) * 0.1f);
						bool flag = true;
						if (BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE || BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE_STANDBY)
						{
							flag = false;
						}
						if (((BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPos()) || ballTouchCnt != 0) && num < 5f && num2 > ballCharacterDistance[_playerNo]) & flag)
						{
							CharaMove(_playerNo, num / 5f * 1.1f);
						}
						else
						{
							CharaMove(_playerNo, 1.1f);
						}
					}
					ballCharacterDistance[_playerNo] = num;
				}
				int num3 = -1;
				num3 = ((BeachVolley_Define.BM.GetBallPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z) ? 1 : 0);
				if (!BeachVolley_Define.MGM.CheckInPlay() || (BeachVolley_Define.Ball.IsServeBall && BallControllTeam != playerTeam))
				{
					return;
				}
				if (BallControllTeam == playerTeam || num3 == playerTeam)
				{
					if (AUTOPLAYFlag && !notcheckAutoFlag)
					{
						if (CheckCanMove(_playerNo, playerTeam))
						{
							if (_chara.CheckMoveBallDropPorediPos())
							{
								_chara.MoveBallDropPorediPos();
							}
						}
						else
						{
							_chara.MoveFormationPos();
						}
					}
					bool flag2 = false;
					if (CheckLastTouch())
					{
						flag2 = true;
						int num4 = BeachVolley_Define.CM.IsTap(_playerNo);
						if ((num4 == 1 || num4 == 0) && _chara.GetControlInterval() <= 0f)
						{
							if (_chara.CheckDivingAttack())
							{
								_chara.DivingAttack(cursor[_playerNo].GetGaugeValue());
							}
							else
							{
								_chara.AttackAction(cursor[_playerNo].GetGaugeValue());
							}
							BeachVolley_Define.CM.ResetTapData(_playerNo);
						}
					}
					else
					{
						int num5 = BeachVolley_Define.CM.IsTap(_playerNo);
						if (BeachVolley_Define.Ball.GetLastHitChara() != null && num5 == 0 && (BeachVolley_Define.Ball.GetLastHitChara().TeamNo != playerTeam || _chara.GetControlInterval() <= 0f))
						{
							_chara.TossAction(cursor[_playerNo].GetGaugeValue());
							BeachVolley_Define.CM.ResetTapData(_playerNo);
						}
						else if (num5 == 1)
						{
							UnityEngine.Debug.Log("アタックした:" + cursor[_playerNo].GetGaugeValue().ToString());
							_chara.AttackAction(cursor[_playerNo].GetGaugeValue());
							BeachVolley_Define.CM.ResetTapData(_playerNo);
						}
					}
					if (flag2)
					{
						Vector3 a2 = BeachVolley_Define.BM.CalcSpikeTargetPos(_chara, cursor[_playerNo].GetGaugeValue(), BeachVolley_Define.CM.GetMoveDir(_playerNo) * BeachVolley_Define.CM.GetMoveLength(_playerNo));
						UpdateCursorDir((a2 - _chara.GetPos()).normalized, _playerNo);
					}
					return;
				}
				Vector3 vector = BeachVolley_Define.CM.GetMoveDir(_playerNo) * BeachVolley_Define.CM.GetMoveLength(_playerNo);
				vector.y = 0f;
				vector.z = Mathf.Abs(vector.z) * _chara.transform.forward.z;
				UpdateCursorDir(vector, _playerNo);
				if (AUTOPLAYFlag)
				{
					if (CheckCanMove(_playerNo, playerTeam, isFront: true))
					{
						_chara.MoveBlockingPos();
					}
					else
					{
						_chara.MoveFormationPos();
					}
				}
				int num6 = BeachVolley_Define.CM.IsTap(_playerNo);
				if (num6 != 1 && num6 != 0)
				{
					return;
				}
				if (GetBallControll() == playerTeam)
				{
					if (_chara.CheckDivingAttack())
					{
						UnityEngine.Debug.Log("とおった２");
						_chara.DivingAttack(cursor[_playerNo].GetGaugeValue());
					}
					else
					{
						_chara.AttackAction(cursor[_playerNo].GetGaugeValue());
					}
				}
				else
				{
					_chara.JumpAction(cursor[_playerNo].GetGaugeValue(), _playerNo);
				}
				BeachVolley_Define.CM.ResetTapData(_playerNo);
			}
			else if (_chara.CheckActionState(BeachVolley_Character.ActionState.JUMP))
			{
				Vector3 vector2 = BeachVolley_Define.CM.GetMoveDir(_playerNo) * BeachVolley_Define.CM.GetMoveLength(_playerNo);
				vector2.y = 0f;
				vector2.z = Mathf.Abs(vector2.z) * _chara.transform.forward.z;
				UpdateCursorDir(vector2, _playerNo);
			}
		}
	}
	private int GetBallControll()
	{
		if (BeachVolley_Define.BM.GetBallPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
		{
			return 1;
		}
		return 0;
	}
	public bool CheckCanMove(int _playerNo, int _teamNo, bool isFront = false)
	{
		if (rotatePlayer4 == _teamNo)
		{
			if ((BallTouchCnt == 0) | isFront)
			{
				int num = -1;
				float num2 = 100f;
				int num3 = -1;
				for (int i = 0; i < playerList[_teamNo].Count; i++)
				{
					float num4 = Vector3.Distance(controlChara[playerList[_teamNo][i]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround());
					if (num2 > num4)
					{
						num2 = num4;
						num3 = (num = playerList[_teamNo][i]);
					}
				}
				if (controlChara[num] == BeachVolley_Define.Ball.GetLastHitChara())
				{
					num2 = 100f;
					num = -1;
					for (int j = 0; j < playerList[_teamNo].Count; j++)
					{
						if (playerList[_teamNo][j] != num3)
						{
							float num5 = Vector3.Distance(controlChara[playerList[_teamNo][j]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround());
							if (num2 > num5)
							{
								num2 = num5;
								num = playerList[_teamNo][j];
							}
						}
					}
				}
				if (isFront)
				{
					rotatePlayerNow4FB[0] = num;
				}
				else
				{
					rotatePlayerNow4 = num;
				}
			}
			if (isFront)
			{
				return rotatePlayerNow4FB[0] == _playerNo;
			}
			return rotatePlayerNow4 == _playerNo;
		}
		if (playerList[_teamNo].Count == 2 && BallTouchCnt == 0)
		{
			List<int> list = new List<int>();
			for (int k = 0; k < playerList[_teamNo].Count; k++)
			{
				list.Add(playerList[_teamNo][k]);
			}
			if (Vector3.Distance(controlChara[list[0]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()) < Vector3.Distance(controlChara[list[1]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()))
			{
				rotatePlayerNow23[_teamNo] = list[0];
				if (controlChara[rotatePlayerNow23[_teamNo]] == BeachVolley_Define.Ball.GetLastHitChara())
				{
					rotatePlayerNow23[_teamNo] = list[1];
				}
			}
			else
			{
				rotatePlayerNow23[_teamNo] = list[1];
				if (controlChara[rotatePlayerNow23[_teamNo]] == BeachVolley_Define.Ball.GetLastHitChara())
				{
					rotatePlayerNow23[_teamNo] = list[0];
				}
			}
		}
		if (playerList[_teamNo].Count == 3 && BallTouchCnt == 0)
		{
			int num6 = -1;
			float num7 = 100f;
			int num8 = -1;
			for (int l = 0; l < playerList[_teamNo].Count; l++)
			{
				float num9 = Vector3.Distance(controlChara[playerList[_teamNo][l]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround());
				if (num7 > num9)
				{
					num7 = num9;
					num8 = (num6 = playerList[_teamNo][l]);
				}
			}
			if (controlChara[num6] == BeachVolley_Define.Ball.GetLastHitChara())
			{
				num7 = 100f;
				num6 = -1;
				for (int m = 0; m < playerList[_teamNo].Count; m++)
				{
					if (playerList[_teamNo][m] != num8)
					{
						float num10 = Vector3.Distance(controlChara[playerList[_teamNo][m]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround());
						if (num7 > num10)
						{
							num7 = num10;
							num6 = playerList[_teamNo][m];
						}
					}
				}
			}
			rotatePlayerNow23[_teamNo] = num6;
		}
		return rotatePlayerNow23[_teamNo] == _playerNo;
	}
	private void InitRotateAutoPlay()
	{
		rotatePlayer4 = -1;
		if (playerList[0].Count == 4 || playerList[1].Count == 4)
		{
			if (playerList[0].Count == 4)
			{
				rotatePlayer4 = 0;
			}
			else
			{
				rotatePlayer4 = 1;
			}
			for (int i = 0; i < rotateAutoPlayer4Front.Length; i++)
			{
				rotateAutoPlayer4Front[i] = i;
			}
			for (int j = 0; j < rotateAutoPlayer4Back.Length; j++)
			{
				rotateAutoPlayer4Back[j] = j + rotateAutoPlayer4Front.Length;
			}
		}
		else
		{
			UnityEngine.Debug.Log("playerList[0].Count:" + playerList[0].Count.ToString());
			UnityEngine.Debug.Log("playerList[1].Count:" + playerList[1].Count.ToString());
			rotateAutoPlayTeam1 = new int[playerList[0].Count];
			rotateAutoPlayTeam2 = new int[playerList[1].Count];
			for (int k = 0; k < rotateAutoPlayTeam1.Length; k++)
			{
				rotateAutoPlayTeam1[k] = playerList[0][k];
			}
			for (int l = 0; l < rotateAutoPlayTeam2.Length; l++)
			{
				rotateAutoPlayTeam2[l] = playerList[1][l];
			}
		}
		rotatePlayerNow23[0] = playerList[0][0];
		rotatePlayerNow23[1] = playerList[1][0];
	}
	private int NextPlayerAutoPlay(int _teamNo, int _playerNo = -1)
	{
		if (rotatePlayer4 == _teamNo)
		{
			int num = 0;
			bool flag;
			if (rotateAutoPlayer4Front[0] == rotatePlayerNow4)
			{
				flag = true;
				num = 0;
			}
			else if (rotateAutoPlayer4Front[1] == rotatePlayerNow4)
			{
				flag = true;
				num = 1;
			}
			else
			{
				flag = false;
			}
			if (rotateAutoPlayer4Back[0] == rotatePlayerNow4)
			{
				num = 0;
			}
			else if (rotateAutoPlayer4Back[1] == rotatePlayerNow4)
			{
				num = 1;
			}
			Vector3 localPos = BeachVolley_Define.FM.ConvertLocalPos(BeachVolley_Define.BM.GetBallDropPrediPosGround(), _teamNo);
			if (BeachVolley_Define.FM.CheckInFrontZone(localPos, _teamNo))
			{
				if (flag)
				{
					rotatePlayerNow4FB[0] = 1 - num;
					rotatePlayerNow4 = rotateAutoPlayer4Front[rotatePlayerNow4FB[0]];
				}
				if (Vector3.Distance(controlChara[rotateAutoPlayer4Back[0]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()) < Vector3.Distance(controlChara[rotateAutoPlayer4Back[1]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()))
				{
					rotatePlayerNow4FB[1] = 0;
					rotatePlayerNow4 = rotateAutoPlayer4Back[rotatePlayerNow4FB[1]];
				}
				else
				{
					rotatePlayerNow4FB[1] = 1;
					rotatePlayerNow4 = rotateAutoPlayer4Back[rotatePlayerNow4FB[1]];
				}
			}
			else
			{
				if (!flag)
				{
					rotatePlayerNow4FB[1] = 1 - num;
					rotatePlayerNow4 = rotateAutoPlayer4Back[rotatePlayerNow4FB[1]];
				}
				if (Vector3.Distance(controlChara[rotateAutoPlayer4Front[0]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()) < Vector3.Distance(controlChara[rotateAutoPlayer4Front[1]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()))
				{
					rotatePlayerNow4FB[0] = 0;
					rotatePlayerNow4 = rotateAutoPlayer4Front[rotatePlayerNow4FB[0]];
				}
				else
				{
					rotatePlayerNow4FB[0] = 1;
					rotatePlayerNow4 = rotateAutoPlayer4Front[rotatePlayerNow4FB[0]];
				}
			}
			return rotatePlayerNow4;
		}
		if (_playerNo == rotatePlayerNow23[_teamNo])
		{
			if (playerList[_teamNo].Count == 2)
			{
				if (_teamNo == 0)
				{
					if (rotatePlayerNow23[_teamNo] == rotateAutoPlayTeam1[0])
					{
						rotatePlayerNow23[_teamNo] = rotateAutoPlayTeam1[1];
					}
					else
					{
						rotatePlayerNow23[_teamNo] = rotateAutoPlayTeam1[0];
					}
				}
				else if (rotatePlayerNow23[_teamNo] == rotateAutoPlayTeam2[0])
				{
					rotatePlayerNow23[_teamNo] = rotateAutoPlayTeam2[1];
				}
				else
				{
					rotatePlayerNow23[_teamNo] = rotateAutoPlayTeam2[0];
				}
			}
			else if (playerList[_teamNo].Count == 3)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < playerList[_teamNo].Count; i++)
				{
					if (rotatePlayerNow23[_teamNo] != playerList[_teamNo][i])
					{
						list.Add(playerList[_teamNo][i]);
					}
				}
				if (Vector3.Distance(controlChara[list[0]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()) < Vector3.Distance(controlChara[list[1]].GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()))
				{
					rotatePlayerNow23[_teamNo] = list[0];
				}
				else
				{
					rotatePlayerNow23[_teamNo] = list[1];
				}
			}
		}
		return rotatePlayerNow23[_teamNo];
	}
	public void RotateAutoPlayer4(int _teamNo)
	{
		if (rotatePlayer4 == _teamNo)
		{
			int num = rotateAutoPlayer4Front[0];
			rotateAutoPlayer4Front[0] = rotateAutoPlayer4Front[1];
			rotateAutoPlayer4Front[1] = rotateAutoPlayer4Back[0];
			rotateAutoPlayer4Back[0] = rotateAutoPlayer4Back[1];
			rotateAutoPlayer4Back[1] = num;
		}
	}
	public void CharacterStop()
	{
		for (int i = 0; i < teamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				teamList[i].charas[j].RigidStop();
			}
		}
	}
	private void CharaMove(int _no, float downSpeed = 1f)
	{
		if (controlChara[_no] == BeachVolley_Define.Ball.GetLastHitChara())
		{
			controlChara[_no].Move(BeachVolley_Define.CM.GetMoveDir(_no), BeachVolley_Define.CM.GetMoveLength(_no) * 1f);
		}
		else if (downSpeed > 0.01f)
		{
			controlChara[_no].Move(BeachVolley_Define.CM.GetMoveDir(_no), BeachVolley_Define.CM.GetMoveLength(_no) * downSpeed);
		}
		else
		{
			GetControlChara(_no).RigidZero();
		}
	}
	public void CatchBall(BeachVolley_Character _chara, int _teamNo)
	{
		SetHaveBallChara(_chara, _teamNo);
		BeachVolley_Define.BM.Catch(_chara);
	}
	public void SetHaveBallChara(BeachVolley_Character _chara, int _teamNo)
	{
		if (haveBallChara != null)
		{
			haveBallCharaPrev = haveBallChara;
		}
		haveBallChara = _chara;
	}
	public void ServeToss(BeachVolley_Character _actionChara, float _gaugeValue)
	{
		_gaugeValue = 0.7f;
		BeachVolley_Define.GUM.FinishTimeLimit(_actionChara.playerNo);
		BeachVolley_Define.MGM.SetAutoAction(_flg: true);
		SingletonCustom<AudioManager>.Instance.SePlay("se_shoot");
		if (BeachVolley_CharacterCursor.CheckToss(_gaugeValue))
		{
			_actionChara.StandServe(_gaugeValue);
			BeachVolley_Define.BM.ServeToss(_actionChara, CalcManager.mVector3Zero, _gaugeValue, _isStand: true);
		}
		else
		{
			_actionChara.JumpServe(_gaugeValue);
			BeachVolley_Define.BM.ServeToss(_actionChara, _actionChara.transform.forward, _gaugeValue, _isStand: false);
		}
		HaveBallCharaBallRelease();
		metaAi.UpdateData();
	}
	public void ServeShot(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _isStand)
	{
		BeachVolley_Define.Ball.transform.SetPositionY(_actionChara.GetPos().y + _actionChara.GetCharaHeight() + 0.5f);
		BeachVolley_Define.MGM.SetAutoAction(_flg: false);
		if (_isStand)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
			BeachVolley_Define.BM.StandServe(_actionChara, _dir);
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_beachvolley_spike", _loop: false, 0f, 0.5f);
			BeachVolley_Define.BM.JumpServe(_actionChara, _gaugeValue, _dir);
		}
		BallControllTeam = 1 - BallControllTeam;
		ResetHaveBallChara();
		BeachVolley_Define.BM.SetBallState(BeachVolley_BallManager.BallState.FREE);
		BeachVolley_Define.MGM.ChangeStateInPlay();
		metaAi.CalcDistanceFromBallDropPrediPos();
	}
	public void Toss(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _isOver)
	{
		if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo != _actionChara.TeamNo)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_beachvolley_toss");
		}
		BeachVolley_Define.BM.Toss(_actionChara, _gaugeValue, _dir, _isOver || BeachVolley_Define.Ball.GetLastHitChara().TeamNo == _actionChara.TeamNo);
		metaAi.CalcDistanceFromBallDropPrediPos();
	}
	public void MiniGameToss(int _ballNo, BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _isOver)
	{
	}
	public void Spike(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _miss)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_beachvolley_spike", _loop: false, 0f, 0.5f);
		BeachVolley_Define.BM.Spike(_actionChara, _gaugeValue, _dir);
		metaAi.CalcDistanceFromBallDropPrediPos();
	}
	public void Attack(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _miss)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_beachvolley_toss");
		BeachVolley_Define.BM.Attack(_actionChara, _gaugeValue, _dir);
		metaAi.CalcDistanceFromBallDropPrediPos();
	}
	public void Block(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _dir, bool _nice, bool _miss)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_ball_bound");
		BeachVolley_Define.BM.Block(_actionChara, _gaugeValue, _dir, _nice, _miss);
		metaAi.CalcDistanceFromBallDropPrediPos();
	}
	public void MissTouch(BeachVolley_Character _actionChara, bool _charaAir)
	{
		if (!_charaAir)
		{
			BeachVolley_Define.BM.MissTouch(_actionChara);
		}
		metaAi.CalcDistanceFromBallDropPrediPos();
		BeachVolley_Define.CM.ResetTapData(_actionChara.TeamNo);
	}
	public void SuperShotReady(int _teamNo)
	{
	}
	public void SuperShotCancel(int _teamNo)
	{
	}
	public void SetControlChara(BeachVolley_Character _chara)
	{
		controlChara[_chara.TeamNo] = _chara;
	}
	public BeachVolley_Character SearchCharaHandleBall(int _teamNo, Vector3 _pos, BeachVolley_Character _lastTouchChara, int _playerNo)
	{
		int num = -1;
		float num2 = 100f;
		BeachVolley_Define.FM.CheckInFrontZone(BeachVolley_Define.FM.ConvertLocalPos(_pos, _teamNo), _teamNo);
		bool flag = false;
		if (lasthitCharaPrev == null)
		{
			lasthitCharaPrev = _lastTouchChara;
		}
		else if (lasthitCharaPrev != _lastTouchChara)
		{
			flag = true;
			lasthitCharaPrev = _lastTouchChara;
		}
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[i];
			if ((beachVolley_Character.playerNo >= 0 && beachVolley_Character.playerNo != _playerNo) || beachVolley_Character == _lastTouchChara || !beachVolley_Character.IsShow())
			{
				continue;
			}
			if (!beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.FREEZE))
			{
				beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.SPIKE_AFTER);
			}
			float num3 = Vector3.Distance(_pos, beachVolley_Character.GetPos());
			if (!flag && cPrev[_playerNo] != null && cPrev[_playerNo] == beachVolley_Character)
			{
				num3 -= 1.5f;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
			}
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		if (num == -1)
		{
			num2 = 100f;
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[j];
				if (beachVolley_Character == _lastTouchChara || !beachVolley_Character.IsShow())
				{
					continue;
				}
				float num3 = Vector3.Distance(_pos, beachVolley_Character.GetPos());
				if (!flag && cPrev[_playerNo] != null && cPrev[_playerNo] == beachVolley_Character)
				{
					num3 -= 1.5f;
					if (num3 < 0f)
					{
						num3 = 0f;
					}
				}
				if (num3 < num2)
				{
					num2 = num3;
					num = j;
				}
			}
		}
		if (num == -1)
		{
			return null;
		}
		cPrev[_playerNo] = teamList[_teamNo].charas[num];
		return teamList[_teamNo].charas[num];
	}
	public BeachVolley_Character SearchCharaAttackStay(int _teamNo, Vector3 _pos, BeachVolley_Character _lastTouchChara, int _playerNo)
	{
		int num = -1;
		float num2 = 100f;
		if (!BeachVolley_Define.FM.CheckInFrontZone(BeachVolley_Define.FM.ConvertLocalPos(_pos, _teamNo), _teamNo))
		{
			return null;
		}
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[i];
			if (beachVolley_Character.playerNo >= 0 || beachVolley_Character == _lastTouchChara || !beachVolley_Character.IsShow() || beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.FREEZE))
			{
				continue;
			}
			float num3 = Vector3.Distance(_pos, beachVolley_Character.GetPos());
			if (cPrevAttackStay[_playerNo] != null && cPrevAttackStay[_playerNo] == beachVolley_Character)
			{
				num3 -= 1.5f;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
			}
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		if (num == -1)
		{
			num2 = 100f;
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[j];
				if (!beachVolley_Character.IsShow() || beachVolley_Character == _lastTouchChara)
				{
					continue;
				}
				float num3 = Vector3.Distance(_pos, beachVolley_Character.GetPos());
				if (cPrevAttackStay[_playerNo] != null && cPrevAttackStay[_playerNo] == beachVolley_Character)
				{
					num3 -= 1.5f;
					if (num3 < 0f)
					{
						num3 = 0f;
					}
				}
				if (num3 < num2)
				{
					num2 = num3;
					num = j;
				}
			}
		}
		if (num == -1)
		{
			return null;
		}
		cPrevAttackStay[_playerNo] = teamList[_teamNo].charas[num];
		return teamList[_teamNo].charas[num];
	}
	public BeachVolley_Character SearchBlockChara(int _teamNo, int _playerNo)
	{
		int num = -1;
		float num2 = 100f;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[i];
			if (!beachVolley_Character.CheckPositionState(BeachVolley_Character.PositionState.BACK_ZONE) && beachVolley_Character.IsShow() && (beachVolley_Character.playerNo < 0 || beachVolley_Character.playerNo == _playerNo) && !beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.FREEZE) && !beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.SPIKE_AFTER))
			{
				float num3 = Mathf.Abs(BeachVolley_Define.BM.GetBallDropPrediPosGround().x - beachVolley_Character.GetPos().x);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num != -1)
		{
			return teamList[_teamNo].charas[num];
		}
		return null;
	}
	public BeachVolley_Character SearchReceiveChara(int _teamNo, int _playerNo)
	{
		int num = -1;
		float num2 = 100f;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			BeachVolley_Character beachVolley_Character = teamList[_teamNo].charas[i];
			if (!beachVolley_Character.CheckPositionState(BeachVolley_Character.PositionState.FRONT_ZONE) && beachVolley_Character.IsShow() && (beachVolley_Character.playerNo < 0 || beachVolley_Character.playerNo == _playerNo) && !beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.FREEZE) && !beachVolley_Character.CheckActionState(BeachVolley_Character.ActionState.SPIKE_AFTER))
			{
				float num3 = Mathf.Abs(BeachVolley_Define.BM.GetBallDropPrediPosGround().x - beachVolley_Character.GetPos().x);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num != -1)
		{
			return teamList[_teamNo].charas[num];
		}
		return null;
	}
	public BeachVolley_Character SearchNearestBallDropPrediPos(int _teamNo, int _order = 0)
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			int distanceFromBallDropPrediPosOrder = metaAi.GetDistanceFromBallDropPrediPosOrder(_teamNo, i);
			if (num2 >= _order)
			{
				num = distanceFromBallDropPrediPosOrder;
				break;
			}
			num2++;
		}
		if (num != -1)
		{
			return teamList[_teamNo].charas[num];
		}
		return null;
	}
	public BeachVolley_Character SearchReceiveCharacter(int _teamNo, int _order = 0)
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			int distanceFromBallDropPrediPosOrder = metaAi.GetDistanceFromBallDropPrediPosOrder(_teamNo, i);
			if (!(teamList[_teamNo].charas[distanceFromBallDropPrediPosOrder] == BeachVolley_Define.Ball.GetLastHitChara()))
			{
				if (num2 >= _order)
				{
					num = distanceFromBallDropPrediPosOrder;
					break;
				}
				num2++;
			}
		}
		if (num != -1)
		{
			return teamList[_teamNo].charas[num];
		}
		return null;
	}
	public Vector3 SearchSpacePos(Vector3 _centerPos, int _xRangeMin, int _xRangeMax, int _yRnageMin, int _yRnageMax, int _teamNo)
	{
		return metaAi.GetSpacePos(_centerPos, _xRangeMin, _xRangeMax, _yRnageMin, _yRnageMax, _teamNo);
	}
	public bool CheckHaveBall(BeachVolley_Character _chara)
	{
		return haveBallChara == _chara;
	}
	public bool CheckHaveBall(GameObject _chara)
	{
		if (haveBallChara == null)
		{
			return false;
		}
		return haveBallChara.gameObject == _chara;
	}
	public bool CheckOpponentHaveBall(int _teamNo)
	{
		if (haveBallChara == null)
		{
			return false;
		}
		return haveBallChara.TeamNo != _teamNo;
	}
	public bool CheckHaveBallTeam(int _teamNo)
	{
		if (haveBallChara == null)
		{
			return false;
		}
		return haveBallChara.TeamNo == _teamNo;
	}
	public int CheckHaveBallTeam()
	{
		if (haveBallChara == null)
		{
			return -1;
		}
		return haveBallChara.TeamNo;
	}
	public bool CheckPrevHaveBallTeam(int _teamNo)
	{
		if (haveBallCharaPrev == null)
		{
			return false;
		}
		return haveBallCharaPrev.TeamNo == _teamNo;
	}
	public bool CheckLastTouched()
	{
		return BallTouchCnt >= BALL_TOUCH_LIMIT;
	}
	public bool CheckLastTouch()
	{
		return BallTouchCnt >= BALL_TOUCH_LIMIT - 1;
	}
	public BeachVolley_CharacterCursor GetCursor(int _teamNo)
	{
		return cursor[_teamNo];
	}
	public BeachVolley_Character GetHaveBallChara()
	{
		return haveBallChara;
	}
	public BeachVolley_Character GetHaveBallCharaPrev()
	{
		return haveBallCharaPrev;
	}
	public float GetDistanceFromBall(int _teamNo, int _charaNo)
	{
		return metaAi.GetDistanceFromBall(_teamNo, _charaNo);
	}
	public float GetDistanceFromBallByOrder(int _teamNo, int _order)
	{
		return metaAi.GetDistanceFromBall(_teamNo, metaAi.GetDistanceFromBallOrder(_teamNo, _order));
	}
	public BeachVolley_Character GetBallNearOrderList(int _teamNo, int _order)
	{
		return teamList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, _order)];
	}
	public List<BeachVolley_Character> TeamCharacterList(int _teamNo, int _no)
	{
		List<BeachVolley_Character> list = new List<BeachVolley_Character>();
		for (int i = 0; i < _no; i++)
		{
			list.Add(teamList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)]);
		}
		return list;
	}
	public BeachVolley_Character GetChara(GameObject _obj)
	{
		for (int i = 0; i < teamList.Length; i++)
		{
			for (int j = 0; j < teamList[i].charas.Length; j++)
			{
				if (teamList[i].charas[j].CheckObj(_obj))
				{
					return teamList[i].charas[j];
				}
			}
		}
		UnityEngine.Debug.LogError("存在しないキャラです : " + _obj.name);
		return null;
	}
	public BeachVolley_Character GetControlChara(int _playerNo)
	{
		return controlChara[_playerNo];
	}
	public bool CheckCharacterState(int _teamNo, BeachVolley_Character.ActionState _actionState)
	{
		for (int i = 0; i < playerList[_teamNo].Count; i++)
		{
			if (controlChara[playerList[_teamNo][i]].CheckActionState(_actionState))
			{
				return true;
			}
		}
		return false;
	}
	public BeachVolley_Character[] GetFrontZoneChara(int _teamNo, BeachVolley_Character _negrectChara)
	{
		UnityEngine.Debug.Log("_teamNo:" + _teamNo.ToString());
		UnityEngine.Debug.Log("_negrectChara:" + _negrectChara?.ToString());
		int num = 0;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			if (!(teamList[_teamNo].charas[i] == _negrectChara))
			{
				zoneCharaList[num] = teamList[_teamNo].charas[i];
				num++;
			}
		}
		return zoneCharaList;
	}
	public BeachVolley_Character[] GetBackZoneChara(int _teamNo, BeachVolley_Character _negrectChara)
	{
		int num = 0;
		for (int i = 0; i < BeachVolley_Define.Return_team_infield_num(); i++)
		{
			if (!(teamList[_teamNo].charas[i] == _negrectChara) && teamList[_teamNo].charas[i].CheckPositionState(BeachVolley_Character.PositionState.BACK_ZONE))
			{
				zoneCharaList[num] = teamList[_teamNo].charas[i];
				num++;
			}
		}
		return zoneCharaList;
	}
	public BeachVolley_MetaAI.InRestrictedArea GetRestrictedArea(int _teamNo)
	{
		return metaAi.GetRestrictedArea(_teamNo);
	}
	public int GetBenchCharaIndex()
	{
		return BeachVolley_Define.Return_team_infield_num();
	}
	public Vector3 ConvertOptimalPositioning(Vector3 _pos, int _teamNo, bool _attack)
	{
		Vector3 vector = BeachVolley_Define.FM.ConvertLocalPosPer(_pos, _teamNo);
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
		return BeachVolley_Define.FM.ConvertPosPerToWorld(posPer, _teamNo);
	}
	public Vector3 ConvertRestrictedArea(BeachVolley_Character _chara, Vector3 _pos)
	{
		return _pos;
	}
	private void OnDrawGizmos()
	{
	}
	public void TutorialInit()
	{
	}
	public void TutorialUpdate()
	{
		UpdateMethod();
	}
	public void TutorialServeInit(int _teamNo)
	{
		BeachVolley_Define.MGM.TutorialStartServe(_teamNo);
	}
	public void TutorialCharaReset()
	{
		for (int i = 0; i < teamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				teamList[i].charas[j].SettingGameStartPos(BeachVolley_Define.FM.GetFormationPos(i, j));
			}
		}
		for (int k = 0; k < controlChara.Length; k++)
		{
			if ((bool)controlChara[k])
			{
				cursor[k].SetArrowPos(controlChara[k] == haveBallChara);
			}
		}
		for (int l = 0; l < teamList.Length; l++)
		{
			for (int m = 0; m < teamList[l].charas.Length; m++)
			{
				teamList[l].charas[m].SetAction(null, _immediate: true, _forcibly: true);
			}
		}
		ChangeActionStateAllChara(BeachVolley_Character.ActionState.STANDARD);
	}
	public void ShowChara(int _teamNo, int _charaNo)
	{
	}
	public void HideChara(int _teamNo, bool _isAll = false)
	{
	}
	public void SetCharaPos(BeachVolley_Character _chara, float _xPer, float _zPer, Vector3 _dir)
	{
		_chara.transform.SetLocalPositionX(BeachVolley_Define.FM.GetFieldData().HalfCourtSize.x * _xPer);
		_chara.transform.SetLocalPositionZ(BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z * _zPer);
		_chara.transform.LookAt(_chara.GetPos() + _dir);
	}
	public void SetCharaLayer(string _layer)
	{
		for (int i = 0; i < teamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				teamList[i].charas[j].SettingLayer(_layer);
				teamList[i].charas[j].gameObject.layer = BeachVolley_Define.ConvertLayerNo(_layer);
			}
		}
	}
}
