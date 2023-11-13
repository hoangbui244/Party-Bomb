using System;
using UnityEngine;
public class FindMask_GetPointUi : MonoBehaviour
{
	private const float VIEW_TIME = 0.6f;
	private const float MOVE_UP_VALUE = 30f;
	private const float FADE_TIME = 0.5f;
	[SerializeField]
	[Header("獲得ポイント表示UIアンカ\u30fc")]
	private Transform anchor;
	[SerializeField]
	private SpriteNumbers pointSpriteNumbers;
	[SerializeField]
	[Header("プラス画像")]
	private SpriteRenderer plusSprite;
	public bool IsShow => base.gameObject.activeSelf;
	public void Init()
	{
		base.gameObject.SetActive(value: false);
	}
	public void Show(int _point, Vector3 _worldPos)
	{
		pointSpriteNumbers.Set(_point);
		int num = 0;
		while (_point > 0)
		{
			_point /= 10;
			num++;
		}
		plusSprite.transform.SetLocalPositionX(pointSpriteNumbers.NumSpace * (float)num * -1f);
		_worldPos = SingletonCustom<FindMask_GameManager>.Instance.GetCamera().WorldToScreenPoint(_worldPos);
		_worldPos = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(_worldPos);
		base.transform.position = _worldPos;
		base.transform.SetLocalPositionZ(0f);
		base.gameObject.SetActive(value: true);
		SetAlpha(1f);
		LeanTween.value(base.gameObject, 0f, 1f, 0.6f).setOnUpdate(delegate(float _value)
		{
			base.transform.SetLocalPositionZ(_value);
		}).setOnComplete((Action)delegate
		{
			LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float _value)
			{
				SetAlpha(_value);
			}).setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
		});
		anchor.SetLocalPositionY(0f);
		LeanTween.moveLocalY(anchor.gameObject, 30f, 0.6f).setEaseOutQuad();
	}
	public void SetPointColor(Color _color)
	{
		pointSpriteNumbers.SetColor(_color);
		plusSprite.color = _color;
	}
	public void SetAlpha(float _alpha)
	{
		pointSpriteNumbers.SetAlpha(_alpha);
		plusSprite.SetAlpha(_alpha);
	}
	public void SetScale(float _scale)
	{
		base.transform.SetLocalScaleX(_scale);
		base.transform.SetLocalScaleY(_scale);
	}
}
