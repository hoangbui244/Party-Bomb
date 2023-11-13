using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_PlayerAnimation : DecoratedMonoBehaviour
{
	private static readonly int IdleHash = Animator.StringToHash("Idle");
	private static readonly int LandingHash = Animator.StringToHash("Landing");
	private static readonly int JumpHash = Animator.StringToHash("Jump");
	[SerializeField]
	[DisplayName("アニメ\u30fcション用キャラ")]
	private CharacterStyle animationStyle;
	[SerializeField]
	[DisplayName("アニメ\u30fcタ\u30fc")]
	private Animator animator;
	[SerializeField]
	[DisplayName("通常オブジェクト")]
	private GameObject normalGameObject;
	[SerializeField]
	[DisplayName("アニメ\u30fcション用オブジェクト")]
	private GameObject animationGameObject;
	[SerializeField]
	[DisplayName("風呂敷展開時の煙のエフェクト")]
	private ParticleSystem smokeParticle;
	[SerializeField]
	[DisplayName("着地アニメ待ち時間")]
	private float goalAnimationWaitDuration = 5f;
	[SerializeField]
	[DisplayName("着地時の土煙エフェクト")]
	private ParticleSystem landingSmokeParticle;
	private FlyingSquirrelRace_Player owner;
	public void Initialize(FlyingSquirrelRace_Player player)
	{
		owner = player;
		normalGameObject.SetActive(value: false);
		animationGameObject.SetActive(value: true);
		animationStyle.SetGameStyle(GS_Define.GameType.ARCHER_BATTLE, (int)owner.Controller);
		PlayIdle();
	}
	public void PlayStartAnimation()
	{
		PlayJump();
	}
	public void PlaySmokeEffect()
	{
		smokeParticle.Play();
	}
	public void ChangeCharacterObject()
	{
		normalGameObject.SetActive(value: true);
		animationGameObject.SetActive(value: false);
		SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.IsStartAnimationEnd = true;
	}
	public void PlayLandingSmokeEffect()
	{
		landingSmokeParticle.Play();
	}
	public IEnumerator PlayPreGoalAnimation()
	{
		FlyingSquirrelRace_Movement movement = owner.GetComponent<FlyingSquirrelRace_Movement>();
		while (!movement.MoveTargetHeight(0f))
		{
			yield return null;
		}
		while (!movement.RotateToBaseRotation())
		{
			yield return null;
		}
	}
	public IEnumerator PlayGoalAnimation()
	{
		if (goalAnimationWaitDuration > 0f)
		{
			yield return new WaitForSeconds(goalAnimationWaitDuration);
		}
		base.transform.localRotation = Quaternion.identity;
		normalGameObject.SetActive(value: false);
		animationGameObject.SetActive(value: true);
		PlayLanding();
	}
	public void PlayLanding()
	{
		animator.enabled = true;
		animator.Play(LandingHash);
	}
	public void PlayIdle()
	{
		animator.enabled = true;
		animator.Play(IdleHash);
	}
	public void PlayJump()
	{
		animator.enabled = true;
		animator.Play(JumpHash);
	}
	public void StopAnimation()
	{
		animator.enabled = false;
	}
}
