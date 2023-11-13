using UnityEngine;
public class MorphingRace_OperationCharacter_AI : MonoBehaviour
{
	protected MorphingRace_Player player;
	protected int aiStrength;
	protected float moveLerpTime;
	protected bool isHitObstacle;
	protected Vector3 moveTargetPos;
	protected float moveInputInterval;
	protected float MOVE_INPUT_INTERVAL;
	protected float MOVE_DEF_INPUT_INTERVAL;
	protected float MOVE_INPUT_MIN_INTERVAL;
	protected float MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME;
	protected float moveSpeedMag;
	protected void Init(MorphingRace_Player _player)
	{
		player = _player;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		MOVE_INPUT_INTERVAL = GetMoveInputInterval();
		MOVE_DEF_INPUT_INTERVAL = MOVE_INPUT_INTERVAL;
		MOVE_INPUT_MIN_INTERVAL = GetMoveInputMinInterval();
		MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME = GetMoveInputMinIntervalUntilTime();
		moveSpeedMag = 1f;
	}
	public virtual void UpdateMethod()
	{
	}
	protected virtual void SetMove()
	{
	}
	protected void SetInput()
	{
		if (moveLerpTime < MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME)
		{
			moveLerpTime += Time.deltaTime;
			moveLerpTime = Mathf.Clamp(moveLerpTime, 0f, MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME);
			float num = moveLerpTime / MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME;
			MOVE_INPUT_INTERVAL = MOVE_DEF_INPUT_INTERVAL - (MOVE_DEF_INPUT_INTERVAL - MOVE_INPUT_MIN_INTERVAL) * num;
			MOVE_INPUT_INTERVAL = Mathf.Clamp(MOVE_INPUT_INTERVAL, MOVE_INPUT_MIN_INTERVAL, MOVE_DEF_INPUT_INTERVAL);
			UnityEngine.Debug.Log("CPU_MOVE_INPUT_INTERVAL : " + MOVE_INPUT_INTERVAL.ToString());
		}
		UnityEngine.Debug.Log("moveLerpTime " + moveLerpTime.ToString());
		moveInputInterval += Time.deltaTime;
		if (moveInputInterval > MOVE_INPUT_INTERVAL)
		{
			moveInputInterval = 0f;
			MoveInput();
		}
	}
	protected virtual void MoveInput()
	{
	}
	protected virtual void Move()
	{
	}
	public virtual void StopMove()
	{
		moveLerpTime = 0f;
	}
	public virtual void MorphingInit()
	{
		MOVE_INPUT_INTERVAL = GetMoveInputInterval();
		MOVE_DEF_INPUT_INTERVAL = MOVE_INPUT_INTERVAL;
		MOVE_INPUT_MIN_INTERVAL = GetMoveInputMinInterval();
		MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME = GetMoveInputMinIntervalUntilTime();
	}
	protected virtual float GetRandomObstacleDecelerateMagnification()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_OBSTACLE_DECELERATE_MAGNIFICATION[aiStrength] - 0.1f, MorphingRace_Define.CPU_OBSTACLE_DECELERATE_MAGNIFICATION[aiStrength] + 0.1f);
	}
	private float GetMoveInputInterval()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_MOVE_INPUT_INTERVAL[aiStrength] - 0.025f, MorphingRace_Define.CPU_MOVE_INPUT_INTERVAL[aiStrength] + 0.025f);
	}
	private float GetMoveInputMinInterval()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_MOVE_INPUT_MIN_INTERVAL[aiStrength] - 0.015f, MorphingRace_Define.CPU_MOVE_INPUT_MIN_INTERVAL[aiStrength] + 0.015f);
	}
	private float GetMoveInputMinIntervalUntilTime()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME[aiStrength] - 0.25f, MorphingRace_Define.CPU_MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME[aiStrength] + 0.25f);
	}
}
