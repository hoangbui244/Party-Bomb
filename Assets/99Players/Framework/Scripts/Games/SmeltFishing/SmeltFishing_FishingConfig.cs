using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_FishingConfig : ScriptableObject
{
	[Header("食いつくまでの時間設定")]
	[SerializeField]
	[DisplayName("最短食いつき時間")]
	private float minBiteTime = 5f;
	[SerializeField]
	[DisplayName("最長食いつき時間")]
	private float maxBiteTime = 10f;
	[Header("食いつく数の設定")]
	[SerializeField]
	[DisplayName("最小食いつき数")]
	private int minBiteSmeltCount = 1;
	[SerializeField]
	[DisplayName("最大食いつき数")]
	private int maxBiteSmeltCount = 3;
	[SerializeField]
	[DisplayName("補正後の最大数")]
	private int maxLimitBiteSmeltCount = 5;
	[Header("ボ\u30fcナスで食いつく数の設定")]
	[SerializeField]
	[DisplayName("最小ボ\u30fcナス食いつき数")]
	private int minBonusBiteSmeltCount;
	[SerializeField]
	[DisplayName("最大ボ\u30fcナス食いつき数")]
	private int maxBonusBiteSmeltCount = 2;
	[SerializeField]
	[DisplayName("ボ\u30fcナスタイムリミット(割合)")]
	private float bonusBiteSmeltTimeLimitRate = 0.5f;
	[Header("逃げられるまでの時間設定")]
	[SerializeField]
	[DisplayName("逃げるまでの最短時間")]
	private float minSmeltEscapeTime = 1f;
	[SerializeField]
	[DisplayName("逃げるまでの最長時間")]
	private float maxSmeltEscapeTime = 3f;
	[SerializeField]
	[DisplayName("１匹あたりの減少時間")]
	private float escapeTimeDecreasePerSmelt = 0.1f;
	[SerializeField]
	[DisplayName("補正後の最短時間")]
	private float minLimitSmeltEscapeTime = 0.5f;
	[Header("ワカサギがかかるまでのミニゲ\u30fcム設定値")]
	[SerializeField]
	[DisplayName("仕掛けの初期位置")]
	private float defaultBaitDepth = 5f;
	[SerializeField]
	[DisplayName("仕掛けの移動する深さの最小値")]
	private float minBaitDepth = 1f;
	[SerializeField]
	[DisplayName("仕掛けの移動する深さの最大値")]
	private float maxBaitDepth = 10f;
	[SerializeField]
	[DisplayName("仕掛けが落ちていくスピ\u30fcド")]
	private float baitFallSpeed = 1f;
	[SerializeField]
	[DisplayName("仕掛けを引いた時の移動量")]
	private float baitPullPower = 2.5f;
	[SerializeField]
	[DisplayName("仕掛けの位置ボ\u30fcナス(小)")]
	private float halfBonusDepthRange = 2.5f;
	[SerializeField]
	[DisplayName("仕掛けの位置ボ\u30fcナス(大)")]
	private float bonusDepthRange = 1.5f;
	public float MinBiteTime => minBiteTime;
	public float MaxBiteTime => maxBiteTime;
	public int MinBiteSmeltCount => minBiteSmeltCount;
	public int MaxBiteSmeltCount => maxBiteSmeltCount;
	public int MaxLimitBiteSmeltCount => maxLimitBiteSmeltCount;
	public int MinBonusBiteSmeltCount => minBonusBiteSmeltCount;
	public int MaxBonusBiteSmeltCount => maxBonusBiteSmeltCount;
	public float BonusBiteSmeltTimeLimitRate => bonusBiteSmeltTimeLimitRate;
	public float MinSmeltEscapeTime => minSmeltEscapeTime;
	public float MaxSmeltEscapeTime => maxSmeltEscapeTime;
	public float EscapeTimeDecreasePerSmelt => escapeTimeDecreasePerSmelt;
	public float MinLimitSmeltEscapeTime => minLimitSmeltEscapeTime;
	public float DefaultBaitDepth => defaultBaitDepth;
	public float MinBaitDepth => minBaitDepth;
	public float MaxBaitDepth => maxBaitDepth;
	public float BaitFallSpeed => baitFallSpeed;
	public float BaitPullPower => baitPullPower;
	public float HalfBonusDepthRange => halfBonusDepthRange;
	public float BonusDepthRange => bonusDepthRange;
	public float MinBaitProperDepth => minBaitDepth + halfBonusDepthRange;
	public float MaxBaitProperDepth => maxBaitDepth - halfBonusDepthRange;
	public float GetRandomBiteTime()
	{
		return Mathf.Lerp(MinBiteTime, MaxBiteTime, UnityEngine.Random.value);
	}
	public int GetRandomBiteSmeltCount(float value)
	{
		int num = MinBiteSmeltCount;
		int num2 = MaxBiteSmeltCount;
		float num3 = 1f - value;
		return Mathf.RoundToInt(Mathf.Lerp(num, num2, Mathf.Clamp01(UnityEngine.Random.value - num3)));
	}
	public int GetRandomBonusBiteSmeltCount(int smeltCount)
	{
		int a = Mathf.RoundToInt(Mathf.Lerp(MinBonusBiteSmeltCount, MaxBonusBiteSmeltCount, UnityEngine.Random.value));
		int b = MaxLimitBiteSmeltCount - smeltCount;
		return Mathf.Min(a, b);
	}
	public float GetRandomSmeltEscapeTime(float biteSmeltCount)
	{
		float num = Mathf.Lerp(MinSmeltEscapeTime, MaxSmeltEscapeTime, UnityEngine.Random.value);
		num -= biteSmeltCount * EscapeTimeDecreasePerSmelt;
		return Mathf.Max(MinLimitSmeltEscapeTime, num);
	}
}
