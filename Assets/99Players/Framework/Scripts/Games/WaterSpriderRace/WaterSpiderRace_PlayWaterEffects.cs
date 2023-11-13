using System.Collections;
using UnityEngine;
public class WaterSpiderRace_PlayWaterEffects : MonoBehaviour
{
	[Header("水飛沫エフェクト")]
	public ParticleSystem splash;
	[Header("水の波紋エフェクト")]
	public ParticleSystem waterimpact;
	private void Start()
	{
	}
	public IEnumerator PlaySplashEffect()
	{
		yield return null;
		splash.Play();
	}
	public IEnumerator PlayWaterImpact()
	{
		yield return null;
		waterimpact.Play();
	}
}
