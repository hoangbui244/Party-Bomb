using GamepadInput;
using System.Collections.Generic;
using UnityEngine;
public class GS_OniAssignment : MonoBehaviour
{
	[SerializeField]
	[Header("【ひとりで】あなたアイコン")]
	private GameObject youIcon;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private GameObject[] teamPlayerIcon;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private SpriteRenderer backFrame;
	private readonly float[] POS_YOU_ICON_X = new float[2]
	{
		-240f,
		240f
	};
	private readonly float[] POS_TEAM_ICON_X = new float[8]
	{
		-420f,
		-303f,
		-186f,
		-68f,
		68f,
		186f,
		303f,
		420f
	};
	private List<int>[] playerGroupList;
	private readonly int TEAM_PLAYER_NUM = 4;
	public void Set(GS_Define.GameType _type)
	{
		base.gameObject.SetActive(value: true);
		StartTeamAssignment();
		SetBackFrame(_type);
	}
	private void StartTeamAssignment()
	{
		playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			youIcon.SetActive(value: true);
			for (int i = 0; i < teamPlayerIcon.Length; i++)
			{
				teamPlayerIcon[i].SetActive(value: false);
			}
			return;
		}
		youIcon.SetActive(value: false);
		for (int j = 0; j < teamPlayerIcon.Length; j++)
		{
			teamPlayerIcon[j].SetActive(j < SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		}
		for (int k = 0; k < playerGroupList.Length; k++)
		{
			for (int l = 0; l < playerGroupList[k].Count; l++)
			{
				teamPlayerIcon[playerGroupList[k][l]].transform.SetLocalPositionX(POS_TEAM_ICON_X[k * TEAM_PLAYER_NUM + playerGroupList[k][l]]);
			}
		}
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
	public bool IsAssignment()
	{
		if (CheckPlayerGroupSetting())
		{
			return false;
		}
		for (int i = 0; i < playerGroupList.Length; i++)
		{
			playerGroupList[i].Clear();
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			playerGroupList[(youIcon.transform.localPosition.x > 0f) ? 1 : 0].Add(0);
		}
		else
		{
			for (int j = 0; j < teamPlayerIcon.Length; j++)
			{
				if (teamPlayerIcon[j].activeSelf)
				{
					playerGroupList[(teamPlayerIcon[j].transform.localPosition.x > 0f) ? 1 : 0].Add(j);
				}
			}
		}
		SingletonCustom<GameSettingManager>.Instance.PlayerGroupList = playerGroupList;
		return true;
	}
	private bool CheckPlayerGroupSetting()
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && (GetLeftTeamNum() <= 0 || GetRightTeamNum() <= 0))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
			SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 60), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate
			{
			}), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
			return true;
		}
		return false;
	}
	private int GetRightTeamNum()
	{
		CalcManager.mCalcInt = 0;
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x > 0f)
			{
				CalcManager.mCalcInt++;
			}
		}
		return CalcManager.mCalcInt;
	}
	private int GetLeftTeamNum()
	{
		CalcManager.mCalcInt = 0;
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x < 0f)
			{
				CalcManager.mCalcInt++;
			}
		}
		return CalcManager.mCalcInt;
	}
	private void SetBackFrame(GS_Define.GameType _type)
	{
		switch (_type)
		{
		case GS_Define.GameType.GET_BALL:
		case GS_Define.GameType.CANNON_SHOT:
		case GS_Define.GameType.BLOCK_WIPER:
		case GS_Define.GameType.MOLE_HAMMER:
		case GS_Define.GameType.BOMB_ROULETTE:
		case GS_Define.GameType.RECEIVE_PON:
			return;
		}
	}
	private void Update()
	{
		if (SingletonCustom<DM>.Instance.IsActive())
		{
			return;
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Left))
			{
				youIcon.transform.SetLocalPositionX(POS_YOU_ICON_X[0]);
				SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Right))
			{
				youIcon.transform.SetLocalPositionX(POS_YOU_ICON_X[1]);
				SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
			}
			return;
		}
		for (int i = 0; i < teamPlayerIcon.Length; i++)
		{
			if (teamPlayerIcon[i].activeSelf)
			{
				if (teamPlayerIcon[i].transform.localPosition.x > 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Left))
				{
					teamPlayerIcon[i].transform.SetLocalPositionX(POS_TEAM_ICON_X[i]);
					SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
				}
				else if (teamPlayerIcon[i].transform.localPosition.x < 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Right))
				{
					teamPlayerIcon[i].transform.SetLocalPositionX(POS_TEAM_ICON_X[TEAM_PLAYER_NUM + i]);
					SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
				}
			}
		}
	}
}
