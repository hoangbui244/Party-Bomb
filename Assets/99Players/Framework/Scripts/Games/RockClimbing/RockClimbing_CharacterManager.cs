using UnityEngine;
public class RockClimbing_CharacterManager : SingletonCustom<RockClimbing_CharacterManager>
{
	[SerializeField]
	[Header("上に登る用のアンカ\u30fc")]
	private Transform[] arrayClimbOnAnchor;
	private readonly float MAX_ANIMATION_SPEED = 0.25f;
	private readonly float THROW_GRAPPLING_HOOK_ANIMATION_TIME = 0.25f;
	private readonly float READY_CLIMBING_ANIMATION_TIME = 0.5f;
	private readonly float CLIMB_ON_ANIMATION_TIME = 0.5f;
	private readonly float GOAL_AFTER_ANIMATION_TIME = 0.5f;
	[SerializeField]
	[Header("敵の忍者キャラのマテリアル")]
	private Material[] arrayCastleNinjaMaterial;
	public Transform[] GetArrayClimbOnAnchor()
	{
		return arrayClimbOnAnchor;
	}
	public float ClampAnimationSpeed(float _intervalLerp)
	{
		float value = MAX_ANIMATION_SPEED * (1f - _intervalLerp);
		UnityEngine.Debug.Log("animSpped " + value.ToString());
		return Mathf.Clamp(value, 0f, MAX_ANIMATION_SPEED);
	}
	public float GetThrowGrapplingHookAnimationTime()
	{
		return THROW_GRAPPLING_HOOK_ANIMATION_TIME;
	}
	public float GetReadyClimbingAnimationTime()
	{
		return READY_CLIMBING_ANIMATION_TIME;
	}
	public float GetClimbOnAnimationTime()
	{
		return CLIMB_ON_ANIMATION_TIME;
	}
	public float GetGoalAfterAnimationTime()
	{
		return GOAL_AFTER_ANIMATION_TIME;
	}
	public Material GetCastleNinjaMaterial()
	{
		return arrayCastleNinjaMaterial[Random.Range(0, arrayCastleNinjaMaterial.Length)];
	}
}
