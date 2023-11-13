using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_IcePlateConfig : ScriptableObject
{
	[SerializeField]
	[DisplayName("穴のないメッシュ")]
	private Mesh icePlateMesh;
	[SerializeField]
	[DisplayName("穴の開いたメッシュ")]
	private Mesh holedIcePlateMesh;
	[SerializeField]
	[DisplayName("穴の開いたメッシュのマテリアル")]
	private Material holedIcePlateMaterial;
	[SerializeField]
	[DisplayName("釣れる量の回復速度")]
	private float recoveryAmount = 0.025f;
	[SerializeField]
	[DisplayName("使用中の回復速度の補正値")]
	private float usingCorrections = 0.1f;
	[SerializeField]
	[DisplayName("釣り上げた魚ごとの減少量")]
	private float subtractAmount = 0.05f;
	public Mesh IcePlateMesh => icePlateMesh;
	public Mesh HoledIcePlateMesh => holedIcePlateMesh;
	public Material HoledIcePlateMaterial => holedIcePlateMaterial;
	public float RecoveryAmount => recoveryAmount;
	public float UsingCorrections => usingCorrections;
	public float SubtractAmount => subtractAmount;
}
