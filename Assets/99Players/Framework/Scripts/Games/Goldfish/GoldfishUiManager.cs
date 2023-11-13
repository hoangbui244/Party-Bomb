using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GoldfishUiManager : SingletonCustom<GoldfishUiManager>
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
	private const string WORLD_PLAYER_SPRITE_SINGLE_NAME = "_common_c_you";
	private static readonly Color[] CHARA_COLORS = new Color[8]
	{
		new Color(86f / 255f, 0.8862746f, 0.1686275f),
		new Color(1f, 0.3058824f, 32f / 85f),
		new Color(0.3568628f, 172f / 255f, 1f),
		new Color(1f, 0.882353f, 0.04705883f),
		new Color(29f / 51f, 0.3294118f, 41f / 51f),
		new Color(0.01568628f, 0.9490197f, 1f),
		new Color(0.3921569f, 23f / 51f, 0.4235294f),
		new Color(1f, 2f / 3f, 0.1215686f)
	};
	private const float WORLD_PLAYER_SPRITE_VIEW_TIME = 3f;
	private const float WORLD_PLAYER_SPRITE_FADE_TIME = 1f;
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
	[SerializeField]
	private Transform getPointUiAnchor;
	[SerializeField]
	private GoldfishGetPointUi getPointUiPrefab;
	private List<GoldfishGetPointUi> getPointUiList = new List<GoldfishGetPointUi>();
	private int[] teamPlayerNos_TeamA = new int[2];
	private int[] teamPlayerNos_TeamB = new int[2];
	private float worldPlayerViewTime;
	private bool isWorldPlayerFadeStart;
	private bool isWorldPlayerFadeEnd;
	public void Init()
	{
		DataInit();
		int playerNum = SingletonCustom<GoldfishGameManager>.Instance.PlayerNum;
		int teamNum = SingletonCustom<GoldfishGameManager>.Instance.TeamNum;
		int charaNum = SingletonCustom<GoldfishGameManager>.Instance.CharaNum;
		if (playerNum == 1)
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
		if (teamNum == 2)
		{
			multiOperationAnchor.SetLocalPositionY(434f);
		}
		for (int i = 0; i < worldPlayerSprites.Length; i++)
		{
			worldPlayerSprites[i].gameObject.SetActive(i < playerNum);
		}
		for (int j = 0; j < 10; j++)
		{
			GoldfishGetPointUi goldfishGetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(goldfishGetPointUi);
			goldfishGetPointUi.Init();
		}
		commonUILayout.Init(SingletonCustom<GoldfishGameManager>.Instance.RemainViewTime, null, SingletonCustom<GoldfishGameManager>.Instance.IsEightBattle);
	}
	public void SecondGroupInit()
	{
		commonUILayout.SecondGroupInit();
		DataInit();
	}
	private void DataInit()
	{
		firstCtrlUI.SetActive(_active: false);
		firstCtrlUI.SetAlpha(0f);
		skipCtrlUI.SetActive(_active: false);
		skipCtrlUI.SetAlpha(0f);
		worldPlayerViewTime = 0f;
		isWorldPlayerFadeStart = false;
		isWorldPlayerFadeEnd = false;
	}
	public void UpdateMethod()
	{
		TimeUpdate();
		WorldPlayerSpriteUpdate();
	}
	public void TimeUpdate()
	{
		float remainViewTime = SingletonCustom<GoldfishGameManager>.Instance.RemainViewTime;
		commonUILayout.SetTime(remainViewTime);
	}
	public void ScoreUpdate()
	{
		int[] scores = SingletonCustom<GoldfishGameManager>.Instance.GetScores();
		commonUILayout.SetScoreArray(scores);
	}
	public void ScoreUpdate(int _no)
	{
		int score = SingletonCustom<GoldfishGameManager>.Instance.GetScore(_no);
		commonUILayout.SetScore(_no, score);
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
		int charaNum = SingletonCustom<GoldfishGameManager>.Instance.CharaNum;
		Camera camera = SingletonCustom<GoldfishGameManager>.Instance.GetCamera();
		for (int i = 0; i < charaNum; i++)
		{
			GoldfishCharacterScript chara = SingletonCustom<GoldfishCharacterManager>.Instance.GetChara(i);
			if (chara.IsPlayer)
			{
				Vector3 position = camera.WorldToScreenPoint(chara.GetPoi().GetPlayerUiPos());
				position = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(position);
				worldPlayerSprites[chara.PlayerNo].transform.position = position;
				worldPlayerSprites[chara.PlayerNo].transform.SetLocalPositionZ(i);
			}
		}
	}
	public void EndGame()
	{
	}
	public void CloseGameUI()
	{
	}
	public void ViewOut(GoldfishCharacterScript _chara)
	{
		if (SingletonCustom<GoldfishGameManager>.Instance.TeamNum != 2 || _chara.IsPlayer || _chara.TeamNo <= 0)
		{
			commonUILayout.SetEndChara(_chara.CharaNo);
		}
	}
	public void ViewGetPoint(int _charaNo, int _point, Vector3 _worldPos)
	{
		Color color = CHARA_COLORS[_charaNo];
		bool flag = false;
		for (int i = 0; i < getPointUiList.Count; i++)
		{
			if (!getPointUiList[i].IsShow)
			{
				getPointUiList[i].SetColor(color);
				getPointUiList[i].Show(_point, _worldPos);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			GoldfishGetPointUi goldfishGetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(goldfishGetPointUi);
			goldfishGetPointUi.SetColor(color);
			goldfishGetPointUi.Show(_point, _worldPos);
		}
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
