using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Shooting_GameManager : SingletonCustom<Shooting_GameManager>
{
	public int PLAY_NUM = 4;
	private Camera uiCamera;
	[SerializeField]
	[Header("シ\u30fcンカメラ(複数人用)")]
	private Camera worldCamera;
	[SerializeField]
	[Header("シ\u30fcンカメラ(1人用)")]
	private Camera worldSingleCamera;
	[SerializeField]
	[Header("リザルト")]
	private WinOrLoseResultManager winResultManager;
	[SerializeField]
	private RankingResultManager rankingResultManager;
	[SerializeField]
	[Header("タイマ\u30fc")]
	private Shooting_Timer timer;
	[SerializeField]
	[Header("花火オブジェクト")]
	private GameObject fireworksObj;
	private bool isGameStart;
	private bool isGameEnd;
	private bool hasSecondGroup;
	private bool isNowSecondGroup;
	private bool isNothingEndCheck;
	private bool isSkipCheck;
	private bool isSkipEnd;
	private int playerNum;
	private int teamNum;
	private int[] arrayPlayerNo;
	private int[] arrayTeamNo;
	private List<int>[] playerGroupList;
	private readonly float WILL_END_SOON_SPEED = 1.5f;
	private Vector3 defaultSingleCameraPos;
	private float defaultSingleCameraFOV;
	[SerializeField]
	[Header("消しゴムくん")]
	private GameObject objKeshigomuKun;
	private bool isKeshigomuKun;
	[SerializeField]
	[Header("いちごちゃん")]
	private GameObject objIchigoChan;
	private bool isIchigoChan;
	public Camera UICamera => uiCamera;
	public Camera WorldCamera => worldCamera;
	public Camera WorldSingleCamera => worldSingleCamera;
	public Shooting_Timer Timer => timer;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public bool HasSecondGroup => hasSecondGroup;
	public bool IsNowSecondGroup => isNowSecondGroup;
	public bool IsSingle => SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
	public int PlayerNum => playerNum;
	public int TeamNum => teamNum;
	public int[] ArrayTeamNo => arrayTeamNo;
	public List<int>[] PlayerGroupList => playerGroupList;
	public float WillEndSoonSpeed => WILL_END_SOON_SPEED;
	public void SetChangeItem(Camera _worldCamera, GameObject _objKeshigomuKun, GameObject _objIchigoChan)
	{
		worldCamera = _worldCamera;
		objKeshigomuKun = _objKeshigomuKun;
		objIchigoChan = _objIchigoChan;
	}
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		uiCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		SetPlayerData();
		timer.Init();
		if (IsSingle)
		{
			worldCamera.gameObject.SetActive(value: false);
			worldSingleCamera.gameObject.SetActive(value: true);
		}
		else
		{
			worldCamera.gameObject.SetActive(value: true);
			worldSingleCamera.gameObject.SetActive(value: false);
		}
	}
	public void SecondGroupInit()
	{
		SingletonCustom<Shooting_UIManager>.Instance.Fade(1f, 2f, delegate
		{
			isGameStart = false;
			isGameEnd = false;
			isNothingEndCheck = false;
			isSkipCheck = false;
			isSkipEnd = false;
			timer.SecondGroupInit();
			LeanTween.delayedCall(1f, (Action)delegate
			{
				SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
			});
			GroupVibration();
			if (IsSingle)
			{
				SingleCameraMoveDirection();
			}
			SingletonCustom<Shooting_ControllerManager>.Instance.SecondGroupInit();
			SingletonCustom<Shooting_TargetManager>.Instance.SecondGroupInit();
			SingletonCustom<Shooting_ScoreManager>.Instance.SecondGroupInit();
			SingletonCustom<Shooting_UIManager>.Instance.SecondGroupInit();
			SingletonCustom<Shooting_BulletManager>.Instance.SecondGroupInit();
		});
	}
	public void UpdateMethod()
	{
		if (!isGameStart || isGameEnd)
		{
			return;
		}
		timer.UpdateMethod();
		if (SingletonCustom<Shooting_ControllerManager>.Instance.CheckAllBulletEnd())
		{
			if (!isNothingEndCheck)
			{
				isNothingEndCheck = true;
				LeanTween.delayedCall(3f, (Action)delegate
				{
					GameEnd();
				});
			}
		}
		else if (SingletonCustom<Shooting_ControllerManager>.Instance.CheckPlayerEnd())
		{
			if (!isSkipCheck)
			{
				isSkipCheck = true;
				SingletonCustom<Shooting_UIManager>.Instance.ViewSkipControlInfo();
			}
			else if (!isSkipEnd && GetSkipButtonDown(0))
			{
				isSkipEnd = true;
				SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
				SingletonCustom<Shooting_ScoreManager>.Instance.CpuFutureScoreCalc();
				GameEnd();
			}
		}
	}
	public void SetPlayerData()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
		playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		if (playerGroupList[0].Count > 2)
		{
			hasSecondGroup = true;
			if (playerGroupList[0][3] < 4)
			{
				switch (UnityEngine.Random.Range(0, 6))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						3
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						3
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						0,
						3,
						1,
						2
					};
					break;
				case 3:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						3
					};
					break;
				case 4:
					playerGroupList[0] = new List<int>
					{
						1,
						3,
						0,
						2
					};
					break;
				case 5:
					playerGroupList[0] = new List<int>
					{
						2,
						3,
						0,
						1
					};
					break;
				}
			}
			else
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						4
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						4
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						4
					};
					break;
				}
			}
		}
		int num = hasSecondGroup ? (PLAY_NUM * 2) : PLAY_NUM;
		arrayPlayerNo = new int[num];
		arrayTeamNo = new int[num];
		if (hasSecondGroup)
		{
			for (int i = 0; i < num; i++)
			{
				arrayPlayerNo[i] = playerGroupList[i / 2 % 2][i % 2 + i / 4 * 2];
				arrayTeamNo[i] = i / 2 % 2;
			}
			return;
		}
		int num2 = 4;
		for (int j = 0; j < num; j++)
		{
			if (j < playerNum)
			{
				arrayPlayerNo[j] = j;
			}
			else
			{
				arrayPlayerNo[j] = num2;
				num2++;
			}
			if (teamNum == 2)
			{
				arrayTeamNo[j] = ((!playerGroupList[0].Contains(arrayPlayerNo[j])) ? 1 : 0);
			}
			else
			{
				arrayTeamNo[j] = j;
			}
		}
	}
	public void SingleCameraMoveInit()
	{
		defaultSingleCameraPos = worldSingleCamera.transform.position;
		defaultSingleCameraFOV = worldSingleCamera.fieldOfView;
		worldSingleCamera.transform.position = worldCamera.transform.position;
		worldSingleCamera.fieldOfView = worldCamera.fieldOfView;
	}
	public void SingleCameraMoveDirection()
	{
		LeanTween.value(base.gameObject, 0f, 1f, 1f).setEaseInOutQuad().setOnUpdate(delegate(float _value)
		{
			worldSingleCamera.transform.position = Vector3.Lerp(worldCamera.transform.position, defaultSingleCameraPos, _value);
			worldSingleCamera.fieldOfView = Mathf.Lerp(worldCamera.fieldOfView, defaultSingleCameraFOV, _value);
			SingletonCustom<Shooting_UIManager>.Instance.SetSingleCursorZoomScaleLerp(_value);
		});
	}
	public void GroupVibration()
	{
		if (!hasSecondGroup)
		{
			return;
		}
		if (isNowSecondGroup)
		{
			int num = playerGroupList[0][2];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
			num = playerGroupList[0][3];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
		}
		else
		{
			int commonVibration = playerGroupList[0][0];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
			commonVibration = playerGroupList[0][1];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
		}
	}
	public void GameStart()
	{
		isGameStart = true;
	}
	public void GameEnd()
	{
		if (isGameEnd)
		{
			return;
		}
		isGameEnd = true;
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		if (Shooting_Define.IS_TEAM_MODE && !hasSecondGroup)
		{
			int num = 0;
			int num2 = 0;
			int[] array = GetArrayTeamNo();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0)
				{
					num += SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[i];
				}
				else if (array[i] == 1)
				{
					num2 += SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[i];
				}
			}
			ResultGameDataParams.SetRecord_WinOrLose(num);
			ResultGameDataParams.SetRecord_WinOrLose(num2, 1);
		}
		else
		{
			ResultGameDataParams.SetRecord_Int(SingletonCustom<Shooting_ScoreManager>.Instance.GetCopyArrayScore(), GetArrayPlayerNo(_isSplitGroup: true, isNowSecondGroup), !isNowSecondGroup);
		}
		if (hasSecondGroup && !isNowSecondGroup)
		{
			SecondGroupInit();
			isNowSecondGroup = true;
		}
		else
		{
			LeanTween.delayedCall(1.5f, (Action)delegate
			{
				LeanTween.delayedCall(1.5f, (Action)delegate
				{
					EndCamera();
				});
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					if (SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[0] >= Shooting_Define.TROPHY_BRONZE_SCORE)
					{
						UnityEngine.Debug.Log("<color=#ac6b25>銅トロフィ\u30fc獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ATTACK_BALL);
					}
					if (SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[0] >= Shooting_Define.TROPHY_SILVER_SCORE)
					{
						UnityEngine.Debug.Log("<color=#c0c0c0>銀トロフィ\u30fc獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ATTACK_BALL);
					}
					if (SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[0] >= Shooting_Define.TROPHY_GOLD_SCORE)
					{
						UnityEngine.Debug.Log("<color=#e6b422>金トロフィ\u30fc獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ATTACK_BALL);
					}
					rankingResultManager.ShowResult_Score();
				}
				else if (Shooting_Define.IS_BATTLE_MODE || hasSecondGroup)
				{
					rankingResultManager.ShowResult_Score();
				}
				else if (Shooting_Define.IS_TEAM_MODE)
				{
					int num3 = 0;
					int num4 = 0;
					int[] array2 = GetArrayTeamNo();
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] == 0)
						{
							num3 += SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[j];
						}
						else if (array2[j] == 1)
						{
							num4 += SingletonCustom<Shooting_ScoreManager>.Instance.ArrayScore[j];
						}
					}
					if (num3 > num4)
					{
						winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
					}
					else if (num4 > num3)
					{
						if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
						{
							winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 1);
						}
						else
						{
							winResultManager.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
						}
					}
					else
					{
						winResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
					}
				}
			});
		}
	}
	private void EndCamera()
	{
		worldCamera.enabled = false;
		fireworksObj.SetActive(value: false);
	}
	public bool GetIsPlayer(int _gunNo, bool _isCheckSecondGroup = true)
	{
		if (_isCheckSecondGroup && isNowSecondGroup)
		{
			_gunNo += PLAY_NUM;
		}
		return arrayPlayerNo[_gunNo] < 4;
	}
	public int GetPlayerNo(int _gunNo, bool _isCheckSecondGroup = true)
	{
		if (_isCheckSecondGroup && isNowSecondGroup)
		{
			return arrayPlayerNo[PLAY_NUM + _gunNo];
		}
		return arrayPlayerNo[_gunNo];
	}
	public int[] GetArrayPlayerNo(bool _isSplitGroup = true, bool _isGetSecondGroup = false)
	{
		if (hasSecondGroup && _isSplitGroup)
		{
			int[] array = new int[arrayPlayerNo.Length / 2];
			int num = _isGetSecondGroup ? array.Length : 0;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = arrayPlayerNo[num + i];
			}
			return array;
		}
		return arrayPlayerNo;
	}
	public int GetTeamNo(int _gunNo, bool _isCheckSecondGroup = true)
	{
		if (_isCheckSecondGroup && isNowSecondGroup)
		{
			return arrayTeamNo[PLAY_NUM + _gunNo];
		}
		return arrayTeamNo[_gunNo];
	}
	public int[] GetArrayTeamNo(bool _isSplitGroup = true, bool _isGetSecondGroup = false)
	{
		if (hasSecondGroup && _isSplitGroup)
		{
			int[] array = new int[arrayTeamNo.Length / 2];
			int num = _isGetSecondGroup ? array.Length : 0;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = arrayTeamNo[num + i];
			}
			return array;
		}
		return arrayTeamNo;
	}
	public static bool GetSkipButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
	}
	private void DebugEnd()
	{
		for (int i = 0; i < PLAY_NUM; i++)
		{
			SingletonCustom<Shooting_ScoreManager>.Instance.SetScore(i, UnityEngine.Random.Range(2, 20) * 100);
		}
		GameEnd();
	}
}
