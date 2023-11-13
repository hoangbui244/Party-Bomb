using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ThrowingShurikenConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("手裏剣を投げる間隔")]
	private float throwInterval = 1f;
	[SerializeField]
	[DisplayName("手裏剣を投げる力")]
	private float throwPower = 10f;
	[SerializeField]
	[DisplayName("手裏剣の軌跡の色")]
	private Color[] trailColors = new Color[8];
	[SerializeField]
	[DisplayName("手裏剣のマテリアル")]
	private Material[] shurikenMaterials = new Material[8];
	[SerializeField]
	[DisplayName("手裏剣を投げた時の効果音")]
	[Note("配列の中からランダムで再生される", DrawPosition.After)]
	private string[] throwSfxNames = new string[2]
	{
		"se_shuriken_throw_0",
		"se_shuriken_throw_2"
	};
	[SerializeField]
	[DisplayName("手裏剣を当てた時の効果音")]
	[Note("一番上の効果音が再生される", DrawPosition.After)]
	private string[] hitSfxNames = new string[2]
	{
		"se_shuriken_hit_0",
		"se_shuriken_hit_1"
	};
	public float ThrowInterval => throwInterval;
	public float ThrowPower => throwPower;
	public Color[] TrailColors => trailColors;
	public Material[] ShurikenMaterials => shurikenMaterials;
	public string[] ThrowSfxNames => throwSfxNames;
	public string[] HitSfxNames => hitSfxNames;
}
