using System;
using UnityEngine;
public class Skijump_TimeLimit : MonoBehaviour
{
	[SerializeField]
	[Header("カウント数値")]
	private SpriteRenderer spTime;
	[SerializeField]
	[Header("カウント数値素材")]
	private Sprite[] arrayTimeSprite;
	private float DEFAULT_POS_X = -757f;
	private float OUT_POS_X = -1200f;
	public void Show()
	{
		base.gameObject.SetActive(value: true);
		spTime.sprite = arrayTimeSprite[10];
		base.gameObject.transform.SetLocalPositionX(OUT_POS_X);
		LeanTween.cancel(base.gameObject);
		LeanTween.moveLocalX(base.gameObject, DEFAULT_POS_X, 0.5f).setEaseOutQuart();
	}
	public void SetTimeLimit(float _time)
	{
		if (_time >= 10f)
		{
			spTime.sprite = arrayTimeSprite[0];
		}
		else
		{
			spTime.sprite = arrayTimeSprite[(int)(10f - _time) + 1];
		}
	}
	public void Close()
	{
		UnityEngine.Debug.Log("Close.");
		LeanTween.cancel(base.gameObject);
		LeanTween.moveLocalX(base.gameObject, OUT_POS_X, 0.25f).setOnComplete((Action)delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
