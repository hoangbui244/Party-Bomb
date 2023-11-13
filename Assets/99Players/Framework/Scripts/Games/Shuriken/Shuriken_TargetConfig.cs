using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_TargetConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("表示状態の向き")]
	private float showEulerAnglesX;
	[SerializeField]
	[DisplayName("表示の所要時間")]
	private float showDuration = 0.5f;
	[SerializeField]
	[DisplayName("非表示状態の向き")]
	private float hideEulerAnglesX;
	[SerializeField]
	[DisplayName("非表示の所要時間")]
	private float hideDuration = 0.5f;
	[SerializeField]
	[DisplayName("表示時間")]
	private float showTime = 20f;
	[SerializeField]
	[DisplayName("表示時間のばらけ")]
	private float showTimeRange = 5f;
	[SerializeField]
	[DisplayName("最短出現遅延時間")]
	private float minShowDelayTime = 0.5f;
	[SerializeField]
	[DisplayName("最長出現遅延時間")]
	private float maxShowDelayTime = 0.5f;
	[SerializeField]
	[DisplayName("スコア")]
	private int score;
	public float ShowEulerAnglesX => showEulerAnglesX;
	public float ShowDuration => showDuration;
	public float HideEulerAnglesX => hideEulerAnglesX;
	public float HideDuration => hideDuration;
	public float ShowTime => showTime;
	public float ShowTimeRange => showTimeRange;
	public float MinShowDelayTime => minShowDelayTime;
	public float MaxShowDelayTime => maxShowDelayTime;
	public int Score => score;
}
