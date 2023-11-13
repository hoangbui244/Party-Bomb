using UnityEngine;
public class SnowBoard_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time[] commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private SnowBoard_UserUIData[] userUIDatas;
	[SerializeField]
	[Header("～組目の画像")]
	private SpriteRenderer groupNumberSp;
	private int goalRankingCnt;
	public void Init()
	{
		if (groupNumberSp != null)
		{
			groupNumberSp.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < commonGameTime.Length; i++)
		{
			commonGameTime[i].SetTime(0f);
		}
	}
	public void SetUserUIData(SnowBoard_Define.UserType[] _userTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i]);
		}
	}
	public void SetUserScore(SnowBoard_Define.UserType _userType, int _score)
	{
	}
	public void SetUserRanking()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (SnowBoard_Define.PM.Players[i].SkiBoard.processType != SnowBoard_SkiBoard.SkiBoardProcessType.GOAL)
			{
				userUIDatas[i].SetRanking(SingletonCustom<SnowBoard_RankingManager>.Instance.playerRanking[i]);
				continue;
			}
			goalRankingCnt = 1;
			for (int j = 0; j < SnowBoard_Define.PM.UserData_Group1.Length; j++)
			{
				if (SnowBoard_Define.PM.UserData_Group1[i].goalTime > SnowBoard_Define.PM.UserData_Group1[j].goalTime)
				{
					goalRankingCnt++;
				}
			}
			userUIDatas[i].SetRanking(goalRankingCnt);
			userUIDatas[i].SetGoalRanking();
		}
	}
	public void ShowControlInfoBalloon()
	{
	}
	public void HideControlInfoBalloon()
	{
	}
	public void TrickUICall(int _player, SnowBoard_UserUIData.TrickType _set)
	{
		if (_player <= userUIDatas.Length)
		{
			userUIDatas[_player].TrickUICall(_set);
		}
	}
	public void SetGameTime(float _time)
	{
		for (int i = 0; i < commonGameTime.Length; i++)
		{
			if (SnowBoard_Define.PM.Players[i].SkiBoard.processType != SnowBoard_SkiBoard.SkiBoardProcessType.GOAL)
			{
				commonGameTime[i].SetTime(_time);
			}
		}
	}
}
