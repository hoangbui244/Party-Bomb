using GamepadInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hidden_GameManager : SingletonCustom<Hidden_GameManager>
{
	public const float TIME_LIMIT = 60f;
	private static bool IS_REBOOT_FIRST_PLAY = true;
	[SerializeField]
	private RankingResultManager resultManager;
	[SerializeField]
	private GameObject firstPlayObj;
	[SerializeField]
	private GameObject firstPlayRootObj;
	private bool isFirstPlayPause;
	private bool isGameStart;
	private bool isGameEnd;
	private float gameTime;
	private float limitTime;
	private int scoreUpCount = 1;
	private int countDownCount = 3;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public int GameMinute => (int)gameTime / 60;
	public int GameSecond => (int)gameTime % 60;
	public int LimitMinute
	{
		get
		{
			if (!isGameStart)
			{
				return 1;
			}
			return (int)Mathf.Max(0f, 60f - limitTime + 1f) / 60;
		}
	}
	public int LimitSecond
	{
		get
		{
			if (!isGameStart)
			{
				return 0;
			}
			return (int)Mathf.Max(0f, 60f - limitTime + 1f) % 60;
		}
	}
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		OpenGameDirection();
	}
	private void OpenGameDirection()
	{
		if (IS_REBOOT_FIRST_PLAY)
		{
			isFirstPlayPause = (!SingletonCustom<SaveDataManager>.Instance.SaveData.isStartHelp[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType] || SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp);
			firstPlayObj.SetActive(value: true);
			firstPlayRootObj.SetActive(!isFirstPlayPause);
		}
		StartCoroutine(_OpenGameDirection());
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			CommonCountDownStart();
		});
	}
	private void CommonCountDownStart()
	{
		if (firstPlayObj.activeSelf)
		{
			StartCoroutine(_FirstPlayDirection());
		}
		else
		{
			SingletonCustom<CommonStartSimple>.Instance.Show(delegate
			{
				GameStart();
			});
		}
	}
	private IEnumerator _FirstPlayDirection()
	{
		SingletonCustom<Hidden_CharacterManager>.Instance.SetCameraMoveActive(_active: false);
		if (isFirstPlayPause)
		{
			yield return new WaitForSeconds(0.1f);
		}
		float timer = 0f;
		bool isCommonOpen2 = false;
		while (timer < 0.5f)
		{
			if (isFirstPlayPause)
			{
				isCommonOpen2 = (Time.timeScale < 0.01f);
			}
			if (isCommonOpen2 == firstPlayRootObj.activeSelf)
			{
				firstPlayRootObj.SetActive(!firstPlayRootObj.activeSelf);
			}
			if (!isCommonOpen2)
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		while (firstPlayObj.activeSelf)
		{
			isCommonOpen2 = (Time.timeScale < 0.01f);
			if (isCommonOpen2 == firstPlayRootObj.activeSelf)
			{
				firstPlayRootObj.SetActive(!firstPlayRootObj.activeSelf);
			}
			if (!isCommonOpen2 && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				firstPlayObj.SetActive(value: false);
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
				IS_REBOOT_FIRST_PLAY = false;
			}
			yield return null;
		}
		SingletonCustom<Hidden_CharacterManager>.Instance.SetCameraMoveActive(_active: true);
		yield return new WaitForSeconds(0.5f);
		SingletonCustom<CommonStartSimple>.Instance.Show(delegate
		{
			GameStart();
		});
	}
	public void UpdateMethod()
	{
		if (!isGameStart || isGameEnd)
		{
			return;
		}
		gameTime += Time.deltaTime;
		limitTime = gameTime;
		if (gameTime >= (float)scoreUpCount)
		{
			SingletonCustom<Hidden_CharacterManager>.Instance.OneSecondScoreUp();
			scoreUpCount++;
		}
		SingletonCustom<Hidden_UiManager>.Instance.SetTime(60f - limitTime);
		if (60f - limitTime < (float)countDownCount)
		{
			if (countDownCount == 3)
			{
				SingletonCustom<Hidden_UiManager>.Instance.SetCountDownView(_active: true);
			}
			else if (countDownCount == 0)
			{
				SingletonCustom<Hidden_UiManager>.Instance.SetCountDownView(_active: false);
			}
			if (countDownCount > 0)
			{
				SingletonCustom<Hidden_UiManager>.Instance.SetCountDownNum(countDownCount);
			}
			countDownCount--;
		}
		if (60f < gameTime)
		{
			GameEnd();
		}
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<Hidden_CharacterManager>.Instance.SetCharaMoveActive(_active: true);
	}
	public void GameEnd()
	{
		if (isGameEnd)
		{
			return;
		}
		isGameEnd = true;
		SingletonCustom<Hidden_CharacterManager>.Instance.SetCharaMoveActive(_active: false);
		SingletonCustom<Hidden_CharacterManager>.Instance.SetCameraMoveActive(_active: false);
		SingletonCustom<Hidden_CharacterManager>.Instance.EndChara();
		SingletonCustom<Hidden_UiManager>.Instance.CloseGameUI();
		if (SingletonCustom<Hidden_CharacterManager>.Instance.PlayerNum == 1)
		{
			CheckTrophy();
		}
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		int num = 4;
		int[] array = new int[num];
		int[] array2 = new int[num];
		for (int i = 0; i < num; i++)
		{
			Hidden_CharacterScript chara = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(i);
			array2[i] = chara.PlayerNo;
			array[i] = chara.Score;
			if (i > 3)
			{
				playerGroupList[i % 4].Add(array2[i]);
			}
		}
		ResultGameDataParams.SetRecord_Int(array, array2);
		StartCoroutine(_GameEnd());
	}
	private IEnumerator _GameEnd()
	{
		SingletonCustom<CommonEndSimple>.Instance.Show();
		yield return new WaitForSeconds(3f);
		resultManager.ShowResult_Score();
		yield return new WaitForSeconds(1.5f);
		SingletonCustom<Hidden_CharacterManager>.Instance.EndCamera();
	}
	private void CheckTrophy()
	{
		if (SingletonCustom<Hidden_CharacterManager>.Instance.PlayerNum != 1)
		{
			return;
		}
		int[] scoreArray = SingletonCustom<Hidden_CharacterManager>.Instance.GetScoreArray();
		for (int i = 1; i < scoreArray.Length; i++)
		{
			if (scoreArray[0] < scoreArray[i])
			{
				return;
			}
		}
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 2:
			SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOCK_WIPER);
			break;
		case 1:
			SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOCK_WIPER);
			break;
		case 0:
			SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOCK_WIPER);
			break;
		}
	}
}
