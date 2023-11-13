using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_HitEffectCache : SingletonMonoBehaviour<Shuriken_HitEffectCache>
{
	[SerializeField]
	[DisplayName("エフェクト")]
	private Shuriken_HitEffect[] effects;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_HitEffectConfig config;
	public void Initialize()
	{
		Shuriken_HitEffect[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Initialize();
		}
	}
	public void Play(Vector3 point)
	{
		float t = Mathf.InverseLerp(config.NearestParticlePositionZ, config.FarthestParticlePositionZ, point.z - config.WorldPositionOffset);
		float size = Mathf.Lerp(config.MinParticleSize, config.MaxParticleSize, t);
		int num = 0;
		while (true)
		{
			if (num < effects.Length)
			{
				if (!effects[num].Isplaying)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		effects[num].SetSize(size);
		effects[num].Play(point);
	}
}
