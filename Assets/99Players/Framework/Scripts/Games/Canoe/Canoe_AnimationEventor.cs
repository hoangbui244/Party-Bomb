using UnityEngine;
public class Canoe_AnimationEventor : MonoBehaviour
{
	private enum AnimationState
	{
		Idle,
		Rowing,
		Rowing_Back,
		Goal
	}
	private Canoe_Player player;
	private Animator animator;
	private AnimationState animationState;
	public void Init(Canoe_Player _player)
	{
		player = _player;
		animator = GetComponent<Animator>();
	}
	public void SetStartAnimation(float _startTime)
	{
		animationState = AnimationState.Rowing;
		animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, _startTime);
	}
	public void PlayIdleAnimation()
	{
		if (animationState != 0)
		{
			animationState = AnimationState.Idle;
			animator.SetTrigger("ToIdle");
		}
	}
	public void PlayRowingAnimation()
	{
		if (animationState != AnimationState.Rowing)
		{
			animationState = AnimationState.Rowing;
			animator.SetTrigger("ToRowing");
		}
	}
	public void PlayRowingBackAnimation()
	{
		if (animationState != AnimationState.Rowing_Back)
		{
			animationState = AnimationState.Rowing_Back;
			animator.SetTrigger("ToRowing_Back");
		}
	}
	public void SetRowingAnimationSpeed(float _rowingSpeed)
	{
		animator.SetFloat("RowingSpeed", _rowingSpeed);
	}
	public void PlayGoalAnimation()
	{
		if (animationState != AnimationState.Goal)
		{
			animationState = AnimationState.Goal;
			animator.SetTrigger("ToGoal");
			animator.speed = 0.5f;
		}
	}
	private void Rowing(int _paddleIdx)
	{
		player.PlayPaddleRowingEffect(_paddleIdx);
	}
}
