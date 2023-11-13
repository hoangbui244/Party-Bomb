using GamepadInput;
using System.Collections.Generic;
using UnityEngine;
public class GS_CharacterSelectLayoutOne : GS_CharacterSelectLayoutBase
{
	private enum State
	{
		CHARACTER_SELECT,
		CPU_SELECT,
		ENTER_WAIT,
		THROW
	}
	[SerializeField]
	[Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
	private CursorManager[] arrayCursorManager;
	[SerializeField]
	[Header("キャラ名テキスト画像")]
	private SpriteRenderer spCharaName;
	[SerializeField]
	[Header("キャラ画像")]
	private SpriteRenderer[] spCharacter;
	[SerializeField]
	[Header("キャラ影画像")]
	private SpriteRenderer[] spCharacterShadow;
	[SerializeField]
	[Header("プレビュ\u30fcキャラ")]
	private CharacterStyle previewChara;
	[SerializeField]
	[Header("プレビュ\u30fcエフェクト")]
	private ParticleSystem psCharaEffect;
	[SerializeField]
	[Header("キャラアニメ")]
	private GS_CharacterAnim charaAnim;
	[SerializeField]
	[Header("表示座標")]
	private Vector3[] arrayThumbPos;
	[SerializeField]
	[Header("CPU選択案内表示")]
	private GameObject objCpuSelect;
	[SerializeField]
	[Header("CPUアイコン配列")]
	private SpriteRenderer[] arrayCpuIcon;
	[SerializeField]
	[Header("CPU選択説明表示")]
	private GameObject objCpuSelectInfo;
	[SerializeField]
	[Header("キャラ選択表示のル\u30fcト")]
	private GameObject objRootBoard;
	[SerializeField]
	[Header("コントロ\u30fcラ\u30fc表示フレ\u30fcム")]
	private GameObject objControllerFrame;
	[SerializeField]
	[Header("選択カ\u30fcソル")]
	private SpriteRenderer selectCurosr;
	[SerializeField]
	[Header("選択カ\u30fcソル上")]
	private SpriteRenderer selectCursorTop;
	private List<int> listSelectIdxAlloc = new List<int>();
	private State currentState;
	private int selectIdx;
	private int[] arraySelectCpu = new int[3];
	private float waitTime;
	public void Show()
	{
		currentState = State.CHARACTER_SELECT;
		selectIdx = 0;
		charaAnim.SetAnim(GS_CharacterAnim.AnimType.WALK);
		base.gameObject.SetActive(value: true);
		for (int i = 0; i < arrayCursorManager.Length; i++)
		{
			arrayCursorManager[i].SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0]);
			arrayCursorManager[i].SetSelectNo(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0]);
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx.Length; j++)
		{
			SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[j] = j;
		}
		for (int k = 0; k < spCharacter.Length; k++)
		{
			spCharacter[k].gameObject.SetActive(k == arrayCursorManager[0].GetSelectNo());
		}
		selectIdx = 0;
		for (int l = 0; l < arraySelectCpu.Length; l++)
		{
			arraySelectCpu[l] = -1;
		}
		for (int m = 0; m < arrayCpuIcon.Length; m++)
		{
			arrayCpuIcon[m].gameObject.SetActive(value: false);
		}
		previewChara.SetGameStyle(GS_Define.GameType.BLOW_AWAY_TANK, arrayCursorManager[0].GetSelectNo());
		UpdateNamePlate();
		objCpuSelect.SetActive(value: true);
		objCpuSelectInfo.SetActive(value: false);
		waitTime = 0f;
		arrayCursorManager[0].IsStop = false;
		LeanTween.cancel(objControllerFrame);
		objControllerFrame.transform.SetLocalPositionX(1145f);
		LeanTween.moveLocalX(objControllerFrame, 618f, 0.55f).setEaseOutQuint();
		selectCurosr.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce");
		selectCursorTop.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce_light");
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	private void UpdateNamePlate()
	{
		string text = "_character_select_name_0" + arrayCursorManager[0].GetSelectNo().ToString();
		if (Localize_Define.Language != 0)
		{
			text = "en" + text;
		}
		spCharaName.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, text);
		for (int i = 0; i < spCharacter.Length; i++)
		{
			spCharacter[i].transform.localPosition = arrayThumbPos[i];
		}
		for (int j = 0; j < spCharacter.Length; j++)
		{
			spCharacter[j].gameObject.SetActive(j == arrayCursorManager[0].GetSelectNo());
			if (j == arrayCursorManager[0].GetSelectNo())
			{
				LeanTween.moveLocalX(spCharacter[j].gameObject, spCharacter[j].transform.localPosition.x, 0.25f).setEaseOutQuart();
				LeanTween.value(spCharacter[j].gameObject, 0f, 1f, 0.125f).setOnUpdate(delegate(float _value)
				{
					spCharacter[arrayCursorManager[0].GetSelectNo()].SetAlpha(_value);
					spCharacterShadow[arrayCursorManager[0].GetSelectNo()].SetAlpha(_value * 0.5f);
				});
				spCharacter[j].SetAlpha(0f);
				spCharacterShadow[j].SetAlpha(0f);
				spCharacter[j].transform.SetLocalPositionX(spCharacter[j].transform.localPosition.x + 300f);
			}
		}
		previewChara.GameStyleModelReset();
		previewChara.SetGameStyle(GS_Define.GameType.BLOW_AWAY_TANK, arrayCursorManager[0].GetSelectNo());
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<SceneManager>.Instance.IsLoading || SingletonCustom<DM>.Instance.IsActive())
		{
			return;
		}
		if (arrayCursorManager[0].IsPushMovedButtonMoment())
		{
			UpdateNamePlate();
		}
		switch (currentState)
		{
		case State.THROW:
			break;
		case State.CHARACTER_SELECT:
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				for (int j = 0; j < arraySelectCpu.Length; j++)
				{
					if (arraySelectCpu[j] == arrayCursorManager[0].GetSelectNo())
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
						return;
					}
				}
				SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
				SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0] = arrayCursorManager[0].GetSelectNo();
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1; k++)
				{
					SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[1 + k] = 1 + k;
				}
				listSelectIdxAlloc.Clear();
				for (int l = 0; l < 8; l++)
				{
					listSelectIdxAlloc.Add(l);
				}
				for (int m = 0; m < arrayCursorManager.Length; m++)
				{
					listSelectIdxAlloc.Remove(arrayCursorManager[m].GetSelectNo());
				}
				bool flag = false;
				if (arraySelectCpu[0] != -1)
				{
					flag = true;
					for (int n = 0; n < arraySelectCpu.Length; n++)
					{
						listSelectIdxAlloc.Remove(arraySelectCpu[n]);
					}
				}
				listSelectIdxAlloc.Shuffle();
				if (flag)
				{
					for (int num = 0; num < listSelectIdxAlloc.Count; num++)
					{
						int num2 = arrayCursorManager.Length + num;
						if (num2 > 3)
						{
							num2 += arraySelectCpu.Length;
						}
						SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num2] = listSelectIdxAlloc[num];
					}
					for (int num3 = 0; num3 < arraySelectCpu.Length; num3++)
					{
						SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4 + num3] = arraySelectCpu[num3];
					}
				}
				else
				{
					for (int num4 = 0; num4 < listSelectIdxAlloc.Count; num4++)
					{
						SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[arrayCursorManager.Length + num4] = listSelectIdxAlloc[num4];
					}
				}
				SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[8] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[3];
				SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[9] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[2];
				SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[10] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[1];
				waitTime = 0f;
				currentState = State.ENTER_WAIT;
				charaAnim.SetAnim(GS_CharacterAnim.AnimType.SELECT);
				arrayCursorManager[0].IsStop = true;
				psCharaEffect.Emit(100);
				SingletonCustom<GameSettingManager>.Instance.IsCpuFixSelect = flag;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B))
			{
				SingletonCustom<GS_GameSelectManager>.Instance.BackModeSelect();
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
			{
				currentState = State.CPU_SELECT;
				selectIdx = 0;
				for (int num5 = 0; num5 < arraySelectCpu.Length; num5++)
				{
					arraySelectCpu[num5] = -1;
				}
				for (int num6 = 0; num6 < arrayCpuIcon.Length; num6++)
				{
					arrayCpuIcon[num6].gameObject.SetActive(value: false);
				}
				objCpuSelect.SetActive(value: false);
				objCpuSelectInfo.SetActive(value: true);
				selectCurosr.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce_red");
				selectCursorTop.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce_light_red");
				SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
			}
			break;
		case State.CPU_SELECT:
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A))
			{
				for (int i = 0; i < arraySelectCpu.Length; i++)
				{
					if (arraySelectCpu[i] == arrayCursorManager[0].GetSelectNo())
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
						return;
					}
				}
				arrayCpuIcon[arrayCursorManager[0].GetSelectNo()].gameObject.SetActive(value: true);
				arrayCpuIcon[arrayCursorManager[0].GetSelectNo()].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "_select_cp" + (selectIdx + 1).ToString());
				arraySelectCpu[selectIdx] = arrayCursorManager[0].GetSelectNo();
				selectIdx++;
				if (selectIdx >= 3)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
					objCpuSelect.SetActive(value: true);
					objCpuSelectInfo.SetActive(value: false);
					selectCurosr.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce");
					selectCursorTop.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce_light");
					currentState = State.CHARACTER_SELECT;
				}
				else
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_character_select_decision");
				}
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
				if (selectIdx > 0)
				{
					selectIdx--;
					arrayCpuIcon[arraySelectCpu[selectIdx]].gameObject.SetActive(value: false);
					arraySelectCpu[selectIdx] = -1;
				}
				else
				{
					objCpuSelect.SetActive(value: true);
					objCpuSelectInfo.SetActive(value: false);
					selectCurosr.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce");
					selectCursorTop.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.CharacterSelect, "character_select_chioce_light");
					currentState = State.CHARACTER_SELECT;
				}
			}
			break;
		case State.ENTER_WAIT:
			waitTime += Time.deltaTime;
			if (waitTime >= 0.75f)
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
				currentState = State.THROW;
				waitTime = 0f;
			}
			break;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objRootBoard);
		LeanTween.cancel(objControllerFrame);
		for (int i = 0; i < spCharacter.Length; i++)
		{
			LeanTween.cancel(spCharacter[i].gameObject);
		}
	}
}
