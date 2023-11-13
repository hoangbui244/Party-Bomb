using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class MainCharacterManager : SingletonCustom<MainCharacterManager>
	{
		public struct CharacterList
		{
			public CharacterScript[] charas;
		}
		private float PLAYER_RESTRICTED_AREA_MAG = 1.5f;
		private CharacterScript haveBallCharaPrev;
		private CharacterScript haveBallChara;
		private CharacterScript[] controlChara = new CharacterScript[4];
		private CharacterList[] charaList;
		private List<CharacterScript> charaListTemp = new List<CharacterScript>();
		private float SHOOT_NEED_TIME = 1f;
		[SerializeField]
		[Header("カ\u30fcソル")]
		private CharacterCursor[] cursor;
		[SerializeField]
		[Header("パス先サ\u30fcクル")]
		private CharacterCursor[] passCircle;
		private float[] controlChangeInterval = new float[4];
		private static bool[] isPlayer = new bool[4]
		{
			true,
			false,
			false,
			false
		};
		private List<int>[] playerList = new List<int>[2];
		private MetaArtificialIntelligence metaAi = new MetaArtificialIntelligence();
		private CpuArtificialIntelligence cpuAi = new CpuArtificialIntelligence();
		private bool[] isPlayerAutoControl = new bool[4];
		public CharacterList[] CharaList => charaList;
		public CpuArtificialIntelligence CpuAi => cpuAi;
		public static bool IsGameWatchingMode()
		{
			return !isPlayer[0];
		}
		public static void SetGameWatchingMode(bool _flg)
		{
			isPlayer[0] = !_flg;
		}
		public void Init()
		{
			for (int i = 0; i < isPlayer.Length; i++)
			{
				UnityEngine.Debug.Log(i.ToString() + " : " + isPlayer[i].ToString());
			}
			SingletonCustom<UniformListManager>.Instance.ChangeEnemyUniformTournament();
			if (!isPlayer[0])
			{
				GameSaveData.MainGameMode selectMainGameMode = GameSaveData.GetSelectMainGameMode();
				if ((uint)(selectMainGameMode - 1) <= 1u || (uint)(selectMainGameMode - 5) <= 1u)
				{
					isPlayer[0] = true;
				}
			}
			for (int j = 1; j < isPlayer.Length; j++)
			{
				isPlayer[j] = (j < GameSaveData.GetSelectMultiPlayerNum());
			}
			for (int k = 0; k < 2; k++)
			{
				playerList[k] = new List<int>();
			}
			playerList = GameSaveData.GetSelectMultiPlayerList();
			charaList = new CharacterList[2];
			for (int l = 0; l < charaList.Length; l++)
			{
				charaList[l].charas = new CharacterScript[BeachSoccerDefine.TEAM_MEMBER_NUM];
				for (int m = 0; m < charaList[l].charas.Length; m++)
				{
					charaList[l].charas[m] = (Object.Instantiate(Resources.Load("Character"), base.transform.position, Quaternion.identity, SingletonCustom<FieldManager>.Instance.TeamAnchor[l]) as GameObject).GetComponent<CharacterScript>();
					charaList[l].charas[m].Init(l, m, SingletonCustom<FieldManager>.Instance.GetFormationAnchor(l, m), SingletonCustom<FormationListManager>.Instance.GetPosType(SingletonCustom<FieldManager>.Instance.FormationNo[l], m));
				}
			}
			metaAi.Init();
			cpuAi.Init();
		}
		public void SettingKickOff(int _teamNo)
		{
			UnityEngine.Debug.Log("キックオフ設定");
			ResetHaveBallChara();
			CharacterScript[] positionCenterNearOrderList = GetPositionCenterNearOrderList((_teamNo == 0) ? 1 : 0);
			for (int i = 0; i < playerList[(_teamNo == 0) ? 1 : 0].Count; i++)
			{
				ChangeControlChara((_teamNo == 0) ? 1 : 0, playerList[(_teamNo == 0) ? 1 : 0][i], positionCenterNearOrderList[i]);
			}
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.KICK_OFF);
			SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			SingletonCustom<MainCharacterManager>.Instance.ChangeActionStateAllChara(CharacterScript.ActionState.KICK_OFF_STANDBY);
			for (int j = 0; j < charaList.Length; j++)
			{
				int k = 0;
				CharacterScript[] positionCenterNearOrderList2 = GetPositionCenterNearOrderList(j);
				if (j == _teamNo)
				{
					haveBallChara = positionCenterNearOrderList2[0];
					SingletonCustom<BallManager>.Instance.GetBall().SetLastHitChara(positionCenterNearOrderList2[0]);
					for (int l = 0; l < playerList[j].Count; l++)
					{
						ChangeControlChara(j, playerList[j][l], positionCenterNearOrderList2[l]);
					}
					positionCenterNearOrderList2[0].ResetPosData();
					if (positionCenterNearOrderList2[0].GetFormationPos(_local: false, _half: true).x < positionCenterNearOrderList2[1].GetFormationPos(_local: false, _half: true).x)
					{
						positionCenterNearOrderList2[0].SettingKickOffStandbyPos(SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position + Vector3.left * 0.5f, 0);
					}
					else
					{
						positionCenterNearOrderList2[0].SettingKickOffStandbyPos(SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position + Vector3.right * 0.5f, 0);
					}
					for (; k < 1; k++)
					{
						positionCenterNearOrderList2[k].SetAction(positionCenterNearOrderList2[k].AiKickOffStandbyKicker, _immediate: true, _forcibly: true);
					}
				}
				for (; k < positionCenterNearOrderList2.Length; k++)
				{
					positionCenterNearOrderList2[k].SettingKickOffStandbyPos(positionCenterNearOrderList2[k].GetFormationPos(_local: false, _half: true), -1, _teamNo == j);
					positionCenterNearOrderList2[k].SetAction(positionCenterNearOrderList2[k].AiKickOffStandby, _immediate: true, _forcibly: true);
				}
			}
			for (int m = 0; m < controlChara.Length; m++)
			{
				if ((bool)controlChara[m])
				{
					cursor[m].SetArrowPos(controlChara[m] == haveBallChara);
				}
			}
			SingletonCustom<BallManager>.Instance.GetBall().GetCollider().isTrigger = true;
		}
		public void UpdateMethod()
		{
			metaAi.UpdateMethod();
			for (int i = 0; i < charaList.Length; i++)
			{
				for (int j = 0; j < charaList[i].charas.Length; j++)
				{
					if (!SingletonCustom<MainGameManager>.Instance.IsStopChara())
					{
						cpuAi.UpdateAutoAction(i, j);
						metaAi.UpdateRestrictedAreaData();
						bool flag = true;
						for (int k = 0; k < 4; k++)
						{
							if (isPlayer[k] && controlChara[k] == charaList[i].charas[j])
							{
								if (!SingletonCustom<MainGameManager>.Instance.IsKickOffStandby() && !SingletonCustom<MainGameManager>.Instance.IsAutoMove() && !CheckInRestrictedArea(controlChara[k]) && !charaList[i].charas[j].CheckAiAction(charaList[i].charas[j].AiMovePos) && !charaList[i].charas[j].CheckActionState(CharacterScript.ActionState.GOAL_PRODUCTION))
								{
									PlayerOperation(i, k);
									isPlayerAutoControl[k] = false;
								}
								else
								{
									isPlayerAutoControl[k] = true;
									charaList[i].charas[j].AiMove();
								}
								flag = false;
								break;
							}
						}
						if (flag)
						{
							charaList[i].charas[j].AiMove();
						}
					}
					charaList[i].charas[j].UpdateMethod();
				}
			}
			for (int l = 0; l < cursor.Length; l++)
			{
				if ((bool)controlChara[l])
				{
					cursor[l].SetArrowPos(controlChara[l] == haveBallChara);
					cursor[l].ShowCircle(controlChara[l] == haveBallChara || true);
					cursor[l].ShowCircleAlpha(controlChara[l] == haveBallChara);
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
			SingletonCustom<CharacterNameManager>.Instance.UpdateNameObj();
		}
		public void ChangeCursor(CharacterScript _chara)
		{
			controlChangeInterval[_chara.PlayerNo] = 1f;
			cursor[_chara.PlayerNo].SetCharacter(_chara);
		}
		public void ShowCursor(bool _show)
		{
			for (int i = 0; i < cursor.Length; i++)
			{
				cursor[i].Show(_show && isPlayer[i]);
			}
		}
		public void ChangePassCircle(CharacterScript _chara, int _teamNo, int _playerNo)
		{
			passCircle[_playerNo].SetCharacter(_chara);
			ShowPassCircle(_chara != null, _teamNo, _playerNo);
		}
		public void ShowPassCircle(bool _show, int _teamNo = -1, int _playerNo = -1)
		{
			if (_teamNo == -1)
			{
				for (int i = 0; i < passCircle.Length; i++)
				{
					passCircle[i].gameObject.SetActive(_show);
				}
			}
			else if (_playerNo == -1)
			{
				for (int j = 0; j < passCircle.Length; j++)
				{
					passCircle[j].gameObject.SetActive(_show);
				}
			}
			else
			{
				passCircle[_playerNo].gameObject.SetActive(_show);
			}
		}
		public void ResetHaveBallChara()
		{
			if (haveBallChara != null && haveBallChara.PlayerNo != -1)
			{
				cursor[haveBallChara.PlayerNo].ResetGauge();
			}
			haveBallCharaPrev = null;
			haveBallChara = null;
		}
		public void HaveBallCharaBallRelease()
		{
			if (haveBallChara != null && haveBallChara.PlayerNo != -1)
			{
				cursor[haveBallChara.PlayerNo].ResetGauge();
			}
			haveBallCharaPrev = haveBallChara;
			haveBallChara = null;
		}
		public void ChangeControlChara(int _teamNo, int _playerNo, CharacterScript _chara = null, bool _isPlayerMove = false)
		{
			if (_playerNo == -1)
			{
				return;
			}
			if ((bool)_chara)
			{
				UnityEngine.Debug.Log("操作キャラ変更 : チ\u30fcム " + _chara.TeamNo.ToString() + " : " + _chara.CharaNo.ToString());
				for (int i = 0; i < controlChara.Length; i++)
				{
					if (controlChara[i] == _chara)
					{
						return;
					}
				}
				controlChara[_playerNo] = _chara;
				for (int j = 0; j < charaList[_teamNo].charas.Length; j++)
				{
					if (charaList[_teamNo].charas[j].PlayerNo == _playerNo)
					{
						charaList[_teamNo].charas[j].PlayerNo = -1;
						break;
					}
				}
				controlChara[_playerNo].PlayerNo = _playerNo;
				ChangeCursor(controlChara[_playerNo]);
				controlChara[_playerNo].ResetControlInterval();
			}
			else
			{
				if (CheckHaveBall(controlChara[_playerNo]))
				{
					return;
				}
				if (controlChara[_playerNo] == null)
				{
					ChangeControlChara(_teamNo, _playerNo, SearchBallNearest(_teamNo, isPlayer[_playerNo]));
				}
				else if (CheckOpponentAttack(_teamNo))
				{
					if ((!(controlChangeInterval[_playerNo] <= 0f) || (_isPlayerMove && !(metaAi.GetDistanceFromMyGoal(controlChara[_playerNo].TeamNo, controlChara[_playerNo].CharaNo) >= metaAi.GetDistanceFromGoalToBall(controlChara[_playerNo].TeamNo) + CharacterScript.NEAR_DISTANCE))) && controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.STANDARD) && !(SingletonCustom<BallManager>.Instance.GetBallPos(_offset: false).y >= controlChara[_playerNo].GetPos().y + controlChara[_playerNo].GetCharaHeight()))
					{
						return;
					}
					CharacterScript characterScript = null;
					for (int k = 0; k < charaList[_teamNo].charas.Length; k++)
					{
						CharacterScript characterScript2 = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, k)];
						if (characterScript2.CheckActionState(CharacterScript.ActionState.STANDARD) && !characterScript2.CheckPositionType(GameDataParams.PositionType.GK) && !(metaAi.GetDistanceFromMyGoal(characterScript2.TeamNo, characterScript2.CharaNo) >= metaAi.GetDistanceFromGoalToBall(characterScript2.TeamNo)))
						{
							characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, k)];
							break;
						}
					}
					if (characterScript == null)
					{
						characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
					}
					if (characterScript != controlChara[_playerNo])
					{
						ChangeControlChara(_teamNo, _playerNo, characterScript);
					}
				}
				else
				{
					if ((!(controlChangeInterval[_playerNo] <= 0f) || ((CheckBallNearest(controlChara[_playerNo]) || _isPlayerMove) && !(metaAi.GetDistanceFromBall(_teamNo, controlChara[_playerNo].CharaNo) >= CharacterScript.PERSONAL_SPACE_OUT))) && controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.STANDARD) && !(SingletonCustom<BallManager>.Instance.GetBallPos(_offset: false).y >= controlChara[_playerNo].GetPos().y + controlChara[_playerNo].GetCharaHeight()))
					{
						return;
					}
					CharacterScript characterScript3 = null;
					for (int l = 0; l < charaList[_teamNo].charas.Length; l++)
					{
						if (charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, l)].CheckActionState(CharacterScript.ActionState.STANDARD) && !charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, l)].CheckPositionType(GameDataParams.PositionType.GK))
						{
							characterScript3 = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, l)];
							break;
						}
					}
					if (characterScript3 == null)
					{
						characterScript3 = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
					}
					if (characterScript3 != controlChara[_playerNo])
					{
						ChangeControlChara(_teamNo, _playerNo, characterScript3);
					}
				}
			}
		}
		public void KickOff()
		{
			for (int i = 0; i < charaList.Length; i++)
			{
				for (int j = 0; j < charaList[i].charas.Length; j++)
				{
					charaList[i].charas[j].SetAction(charaList[i].charas[j].AiStandby, _immediate: true, _forcibly: true);
				}
			}
			SingletonCustom<BallManager>.Instance.GetBall().GetCollider().isTrigger = false;
			ChangeActionStateAllChara(CharacterScript.ActionState.STANDARD);
			SingletonCustom<MainGameManager>.Instance.ChangeStateInPlay();
		}
		public void ChangeActionStateAllChara(CharacterScript.ActionState _state, int _teamNo = -1)
		{
			if (_teamNo != -1)
			{
				for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
				{
					charaList[_teamNo].charas[i].SetActionState(_state);
				}
				return;
			}
			for (int j = 0; j < charaList.Length; j++)
			{
				for (int k = 0; k < charaList[j].charas.Length; k++)
				{
					charaList[j].charas[k].SetActionState(_state);
				}
			}
		}
		private void PlayerOperation(int _teamNo, int _playerNo)
		{
			if (controlChara[_playerNo] == haveBallChara)
			{
				if (SingletonCustom<ControllerManager>.Instance.GetTapTime(_playerNo) > 0f)
				{
					cursor[_playerNo].SetGauge(Mathf.Min(SingletonCustom<ControllerManager>.Instance.GetTapTime(_playerNo) / SHOOT_NEED_TIME, 1f));
				}
				else
				{
					cursor[_playerNo].ResetGauge();
				}
				CharacterScript characterScript = null;
				characterScript = ((!controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.THROW_IN) && !controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.CORNER_KICK)) ? SearchAllyPassed(haveBallChara, haveBallChara.TeamNo) : SearchAllyPassed(haveBallChara, GetCursor(_playerNo).transform.forward, haveBallChara.TeamNo));
				ChangePassCircle(characterScript, _teamNo, _playerNo);
			}
			else
			{
				ChangePassCircle(null, _teamNo, _playerNo);
				cursor[_playerNo].ResetGauge();
				ShowPassCircle(_show: false, _teamNo, _playerNo);
			}
			if (controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.KICK_OFF_STANDBY))
			{
				if (controlChara[_playerNo] == haveBallChara)
				{
					controlChara[_playerNo].transform.LookAt(SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position);
					if (SingletonCustom<ControllerManager>.Instance.IsTap(_playerNo) || SingletonCustom<GameUiManager>.Instance.IsTimeLimitFinish(_teamNo))
					{
						SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
						controlChara[_playerNo].transform.LookAt(SearchAllyPassed(haveBallChara, haveBallChara.TeamNo).transform);
						KickBall(cursor[_playerNo].GetGaugeValue(), controlChara[_playerNo], SearchAllyPassed(haveBallChara, haveBallChara.TeamNo));
						KickOff();
						SingletonCustom<ControllerManager>.Instance.ResetTapData(_playerNo);
					}
				}
			}
			else if (controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.THROW_IN))
			{
				if (SingletonCustom<ControllerManager>.Instance.IsTap(_playerNo) || SingletonCustom<GameUiManager>.Instance.IsTimeLimitFinish(_teamNo))
				{
					SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
					UnityEngine.Debug.Log("P:スロ\u30fcインする");
					controlChara[_playerNo].StartBallThrowIn(cursor[_playerNo].GetGaugeValue());
					SingletonCustom<ControllerManager>.Instance.ResetTapData(_playerNo);
				}
			}
			else if (controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.CORNER_KICK))
			{
				if (SingletonCustom<ControllerManager>.Instance.IsTap(_playerNo) || SingletonCustom<GameUiManager>.Instance.IsTimeLimitFinish(_teamNo))
				{
					SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
					controlChara[_playerNo].StartCornerKick(cursor[_playerNo].GetGaugeValue(), _player: true);
					SingletonCustom<ControllerManager>.Instance.ResetTapData(_playerNo);
				}
			}
			else if (controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.GOAL_KICK))
			{
				if (SingletonCustom<ControllerManager>.Instance.IsTap(_playerNo) || SingletonCustom<GameUiManager>.Instance.IsTimeLimitFinish(_teamNo))
				{
					SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
					controlChara[_playerNo].StartGoalKick(cursor[_playerNo].GetGaugeValue(), _player: true);
					SingletonCustom<ControllerManager>.Instance.ResetTapData(_playerNo);
				}
			}
			else if (controlChara[_playerNo].CheckActionState(CharacterScript.ActionState.STANDARD))
			{
				if ((controlChara[_playerNo].GetControlInterval() >= 0.3f || CheckHaveBall(controlChara[_playerNo])) && !SingletonCustom<MainGameManager>.Instance.CheckGameState(MainGameManager.GameState.KICK_OFF) && SingletonCustom<ControllerManager>.Instance.IsMove(_playerNo))
				{
					CharaMove(_playerNo);
				}
				if (SingletonCustom<ControllerManager>.Instance.IsTap(_playerNo) || SingletonCustom<GameUiManager>.Instance.IsTimeLimitFinish(_teamNo))
				{
					if (SingletonCustom<BallManager>.Instance.CheckBallState(BallManager.BallState.FREE) && SingletonCustom<BallManager>.Instance.GetBallPos(_offset: false).y >= controlChara[_playerNo].GetPos().y + controlChara[_playerNo].GetCharaHeight())
					{
						switch (Random.Range(0, 4))
						{
						case 0:
							controlChara[_playerNo].Heading();
							break;
						case 1:
							controlChara[_playerNo].DivingHead();
							break;
						case 2:
							controlChara[_playerNo].OverHeadKick();
							break;
						case 3:
							controlChara[_playerNo].JumpingVolley();
							break;
						}
					}
					else if (CheckOpponentAttack(_teamNo))
					{
						if (SingletonCustom<MainGameManager>.Instance.CheckInPlay())
						{
							if (controlChara[_playerNo].CheckPositionType(GameDataParams.PositionType.GK))
							{
								controlChara[_playerNo].DivingCatch(SingletonCustom<BallManager>.Instance.GetBallPos());
							}
							else
							{
								controlChara[_playerNo].Sliding();
								UnityEngine.Debug.Log("スライディング");
							}
						}
					}
					else if (controlChara[_playerNo] == haveBallChara && SingletonCustom<MainGameManager>.Instance.CheckInPlay())
					{
						SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
						KickBall(cursor[_playerNo].GetGaugeValue(), controlChara[_playerNo], SearchAllyPassed(haveBallChara, haveBallChara.TeamNo));
					}
					SingletonCustom<ControllerManager>.Instance.ResetTapData(_playerNo);
				}
				if (SingletonCustom<ControllerManager>.Instance.IsChangeChara(_playerNo))
				{
					if (haveBallChara == null)
					{
						if (!controlChara[_playerNo].CheckPositionType(GameDataParams.PositionType.GK))
						{
							ChangeControlChara(_teamNo, _playerNo, SearchBallNearestAiChara(_teamNo, _playerNo));
							controlChara[_playerNo].SetControlInterval(1f);
						}
					}
					else if (!haveBallChara.CheckPositionType(GameDataParams.PositionType.GK) && haveBallChara.PlayerNo != _playerNo)
					{
						ChangeControlChara(_teamNo, _playerNo, SearchBallNearestAiChara(_teamNo, _playerNo));
						controlChara[_playerNo].SetControlInterval(1f);
					}
					SingletonCustom<ControllerManager>.Instance.ResetChangeChara(_playerNo);
				}
			}
			else if (SingletonCustom<ControllerManager>.Instance.IsChangeChara(_playerNo))
			{
				if (haveBallChara == null)
				{
					if (!controlChara[_playerNo].CheckPositionType(GameDataParams.PositionType.GK))
					{
						ChangeControlChara(_teamNo, _playerNo, SearchBallNearestAiChara(_teamNo, _playerNo));
						controlChara[_playerNo].SetControlInterval(1f);
					}
				}
				else if (!haveBallChara.CheckPositionType(GameDataParams.PositionType.GK) && haveBallChara.PlayerNo != _playerNo)
				{
					ChangeControlChara(_teamNo, _playerNo, SearchBallNearestAiChara(_teamNo, _playerNo));
					controlChara[_playerNo].SetControlInterval(1f);
				}
				SingletonCustom<ControllerManager>.Instance.ResetChangeChara(_playerNo);
			}
			if (SingletonCustom<MainGameManager>.Instance.CheckInPlay())
			{
				CheckOpponentKeeperHaveBall(_teamNo);
			}
		}
		private void CharaMove(int _no)
		{
			Vector3 vector = SingletonCustom<ControllerManager>.Instance.GetMoveDir(_no);
			if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.HORIZONTAL) && !SingletonCustom<MainGameManager>.Instance.IsFirstHalf())
			{
				vector *= -1f;
			}
			controlChara[_no].Move(vector, SingletonCustom<ControllerManager>.Instance.GetMoveLength(_no));
		}
		public void CatchBall(CharacterScript _chara)
		{
			if (haveBallCharaPrev != null && haveBallCharaPrev.PlayerNo != -1)
			{
				SingletonCustom<ControllerManager>.Instance.ResetTapData(haveBallCharaPrev.PlayerNo);
			}
			SetHaveBallChara(_chara);
			if (_chara.PlayerNo != -1)
			{
				SingletonCustom<ControllerManager>.Instance.ResetTapData(_chara.PlayerNo);
			}
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.KEEP);
			SingletonCustom<BallManager>.Instance.Catch(_chara);
			if (_chara.CheckPositionType(GameDataParams.PositionType.GK) && _chara.IsBallCatch)
			{
				SingletonCustom<GameUiManager>.Instance.TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE.PUNT_KICK, _chara.TeamNo);
			}
		}
		public void SetHaveBallChara(CharacterScript _chara)
		{
			if (haveBallCharaPrev == null)
			{
				_chara.IsPassFromFriend = false;
				_chara.IsBallCatch = (_chara.CheckPositionType(GameDataParams.PositionType.GK) && SingletonCustom<FieldManager>.Instance.CheckInPenaltyArea(_chara));
			}
			else
			{
				_chara.IsPassFromFriend = (_chara.TeamNo == haveBallCharaPrev.TeamNo);
				_chara.IsBallCatch = (_chara.CheckPositionType(GameDataParams.PositionType.GK) && !_chara.IsPassFromFriend && SingletonCustom<FieldManager>.Instance.CheckInPenaltyArea(_chara));
			}
			haveBallCharaPrev = haveBallChara;
			haveBallChara = _chara;
			ChangeControlChara(haveBallChara.TeamNo, SearchMeNearestPlayerChara(haveBallChara, haveBallChara.TeamNo).PlayerNo, _chara);
			if (haveBallChara.PlayerNo != -1)
			{
				SingletonCustom<ControllerManager>.Instance.ResetTouchData(haveBallChara.PlayerNo, _bkReset: true);
			}
		}
		public void KickBall(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			UnityEngine.Debug.Log("ボ\u30fcルを蹴る");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			_chara.KickAnimation();
			SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
			SingletonCustom<BallManager>.Instance.Kick(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			if (_chara.CheckPositionType(GameDataParams.PositionType.GK))
			{
				CharacterScript chara = SearchBallNearestDefense(_chara.TeamNo);
				ChangeControlChara(_chara.TeamNo, _chara.PlayerNo, chara);
			}
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void Heading(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.Heading(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void DivingHead(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.DivingHead(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void OverHeadKick(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.OverHeadKick(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void JumpingVolley(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.JumpingVolley(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void ThrowIn(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<BallManager>.Instance.ThrowIn(_power, _chara, _passAlly);
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
		}
		public void CornerKick(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.CornerKick(_power, _chara, _passAlly);
			_chara.KickAnimation();
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public void GoalKick(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_chara.PlayerNo);
			SingletonCustom<BallManager>.Instance.GoalKick(_power, _chara, _passAlly);
			_chara.KickAnimation();
			HaveBallCharaBallRelease();
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.FREE);
			CharacterScript chara = SearchBallNearestDefense(_chara.TeamNo);
			ChangeControlChara(_chara.TeamNo, _chara.PlayerNo, chara);
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		public CharacterScript SearchNearestWhoCanPass(CharacterScript _chara, bool _frontward = true)
		{
			float num = 100f;
			int num2 = -1;
			for (int i = 0; i < charaList[_chara.TeamNo].charas.Length; i++)
			{
				if (charaList[_chara.TeamNo].charas[i] == _chara)
				{
					continue;
				}
				if (_frontward)
				{
					if (charaList[_chara.TeamNo].charas[i].GetPos(_isLocal: true).z < _chara.GetPos(_isLocal: true).z)
					{
						continue;
					}
				}
				else if (charaList[_chara.TeamNo].charas[i].GetPos(_isLocal: true).z > _chara.GetPos(_isLocal: true).z)
				{
					continue;
				}
				UnityEngine.Debug.Log("空いていいる");
				float distanceFromBall = metaAi.GetDistanceFromBall(_chara.TeamNo, i);
				if (distanceFromBall < num)
				{
					num = distanceFromBall;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				return charaList[_chara.TeamNo].charas[num2];
			}
			return SearchNearestWhoCanPass(_chara, !_frontward);
		}
		public CharacterScript SearchBallNearest(int _teamNo, bool _exceptKeeper = false)
		{
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)].CheckPositionType(GameDataParams.PositionType.GK) || !charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)].CheckActionState(CharacterScript.ActionState.FREEZE))
				{
					return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)];
				}
			}
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
		}
		public CharacterScript SearchBallNearestDefense(int _teamNo)
		{
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CharacterScript characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)];
				if (!characterScript.CheckPositionType(GameDataParams.PositionType.GK) && !characterScript.CheckActionState(CharacterScript.ActionState.FREEZE) && !(metaAi.GetDistanceFromGoalToBall(_teamNo) <= metaAi.GetDistanceFromBall(_teamNo, characterScript.CharaNo)))
				{
					return characterScript;
				}
			}
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
		}
		public CharacterScript SearchBallNearestAiDefenseChara(int _teamNo)
		{
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CharacterScript characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)];
				if (!characterScript.CheckPositionType(GameDataParams.PositionType.GK) && !characterScript.CheckActionState(CharacterScript.ActionState.FREEZE) && characterScript.PlayerNo == -1 && !(metaAi.GetDistanceFromGoalToBall(_teamNo) <= metaAi.GetDistanceFromBall(_teamNo, characterScript.CharaNo)))
				{
					return characterScript;
				}
			}
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
		}
		public CharacterScript SearchBallNearestAiChara(int _teamNo, int _playerNo = -1)
		{
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CharacterScript characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)];
				if (!characterScript.CheckPositionType(GameDataParams.PositionType.GK) && !characterScript.CheckActionState(CharacterScript.ActionState.FREEZE) && (characterScript.PlayerNo == -1 || characterScript.PlayerNo == _playerNo))
				{
					return characterScript;
				}
			}
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, 0)];
		}
		public CharacterScript SearchDefenseCover(int _teamNo, bool _isPlayer = false)
		{
			bool flag = false;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CharacterScript characterScript = charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, i)];
				if (!characterScript.CheckPositionType(GameDataParams.PositionType.GK) && !characterScript.CheckActionState(CharacterScript.ActionState.FREEZE) && !(metaAi.GetDistanceFromGoalToBall(_teamNo) <= metaAi.GetDistanceFromBall(_teamNo, characterScript.CharaNo)))
				{
					if (flag | _isPlayer)
					{
						return characterScript;
					}
					flag = true;
				}
			}
			return null;
		}
		public CharacterScript SearchMeNearest(CharacterScript _chara, int _teamNo)
		{
			float num = 100f;
			int num2 = -1;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!(charaList[_teamNo].charas[i] == _chara) && charaList[_teamNo].charas[i].PlayerNo == -1)
				{
					CalcManager.mCalcFloat = CalcManager.Length(charaList[_teamNo].charas[i].GetPos(), _chara.GetPos());
					if (CalcManager.mCalcFloat < num)
					{
						num = CalcManager.mCalcFloat;
						num2 = i;
					}
				}
			}
			return charaList[_teamNo].charas[num2];
		}
		public CharacterScript SearchMeNearestNotGKChara(CharacterScript _chara, int _teamNo, int _playerNo = -1)
		{
			float num = 100f;
			int num2 = -1;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!(charaList[_teamNo].charas[i] == _chara) && charaList[_teamNo].charas[i].PlayerNo == -1 && !charaList[_teamNo].charas[i].CheckPositionType(GameDataParams.PositionType.GK) && !charaList[_teamNo].charas[i].CheckActionState(CharacterScript.ActionState.FREEZE))
				{
					CalcManager.mCalcFloat = CalcManager.Length(charaList[_teamNo].charas[i].GetPos(), _chara.GetPos());
					if (CalcManager.mCalcFloat < num)
					{
						num = CalcManager.mCalcFloat;
						num2 = i;
					}
				}
			}
			if (num2 == -1)
			{
				return _chara;
			}
			return charaList[_teamNo].charas[num2];
		}
		public CharacterScript SearchMeNearestPlayerChara(CharacterScript _chara, int _teamNo)
		{
			CharacterScript ballNearOrderList = GetBallNearOrderList(_teamNo, 0);
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				ballNearOrderList = GetBallNearOrderList(_teamNo, i);
				if (ballNearOrderList.PlayerNo != -1)
				{
					return ballNearOrderList;
				}
			}
			return GetBallNearOrderList(_teamNo, 0);
		}
		public CharacterScript SearchPosNearest(Vector3 _pos, int _teamNo)
		{
			float num = 100f;
			int num2 = -1;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CalcManager.mCalcFloat = CalcManager.Length(charaList[_teamNo].charas[i].GetPos(), _pos);
				if (CalcManager.mCalcFloat < num)
				{
					num = CalcManager.mCalcFloat;
					num2 = i;
				}
			}
			return charaList[_teamNo].charas[num2];
		}
		public CharacterScript SearchPosNearestFormation(Vector3 _pos, int _teamNo)
		{
			float num = 100f;
			int num2 = -1;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				CalcManager.mCalcFloat = CalcManager.Length(charaList[_teamNo].charas[i].GetFormationPos(_local: false, _half: true), _pos);
				if (CalcManager.mCalcFloat < num)
				{
					num = CalcManager.mCalcFloat;
					num2 = i;
				}
			}
			return charaList[_teamNo].charas[num2];
		}
		public CharacterScript SearchAllyPass(CharacterScript _chara, int _teamNo)
		{
			charaListTemp.Clear();
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!(charaList[_teamNo].charas[i] == _chara) && charaList[_teamNo].charas[i].GetPos(_isLocal: true).z > _chara.GetPos(_isLocal: true).z + SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 2f)
				{
					charaListTemp.Add(charaList[_teamNo].charas[i]);
				}
			}
			float num = 100f;
			int num2 = -1;
			if (charaListTemp.Count > 0)
			{
				num = 100f;
				num2 = 0;
				for (int j = 0; j < charaListTemp.Count; j++)
				{
					CalcManager.mCalcFloat = CalcManager.Length(charaListTemp[j].GetPos(), _chara.GetPos());
					if (CalcManager.mCalcFloat < num)
					{
						num = CalcManager.mCalcFloat;
						num2 = j;
					}
				}
				return charaListTemp[num2];
			}
			num = 100f;
			for (int k = 0; k < charaList[_teamNo].charas.Length; k++)
			{
				if (!(charaList[_teamNo].charas[k] == _chara))
				{
					CalcManager.mCalcFloat = CalcManager.Length(charaList[_teamNo].charas[k].GetPos(), _chara.GetPos());
					if (CalcManager.mCalcFloat < num)
					{
						num = CalcManager.mCalcFloat;
						num2 = k;
					}
				}
			}
			return charaList[_teamNo].charas[num2];
		}
		public CharacterScript SearchAllyPassed(CharacterScript _chara, int _teamNo)
		{
			return SearchAllyPassed(_chara, _chara.transform.forward, _teamNo);
		}
		public CharacterScript SearchAllyPassed(CharacterScript _chara, Vector3 _dir, int _teamNo)
		{
			float num = CalcManager.Rot(_dir, CalcManager.AXIS.Y);
			int charaNo = _chara.CharaNo;
			float num3 = (num + 45f) % 360f;
			charaListTemp.Clear();
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!(charaList[_teamNo].charas[i] == _chara) && charaList[_teamNo].charas[i].PlayerNo == -1)
				{
					CalcManager.mCalcFloat = CalcManager.Rot(charaList[_teamNo].charas[i].GetPos() - _chara.GetPos(), CalcManager.AXIS.Y);
					if (Mathf.Abs(num - CalcManager.mCalcFloat) < 30f)
					{
						charaListTemp.Add(charaList[_teamNo].charas[i]);
					}
				}
			}
			if (charaListTemp.Count > 0)
			{
				float num2 = 100f;
				charaNo = 0;
				for (int j = 0; j < charaListTemp.Count; j++)
				{
					CalcManager.mCalcFloat = CalcManager.Length(charaListTemp[j].GetPos(), _chara.GetPos());
					if (CalcManager.mCalcFloat < num2)
					{
						num2 = CalcManager.mCalcFloat;
						charaNo = j;
					}
				}
				return charaListTemp[charaNo];
			}
			return null;
		}
		public CharacterScript SearchThrowIn(int _teamNo)
		{
			CharacterScript ballNearOrderList = GetBallNearOrderList(_teamNo, 0);
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				ballNearOrderList = GetBallNearOrderList(_teamNo, i);
				if (ballNearOrderList.PlayerNo != -1)
				{
					break;
				}
			}
			CharacterScript ballNearOrderList2 = GetBallNearOrderList(_teamNo, 0);
			if (ballNearOrderList2.PlayerNo == -1 && ballNearOrderList.PlayerNo != -1)
			{
				ChangeControlChara(_teamNo, ballNearOrderList.PlayerNo, ballNearOrderList2);
			}
			return ballNearOrderList2;
		}
		public Vector3 SearchSpacePos(Vector3 _centerPos, int _xRangeMin, int _xRangeMax, int _yRnageMin, int _yRnageMax, int _teamNo)
		{
			return metaAi.GetSpacePos(_centerPos, _xRangeMin, _xRangeMax, _yRnageMin, _yRnageMax, _teamNo);
		}
		public bool CheckHaveBall(CharacterScript _chara)
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
		public bool CheckBallNearest(CharacterScript _chara)
		{
			return metaAi.GetDistanceFromBallOrder(_chara.TeamNo, 0) == _chara.CharaNo;
		}
		public bool CheckDefenseCoverChara(CharacterScript _chara)
		{
			return SearchDefenseCover(_chara.TeamNo) == _chara;
		}
		public bool CheckBallNearestDefenseChara(CharacterScript _chara)
		{
			return SearchBallNearestDefense(_chara.TeamNo) == _chara;
		}
		public bool CheckOpponentHaveBall(int _teamNo)
		{
			if (haveBallChara == null)
			{
				return false;
			}
			return haveBallChara.TeamNo != _teamNo;
		}
		public bool CheckOpponentKeeperHaveBall(int _teamNo)
		{
			if (haveBallChara == null)
			{
				return false;
			}
			if (haveBallChara.TeamNo != _teamNo)
			{
				return haveBallChara.CheckPositionType(GameDataParams.PositionType.GK);
			}
			return false;
		}
		public bool CheckHaveBallTeam(int _teamNo)
		{
			if (haveBallChara == null)
			{
				return false;
			}
			return haveBallChara.TeamNo == _teamNo;
		}
		public bool CheckPrevHaveBallTeam(int _teamNo)
		{
			if (haveBallCharaPrev == null)
			{
				return false;
			}
			return haveBallCharaPrev.TeamNo == _teamNo;
		}
		public bool CheckOpponentAttack(int _teamNo)
		{
			if (haveBallChara == null)
			{
				if (haveBallCharaPrev == null)
				{
					return false;
				}
				return haveBallCharaPrev.TeamNo != _teamNo;
			}
			return haveBallChara.TeamNo != _teamNo;
		}
		public bool CheckWichOneCornerKick(int _teamNo)
		{
			return haveBallCharaPrev.TeamNo == _teamNo;
		}
		public bool CheckControlChara(CharacterScript _chara)
		{
			if (_chara.PlayerNo == -1)
			{
				return false;
			}
			return controlChara[_chara.PlayerNo] == _chara;
		}
		public bool CheckAllKickOffStandby()
		{
			for (int i = 0; i < charaList.Length; i++)
			{
				for (int j = 0; j < charaList[i].charas.Length; j++)
				{
					if (!charaList[i].charas[j].CheckKickoffPosition())
					{
						return false;
					}
				}
			}
			return true;
		}
		public bool CheckPlayerControl(CharacterScript _chara)
		{
			if (isPlayer[_chara.PlayerNo] && controlChara[_chara.PlayerNo] == _chara)
			{
				return true;
			}
			return false;
		}
		public bool CheckInRestrictedArea(CharacterScript _chara)
		{
			if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.KEEPER_KEEP)
			{
				if (!CheckHaveBall(_chara) || !_chara.CheckPositionType(GameDataParams.PositionType.GK))
				{
					float num = metaAi.GetRestrictedArea(_chara.TeamNo).size;
					if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
					{
						num *= PLAYER_RESTRICTED_AREA_MAG;
					}
					if (CalcManager.Length(haveBallChara.GetPos(), _chara.GetPos()) < num)
					{
						return true;
					}
				}
			}
			else if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.GOAL_KICK)
			{
				float num2 = metaAi.GetRestrictedArea(_chara.TeamNo).size;
				if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
				{
					num2 *= PLAYER_RESTRICTED_AREA_MAG;
				}
				if (Mathf.Abs(metaAi.GetRestrictedArea(_chara.TeamNo).pos.z - _chara.GetPos().z) <= num2)
				{
					return true;
				}
			}
			else if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.CORNER_KICK)
			{
				float num3 = metaAi.GetRestrictedArea(_chara.TeamNo).size;
				if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
				{
					num3 *= PLAYER_RESTRICTED_AREA_MAG;
				}
				if (CalcManager.Length(haveBallChara.GetPos(), _chara.GetPos()) < num3)
				{
					return true;
				}
			}
			else
			{
				MetaArtificialIntelligence.RestrictedAreaType type = metaAi.GetRestrictedArea(_chara.TeamNo).type;
			}
			return false;
		}
		public CharacterCursor GetCursor(int _teamNo)
		{
			return cursor[_teamNo];
		}
		public CharacterScript GetHaveBallChara()
		{
			return haveBallChara;
		}
		public float GetDistanceFromBall(int _teamNo, int _charaNo)
		{
			return metaAi.GetDistanceFromBall(_teamNo, _charaNo);
		}
		public float GetDistanceFromMyGoal(int _teamNo, int _charaNo)
		{
			return metaAi.GetDistanceFromMyGoal(_teamNo, _charaNo);
		}
		public float GetDistanceFromOpponentGoal(int _teamNo, int _charaNo)
		{
			return metaAi.GetDistanceFromOpponentGoal(_teamNo, _charaNo);
		}
		public CharacterScript GetBallNearOrderList(int _teamNo, int _order)
		{
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, _order)];
		}
		public CharacterScript GetKeeper(int _teamNo)
		{
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (charaList[_teamNo].charas[i].CheckPositionType(GameDataParams.PositionType.GK))
				{
					return charaList[_teamNo].charas[i];
				}
			}
			UnityEngine.Debug.LogError("チ\u30fcム" + _teamNo.ToString() + "にキ\u30fcパ\u30fcが存在しません");
			return null;
		}
		public CharacterScript GetOpponentKeeper(int _teamNo)
		{
			_teamNo = ((_teamNo == 0) ? 1 : 0);
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (charaList[_teamNo].charas[i].CheckPositionType(GameDataParams.PositionType.GK))
				{
					return charaList[_teamNo].charas[i];
				}
			}
			UnityEngine.Debug.LogError("チ\u30fcム" + _teamNo.ToString() + "にキ\u30fcパ\u30fcが存在しません");
			return null;
		}
		public CharacterScript[] GetPositionCenterNearOrderList(int _teamNo)
		{
			CharacterScript[] array = new CharacterScript[charaList[_teamNo].charas.Length];
			float[] array2 = new float[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = charaList[_teamNo].charas[i];
				array2[i] = CalcManager.Length(array[i].GetFormationPos(_local: false, _half: true), SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position);
			}
			for (int j = 0; j < array.Length - 1; j++)
			{
				for (int k = j; k < array.Length; k++)
				{
					if (array2[k] < array2[j])
					{
						float num = array2[j];
						array2[j] = array2[k];
						array2[k] = num;
						CharacterScript characterScript = array[j];
						array[j] = array[k];
						array[k] = characterScript;
					}
				}
			}
			return array;
		}
		public int GetAttackTeamNo()
		{
			if (haveBallChara == null)
			{
				return -1;
			}
			return haveBallChara.TeamNo;
		}
		public CharacterScript GetChara(GameObject _obj)
		{
			for (int i = 0; i < charaList.Length; i++)
			{
				for (int j = 0; j < charaList[i].charas.Length; j++)
				{
					if (charaList[i].charas[j].CheckObj(_obj))
					{
						return charaList[i].charas[j];
					}
				}
			}
			UnityEngine.Debug.LogError("存在しないキャラです : " + _obj.name);
			return null;
		}
		public CharacterScript GetChara(int _teamNo, int _ballNearOrder)
		{
			return charaList[_teamNo].charas[metaAi.GetDistanceFromBallOrder(_teamNo, _ballNearOrder)];
		}
		public CharacterScript GetControlChara(int _playerNo)
		{
			return controlChara[_playerNo];
		}
		public Vector3 GetOpponentOffsideLine(int _teamNo)
		{
			_teamNo = ((_teamNo == 0) ? 1 : 0);
			float num = SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.z * 2f;
			int num2 = 0;
			for (int i = 0; i < charaList[_teamNo].charas.Length; i++)
			{
				if (!charaList[_teamNo].charas[i].CheckPositionType(GameDataParams.PositionType.GK) && charaList[_teamNo].charas[i].GetPos(_isLocal: true).z < num)
				{
					num = charaList[_teamNo].charas[i].GetPos(_isLocal: true).z;
					num2 = i;
				}
			}
			Vector3 vector = charaList[_teamNo].charas[num2].GetPos();
			vector.x = SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position.x;
			_teamNo = ((_teamNo == 0) ? 1 : 0);
			vector = SingletonCustom<FieldManager>.Instance.ConvertLocalPos(vector, _teamNo);
			Vector3 vector2 = SingletonCustom<FieldManager>.Instance.ConvertLocalPos(SingletonCustom<BallManager>.Instance.GetBallPos(), _teamNo);
			if (vector2.z > vector.z)
			{
				vector.z = vector2.z;
			}
			vector.z -= charaList[_teamNo].charas[num2].GetCharaBodySize() * 2f;
			return vector;
		}
		public MetaArtificialIntelligence.InRestrictedArea GetRestrictedArea(int _teamNo)
		{
			return metaAi.GetRestrictedArea(_teamNo);
		}
		public bool IsPlayer(int _playerNo)
		{
			if (_playerNo == -1)
			{
				return false;
			}
			return isPlayer[_playerNo];
		}
		public Vector3 ConvertOptimalPositioning(Vector3 _pos, int _teamNo, GameDataParams.PositionType _positionType)
		{
			Vector3 ballPosPer = metaAi.GetBallPosPer(_teamNo);
			ballPosPer.x += 0.5f;
			Vector3 vector = ballPosPer;
			vector.x = Mathf.Max(1f - Mathf.Abs(vector.x - 0.5f), 0.95f);
			vector.z = Mathf.Max(1f - Mathf.Abs(vector.z - 0.5f), 0.9f);
			Vector3 vector2 = SingletonCustom<FieldManager>.Instance.ConvertLocalPosPer(_pos, _teamNo);
			vector2.x += 0.5f;
			Vector3 posPer = default(Vector3);
			posPer.x = vector.x * vector2.x;
			posPer.z = vector.z * vector2.z;
			posPer.y = 0f;
			if (ballPosPer.x >= 0.5f)
			{
				posPer.x += 1f - vector.x;
			}
			if (ballPosPer.z >= 0.5f)
			{
				posPer.z += 1f - vector.z;
			}
			posPer.x -= 0.5f;
			return SingletonCustom<FieldManager>.Instance.ConvertPosPerToWorld(posPer, _teamNo);
		}
		public Vector3 ConvertRestrictedArea(CharacterScript _chara, Vector3 _pos)
		{
			if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.KEEPER_KEEP)
			{
				if (CheckInRestrictedArea(_chara))
				{
					float num = metaAi.GetRestrictedArea(_chara.TeamNo).size;
					if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
					{
						num *= PLAYER_RESTRICTED_AREA_MAG * 1.1f;
					}
					_pos = haveBallChara.GetPos() + (_pos - haveBallChara.GetPos()).normalized * num;
				}
			}
			else if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.GOAL_KICK)
			{
				if (CheckInRestrictedArea(_chara))
				{
					float num2 = metaAi.GetRestrictedArea(_chara.TeamNo).size;
					if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
					{
						num2 *= PLAYER_RESTRICTED_AREA_MAG * 1.1f;
					}
					_pos.z = metaAi.GetRestrictedArea(_chara.TeamNo).pos.z + Mathf.Sign(_chara.GetPos().z - metaAi.GetRestrictedArea(_chara.TeamNo).pos.z) * num2;
				}
			}
			else if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.CORNER_KICK)
			{
				if (CheckInRestrictedArea(_chara))
				{
					float num3 = metaAi.GetRestrictedArea(_chara.TeamNo).size;
					if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
					{
						num3 *= PLAYER_RESTRICTED_AREA_MAG * 1.1f;
					}
					_pos = haveBallChara.GetPos() + (_pos - haveBallChara.GetPos()).normalized * num3;
				}
			}
			else if (metaAi.GetRestrictedArea(_chara.TeamNo).type == MetaArtificialIntelligence.RestrictedAreaType.ALLY_KEEP && CheckInRestrictedArea(_chara))
			{
				float num4 = metaAi.GetRestrictedArea(_chara.TeamNo).size;
				if (_chara.PlayerNo != -1 && isPlayerAutoControl[_chara.PlayerNo] && _chara == controlChara[_chara.PlayerNo])
				{
					num4 *= PLAYER_RESTRICTED_AREA_MAG * 1.1f;
				}
				_pos = haveBallChara.GetPos() + (_pos - haveBallChara.GetPos()).normalized * num4;
			}
			return _pos;
		}
		private void OnDrawGizmos()
		{
			if (charaList == null)
			{
				return;
			}
			Gizmos.color = ColorPalet.blue;
			Vector3 opponentOffsideLine = GetOpponentOffsideLine(0);
			Gizmos.DrawLine(opponentOffsideLine + Vector3.left * 5f, opponentOffsideLine + Vector3.right * 5f);
			Gizmos.color = ColorPalet.red;
			Vector3 opponentOffsideLine2 = GetOpponentOffsideLine(1);
			Gizmos.DrawLine(opponentOffsideLine2 + Vector3.left * 5f, opponentOffsideLine2 + Vector3.right * 5f);
			for (int i = 0; i < charaList.Length; i++)
			{
				CharacterScript characterScript = SearchDefenseCover(i);
				if (characterScript != null)
				{
					Gizmos.color = ColorPalet.green;
					Gizmos.DrawWireSphere(characterScript.GetPos(), 1f);
				}
				characterScript = SearchBallNearestDefense(i);
				if (characterScript != null)
				{
					Gizmos.color = ColorPalet.lightgreen;
					Gizmos.DrawWireSphere(characterScript.GetPos(), 1f);
				}
			}
			metaAi.DrawCharaDistribution();
			Gizmos.color = ColorPalet.pink;
			Gizmos.DrawWireSphere(SingletonCustom<MainCharacterManager>.Instance.CharaList[0].charas[metaAi.GetDistanceFromBallOrder(0, 0)].transform.position, 1f);
		}
	}
}
