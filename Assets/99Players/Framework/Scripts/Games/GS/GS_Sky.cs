using System;
using UnityEngine;
public class GS_Sky : MonoBehaviour
{
	public enum Type
	{
		Morning,
		Noon,
		Evening,
		Night
	}
	[SerializeField]
	[Header("空画像")]
	private SpriteRenderer[] arraySkySp;
	private Type currentType;
	private Type prevType;
	private bool isInit;
	private DateTime dt;
	private Type nowType;
	private void SetSky()
	{
		dt = SingletonCustom<GS_GameSelectManager>.Instance.GetDateTime();
		if (dt.Hour >= 5 && dt.Hour < 10)
		{
			nowType = Type.Morning;
		}
		else if (dt.Hour >= 10 && dt.Hour < 16)
		{
			nowType = Type.Noon;
		}
		else
		{
			nowType = Type.Evening;
		}
		if (currentType != nowType)
		{
			for (int i = 0; i < arraySkySp.Length; i++)
			{
				arraySkySp[i].SetAlpha((i == (int)currentType) ? 1f : 0f);
			}
			prevType = currentType;
			currentType = nowType;
			if (!isInit)
			{
				for (int j = 0; j < arraySkySp.Length; j++)
				{
					arraySkySp[j].SetAlpha((j == (int)currentType) ? 1f : 0f);
				}
				isInit = true;
			}
			else
			{
				LeanTween.cancel(arraySkySp[(int)prevType].gameObject);
				LeanTween.value(arraySkySp[(int)prevType].gameObject, 1f, 0f, 5f).setOnUpdate(delegate(float _value)
				{
					arraySkySp[(int)prevType].SetAlpha(_value);
				});
				LeanTween.cancel(arraySkySp[(int)currentType].gameObject);
				LeanTween.value(arraySkySp[(int)currentType].gameObject, 0f, 1f, 5f).setOnUpdate(delegate(float _value)
				{
					arraySkySp[(int)currentType].SetAlpha(_value);
				});
			}
		}
		else
		{
			isInit = true;
		}
	}
	private void OnEnable()
	{
		isInit = false;
		SetSky();
	}
	private void LateUpdate()
	{
		SetSky();
	}
	private void OnDisable()
	{
		for (int i = 0; i < arraySkySp.Length; i++)
		{
			LeanTween.cancel(arraySkySp[i].gameObject);
		}
	}
}
