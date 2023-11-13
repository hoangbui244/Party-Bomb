using UnityEngine;
public class AlpineSkiing_RankingManager : SingletonCustom<AlpineSkiing_RankingManager> {
    private Transform[] player;
    public int[] playerRanking;
    private int rankingCnt;
    public void Init() {
        player = new Transform[AlpineSkiing_Define.MEMBER_NUM];
        playerRanking = new int[AlpineSkiing_Define.MEMBER_NUM];
        for (int i = 0; i < player.Length; i++) {
            player[i] = AlpineSkiing_Define.PM.Players[i].SkiBoard.transform;
        }
    }
    private void FixedUpdate() {
        for (int i = 0; i < playerRanking.Length; i++) {
            rankingCnt = 0;
            for (int j = 0; j < player.Length; j++) {
                if (player[i].position.z < player[j].position.z) {
                    rankingCnt++;
                }
            }
            playerRanking[i] = rankingCnt + 1;
        }
    }
}
