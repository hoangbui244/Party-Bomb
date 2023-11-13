using System.Collections;
using UnityEngine;
public class Bowling_SplitEffect : ThrowResultEffect
{
	[SerializeField]
	[Header("文字リスト")]
	private SpriteRenderer[] textList;
	[SerializeField]
	[Header("ボ\u30fcル")]
	private SpriteRenderer ball;
	[SerializeField]
	[Header("出現時間")]
	private float APPEAR_TIME = 1f;
	[SerializeField]
	[Header("移動遅延時間")]
	private float MOVE_DELAY = 0.5f;
	[SerializeField]
	[Header("移動時間")]
	private float MOVE_TIME = 1f;
	[SerializeField]
	[Header("ボ\u30fcルフェ\u30fcド時間")]
	private float BALL_FADE_TIME = 0.2f;
	[SerializeField]
	[Header("ボ\u30fcル移動時間")]
	private float BALL_MOVE_TIME = 1f;
	[SerializeField]
	[Header("ジャンプ時間")]
	private float JUMP_TIME = 1f;
	[SerializeField]
	[Header("消える時間")]
	private float HIDE_TIME = 1f;
	[SerializeField]
	[Header("サ\u30fcクルエフェクト")]
	private ParticleSystem circleEffect;
	[SerializeField]
	[Header("ヒットエフェクト")]
	private ParticleSystem hitEffect;
	[SerializeField]
	[Header("右に星が流れるエフェクト")]
	private ParticleSystem starFlowRightEffect;
	[SerializeField]
	[Header("左に星が流れるエフェクト")]
	private ParticleSystem starFlowLeftEffect;
	private float[] textDefLocalPosX;
	private float defBallPosY;
	private void Start()
	{
		defBallPosY = ball.transform.localPosition.y;
	}
	public override void Init()
	{
		base.Init();
		if (base.enabled)
		{
			for (int i = 0; i < textList.Length; i++)
			{
				textList[i].SetAlpha(0f);
			}
			ball.SetAlpha(0f);
			textDefLocalPosX = new float[textList.Length];
			for (int j = 0; j < textList.Length; j++)
			{
				textDefLocalPosX[j] = textList[j].transform.localPosition.x;
				textList[j].transform.SetLocalPositionX(0f);
			}
			StartCoroutine(_TextAnimation());
		}
	}
	private IEnumerator _TextAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		for (int i = 0; i < textList.Length; i++)
		{
			LeanTween.color(textList[i].gameObject, ColorPalet.white, APPEAR_TIME);
		}
		yield return new WaitForSeconds(APPEAR_TIME + MOVE_DELAY);
		for (int j = 0; j < textList.Length; j++)
		{
			LeanTween.moveLocalX(textList[j].gameObject, textDefLocalPosX[j], MOVE_TIME);
		}
		starFlowRightEffect.Play();
		starFlowLeftEffect.Play();
		yield return new WaitForSeconds(MOVE_TIME);
		ball.transform.SetLocalPositionY(defBallPosY);
		LeanTween.color(ball.gameObject, ColorPalet.white, BALL_FADE_TIME);
		LeanTween.moveLocalY(ball.gameObject, ball.transform.localPosition.y * -1f, BALL_MOVE_TIME);
		LeanTween.color(ball.gameObject, new Color(1f, 1f, 1f, 0f), BALL_FADE_TIME).setDelay(BALL_MOVE_TIME - BALL_FADE_TIME * 2f);
		yield return new WaitForSeconds(BALL_FADE_TIME);
		hitEffect.Play();
		LeanTween.moveLocalY(textList[2].gameObject, 3f, JUMP_TIME * 0.5f);
		LeanTween.moveLocalY(textList[2].gameObject, 0f, JUMP_TIME * 0.5f).setDelay(JUMP_TIME * 0.5f);
		LeanTween.rotateZ(textList[2].gameObject, 720f, JUMP_TIME);
		yield return new WaitForSeconds(JUMP_TIME);
		circleEffect.Play();
		for (int k = 0; k < textList.Length; k++)
		{
			LeanTween.color(textList[k].gameObject, new Color(1f, 1f, 1f, 0f), HIDE_TIME);
		}
	}
}
