using UnityEngine;
public class Canoe_PlayerManager : SingletonCustom<Canoe_PlayerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private Canoe_Player[] arrayPlayer;
	[SerializeField]
	[Header("カヌ\u30fcのマテリアル")]
	private Material[] arrayCanoeMaterial;
	[SerializeField]
	[Header("アニメ\u30fcションの開始をバラつかせる時間")]
	private float[] arrayStartAnimationTime;
	[SerializeField]
	[Header("移動速度")]
	private float BASE_MOVE_SPEED;
	[SerializeField]
	[Header("速度を徐\u3005に上げる速度")]
	private float CORRECTION_UP_DIFF_MOVE_SPEED;
	[SerializeField]
	[Header("速度を徐\u3005に下げる速度")]
	private float CORRECTION_DOWN_DIFF_MOVE_SPEED;
	[SerializeField]
	[Header("入力間隔の最大値（この値以上の入力間隔の場合は、最低速度で走る）")]
	private float MAX_INPUT_INTERVAL;
	[SerializeField]
	[Header("入力継続時間の最大値")]
	private float MAX_INPUT_CONTINUE_TIME;
	[SerializeField]
	[Header("最大速度（前進用）")]
	private float MAX_ACCELE_MOVE_SPEED;
	[SerializeField]
	[Header("最大速度（後退用）")]
	private float MAX_FALL_BACK_MOVE_SPEED;
	[SerializeField]
	[Header("最低速度")]
	private float MIN_MOVE_SPEED;
	[SerializeField]
	[Header("滝落下による最大加速値")]
	private float MAX_ADD_SPEED_WATER_FALL;
	[SerializeField]
	[Header("岩間通過による最大加速値")]
	private float MAX_ADD_SPEED_AMONG_ROCKS;
	[SerializeField]
	[Header("スリップストリ\u30fcムによる最大加速値")]
	private float MAX_SLIP_STREAM_ADD_SPEED;
	[SerializeField]
	[Header("回転速度")]
	private float ROT_SPEED;
	[SerializeField]
	[Header("Ｘ軸の角度制限")]
	private float LIMIT_ROT_X;
	[SerializeField]
	[Header("落下判定用のＹ方向のベクトル")]
	private float FALL_VELOCITY_Y;
	[SerializeField]
	[Header("スタミナを消費による強制加速量")]
	private float STAMINA_USE_ACCELE_MOVE_SPEED;
	[SerializeField]
	[Header("スタミナを消費する速度")]
	private float STAMINA_USE_SPEED;
	[SerializeField]
	[Header("スタミナが回復する速度")]
	private float STAMINA_HEAL_SPEED;
	[SerializeField]
	[Header("スタミナを使い切った時の回復する速度にかける補正倍率")]
	private float USE_UP_STAMINA_HEAL_SPEED_MAG;
	[SerializeField]
	[Header("スタミナを使い切った時の回復しはじめるまでの待機時間")]
	private float USE_UP_STAMINA_HEAL_WAIT_TIME;
	[SerializeField]
	[Header("スタミナを使い切った時の再度使用可能になるスタミナ値")]
	private float USE_UP_STAMINA_RE_USE_VALUE;
	[SerializeField]
	[Header("スタミナを使い切った時の移動速度に補正をかける倍率")]
	private float USE_UP_STAMINA_MOVE_SPEED_MAG;
	public void Init()
	{
		arrayStartAnimationTime.Shuffle();
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num];
			arrayPlayer[i].Init(i, num);
			arrayPlayer[i].SetCanoeMaterial(arrayCanoeMaterial[num2]);
			arrayPlayer[i].SetStartAnimation(arrayStartAnimationTime[i]);
			UnityEngine.Debug.Log("i : " + i.ToString() + " arrayStartAnimationTime : " + arrayStartAnimationTime[i].ToString());
		}
	}
	public void FixedUpdateMethod()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (!arrayPlayer[i].GetIsMoveGoalAnchor())
			{
				arrayPlayer[i].FixedUpdateMethod();
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (!arrayPlayer[i].GetIsMoveGoalAnchor())
			{
				arrayPlayer[i].UpdateMethod();
			}
		}
	}
	public Canoe_Player GetPlayer(int _playerNo)
	{
		return arrayPlayer[_playerNo];
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
	public float GetMaxAcceleMoveSpeed()
	{
		return MAX_ACCELE_MOVE_SPEED;
	}
	public float GetMaxFallBackMoveSpeed()
	{
		return MAX_FALL_BACK_MOVE_SPEED;
	}
	public float GetMaxAddSpeed(Canoe_AddSpeedUp.Type _addSpeedUpType)
	{
		switch (_addSpeedUpType)
		{
		case Canoe_AddSpeedUp.Type.WATER_FALL:
			return MAX_ADD_SPEED_WATER_FALL;
		case Canoe_AddSpeedUp.Type.AMONG_ROCKS:
			return MAX_ADD_SPEED_AMONG_ROCKS;
		default:
			return MAX_ADD_SPEED_WATER_FALL;
		}
	}
	public float GetMaxSlipStreamAddSpeed()
	{
		return MAX_SLIP_STREAM_ADD_SPEED;
	}
	public float GetRotSpeed()
	{
		return ROT_SPEED;
	}
	public float GetIntervalLerp(float _inputInterval)
	{
		float num = Mathf.Clamp(_inputInterval, 0f, MAX_INPUT_INTERVAL);
		return (MAX_INPUT_INTERVAL - num) / MAX_INPUT_INTERVAL;
	}
	public float ClampAcceleMoveSpeed(float _intervalLerp)
	{
		float maxAcceleMoveSpeed = GetMaxAcceleMoveSpeed();
		return Mathf.Clamp(maxAcceleMoveSpeed * _intervalLerp, 0f, maxAcceleMoveSpeed);
	}
	public float ClampFallBackMoveSpeed(float _intervalLerp)
	{
		float maxFallBackMoveSpeed = GetMaxFallBackMoveSpeed();
		return Mathf.Clamp(maxFallBackMoveSpeed * _intervalLerp, 0f, maxFallBackMoveSpeed);
	}
	public float GetMaxInputContinueTime()
	{
		return MAX_INPUT_CONTINUE_TIME;
	}
	public float GetInputContinueLerp(float _inputContinueTime)
	{
		return _inputContinueTime / MAX_INPUT_CONTINUE_TIME;
	}
	public float GetLimitRotX()
	{
		return LIMIT_ROT_X;
	}
	public float GetFallVelocityY()
	{
		return FALL_VELOCITY_Y;
	}
	public float GetStaminaUseAcceleMoveSpeed()
	{
		return STAMINA_USE_ACCELE_MOVE_SPEED;
	}
	public float GetStaminaUseSpeed()
	{
		return STAMINA_USE_SPEED;
	}
	public float GetStaminaHealSpeed()
	{
		return STAMINA_HEAL_SPEED;
	}
	public float GetUseUpStaminaHealSpeedMag()
	{
		return USE_UP_STAMINA_HEAL_SPEED_MAG;
	}
	public float GetUseUpStaminaHealWaitTime()
	{
		return USE_UP_STAMINA_HEAL_WAIT_TIME;
	}
	public float GetUseUpStaminaReUseValue()
	{
		return USE_UP_STAMINA_RE_USE_VALUE;
	}
	public float GetUseUpStaminaMoveSpeedMag()
	{
		return USE_UP_STAMINA_MOVE_SPEED_MAG;
	}
	public void GroupVibration()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (!arrayPlayer[i].GetIsCpu())
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)arrayPlayer[i].GetUserType());
			}
		}
	}
}
