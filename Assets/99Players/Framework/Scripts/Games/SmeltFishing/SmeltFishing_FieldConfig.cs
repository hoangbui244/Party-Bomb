using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_FieldConfig : ScriptableObject
{
	[Header("設定値")]
	[SerializeField]
	[DisplayName("氷に開ける穴の最小数")]
	private int minHoleCount = 4;
	[SerializeField]
	[DisplayName("氷に開ける穴の最小数")]
	private int maxHoleCount = 12;
	[SerializeField]
	[DisplayName("氷が隣合うのを許可")]
	private bool allowNeighboring;
	public int MinHoleCount => minHoleCount;
	public int MaxHoleCount => maxHoleCount;
	public bool AllowNeighboring => allowNeighboring;
	public int GetRandomHoleCount()
	{
		return UnityEngine.Random.Range(minHoleCount, maxHoleCount + 1);
	}
}
