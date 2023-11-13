using System;
using UnityEngine;
public class MonsterKill_Enemy_DeadEffect : MonoBehaviour
{
	[Serializable]
	private struct ColorType
	{
		[Header("startColor")]
		public Color startColor;
	}
	private ParticleSystem effect;
	[SerializeField]
	[Header("カラ\u30fc種類ごとのエフェクトの色")]
	private ColorType[] arrayColorType;
	public void SetEffectColor(int _colorIdx)
	{
		effect = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = effect.main;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = effect.colorOverLifetime;
		main.startColor = arrayColorType[_colorIdx].startColor;
	}
	public void DebugSetEffectColor(int _colorIdx)
	{
		if (_colorIdx < arrayColorType.Length)
		{
			ParticleSystem component = GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = component.colorOverLifetime;
			main.startColor = arrayColorType[_colorIdx].startColor;
			ParticleSystem particleSystem = base.transform.parent.GetComponent<ParticleSystem>();
			if (particleSystem == null)
			{
				particleSystem = component;
			}
			particleSystem.Play();
		}
	}
	public void PlayDeadEffect()
	{
		effect.Play();
	}
}
