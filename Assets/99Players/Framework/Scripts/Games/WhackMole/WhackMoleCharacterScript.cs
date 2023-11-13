using System;
using UnityEngine;
public class WhackMoleCharacterScript : MonoBehaviour
{
	private const float BORDER_HALF = 22.5f;
	private const float BORDER_LONG_DIFF = 15f;
	private const float FIRST_REPEAT_TIME = 0.46667f;
	private const float NORMAL_REPEAT_TIME = 0.11667f;
	[SerializeField]
	private WhackMoleHammer hammer;
	[SerializeField]
	private Transform[] hammerPosAnchors;
	[SerializeField]
	private GameObject cursorAnchorObj;
	[SerializeField]
	private GameObject[] cursorObjs;
	private bool isPlayer;
	private int playerNo;
	private int charaNo;
	private int teamNo;
	private int holeNo;
	private bool isCtrlDown;
	private float ctrlTime;
	private int ctrlDirNo = -1;
	private int ctrlUpDownNum;
	private int ctrlRightLeftNum;
	public bool IsPlayer => isPlayer;
	public int PlayerNo => playerNo;
	public int CharaNo => charaNo;
	public int TeamNo => teamNo;
	public int HoleNo => holeNo;
	public bool IsWhackAnim => hammer.IsAnimation;
	public void Init(int _charaNo)
	{
		charaNo = _charaNo;
		playerNo = SingletonCustom<WhackMoleGameManager>.Instance.GetCharaPlayerNo(charaNo);
		teamNo = SingletonCustom<WhackMoleGameManager>.Instance.GetCharaTeamNo(charaNo);
		isPlayer = (playerNo < 4);
		if (isPlayer)
		{
			Texture cursorTexture = SingletonCustom<WhackMoleCharacterManager>.Instance.GetCursorTexture(playerNo);
			Material material = cursorObjs[0].GetComponent<MeshRenderer>().material;
			material.SetTexture("_MainTex", cursorTexture);
			for (int i = 1; i < cursorObjs.Length; i++)
			{
				cursorObjs[i].GetComponent<MeshRenderer>().sharedMaterial = material;
			}
		}
		DataInit();
	}
	public void SecondGroupInit()
	{
		playerNo = SingletonCustom<WhackMoleGameManager>.Instance.GetCharaPlayerNo(charaNo);
		teamNo = SingletonCustom<WhackMoleGameManager>.Instance.GetCharaTeamNo(charaNo);
		isPlayer = (playerNo < 4);
		if (isPlayer)
		{
			Texture cursorTexture = SingletonCustom<WhackMoleCharacterManager>.Instance.GetCursorTexture(playerNo);
			Material material = cursorObjs[0].GetComponent<MeshRenderer>().material;
			material.SetTexture("_MainTex", cursorTexture);
			for (int i = 1; i < cursorObjs.Length; i++)
			{
				cursorObjs[i].GetComponent<MeshRenderer>().sharedMaterial = material;
			}
		}
		DataInit();
	}
	private void DataInit()
	{
		SetHoleNo(4);
		hammer.Init();
		hammer.SetPos(hammerPosAnchors[4].position, _immediate: true);
		cursorAnchorObj.SetActive(isPlayer);
	}
	public void UpdateMethod()
	{
		hammer.UpdateMethod();
	}
	public void WhackAnimation(Action _callback = null)
	{
		hammer.AnimationStart(_callback);
	}
	public void SetHoleNo(int _holeNo)
	{
		if (holeNo != _holeNo)
		{
			hammer.SetPos(hammerPosAnchors[_holeNo].position, _immediate: false);
			cursorObjs[holeNo].SetActive(value: false);
			cursorObjs[_holeNo].SetActive(value: true);
			holeNo = _holeNo;
		}
	}
	public void CursorControl(Vector2 _stickDir)
	{
		SetHoleNo(CalcAdjacentNo_Hole_9(_stickDir));
	}
	private int CalcAdjacentNo_Hole_9(Vector2 _stickDir)
	{
		if (_stickDir.sqrMagnitude < 0.01f)
		{
			ctrlUpDownNum = 0;
			ctrlRightLeftNum = 0;
			return holeNo;
		}
		float num = Mathf.Atan2(_stickDir.y, _stickDir.x) * 57.29578f;
		float num2 = Mathf.Abs(num);
		int num3 = 0;
		int num4 = 0;
		if (40f < num2 && num2 < 140f)
		{
			num3 = ((!(num > 0f)) ? (num3 - 1) : (num3 + 1));
		}
		if (num2 < 50f)
		{
			num4++;
		}
		else if (num2 > 130f)
		{
			num4--;
		}
		int result = holeNo;
		if (ctrlUpDownNum != num3)
		{
			if (num3 > 0)
			{
				int adjacentDirHoleNo = WhackMoleGameManager.GetAdjacentDirHoleNo(result, 1);
				if (adjacentDirHoleNo != -1)
				{
					result = adjacentDirHoleNo;
				}
			}
			else if (num3 < 0)
			{
				int adjacentDirHoleNo2 = WhackMoleGameManager.GetAdjacentDirHoleNo(result, 7);
				if (adjacentDirHoleNo2 != -1)
				{
					result = adjacentDirHoleNo2;
				}
			}
			ctrlUpDownNum = num3;
		}
		if (ctrlRightLeftNum != num4)
		{
			if (num4 > 0)
			{
				int adjacentDirHoleNo3 = WhackMoleGameManager.GetAdjacentDirHoleNo(result, 5);
				if (adjacentDirHoleNo3 != -1)
				{
					result = adjacentDirHoleNo3;
				}
			}
			else if (num4 < 0)
			{
				int adjacentDirHoleNo4 = WhackMoleGameManager.GetAdjacentDirHoleNo(result, 3);
				if (adjacentDirHoleNo4 != -1)
				{
					result = adjacentDirHoleNo4;
				}
			}
			ctrlRightLeftNum = num4;
		}
		return result;
	}
	private int CalcAdjacentNo_Old(Vector2 _stickDir)
	{
		if (_stickDir.sqrMagnitude < 0.01f)
		{
			isCtrlDown = false;
			ctrlTime = 0f;
			ctrlDirNo = -1;
			return holeNo;
		}
		int num = -1;
		int num2 = -1;
		float num3 = Mathf.Atan2(_stickDir.y, _stickDir.x) * 57.29578f;
		num3 += 22.5f;
		if (num3 > 165f)
		{
			num = 3;
			num2 = ((num3 - 180f + 7.5f > 22.5f) ? 6 : 0);
		}
		else if (num3 > 135f)
		{
			num = 0;
			num2 = ((!(num3 - 135f - 7.5f > 22.5f)) ? 1 : 3);
		}
		else if (num3 > 75f)
		{
			num = 1;
			num2 = ((!(num3 - 90f + 7.5f > 22.5f)) ? 2 : 0);
		}
		else if (num3 > 45f)
		{
			num = 2;
			num2 = ((num3 - 45f - 7.5f > 22.5f) ? 1 : 5);
		}
		else if (num3 > -15f)
		{
			num = 5;
			num2 = ((num3 + 7.5f > 22.5f) ? 2 : 8);
		}
		else if (num3 > -45f)
		{
			num = 8;
			num2 = ((num3 + 45f - 7.5f > 22.5f) ? 5 : 7);
		}
		else if (num3 > -105f)
		{
			num = 7;
			num2 = ((num3 + 90f + 7.5f > 22.5f) ? 8 : 6);
		}
		else
		{
			num = 6;
			num2 = ((num3 + 135f - 7.5f > 22.5f) ? 7 : 3);
		}
		int num4 = -1;
		if (num != ctrlDirNo)
		{
			num4 = WhackMoleGameManager.GetAdjacentDirHoleNo(holeNo, num);
			if (num4 != -1)
			{
				ctrlDirNo = num;
			}
			if (num4 == -1 && num2 != ctrlDirNo)
			{
				num4 = WhackMoleGameManager.GetAdjacentDirHoleNo(holeNo, num2);
				if (num4 != -1)
				{
					ctrlDirNo = num2;
				}
			}
		}
		if (num4 == -1)
		{
			return holeNo;
		}
		return num4;
	}
	private int CalcSelectNo_Hole_9(Vector2 _stickDir)
	{
		if (_stickDir.sqrMagnitude < 0.01f)
		{
			return 4;
		}
		float num = Mathf.Atan2(_stickDir.y, _stickDir.x) * 57.29578f;
		float num2 = Mathf.Abs(num);
		int num3 = 0;
		int num4 = 0;
		if (22.5f < num2 && num2 < 157.5f)
		{
			num3 = ((!(num > 0f)) ? (num3 - 1) : (num3 + 1));
		}
		if (num2 < 67.5f)
		{
			num4++;
		}
		else if (num2 > 112.5f)
		{
			num4--;
		}
		return 4 + num4 + num3 * -3;
	}
	private int CalcSelectNo_Hole_5(Vector2 _stickDir)
	{
		if (_stickDir.sqrMagnitude < 0.01f)
		{
			return 4;
		}
		float num = Mathf.Atan2(_stickDir.y, _stickDir.x) * 57.29578f;
		float num2 = Mathf.Abs(num);
		int num3 = 0;
		int num4 = 0;
		num3 = ((!(num > 0f)) ? (num3 - 1) : (num3 + 1));
		num4 = ((!(num2 < 90f)) ? (num4 - 1) : (num4 + 1));
		return 4 + num4 + num3 * -3;
	}
}
