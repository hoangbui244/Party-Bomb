using UnityEngine;
public class SmeltFishing_SmeltShadowsConfig : ScriptableObject
{
	[SerializeField]
	private int minShadows = 2;
	[SerializeField]
	private int maxShadows = 9;
	[SerializeField]
	private float minDistance;
	[SerializeField]
	private float maxDistance = 6f;
	public int MinShadows => minShadows;
	public int MaxShadows => maxShadows;
	public float MinDistance => minDistance;
	public float MaxDistance => maxDistance;
}
