using UnityEngine;
public class Bowling_ScoreBoardUI : MonoBehaviour
{
	[SerializeField]
	[Header("スコアフレ\u30fcムUI")]
	private Bowling_ScoreFrameUI[] scoreFrameUI;
	public void Init()
	{
		for (int i = 0; i < scoreFrameUI.Length; i++)
		{
			if (i < Bowling_Define.MEMBER_NUM)
			{
				scoreFrameUI[i].gameObject.SetActive(value: true);
				scoreFrameUI[i].Init(Bowling_Define.MPM.MemberOrder[i]);
			}
			else
			{
				scoreFrameUI[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void StartFrameFlashing(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < scoreFrameUI.Length; i++)
		{
			if (scoreFrameUI[i].UserType == _userType)
			{
				scoreFrameUI[i].StartFrameFlashing();
			}
			else
			{
				scoreFrameUI[i].StopFrameFlashing();
			}
		}
	}
	public void SetData(Bowling_GameUIManager.ScoreData _data, int _frameNo, Bowling_Define.UserType _userType)
	{
		int num = 0;
		while (true)
		{
			if (num < scoreFrameUI.Length)
			{
				if (scoreFrameUI[num].UserType == _userType)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		scoreFrameUI[num].SetScore(_data, _frameNo);
	}
	public int GetTotalScore(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < scoreFrameUI.Length; i++)
		{
			if (scoreFrameUI[i].UserType == _userType)
			{
				return scoreFrameUI[i].GetTotalScore();
			}
		}
		return 0;
	}
	public void SetDebugRecord()
	{
		for (int i = 0; i < scoreFrameUI.Length; i++)
		{
			scoreFrameUI[i].SetTotalScore(UnityEngine.Random.Range(10, 60));
		}
	}
}
