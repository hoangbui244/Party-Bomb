using UnityEngine;
public class Golf_AnimationEventor : MonoBehaviour
{
	private Golf_Player player;
	private Animator animator;
	public void Init(Golf_Player _player)
	{
		player = _player;
		animator = GetComponent<Animator>();
	}
	public void PlayIdleAnimation()
	{
		animator.SetTrigger("ToIdle");
	}
	public void PlaySwingIdleAnimation()
	{
		animator.SetTrigger("ToSwingIdle");
	}
	public void PlaySwingAnimation()
	{
		animator.SetTrigger("ToSwing");
	}
	private void PlaySwingSE()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_golf_swing");
	}
	private void Shot()
	{
		player.Shot();
	}
}
