using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_GameResult : SingletonMonoBehaviour<FlyingSquirrelRace_GameResult>
{
	[SerializeField]
	[DisplayName("リザルトUI")]
	private RankingResultManager result;
	public void ShowResult()
	{
		ResultGameDataParams.SetPoint();
		FlyingSquirrelRace_Definition.Controller[] controllers = SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.Controllers;
		int[] scores = SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GetScores();
		AcquisitionTrophyIfAble(scores);
		int[] array = new int[controllers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (int)controllers[i];
		}
		ResultGameDataParams.SetRecord_Int(scores, array);
		result.ShowResult_Score();
	}
	private void AcquisitionTrophyIfAble(int[] scores)
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			return;
		}
		int num = scores.Max();
		if (scores[0] >= num)
		{
			switch (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.AIStrength)
			{
			case FlyingSquirrelRace_Definition.AIStrength.Easy:
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ARCHER_BATTLE);
				break;
			case FlyingSquirrelRace_Definition.AIStrength.Normal:
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ARCHER_BATTLE);
				break;
			case FlyingSquirrelRace_Definition.AIStrength.Hard:
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ARCHER_BATTLE);
				break;
			}
		}
	}
}
