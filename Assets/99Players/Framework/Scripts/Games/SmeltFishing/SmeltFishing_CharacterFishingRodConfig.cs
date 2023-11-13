using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CharacterFishingRodConfig : ScriptableObject
{
	[Header("釣り糸をたれる際の設定値")]
	[SerializeField]
	[DisplayName("仕掛けの初期位置")]
	private Vector3 originalLinePosition;
	[SerializeField]
	[DisplayName("仕掛けを下す位置")]
	private Vector3 castLinePosition;
	[SerializeField]
	[DisplayName("糸を垂らす")]
	private float castDuration = 1f;
	[SerializeField]
	[DisplayName("巻き上げる時間")]
	private float rollUpDuration = 1f;
	public Vector3 OriginalLinePosition => originalLinePosition;
	public Vector3 CastLinePosition => castLinePosition;
	public float CastDuration => castDuration;
	public float RollUpDuration => rollUpDuration;
}
