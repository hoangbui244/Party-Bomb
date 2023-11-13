using System.Collections;
using UnityEngine;
public class Bowling_StrikeEffect : ThrowResultEffect
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
	[Header("出現インタ\u30fcバル時間")]
	private float APPEAR_INTERVAL = 0.2f;
	[SerializeField]
	[Header("ボ\u30fcル出現時間")]
	private float BALL_APPEAR_TIME = 0.2f;
	[SerializeField]
	[Header("ボ\u30fcル移動時間")]
	private float BALL_MOVE_TIME = 2f;
	[SerializeField]
	[Header("回転時間")]
	private float ROT_TIME = 1f;
	[SerializeField]
	[Header("回転インタ\u30fcバル時間")]
	private float ROT_INTERVAL = 0.2f;
	[SerializeField]
	[Header("表示時間")]
	private float SHOW_TIME = 1f;
	[SerializeField]
	[Header("消える時間")]
	private float HIDE_TIME = 1f;
	[SerializeField]
	[Header("連続ストライク表示")]
	private SpriteRenderer continuousStrike;
	[SerializeField]
	[Header("連続ストライク画像")]
	private Sprite[] continuousStrikeSprite;
	[SerializeField]
	[Header("背景エフェクト１")]
	private ParticleSystem backEffect;
	[SerializeField]
	[Header("背景エフェクト２")]
	private ParticleSystem backEffect2;
	[SerializeField]
	[Header("背景エフェクト３")]
	private ParticleSystem backEffect3;
	[SerializeField]
	[Header("ライトエフェクト１")]
	private ParticleSystem lightEffect;
	[SerializeField]
	[Header("スタ\u30fcエフェクト１")]
	private ParticleSystem starEffect;
	[SerializeField]
	[Header("スタ\u30fcエフェクト２")]
	private ParticleSystem starEffect2;
	[SerializeField]
	[Header("流れるエフェクト１")]
	private ParticleSystem flowEffect;
	[SerializeField]
	[Header("流れるエフェクト２")]
	private ParticleSystem flowEffect2;
	[SerializeField]
	[Header("文字エフェクト")]
	private ParticleSystem[] textEffectList;
	private const float DEF_BALL_POSX = -10f;
	private const float DEF_TEXT_YPOS = 3f;
	private const float CONTINUOUS_STRIKE_TEXT_YPOS = 0.97f;
	private bool isContinuousStrike;
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < textList.Length; i++)
		{
			textList[i].SetAlpha(0f);
			textList[i].transform.SetLocalPositionY(3f);
		}
		continuousStrike.SetAlpha(0f);
		ball.SetAlpha(0f);
		if (Bowling_Define.GUIM.GetScoreData(Bowling_Define.MPM.NowThrowUserType).GetContinuousStrikeNum() == Bowling_Define.PLAY_FRAME_NUM + 1)
		{
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_0");
			isContinuousStrike = false;
		}
		else
		{
			int continuousStrikeNum = Bowling_Define.GUIM.GetScoreData(Bowling_Define.MPM.NowThrowUserType).GetContinuousStrikeNum();
			continuousStrike.sprite = continuousStrikeSprite[continuousStrikeNum];
			switch (continuousStrikeNum)
			{
			case 0:
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_0");
				break;
			case 1:
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_1");
				break;
			case 2:
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_2");
				break;
			case 3:
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_3");
				break;
			case 4:
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_strike_4");
				break;
			}
			if (continuousStrikeNum >= 1)
			{
				isContinuousStrike = true;
			}
			else
			{
				isContinuousStrike = false;
			}
			StartCoroutine(_ContinuousStrikeAnimation());
		}
		StartCoroutine(_TextAnimation());
	}
	private IEnumerator _TextAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		for (int j = 0; j < textList.Length; j++)
		{
			LeanTween.cancel(textList[j].gameObject);
			LeanTween.moveLocalY(textList[j].gameObject, isContinuousStrike ? 0.97f : 0f, APPEAR_TIME).setDelay(APPEAR_INTERVAL * (float)j);
		}
		for (int k = 0; k < textList.Length; k++)
		{
			LeanTween.color(textList[k].gameObject, new Color(1f, 1f, 1f, 1f), APPEAR_TIME).setDelay(APPEAR_INTERVAL * (float)k);
		}
		yield return new WaitForSeconds(APPEAR_TIME * 0.8f);
		for (int i = 0; i < textEffectList.Length; i++)
		{
			if (isContinuousStrike)
			{
				textEffectList[i].transform.SetLocalPositionY(0.97f);
			}
			else
			{
				textEffectList[i].transform.SetLocalPositionY(0f);
			}
			textEffectList[i].Play();
			yield return new WaitForSeconds(APPEAR_INTERVAL);
		}
		ball.transform.SetLocalPositionX(-10f);
		if (isContinuousStrike)
		{
			ball.transform.SetLocalPositionY(0.97f);
		}
		else
		{
			ball.transform.SetLocalPositionY(0f);
		}
		LeanTween.moveLocalX(ball.gameObject, ball.transform.localPosition.x * -1f, BALL_MOVE_TIME);
		LeanTween.color(ball.gameObject, new Color(1f, 1f, 1f, 1f), BALL_APPEAR_TIME);
		LeanTween.color(ball.gameObject, new Color(1f, 1f, 1f, 0f), BALL_APPEAR_TIME).setDelay(BALL_MOVE_TIME - BALL_APPEAR_TIME);
		for (int l = 0; l < textList.Length; l++)
		{
			LeanTween.cancel(textList[l].gameObject);
			LeanTween.rotateY(textList[l].gameObject, 1080f, ROT_TIME).setDelay(ROT_INTERVAL * (float)l);
		}
		backEffect.Play();
		backEffect2.Play();
		backEffect3.Play();
		lightEffect.Play();
		flowEffect.Play();
		flowEffect2.Play();
		yield return new WaitForSeconds(ROT_TIME + SHOW_TIME);
		for (int m = 0; m < textList.Length; m++)
		{
			LeanTween.color(textList[m].gameObject, new Color(1f, 1f, 1f, 0f), HIDE_TIME);
		}
		starEffect.Play();
		starEffect2.Play();
	}
	private IEnumerator _ContinuousStrikeAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		yield return new WaitForSeconds(APPEAR_TIME * 0.8f);
		for (int i = 0; i < textEffectList.Length; i++)
		{
			yield return new WaitForSeconds(APPEAR_INTERVAL);
		}
		continuousStrike.transform.SetLocalEulerAnglesX(0f);
		LeanTween.color(continuousStrike.gameObject, ColorPalet.white, BALL_MOVE_TIME * 0.5f);
		LeanTween.rotateAround(continuousStrike.gameObject, Vector3.right, 1800f, BALL_MOVE_TIME).setEaseOutCubic();
		yield return new WaitForSeconds(ROT_TIME + SHOW_TIME);
		LeanTween.color(continuousStrike.gameObject, new Color(1f, 1f, 1f, 0f), HIDE_TIME);
	}
}
