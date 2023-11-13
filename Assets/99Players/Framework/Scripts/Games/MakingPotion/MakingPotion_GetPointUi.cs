using System;
using UnityEngine;
public class MakingPotion_GetPointUi : MonoBehaviour
{
	private const float VIEW_TIME = 0.5f;
	private const float MOVE_UP_VALUE = 30f;
	[SerializeField]
	private Transform anchor;
	[SerializeField]
	private GameObject perfectObj;
	[SerializeField]
	private GameObject goodObj;
	[SerializeField]
	private GameObject badObj;
	[SerializeField]
	private GameObject tooFastObj;
	[SerializeField]
	private GameObject fastObj;
	[SerializeField]
	private GameObject slowObj;
	[SerializeField]
	private GameObject tooSlowObj;
	[SerializeField]
	private SpriteNumbers pointSpriteNumbers;
	[SerializeField]
	private SpriteRenderer plusSprite;
	[SerializeField]
	private SpriteNumbers pointColorSpriteNumbers;
	[SerializeField]
	private SpriteRenderer plusColorSprite;
	public bool IsShow => base.gameObject.activeSelf;
	public void Init()
	{
		base.gameObject.SetActive(value: false);
	}
	public void PerfectShow(int _charaNo, Vector3 _worldPos)
	{
		perfectObj.SetActive(value: true);
		goodObj.SetActive(value: false);
		badObj.SetActive(value: false);
		tooFastObj.SetActive(value: false);
		fastObj.SetActive(value: false);
		slowObj.SetActive(value: false);
		tooSlowObj.SetActive(value: false);
		pointSpriteNumbers.gameObject.SetActive(value: false);
		pointColorSpriteNumbers.gameObject.SetActive(value: false);
		MoveDirection(_charaNo, _worldPos);
	}
	public void GoodShow(int _charaNo, Vector3 _worldPos)
	{
		perfectObj.SetActive(value: false);
		goodObj.SetActive(value: true);
		badObj.SetActive(value: false);
		pointSpriteNumbers.gameObject.SetActive(value: false);
		pointColorSpriteNumbers.gameObject.SetActive(value: false);
		MoveDirection(_charaNo, _worldPos);
	}
	public void BadShow(int _charaNo, Vector3 _worldPos)
	{
		perfectObj.SetActive(value: false);
		goodObj.SetActive(value: false);
		badObj.SetActive(value: true);
		pointSpriteNumbers.gameObject.SetActive(value: false);
		pointColorSpriteNumbers.gameObject.SetActive(value: false);
		MoveDirection(_charaNo, _worldPos);
	}
	public void PaceShow(int _charaNo, MakingPotion_PlayerScript.SpinSpeedType _speedType, Vector3 _worldPos)
	{
		perfectObj.SetActive(value: false);
		tooFastObj.SetActive(value: false);
		fastObj.SetActive(value: false);
		slowObj.SetActive(value: false);
		tooSlowObj.SetActive(value: false);
		pointSpriteNumbers.gameObject.SetActive(value: false);
		pointColorSpriteNumbers.gameObject.SetActive(value: false);
		switch (_speedType)
		{
		case MakingPotion_PlayerScript.SpinSpeedType.TooSlow:
			tooSlowObj.SetActive(value: true);
			break;
		case MakingPotion_PlayerScript.SpinSpeedType.Slow:
			slowObj.SetActive(value: true);
			break;
		case MakingPotion_PlayerScript.SpinSpeedType.Normal:
			perfectObj.SetActive(value: true);
			break;
		case MakingPotion_PlayerScript.SpinSpeedType.Fast:
			fastObj.SetActive(value: true);
			break;
		case MakingPotion_PlayerScript.SpinSpeedType.TooFast:
			tooFastObj.SetActive(value: true);
			break;
		}
		MoveDirection(_charaNo, _worldPos);
	}
	public void Show(int _charaNo, int _point, Vector3 _worldPos)
	{
		perfectObj.SetActive(value: false);
		goodObj.SetActive(value: false);
		badObj.SetActive(value: false);
		tooFastObj.SetActive(value: false);
		fastObj.SetActive(value: false);
		slowObj.SetActive(value: false);
		tooSlowObj.SetActive(value: false);
		pointSpriteNumbers.gameObject.SetActive(value: true);
		pointColorSpriteNumbers.gameObject.SetActive(value: true);
		pointSpriteNumbers.Set(_point);
		pointColorSpriteNumbers.Set(_point);
		int num = 0;
		if (_point == 0)
		{
			num = 1;
		}
		else
		{
			while (_point > 0)
			{
				_point /= 10;
				num++;
			}
		}
		plusSprite.transform.SetLocalPositionX(pointSpriteNumbers.NumSpace * (float)num * -1f);
		plusColorSprite.transform.SetLocalPositionX(pointColorSpriteNumbers.NumSpace * (float)num * -1f);
		MoveDirection(_charaNo, _worldPos);
	}
	private void MoveDirection(int _charaNo, Vector3 _worldPos)
	{
		_worldPos = SingletonCustom<MakingPotion_GameManager>.Instance.GetCamera(_charaNo).WorldToScreenPoint(_worldPos);
		_worldPos = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(_worldPos);
		base.transform.position = _worldPos;
		base.transform.SetLocalPositionZ(0f);
		base.gameObject.SetActive(value: true);
		SetAlpha(1f);
		LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
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
		LeanTween.moveLocalY(anchor.gameObject, 30f, 0.5f).setEaseOutQuad();
	}
	public void SetColor(Color _color)
	{
		pointColorSpriteNumbers.SetColor(_color);
		plusColorSprite.color = _color;
	}
	public void SetAlpha(float _alpha)
	{
		pointSpriteNumbers.SetAlpha(_alpha);
		pointColorSpriteNumbers.SetAlpha(_alpha);
		plusSprite.SetAlpha(_alpha);
		plusColorSprite.SetAlpha(_alpha);
	}
	public void SetScale(float _scale)
	{
		base.transform.SetLocalScaleX(_scale);
		base.transform.SetLocalScaleY(_scale);
	}
}
