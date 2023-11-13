﻿using GamepadInput;
using System.Collections.Generic;
using UnityEngine;
public class GS_CharacterSelectLayoutFour : GS_CharacterSelectLayoutBase
{
	private enum State
	{
		SELECT_WAIT,
		ENTER_WAIT
	}
	[SerializeField]
	[Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
	private CursorManager[] arrayCursorManager;
	[SerializeField]
	[Header("キャラ名テキスト画像")]
	private SpriteRenderer[] spCharaName;
	[SerializeField]
	[Header("キャラ画像_1p")]
	private GameObject[] spCharacter1p;
	[SerializeField]
	[Header("キャラ画像_2p")]
	private GameObject[] spCharacter2p;
	[SerializeField]
	[Header("キャラ画像_3p")]
	private GameObject[] spCharacter3p;
	[SerializeField]
	[Header("キャラ画像_4p")]
	private GameObject[] spCharacter4p;
	[SerializeField]
	[Header("プレビュ\u30fcキャラ")]
	private CharacterStyle[] arrayPreviewChara;
	[SerializeField]
	[Header("キャラアニメ")]
	private GS_CharacterAnim[] charaAnim;
	[SerializeField]
	[Header("キャラエフェクト")]
	private ParticleSystem[] arrayCharaEffect;
	[SerializeField]
	[Header("表示座標_1p")]
	private Vector3[] arrayThumbPos1p;
	[SerializeField]
	[Header("表示座標_2p")]
	private Vector3[] arrayThumbPos2p;
	[SerializeField]
	[Header("表示座標_3p")]
	private Vector3[] arrayThumbPos3p;
	[SerializeField]
	[Header("表示座標_4p")]
	private Vector3[] arrayThumbPos4p;
	[SerializeField]
	[Header("コントロ\u30fcラ\u30fc表示フレ\u30fcム")]
	private GameObject objControllerFrame;
	private List<int> listSelectIdxAlloc = new List<int>();
	private int[] arraySelectIdx;
	private State currentState;
	private float waitTime;
	public void Show()
	{
		waitTime = 0f;
		currentState = State.SELECT_WAIT;
		for (int i = 0; i < charaAnim.Length; i++)
		{
			charaAnim[i].SetAnim(GS_CharacterAnim.AnimType.WALK);
		}
		UnityEngine.Debug.Log("★選択状態:" + SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0].ToString());
		base.gameObject.SetActive(value: true);
		for (int j = 0; j < arrayCursorManager.Length; j++)
		{
			arrayCursorManager[j].TargetPadId = SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(j);
			arrayCursorManager[j].SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[j]);
			arrayCursorManager[j].SetSelectNo(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[j]);
			arrayCursorManager[j].IsStop = false;
		}
		for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx.Length; k++)
		{
			SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[k] = k;
		}
		arrayThumbPos1p = new Vector3[spCharacter1p.Length];
		for (int l = 0; l < arrayThumbPos1p.Length; l++)
		{
			arrayThumbPos1p[l] = spCharacter1p[l].transform.localPosition;
		}
		arrayThumbPos2p = new Vector3[spCharacter2p.Length];
		for (int m = 0; m < arrayThumbPos2p.Length; m++)
		{
			arrayThumbPos2p[m] = spCharacter2p[m].transform.localPosition;
		}
		arrayThumbPos3p = new Vector3[spCharacter3p.Length];
		for (int n = 0; n < arrayThumbPos3p.Length; n++)
		{
			arrayThumbPos3p[n] = spCharacter3p[n].transform.localPosition;
		}
		arrayThumbPos4p = new Vector3[spCharacter4p.Length];
		for (int num = 0; num < arrayThumbPos4p.Length; num++)
		{
			arrayThumbPos4p[num] = spCharacter4p[num].transform.localPosition;
		}
		if (arraySelectIdx == null)
		{
			arraySelectIdx = new int[arrayCursorManager.Length];
		}
		for (int num2 = 0; num2 < arraySelectIdx.Length; num2++)
		{
			arraySelectIdx[num2] = -1;
		}
		for (int num3 = 0; num3 < arrayCursorManager.Length; num3++)
		{
			switch (num3)
			{
			case 0:
				for (int num7 = 0; num7 < spCharacter1p.Length; num7++)
				{
					spCharacter1p[num7].SetActive(num7 == arrayCursorManager[num3].GetSelectNo());
				}
				break;
			case 1:
				for (int num5 = 0; num5 < spCharacter2p.Length; num5++)
				{
					spCharacter2p[num5].SetActive(num5 == arrayCursorManager[num3].GetSelectNo());
				}
				break;
			case 2:
				for (int num6 = 0; num6 < spCharacter3p.Length; num6++)
				{
					spCharacter3p[num6].SetActive(num6 == arrayCursorManager[num3].GetSelectNo());
				}
				break;
			case 3:
				for (int num4 = 0; num4 < spCharacter4p.Length; num4++)
				{
					spCharacter4p[num4].SetActive(num4 == arrayCursorManager[num3].GetSelectNo());
				}
				break;
			}
			UpdateNamePlate(num3);
		}
		for (int num8 = 0; num8 < arrayCursorManager.Length; num8++)
		{
			arrayPreviewChara[num8].SetGameStyle(GS_Define.GameType.BOMB_ROULETTE, arrayCursorManager[num8].GetSelectNo());
		}
		LeanTween.cancel(objControllerFrame);
		objControllerFrame.transform.SetLocalPositionX(1145f);
		LeanTween.moveLocalX(objControllerFrame, 679f, 0.55f).setEaseOutQuint();
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	private void UpdateNamePlate(int _updateIdx)
	{
		string text = "_character_select_name_0" + arrayCursorManager[_updateIdx].GetSelectNo().ToString();
		if (Localize_Define.Language != 0)
		{
			text = "en" + text;
		}
		spCharaName[_updateIdx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, text);
		switch (_updateIdx)
		{
		case 0:
			for (int l = 0; l < spCharacter1p.Length; l++)
			{
				spCharacter1p[l].transform.localPosition = arrayThumbPos1p[l];
				spCharacter1p[l].SetActive(l == arrayCursorManager[_updateIdx].GetSelectNo());
			}
			break;
		case 1:
			for (int j = 0; j < spCharacter2p.Length; j++)
			{
				spCharacter2p[j].transform.localPosition = arrayThumbPos2p[j];
				spCharacter2p[j].SetActive(j == arrayCursorManager[_updateIdx].GetSelectNo());
			}
			break;
		case 2:
			for (int k = 0; k < spCharacter3p.Length; k++)
			{
				spCharacter3p[k].transform.localPosition = arrayThumbPos3p[k];
				spCharacter3p[k].SetActive(k == arrayCursorManager[_updateIdx].GetSelectNo());
			}
			break;
		case 3:
			for (int i = 0; i < spCharacter4p.Length; i++)
			{
				spCharacter4p[i].transform.localPosition = arrayThumbPos4p[i];
				spCharacter4p[i].SetActive(i == arrayCursorManager[_updateIdx].GetSelectNo());
			}
			break;
		}
		arrayPreviewChara[_updateIdx].GameStyleModelReset();
		arrayPreviewChara[_updateIdx].SetGameStyle(GS_Define.GameType.BOMB_ROULETTE, arrayCursorManager[_updateIdx].GetSelectNo());
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<SceneManager>.Instance.IsLoading || SingletonCustom<DM>.Instance.IsActive())
		{
			return;
		}
		switch (currentState)
		{
		case State.SELECT_WAIT:
		{
			for (int i = 0; i < arrayCursorManager.Length; i++)
			{
				if (arrayCursorManager[i].IsPushMovedButtonMoment())
				{
					UpdateNamePlate(i);
				}
				if (!arrayCursorManager[i].IsOkButton())
				{
					continue;
				}
				CalcManager.mCalcBool = false;
				for (int j = 0; j < arraySelectIdx.Length; j++)
				{
					if (arraySelectIdx[j] == arrayCursorManager[i].GetSelectNo())
					{
						CalcManager.mCalcBool = true;
					}
				}
				if (CalcManager.mCalcBool)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
					continue;
				}
				charaAnim[i].SetAnim(GS_CharacterAnim.AnimType.SELECT);
				arrayCharaEffect[i].Emit(100);
				arrayCursorManager[i].IsStop = true;
				arraySelectIdx[i] = arrayCursorManager[i].GetSelectNo();
				bool flag = true;
				for (int k = 0; k < arraySelectIdx.Length; k++)
				{
					if (arraySelectIdx[k] == -1)
					{
						flag = false;
					}
				}
				if (flag)
				{
					currentState = State.ENTER_WAIT;
					SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
					for (int l = 0; l < arrayCursorManager.Length; l++)
					{
						SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[l] = arrayCursorManager[l].GetSelectNo();
					}
					listSelectIdxAlloc.Clear();
					for (int m = 0; m < 8; m++)
					{
						listSelectIdxAlloc.Add(m);
					}
					for (int n = 0; n < arrayCursorManager.Length; n++)
					{
						listSelectIdxAlloc.Remove(arrayCursorManager[n].GetSelectNo());
					}
					listSelectIdxAlloc.Shuffle();
					for (int num = 0; num < listSelectIdxAlloc.Count; num++)
					{
						SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[arrayCursorManager.Length + num] = listSelectIdxAlloc[num];
					}
					SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[8] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[3];
					SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[9] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[2];
					SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[10] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[1];
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
				}
			}
			int num2 = 0;
			while (true)
			{
				if (num2 >= arrayCursorManager.Length)
				{
					return;
				}
				if (IsCancelButton(num2))
				{
					if (arraySelectIdx[num2] != -1)
					{
						break;
					}
					if (num2 == 0)
					{
						SingletonCustom<GS_GameSelectManager>.Instance.BackModeSelect();
					}
				}
				num2++;
			}
			arrayCursorManager[num2].IsStop = false;
			charaAnim[num2].SetAnim(GS_CharacterAnim.AnimType.WALK);
			arraySelectIdx[num2] = -1;
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
			break;
		}
		case State.ENTER_WAIT:
			waitTime += Time.deltaTime;
			if (waitTime >= 1.05f)
			{
				switch (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType)
				{
				case GameSettingManager.GameProgressType.SINGLE:
					SingletonCustom<SceneManager>.Instance.FadeExec(delegate
					{
						SingletonCustom<GS_GameSelectManager>.Instance.OnCharacterSelect();
					});
					break;
				case GameSettingManager.GameProgressType.ALL_SPORTS:
					StartPlayKing();
					break;
				}
			}
			break;
		}
	}
	private void SetAlloc()
	{
		listSelectIdxAlloc.Clear();
		for (int i = 0; i < 8; i++)
		{
			listSelectIdxAlloc.Add(i);
		}
		for (int j = 0; j < arrayCursorManager.Length; j++)
		{
			listSelectIdxAlloc.Remove(arrayCursorManager[j].GetSelectNo());
		}
		for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1; k++)
		{
			SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[1 + k] = listSelectIdxAlloc[k];
		}
	}
	public bool IsCancelButton(int _playerNo)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _playerNo : 0;
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B))
		{
			return true;
		}
		return false;
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objControllerFrame);
	}
}
