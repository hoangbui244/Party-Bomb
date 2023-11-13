using System;
using UnityEngine;
public class DragonBattleDispPoint : MonoBehaviour
{
	[SerializeField]
	[Header("スコア画像")]
	private SpriteRenderer spScore;
	[SerializeField]
	[Header("スコア画像差分")]
	private Sprite[] arraySpScoreDiff;
	[SerializeField]
	[Header("カラ\u30fc設定")]
	private Color[] arrayColor;
	[SerializeField]
	[Header("カラ\u30fc設定対象")]
	private SpriteRenderer[] arrayColorTarget;
	public void Init(int _colorIdx)
	{
		for (int i = 0; i < arrayColorTarget.Length; i++)
		{
			arrayColorTarget[i].color = arrayColor[_colorIdx];
		}
		spScore.sprite = arraySpScoreDiff[_colorIdx];
		base.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.35f).setEaseInOutBack();
		LeanTween.moveLocalY(base.gameObject, base.transform.localPosition.y + 10f, 1f).setDelay(0.5f);
		LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float _value)
		{
			spScore.SetAlpha(_value);
		}).setDelay(0.5f)
			.setOnComplete((Action)delegate
			{
				if ((bool)this)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			});
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
