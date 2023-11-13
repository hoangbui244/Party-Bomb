using UnityEngine;
public class SnowBoard_RankingManager : SingletonCustom<SnowBoard_RankingManager>
{
	[SerializeField]
	[Header("SnowBoardオブジェクト")]
	private Transform[] player;
	[SerializeField]
	[Header("SnowBoard_AI")]
	private SnowBoard_AI[] ai;
	public int[] playerRanking;
	private int rankingCnt;
	public void Init()
	{
		playerRanking = new int[player.Length];
	}
	private void FixedUpdate()
	{
		for (int i = 0; i < playerRanking.Length; i++)
		{
			rankingCnt = 0;
			for (int j = 0; j < player.Length; j++)
			{
				if (player[j].gameObject.activeInHierarchy)
				{
					if (ai[i].CurrentCheckPointIdx < ai[j].CurrentCheckPointIdx)
					{
						rankingCnt++;
					}
					else if (ai[i].CurrentCheckPointIdx == ai[j].CurrentCheckPointIdx && (player[i].position - SingletonCustom<SnowBoard_CourseManager>.Instance.GetCheckPointAnchor(ai[i].CurrentCheckPointIdx).position).sqrMagnitude > (player[j].position - SingletonCustom<SnowBoard_CourseManager>.Instance.GetCheckPointAnchor(ai[i].CurrentCheckPointIdx).position).sqrMagnitude)
					{
						rankingCnt++;
					}
				}
			}
			playerRanking[i] = rankingCnt + 1;
		}
	}
}
