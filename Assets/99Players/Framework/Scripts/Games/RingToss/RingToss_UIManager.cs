using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class RingToss_UIManager : SingletonCustom<RingToss_UIManager>
{
	[Serializable]
	private class ControlInfomationUI
	{
		[Header("アンカ\u30fc")]
		public GameObject anchor;
		[Header("操作情報を示す画像デ\u30fcタ")]
		public SpriteRenderer[] infomationSpriteUI;
		[Header("操作情報を示す文字デ\u30fcタ")]
		public TextMeshPro[] infomationTextUI;
		public float NowAlpha
		{
			get;
			set;
		}
		public void SetActive(bool _active)
		{
			anchor.SetActive(_active);
		}
		public void SetAlpha(float _alpha)
		{
			for (int i = 0; i < infomationSpriteUI.Length; i++)
			{
				infomationSpriteUI[i].SetAlpha(_alpha);
			}
			for (int j = 0; j < infomationTextUI.Length; j++)
			{
				infomationTextUI[j].SetAlpha(_alpha);
			}
			NowAlpha = _alpha;
		}
	}
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("共通レイアウト")]
	private CommonWaterPistolBattleUILayout commonUILayout;
	[SerializeField]
	private GameObject singleOnlyObj;
	[SerializeField]
	private GameObject multiOnlyObj;
	[SerializeField]
	private Transform multiOperationAnchor;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI firstCtrlUI;
	[SerializeField]
	[Header("スキップ表示UI")]
	private ControlInfomationUI skipCtrlUI;
	[SerializeField]
	private SpriteRenderer[] worldPlayerSprites;
	private float worldPlayerViewTime;
	private bool isWorldPlayerFadeStart;
	private bool isWorldPlayerFadeEnd;
	[SerializeField]
	[Header("ポイントゲット表示")]
	private RingToss_GetPointUi[] getPointUis;
	private int[] ctrlNoTemp;
	private int[] scoreNoTemp;
	private readonly float MAX_TEXT_POSX = -17.5f;
	[SerializeField]
	[Header("弾数表示アンカ\u30fc")]
	private GameObject[] arrayRemainingBulletObjAnchor;
	[SerializeField]
	[Header("弾数のテキスト")]
	private TextMeshPro[] arrayRemainingBulletNumText;
	[SerializeField]
	[Header("弾数のSpriteNumbers")]
	private SpriteNumbers[] arrayRemainingBulletSpriteNum;
	[SerializeField]
	[Header("弾数近くのプレイヤ\u30fc表示スプライト")]
	private SpriteRenderer[] arrayRemainingBulletPlayerSprite;
	public void Init()
	{
		ctrlNoTemp = SingletonCustom<RingToss_ControllerManager>.Instance.GetArrayCtrlNo();
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleOnlyObj.SetActive(value: true);
			multiOnlyObj.SetActive(value: false);
			worldPlayerSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_you");
		}
		else
		{
			singleOnlyObj.SetActive(value: false);
			multiOnlyObj.SetActive(value: true);
		}
		if (SingletonCustom<RingToss_GameManager>.Instance.TeamNum == 2)
		{
			multiOperationAnchor.SetLocalPositionY(434f);
		}
		int num = SingletonCustom<RingToss_GameManager>.Instance.PlayerNum;
		if (SingletonCustom<RingToss_GameManager>.Instance.HasSecondGroup)
		{
			num = 2;
		}
		for (int i = 0; i < worldPlayerSprites.Length; i++)
		{
			worldPlayerSprites[i].gameObject.SetActive(i < num);
			if (worldPlayerSprites[i].gameObject.activeSelf && SingletonCustom<RingToss_GameManager>.Instance.HasSecondGroup)
			{
				worldPlayerSprites[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RingToss_Define.WORLD_PLAYER_SPRITE_NAMES[SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(i)]);
			}
		}
		SetRemainingBulletUI();
		SetControlUI();
		for (int j = 0; j < RingToss_Define.MAX_PLAYER_NUM; j++)
		{
			UpdateRemainingRingNumUI(j, SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum);
		}
		for (int k = 0; k < getPointUis.Length; k++)
		{
			getPointUis[k].Init();
			getPointUis[k].SetColor(RingToss_Define.CHARA_COLORS[k]);
			getPointUis[k].SetScale(1.5f);
		}
		WorldPlayerSpriteUpdate();
		commonUILayout.Init(60f, SingletonCustom<RingToss_GameManager>.Instance.PlayerGroupList, _isEightBattle: false, SingletonCustom<RingToss_GameManager>.Instance.HasSecondGroup);
	}
	public void SecondGroupInit()
	{
		SetRemainingBulletUI();
		SetControlUI();
		int num = SingletonCustom<RingToss_GameManager>.Instance.PlayerNum / 2;
		for (int i = 0; i < worldPlayerSprites.Length; i++)
		{
			worldPlayerSprites[i].gameObject.SetActive(i < num);
			if (worldPlayerSprites[i].gameObject.activeSelf)
			{
				worldPlayerSprites[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RingToss_Define.WORLD_PLAYER_SPRITE_NAMES[SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(i)]);
			}
		}
		for (int j = 0; j < RingToss_Define.MAX_PLAYER_NUM; j++)
		{
			UpdateRemainingRingNumUI(j, SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum);
		}
		worldPlayerViewTime = 0f;
		isWorldPlayerFadeStart = false;
		isWorldPlayerFadeEnd = false;
		for (int k = 0; k < worldPlayerSprites.Length; k++)
		{
			worldPlayerSprites[k].SetAlpha(1f);
		}
		WorldPlayerSpriteUpdate();
		commonUILayout.SecondGroupInit();
	}
	private void SetRemainingBulletUI()
	{
		if (SingletonCustom<RingToss_GameManager>.Instance.IsSingle)
		{
			for (int i = 1; i < arrayRemainingBulletObjAnchor.Length; i++)
			{
				arrayRemainingBulletObjAnchor[i].SetActive(value: false);
			}
			arrayRemainingBulletPlayerSprite[0].gameObject.SetActive(value: false);
			arrayRemainingBulletObjAnchor[0].transform.SetLocalPositionX(-200f);
		}
		for (int j = 0; j < arrayRemainingBulletPlayerSprite.Length; j++)
		{
			int playerNo = SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(j);
			arrayRemainingBulletPlayerSprite[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RingToss_Define.PLAYER_SPRITE_NAMES[playerNo]);
		}
	}
	private void SetControlUI()
	{
		firstCtrlUI.SetActive(_active: false);
		firstCtrlUI.SetAlpha(0f);
		skipCtrlUI.SetActive(_active: false);
		skipCtrlUI.SetAlpha(0f);
	}
	public void UpdateMethod()
	{
		WorldPlayerSpriteUpdate();
	}
	public void UpdateScoreUI(int _ctrlNo, int _score)
	{
		commonUILayout.SetScore(_ctrlNo, _score);
	}
	public void UpdateTimeUI(float _time)
	{
		commonUILayout.SetTime(_time);
	}
	private void WorldPlayerSpriteUpdate()
	{
		if (isWorldPlayerFadeEnd)
		{
			return;
		}
		worldPlayerViewTime += Time.deltaTime;
		if (!isWorldPlayerFadeStart && worldPlayerViewTime > 3f)
		{
			isWorldPlayerFadeStart = true;
			LeanTween.value(base.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float _value)
			{
				for (int k = 0; k < worldPlayerSprites.Length; k++)
				{
					worldPlayerSprites[k].SetAlpha(_value);
				}
			}).setOnComplete((Action)delegate
			{
				for (int j = 0; j < worldPlayerSprites.Length; j++)
				{
					worldPlayerSprites[j].gameObject.SetActive(value: false);
				}
				isWorldPlayerFadeEnd = true;
			});
		}
		RingToss_Controller[] arrayController = SingletonCustom<RingToss_ControllerManager>.Instance.ArrayController;
		Camera worldCamera = SingletonCustom<RingToss_GameManager>.Instance.WorldCamera;
		for (int i = 0; i < arrayController.Length; i++)
		{
			RingToss_Controller ringToss_Controller = arrayController[i];
			if (ringToss_Controller.IsPlayer)
			{
				Vector3 position = worldCamera.WorldToScreenPoint(ringToss_Controller.GetPlayerUiPos());
				position = SingletonCustom<RingToss_GameManager>.Instance.UICamera.ScreenToWorldPoint(position);
				worldPlayerSprites[ringToss_Controller.CtrlNo].transform.position = position;
				worldPlayerSprites[ringToss_Controller.CtrlNo].transform.SetLocalPositionZ(i);
			}
		}
	}
	public void PlayGetPointAnimation(int _ctrlNo, int _point, Vector3 _pos)
	{
		getPointUis[_ctrlNo].Show(_ctrlNo, _point, _pos);
	}
	public void UpdateRemainingRingNumUI(int _ctrlNo, int _bulletNum)
	{
		if (_bulletNum < 0)
		{
			_bulletNum = 0;
		}
		arrayRemainingBulletSpriteNum[_ctrlNo].Set(_bulletNum);
	}
	public void ViewRingEnd(int _ctrlNo)
	{
		commonUILayout.SetEndChara(_ctrlNo);
	}
	public void ViewFirstControlInfo()
	{
		firstCtrlUI.SetActive(_active: true);
		ControlInfomationFade(firstCtrlUI, 1f, 0.5f);
		ControlInfomationFade(firstCtrlUI, 0f, 0.5f, 3f, delegate
		{
			firstCtrlUI.SetActive(_active: false);
		});
	}
	public void ViewSkipControlInfo()
	{
		skipCtrlUI.SetActive(_active: true);
		ControlInfomationFade(skipCtrlUI, 1f, 0.5f, 1f);
	}
	public void Fade(float _time, float _delay, Action _act)
	{
		Color color = fade.color;
		color.a = 1f;
		fade.SetAlpha(0f);
		fade.gameObject.SetActive(value: true);
		LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				_act();
			});
		color.a = 0f;
		LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				fade.gameObject.SetActive(fade);
			});
	}
	private void ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		StartCoroutine(_ControlInfomationFade(_infoUI, _setAlpha, _fadeTime, _delayTime, _callback));
	}
	private IEnumerator _ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		yield return new WaitForSeconds(_delayTime);
		float startAlpha = _infoUI.NowAlpha;
		while (time < _fadeTime)
		{
			_infoUI.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			time += Time.deltaTime;
			yield return null;
		}
		_infoUI.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
}
