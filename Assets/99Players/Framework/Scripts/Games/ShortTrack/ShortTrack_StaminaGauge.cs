using UnityEngine;
public class ShortTrack_StaminaGauge : MonoBehaviour
{
	[SerializeField]
	[Header("スタミナのゲ\u30fcジ")]
	private SpriteRenderer gauge;
	private Vector2 SpriteSizeMax;
	private float gauge_x;
	[SerializeField]
	[Header("通常の色")]
	private Color normalColor;
	[SerializeField]
	[Header("最高速度")]
	private Color maxSpeedColor;
	[SerializeField]
	[Header("スタミナ切れた時の色")]
	private Color noStaminaColor;
	private bool colorChange;
	private float colorChengeTime;
	public void Init()
	{
		colorChengeTime = 0f;
		gauge_x = gauge.size.x;
		SpriteSizeMax = gauge.size;
	}
	public void StaminaGauge(ShortTrack_Character _character)
	{
		gauge_x = SpriteSizeMax.x * StaminaPercent(_character);
		gauge.size = new Vector2(Mathf.Clamp(gauge_x, 0f, SpriteSizeMax.x), gauge.size.y);
		if (StaminaPercent(_character) <= 0.005f)
		{
			colorChange = true;
		}
		else if (StaminaPercent(_character) >= 0.3f)
		{
			colorChange = false;
		}
		if (colorChange)
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
	public float StaminaPercent(ShortTrack_Character _character)
	{
		return _character.StaminaPoint / _character.StaminaMaxPoint;
	}
}
