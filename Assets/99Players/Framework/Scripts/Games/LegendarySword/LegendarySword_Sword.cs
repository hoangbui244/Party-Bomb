using UnityEngine;
public class LegendarySword_Sword : MonoBehaviour
{
	[SerializeField]
	[Header("引き抜いた時のエフェクト")]
	private ParticleSystem swordShineEffect;
	public void PlayShineEffect()
	{
		if (!swordShineEffect.isPlaying)
		{
			swordShineEffect.Play();
		}
	}
	public void StopShineEffect()
	{
		if (swordShineEffect.isPlaying)
		{
			swordShineEffect.Stop();
		}
	}
}
