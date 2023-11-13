using UnityEngine;
public class Bowling_GutterEffect : ThrowResultEffect
{
	[SerializeField]
	[Header("文字リスト")]
	private SpriteRenderer[] textList;
	[SerializeField]
	[Header("ガタ\u30fcアニメ\u30fcション")]
	private Animator anim;
	[SerializeField]
	[Header("ヒットエフェクト１")]
	private ParticleSystem hitEffect;
	[SerializeField]
	[Header("ヒットエフェクト２")]
	private ParticleSystem hitEffect2;
	[SerializeField]
	[Header("サ\u30fcクルエフェクト")]
	private ParticleSystem circleEffect;
	private float[] defTextXPos;
	public override void Init()
	{
		base.Init();
		defTextXPos = new float[textList.Length];
		for (int i = 0; i < textList.Length; i++)
		{
			defTextXPos[i] = textList[i].transform.localPosition.x;
			textList[i].transform.SetLocalPosition(15f, 0f, 0f);
			textList[i].transform.localEulerAngles = Vector3.zero;
			textList[i].SetAlpha(1f);
			LeanTween.moveLocalX(textList[i].gameObject, defTextXPos[i], 0.4f).setDelay((float)i * 0.1f);
		}
		anim.SetTrigger("Play");
	}
	public void PlayHitEffect()
	{
		hitEffect.Play();
		LeanTween.moveLocalY(textList[2].gameObject, -0.7f, 0.02f);
	}
	public void PlayHitEffect2()
	{
		hitEffect2.Play();
		LeanTween.moveLocalX(textList[5].gameObject, 5.25f, 0.01f);
		LeanTween.moveLocalY(textList[5].gameObject, -0.5f, 0.01f);
		LeanTween.rotateZ(textList[5].gameObject, 320f, 0.01f);
	}
	public void PlayCircleEffect()
	{
		circleEffect.Play();
		for (int i = 0; i < textList.Length; i++)
		{
			LeanTween.color(textList[i].gameObject, new Color(1f, 1f, 1f, 0f), 0.3f).setDelay(1f);
		}
	}
	public void EndEffect()
	{
	}
	public void PlayVoiceSound()
	{
	}
	public void PlaySE1Sound()
	{
	}
}
