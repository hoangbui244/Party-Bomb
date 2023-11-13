using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class SnowBoard_UserUIData : MonoBehaviour
{
	[Serializable]
	public struct BalloonTextData
	{
		[Header("吹き出しの親アンカ\u30fc")]
		public GameObject balloonRoot;
		[Header("吹き出しの下敷き画像")]
		public SpriteRenderer balloonUnderlay;
		[Header("吹き出しのボタン画像")]
		public SpriteRenderer[] balloonButton;
		[Header("吹き出しの文字")]
		public TextMeshPro balloonText;
		public void Init()
		{
			if (!(balloonRoot != null))
			{
				return;
			}
			balloonRoot.SetActive(value: false);
			balloonUnderlay.SetAlpha(0f);
			balloonText.SetAlpha(0f);
			for (int i = 0; i < balloonButton.Length; i++)
			{
				if (balloonButton[i] != null)
				{
					balloonButton[i].SetAlpha(0f);
				}
			}
		}
	}
	public enum TrickType
	{
		Frontside360,
		Frontside720,
		Backside360,
		Backside720,
		Frontflip,
		WFrontflip,
		Backflip,
		WBackflip,
		Rail
	}
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン画像")]
	private SpriteRenderer playerIconSp;
	[SerializeField]
	[Header("チ\u30fcムアイコン画像")]
	private SpriteRenderer teamIconSp;
	[SerializeField]
	[Header("スコアフレ\u30fcムアンカ\u30fc")]
	private Transform scoreFrameAnchor;
	[SerializeField]
	[Header("キャラクタ\u30fcアイコン画像")]
	private SpriteRenderer characterIconSp;
	[SerializeField]
	[Header("順位画像")]
	private SpriteRenderer rankingSp;
	[SerializeField]
	[Header("ゴ\u30fcル順位画像")]
	private SpriteRenderer goalRankingSp;
	[SerializeField]
	[Header("スコア処理")]
	private TextMeshPro scoreNumbers;
	[SerializeField]
	[Header("トリックUI_Frontside360")]
	private SpriteRenderer trick_fs360;
	[SerializeField]
	[Header("トリックUI_Frontside720")]
	private SpriteRenderer trick_fs720;
	[SerializeField]
	[Header("トリックUI_Backside360")]
	private SpriteRenderer trick_bs360;
	[SerializeField]
	[Header("トリックUI_Backside720")]
	private SpriteRenderer trick_bs720;
	[SerializeField]
	[Header("トリックUI_Frontflip")]
	private SpriteRenderer trick_ff;
	[SerializeField]
	[Header("トリックUI_WFrontflip")]
	private SpriteRenderer trick_wff;
	[SerializeField]
	[Header("トリックUI_Backflip")]
	private SpriteRenderer trick_bf;
	[SerializeField]
	[Header("トリックUI_WBackflip")]
	private SpriteRenderer trick_wbf;
	[SerializeField]
	[Header("レ\u30fcルアクションUI")]
	private SpriteRenderer rail;
	[SerializeField]
	[Header("操作吹き出しのデ\u30fcタ")]
	private BalloonTextData balloonTextData;
	private SnowBoard_Define.UserType userType;
	private GameObject rankingSpObj;
	private Vector3 rankingSpScale;
	private int nowRank;
	private bool isGoal;
	public SnowBoard_Define.UserType UserType => userType;
	public void Init(SnowBoard_Define.UserType _userType)
	{
		userType = _userType;
		SetPlayerIcon();
		SetCharacterIcon();
		goalRankingSp.gameObject.SetActive(value: false);
		if (trick_fs360 != null && trick_fs360.gameObject.activeInHierarchy)
		{
			trick_fs360.SetAlpha(0f);
		}
		if (trick_fs720 != null && trick_fs720.gameObject.activeInHierarchy)
		{
			trick_fs720.SetAlpha(0f);
		}
		if (trick_bs360 != null && trick_bs360.gameObject.activeInHierarchy)
		{
			trick_bs360.SetAlpha(0f);
		}
		if (trick_bs720 != null && trick_bs720.gameObject.activeInHierarchy)
		{
			trick_bs720.SetAlpha(0f);
		}
		if (trick_ff != null && trick_ff.gameObject.activeInHierarchy)
		{
			trick_ff.SetAlpha(0f);
		}
		if (trick_wff != null && trick_wff.gameObject.activeInHierarchy)
		{
			trick_wff.SetAlpha(0f);
		}
		if (trick_bf != null && trick_bf.gameObject.activeInHierarchy)
		{
			trick_bf.SetAlpha(0f);
		}
		if (trick_wbf != null && trick_wbf.gameObject.activeInHierarchy)
		{
			trick_wbf.SetAlpha(0f);
		}
		if (rail != null && rail.gameObject.activeInHierarchy)
		{
			rail.SetAlpha(0f);
		}
		if (rankingSp != null && rankingSp.gameObject.activeInHierarchy)
		{
			rankingSp.SetAlpha(0f);
			rankingSpScale = rankingSp.gameObject.transform.localScale;
			rankingSpObj = rankingSp.gameObject;
		}
		nowRank = -1;
	}
	public void SetScore(int _score)
	{
		if (scoreNumbers != null)
		{
			scoreNumbers.text = _score.ToString() + "pt";
		}
	}
	private void SetPlayerIcon()
	{
		if (!(playerIconSp == null))
		{
			switch (userType)
			{
			case SnowBoard_Define.UserType.PLAYER_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
				break;
			case SnowBoard_Define.UserType.PLAYER_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
				break;
			case SnowBoard_Define.UserType.PLAYER_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
				break;
			case SnowBoard_Define.UserType.PLAYER_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
				break;
			case SnowBoard_Define.UserType.CPU_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
				break;
			case SnowBoard_Define.UserType.CPU_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
				break;
			case SnowBoard_Define.UserType.CPU_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
				break;
			case SnowBoard_Define.UserType.CPU_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
				break;
			case SnowBoard_Define.UserType.CPU_5:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
				break;
			}
		}
	}
	private void SetCharacterIcon()
	{
		if (!(characterIconSp == null))
		{
			switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
			{
			case 0:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_0" + 2.ToString());
				break;
			case 1:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_0" + 2.ToString());
				break;
			case 2:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_0" + 2.ToString());
				break;
			case 3:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_0" + 2.ToString());
				break;
			case 4:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_0" + 2.ToString());
				break;
			case 5:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_0" + 2.ToString());
				break;
			case 6:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_0" + 2.ToString());
				break;
			case 7:
				characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_0" + 2.ToString());
				break;
			}
		}
	}
	public void SetRanking(int _set)
	{
		if (!(rankingSp == null) && nowRank != _set)
		{
			switch (_set)
			{
			case 1:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_0");
				rankingSp.sprite.name = "_common_rank_s_0";
				nowRank = 1;
				break;
			case 2:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_1");
				rankingSp.sprite.name = "_common_rank_s_1";
				nowRank = 2;
				break;
			case 3:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_2");
				rankingSp.sprite.name = "_common_rank_s_2";
				nowRank = 3;
				break;
			case 4:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_3");
				rankingSp.sprite.name = "_common_rank_s_3";
				nowRank = 4;
				break;
			case 5:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_4");
				rankingSp.sprite.name = "_common_rank_s_4";
				nowRank = 5;
				break;
			case 6:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_5");
				rankingSp.sprite.name = "_common_rank_s_5";
				nowRank = 6;
				break;
			case 7:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_6");
				rankingSp.sprite.name = "_common_rank_s_6";
				nowRank = 7;
				break;
			case 8:
				rankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_7");
				rankingSp.sprite.name = "_common_rank_s_7";
				nowRank = 8;
				break;
			}
			LeanTween.cancel(rankingSpObj);
			LeanTween.scale(rankingSpObj.gameObject, rankingSpScale, 0.1f).setFrom(new Vector3(0f, 0f, 0f));
			LeanTween.value(rankingSpObj, 0f, 1f, 0.1f).setOnUpdate(delegate(float _value)
			{
				rankingSp.SetAlpha(_value);
			});
		}
	}
	public void SetGoalRanking()
	{
		if (!(goalRankingSp == null) && rankingSp.gameObject.activeSelf && !isGoal)
		{
			isGoal = true;
			switch (rankingSp.sprite.name)
			{
			case "_common_rank_s_0":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_0");
				break;
			case "_common_rank_s_1":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_1");
				break;
			case "_common_rank_s_2":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_2");
				break;
			case "_common_rank_s_3":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_3");
				break;
			case "_common_rank_s_4":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_4");
				break;
			case "_common_rank_s_5":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_5");
				break;
			case "_common_rank_s_6":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_6");
				break;
			case "_common_rank_s_7":
				goalRankingSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_7");
				break;
			}
			rankingSp.gameObject.SetActive(value: false);
			goalRankingSp.gameObject.SetActive(value: true);
			goalRankingSp.gameObject.GetComponent<ResultPlacementAnimation>().Play();
		}
	}
	public void TrickUICall(TrickType _set)
	{
		switch (_set)
		{
		case TrickType.Frontside360:
			if (trick_fs360 != null && trick_fs360.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_fs360, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_fs360));
			}
			break;
		case TrickType.Frontside720:
			if (trick_fs720 != null && trick_fs720.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_fs720, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_fs720));
			}
			break;
		case TrickType.Backside360:
			if (trick_bs360 != null && trick_bs360.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_bs360, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_bs360));
			}
			break;
		case TrickType.Backside720:
			if (trick_bs720 != null && trick_bs720.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_bs720, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_bs720));
			}
			break;
		case TrickType.Frontflip:
			if (trick_ff != null && trick_ff.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_ff, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_ff));
			}
			break;
		case TrickType.WFrontflip:
			if (trick_wff != null && trick_wff.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_wff, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_wff));
			}
			break;
		case TrickType.Backflip:
			if (trick_bf != null && trick_bf.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_bf, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_bf));
			}
			break;
		case TrickType.WBackflip:
			if (trick_wbf != null && trick_wbf.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(trick_wbf, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(trick_wbf));
			}
			break;
		case TrickType.Rail:
			if (rail != null && rail.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeProcess(rail, 1f, 0.2f));
				StartCoroutine(TrickUIFadeoutWait(rail));
			}
			break;
		}
	}
	public void FadeProcess_ControlInfomationUI(bool _fadeIn)
	{
		if (balloonTextData.balloonRoot == null || !balloonTextData.balloonRoot.activeInHierarchy)
		{
			return;
		}
		if (_fadeIn)
		{
			StartCoroutine(FadeProcess(balloonTextData.balloonUnderlay, 1f, 0.5f));
			StartCoroutine(FadeProcess(balloonTextData.balloonText, 1f, 0.5f));
			for (int i = 0; i < balloonTextData.balloonButton.Length; i++)
			{
				if (balloonTextData.balloonButton[i] != null)
				{
					StartCoroutine(FadeProcess(balloonTextData.balloonButton[i], 1f, 0.5f));
				}
			}
			return;
		}
		StartCoroutine(FadeProcess(balloonTextData.balloonUnderlay, 0f, 0.5f));
		StartCoroutine(FadeProcess(balloonTextData.balloonText, 0f, 0.5f));
		for (int j = 0; j < balloonTextData.balloonButton.Length; j++)
		{
			if (balloonTextData.balloonButton[j] != null)
			{
				StartCoroutine(FadeProcess(balloonTextData.balloonButton[j], 0f, 0.5f));
			}
		}
	}
	private IEnumerator FadeProcess(SpriteRenderer _sprite, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float startAlpha = _sprite.color.a;
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			if (_sprite != null)
			{
				_sprite.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			}
			time += Time.deltaTime;
			yield return null;
		}
		_sprite.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
	private IEnumerator FadeProcess(TextMeshPro _text, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float startAlpha = _text.color.a;
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			if (_text != null)
			{
				_text.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			}
			time += Time.deltaTime;
			yield return null;
		}
		_text.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
	private IEnumerator TrickUIFadeoutWait(SpriteRenderer _sp)
	{
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(FadeProcess(_sp, 0f, 0.2f));
	}
}
