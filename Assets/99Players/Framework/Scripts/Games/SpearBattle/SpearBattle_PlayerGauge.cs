using UnityEngine;
public class SpearBattle_PlayerGauge : MonoBehaviour
{
	[SerializeField]
	[Header("左画面かどうか")]
	private bool isLeftScreen;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private SpriteRenderer spPlayerNo;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer spPlayerIcon;
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer spFrame;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private Sprite[] arrayFrame;
	[SerializeField]
	[Header("ゲ\u30fcジ黄色")]
	private SpriteRenderer gaugeYellow;
	[SerializeField]
	[Header("ゲ\u30fcジ赤色")]
	private SpriteRenderer gaugeRed;
	[SerializeField]
	[Header("プレイヤ\u30fcフレ\u30fcム")]
	private SpriteRenderer[] arrayPlayerFrame;
	private float currentScale;
	private float nowHpScale;
	public void Init()
	{
		if (!isLeftScreen)
		{
			int styleCharaNo = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.styleCharaNo;
		}
		else
		{
			int styleCharaNo2 = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.styleCharaNo;
		}
		spPlayerNo.sprite = SingletonCustom<SpearBattle_UIManager>.Instance.GetNowWorldPlayerIconSprite(isLeftScreen);
		spPlayerIcon.sprite = SingletonCustom<SpearBattle_UIManager>.Instance.GetNowCharaIconSprite(isLeftScreen);
		arrayPlayerFrame[0].color = SingletonCustom<SpearBattle_UIManager>.Instance.GetNowPlayerFrameColor(isLeftScreen);
		currentScale = 1f;
		nowHpScale = 1f;
		gaugeYellow.transform.SetLocalScaleX(1f);
		LeanTween.cancel(gaugeYellow.gameObject);
		gaugeRed.transform.SetLocalScaleX(1f);
	}
	public void UpdateMethod()
	{
		if (currentScale != nowHpScale)
		{
			currentScale = nowHpScale;
			gaugeYellow.transform.SetLocalScaleX(nowHpScale);
			LeanTween.cancel(gaugeYellow.gameObject);
			LeanTween.scaleX(gaugeRed.gameObject, nowHpScale, 0.25f).setDelay(0.25f);
		}
	}
	public void SetNowHpScale(float _scale)
	{
		nowHpScale = _scale;
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(gaugeYellow.gameObject);
	}
}
