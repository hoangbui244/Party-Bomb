using UnityEngine;
public class MorphingRace_PlayerManager : SingletonCustom<MorphingRace_PlayerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc配列")]
	private MorphingRace_Player[] arrayPlayer;
	private readonly float BASE_MOVE_SPEED = 150f;
	private readonly float CORRECTION_UP_DIFF_MOVE_SPEED = 1.25f;
	private readonly float CORRECTION_DOWN_DIFF_MOVE_SPEED = 3f;
	private readonly float MAX_INPUT_INTERVAL = 0.5f;
	private readonly float MAX_INPUT_CONTINUE_TIME = 2f;
	private readonly float[] MAX_MOVE_SPEED = new float[5]
	{
		3.5f,
		2.75f,
		3f,
		2.5f,
		3.25f
	};
	private readonly float MIN_MOVE_SPEED = 0.5f;
	private readonly float HIT_OBSTACLE_INTERVAL = 1f;
	[SerializeField]
	[Header("変身エフェクトの各プレイヤ\u30fcの色")]
	private Color[] arrayMorphingEffectColor;
	private readonly float JUMP_BASE_POWER = 5f;
	public void Init()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayPlayer[i].Init(i, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (!arrayPlayer[i].GetIsGoal())
			{
				arrayPlayer[i].UpdateMethod();
			}
		}
	}
	public MorphingRace_Player GetPlayer(int _playerNo)
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
	public float GetMaxMoveSpeed(int _charaType)
	{
		return MAX_MOVE_SPEED[_charaType];
	}
	public float GetIntervalLerp(float _inputInterval)
	{
		float num = Mathf.Clamp(_inputInterval, 0f, MAX_INPUT_INTERVAL);
		return (MAX_INPUT_INTERVAL - num) / MAX_INPUT_INTERVAL;
	}
	public float ClampMoveSpeed(int _charaType, float _intervalLerp)
	{
		float maxMoveSpeed = GetMaxMoveSpeed(_charaType);
		return Mathf.Clamp(maxMoveSpeed * _intervalLerp, MIN_MOVE_SPEED, maxMoveSpeed);
	}
	public float GetInputContinueLerp(float _inputContinueTime)
	{
		return _inputContinueTime / MAX_INPUT_CONTINUE_TIME;
	}
	public float GetMaxInputContinueTime()
	{
		return MAX_INPUT_CONTINUE_TIME;
	}
	public float GetHitObstacleInterval()
	{
		return HIT_OBSTACLE_INTERVAL;
	}
	public void GameStartAnimation()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayPlayer[i].GameStartAnimation();
		}
	}
	public Color GetMorphingEffectColor(int _userType)
	{
		return arrayMorphingEffectColor[_userType];
	}
	public float GetJumpBasePower()
	{
		return JUMP_BASE_POWER;
	}
}
