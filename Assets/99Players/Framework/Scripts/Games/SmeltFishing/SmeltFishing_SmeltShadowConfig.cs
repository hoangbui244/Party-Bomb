using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_SmeltShadowConfig : ScriptableObject
{
	[SerializeField]
	[DisplayName("移動半径")]
	private float moveRadius = 2f;
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed = 2f;
	[SerializeField]
	[DisplayName("表示時間")]
	private float minLivingTime = 3f;
	[SerializeField]
	[DisplayName("表示時間")]
	private float maxLivingTime = 8f;
	[SerializeField]
	[DisplayName("アルファカ\u30fcブ")]
	private AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 1f), new Keyframe(0.85f, 1f), new Keyframe(1f, 0f));
	public float MoveRadius => moveRadius;
	public float MoveSpeed => moveSpeed;
	public float MinLivingTime => minLivingTime;
	public float MaxLivingTime => maxLivingTime;
	public AnimationCurve AlphaCurve => alphaCurve;
}
