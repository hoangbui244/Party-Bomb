using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwordFight_GameUiManager : SingletonCustom<SwordFight_GameUiManager>
{
	[Serializable]
	public struct SetIconSprite
	{
		public SpriteRenderer[] setIconSprite;
	}
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private GameObject gameTimeText;
	[SerializeField]
	[Header("ゲ\u30fcム時間_分")]
	private SpriteNumbers gameTimeText_Minutes;
	[SerializeField]
	[Header("ゲ\u30fcム時間_秒")]
	private SpriteNumbers gameTimeText_Second;
	[SerializeField]
	[Header("スキップ")]
	private ControllerBalloonUI skipButton;
	[SerializeField]
	[Header("現在の勝ち抜き人数アンカ\u30fc")]
	private GameObject nowWinningNumAnchor;
	[SerializeField]
	[Header("現在の勝ち抜き人数数値")]
	private SpriteNumbers nowWinningNumSpriteNumbers;
	[SerializeField]
	[Header("ポ\u30fcズUIオブエジェクト")]
	private GameObject pauseUIObject;
	[SerializeField]
	[Header("操作説明オブジェクト")]
	private GameObject controleHelp;
	private int[] playerGetSetNum = new int[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	[SerializeField]
	[Header("各プレイヤ\u30fcのセットのアイコン下敷き画像")]
	private SetIconSprite[] teamSetIconUnderlaySprite_Coop;
	private int[] teamGetSetNum = new int[SwordFight_Define.MAX_GAME_TEAM_NUM];
	private List<int>[] teamMemberNoList;
	private int playerNum = 1;
	private int setNum = 1;
	private bool isTeamMode;
	private float gameTime;
	private bool isStopTime = true;
	public bool IsSkipShow => skipButton.gameObject.activeSelf;
	public bool IsStopTime
	{
		get
		{
			return isStopTime;
		}
		set
		{
			isStopTime = value;
		}
	}
	public void Init()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		isStopTime = true;
		setNum = 2;
		isTeamMode = SwordFight_Define.IS_TEAM_MODE;
		nowWinningNumAnchor.SetActive(value: false);
		if (isTeamMode)
		{
			teamMemberNoList = SwordFight_Define.PlayerGroupList;
			Init_CoopMode();
			gameTimeText.transform.SetLocalPositionX(0f);
		}
		else
		{
			Init_BattleMode();
			gameTimeText.transform.SetLocalPositionX(648f);
		}
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode)
		{
			gameTimeText_Second.Set(99);
			gameTimeText_Minutes.Set(99);
		}
		else
		{
			gameTimeText_Second.Set(1);
			gameTimeText_Minutes.Set(0);
		}
		gameTimeText.SetActive(value: false);
		skipButton.Init();
	}
	private void Update()
	{
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.IsGameStart && !SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode && !isStopTime)
		{
			if (gameTime <= 0f)
			{
				gameTime = 0f;
				UpdateGameTime(gameTime);
				SingletonCustom<SwordFight_MainGameManager>.Instance.EndRound();
			}
			UpdateGameTime(gameTime);
		}
	}
	public void SettingGameTime()
	{
		gameTime = SwordFight_Define.GAME_TIME_TOTAL;
		UpdateGameTime(gameTime);
	}
	private void UpdateGameTime(float _time)
	{
		bool isAnimationDebugMode = SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode;
	}
	private void Init_BattleMode()
	{
		for (int i = 0; i < playerGetSetNum.Length; i++)
		{
			playerGetSetNum[i] = 0;
		}
		pauseUIObject.SetActive(value: true);
		controleHelp.SetActive(value: true);
	}
	private void Init_CoopMode()
	{
		pauseUIObject.SetActive(value: false);
		controleHelp.SetActive(value: false);
		for (int i = 0; i < teamGetSetNum.Length; i++)
		{
			teamGetSetNum[i] = 0;
		}
		for (int j = 0; j < teamSetIconUnderlaySprite_Coop.Length; j++)
		{
			SpriteRenderer[] setIconSprite = teamSetIconUnderlaySprite_Coop[j].setIconSprite;
			for (int k = 0; k < setIconSprite.Length; k++)
			{
				setIconSprite[k].gameObject.SetActive(value: false);
			}
		}
	}
	public void ShowSkip()
	{
		skipButton.FadeProcess_ControlInfomationUI(_fadeIn: true);
	}
	public void HideSkip()
	{
		skipButton.SetControlInfomationUIActive(_isActive: false);
	}
	public void ResetSet()
	{
		for (int i = 0; i < playerGetSetNum.Length; i++)
		{
			playerGetSetNum[i] = 0;
		}
		skipButton.Init();
	}
	public void AddSet(int _playerNo, int _teamNo)
	{
		skipButton.SetControlInfomationUIActive(_isActive: false);
		if (isTeamMode)
		{
			if (_teamNo < 0)
			{
				UnityEngine.Debug.Log("同時に死んだので引き分け！");
			}
			else
			{
				teamGetSetNum[_teamNo]++;
			}
		}
		else if (_playerNo < 0)
		{
			UnityEngine.Debug.Log("同時に死んだので引き分け！");
		}
		else
		{
			playerGetSetNum[_playerNo]++;
		}
	}
	public int GetSetNum(int _playerNo, int _teamNo)
	{
		if (isTeamMode)
		{
			return teamGetSetNum[_teamNo];
		}
		return playerGetSetNum[_playerNo];
	}
	private IEnumerator SetAnimation(SpriteRenderer _sprite)
	{
		_sprite.SetAlpha(0f);
		_sprite.transform.SetLocalEulerAnglesZ(180f);
		_sprite.transform.SetLocalScale(2f, 2f, 2f);
		_sprite.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		LeanTween.value(_sprite.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float alpha)
		{
			_sprite.SetAlpha(alpha);
		});
		LeanTween.rotateZ(_sprite.gameObject, 0f, 0.5f);
		LeanTween.scale(_sprite.gameObject, new Vector3(1f, 1f, 1f), 0.5f);
	}
	public float GetGameTime()
	{
		return gameTime;
	}
}
