using UnityEngine;
public class LegendarySword_AI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private LegendarySword_Player player;
	[SerializeField]
	[Header("キャラクタ\u30fc処理")]
	private LegendarySword_Chara chara;
	private int playerNo;
	private LegendarySword_Define.AiStrength aiStrength;
	private int checkPointIdx;
	private int balance;
	private float waitTimeBase;
	private float waitTimeAdd;
	private float time;
	public int PlayerNo => playerNo;
	public int CheckPointIdx => checkPointIdx;
	public void Init()
	{
		aiStrength = (LegendarySword_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		checkPointIdx = 0;
		for (int i = 0; i < LegendarySword_Define.PM.Players.Length; i++)
		{
			if (LegendarySword_Define.PM.Players[i].UserType <= LegendarySword_Define.UserType.PLAYER_4)
			{
				balance++;
			}
			else
			{
				balance--;
			}
		}
		waitTimeAdd = 0f;
		switch (aiStrength)
		{
		case LegendarySword_Define.AiStrength.WEAK:
			waitTimeBase = 0.3f;
			break;
		case LegendarySword_Define.AiStrength.COMMON:
			waitTimeBase = 0.2f;
			break;
		case LegendarySword_Define.AiStrength.STRONG:
			waitTimeBase = 0.1f;
			break;
		}
	}
	public void BattleStartControl()
	{
		switch (SingletonCustom<LegendarySword_GameManager>.Instance.CurrentTournamentType)
		{
		case LegendarySword_GameManager.RoundType.Round_Final:
			waitTimeAdd = -0.02f;
			break;
		case LegendarySword_GameManager.RoundType.LoserBattle:
			waitTimeAdd = 0.025f;
			break;
		}
	}
	public void UpdateMethod()
	{
		time += Time.deltaTime;
		float num = UnityEngine.Random.Range(0f, 0.05f);
		if (time > waitTimeBase + waitTimeAdd + num)
		{
			if (chara.AccelInput())
			{
				player.CharacterShake();
			}
			time = 0f;
		}
	}
}
