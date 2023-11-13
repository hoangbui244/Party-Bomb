using GamepadInput;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class BeachVolley_ControllerManager : SingletonCustom<BeachVolley_ControllerManager>
{
	[Serializable]
	public struct StickObj
	{
		public Transform back;
		public Transform stick;
		public Transform button;
		public TextMeshPro btnText;
	}
	[Serializable]
	public struct TouchData
	{
		public int fingerNo;
		public Vector3 startPos;
		public Vector3 nowPos;
		public Vector3 prevPos;
		public float touchTime;
		public float touchTimeBk;
		public float touchTimeResetTime;
		public bool isTouch;
		public float isTouchNow;
		public bool isTouchLeft;
		public Vector3 temp;
		public Vector3 GetDir()
		{
			temp = (nowPos - startPos).normalized;
			temp.z = temp.y;
			temp.y = 0f;
			return temp;
		}
		public float GetLength()
		{
			return Mathf.Min(FINGER_MOVE_MAX, (nowPos - startPos).magnitude);
		}
	}
	[Serializable]
	public struct ControllerData
	{
		public TouchData[] touchData;
		public int isMove;
		public int isTap;
		public bool isAttack;
		public int isTouchRightStart;
		public List<float> touchDownTimeList;
		public ControllerData(int _touchCnt)
		{
			touchData = new TouchData[_touchCnt];
			isMove = -1;
			isAttack = false;
			isTap = -1;
			isTouchRightStart = -1;
			touchDownTimeList = new List<float>();
			touchDownTimeList.Add(-1f);
		}
		public void ResetTouchData(int _no, bool _bkReset)
		{
			touchData[_no].isTouch = false;
			touchData[_no].isTouchNow = 0f;
			touchData[_no].touchTime = 0f;
			if (_bkReset)
			{
				touchData[_no].touchTimeBk = 0f;
			}
			touchData[_no].fingerNo = -1;
			if (isMove == _no)
			{
				isMove = -1;
			}
			if (isTap == _no)
			{
				isTap = -1;
			}
			if (isTouchRightStart == _no)
			{
				isTouchRightStart = -1;
			}
		}
		public bool IsTap()
		{
			return isTap != -1;
		}
		public void ResetTapData()
		{
			for (int i = 0; i < touchData.Length; i++)
			{
				if (isTap == i || !touchData[i].isTouchLeft)
				{
					ResetTouchData(i, _bkReset: true);
				}
			}
		}
		public void AddTouchDownTime()
		{
			touchDownTimeList.Add(Time.time);
			while (10 < touchDownTimeList.Count)
			{
				touchDownTimeList.RemoveAt(0);
			}
		}
	}
	private static readonly float STICK_MOVE_MAX = 85f;
	private static readonly float FINGER_MOVE_MAX = 85f;
	private string PREF_IS_SHOW_VIRTUAL_STICK = "PREF_IS_SHOW_VIRTUAL_STICK";
	private bool isShowVirtualStick = true;
	private ControllerData[] controllerData;
	private int playerNum = 1;
	private int nowTouchCnt;
	private int playerNoTemp;
	private Vector3 touchPosTemp;
	private Vector3[] playerMoveVec3 = new Vector3[5];
	public bool IsShowVirtualStick
	{
		get
		{
			return isShowVirtualStick;
		}
		set
		{
			PlayerPrefs.SetInt(PREF_IS_SHOW_VIRTUAL_STICK, value ? 1 : 0);
			PlayerPrefs.Save();
			isShowVirtualStick = value;
		}
	}
	public void Init()
	{
		isShowVirtualStick = (PlayerPrefs.GetInt(PREF_IS_SHOW_VIRTUAL_STICK, 1) == 1);
		controllerData = new ControllerData[BeachVolley_Define.PLAYER_NUM];
		for (int i = 0; i < controllerData.Length; i++)
		{
			controllerData[i] = new ControllerData(1);
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < controllerData.Length; i++)
		{
			for (int j = 0; j < controllerData[i].touchData.Length; j++)
			{
				if (controllerData[i].touchData[j].isTouchNow > 0f)
				{
					if (GetButton(i) || GetButtonUp(i))
					{
						controllerData[i].touchData[j].touchTime += Time.deltaTime * 6.5f;
						controllerData[i].touchData[j].touchTimeBk += Time.deltaTime * 6.5f;
						controllerData[i].touchData[j].prevPos = controllerData[i].touchData[j].nowPos;
						controllerData[i].touchData[j].isTouchNow -= Time.deltaTime;
						if (controllerData[i].touchData[j].isTouchNow < 0f)
						{
							controllerData[i].touchData[j].isTouchNow = 0f;
							if (controllerData[i].isMove != j)
							{
								controllerData[i].isTap = j;
							}
						}
					}
					else
					{
						controllerData[i].touchData[j].isTouchNow = 0f;
						if (controllerData[i].isMove != j)
						{
							controllerData[i].isTap = j;
						}
					}
				}
				else
				{
					controllerData[i].ResetTouchData(j, controllerData[i].touchData[j].touchTimeResetTime >= 0.5f);
					controllerData[i].touchData[j].touchTimeResetTime += Time.deltaTime;
				}
			}
		}
		for (int k = 0; k < controllerData.Length; k++)
		{
			if (GetButtonDown(k))
			{
				playerNoTemp = k;
				for (int l = 0; l < controllerData[playerNoTemp].touchData.Length; l++)
				{
					if (!controllerData[playerNoTemp].touchData[l].isTouch)
					{
						controllerData[playerNoTemp].touchData[l].isTouch = true;
						controllerData[playerNoTemp].touchData[l].isTouchNow = 0.15f;
						controllerData[playerNoTemp].touchData[l].fingerNo = k;
						controllerData[playerNoTemp].touchData[l].touchTime = 0f;
						controllerData[playerNoTemp].touchData[l].touchTimeBk = 0f;
						controllerData[playerNoTemp].touchData[l].startPos = (controllerData[playerNoTemp].touchData[l].prevPos = (controllerData[playerNoTemp].touchData[l].nowPos = touchPosTemp));
						controllerData[playerNoTemp].isTouchRightStart = l;
						controllerData[playerNoTemp].AddTouchDownTime();
						controllerData[playerNoTemp].touchData[l].touchTimeResetTime = 0f;
						controllerData[playerNoTemp].touchData[l].isTouch = false;
						controllerData[playerNoTemp].touchData[l].fingerNo = -1;
						if (controllerData[playerNoTemp].isMove == l)
						{
							controllerData[playerNoTemp].isMove = -1;
						}
						break;
					}
				}
			}
			else if (GetButtonUp(k))
			{
				for (int m = 0; m < controllerData[k].touchData.Length; m++)
				{
				}
			}
		}
		for (int n = 0; n < playerMoveVec3.Length; n++)
		{
			playerMoveVec3[n] *= 0.86f;
		}
	}
	public bool GetButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X) || SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y))
		{
			controllerData[_no].isAttack = true;
			return true;
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A) || SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B))
		{
			controllerData[_no].isAttack = false;
			return true;
		}
		return false;
	}
	public bool GetButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y))
		{
			controllerData[_no].isAttack = true;
			return true;
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B))
		{
			controllerData[_no].isAttack = false;
			return true;
		}
		return false;
	}
	public bool GetButtonUp(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X) || SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y))
		{
			controllerData[_no].isAttack = true;
			return true;
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A) || SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B))
		{
			controllerData[_no].isAttack = false;
			return true;
		}
		return false;
	}
	public bool IsMove(int _no)
	{
		return true;
	}
	public int IsTap(int _no)
	{
		if (controllerData[_no].IsTap())
		{
			if (controllerData[_no].isAttack)
			{
				return 1;
			}
			return 0;
		}
		return -1;
	}
	public void ResetTapData(int _no)
	{
		if (_no < controllerData.Length)
		{
			controllerData[_no].ResetTapData();
		}
	}
	public Vector3 GetMoveDir(int _no)
	{
		if (!BeachVolley_Define.MCM.ControlChara[_no].IsPlayer)
		{
			return Vector3.zero;
		}
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		mVector3Zero = new Vector3(num, 0f, num2);
		if (!BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			mVector3Zero = CalcManager.PosRotation2D(mVector3Zero, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
			if (BeachVolley_Define.MGM.GetChangeCort())
			{
				mVector3Zero = (mVector3Zero = CalcManager.PosRotation2D(mVector3Zero, CalcManager.mVector3Zero, 180f, CalcManager.AXIS.Y));
			}
		}
		playerMoveVec3[_no] += mVector3Zero;
		if (playerMoveVec3[_no].magnitude > 1f)
		{
			playerMoveVec3[_no].Normalize();
		}
		if (BeachVolley_Define.MGM.IsAutoAction())
		{
			return mVector3Zero;
		}
		return playerMoveVec3[_no];
	}
	public float GetMoveLength(int _no)
	{
		return 1f;
	}
	public float GetTapTime(int _no)
	{
		for (int i = 0; i < controllerData[_no].touchData.Length; i++)
		{
			if (i == controllerData[_no].isMove || controllerData[_no].touchData[i].isTouchLeft)
			{
				continue;
			}
			if (controllerData[_no].touchData[i].touchTime < 0.6f)
			{
				return 0f;
			}
			if (controllerData[_no].touchData[i].touchTime > BeachVolley_Define.MCM.GAUGE_MAX_NEED_TIME)
			{
				float num = BeachVolley_Define.MCM.GAUGE_MAX_NEED_TIME * 2f - controllerData[_no].touchData[i].touchTime;
				if (num < 0.8f)
				{
					return 0.8f;
				}
				return num;
			}
			return controllerData[_no].touchData[i].touchTime;
		}
		return 0f;
	}
}
