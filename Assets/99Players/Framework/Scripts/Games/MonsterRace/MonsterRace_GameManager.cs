using System;
using System.Collections;
using UnityEngine;
public class MonsterRace_GameManager : SingletonCustom<MonsterRace_GameManager>
{
	[SerializeField]
	private RankingResultManager resultManager;
	private const float AUDIENCE_SE_TIME = 1f;
	private const float AUDIENCE_SE_PLAY_INTERVAL = 0.7f;
	private bool isGameStart;
	private bool isGameEnd;
	private float gameTime;
	private bool hasNextRun;
	private bool isNowNextRun;
	private float audienceSePlayTime;
	private float audienceSeTimer;
	private int audienceSeCount;
	private bool isAudienceSeReset;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public int GameMinute => (int)gameTime / 60;
	public int GameSecond => (int)gameTime % 60;
	public bool HasNextRun
	{
		get
		{
			return hasNextRun;
		}
		set
		{
			hasNextRun = value;
		}
	}
	public bool IsNowNextRun
	{
		get
		{
			return isNowNextRun;
		}
		set
		{
			isNowNextRun = value;
		}
	}
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		StartCoroutine(_OpenGameDirection());
		audienceSeTimer = 0.7f;
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			if (SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum == 1)
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.FirstSingleCameraDirection();
				LeanTween.delayedCall(1.6f, (Action)delegate
				{
					SingletonCustom<CommonStartProduction>.Instance.Play(delegate
					{
						GameStart();
					});
				});
			}
			else
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.FirstCameraDirection();
				SingletonCustom<CommonStartProduction>.Instance.Play(delegate
				{
					GameStart();
				});
			}
			SingletonCustom<MonsterRace_CarManager>.Instance.SetCharaMoveActive(_active: true);
		});
		SingletonCustom<CommonNotificationManager>.Instance.OnCourseSelectCursorMove.AddListener(delegate
		{
			SingletonCustom<MonsterRace_CarManager>.Instance.ChangeStartBeforeCourse(SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx);
		});
	}
	public void NextRunInit()
	{
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		StartCoroutine(_NextGameDirection());
	}
	private IEnumerator _NextGameDirection()
	{
		yield return new WaitForSeconds(2f);
		SingletonCustom<MonsterRace_CarManager>.Instance.FirstCameraDirection();
		SingletonCustom<CommonOnYourMarks>.Instance.Play(delegate
		{
			GameStart();
		});
		SingletonCustom<MonsterRace_CarManager>.Instance.SetCharaMoveActive(_active: true);
	}
	public void UpdateMethod()
	{
		if (!isGameStart || isGameEnd)
		{
			return;
		}
		if (Time.time - audienceSePlayTime < 1f)
		{
			isAudienceSeReset = false;
			audienceSeTimer += Time.deltaTime;
			if (audienceSeTimer > 0.7f)
			{
				audienceSeTimer -= 0.7f;
				switch (audienceSeCount % 6)
				{
				case 0:
				case 2:
				{
					float volume = (audienceSeCount == 0) ? 1f : 0.5f;
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, volume, 1f, 0f, _overlap: true);
					break;
				}
				case 4:
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
					break;
				case 1:
				case 3:
				case 5:
					SingletonCustom<AudioManager>.Instance.SePlay("se_applause_0", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
					break;
				}
				audienceSeCount++;
			}
		}
		else if (!isAudienceSeReset)
		{
			isAudienceSeReset = true;
			audienceSeTimer = 0.7f;
			audienceSeCount = 0;
		}
	}
	public void AudienceSeAreaIn()
	{
		audienceSePlayTime = Time.time;
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<MonsterRace_CarManager>.Instance.GameStart();
		StartCoroutine(_RankUiStartDirection());
	}
	private IEnumerator _RankUiStartDirection()
	{
		SingletonCustom<MonsterRace_UiManager>.Instance.ShowRankSprite(_isFade: true);
		yield return new WaitForSeconds(1f);
		SingletonCustom<MonsterRace_UiManager>.Instance.IsCanChangeRank = true;
	}
	public void GameEnd()
	{
		if (hasNextRun && !isNowNextRun)
		{
			NextRunInit();
			isNowNextRun = true;
		}
		else
		{
			isGameEnd = true;
			StartCoroutine(_GameEnd());
		}
	}
	private IEnumerator _GameEnd()
	{
		resultManager.ShowResult_Time();
		yield return new WaitForSeconds(1.5f);
		SingletonCustom<MonsterRace_CarManager>.Instance.EndCamera();
	}
	private IEnumerator _StartDirection()
	{
		yield return null;
	}
}
