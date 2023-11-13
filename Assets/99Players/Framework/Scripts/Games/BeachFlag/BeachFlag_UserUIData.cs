using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class BeachFlag_UserUIData : MonoBehaviour
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
	[Header("コ\u30fcスアウト表示")]
	private SpriteRenderer courseOutSp;
	[SerializeField]
	[Header("操作吹き出しのデ\u30fcタ")]
	private BalloonTextData balloonTextData;
	private BeachFlag_Define.UserType userType;
	private GameObject rankingSpObj;
	private Vector3 rankingSpScale;
	private int nowRank;
	private bool isGoal;
	public BeachFlag_Define.UserType UserType => userType;
	public void Init(BeachFlag_Define.UserType _userType)
	{
		userType = _userType;
		SetPlayerIcon();
		SetCharacterIcon();
		goalRankingSp.gameObject.SetActive(value: false);
		if (courseOutSp != null)
		{
			courseOutSp.SetAlpha(0f);
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
			case BeachFlag_Define.UserType.PLAYER_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
				break;
			case BeachFlag_Define.UserType.PLAYER_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
				break;
			case BeachFlag_Define.UserType.PLAYER_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
				break;
			case BeachFlag_Define.UserType.PLAYER_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
				break;
			case BeachFlag_Define.UserType.CPU_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
				break;
			case BeachFlag_Define.UserType.CPU_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
				break;
			case BeachFlag_Define.UserType.CPU_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
				break;
			case BeachFlag_Define.UserType.CPU_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
				break;
			case BeachFlag_Define.UserType.CPU_5:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
				break;
			}
		}
	}
	public void FadeOutPlayerIcon()
	{
		if (!(playerIconSp == null))
		{
			StartCoroutine(FadeProcess(playerIconSp, 0f, 0.5f));
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
			}
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
			}
			rankingSp.gameObject.SetActive(value: false);
			goalRankingSp.gameObject.SetActive(value: true);
			goalRankingSp.gameObject.GetComponent<ResultPlacementAnimation>().Play();
		}
	}
	public void CourseOutWarning()
	{
		if (!(courseOutSp == null) && courseOutSp.gameObject.activeInHierarchy)
		{
			StartCoroutine(FadeProcess(courseOutSp, 1f, 0.2f));
			StartCoroutine(CourseOutWarningWait());
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
	private IEnumerator CourseOutWarningWait()
	{
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(FadeProcess(courseOutSp, 0f, 0.2f));
	}
}
