using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpearBattle_GameManager : SingletonCustom<SpearBattle_GameManager>
{
	public enum GameState
	{
		Init,
		Select,
		Battle,
		End
	}
	[Serializable]
	public class BattleData
	{
		public int battleNo;
		public CharaData leftCharaData;
		public CharaData rightCharaData;
		public int winnerNo = -1;
		public bool IsCpuBattle
		{
			get
			{
				if (!leftCharaData.isPlayer)
				{
					return !rightCharaData.isPlayer;
				}
				return false;
			}
		}
		public CharaData GetWinnerCharaData()
		{
			switch (winnerNo)
			{
			case 0:
				return leftCharaData;
			case 1:
				return rightCharaData;
			default:
				return null;
			}
		}
		public CharaData GetLoserCharaData()
		{
			switch (winnerNo)
			{
			case 0:
				return rightCharaData;
			case 1:
				return leftCharaData;
			default:
				return null;
			}
		}
	}
	[Serializable]
	public class CharaData
	{
		public int charaNo;
		public int playerNo;
		public int styleCharaNo;
		public int tournamentNo;
		public bool isPlayer;
		public int winNum;
		public CharaData(int _charaNo, int _playerNo, int _styleCharaNo, int _tournamentNo)
		{
			charaNo = _charaNo;
			playerNo = _playerNo;
			styleCharaNo = _styleCharaNo;
			tournamentNo = _tournamentNo;
			isPlayer = (_playerNo < 4);
		}
	}
	private const int CHARA_NUM = 4;
	private GameState gameState;
	private BattleData[] battleDataArray = new BattleData[4];
	private CharaData[] charaDataArray = new CharaData[4];
	private int[] tournamentCharaNoArray = new int[4];
	private string[] tournamentVSTeamData = new string[2];
	private SpearBattle_Define.SkillType[] leftSkillArray = new SpearBattle_Define.SkillType[5];
	private SpearBattle_Define.SkillType[] rightSkillArray = new SpearBattle_Define.SkillType[5];
	private int playerNum;
	private int battleNo;
	private bool isGameStart;
	private int aiStrength;
	[SerializeField]
	private RankingResultManager resultManager;
	[SerializeField]
	private GameObject selectAnchorObj;
	[SerializeField]
	private GameObject battleAnchorObj;
	[SerializeField]
	private Transform charaAnchor;
	[SerializeField]
	private SpearBattle_CharacterScript leftChara;
	[SerializeField]
	private SpearBattle_CharacterScript rightChara;
	[SerializeField]
	private Material[] spearAndShieldMaterials;
	public int PlayerNum => playerNum;
	public int AiStrength => aiStrength;
	public void Init()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		switch (playerNum)
		{
		case 1:
			tournamentCharaNoArray = new int[4]
			{
				0,
				1,
				2,
				3
			};
			break;
		case 2:
			tournamentCharaNoArray = new int[4]
			{
				0,
				2,
				1,
				3
			};
			break;
		case 3:
		case 4:
			tournamentCharaNoArray = new int[4]
			{
				0,
				1,
				2,
				3
			};
			for (int i = 0; i < 50; i++)
			{
				int num = UnityEngine.Random.Range(0, 4);
				int num2 = UnityEngine.Random.Range(0, 4);
				int num3 = tournamentCharaNoArray[num];
				tournamentCharaNoArray[num] = tournamentCharaNoArray[num2];
				tournamentCharaNoArray[num2] = num3;
			}
			break;
		}
		for (int j = 0; j < charaDataArray.Length; j++)
		{
			charaDataArray[j] = new CharaData(j, playerGroupList[j][0], SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerGroupList[j][0]], GetTournamentNo(j));
		}
		for (int k = 0; k < battleDataArray.Length; k++)
		{
			battleDataArray[k] = new BattleData();
			battleDataArray[k].battleNo = k;
			if (k < 2)
			{
				battleDataArray[k].leftCharaData = charaDataArray[tournamentCharaNoArray[k * 2]];
				battleDataArray[k].rightCharaData = charaDataArray[tournamentCharaNoArray[k * 2 + 1]];
			}
		}
		tournamentVSTeamData[0] = (battleDataArray[0].leftCharaData.charaNo + 1).ToString() + "-" + (battleDataArray[0].rightCharaData.charaNo + 1).ToString();
		tournamentVSTeamData[1] = (battleDataArray[1].leftCharaData.charaNo + 1).ToString() + "-" + (battleDataArray[1].rightCharaData.charaNo + 1).ToString();
		SingletonCustom<SpearBattle_UIManager>.Instance.SetTournamentTeamData();
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		InitSkillArray();
		ChangeSelect();
		OpenGameDirection();
	}
	public void InitSkillArray()
	{
		for (int i = 0; i < leftSkillArray.Length; i++)
		{
			leftSkillArray[i] = SpearBattle_Define.SkillType.Empty;
		}
		for (int j = 0; j < rightSkillArray.Length; j++)
		{
			rightSkillArray[j] = SpearBattle_Define.SkillType.Empty;
		}
	}
	private void OpenGameDirection()
	{
		StartCoroutine(_OpenGameDirection());
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
			{
				SingletonCustom<SpearBattle_UIManager>.Instance.HideAnnounceText();
				LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
				{
					GameStart();
				});
			});
		});
	}
	public void UpdateMethod()
	{
		switch (gameState)
		{
		case GameState.Select:
			SingletonCustom<SpearBattle_SelectManager>.Instance.UpdateMethod();
			break;
		case GameState.Battle:
			SingletonCustom<SpearBattle_BattleManager>.Instance.UpdateMethod();
			break;
		}
	}
	public SpearBattle_Define.SkillType GetSkill(bool _isLeft, int _order)
	{
		if (!_isLeft)
		{
			return rightSkillArray[_order];
		}
		return leftSkillArray[_order];
	}
	public SpearBattle_Define.SkillType[] GetSkillArray(bool _isLeft)
	{
		if (!_isLeft)
		{
			return rightSkillArray;
		}
		return leftSkillArray;
	}
	public BattleData GetNowBattleData()
	{
		return battleDataArray[battleNo];
	}
	public BattleData GetBattleData(int _battleNo)
	{
		return battleDataArray[_battleNo];
	}
	public int GetTournamentNo(int _charaNo)
	{
		for (int i = 0; i < tournamentCharaNoArray.Length; i++)
		{
			if (_charaNo == tournamentCharaNoArray[i])
			{
				return i;
			}
		}
		return 0;
	}
	public SpearBattle_CharacterScript GetChara(bool _isLeft)
	{
		if (!_isLeft)
		{
			return rightChara;
		}
		return leftChara;
	}
	public string[] GetTournamentVSTeamData()
	{
		return tournamentVSTeamData;
	}
	public void SetSkill(bool _isLeft, int _order, SpearBattle_Define.SkillType _skill)
	{
		if (_isLeft)
		{
			leftSkillArray[_order] = _skill;
		}
		else
		{
			rightSkillArray[_order] = _skill;
		}
	}
	public void SetSkillArray(bool _isLeft, SpearBattle_Define.SkillType[] _skillArray)
	{
		if (_isLeft)
		{
			leftSkillArray = _skillArray;
		}
		else
		{
			rightSkillArray = _skillArray;
		}
	}
	private void NowBattleCharaStyleSetting()
	{
		leftChara.SetCharaStyle(battleDataArray[battleNo].leftCharaData);
		rightChara.SetCharaStyle(battleDataArray[battleNo].rightCharaData);
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[battleDataArray[battleNo].leftCharaData.playerNo];
		leftChara.SetSpearAndShieldMaterial(spearAndShieldMaterials[num]);
		num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[battleDataArray[battleNo].rightCharaData.playerNo];
		rightChara.SetSpearAndShieldMaterial(spearAndShieldMaterials[num]);
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<CommonStartSimple>.Instance.Show(delegate
		{
			SingletonCustom<SpearBattle_SelectManager>.Instance.SelectStart();
			SingletonCustom<SpearBattle_BattleManager>.Instance.BattleStart();
		});
	}
	public void BattleDraw()
	{
		SingletonCustom<SpearBattle_BattleResultSimple>.Instance.DrawSetting();
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<SpearBattle_BattleResultSimple>.Instance.Show(delegate
			{
				SingletonCustom<SpearBattle_UIManager>.Instance.Fade(0.5f, 0f, delegate
				{
					ChangeSelect();
				}, delegate
				{
					LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
					{
						SingletonCustom<CommonStartSimple>.Instance.Show(delegate
						{
							if (battleDataArray[battleNo].IsCpuBattle)
							{
								SingletonCustom<SpearBattle_UIManager>.Instance.ShowSkip();
							}
							else
							{
								SingletonCustom<SpearBattle_UIManager>.Instance.HideSkip();
							}
							SingletonCustom<SpearBattle_SelectManager>.Instance.SelectStart();
							SingletonCustom<SpearBattle_BattleManager>.Instance.BattleStart();
						});
					});
				});
			});
		});
	}
	public void BattleEnd(bool _isWinnerLeft)
	{
		BattleEndDataSetting(_isWinnerLeft);
		if (battleDataArray[battleNo].IsCpuBattle || battleDataArray[battleNo].GetWinnerCharaData().isPlayer)
		{
			SingletonCustom<SpearBattle_BattleResultSimple>.Instance.WinSetting(battleDataArray[battleNo].GetWinnerCharaData().playerNo);
		}
		else
		{
			SingletonCustom<SpearBattle_BattleResultSimple>.Instance.LoseSetting(battleDataArray[battleNo].GetLoserCharaData().playerNo);
		}
		battleNo++;
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<SpearBattle_BattleResultSimple>.Instance.Show(delegate
			{
				if (battleNo < 4)
				{
					SingletonCustom<SpearBattle_UIManager>.Instance.StartTournamentUIAnimation(_isWinnerLeft, null);
					LeanTween.delayedCall(base.gameObject, 5.75f, (Action)delegate
					{
						SingletonCustom<SpearBattle_UIManager>.Instance.Fade(0.5f, 0f, delegate
						{
							ChangeSelect();
							SingletonCustom<SpearBattle_UIManager>.Instance.NextSettingTournament();
						}, delegate
						{
							LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
							{
								SingletonCustom<CommonStartSimple>.Instance.Show(delegate
								{
									if (battleDataArray[battleNo].IsCpuBattle)
									{
										SingletonCustom<SpearBattle_UIManager>.Instance.ShowSkip();
									}
									else
									{
										SingletonCustom<SpearBattle_UIManager>.Instance.HideSkip();
									}
									SingletonCustom<SpearBattle_SelectManager>.Instance.SelectStart();
									SingletonCustom<SpearBattle_BattleManager>.Instance.BattleStart();
								});
							});
						});
					});
				}
				else
				{
					SingletonCustom<SpearBattle_UIManager>.Instance.StartTournamentUIAnimation(_isWinnerLeft, delegate
					{
						GameEnd();
					});
				}
			});
		});
	}
	private void BattleEndDataSetting(bool _isWinnerLeft)
	{
		battleDataArray[battleNo].winnerNo = ((!_isWinnerLeft) ? 1 : 0);
		if (battleNo < 2)
		{
			CharaData charaData = _isWinnerLeft ? battleDataArray[battleNo].leftCharaData : battleDataArray[battleNo].rightCharaData;
			CharaData charaData2 = (!_isWinnerLeft) ? battleDataArray[battleNo].leftCharaData : battleDataArray[battleNo].rightCharaData;
			if (battleNo == 0)
			{
				battleDataArray[2].leftCharaData = charaData2;
				battleDataArray[3].leftCharaData = charaData;
			}
			else
			{
				if (!battleDataArray[2].leftCharaData.isPlayer && charaData2.isPlayer)
				{
					battleDataArray[2].rightCharaData = battleDataArray[2].leftCharaData;
					battleDataArray[2].leftCharaData = charaData2;
				}
				else
				{
					battleDataArray[2].rightCharaData = charaData2;
				}
				if (!battleDataArray[3].leftCharaData.isPlayer && charaData.isPlayer)
				{
					battleDataArray[3].rightCharaData = battleDataArray[3].leftCharaData;
					battleDataArray[3].leftCharaData = charaData;
				}
				else
				{
					battleDataArray[3].rightCharaData = charaData;
				}
			}
			battleDataArray[battleNo].GetWinnerCharaData().winNum += 2;
		}
		else
		{
			battleDataArray[battleNo].GetWinnerCharaData().winNum++;
		}
	}
	public void ChangeSelect()
	{
		if (gameState != GameState.Select && gameState != GameState.End)
		{
			gameState = GameState.Select;
			selectAnchorObj.SetActive(value: true);
			battleAnchorObj.SetActive(value: false);
			SingletonCustom<SpearBattle_SelectManager>.Instance.Init();
			SingletonCustom<SpearBattle_UIManager>.Instance.ChangeSelect();
			leftChara.Init();
			rightChara.Init();
			NowBattleCharaStyleSetting();
			ChangeBattle();
		}
	}
	public void ChangeBattle()
	{
		if (gameState != GameState.Battle && gameState != GameState.End)
		{
			gameState = GameState.Battle;
			selectAnchorObj.SetActive(value: false);
			battleAnchorObj.SetActive(value: true);
			SingletonCustom<SpearBattle_BattleManager>.Instance.Init();
			SingletonCustom<SpearBattle_UIManager>.Instance.ChangeBattle();
		}
	}
	public void GameEnd()
	{
		gameState = GameState.End;
		StartCoroutine(_GameEnd());
	}
	private IEnumerator _GameEnd()
	{
		CheckTrophy();
		int num = 4;
		int[] array = new int[num];
		int[] array2 = new int[num];
		int[] array3 = new int[num];
		string[] tournamentMatch = new string[2]
		{
			tournamentVSTeamData[0],
			tournamentVSTeamData[1]
		};
		for (int i = 0; i < num; i++)
		{
			array[i] = charaDataArray[i].winNum;
			array2[i] = charaDataArray[i].playerNo;
			array3[i] = battleDataArray[i].GetWinnerCharaData().charaNo;
		}
		ResultGameDataParams.SetRecord_Int_Tournament(array, array2, array3, tournamentMatch);
		resultManager.ShowResult_Tournament();
		yield return new WaitForSeconds(1.5f);
		selectAnchorObj.SetActive(value: false);
		battleAnchorObj.SetActive(value: false);
	}
	private void CheckTrophy()
	{
		if (playerNum == 1 && battleDataArray[3].winnerNo == 0 && battleDataArray[3].leftCharaData.isPlayer)
		{
			if (aiStrength == 0)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.GET_BALL);
			}
			if (aiStrength == 1)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.GET_BALL);
			}
			if (aiStrength == 2)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.GET_BALL);
			}
		}
	}
	private void DebugEnd()
	{
		if (gameState == GameState.End)
		{
			return;
		}
		gameState = GameState.End;
		LeanTween.cancel(base.gameObject);
		for (int i = 0; i < battleDataArray.Length; i++)
		{
			if (battleDataArray[i].winnerNo == -1)
			{
				BattleEndDataSetting(UnityEngine.Random.Range(0, 2) == 1);
				battleNo++;
			}
		}
		GameEnd();
	}
}
