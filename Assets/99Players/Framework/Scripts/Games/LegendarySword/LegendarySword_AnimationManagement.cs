using UnityEngine;
public class LegendarySword_AnimationManagement : MonoBehaviour
{
	private LegendarySword_Player player;
	private Animator animator;
	public void Init(LegendarySword_Player _player)
	{
		player = _player;
		animator = GetComponent<Animator>();
		animator.enabled = _player.isPlay;
	}
	public void SetStandbyAnimation()
	{
		animator.SetTrigger("ToStandby");
	}
	public void SetPulloutAnimationValue(float _value)
	{
		animator.SetFloat("PulloutValue", _value);
	}
	public void SetRaiseAnimation()
	{
		animator.SetTrigger("ToRaise");
	}
	public void SetResetAnimation()
	{
		animator.SetFloat("PulloutValue", 0f);
		animator.SetTrigger("ToReset");
	}
}
