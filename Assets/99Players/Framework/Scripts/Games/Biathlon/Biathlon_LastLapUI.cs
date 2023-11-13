using System;
using UnityEngine;
public class Biathlon_LastLapUI : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	[SerializeField]
	private AnimationCurve alphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	[SerializeField]
	private AnimationCurve movePosition = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	[SerializeField]
	private SpriteRenderer renderer;
	[SerializeField]
	private Sprite englishSprite;
	private float basePositionX;
	private bool IsShow
	{
		get;
		set;
	}
	public void Init()
	{
		renderer.enabled = false;
		renderer.SetAlpha(0f);
		basePositionX = base.transform.localPosition.x;
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			renderer.sprite = englishSprite;
		}
	}
	public void Show()
	{
		if (!IsShow)
		{
			IsShow = true;
			renderer.enabled = true;
			base.transform.SetLocalPositionX(basePositionX);
			LeanTween.value(base.gameObject, 0f, 1f, 0.9f).setOnUpdate(delegate(float value)
			{
				float num = scaleCurve.Evaluate(value);
				base.transform.localScale = new Vector3(num, num, 1f);
				float a = alphaCurve.Evaluate(value);
				renderer.color = new Color(1f, 1f, 1f, a);
				renderer.transform.AddLocalPositionX(movePosition.Evaluate(value) * 15f * Time.deltaTime);
			}).setOnComplete((Action)delegate
			{
				renderer.enabled = false;
				IsShow = false;
			});
		}
	}
}
