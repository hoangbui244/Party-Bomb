using System;
using UnityEngine;
public class BlackSmith_GameManager : SingletonCustom<BlackSmith_GameManager>
{
	[SerializeField]
	[Header("順位リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("UIル\u30fcト")]
	private GameObject rootUI;
	private float gameTime;
	private bool isGameStart;
	private bool isGameEnd;
	private bool isGaugeBarSpeedUp;
	[SerializeField]
	[Header("時間によってゲ\u30fcジの速度を上げるかどうかのフラグ")]
	private bool isTimeGaugeSpeedUP = true;
	private bool isHidePlayerUI;
	public void Init()
	{
		gameTime = BlackSmith_Define.GAME_TIME;
		SingletonCustom<BlackSmith_UIManager>.Instance.SetTime(gameTime);
	}
	public void UpdateMethod()
	{
		gameTime -= Time.deltaTime;
		gameTime = Mathf.Clamp(gameTime, 0f, BlackSmith_Define.GAME_TIME);
		SingletonCustom<BlackSmith_UIManager>.Instance.SetTime(gameTime);
		if (!isHidePlayerUI && gameTime <= 10f)
		{
			isHidePlayerUI = true;
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				SingletonCustom<BlackSmith_UIManager>.Instance.HidePointUI(i);
			}
		}
		if (isTimeGaugeSpeedUP && !isGaugeBarSpeedUp && gameTime <= 30f)
		{
			isGaugeBarSpeedUp = true;
			SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
			SingletonCustom<BlackSmith_UIManager>.Instance.SetIsShowSpeedUpText(_isShowSpeedUpText: true);
			LeanTween.delayedCall(base.gameObject, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime() * 2f + SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIShowTime(), (Action)delegate
			{
				SingletonCustom<BlackSmith_UIManager>.Instance.SetIsShowSpeedUpText(_isShowSpeedUpText: false);
			});
			for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
			{
				SingletonCustom<BlackSmith_UIManager>.Instance.ShowSpeedUpText(j);
			}
			if (SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_main_0"))
			{
				SingletonCustom<AudioManager>.Instance.BgmStop();
				LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
				{
					SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_main_0", _loop: true, 0f, 1f, 1.2f);
				});
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
	public bool GetIsTimeGaugeSpeedUP()
	{
		return isTimeGaugeSpeedUP;
	}
	public bool GetIsGaugeBarSpeedUp()
	{
		return isGaugeBarSpeedUp;
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
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				SingletonCustom<BlackSmith_UIManager>.Instance.HideUI(i);
			}
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					int createWeaponCnt = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(0).GetCreateWeaponCnt();
					if (createWeaponCnt >= BlackSmith_Define.BRONZE_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.DELIVERY_ORDER);
					}
					if (createWeaponCnt >= BlackSmith_Define.SILVER_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.DELIVERY_ORDER);
					}
					if (createWeaponCnt >= BlackSmith_Define.GOLD_BORDER)
					{
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.DELIVERY_ORDER);
					}
				}
				ResultGameDataParams.SetPoint();
				int[] array = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
				{
					BlackSmith_Player player = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(j);
					array[j] = player.GetCreateWeaponCnt();
					array2[j] = (int)player.GetUserType();
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
			SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(i).SetCreateWeaponCnt(UnityEngine.Random.Range(5, 10));
		}
		GameEnd();
	}
}
