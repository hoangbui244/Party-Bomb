using System;
using UnityEngine;
using UnityEngine.Extension;
[Serializable]
public class SmeltFishing_SpotRevise
{
	[SerializeField]
	[DisplayName("最小値")]
	private float min;
	[SerializeField]
	[DisplayName("最大値")]
	private float max;
	[SerializeField]
	[DisplayName("補正カ\u30fcブ")]
	private AnimationCurve curve;
	public SmeltFishing_SpotRevise()
	{
		min = -1f;
		max = 1f;
		curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	}
	public SmeltFishing_SpotRevise(float min, float max)
	{
		this.min = min;
		this.max = max;
		curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	}
	public SmeltFishing_SpotRevise(float min, float max, AnimationCurve curve)
	{
		this.min = min;
		this.max = max;
		this.curve = curve;
	}
	public float Evaluate(float distance)
	{
		return curve.Evaluate(distance);
	}
	public int EvaluateToInt(float distance)
	{
		return Mathf.RoundToInt(Evaluate(distance));
	}
}
