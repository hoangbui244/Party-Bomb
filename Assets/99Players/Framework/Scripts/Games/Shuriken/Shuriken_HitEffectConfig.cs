using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_HitEffectConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("パ\u30fcティクルの最も近い位置")]
	private float nearestParticlePositionZ = 10.5f;
	[SerializeField]
	[DisplayName("パ\u30fcティクルの最も遠い位置")]
	private float farthestParticlePositionZ = 27f;
	[SerializeField]
	[DisplayName("ワ\u30fcルド座標のオフセット")]
	private float worldPositionOffset = -1200f;
	[SerializeField]
	[DisplayName("最小パ\u30fcティクルサイズ")]
	private float minParticleSize = 1f;
	[SerializeField]
	[DisplayName("最大パ\u30fcティクルサイズ")]
	private float maxParticleSize = 2.2f;
	public float NearestParticlePositionZ => nearestParticlePositionZ;
	public float FarthestParticlePositionZ => farthestParticlePositionZ;
	public float WorldPositionOffset => worldPositionOffset;
	public float MinParticleSize => minParticleSize;
	public float MaxParticleSize => maxParticleSize;
}
