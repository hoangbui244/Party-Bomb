using UnityEngine;
public class WaterSpriderRace_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time[] commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private WaterSpriderRace_UserUIData[] userUIDatas;
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
	public void SetUserUIData(WaterSpriderRace_Define.UserType[] _userTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i]);
		}
	}
	public void SetUserScore(WaterSpriderRace_Define.UserType _userType, int _score)
	{
	}
	public void SetUserRanking()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (WaterSpriderRace_Define.PM.Players[i].WaterSprider.processType != WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL)
			{
				userUIDatas[i].SetRanking(SingletonCustom<WaterSpriderRace_RankingManager>.Instance.playerRanking[i]);
				continue;
			}
			goalRankingCnt = 1;
			for (int j = 0; j < WaterSpriderRace_Define.PM.UserData_Group1.Length; j++)
			{
				if (WaterSpriderRace_Define.PM.UserData_Group1[i].goalTime > WaterSpriderRace_Define.PM.UserData_Group1[j].goalTime)
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
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].FadeProcess_ControlInfomationUI(_fadeIn: true);
			userUIDatas[i].FadeOutPlayerIcon();
		}
	}
	public void HideControlInfoBalloon()
	{
	}
	public void CourseOutWarning(int _player)
	{
		if (_player <= userUIDatas.Length)
		{
			userUIDatas[_player].CourseOutWarning();
		}
	}
	public void SetGroupNumberIcon()
	{
		if (!(groupNumberSp == null))
		{
			groupNumberSp.gameObject.SetActive(value: true);
			if (WaterSpriderRace_Define.PM.CheckNowGroup1Playing())
			{
				groupNumberSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_1group");
			}
			else
			{
				groupNumberSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
			}
		}
	}
	public void SetGameTime(float _time)
	{
		for (int i = 0; i < commonGameTime.Length; i++)
		{
			if (WaterSpriderRace_Define.PM.Players[i].WaterSprider.processType != WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL)
			{
				commonGameTime[i].SetTime(_time);
			}
		}
	}
}
