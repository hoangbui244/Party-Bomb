using UnityEngine;
public class LegendarySword_SwordField : MonoBehaviour
{
	[SerializeField]
	[Header("剣を抜くエフェクト")]
	private ParticleSystem swordEffect;
	[SerializeField]
	[Header("剣を抜く煙のエフェクト")]
	private ParticleSystem swordSmorkeEffect;
	[SerializeField]
	[Header("半分くらい抜いた時のエフェクト")]
	private ParticleSystem halfPullOutSparkleEffect;
	[SerializeField]
	[Header("抜く直前のエフェクト")]
	private ParticleSystem pulloutSparkleEffect;
	[SerializeField]
	[Header("抜く時のエフェクト")]
	private ParticleSystem pulloutSparkleRainbowEffect;
	public void SwordUp()
	{
		swordEffect.Play();
		swordSmorkeEffect.Play();
	}
	public void PlayHalfPullOutSparkleEffect()
	{
		halfPullOutSparkleEffect.Play();
	}
	public void StopHalfPullOutSparkleEffect()
	{
		halfPullOutSparkleEffect.Stop();
	}
	public void PlayPullOutSparkleEffect()
	{
		pulloutSparkleEffect.Play();
	}
	public void PlayPullOutSparkleRainbowEffect()
	{
		halfPullOutSparkleEffect.Stop();
		pulloutSparkleEffect.Stop();
		pulloutSparkleRainbowEffect.Play();
	}
}
