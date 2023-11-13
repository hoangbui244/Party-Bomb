using UnityEngine;
public class Shooting_ScoreManager : SingletonCustom<Shooting_ScoreManager>
{
	private int[] arrayScore;
	private readonly int START_COMBO_BONUS_COUNT = 1;
	private readonly int LIMIT_COMBO_BONUS_VALUE = 2;
	public int[] ArrayScore => arrayScore;
	public int StartComboBonusCount => START_COMBO_BONUS_COUNT;
	public int LimitComboBonusValue => LIMIT_COMBO_BONUS_VALUE;
	public void Init()
	{
		arrayScore = new int[SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM];
		for (int i = 0; i < arrayScore.Length; i++)
		{
			arrayScore[i] = 0;
		}
	}
	public void SecondGroupInit()
	{
		arrayScore = new int[SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM];
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
	public void AddScore(int _gunNo, Sprite _scoreSprite, Vector3 _pos, int _point, Vector3 _scale)
	{
		SingletonCustom<Shooting_UIManager>.Instance.PlayGetPointAnimation(_gunNo, _scoreSprite, _pos, _scale);
		arrayScore[_gunNo] = Mathf.Clamp(arrayScore[_gunNo] + _point, 0, 99999);
		SingletonCustom<Shooting_UIManager>.Instance.UpdateScoreUI(_gunNo, arrayScore[_gunNo]);
	}
	public void SetScore(int _gunNo, int _point)
	{
		arrayScore[_gunNo] = Mathf.Clamp(arrayScore[_gunNo] + _point, 0, 99999);
		SingletonCustom<Shooting_UIManager>.Instance.UpdateScoreUI(_gunNo, arrayScore[_gunNo]);
	}
	public void CpuFutureScoreCalc()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		int remainingBulletNum = SingletonCustom<Shooting_ControllerManager>.Instance.RemainingBulletNum;
		float num = SingletonCustom<Shooting_GameManager>.Instance.Timer.RemainingTime;
		float num2 = Shooting_Define.AI_SKIP_ONE_SHOT_TIMES[aiStrength];
		int num3 = 0;
		while (num > num2)
		{
			num -= num2;
			num3++;
			for (int i = 0; i < arrayScore.Length; i++)
			{
				if (SingletonCustom<Shooting_GameManager>.Instance.GetIsPlayer(i) || num3 > SingletonCustom<Shooting_ControllerManager>.Instance.ArrayController[i].RemainingBulletNum)
				{
					continue;
				}
				if (arrayScore[i] >= Shooting_Define.AI_MAXSCORE_SHOT[aiStrength])
				{
					UnityEngine.Debug.Log("点数超えてたよ～");
					continue;
				}
				Shooting_Target shooting_Target = SingletonCustom<Shooting_TargetManager>.Instance.SearchRandomTarget();
				for (int j = 0; j < shooting_Target.TARGETData.Length; j++)
				{
					if (Random.Range(0, 101) <= Shooting_Define.AI_SKIP_ONE_SHOT_DROP_PER[j] - Shooting_Define.AI_SKIP_ONE_SHOT_DROP_MINUS[aiStrength])
					{
						UnityEngine.Debug.Log((i + 1).ToString() + "Pに" + shooting_Target.TARGETData[j].point.ToString() + "Point追加しました。");
						arrayScore[i] += shooting_Target.TARGETData[j].point;
						break;
					}
				}
				bool flag = shooting_Target == null;
			}
		}
	}
}
