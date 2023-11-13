using UnityEngine;
public class HandSeal_FogManager : MonoBehaviour
{
	[SerializeField]
	[Header("Fogを有効にするか")]
	private bool isFog;
	[SerializeField]
	[Header("Fogカラ\u30fc")]
	private Color fogColor;
	[SerializeField]
	[Header("Fogモ\u30fcド")]
	private FogMode fogMode;
	[SerializeField]
	[Header("Fog強度係数(Exponentialモ\u30fcド限定)")]
	private float fogDensity;
	[SerializeField]
	[Header("Fog終点距離(Linearモ\u30fcド限定)")]
	private float fogEndDistance;
	[SerializeField]
	[Header("Fog始点距離(Linearモ\u30fcド限定)")]
	private float fogStartDistance;
	private bool isFogOrigin;
	private Color fogColorOrigin;
	private FogMode fogModeOrigin;
	private float fogDensityOrigin;
	private float fogEndDistanceOrigin;
	private float fogStartDistanceOrigin;
	private void Start()
	{
		if (isFog)
		{
			isFogOrigin = RenderSettings.fog;
			fogColorOrigin = RenderSettings.fogColor;
			fogModeOrigin = RenderSettings.fogMode;
			fogDensityOrigin = RenderSettings.fogDensity;
			fogEndDistanceOrigin = RenderSettings.fogEndDistance;
			fogStartDistanceOrigin = RenderSettings.fogStartDistance;
			RenderSettings.fog = isFog;
			RenderSettings.fogColor = fogColor;
			RenderSettings.fogMode = fogMode;
			RenderSettings.fogDensity = fogDensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
		}
	}
	private void OnDestroy()
	{
		if (isFog)
		{
			RenderSettings.fog = isFogOrigin;
			RenderSettings.fogColor = fogColorOrigin;
			RenderSettings.fogMode = fogModeOrigin;
			RenderSettings.fogDensity = fogDensityOrigin;
			RenderSettings.fogEndDistance = fogEndDistanceOrigin;
			RenderSettings.fogStartDistance = fogStartDistanceOrigin;
		}
	}
}
