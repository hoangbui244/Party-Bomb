using System;
using System.Collections;
using UnityEngine;
public class SwordFight_CharacterUIManager : SingletonCustom<SwordFight_CharacterUIManager>
{
	[Serializable]
	public struct DeffenceIcon
	{
		public SpriteRenderer[] deffenceIcon;
	}
	[SerializeField]
	[Header("キャラUIリスト")]
	private GameObject[] charaUIList;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer[] playerIcon;
	[SerializeField]
	[Header("プレイヤ\u30fcゲ\u30fcジ")]
	private SwordFight_PlayerGauge[] playerGauge;
	[SerializeField]
	[Header("防御回数アイコン")]
	private DeffenceIcon[] deffenceIconList;
	[SerializeField]
	[Header("HPゲ\u30fcジ")]
	private SpriteRenderer[] arrayHpGauge;
	[SerializeField]
	[Header("プレイヤ\u30fcフレ\u30fcム")]
	private SpriteRenderer[] arrayPlayerFrame;
	[SerializeField]
	[Header("フレ\u30fcムカラ\u30fc")]
	private Color[] arrayPlayerFrameColor;
	private float[] showPosOffset = new float[4];
	private SwordFight_CharacterScript[] showNameChara = new SwordFight_CharacterScript[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	private int playerNum = 1;
	private bool isTeamMode;
	private bool isSingleBattle;
	[SerializeField]
	[Header("ト\u30fcナメント戦UI管理処理")]
	private MS_TournamentUIManager tournamentUIManager;
	[SerializeField]
	[Header("ト\u30fcナメント戦UIオブジェクト")]
	private GameObject object_Battle_Tournament_UI;
	public MS_TournamentUIManager TournamentUI => tournamentUIManager;
	public void Init()
	{
		for (int i = 0; i < showPosOffset.Length; i++)
		{
			showPosOffset[i] = 85f;
		}
		for (int j = 0; j < charaUIList.Length; j++)
		{
			charaUIList[j].gameObject.SetActive(value: false);
		}
		playerNum = SwordFight_Define.CHAMBARA_CHARACTER_MAX;
		isTeamMode = SwordFight_Define.IS_TEAM_MODE;
		object_Battle_Tournament_UI.SetActive(value: true);
		tournamentUIManager.Init();
	}
	private void SetIcon()
	{
		int num = 0;
		for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; i++)
		{
			playerIcon[i].SetAlpha(1f);
		}
		for (int j = 0; j < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; j++)
		{
			UnityEngine.Debug.Log("アイコン：" + SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[j].PlayerNo.ToString());
			switch (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[j].PlayerNo)
			{
			case 0:
			case 1:
			case 2:
			case 3:
				playerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_" + (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[j].PlayerNo + 1).ToString() + "P");
				break;
			case 4:
				playerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_common_b_cpu_a");
				break;
			case 5:
				playerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_common_b_cpu_b");
				break;
			case 6:
				playerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_common_b_cpu_c");
				break;
			}
			num++;
		}
		LeanTween.delayedCall(base.gameObject, 4f, (Action)delegate
		{
			LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
			{
				for (int k = 0; k < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; k++)
				{
					playerIcon[k].SetAlpha(_value);
				}
			});
		});
	}
	public void SetGauge(int _idx, SwordFight_CharacterScript _player)
	{
		playerGauge[_idx].Init(_player);
	}
	public void Reset()
	{
		for (int i = 0; i < charaUIList.Length; i++)
		{
			charaUIList[i].gameObject.SetActive(value: false);
		}
	}
	public void UpdateCharacterUI()
	{
		for (int i = 0; i < playerNum; i++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.IsPlayer(i))
			{
				showNameChara[i] = SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i);
				switch (i)
				{
				case 0:
					CalcManager.mCalcInt = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0];
					break;
				case 1:
					CalcManager.mCalcInt = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0];
					break;
				}
				charaUIList[i].gameObject.SetActive(!showNameChara[i].CheckCharaHide());
				if (charaUIList[i].gameObject.activeSelf)
				{
					CalcManager.mCalcVector3 = SingletonCustom<SwordFight_FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(showNameChara[i].GetPos());
					CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
					CalcManager.mCalcVector3.y -= showPosOffset[i];
					charaUIList[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, charaUIList[i].transform.position.z);
					arrayHpGauge[i].transform.SetLocalScaleX(showNameChara[i].HpScale);
				}
				SetDeffenceActiveIcon(i, SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i).PlayerNo, SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i).NowUseDeffenceCount);
			}
			else
			{
				showNameChara[i] = SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i);
				charaUIList[i].gameObject.SetActive(!showNameChara[i].CheckCharaHide());
				if (charaUIList[i].gameObject.activeSelf)
				{
					CalcManager.mCalcVector3 = SingletonCustom<SwordFight_FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(showNameChara[i].GetPos());
					CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
					CalcManager.mCalcVector3.y -= showPosOffset[i];
					charaUIList[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, charaUIList[i].transform.position.z);
					arrayHpGauge[i].transform.SetLocalScaleX(showNameChara[i].HpScale);
				}
				SetDeffenceActiveIcon(i, SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i).PlayerNo, SingletonCustom<SwordFight_MainCharacterManager>.Instance.GetControlChara(i).NowUseDeffenceCount);
			}
		}
		for (int j = 0; j < playerGauge.Length; j++)
		{
			playerGauge[j].UpdateMethod();
		}
	}
	public void SetDeffenceActiveIcon(int _index, int _playerNo, int _deffenceUseCount)
	{
		for (int i = 0; i < deffenceIconList[_index].deffenceIcon.Length; i++)
		{
			deffenceIconList[_index].deffenceIcon[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_empty");
		}
		for (int j = 0; j < SwordFight_Define.DEFFENCE_USE_COUNT - _deffenceUseCount; j++)
		{
			switch (_playerNo)
			{
			case 0:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_green");
				break;
			case 1:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_red");
				break;
			case 2:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_blue");
				break;
			case 3:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_yellow");
				break;
			case -1:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_purple_a");
				break;
			case -2:
				deffenceIconList[_index].deffenceIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "chambara_shield_purple_b");
				break;
			}
		}
	}
	public void SetTournamentTeamData()
	{
		tournamentUIManager.SetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTournamentVSTeamData()[0], MS_TournamentUIManager.RoundType.Round_1st);
		tournamentUIManager.SetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTournamentVSTeamData()[1], MS_TournamentUIManager.RoundType.Round_2nd);
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.CreatePlayer(_isInstantiate: true);
		SetIcon();
	}
	public void StartTournamentUIAnimation(bool _winnerTeamLeft, Action _endCallBack)
	{
		tournamentUIManager.StartLineAnimation(_winnerTeamLeft, _endCallBack);
	}
	private IEnumerator SetAlphaColor(SpriteRenderer _tk2dSprite, float _fadeTime, float _alpha = 1f, float _delayTime = 0f, bool _isFadeOut = false)
	{
		float time = 0f;
		Color color = Color.white;
		float startAlpha = 0f;
		float endAlpha = _alpha;
		if (_isFadeOut)
		{
			startAlpha = _alpha;
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
	public void NextSettingTornament()
	{
		SetIcon();
		tournamentUIManager.NextSetting();
	}
	public int[] GetTournamentFinalRoundTeamNoArray()
	{
		return tournamentUIManager.GetFinalRoundTeamNo();
	}
	public int[] GetTournamentLoserBattleTeamNoArray()
	{
		return tournamentUIManager.GetLoserBattleTeamNo();
	}
	public MS_TournamentUIManager.RoundType GetNowTournamentType()
	{
		return tournamentUIManager.CurrentRoundType;
	}
	public int[] GetTournamentWinnerTeamNoList()
	{
		return tournamentUIManager.GetWinnerTeamNo();
	}
	public string[] GetTournamentMatchTeamNoList()
	{
		return tournamentUIManager.GetMatchTeamNo();
	}
	public void Debug_SetTournamentWinnerTeamNoList()
	{
		tournamentUIManager.Debug_TournamentWinnerList();
	}
}
