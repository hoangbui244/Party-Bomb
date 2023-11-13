using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Curling_GameManager : SingletonCustom<Curling_GameManager>
{
	public enum State
	{
		COIN_TOSS,
		PLAY_START,
		PREP_THROW,
		THROW,
		SWEEP,
		HOUSE_SWEEP,
		PLAY_END,
		CHANGE_TURN
	}
	public enum Team
	{
		TEAM_A,
		TEAM_B
	}
	public enum PointGetTeam
	{
		TEAM_A,
		TEAM_B,
		DRAW
	}
	[Serializable]
	public struct TeamInfo
	{
		[Header("投げた回数")]
		public int throwCnt;
		[Header("ポイント")]
		public int point;
		[Header("石")]
		public Curling_Stone[] arrayStone;
		[Header("プレイヤ\u30fc")]
		public Curling_Player[] arrayPlayer;
		[Header("投げる回数ごとの投げるプレイヤ\u30fcの順番")]
		public int[] arrayThrowPlayerNo;
		[Header("投げる回数ごとのハウス付近でこするプレイヤ\u30fcの順番")]
		public int[] arrayHouseSweepPlayerNo;
	}
	[Serializable]
	private struct CharacterAcnhor
	{
		public Transform[] arrayCharaAnchor;
	}
	[SerializeField]
	[Header("勝敗リザルト")]
	private WinOrLoseResultManager winOrLoseResultManager;
	[SerializeField]
	[Header("予測する石")]
	private Curling_PredictStone predictStone;
	[SerializeField]
	[Header("予測する石のライン")]
	private LineRenderer predictLine;
	private State state;
	private Team turnTeam;
	[SerializeField]
	[Header("チ\u30fcム情報")]
	private TeamInfo[] arrayTeamInfo;
	[NonReorderable]
	private List<int> teamAPlayerNoList = new List<int>();
	[NonReorderable]
	private List<int> teamBPlayerNoList = new List<int>();
	[SerializeField]
	[Header("石のPhysicMaterial")]
	private PhysicMaterial stonePhysicMaterial;
	[SerializeField]
	[Header("石のPhysicMaterialのdynamicFriction値")]
	private float stonePhysicMaterial_DynamicFriction;
	[SerializeField]
	[Header("石のPhysicMaterialのstaticFriction値")]
	private float stonePhysicMaterial_StaticFriction;
	[SerializeField]
	[Header("カメラクラス")]
	private Curling_Camera camera;
	[SerializeField]
	[Header("石のチ\u30fcム別マテリアル")]
	private Material[] arrayStoneMaterial;
	[SerializeField]
	[Header("各キャラのブラシマテリアル")]
	private Material[] arrayBrushMaterial;
	[SerializeField]
	[Header("石を投げる基本パワ\u30fc")]
	private float STONE_THROW_BASE_POWER;
	[SerializeField]
	[Header("石を投げる基本パワ\u30fcに加算する最大補正パワ\u30fc")]
	private float STONE_THROW_DIFF_MAX_POWER;
	[SerializeField]
	[Header("石を最大パワ\u30fcで投げた時に曲がる距離")]
	private float STONE_THROW_CURVE_VECTOR_DIFF;
	private readonly float THROW_MOVE_DISTANCE = 9.15f;
	[SerializeField]
	[Header("石を投げるまでの時間（投げるキャラが移動を終える時間）")]
	private float STONE_THROW_MOVE_TIME;
	[SerializeField]
	[Header("カメラが遷移するまでの待機時間（投げる状態→こする状態）")]
	private float CAMERA_CHANGE_WAIT_TIME;
	[SerializeField]
	[Header("カメラの遷移時間（投げる状態→こする状態）")]
	private float CAMERA_CHANGE_TIME;
	[SerializeField]
	[Header("キャラが石を追従する時の移動時間")]
	private float CHARA_CHANGE_SWEEP_TIME;
	[SerializeField]
	[Header("プレイするチ\u30fcムのキャラアンカ\u30fc")]
	private Transform[] arrayPlayTeamCharaAnchor;
	[SerializeField]
	[Header("観戦するチ\u30fcムのキャラアンカ\u30fc")]
	private CharacterAcnhor[] arrayViewingTeam;
	[SerializeField]
	[Header("こするキャラを石に追従させるアンカ\u30fc")]
	private Transform sweepStartAnchor;
	[SerializeField]
	[Header("こするキャラが石を追従するのをやめるアンカ\u30fc")]
	private Transform sweepStopAnchor;
	[SerializeField]
	[Header("こするキャラが石を追従するのをやめて、掃けるアンカ\u30fc")]
	private Transform[] arraySweepMoveEndAnchor;
	[SerializeField]
	[Header("ハウス付近でこするキャラを石に追従させるアンカ\u30fc")]
	private Transform houseSweepStartAnchor;
	private bool isGameStart;
	private bool isGameEnd;
	private int gameCnt;
	private bool isSweepStart;
	private bool isSweepStop;
	private bool isHouseSweepStart;
	private bool isThrowStone;
	private Vector3 charaToStoneVec;
	[SerializeField]
	[Header("カメラの移動限界のアンカ\u30fc")]
	private Transform cameraMoveLimitAnchor;
	private bool isSkip;
	[SerializeField]
	[Header("スキップ時のフェ\u30fcドが開けるアンカ\u30fc")]
	private Transform skipFadeOutAnchor;
	[NonReorderable]
	private int[] pointCalcPointTmp = new int[2];
	[SerializeField]
	[Header("カメラをティ\u30fcの上に移動させる最大時間")]
	private float CAMERA_NEAR_TEA_Z_TIME;
	[SerializeField]
	[Header("カメラをハウスに近づける時間")]
	private float CAMERA_NEAR_HOUSE_TIME;
	public void Init()
	{
		stonePhysicMaterial.dynamicFriction = stonePhysicMaterial_DynamicFriction;
		stonePhysicMaterial.staticFriction = stonePhysicMaterial_StaticFriction;
		Curling_Define.UserType[] array = new Curling_Define.UserType[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - SingletonCustom<GameSettingManager>.Instance.PlayerNum];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (Curling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[SingletonCustom<GameSettingManager>.Instance.PlayerNum + i][0];
		}
		array = CalcManager.ShuffleList(array);
		Curling_Define.UserType[] array2 = new Curling_Define.UserType[4];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = (Curling_Define.UserType)(7 - (3 - array.Length) + j);
		}
		array2 = CalcManager.ShuffleList(array2);
		int num = 0;
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			for (int num4 = 0; num4 < arrayTeamInfo.Length; num4++)
			{
				for (int num5 = 0; num5 < arrayTeamInfo[num4].arrayPlayer.Length; num5++)
				{
					if ((num4 == 0 && num5 == 0) || (num4 == 0 && num5 == 3))
					{
						arrayTeamInfo[num4].arrayPlayer[num5].Init((Team)num4, Curling_Define.UserType.PLAYER_1, num5);
						if (num4 == 0 && num5 == 0)
						{
							if (!teamAPlayerNoList.Contains(0))
							{
								teamAPlayerNoList.Add(0);
								arrayTeamInfo[num4].arrayPlayer[num5].SetCharaIdx(0);
							}
						}
						else if (!teamAPlayerNoList.Contains((int)array[0]))
						{
							teamAPlayerNoList.Add((int)array[0]);
							arrayTeamInfo[num4].arrayPlayer[num5].SetCharaIdx((int)array[0]);
						}
					}
					else if ((num4 == 1 && num5 == 0) || (num4 == 1 && num5 == 3))
					{
						arrayTeamInfo[num4].arrayPlayer[num5].Init((Team)num4, Curling_Define.UserType.CPU_4, num5);
						if (num4 == 1 && num5 == 0)
						{
							if (!teamBPlayerNoList.Contains((int)array[1]))
							{
								teamBPlayerNoList.Add((int)array[1]);
								arrayTeamInfo[num4].arrayPlayer[num5].SetCharaIdx((int)array[1]);
							}
						}
						else if (!teamBPlayerNoList.Contains((int)array[2]))
						{
							teamBPlayerNoList.Add((int)array[2]);
							arrayTeamInfo[num4].arrayPlayer[num5].SetCharaIdx((int)array[2]);
						}
					}
					else
					{
						arrayTeamInfo[num4].arrayPlayer[num5].Init((Team)num4, (Curling_Define.UserType)(num5 + 3 + 4 * num4), num5);
						arrayTeamInfo[num4].arrayPlayer[num5].SetCharaIdx((int)array2[num]);
						num++;
					}
					arrayTeamInfo[num4].arrayPlayer[num5].SetBrushMaterial(arrayBrushMaterial[num4]);
				}
			}
			break;
		case 2:
			for (int num6 = 0; num6 < arrayTeamInfo.Length; num6++)
			{
				for (int num7 = 0; num7 < arrayTeamInfo[num6].arrayPlayer.Length; num7++)
				{
					if ((num6 == 0 && num7 == 0) || (num6 == 0 && num7 == 3))
					{
						arrayTeamInfo[num6].arrayPlayer[num7].Init((Team)num6, Curling_Define.UserType.PLAYER_1, num7);
						if (num6 == 0 && num7 == 0)
						{
							if (!teamAPlayerNoList.Contains(0))
							{
								teamAPlayerNoList.Add(0);
								arrayTeamInfo[num6].arrayPlayer[num7].SetCharaIdx(0);
							}
						}
						else if (!teamAPlayerNoList.Contains((int)array[0]))
						{
							teamAPlayerNoList.Add((int)array[0]);
							arrayTeamInfo[num6].arrayPlayer[num7].SetCharaIdx((int)array[0]);
						}
					}
					else if ((num6 == 1 && num7 == 0) || (num6 == 1 && num7 == 3))
					{
						arrayTeamInfo[num6].arrayPlayer[num7].Init((Team)num6, Curling_Define.UserType.PLAYER_2, num7);
						if (num6 == 1 && num7 == 0)
						{
							if (!teamBPlayerNoList.Contains(1))
							{
								teamBPlayerNoList.Add(1);
								arrayTeamInfo[num6].arrayPlayer[num7].SetCharaIdx(1);
							}
						}
						else if (!teamBPlayerNoList.Contains((int)array[1]))
						{
							teamBPlayerNoList.Add((int)array[1]);
							arrayTeamInfo[num6].arrayPlayer[num7].SetCharaIdx((int)array[1]);
						}
					}
					else
					{
						arrayTeamInfo[num6].arrayPlayer[num7].Init((Team)num6, (Curling_Define.UserType)(num7 + 3 + 4 * num6), num7);
						arrayTeamInfo[num6].arrayPlayer[num7].SetCharaIdx((int)array2[num]);
						num++;
					}
					arrayTeamInfo[num6].arrayPlayer[num7].SetBrushMaterial(arrayBrushMaterial[num6]);
				}
			}
			break;
		case 3:
		{
			Curling_Define.UserType[] array3 = new Curling_Define.UserType[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
			for (int n = 0; n < array3.Length; n++)
			{
				array3[n] = (Curling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[n][0];
			}
			array3 = CalcManager.ShuffleList(array3);
			for (int num2 = 0; num2 < arrayTeamInfo.Length; num2++)
			{
				for (int num3 = 0; num3 < arrayTeamInfo[num2].arrayPlayer.Length; num3++)
				{
					if (num2 == 0 && num3 == 0)
					{
						arrayTeamInfo[num2].arrayPlayer[num3].Init((Team)num2, array3[0], num3);
						if (!teamAPlayerNoList.Contains((int)array3[0]))
						{
							teamAPlayerNoList.Add((int)array3[0]);
							arrayTeamInfo[num2].arrayPlayer[num3].SetCharaIdx((int)array3[0]);
						}
					}
					else if (num2 == 0 && num3 == 3)
					{
						arrayTeamInfo[num2].arrayPlayer[num3].Init((Team)num2, array3[1], num3);
						if (!teamAPlayerNoList.Contains((int)array3[1]))
						{
							teamAPlayerNoList.Add((int)array3[1]);
							arrayTeamInfo[num2].arrayPlayer[num3].SetCharaIdx((int)array3[1]);
						}
					}
					else if ((num2 == 1 && num3 == 0) || (num2 == 1 && num3 == 3))
					{
						arrayTeamInfo[num2].arrayPlayer[num3].Init((Team)num2, array3[2], num3);
						if (num2 == 1 && num3 == 0)
						{
							if (!teamBPlayerNoList.Contains((int)array3[2]))
							{
								teamBPlayerNoList.Add((int)array3[2]);
								arrayTeamInfo[num2].arrayPlayer[num3].SetCharaIdx((int)array3[2]);
							}
						}
						else if (!teamBPlayerNoList.Contains((int)array[0]))
						{
							teamBPlayerNoList.Add((int)array[0]);
							arrayTeamInfo[num2].arrayPlayer[num3].SetCharaIdx((int)array[0]);
						}
					}
					else
					{
						arrayTeamInfo[num2].arrayPlayer[num3].Init((Team)num2, (Curling_Define.UserType)(num3 + 3 + 4 * num2), num3);
						arrayTeamInfo[num2].arrayPlayer[num3].SetCharaIdx((int)array2[num]);
						num++;
					}
					arrayTeamInfo[num2].arrayPlayer[num3].SetBrushMaterial(arrayBrushMaterial[num2]);
				}
			}
			break;
		}
		case 4:
		{
			Curling_Define.UserType[] array3 = new Curling_Define.UserType[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k] = (Curling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[k][0];
			}
			array3 = CalcManager.ShuffleList(array3);
			for (int l = 0; l < arrayTeamInfo.Length; l++)
			{
				for (int m = 0; m < arrayTeamInfo[l].arrayPlayer.Length; m++)
				{
					if (l == 0 && m == 0)
					{
						arrayTeamInfo[l].arrayPlayer[m].Init((Team)l, array3[0], m);
						if (!teamAPlayerNoList.Contains((int)array3[0]))
						{
							teamAPlayerNoList.Add((int)array3[0]);
							arrayTeamInfo[l].arrayPlayer[m].SetCharaIdx((int)array3[0]);
						}
					}
					else if (l == 0 && m == 3)
					{
						arrayTeamInfo[l].arrayPlayer[m].Init((Team)l, array3[1], m);
						if (!teamAPlayerNoList.Contains((int)array3[1]))
						{
							teamAPlayerNoList.Add((int)array3[1]);
							arrayTeamInfo[l].arrayPlayer[m].SetCharaIdx((int)array3[1]);
						}
					}
					else if (l == 1 && m == 0)
					{
						arrayTeamInfo[l].arrayPlayer[m].Init((Team)l, array3[2], m);
						if (!teamBPlayerNoList.Contains((int)array3[2]))
						{
							teamBPlayerNoList.Add((int)array3[2]);
							arrayTeamInfo[l].arrayPlayer[m].SetCharaIdx((int)array3[2]);
						}
					}
					else if (l == 1 && m == 3)
					{
						arrayTeamInfo[l].arrayPlayer[m].Init((Team)l, array3[3], m);
						if (!teamBPlayerNoList.Contains((int)array3[3]))
						{
							teamBPlayerNoList.Add((int)array3[3]);
							arrayTeamInfo[l].arrayPlayer[m].SetCharaIdx((int)array3[3]);
						}
					}
					else
					{
						arrayTeamInfo[l].arrayPlayer[m].Init((Team)l, (Curling_Define.UserType)(m + 3 + 4 * l), m);
						arrayTeamInfo[l].arrayPlayer[m].SetCharaIdx((int)array2[num]);
						num++;
					}
					arrayTeamInfo[l].arrayPlayer[m].SetBrushMaterial(arrayBrushMaterial[l]);
				}
			}
			break;
		}
		}
		for (int num8 = 0; num8 < arrayTeamInfo.Length; num8++)
		{
			arrayTeamInfo[num8].arrayThrowPlayerNo = new int[Curling_Define.THROW_CNT];
			arrayTeamInfo[num8].arrayHouseSweepPlayerNo = new int[Curling_Define.THROW_CNT];
		}
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
		case 2:
			for (int num11 = 0; num11 < arrayTeamInfo.Length; num11++)
			{
				for (int num12 = 0; num12 < arrayTeamInfo[num11].arrayPlayer.Length; num12++)
				{
					arrayTeamInfo[num11].arrayThrowPlayerNo[num12] = arrayTeamInfo[num11].arrayPlayer[0].GetPlayerIdx();
					arrayTeamInfo[num11].arrayHouseSweepPlayerNo[num12] = arrayTeamInfo[num11].arrayPlayer[arrayTeamInfo[num11].arrayPlayer.Length - 1].GetPlayerIdx();
				}
			}
			break;
		case 3:
		{
			int[] array4 = new int[Curling_Define.SAME_TEAM_PLAYER_CNT];
			array4[0] = arrayTeamInfo[0].arrayPlayer[0].GetPlayerIdx();
			array4[1] = arrayTeamInfo[0].arrayPlayer[arrayTeamInfo[1].arrayPlayer.Length - 1].GetPlayerIdx();
			array4 = CalcManager.ShuffleList(array4);
			for (int num13 = 0; num13 < arrayTeamInfo.Length; num13++)
			{
				for (int num14 = 0; num14 < arrayTeamInfo[num13].arrayPlayer.Length; num14++)
				{
					if (num13 == 0)
					{
						arrayTeamInfo[num13].arrayThrowPlayerNo[num14] = array4[(num14 % 2 != 0) ? 1 : 0];
						arrayTeamInfo[num13].arrayHouseSweepPlayerNo[num14] = array4[(num14 % 2 == 0) ? 1 : 0];
					}
					else
					{
						arrayTeamInfo[num13].arrayThrowPlayerNo[num14] = arrayTeamInfo[num13].arrayPlayer[0].GetPlayerIdx();
						arrayTeamInfo[num13].arrayHouseSweepPlayerNo[num14] = arrayTeamInfo[num13].arrayPlayer[arrayTeamInfo[num13].arrayPlayer.Length - 1].GetPlayerIdx();
					}
				}
			}
			break;
		}
		case 4:
		{
			int[] array4 = new int[Curling_Define.SAME_TEAM_PLAYER_CNT];
			for (int num9 = 0; num9 < arrayTeamInfo.Length; num9++)
			{
				array4[0] = arrayTeamInfo[num9].arrayPlayer[0].GetPlayerIdx();
				array4[1] = arrayTeamInfo[num9].arrayPlayer[arrayTeamInfo[1].arrayPlayer.Length - 1].GetPlayerIdx();
				array4 = CalcManager.ShuffleList(array4);
				for (int num10 = 0; num10 < arrayTeamInfo[num9].arrayPlayer.Length; num10++)
				{
					arrayTeamInfo[num9].arrayThrowPlayerNo[num10] = array4[(num10 % 2 != 0) ? 1 : 0];
					arrayTeamInfo[num9].arrayHouseSweepPlayerNo[num10] = array4[(num10 % 2 == 0) ? 1 : 0];
				}
			}
			break;
		}
		}
		for (int num15 = 0; num15 < arrayTeamInfo.Length; num15++)
		{
			for (int num16 = 0; num16 < arrayTeamInfo[num15].arrayPlayer.Length; num16++)
			{
				arrayTeamInfo[num15].arrayPlayer[num16].SetCharacterStyle();
			}
		}
		for (int num17 = 0; num17 < arrayTeamInfo.Length; num17++)
		{
			for (int num18 = 0; num18 < arrayTeamInfo[num17].arrayPlayer.Length; num18++)
			{
				if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && num17 == 0) || SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4)
				{
					if (num18 == 0 || num18 == 3)
					{
						arrayTeamInfo[num17].arrayPlayer[arrayTeamInfo[num17].arrayThrowPlayerNo[num18]].SetCharacterBibsStyle(num18);
					}
					else
					{
						arrayTeamInfo[num17].arrayPlayer[num18].SetCharacterBibsStyle(num18);
					}
				}
				else
				{
					arrayTeamInfo[num17].arrayPlayer[num18].SetCharacterBibsStyle(num18);
				}
			}
		}
		camera.Init();
		for (int num19 = 0; num19 < arrayTeamInfo.Length; num19++)
		{
			for (int num20 = 0; num20 < arrayTeamInfo[num19].arrayStone.Length; num20++)
			{
				arrayTeamInfo[num19].arrayStone[num20].Init((Team)num19);
				arrayTeamInfo[num19].arrayStone[num20].SetMaterial(arrayStoneMaterial[num19]);
			}
		}
		for (int num21 = 0; num21 < arrayTeamInfo.Length; num21++)
		{
			Curling_Define.UserType userType = arrayTeamInfo[num21].arrayPlayer[arrayTeamInfo[num21].arrayThrowPlayerNo[arrayTeamInfo[num21].throwCnt]].GetUserType();
			Curling_Define.UserType userType2 = arrayTeamInfo[num21].arrayPlayer[arrayTeamInfo[num21].arrayHouseSweepPlayerNo[arrayTeamInfo[num21].throwCnt]].GetUserType();
			SingletonCustom<Curling_UIManager>.Instance.TeamFrameInit(num21, userType, userType2);
		}
		predictLine.positionCount = 2;
		state = State.COIN_TOSS;
		InitPlay(_isInit: true);
		predictStone.Init();
		charaToStoneVec = GetThrowPlayer().transform.position - GetThrowStone().transform.position;
		charaToStoneVec.y = 0f;
	}
	private void InitPlay(bool _isInit = false, PointGetTeam _pointGetTeam = PointGetTeam.TEAM_A)
	{
		if (_isInit)
		{
			turnTeam = ((UnityEngine.Random.Range(0, 2) != 0) ? Team.TEAM_B : Team.TEAM_A);
		}
		else if (_pointGetTeam == PointGetTeam.DRAW)
		{
			turnTeam = ((turnTeam == Team.TEAM_A) ? Team.TEAM_B : Team.TEAM_A);
		}
		else
		{
			turnTeam = (Team)_pointGetTeam;
		}
		for (int i = 0; i < arrayTeamInfo.Length; i++)
		{
			arrayTeamInfo[i].throwCnt = 0;
			for (int j = 0; j < arrayTeamInfo[i].arrayPlayer.Length; j++)
			{
				arrayTeamInfo[i].arrayPlayer[j].InitPlay();
			}
			for (int k = 0; k < arrayTeamInfo[i].arrayStone.Length; k++)
			{
				arrayTeamInfo[i].arrayStone[k].InitPlay();
			}
			SingletonCustom<Curling_UIManager>.Instance.TeamFrameInitPlay(i);
		}
		SetPlayTeam();
		SetViewingTeam();
		camera.InitPlay();
		isSweepStart = false;
		isSweepStop = false;
		isHouseSweepStart = false;
		isThrowStone = false;
		SetPredictLineActive(_isActive: false);
		SingletonCustom<Curling_CurlingRinkManager>.Instance.InitPlay();
		SingletonCustom<Curling_UIManager>.Instance.InitPlay();
		if (!_isInit)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetGameCnt();
		}
		SingletonCustom<Curling_UIManager>.Instance.SetTrunFrame((int)turnTeam);
	}
	public void FixedUpdateMethod()
	{
		for (int i = 0; i < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; i++)
		{
			arrayTeamInfo[(int)turnTeam].arrayPlayer[i].FixedUpdateMethod();
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayTeamInfo.Length; i++)
		{
			for (int j = 0; j < arrayTeamInfo[i].arrayPlayer.Length; j++)
			{
				arrayTeamInfo[i].arrayPlayer[j].SetNpadId();
			}
		}
		if (state == State.COIN_TOSS || state == State.PLAY_END)
		{
			return;
		}
		if (state != State.PLAY_START)
		{
			if (!isSweepStart)
			{
				if (sweepStartAnchor.position.z <= GetThrowStone().transform.position.z)
				{
					isSweepStart = true;
					state = State.SWEEP;
					for (int k = 0; k < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; k++)
					{
						Curling_Player curling_Player = arrayTeamInfo[(int)turnTeam].arrayPlayer[k];
						if (curling_Player.GetActionState() == Curling_Player.ActionState.SWEEP_0 || curling_Player.GetActionState() == Curling_Player.ActionState.SWEEP_1)
						{
							curling_Player.ChangeSweep();
						}
					}
				}
			}
			else if (!isSweepStop)
			{
				if (sweepStopAnchor.position.z <= GetThrowStone().transform.position.z)
				{
					isSweepStop = true;
					for (int l = 0; l < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; l++)
					{
						Curling_Player curling_Player = arrayTeamInfo[(int)turnTeam].arrayPlayer[l];
						if (curling_Player.GetActionState() == Curling_Player.ActionState.SWEEP_0 || curling_Player.GetActionState() == Curling_Player.ActionState.SWEEP_1)
						{
							curling_Player.StopSweep();
						}
					}
				}
			}
			else if (!isHouseSweepStart && houseSweepStartAnchor.position.z <= GetThrowStone().transform.position.z)
			{
				isHouseSweepStart = true;
				state = State.HOUSE_SWEEP;
				for (int m = 0; m < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; m++)
				{
					Curling_Player curling_Player = arrayTeamInfo[(int)turnTeam].arrayPlayer[m];
					if (curling_Player.GetActionState() == Curling_Player.ActionState.HOUSE_SWEEP)
					{
						curling_Player.ChangeSweep();
					}
				}
			}
		}
		for (int n = 0; n < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; n++)
		{
			arrayTeamInfo[(int)turnTeam].arrayPlayer[n].UpdateMethod();
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && turnTeam == Team.TEAM_B)
		{
			if (!isSkip && (state == State.PLAY_START || state == State.PREP_THROW || state == State.THROW))
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.SKIP);
				if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(0, SatGamePad.Button.X))
				{
					isSkip = true;
					SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.MAX);
					SingletonCustom<Curling_UIManager>.Instance.StartScreenFade(delegate
					{
						if (!GetThrowStone().GetIsThrow() && state < State.SWEEP)
						{
							if (state == State.PLAY_START)
							{
								SingletonCustom<Curling_UIManager>.Instance.SkipTurnCutIn();
								EndTurnCutIn();
							}
							if (state == State.THROW)
							{
								LeanTween.cancel(camera.gameObject);
								LeanTween.cancel(GetThrowPlayer().gameObject);
								LeanTween.cancel(base.gameObject);
							}
							GetThrowPlayer().SkipThrowStone();
						}
					}, delegate
					{
						isSkip = false;
					});
					return;
				}
			}
			else
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.MAX);
			}
		}
		if (state == State.PLAY_START || !isThrowStone)
		{
			return;
		}
		bool flag = true;
		for (int num = 0; num < arrayTeamInfo.Length; num++)
		{
			for (int num2 = 0; num2 < arrayTeamInfo[num].arrayStone.Length; num2++)
			{
				Curling_Stone curling_Stone = arrayTeamInfo[num].arrayStone[num2];
				if (curling_Stone.gameObject.activeSelf && !curling_Stone.GetRigid().isKinematic && !curling_Stone.GetIsFailure())
				{
					curling_Stone.UpdateMetthod();
					if (curling_Stone.GetRigid().velocity != Vector3.zero)
					{
						flag = false;
						break;
					}
				}
			}
		}
		if (flag)
		{
			state = State.PLAY_END;
			SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.MAX);
			SingletonCustom<Curling_UIManager>.Instance.SetPlayerIconActive(1, _isActive: false);
			camera.SetNearHouse();
			LeanTween.delayedCall(base.gameObject, CAMERA_NEAR_HOUSE_TIME, (Action)delegate
			{
				PointCalculation_2();
			});
		}
	}
	public void LateUpdateMethod()
	{
		camera.LateUpdateMethod();
	}
	public void StartPlayGame()
	{
		state = State.PLAY_START;
		SingletonCustom<Curling_UIManager>.Instance.ShowTurnCutIn(delegate
		{
			EndTurnCutIn();
		});
	}
	private void EndTurnCutIn()
	{
		Curling_Define.UserType userType = GetThrowPlayer().GetUserType();
		Curling_Define.UserType userType2 = GetHouseSweepPlayer().GetUserType();
		state = State.PREP_THROW;
		SingletonCustom<Curling_UIManager>.Instance.SetPlayerIcon(userType, userType2);
		if (userType < Curling_Define.UserType.CPU_1)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetPlayerIconActive(0, _isActive: true);
			SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.THROW_POWER);
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
			if (userType != userType2)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType2);
			}
		}
	}
	public void SetPlayTeam()
	{
		int num = arrayTeamInfo[(int)turnTeam].arrayThrowPlayerNo[arrayTeamInfo[(int)turnTeam].throwCnt];
		int num2 = arrayTeamInfo[(int)turnTeam].arrayHouseSweepPlayerNo[arrayTeamInfo[(int)turnTeam].throwCnt];
		int num3 = 0;
		Curling_Stone throwStone = GetThrowStone();
		for (int i = 0; i < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; i++)
		{
			Curling_Player curling_Player = arrayTeamInfo[(int)turnTeam].arrayPlayer[i];
			if (num == curling_Player.GetPlayerIdx())
			{
				curling_Player.SetThrowState();
				throwStone.PrepForThrow(curling_Player.GetStoneHaveHand());
				camera.SetStone(throwStone.gameObject);
			}
			if (num2 == curling_Player.GetPlayerIdx())
			{
				curling_Player.SetHouseSweepState();
				curling_Player.SetStone(throwStone);
			}
			if (num != curling_Player.GetPlayerIdx() && num2 != curling_Player.GetPlayerIdx())
			{
				curling_Player.SetSweepState(num3);
				num3++;
				curling_Player.SetStone(throwStone);
			}
		}
	}
	public void SetViewingTeam()
	{
		Team team = (turnTeam == Team.TEAM_A) ? Team.TEAM_B : Team.TEAM_A;
		int num = 0;
		for (int i = 0; i < arrayTeamInfo[(int)team].arrayPlayer.Length; i++)
		{
			arrayTeamInfo[(int)team].arrayPlayer[i].SetViewingState(num);
			num++;
		}
	}
	public void AddThrowCnt()
	{
		UnityEngine.Debug.Log("投げた回数を加算");
		arrayTeamInfo[(int)turnTeam].throwCnt++;
		SingletonCustom<Curling_UIManager>.Instance.SetThrowStoneIcon((int)turnTeam, arrayTeamInfo[(int)turnTeam].throwCnt);
		if (arrayTeamInfo[0].throwCnt == Curling_Define.THROW_CNT && arrayTeamInfo[1].throwCnt == Curling_Define.THROW_CNT)
		{
			PointCalculation();
		}
		else
		{
			ChangeTurn();
		}
	}
	public void ChangeTurn()
	{
		UnityEngine.Debug.Log("タ\u30fcン切り替え処理");
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<Curling_UIManager>.Instance.StartScreenFade(delegate
			{
				state = State.CHANGE_TURN;
				for (int i = 0; i < arrayTeamInfo[(int)turnTeam].arrayPlayer.Length; i++)
				{
					arrayTeamInfo[(int)turnTeam].arrayPlayer[i].LeanTweenCancel();
				}
				turnTeam = ((turnTeam == Team.TEAM_A) ? Team.TEAM_B : Team.TEAM_A);
				for (int j = 0; j < arrayTeamInfo.Length; j++)
				{
					for (int k = 0; k < arrayTeamInfo[j].arrayPlayer.Length; k++)
					{
						arrayTeamInfo[j].arrayPlayer[k].InitPlay();
					}
					for (int l = 0; l < arrayTeamInfo[j].arrayStone.Length; l++)
					{
						if (arrayTeamInfo[j].arrayStone[l].GetIsFailure())
						{
							arrayTeamInfo[j].arrayStone[l].gameObject.SetActive(value: false);
						}
					}
				}
				SetPlayTeam();
				SetViewingTeam();
				camera.InitPlay();
				isSweepStart = false;
				isSweepStop = false;
				isHouseSweepStart = false;
				isThrowStone = false;
				SetPredictLineActive(_isActive: false);
				SingletonCustom<Curling_CurlingRinkManager>.Instance.InitPlay();
				SingletonCustom<Curling_CurlingRinkManager>.Instance.LeanTweenCancel();
				SingletonCustom<Curling_UIManager>.Instance.InitPlay();
				SingletonCustom<Curling_UIManager>.Instance.SetTrunFrame((int)turnTeam);
			}, delegate
			{
				StartPlayGame();
			});
		});
	}
	private void PointCalculation()
	{
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			PointGetTeam pointGetTeam = PointGetTeam.DRAW;
			float num = 1000f;
			float num2 = 0f;
			for (int i = 0; i < arrayTeamInfo.Length; i++)
			{
				for (int j = 0; j < arrayTeamInfo[i].arrayStone.Length; j++)
				{
					Curling_Stone curling_Stone = arrayTeamInfo[i].arrayStone[j];
					if (curling_Stone.gameObject.activeSelf)
					{
						num2 = CalcManager.Length(new Vector3(curling_Stone.transform.position.x, 0f, curling_Stone.transform.position.z), SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos());
						if (num2 < num)
						{
							pointGetTeam = (PointGetTeam)i;
							num = num2;
						}
					}
				}
			}
			num = 1000f;
			PointGetTeam pointGetTeam2 = (pointGetTeam == PointGetTeam.TEAM_A) ? PointGetTeam.TEAM_B : PointGetTeam.TEAM_A;
			for (int k = 0; k < arrayTeamInfo[(int)pointGetTeam2].arrayStone.Length; k++)
			{
				Curling_Stone curling_Stone = arrayTeamInfo[(int)pointGetTeam2].arrayStone[k];
				num2 = CalcManager.Length(new Vector3(curling_Stone.transform.position.x, 0f, curling_Stone.transform.position.z), SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos());
				if (num2 < num)
				{
					num = num2;
				}
			}
			int num3 = 0;
			for (int l = 0; l < arrayTeamInfo[(int)pointGetTeam].arrayStone.Length; l++)
			{
				Curling_Stone curling_Stone = arrayTeamInfo[(int)pointGetTeam].arrayStone[l];
				num2 = CalcManager.Length(new Vector3(curling_Stone.transform.position.x, 0f, curling_Stone.transform.position.z), SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos());
				if (num2 < num)
				{
					num3++;
				}
			}
			arrayTeamInfo[(int)pointGetTeam].point += num3;
			SingletonCustom<Curling_UIManager>.Instance.SetPoint((int)pointGetTeam, arrayTeamInfo[(int)pointGetTeam].point);
			LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
			{
				AddGameCnt();
			});
		});
	}
	private void PointCalculation_2()
	{
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			int num = 0;
			for (int i = 0; i < arrayTeamInfo.Length; i++)
			{
				pointCalcPointTmp[i] = 0;
				for (int j = 0; j < arrayTeamInfo[i].arrayStone.Length; j++)
				{
					Curling_Stone curling_Stone = arrayTeamInfo[i].arrayStone[j];
					if (curling_Stone.gameObject.activeSelf && !curling_Stone.GetRigid().isKinematic && !curling_Stone.GetIsFailure())
					{
						int pointIdx = SingletonCustom<Curling_CurlingRinkManager>.Instance.GetPointIdx(curling_Stone);
						int num2 = SingletonCustom<Curling_CurlingRinkManager>.Instance.GetArrayPoint()[pointIdx];
						if (num2 > 0)
						{
							pointCalcPointTmp[i] += num2;
							SingletonCustom<Curling_UIManager>.Instance.ShowGetPoint((int)curling_Stone.GetTeam(), curling_Stone.transform.position, pointIdx, num);
							num++;
						}
					}
				}
			}
			if (SingletonCustom<Curling_CurlingRinkManager>.Instance.GetIsHouseBlink())
			{
				SingletonCustom<Curling_CurlingRinkManager>.Instance.BlinkHouseCircle();
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
			LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
			{
				arrayTeamInfo[(int)turnTeam].throwCnt++;
				SingletonCustom<Curling_UIManager>.Instance.SetThrowStoneIcon((int)turnTeam, arrayTeamInfo[(int)turnTeam].throwCnt);
				for (int k = 0; k < arrayTeamInfo.Length; k++)
				{
					pointCalcPointTmp[k] -= arrayTeamInfo[k].point;
				}
				if (pointCalcPointTmp[0] != 0 || pointCalcPointTmp[1] != 0)
				{
					Curling_GameManager curling_GameManager = this;
					float seTiming = 0f;
					LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
					{
						seTiming += Time.deltaTime;
						if (seTiming > 0.1f)
						{
							seTiming = 0f;
							SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
						}
						for (int m = 0; m < curling_GameManager.arrayTeamInfo.Length; m++)
						{
							SingletonCustom<Curling_UIManager>.Instance.SetPoint(m, Mathf.CeilToInt((float)curling_GameManager.arrayTeamInfo[m].point + (float)curling_GameManager.pointCalcPointTmp[m] * _value));
						}
					});
				}
				LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
				{
					for (int l = 0; l < arrayTeamInfo.Length; l++)
					{
						arrayTeamInfo[l].point += pointCalcPointTmp[l];
					}
					if (arrayTeamInfo[0].throwCnt == Curling_Define.THROW_CNT && arrayTeamInfo[1].throwCnt == Curling_Define.THROW_CNT)
					{
						AddGameCnt();
					}
					else
					{
						ChangeTurn();
					}
				});
			});
		});
	}
	private void AddGameCnt()
	{
		gameCnt++;
		if (gameCnt == Curling_Define.GAME_CNT)
		{
			GameEnd();
		}
		else
		{
			LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
			{
				SingletonCustom<Curling_UIManager>.Instance.StartScreenFade(delegate
				{
					state = State.CHANGE_TURN;
					InitPlay();
				}, delegate
				{
					StartPlayGame();
				});
			});
		}
	}
	public void ThrowStone(Vector3 _curveDir, Vector3 _velDir, float _diffPower)
	{
		if (state != State.THROW)
		{
			state = State.THROW;
			SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.MAX);
			SingletonCustom<Curling_UIManager>.Instance.SetPlayerIconActive(0, _isActive: false);
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: false);
			SingletonCustom<Curling_UIManager>.Instance.MoveUIOutside();
			camera.SetParent();
		}
		LeanTween.moveY(camera.gameObject, 5f, isSkip ? 0f : STONE_THROW_MOVE_TIME);
		LeanTween.rotateX(camera.gameObject, 35f, isSkip ? 0f : STONE_THROW_MOVE_TIME);
		Curling_Player throwPlayer = GetThrowPlayer();
		LeanTween.move(throwPlayer.gameObject, GetThrowMovePos(_velDir), isSkip ? 0f : STONE_THROW_MOVE_TIME);
		LeanTween.delayedCall(base.gameObject, isSkip ? 0f : CAMERA_CHANGE_WAIT_TIME, (Action)delegate
		{
			camera.SetSweepCamera(GetThrowMovePos(_velDir).z, isSkip ? 0f : CAMERA_CHANGE_TIME);
		});
		LeanTween.delayedCall(base.gameObject, isSkip ? 0f : STONE_THROW_MOVE_TIME, (Action)delegate
		{
			if (!isSkip)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_curling_throw");
			}
			if (!throwPlayer.GetIsCpu())
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)throwPlayer.GetUserType());
			}
			GetThrowStone().Throw(_curveDir, _velDir, STONE_THROW_BASE_POWER + STONE_THROW_DIFF_MAX_POWER * _diffPower);
			LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
			{
				isThrowStone = true;
			});
		});
	}
	public float GetThrowMoveDistance()
	{
		return THROW_MOVE_DISTANCE;
	}
	public Vector3 GetThrowMovePos(Vector3 _dir)
	{
		return arrayPlayTeamCharaAnchor[0].position + _dir * THROW_MOVE_DISTANCE;
	}
	public bool GetIsGameStart()
	{
		return isGameStart;
	}
	public bool GetIsGameEnd()
	{
		return isGameEnd;
	}
	public Camera GetCamera()
	{
		return camera.GetCamera();
	}
	public int GetGameCnt()
	{
		return gameCnt;
	}
	public State GetState()
	{
		return state;
	}
	public float GetStoneThrowBasePower()
	{
		return STONE_THROW_BASE_POWER;
	}
	public float GetStoneThrowDiffMaxPower()
	{
		return STONE_THROW_DIFF_MAX_POWER;
	}
	public float GetStoneThrowCurveVectorDiff()
	{
		return STONE_THROW_CURVE_VECTOR_DIFF;
	}
	public float GetCharaChangeSweepTime()
	{
		return CHARA_CHANGE_SWEEP_TIME;
	}
	public Team GetTurnTeam()
	{
		return turnTeam;
	}
	public Curling_Stone GetThrowStone()
	{
		return arrayTeamInfo[(int)turnTeam].arrayStone[arrayTeamInfo[(int)turnTeam].throwCnt];
	}
	public TeamInfo[] GetArrayTeamInfo()
	{
		return arrayTeamInfo;
	}
	public Curling_Stone[] GetArrayStone(int _teamNo)
	{
		return arrayTeamInfo[_teamNo].arrayStone;
	}
	public Curling_Player GetThrowPlayer()
	{
		int num = Mathf.Clamp(arrayTeamInfo[(int)turnTeam].throwCnt, 0, Curling_Define.THROW_CNT - 1);
		int num2 = arrayTeamInfo[(int)turnTeam].arrayThrowPlayerNo[num];
		return arrayTeamInfo[(int)turnTeam].arrayPlayer[num2];
	}
	public Curling_Player GetHouseSweepPlayer()
	{
		int num = Mathf.Clamp(arrayTeamInfo[(int)turnTeam].throwCnt, 0, Curling_Define.THROW_CNT - 1);
		int num2 = arrayTeamInfo[(int)turnTeam].arrayHouseSweepPlayerNo[num];
		return arrayTeamInfo[(int)turnTeam].arrayPlayer[num2];
	}
	public Transform GetPlayTeamArrayCharaAnchor(int _idx)
	{
		return arrayPlayTeamCharaAnchor[_idx];
	}
	public Transform[] GetViewingTeamArrayCharaAnchor(Team _team)
	{
		return arrayViewingTeam[(int)_team].arrayCharaAnchor;
	}
	public Transform[] GetArraySweepMoveEndAnchor()
	{
		return arraySweepMoveEndAnchor;
	}
	public Vector3 GetCharaToStoneVec()
	{
		return charaToStoneVec;
	}
	public Transform GetCameraMoveLimitAnchor()
	{
		return cameraMoveLimitAnchor;
	}
	public bool GetIsSkip()
	{
		return isSkip;
	}
	public Transform GetSkipFadeOutAnchor()
	{
		return skipFadeOutAnchor;
	}
	public float GetCameraNearTeaZTime()
	{
		return CAMERA_NEAR_TEA_Z_TIME;
	}
	public float GetCameraNearHouseTime()
	{
		return CAMERA_NEAR_HOUSE_TIME;
	}
	public void GameStart()
	{
		isGameStart = true;
	}
	public void GameEnd()
	{
		isGameEnd = true;
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<Curling_UIManager>.Instance.ClearGetPointList();
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				ResultGameDataParams.SetPoint();
				ResultGameDataParams.SetRecord_WinOrLose(arrayTeamInfo[0].point);
				ResultGameDataParams.SetRecord_WinOrLose(arrayTeamInfo[1].point, 1);
				winOrLoseResultManager.SetTeamPlayerGroupList(teamAPlayerNoList, teamBPlayerNoList);
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					if (arrayTeamInfo[0].point > arrayTeamInfo[1].point)
					{
						winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
						int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
						if (aiStrength == 0)
						{
							UnityEngine.Debug.Log("<color=#ac6b25>銅トロフィ\u30fc獲得</color>");
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.RECEIVE_PON);
						}
						if (aiStrength == 1)
						{
							UnityEngine.Debug.Log("<color=#c0c0c0>銀トロフィ\u30fc獲得</color>");
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.RECEIVE_PON);
						}
						if (aiStrength == 2)
						{
							UnityEngine.Debug.Log("<color=#e6b422>金トロフィ\u30fc獲得</color>");
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.RECEIVE_PON);
						}
					}
					else if (arrayTeamInfo[0].point < arrayTeamInfo[1].point)
					{
						winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
					}
					else
					{
						winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
					}
				}
				else if (arrayTeamInfo[0].point > arrayTeamInfo[1].point)
				{
					winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (arrayTeamInfo[0].point < arrayTeamInfo[1].point)
				{
					winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				}
				else
				{
					winOrLoseResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
				}
			});
		});
	}
	public void SetPredictLineActive(bool _isActive)
	{
		predictLine.gameObject.SetActive(_isActive);
		SingletonCustom<Curling_UIManager>.Instance.SetPredictArrowActive(_isActive);
	}
	public void SetPredictLine(Vector3 _velDir, float _throwPower)
	{
		float f = Mathf.Atan2(_velDir.z, _velDir.x);
		float x = Mathf.Cos(f);
		float z = Mathf.Sin(f);
		float num = CalcManager.Length(predictStone.GetMaxPos(0), predictStone.GetMaxPos(1));
		Vector3 maxPos = predictStone.GetMaxPos(0);
		maxPos.z += num * _throwPower;
		float d = CalcManager.Length(predictStone.GetOriginPos(), GetThrowStone().transform.position);
		float d2 = CalcManager.Length(predictStone.GetMaxPos(0), GetThrowStone().transform.position);
		float d3 = CalcManager.Length(maxPos, GetThrowStone().transform.position);
		Vector3 vector = new Vector3(x, 0f, z) * d + GetThrowStone().transform.position;
		Vector3 minPos = new Vector3(x, 0f, z) * d2 + GetThrowStone().transform.position;
		Vector3 vector2 = new Vector3(x, 0f, z) * d3 + GetThrowStone().transform.position;
		predictLine.SetPosition(0, vector);
		predictLine.SetPosition(1, vector2);
		SingletonCustom<Curling_UIManager>.Instance.SetPredictArrow(vector, minPos, vector2);
	}
	public void GroupVibration()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0] < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
			}
		}
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < arrayTeamInfo.Length; i++)
		{
			arrayTeamInfo[i].point = 0;
			for (int j = 0; j < arrayTeamInfo[i].arrayStone.Length; j++)
			{
				int num = UnityEngine.Random.Range(0, SingletonCustom<Curling_CurlingRinkManager>.Instance.GetArrayPoint().Length);
				arrayTeamInfo[i].point += SingletonCustom<Curling_CurlingRinkManager>.Instance.GetArrayPoint()[num];
			}
		}
		GameEnd();
	}
}
