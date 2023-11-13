public class FindMask_ScoreManager : SingletonCustom<FindMask_ScoreManager>
{
	private int[] arrayScore;
	public int[] ArrayScore => arrayScore;
	public void Init()
	{
		if (!SingletonCustom<FindMask_GameManager>.Instance.IsEightPlsyers)
		{
			arrayScore = new int[FindMask_Define.CHARA_NUM];
		}
		else
		{
			arrayScore = new int[FindMask_Define.CHARA_NUM * 2];
		}
		for (int i = 0; i < arrayScore.Length; i++)
		{
			arrayScore[i] = 0;
		}
	}
	public void AddCharaScore(int _point)
	{
		arrayScore[SingletonCustom<FindMask_GameManager>.Instance.CurrentTurnNum] += _point;
	}
	public void SetDebugRecord()
	{
		for (int i = 0; i < arrayScore.Length; i++)
		{
			arrayScore[i] = i * 10;
		}
	}
	public bool WinJudgPlayer()
	{
		for (int i = 0; i < arrayScore.Length; i++)
		{
			if (i != 0 && arrayScore[0] < arrayScore[i])
			{
				return false;
			}
		}
		return true;
	}
	public int GetTeamTotalScore(int _teamNum)
	{
		int num = 0;
		for (int i = 0; i < arrayScore.Length; i++)
		{
			if (SingletonCustom<FindMask_GameManager>.Instance.ATeamIn1P())
			{
				if (_teamNum == 0)
				{
					if (i % 2 == 0)
					{
						num += arrayScore[i];
					}
				}
				else if (i % 2 != 0)
				{
					num += arrayScore[i];
				}
			}
			else if (_teamNum != 0)
			{
				if (i % 2 == 0)
				{
					num += arrayScore[i];
				}
			}
			else if (i % 2 != 0)
			{
				num += arrayScore[i];
			}
		}
		return num;
	}
}
