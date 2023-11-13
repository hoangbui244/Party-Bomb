using UnityEngine;
using UnityEngine.UI;
public class SnowBoard_AccelGauge : MonoBehaviour
{
	[SerializeField]
	[Header("SnowBoard_SkiBoard")]
	private SnowBoard_SkiBoard skiBoard;
	[SerializeField]
	[Header("Slider")]
	private Slider slider;
	private void Start()
	{
	}
	private void Update()
	{
		if (slider.gameObject.activeInHierarchy)
		{
			slider.value = skiBoard.AccelGauge;
		}
	}
}
