using System.Collections;
using UnityEngine;
public class Bowling_SpareEffect : ThrowResultEffect
{
	[SerializeField]
	[Header("文字リスト")]
	private SpriteRenderer[] textList;
	[SerializeField]
	[Header("出現時間")]
	private float APPEAR_TIME = 1f;
	[SerializeField]
	[Header("出現インタ\u30fcバル時間")]
	private float APPEAR_INTERVAL = 0.2f;
	[SerializeField]
	[Header("回転時間")]
	private float ROT_TIME = 1f;
	[SerializeField]
	[Header("消える時間")]
	private float HIDE_TIME = 1f;
	[SerializeField]
	[Header("星エフェクト１")]
	private ParticleSystem starEffect;
	[SerializeField]
	[Header("星エフェクト２")]
	private ParticleSystem starEffect2;
	[SerializeField]
	[Header("背景エフェクト１")]
	private ParticleSystem backEffect;
	[SerializeField]
	[Header("背景エフェクト１")]
	private ParticleSystem backEffect2;
	[SerializeField]
	[Header("ライトエフェクト１")]
	private ParticleSystem lightEffect;
	[SerializeField]
	[Header("ライトエフェクト２")]
	private ParticleSystem lightEffect2;
	[SerializeField]
	[Header("文字エフェクト")]
	private ParticleSystem[] textEffectList;
	private const float DEF_TEXT_YPOS = 3f;
	public override void Init()
	{
		base.Init();
		if (base.enabled)
		{
			for (int i = 0; i < textList.Length; i++)
			{
				textList[i].SetAlpha(0f);
				textList[i].transform.SetLocalPositionY(3f);
			}
			StartCoroutine(_TextAnimation());
		}
	}
	private IEnumerator _TextAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		backEffect.Play();
		backEffect2.Play();
		yield return new WaitForSeconds(0.2f);
		for (int j = 0; j < textList.Length; j++)
		{
			LeanTween.color(textList[j].gameObject, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), APPEAR_TIME * 0.5f).setDelay(APPEAR_INTERVAL * (float)j);
		}
		for (int k = 0; k < textList.Length; k++)
		{
			LeanTween.moveLocalY(textList[k].gameObject, 0f, APPEAR_TIME).setEaseOutCubic().setDelay(APPEAR_INTERVAL * (float)k);
			LeanTween.rotateY(textList[k].gameObject, 1800f, ROT_TIME).setEaseOutCubic().setDelay(APPEAR_INTERVAL * (float)k);
		}
		yield return new WaitForSeconds(APPEAR_TIME * 0.2f);
		for (int i = 0; i < textEffectList.Length; i++)
		{
			textEffectList[i].Play();
			yield return new WaitForSeconds(APPEAR_INTERVAL);
		}
		yield return new WaitForSeconds(0.2f);
		lightEffect.Play();
		lightEffect2.Play();
		yield return new WaitForSeconds(lightEffect.main.startLifetime.constant * 0.8f);
		for (int l = 0; l < textList.Length; l++)
		{
			LeanTween.color(textList[l].gameObject, new Color(1f, 1f, 1f, 0f), HIDE_TIME);
		}
		starEffect.Play();
		starEffect2.Play();
	}
}
