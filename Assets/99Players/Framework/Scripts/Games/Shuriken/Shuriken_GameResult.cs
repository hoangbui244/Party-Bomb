using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_GameResult : SingletonMonoBehaviour<Shuriken_GameResult>
{
	private static readonly float[] AverageScores = new float[4];
	[SerializeField]
	[DisplayName("ランキングリザルト")]
	private RankingResultManager rankingResult;
	public void ShowResult()
	{
		ResultGameDataParams.SetPoint();
		int[] scores = SingletonMonoBehaviour<Shuriken_Players>.Instance.GetScores();
		AcquisitionTrophyIfAble(scores[0]);
		int[] userIds = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.GetUserIds();
		ResultGameDataParams.SetRecord_Int(scores, userIds);
		for (int i = 0; i < scores.Length; i++)
		{
			if (AverageScores[i] == 0f)
			{
				AverageScores[i] = scores[i];
			}
			AverageScores[i] *= 0.9f;
			AverageScores[i] += (float)scores[i] * 0.1f;
		}
		rankingResult.ShowResult_Score();
	}
	private void AcquisitionTrophyIfAble(int score)
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || (float)score < Shuriken_Definition.BronzeScore)
		{
			return;
		}
		UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
		SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.GET_BALL);
		if (!((float)score < Shuriken_Definition.SilverScore))
		{
			UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
			SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.GET_BALL);
			if (!((float)score < Shuriken_Definition.GoldScore))
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.GET_BALL);
			}
		}
	}
}
