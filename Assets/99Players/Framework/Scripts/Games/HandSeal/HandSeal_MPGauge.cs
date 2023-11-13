using UnityEngine;
using UnityEngine.UI;
public class HandSeal_MPGauge : MonoBehaviour
{
	[SerializeField]
	[Header("SnowBoard_SkiBoard")]
	private HandSeal_Hand hand;
	[SerializeField]
	[Header("Slider(現在値)")]
	private Slider slider;
	[SerializeField]
	[Header("Slider(現在値)のimage")]
	private Image sliderImage;
	[SerializeField]
	[Header("Slider(追尾)")]
	private Slider slider_delay;
	[SerializeField]
	[Header("ゲ\u30fcジMAXで強調表示するアイコン_1")]
	private SpriteRenderer spIcon_1;
	[SerializeField]
	[Header("ゲ\u30fcジMAXで強調表示するアイコン_2")]
	private SpriteRenderer spIcon_2;
	[SerializeField]
	[Header("ゲ\u30fcジMAXで表示するアイコン1_フェ\u30fcドエフェクト")]
	private SpriteRenderer spIcon_1_Fade;
	[SerializeField]
	[Header("ゲ\u30fcジMAXで表示するアイコン2_フェ\u30fcドエフェクト")]
	private SpriteRenderer spIcon_2_Fade;
	private Color originColor;
	private Color color;
	private int id_color;
	private bool isBlink;
	private Transform spIcon_1_obj;
	private Transform spIcon_2_obj;
	private Transform spIcon_1_Fade_obj;
	private Transform spIcon_2_Fade_obj;
	private void Start()
	{
		slider.value = 0f;
		slider_delay.value = 0f;
		originColor = sliderImage.color;
		color = originColor;
		if (spIcon_1 != null)
		{
			spIcon_1.SetAlpha(0f);
			spIcon_1_obj = spIcon_1.gameObject.transform;
		}
		if (spIcon_2 != null)
		{
			spIcon_2.SetAlpha(0f);
			spIcon_2_obj = spIcon_2.gameObject.transform;
		}
		if (spIcon_1_Fade != null)
		{
			spIcon_1_Fade.SetAlpha(0f);
			spIcon_1_Fade_obj = spIcon_1_Fade.gameObject.transform;
		}
		if (spIcon_2_Fade != null)
		{
			spIcon_2_Fade.SetAlpha(0f);
			spIcon_2_Fade_obj = spIcon_2_Fade.gameObject.transform;
		}
	}
	private void Update()
	{
		if (slider.gameObject.activeInHierarchy)
		{
			slider.value = hand.secretGauge;
			if (slider.value == 1f && !isBlink)
			{
				isBlink = true;
				id_color = LeanTween.value(base.gameObject, originColor.g, originColor.g - 0.3f, 0.5f).setOnUpdate(delegate(float val)
				{
					color.g = val;
					sliderImage.color = color;
				}).setLoopPingPong()
					.id;
					RepeatScaleIcon(_set: true);
					FadeEffectIcon();
					if (HandSeal_Define.GM.IsDuringGame() && hand.player.UserType <= HandSeal_Define.UserType.PLAYER_4)
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_handseal_gaugemax", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
					}
				}
				else if (slider.value < 1f && isBlink)
				{
					isBlink = false;
					LeanTween.cancel(id_color);
					sliderImage.color = originColor;
					RepeatScaleIcon(_set: false);
				}
			}
			if (slider_delay.gameObject.activeInHierarchy)
			{
				if (slider.value > slider_delay.value)
				{
					slider_delay.value = slider.value;
				}
				else if (slider.value < slider_delay.value)
				{
					slider_delay.value = Mathf.Lerp(slider_delay.value, slider.value, Time.deltaTime);
				}
			}
		}
		private void EmphasizeIcon(bool _set)
		{
			if (_set)
			{
				if (spIcon_1 != null)
				{
					LeanTween.value(base.gameObject, 1f, 0.3f, 0.5f).setOnUpdate(delegate(float val)
					{
						spIcon_1.SetAlpha(val);
					}).setLoopPingPong();
				}
				if (spIcon_2 != null)
				{
					LeanTween.value(base.gameObject, 1f, 0.3f, 0.5f).setOnUpdate(delegate(float val)
					{
						spIcon_2.SetAlpha(val);
					}).setLoopPingPong();
				}
			}
			else
			{
				if (spIcon_1 != null)
				{
					spIcon_1.SetAlpha(0f);
				}
				if (spIcon_2 != null)
				{
					spIcon_2.SetAlpha(0f);
				}
			}
		}
		private void RepeatScaleIcon(bool _set)
		{
			if (_set)
			{
				if (spIcon_1 != null)
				{
					spIcon_1.SetAlpha(1f);
					LeanTween.value(base.gameObject, 1f, 0.9f, 0.5f).setOnUpdate(delegate(float val)
					{
						spIcon_1_obj.SetLocalScale(val, val, 1f);
					}).setLoopPingPong();
				}
				if (spIcon_2 != null)
				{
					spIcon_2.SetAlpha(1f);
					LeanTween.value(base.gameObject, 1f, 0.9f, 0.5f).setOnUpdate(delegate(float val)
					{
						spIcon_2_obj.SetLocalScale(val, val, 1f);
					}).setLoopPingPong();
				}
			}
			else
			{
				if (spIcon_1 != null)
				{
					spIcon_1.SetAlpha(0f);
					spIcon_1_obj.SetLocalScale(1f, 1f, 1f);
				}
				if (spIcon_2 != null)
				{
					spIcon_2.SetAlpha(0f);
					spIcon_2_obj.SetLocalScale(1f, 1f, 1f);
				}
			}
		}
		private void FadeEffectIcon()
		{
			if (spIcon_1_Fade != null)
			{
				spIcon_1_Fade_obj.SetLocalScale(1f, 1f, 1f);
				spIcon_1_Fade.SetAlpha(1f);
				spIcon_1_Fade_obj.position = spIcon_1_obj.position;
				LeanTween.value(base.gameObject, 1f, 1.5f, 0.5f).setOnUpdate(delegate(float val)
				{
					spIcon_1_Fade_obj.SetLocalScale(val, val, 1f);
				});
				LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float val)
				{
					spIcon_1_Fade.SetAlpha(val);
				});
			}
			if (spIcon_2_Fade != null)
			{
				spIcon_2_Fade_obj.SetLocalScale(1f, 1f, 1f);
				spIcon_2_Fade.SetAlpha(1f);
				spIcon_2_Fade_obj.position = spIcon_2_obj.position;
				LeanTween.value(base.gameObject, 1f, 1.5f, 0.5f).setOnUpdate(delegate(float val)
				{
					spIcon_2_Fade_obj.SetLocalScale(val, val, 1f);
				});
				LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float val)
				{
					spIcon_2_Fade.SetAlpha(val);
				});
			}
		}
	}
