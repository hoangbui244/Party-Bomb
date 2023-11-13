using System;
using System.Collections;
using UnityEngine;
public class ShortTrack_LayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("タイムUI")]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("変動順位アイコン")]
	private SpriteRenderer changeRankIcon;
	[SerializeField]
	[Header("ゴ\u30fcル順位アイコン")]
	private SpriteRenderer goalRankIcon;
	[SerializeField]
	[Header("加速時のボタンUI")]
	private ShortTrack_DashButtonPress butonPressUI;
	[SerializeField]
	[Header("スタミナゲ\u30fcジのUI")]
	private ShortTrack_StaminaGauge GaugeUI;
	[SerializeField]
	[Header("現在のラップ数")]
	private SpriteRenderer currentLapSprite;
	[SerializeField]
	[Header("ト\u30fcタルラップ数")]
	private SpriteRenderer totalLapSprite;
	[SerializeField]
	[Header("最終ラップ")]
	private SpriteRenderer finalLap;
	[SerializeField]
	[Header("最終ラップ用のアニメ\u30fcションカ\u30fcブ（サイズ）")]
	private AnimationCurve finalLapScaleCurve;
	[SerializeField]
	[Header("最終ラップ用のアニメ\u30fcションカ\u30fcブ（透過）")]
	private AnimationCurve finalLapAlphaCurve;
	[SerializeField]
	[Header("最終ラップ用のアニメ\u30fcションカ\u30fcブ（座標X）")]
	private AnimationCurve finalLapMoveXCurve;
	[SerializeField]
	[Header("ゲ\u30fcムの説明")]
	private GameObject infoIcon;
	private Vector3 finalLapScale;
	private Vector3 finalLapPos;
	private Vector3 changeRankIconScale;
	private Vector3 playerIconScale;
	private float goalRankMoveTime;
	private float goalRankTime;
	private const float GOAL_RANK_TIME = 3f;
	private Vector3 goalRankStartScale;
	[SerializeField]
	[Header("ゴ\u30fcル順位の最後の大きさ")]
	private Vector3 goalRankEndScale;
	private Vector3 goalRankPos;
	[SerializeField]
	[Header("順位変動アイコンの透明")]
	private Color startColor;
	private float moveTime;
	public void Init()
	{
		finalLapPos = finalLap.gameObject.transform.localPosition;
		finalLapScale = finalLap.gameObject.transform.localScale;
		changeRankIconScale = changeRankIcon.transform.localScale;
		changeRankIcon.color = startColor;
		goalRankPos = goalRankIcon.transform.localPosition;
		goalRankStartScale = goalRankIcon.transform.localScale;
		playerIconScale = playerIcon.gameObject.transform.localScale;
		butonPressUI.Init();
		GaugeUI.Init();
		playerIcon.gameObject.SetActive(value: false);
		goalRankIcon.gameObject.SetActive(value: false);
		SetCurrentLap(0);
		totalLapSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + 9.ToString());
		finalLap.gameObject.SetActive(value: false);
		finalLap.transform.localScale = Vector3.zero;
		finalLap.SetAlpha(0f);
	}
	public ShortTrack_DashButtonPress GetDashButtonPressUI()
	{
		return butonPressUI;
	}
	public ShortTrack_StaminaGauge GetStaminaGaugeUI()
	{
		return GaugeUI;
	}
	public void SetTime(float _time)
	{
		timeUI.SetTime(_time);
	}
	public void SetPlayerIcon(int _userType)
	{
		playerIcon.transform.localScale = playerIconScale;
		playerIcon.gameObject.SetActive(value: true);
		if (_userType < 4)
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_userType + 1).ToString() + "p");
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_userType - 4 + 1).ToString());
		}
	}
	public void RightUpInfoIconHide()
	{
		infoIcon.SetActive(value: false);
	}
	public void SetChangeRankIcon(int _rankNum)
	{
		if (Localize_Define.Language != 0)
		{
			changeRankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "en_common_rank_s_" + _rankNum.ToString());
		}
		else
		{
			changeRankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_" + _rankNum.ToString());
		}
	}
	public void ChangeRankIconShow(bool flg)
	{
		moveTime = Mathf.Clamp(moveTime + Time.deltaTime / 0.2f, 0f, 1f);
		changeRankIcon.color = Color.Lerp(startColor, Color.white, moveTime);
		if (moveTime >= 1f)
		{
			flg = true;
		}
	}
	public void NonActiveChangeRankIcon()
	{
		changeRankIcon.gameObject.SetActive(value: false);
	}
	public void SetGoalRankIcon(int _rankNum)
	{
		if (Localize_Define.Language == Localize_Define.LanguageType.Japanese)
		{
			goalRankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_" + _rankNum.ToString());
		}
		else if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			goalRankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "en_common_rank_" + _rankNum.ToString());
		}
	}
	public void ActiveGoalRankIcon()
	{
		goalRankIcon.gameObject.SetActive(value: true);
	}
	public void GoalRankIconMove()
	{
		if (goalRankIcon.gameObject.activeSelf)
		{
			goalRankTime += Time.deltaTime;
		}
		if (goalRankTime >= 3f)
		{
			goalRankMoveTime = Mathf.Clamp(goalRankMoveTime + Time.deltaTime, 0f, 1f);
			goalRankIcon.transform.localPosition = Vector3.Lerp(goalRankPos, changeRankIcon.transform.localPosition, goalRankMoveTime);
			goalRankIcon.transform.localScale = Vector3.Lerp(goalRankStartScale, goalRankEndScale, goalRankMoveTime);
		}
	}
	private IEnumerator SetAlphaColor(SpriteRenderer _tk2dSprite, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false)
	{
		float time = 0f;
		Color color = Color.white;
		float startAlpha = 0f;
		float endAlpha = 1f;
		if (_isFadeOut)
		{
			startAlpha = 1f;
			endAlpha = 0f;
		}
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			color = _tk2dSprite.color;
			color.a = Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime);
			_tk2dSprite.color = color;
			time += Time.deltaTime;
			yield return null;
		}
	}
	public Vector3 GetRankIconScale()
	{
		return changeRankIcon.transform.localScale;
	}
	public void SetRankIconScale(float _scale)
	{
		changeRankIcon.transform.SetLocalScale(Mathf.Clamp(_scale, 0f, changeRankIconScale.x), Mathf.Clamp(_scale, 0f, changeRankIconScale.y), 1f);
	}
	public void SetCurrentLap(int _lapNum)
	{
		currentLapSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + (_lapNum + 1).ToString());
	}
	public void PlayLastLapAnimation()
	{
		finalLap.gameObject.SetActive(value: true);
		finalLap.transform.SetLocalPositionX(finalLapPos.x);
		LeanTween.value(finalLap.gameObject, 0f, 1f, 0.9f).setOnUpdate(delegate(float _value)
		{
			float value = finalLapScaleCurve.Evaluate(_value);
			finalLap.transform.localScale = new Vector3(Mathf.Clamp(value, 0f, finalLapScale.x), Mathf.Clamp(value, 0f, finalLapScale.y), 1f);
			float a = finalLapAlphaCurve.Evaluate(_value);
			finalLap.color = new Color(1f, 1f, 1f, a);
			finalLap.transform.AddLocalPositionX(finalLapMoveXCurve.Evaluate(_value) * 15f * Time.deltaTime);
		}).setOnComplete((Action)delegate
		{
			finalLap.gameObject.SetActive(value: false);
		});
	}
}
