using SaveDataDefine;
using UnityEngine;
public class MonsterKill_PlayerManager : SingletonCustom<MonsterKill_PlayerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private MonsterKill_Player[] arrayPlayer;
	[SerializeField]
	[Header("剣のマテリアル")]
	private Material[] arraySwordMaterial;
	[SerializeField]
	[Header("魔法の色")]
	private Gradient[] arrayMagicColor;
	[SerializeField]
	[Header("魔法弾を格納するアンカ\u30fc")]
	private Transform magicBulletEffectAnchor;
	[SerializeField]
	[Header("初期のＨＰ")]
	private int DEFAULT_HP;
	[SerializeField]
	[Header("ベ\u30fcスの移動速度")]
	private float BASE_MOVE_SPEED;
	[SerializeField]
	[Header("速度を徐\u3005に上げる速度")]
	private float CORRECTION_UP_DIFF_MOVE_SPEED;
	[SerializeField]
	[Header("速度を徐\u3005に下げる速度")]
	private float CORRECTION_DOWN_DIFF_MOVE_SPEED;
	[SerializeField]
	[Header("最高速度")]
	private float MAX_MOVE_SPEED;
	[SerializeField]
	[Header("ダッシュ速度補正")]
	private float DASH_SPEED;
	[SerializeField]
	[Header("魔法使用状態の移動速度補正")]
	private float USE_MAGIC_MOVE_SPEED;
	[SerializeField]
	[Header("スタミナを使い切った時の移動速度補正")]
	private float USE_UP_STAMINA_MOVE_SPEED;
	[SerializeField]
	[Header("最大スタミナ")]
	private int MAX_STAMINA;
	[SerializeField]
	[Header("スタミナが回復するまでの時間")]
	private float RECOVERY_STAMINA_WAIT_TIME;
	[SerializeField]
	[Header("スタミナ回復速度")]
	private float RECOVERY_STAMINA_SPEED;
	[SerializeField]
	[Header("スタミナを使い切った時のスタミナ回復速度補正")]
	private float USE_UP_RECOVERY_STAMINA_SPEED_MAG;
	[SerializeField]
	[Header("スタミナを使い切った時の再度使用可能になるスタミナ")]
	private float USE_UP_RE_USE_STAMINA;
	[SerializeField]
	[Header("スタミナ消費速度")]
	private float USE_STAMINA_SPEED;
	[SerializeField]
	[Header("ジャンプの強さ")]
	private float JUMP_POWER;
	[SerializeField]
	[Header("回避の強さ")]
	private float DODGE_POWER;
	[SerializeField]
	[Header("回避による消費スタミナ")]
	private float DODGE_STAMINA;
	[SerializeField]
	[Header("ダメ\u30fcジ判定中の時間")]
	private float DAMAGE_TIME;
	[SerializeField]
	[Header("気絶する時間")]
	private float STUN_TIME;
	private float[] arrayChangeAIStrengthDelayTime;
	public void Init()
	{
		SystemData.AiStrength aiStrength = (SystemData.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		if ((uint)(aiStrength - 1) <= 1u && SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - SingletonCustom<GameSettingManager>.Instance.PlayerNum;
			arrayChangeAIStrengthDelayTime = new float[num];
			for (int i = 0; i < num; i++)
			{
				arrayChangeAIStrengthDelayTime[i] = Random.Range(3f, 5f) + (float)i * 3f;
			}
			arrayChangeAIStrengthDelayTime.Shuffle();
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			int num2 = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0];
			int num3 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num2];
			arrayPlayer[j].Init(j, num2);
			arrayPlayer[j].SetSwordMaterial(arraySwordMaterial[num3]);
			arrayPlayer[j].SetMagicColor(arrayMagicColor[num3]);
			arrayPlayer[j].SetMiniMapIconColor(SingletonCustom<MonsterKill_UIManager>.Instance.GetMiniMapIconColor(num3));
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayPlayer[i].UpdateMethod();
		}
	}
	public MonsterKill_Player GetPlayer(int _playerNo)
	{
		return arrayPlayer[_playerNo];
	}
	public float GetChangeAIStrengthDelayTime(int _idx)
	{
		return arrayChangeAIStrengthDelayTime[_idx];
	}
	public Transform GetMagicBulletEffectAnchor()
	{
		return magicBulletEffectAnchor;
	}
	public int GetDefaultHP()
	{
		return DEFAULT_HP;
	}
	public float GetBaseMoveSpeed()
	{
		return BASE_MOVE_SPEED;
	}
	public float GetCorrectionUpDiffMoveSpeed()
	{
		return CORRECTION_UP_DIFF_MOVE_SPEED;
	}
	public float GetCorrectionDownDiffMoveSpeed()
	{
		return CORRECTION_DOWN_DIFF_MOVE_SPEED;
	}
	public float GetMaxMoveSpeed()
	{
		return MAX_MOVE_SPEED;
	}
	public float GetDashSpeed()
	{
		return DASH_SPEED;
	}
	public float GetUseMagicMoveSpeed()
	{
		return USE_MAGIC_MOVE_SPEED;
	}
	public float GetUseUpStaminaMoveSpeed()
	{
		return USE_UP_STAMINA_MOVE_SPEED;
	}
	public int GetMaxStamina()
	{
		return MAX_STAMINA;
	}
	public float GetRecoveryStaminaWaitTime()
	{
		return RECOVERY_STAMINA_WAIT_TIME;
	}
	public float GetRecoveryStaminaSpeed()
	{
		return RECOVERY_STAMINA_SPEED;
	}
	public float GetUseUpRecoveryStaminaSpeedMag()
	{
		return USE_UP_RECOVERY_STAMINA_SPEED_MAG;
	}
	public float GetUseUpReUseStamina()
	{
		return USE_UP_RE_USE_STAMINA;
	}
	public float GetUseStaminaSpeed()
	{
		return USE_STAMINA_SPEED;
	}
	public float GetJumpPower()
	{
		return JUMP_POWER;
	}
	public float GetDodgePower()
	{
		return DODGE_POWER;
	}
	public float GetDodgeStamina()
	{
		return DODGE_STAMINA;
	}
	public float GetDamageTime()
	{
		return DAMAGE_TIME;
	}
	public float GetStunTime()
	{
		return STUN_TIME;
	}
}
