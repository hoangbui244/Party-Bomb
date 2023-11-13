using UnityEngine;
public class MorphingRace_CharacterManager : SingletonCustom<MorphingRace_CharacterManager>
{
	private readonly float[] MAX_MOVE_ANIMATION_SPEED = new float[5]
	{
		1.5f,
		1f,
		1f,
		1f,
		1f
	};
	private readonly float[] MIN_MOVE_ANIMATION_SPEED = new float[5]
	{
		1f,
		0f,
		0f,
		0f,
		0f
	};
	private readonly float MORPHING_FISH_JUMP_ANIMATION_TIME = 0.5f;
	private readonly float DESCENT_LANDING_MAX_HEIGHT = 5f;
	private readonly float DESCENT_LANDING_MAX_TIME = 1f;
	private readonly float GOAL_ANIMATION_TIME = 0.5f;
	public float GetMoveAnimationSpeed(int _charaType, float _intervalLerp)
	{
		float num = MAX_MOVE_ANIMATION_SPEED[_charaType];
		return Mathf.Clamp(num * _intervalLerp, 0f, num);
	}
	public bool CheckWalkMoveAnimationSpeed(int _charaType, float _animSpeed)
	{
		return _animSpeed < MAX_MOVE_ANIMATION_SPEED[_charaType] * 0.1f;
	}
	public bool CheckRunMoveAnimationSpeed(int _charaType, float _animSpeed)
	{
		return _animSpeed < MAX_MOVE_ANIMATION_SPEED[_charaType] * 0.6f;
	}
	public float GetMorphingFishJumpAnimationTime()
	{
		return MORPHING_FISH_JUMP_ANIMATION_TIME;
	}
	public float GetDescentLandingMaxHeight()
	{
		return DESCENT_LANDING_MAX_HEIGHT;
	}
	public float GetDescentLandingMaxTime()
	{
		return DESCENT_LANDING_MAX_TIME;
	}
	public float GetGoalAnimationTime()
	{
		return GOAL_ANIMATION_TIME;
	}
}
