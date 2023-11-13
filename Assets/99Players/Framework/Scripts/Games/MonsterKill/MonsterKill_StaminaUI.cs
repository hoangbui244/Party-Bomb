using UnityEngine;
public class MonsterKill_StaminaUI : MonoBehaviour
{
	private int playerNo;
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
	[SerializeField]
	[Header("補正座標")]
	private Vector3 diffPos;
	public void Init(int _playerNo)
	{
		playerNo = _playerNo;
		colorChengeTime = 0f;
		originGaugeMaxX = gauge.size.x;
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			base.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
		}
	}
	public void UpdateMethod()
	{
		Vector3 position = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(playerNo).transform.position;
		Vector3 position2 = SingletonCustom<MonsterKill_CameraManager>.Instance.GetCamera(playerNo).WorldToScreenPoint(position);
		position2 = SingletonCustom<MonsterKill_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(position2);
		base.transform.SetPosition(position2.x + diffPos.x, position2.y + diffPos.y, base.transform.position.z);
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
