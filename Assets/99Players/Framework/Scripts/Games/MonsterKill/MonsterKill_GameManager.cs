using System;
using UnityEngine;
public class MonsterKill_GameManager : SingletonCustom<MonsterKill_GameManager>
{
	[SerializeField]
	[Header("デバッグ：タイムの停止フラグ")]
	private bool isDebugTimeStop;
	[SerializeField]
	[Header("順位リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("UIル\u30fcト")]
	private GameObject rootUI;
	private float gameTime;
	private bool isGameStart;
	private bool isGameEnd;
	private bool isHidePointUI;
	public void Init()
	{
		gameTime = MonsterKill_Define.GAME_TIME;
		SingletonCustom<MonsterKill_UIManager>.Instance.SetTime(gameTime);
	}
	public void UpdateMethod()
	{
		if (isDebugTimeStop)
		{
			return;
		}
		gameTime -= Time.deltaTime;
		gameTime = Mathf.Clamp(gameTime, 0f, MonsterKill_Define.GAME_TIME);
		SingletonCustom<MonsterKill_UIManager>.Instance.SetTime(gameTime);
		if (!isHidePointUI && gameTime <= 10f)
		{
			isHidePointUI = true;
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				SingletonCustom<MonsterKill_UIManager>.Instance.HidePointUI(i, i);
			}
		}
		if (gameTime <= 0f)
		{
			GameEnd();
		}
	}
	public bool GetIsGameStart()
	{
		return isGameStart;
	}
	public bool GetIsGameEnd()
	{
		return isGameEnd;
	}
	public float GetGameTime()
	{
		return gameTime;
	}
	public void SetIsGameStart()
	{
		isGameStart = true;
	}
	public void GameStart()
	{
	}
	private void GameEnd()
	{
		UnityEngine.Debug.Log("ゲ\u30fcム終了処理");
		isGameEnd = true;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(i).SetGameEnd();
		}
		for (int j = 0; j < SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList().Count; j++)
		{
			SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList()[j].SetGameEnd();
		}
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++)
			{
				if (SingletonCustom<MonsterKill_CameraManager>.Instance.GetIsActive(k))
				{
					SingletonCustom<MonsterKill_UIManager>.Instance.HideUI(k);
				}
			}
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					int point = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(0).GetPoint();
					if (point >= MonsterKill_Define.BRONZE_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.RECEIVE_PON);
					}
					if (point >= MonsterKill_Define.SILVER_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.RECEIVE_PON);
					}
					if (point >= MonsterKill_Define.GOLD_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.RECEIVE_PON);
					}
				}
				ResultGameDataParams.SetPoint();
				int[] array = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; l++)
				{
					MonsterKill_Player player = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(l);
					array[l] = player.GetPoint();
					array2[l] = (int)player.GetUserType();
				}
				ResultGameDataParams.SetRecord_Int(array, array2);
				rankingResult.ShowResult_Score();
				LeanTween.delayedCall(base.gameObject, 1.25f, (Action)delegate
				{
					rootUI.SetActive(value: false);
				});
			});
		});
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(i).SetPoint(UnityEngine.Random.Range(5000, 7000));
		}
		GameEnd();
	}
}
