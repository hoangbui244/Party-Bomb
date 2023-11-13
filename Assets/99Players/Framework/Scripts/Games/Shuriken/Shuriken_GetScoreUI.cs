using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_GetScoreUI : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_GetScoreUIConfig config;
	[SerializeField]
	[DisplayName("ル\u30fcト")]
	private Transform root;
	[SerializeField]
	[DisplayName("アニメ\u30fcションル\u30fcト")]
	private Transform animationRoot;
	[SerializeField]
	[Header("ポイント画像")]
	private SpriteRenderer spScore;
	[SerializeField]
	[Header("画像差分")]
	private Sprite[] arrayScoreDiff;
	private readonly int POINT_TYPE_NUM = 13;
	public bool IsUsing => base.gameObject.activeSelf;
	public void Initialize()
	{
		base.gameObject.SetActive(value: false);
	}
	public void Show(int point, int playerNo, Vector3 worldPosition)
	{
		SetUp(point, playerNo, worldPosition);
		this.CoffeeBreak().Keep(config.MoveTime, delegate(float t)
		{
			Shuriken_GetScoreUI shuriken_GetScoreUI = this;
			animationRoot.LocalPosition((Vector3 v) => v.Y(Mathf.Lerp(0f, shuriken_GetScoreUI.config.MoveY, t)));
		}).Delay(config.ShowTime - config.MoveTime)
			.Keep(config.FadeOutTime, delegate(float v)
			{
				SetAlpha(1f - v);
			})
			.DelayCall(0f, delegate
			{
				base.gameObject.SetActive(value: false);
			})
			.Start();
		LeanTween.cancel(spScore.gameObject);
		spScore.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(spScore.gameObject, Vector3.one * 0.5f, 0.35f).setEaseOutBack();
	}
	private void SetUp(int point, int playerNo, Vector3 worldPosition)
	{
		SetColor(point, playerNo);
		OffsetPlus(point);
		Vector3 localPosition = SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(worldPosition);
		root.localPosition = localPosition;
		base.gameObject.SetActive(value: true);
		SetAlpha(1f);
		animationRoot.LocalPosition((Vector3 v) => v.Y(0f));
	}
	private void OffsetPlus(int point)
	{
	}
	private void SetColor(int point, int playerNo)
	{
		int num = 0;
		switch (point)
		{
		case 50:
			num = 0;
			break;
		case 80:
			num = 1;
			break;
		case 100:
			num = 2;
			break;
		case 110:
			num = 3;
			break;
		case 120:
			num = 4;
			break;
		case 150:
			num = 5;
			break;
		case 200:
			num = 6;
			break;
		case 250:
			num = 7;
			break;
		case 300:
			num = 8;
			break;
		case 500:
			num = 9;
			break;
		case 700:
			num = 10;
			break;
		case 800:
			num = 11;
			break;
		case 1000:
			num = 12;
			break;
		}
		int num2 = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.CharacterIndexes[playerNo];
		spScore.sprite = arrayScoreDiff[num2 * POINT_TYPE_NUM + num];
	}
	private void SetAlpha(float alpha)
	{
		spScore.SetAlpha(alpha);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(spScore.gameObject);
	}
}
