using GamepadInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Scuba_GameManager : SingletonCustom<Scuba_GameManager>
{
	public const float TIME_LIMIT = 120f;
	private const int TROPHY_GOLD_SCORE = 3500;
	private const int TROPHY_SILVER_SCORE = 2000;
	private const int TROPHY_BRONZE_SCORE = 1000;
	private static bool IS_REBOOT_FIRST_PLAY = true;
	[SerializeField]
	private RankingResultManager resultManager;
	[SerializeField]
	private GameObject[] startDirectionObjs;
	[SerializeField]
	private GameObject[] startGameObjs;
	[SerializeField]
	private ParticleSystem[] startGameEffects;
	[SerializeField]
	private PlayableDirector playableDirector;
	[SerializeField]
	private GameObject firstPlayObj;
	[SerializeField]
	private GameObject firstPlayRootObj;
	private bool isFirstPlayPause;
	private bool isGameStart;
	private bool isGameEnd;
	private float gameTime;
	private float limitTime;
	private int countDownCount = 3;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public float GameTime => gameTime;
	public int GameMinute => (int)gameTime / 60;
	public int GameSecond => (int)gameTime % 60;
	public int LimitMinute
	{
		get
		{
			if (!isGameStart)
			{
				return 2;
			}
			return (int)Mathf.Max(0f, 120f - limitTime + 1f) / 60;
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
			return (int)Mathf.Max(0f, 120f - limitTime + 1f) % 60;
		}
	}
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		OpenGameDirection();
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
			if (IS_REBOOT_FIRST_PLAY)
			{
				isFirstPlayPause = (!SingletonCustom<SaveDataManager>.Instance.SaveData.isStartHelp[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType] || SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp);
				firstPlayObj.SetActive(value: true);
				firstPlayRootObj.SetActive(!isFirstPlayPause);
				playableDirector.Pause();
				SingletonCustom<Scuba_UiManager>.Instance.SetUIActive(_active: false);
				StartCoroutine(_FirstPlayDirection());
			}
		});
	}
	private void CommonCountDownStart()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(delegate
		{
			GameStart();
		});
	}
	private IEnumerator _FirstPlayDirection()
	{
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
				SingletonCustom<Scuba_UiManager>.Instance.SetUIActive(_active: true);
				playableDirector.Play();
			}
			yield return null;
		}
	}
	public void UpdateMethod()
	{
		if (isGameStart && !isGameEnd)
		{
			gameTime += Time.deltaTime;
			limitTime = gameTime;
			SingletonCustom<Scuba_UiManager>.Instance.SetTime(120f - limitTime);
			if (120f - limitTime < (float)countDownCount)
			{
				countDownCount--;
			}
			if (120f < gameTime)
			{
				GameEnd();
			}
		}
	}
	public void StartDirectionEnd()
	{
		for (int i = 0; i < startDirectionObjs.Length; i++)
		{
			startDirectionObjs[i].SetActive(value: false);
		}
		for (int j = 0; j < startGameObjs.Length; j++)
		{
			startGameObjs[j].SetActive(value: true);
		}
		for (int k = 0; k < startGameEffects.Length; k++)
		{
			startGameEffects[k].Play();
		}
		SingletonCustom<Scuba_CharacterManager>.Instance.StartChara();
		CommonCountDownStart();
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<Scuba_CharacterManager>.Instance.SetCharaMoveActive(_active: true);
	}
	public void GameEnd()
	{
		if (!isGameEnd)
		{
			isGameEnd = true;
			SingletonCustom<Scuba_CharacterManager>.Instance.SetCharaMoveActive(_active: false);
			SingletonCustom<Scuba_CharacterManager>.Instance.SetCameraMoveActive(_active: false);
			SingletonCustom<Scuba_CharacterManager>.Instance.EndChara();
			SingletonCustom<Scuba_UiManager>.Instance.CloseGameUI();
			StartCoroutine(_GameEnd());
		}
	}
	private IEnumerator _GameEnd()
	{
		SingletonCustom<CommonEndSimple>.Instance.Show();
		yield return new WaitForSeconds(3f);
		if (SingletonCustom<Scuba_CharacterManager>.Instance.PlayerNum == 1)
		{
			CheckTrophy();
		}
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		int num = 4;
		int[] array = new int[num];
		int[] array2 = new int[num];
		for (int i = 0; i < num; i++)
		{
			Scuba_CharacterScript chara = SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(i);
			array2[i] = chara.PlayerNo;
			array[i] = chara.Score;
			if (i > 3)
			{
				playerGroupList[i % 4].Add(array2[i]);
			}
		}
		ResultGameDataParams.SetRecord_Int(array, array2);
		resultManager.ShowResult_Score();
		yield return new WaitForSeconds(1.5f);
		SingletonCustom<Scuba_CharacterManager>.Instance.EndCamera();
	}
	private void CheckTrophy()
	{
		if (SingletonCustom<Scuba_CharacterManager>.Instance.PlayerNum == 1)
		{
			int num = SingletonCustom<Scuba_CharacterManager>.Instance.GetScoreArray()[0];
			if (num >= 3500)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOCK_WIPER);
			}
			if (num >= 2000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOCK_WIPER);
			}
			if (num >= 1000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOCK_WIPER);
			}
		}
	}
}
