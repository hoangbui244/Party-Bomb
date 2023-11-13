using System;
using UnityEngine;
public class ShortTrack_ConcentratedLine : MonoBehaviour
{
	private const float SPEED_LINE_SCROLL_SPEED = 0.5f;
	[SerializeField]
	public GameObject concentratedLine;
	[SerializeField]
	private Material concentratedLineMaterial;
	private bool isViewSpeedLines;
	public void UpdateMethod()
	{
		SpeedLineUpdate();
	}
	private void Start()
	{
		concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
		concentratedLine.SetActive(value: false);
		isViewSpeedLines = false;
	}
	public void SpeedLineStart()
	{
		LeanTween.cancel(concentratedLine, callOnComplete: false);
		concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
		concentratedLine.SetActive(value: true);
		isViewSpeedLines = true;
		LeanTween.value(concentratedLine, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
		{
			concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, _value));
		}).setOnComplete((Action)delegate
		{
			concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		});
	}
	public void SpeedLineEnd()
	{
		LeanTween.cancel(concentratedLine, callOnComplete: false);
		concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		concentratedLine.SetActive(value: false);
		LeanTween.value(concentratedLine, 1f, 0f, 1f).setOnUpdate(delegate(float _value)
		{
			concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, _value));
		}).setOnComplete((Action)delegate
		{
			concentratedLineMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
			isViewSpeedLines = false;
		});
	}
	private void SpeedLineUpdate()
	{
		if (isViewSpeedLines)
		{
			Vector2 mainTextureOffset = concentratedLineMaterial.mainTextureOffset;
			mainTextureOffset.y = (mainTextureOffset.y + Time.deltaTime * 0.5f) % 1f;
			concentratedLineMaterial.mainTextureOffset = mainTextureOffset;
		}
	}
}
