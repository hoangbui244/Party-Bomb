using System;
using UnityEngine;
public class MikoshiRaceShakeUi : MonoBehaviour
{
	private const float PUSH_INTERVAL = 0.05f;
	[SerializeField]
	private Transform centerAnchor;
	[SerializeField]
	private Transform underAnchor;
	[SerializeField]
	private SpriteRenderer arrowSprite;
	[SerializeField]
	private SpriteRenderer pushButtonSprite;
	[SerializeField]
	private SpriteRenderer controlButtonSprite;
	[SerializeField]
	private SpriteRenderer controlTextSprite;
	[SerializeField]
	private SpriteRenderer centerTextSprite;
	[SerializeField]
	private ParticleSystem pushParticle;
	[SerializeField]
	private Transform gaugeAnchor;
	private bool isPush;
	private bool isArrowTween;
	private Vector3 arrowInitPos;
	public void Init()
	{
		arrowInitPos = arrowSprite.transform.localPosition;
		base.gameObject.SetActive(value: false);
	}
	public void Show()
	{
		base.gameObject.SetActive(value: true);
		centerAnchor.gameObject.SetActive(value: true);
		underAnchor.gameObject.SetActive(value: false);
		CenterSpriteDirection();
		LeanTween.delayedCall(1f, (Action)delegate
		{
			underAnchor.gameObject.SetActive(value: true);
			ArrowDirection();
		});
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	public void Push()
	{
		if (!isPush)
		{
			isPush = true;
			SingletonCustom<AudioManager>.Instance.SePlay("se_mikoshi_shake_button");
			pushParticle.Play();
			pushButtonSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.MikoshiRace, "push_button_1");
			pushButtonSprite.transform.SetLocalPositionY(-419.5f);
			LeanTween.delayedCall(0.05f, (Action)delegate
			{
				isPush = false;
				pushButtonSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.MikoshiRace, "push_button_0");
				pushButtonSprite.transform.SetLocalPositionY(-411f);
			});
		}
	}
	public void SetGauge(float _lerp)
	{
		gaugeAnchor.SetLocalScaleX(_lerp);
	}
	private void CenterSpriteDirection()
	{
		centerTextSprite.SetAlpha(0f);
		LeanTween.value(centerTextSprite.gameObject, 0f, 1f, 0.2f).setOnUpdate(delegate(float _value)
		{
			centerTextSprite.SetAlpha(_value);
		}).setOnComplete((Action)delegate
		{
			centerTextSprite.SetAlpha(1f);
		});
		LeanTween.scale(centerTextSprite.gameObject, Vector3.one * 1.3f, 0.3f).setLoopPingPong();
		LeanTween.delayedCall(1f, (Action)delegate
		{
			LeanTween.value(centerTextSprite.gameObject, 1f, 0f, 0.2f).setOnUpdate(delegate(float _value)
			{
				centerTextSprite.SetAlpha(_value);
			}).setOnComplete((Action)delegate
			{
				LeanTween.cancel(centerTextSprite.gameObject);
				centerAnchor.gameObject.SetActive(value: false);
			});
		});
	}
	private void ArrowDirection()
	{
		isArrowTween = true;
		arrowSprite.gameObject.SetActive(value: true);
		arrowSprite.transform.localPosition = arrowInitPos;
		ArrowTweenLoop();
		LeanTween.delayedCall(1f, (Action)delegate
		{
			LeanTween.value(arrowSprite.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float _value)
			{
				arrowSprite.SetAlpha(_value);
			}).setOnComplete((Action)delegate
			{
				isArrowTween = false;
				arrowSprite.gameObject.SetActive(value: false);
				arrowSprite.SetAlpha(1f);
			});
		});
	}
	private void ArrowTweenLoop()
	{
		if (isArrowTween)
		{
			LeanTween.value(arrowSprite.gameObject, 0f, 2f, 0.5f).setEaseInOutQuad().setOnUpdate(delegate(float _value)
			{
				if (_value > 1f)
				{
					_value = 2f - _value;
				}
				arrowSprite.transform.SetLocalPositionY(arrowInitPos.y - 30f * _value);
			})
				.setOnComplete((Action)delegate
				{
					ArrowTweenLoop();
				});
		}
	}
}
