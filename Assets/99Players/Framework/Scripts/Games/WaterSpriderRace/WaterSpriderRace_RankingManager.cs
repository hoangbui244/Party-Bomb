using UnityEngine;
public class WaterSpriderRace_RankingManager : SingletonCustom<WaterSpriderRace_RankingManager>
{
	private Transform[] player;
	public int[] playerRanking;
	private int rankingCnt;
	public void Init()
	{
		player = new Transform[WaterSpriderRace_Define.MEMBER_NUM];
		playerRanking = new int[WaterSpriderRace_Define.MEMBER_NUM];
		for (int i = 0; i < player.Length; i++)
		{
			player[i] = WaterSpriderRace_Define.PM.Players[i].WaterSprider.gameObject.transform;
		}
	}
	private void FixedUpdate()
	{
		for (int i = 0; i < playerRanking.Length; i++)
		{
			rankingCnt = 0;
			for (int j = 0; j < player.Length; j++)
			{
				if (player[i].position.z < player[j].position.z)
				{
					rankingCnt++;
				}
			}
			playerRanking[i] = rankingCnt + 1;
		}
	}
}
