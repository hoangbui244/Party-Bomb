using SaveDataDefine;
using UnityEngine;
public class BlackSmith_PlayerManager : SingletonCustom<BlackSmith_PlayerManager>
{
	public enum State
	{
		CreateWeapon,
		HammerStrike,
		CreateWeaponComplete
	}
	public enum EvaluationType
	{
		Bad,
		Good,
		Nice,
		Perfect,
		Max
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private BlackSmith_Player[] arrayPlayer;
	private BlackSmith_Define.UserType changeAIStrengthUserType = BlackSmith_Define.UserType.MAX;
	private float[] arrayChangeAIStrengthDelayTime;
	[SerializeField]
	[Header("ハンマ\u30fcのマテリアル")]
	private Material[] arrayHammerMaterial;
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時のエフェクトを表示するまでの時間")]
	private float HAMMER_STRIKE_TIME;
	[SerializeField]
	[Header("ハンマ\u30fcを叩くアニメ\u30fcション時間")]
	private float HAMMER_STRIKE_ANIM_TIME;
	public void Init()
	{
		SystemData.AiStrength aiStrength = (SystemData.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		if ((uint)(aiStrength - 1) <= 1u && SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - SingletonCustom<GameSettingManager>.Instance.PlayerNum;
			arrayChangeAIStrengthDelayTime = new float[num];
			for (int i = 0; i < num; i++)
			{
				arrayChangeAIStrengthDelayTime[i] = Random.Range(2f, 6f) + (float)i * 3f;
			}
			arrayChangeAIStrengthDelayTime.Shuffle();
			switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
			case 1:
				changeAIStrengthUserType = (BlackSmith_Define.UserType)Random.Range(4, 7);
				break;
			case 2:
				changeAIStrengthUserType = (BlackSmith_Define.UserType)Random.Range(4, 6);
				break;
			}
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			int num2 = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0];
			int num3 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num2];
			arrayPlayer[j].Init(j, num2);
			arrayPlayer[j].SetHammerMaterial(arrayHammerMaterial[num3]);
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayPlayer[i].UpdateMethod();
		}
	}
	public BlackSmith_Player GetPlayer(int _playerNo)
	{
		return arrayPlayer[_playerNo];
	}
	public BlackSmith_Define.UserType GetChangeAIStrengthUserType()
	{
		return changeAIStrengthUserType;
	}
	public float GetChangeAIStrengthDelayTime(int _idx)
	{
		return arrayChangeAIStrengthDelayTime[_idx];
	}
	public float GetHammerStrikeTime()
	{
		return HAMMER_STRIKE_TIME;
	}
	public float GetHammerStrikeAnimationTime()
	{
		return HAMMER_STRIKE_ANIM_TIME;
	}
}
