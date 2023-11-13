using UnityEngine;
public class RingToss_ScoreManager : SingletonCustom<RingToss_ScoreManager>
{
	private int[] arrayScore;
	private readonly int START_COMBO_BONUS_COUNT = 1;
	private readonly int LIMIT_COMBO_BONUS_VALUE = 2;
	public int[] ArrayScore => arrayScore;
	public int StartComboBonusCount => START_COMBO_BONUS_COUNT;
	public int LimitComboBonusValue => LIMIT_COMBO_BONUS_VALUE;
	public void Init()
	{
		arrayScore = new int[RingToss_Define.MAX_PLAYER_NUM];
		for (int i = 0; i < arrayScore.Length; i++)
		{
			arrayScore[i] = 0;
		}
	}
	public void SecondGroupInit()
	{
		arrayScore = new int[RingToss_Define.MAX_PLAYER_NUM];
		for (int i = 0; i < arrayScore.Length; i++)
		{
			arrayScore[i] = 0;
		}
	}
	public int[] GetCopyArrayScore()
	{
		int[] array = new int[arrayScore.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = arrayScore[i];
		}
		return array;
	}
	public void AddScore(int _ctrlNo, int _point, Vector3 _pos)
	{
		if (SingletonCustom<RingToss_GameManager>.Instance.GetIsPlayer(_ctrlNo))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
		}
		SingletonCustom<RingToss_UIManager>.Instance.PlayGetPointAnimation(_ctrlNo, _point, _pos);
		arrayScore[_ctrlNo] = Mathf.Clamp(arrayScore[_ctrlNo] + _point, 0, 99999);
		SingletonCustom<RingToss_UIManager>.Instance.UpdateScoreUI(_ctrlNo, arrayScore[_ctrlNo]);
	}
	public void SetScore(int _ctrlNo, int _point)
	{
		arrayScore[_ctrlNo] = Mathf.Clamp(arrayScore[_ctrlNo] + _point, 0, 99999);
		SingletonCustom<RingToss_UIManager>.Instance.UpdateScoreUI(_ctrlNo, arrayScore[_ctrlNo]);
	}
	public void CpuFutureScoreCalc()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		int remainingRingNum = SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum;
		float num = SingletonCustom<RingToss_GameManager>.Instance.Timer.RemainingTime;
		float num2 = RingToss_Define.AI_SKIP_ONE_THROW_TIMES[aiStrength];
		int num3 = 0;
		while (num > num2)
		{
			num -= num2;
			num3++;
			for (int i = 0; i < arrayScore.Length; i++)
			{
				if (!SingletonCustom<RingToss_GameManager>.Instance.GetIsPlayer(i) && num3 <= SingletonCustom<RingToss_ControllerManager>.Instance.ArrayController[i].RemainingRingNum && Random.Range(0, 100) < RingToss_Define.AI_SKIP_ONE_THROW_GET_PER[aiStrength])
				{
					RingToss_Target ringToss_Target = SingletonCustom<RingToss_TargetManager>.Instance.SearchRandomTarget();
					if (!(ringToss_Target == null))
					{
						arrayScore[i] += ringToss_Target.Point;
					}
				}
			}
		}
	}
}
