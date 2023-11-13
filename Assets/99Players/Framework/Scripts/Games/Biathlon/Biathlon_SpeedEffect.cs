using System;
using UnityEngine;
public class Biathlon_SpeedEffect : MonoBehaviour
{
	private static readonly int ColorProperty = Shader.PropertyToID("_Color");
	private static readonly float TextureOffsetScrollSpeed = 0.5f;
	[SerializeField]
	private Material lineMaterial;
	[SerializeField]
	private MeshRenderer renderer;
	public bool IsShow
	{
		get;
		private set;
	}
	public void Init()
	{
		lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, 0f));
		base.gameObject.SetActive(value: false);
		IsShow = false;
	}
	public void Show(float duration = 0.5f)
	{
		LeanTween.cancel(base.gameObject, callOnComplete: false);
		lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, 0f));
		base.gameObject.SetActive(value: true);
		IsShow = true;
		LeanTween.value(base.gameObject, 0f, 1f, duration).setOnUpdate(delegate(float alpha)
		{
			lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, alpha));
		}).setOnComplete((Action)delegate
		{
			lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, 1f));
		});
	}
	public void Hide(float duration = 1f)
	{
		LeanTween.cancel(base.gameObject, callOnComplete: false);
		lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, 1f));
		IsShow = false;
		LeanTween.value(base.gameObject, 1f, 0f, duration).setOnUpdate(delegate(float alpha)
		{
			lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, alpha));
		}).setOnComplete((Action)delegate
		{
			lineMaterial.SetColor(ColorProperty, new Color(1f, 1f, 1f, 0f));
			base.gameObject.SetActive(value: false);
		});
	}
	public void UpdateMethod()
	{
		SpeedLineUpdate();
	}
	private void SpeedLineUpdate()
	{
		if (IsShow)
		{
			Vector2 mainTextureOffset = lineMaterial.mainTextureOffset;
			mainTextureOffset.y += Time.deltaTime * TextureOffsetScrollSpeed;
			mainTextureOffset.y %= 1f;
			lineMaterial.mainTextureOffset = mainTextureOffset;
		}
	}
}
