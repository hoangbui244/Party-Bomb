using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.L10;
public class FlyingSquirrelRace_ResultPlacement : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("順位アイコン")]
	private SpriteRenderer placementIcon;
	[SerializeField]
	[DisplayName("スコアボ\u30fcナスのル\u30fcト")]
	private GameObject scoreBonusRoot;
	[SerializeField]
	[DisplayName("スコアボ\u30fcナスのテキスト")]
	private SpriteRenderer scoreBonusText;
	[SerializeField]
	[DisplayName("スコアボ\u30fcナスのスコア")]
	private SpriteNumbers scoreBonus;
	private float originScoreBonusRootScaleY;
	[SerializeField]
	[DisplayName("使用スプライト名")]
	private string useSpriteNameBase = "_common_rank_";
	[SerializeField]
	private FlyingSquirrelRace_PlayerConfig config;
	public void Initialize()
	{
		placementIcon.enabled = false;
		placementIcon.color = Color.white.A(0f);
		scoreBonusRoot.SetActive(value: false);
		originScoreBonusRootScaleY = scoreBonusRoot.transform.localScale.y;
		scoreBonusRoot.transform.SetLocalScaleY(0f);
	}
	public void Show(int placement)
	{
		placementIcon.enabled = true;
		placementIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, GetSpriteNameByPlacement(placement));
		placementIcon.color = Color.white;
		int num = config.GoalScore[placement];
		int num2 = 4 - num.ToString().Length;
		float num3 = scoreBonus.NumSpace / 2f * scoreBonus.transform.localScale.x * (float)num2;
		scoreBonusText.transform.AddLocalPositionX(num3);
		scoreBonus.Set(num);
		scoreBonus.transform.AddLocalPositionX(0f - num3);
		scoreBonusRoot.SetActive(value: true);
		LeanTween.scaleY(scoreBonusRoot, originScoreBonusRootScaleY, 0.5f).setEaseOutBack();
	}
	private string GetSpriteNameByPlacement(int placement)
	{
		string text = $"{useSpriteNameBase}{placement}";
		if (Localization.Language == Localization.SupportedLanguage.English)
		{
			text = "en" + text;
		}
		return text;
	}
	public void Hide()
	{
		placementIcon.gameObject.SetActive(value: false);
		scoreBonusRoot.SetActive(value: false);
	}
}
