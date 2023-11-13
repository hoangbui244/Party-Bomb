using UnityEngine;
public class Canoe_StaminaGaugeUI : MonoBehaviour
{
	[SerializeField]
	[Header("スタミナのゲ\u30fcジ")]
	private SpriteRenderer gauge;
	private float originGaugeMaxX;
	[SerializeField]
	[Header("通常の色")]
	private Color normalColor;
	[SerializeField]
	[Header("スタミナが切れた時の色")]
	private Color noStaminaColor;
	private float colorChengeTime;
	public void Init()
	{
		colorChengeTime = 0f;
		originGaugeMaxX = gauge.size.x;
	}
	public void SetStaminaGauge(float _staminaLerp, bool _isUseUpStamina)
	{
		Vector2 size = gauge.size;
		size.x = originGaugeMaxX * _staminaLerp;
		gauge.size = size;
		if (_isUseUpStamina)
		{
			colorChengeTime = 1f;
			gauge.color = Color.Lerp(normalColor, noStaminaColor, colorChengeTime);
		}
		else
		{
			colorChengeTime = Mathf.Clamp(colorChengeTime - Time.deltaTime / 0.3f, 0f, 1f);
			gauge.color = Color.Lerp(normalColor, noStaminaColor, colorChengeTime);
		}
	}
}
