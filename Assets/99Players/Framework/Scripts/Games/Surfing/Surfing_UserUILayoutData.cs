using UnityEngine;
public class Surfing_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time[] commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private Surfing_UserUIData[] userUIDatas;
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
	public void SetUserUIData(Surfing_Define.UserType[] _userTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i]);
		}
	}
	public void SetUserScore(Surfing_Define.UserType _userType, int _score, int _point)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (userUIDatas[i].UserType == _userType)
			{
				userUIDatas[i].SetScore(_score);
				userUIDatas[i].GetPointUI(_point);
			}
		}
	}
	public void SetUserFade(Surfing_Define.UserType _userType, bool _set = true)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (userUIDatas[i].UserType == _userType)
			{
				userUIDatas[i].Fade(_set);
			}
		}
	}
	public void SetUserRanking()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (Surfing_Define.PM.Players[i].Surfer.processType != Surfing_Surfer.SurferProcessType.GOAL)
			{
				continue;
			}
			goalRankingCnt = 1;
			for (int j = 0; j < Surfing_Define.PM.UserData_Group1.Length; j++)
			{
				if (Surfing_Define.PM.UserData_Group1[i].point < Surfing_Define.PM.UserData_Group1[j].point)
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
			if (Surfing_Define.PM.CheckNowGroup1Playing())
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
			if (Surfing_Define.PM.Players[i].Surfer.processType != Surfing_Surfer.SurferProcessType.GOAL)
			{
				commonGameTime[i].SetTime(_time);
			}
		}
	}
}
