using GamepadInput;
using System;
using UnityEngine;
namespace BeachSoccer
{
	public class ControllerManager : SingletonCustom<ControllerManager>
	{
		[Serializable]
		public struct StickObj
		{
			public Transform back;
			public Transform stick;
			public Transform button;
		}
		[Serializable]
		public struct TouchData
		{
			public float touchTime;
			public float touchTimeBk;
			public float touchTimeResetTime;
			public bool isTouch;
		}
		[Serializable]
		public struct ControllerData
		{
			public TouchData touchData;
			public int isMove;
			public bool isTap;
			public bool isChangeChara_L;
			public bool isChangeChara_R;
			public ControllerData(int _touchCnt)
			{
				touchData = default(TouchData);
				isMove = -1;
				isTap = false;
				isChangeChara_L = false;
				isChangeChara_R = false;
			}
			public void ResetTouchData(bool _bkReset)
			{
				touchData.isTouch = false;
				touchData.touchTime = 0f;
				if (_bkReset)
				{
					touchData.touchTimeBk = 0f;
				}
				isTap = false;
			}
			public bool IsTap()
			{
				return isTap;
			}
			public void ResetTapData()
			{
				if (IsTap())
				{
					ResetTouchData(_bkReset: true);
				}
			}
			public bool IsChangeChara_L()
			{
				return isChangeChara_L;
			}
			public bool IsChangeChara_R()
			{
				return isChangeChara_R;
			}
			public void ResetChangeChara_L()
			{
				isChangeChara_L = false;
			}
			public void ResetChangeChara_R()
			{
				isChangeChara_R = false;
			}
		}
		private enum ButtonStateType
		{
			DOWN,
			HOLD,
			UP
		}
		[SerializeField]
		[Header("タッチエリア")]
		private GameObject touchArea;
		[SerializeField]
		[Header("スティック")]
		private StickObj[] stickObj;
		private static readonly float STICK_MOVE_MAX = 85f;
		private static readonly float FINGER_MOVE_MAX = 85f;
		private string PREF_IS_SHOW_VIRTUAL_STICK = "PREF_IS_SHOW_VIRTUAL_STICK";
		private bool isShowVirtualStick = true;
		private ControllerData[] controllerData = new ControllerData[4]
		{
			new ControllerData(1),
			new ControllerData(1),
			new ControllerData(1),
			new ControllerData(1)
		};
		private int playerNum = 1;
		private int nowTouchCnt;
		private int playerNoTemp;
		private Vector3 touchPosTemp;
		public bool IsShowVirtualStick
		{
			get
			{
				return isShowVirtualStick;
			}
			set
			{
				PlayerPrefs.SetInt(PREF_IS_SHOW_VIRTUAL_STICK, value ? 1 : 0);
				isShowVirtualStick = value;
			}
		}
		public void Init()
		{
			isShowVirtualStick = (PlayerPrefs.GetInt(PREF_IS_SHOW_VIRTUAL_STICK, 1) == 1);
		}
		public void UpdateMethod()
		{
			for (int i = 0; i < controllerData.Length; i++)
			{
				if (controllerData[i].touchData.isTouch)
				{
					if (IsKickButton(i, ButtonStateType.DOWN) || IsKickButton(i, ButtonStateType.HOLD) || IsKickButton(i, ButtonStateType.UP))
					{
						controllerData[i].touchData.touchTime += Time.deltaTime;
						controllerData[i].touchData.touchTimeBk += Time.deltaTime;
					}
					else
					{
						controllerData[i].touchData.isTouch = false;
					}
				}
				else
				{
					controllerData[i].ResetTouchData(controllerData[i].touchData.touchTimeResetTime >= 0.5f);
					controllerData[i].touchData.touchTimeResetTime += Time.deltaTime;
				}
			}
			int num = 0;
			while (true)
			{
				if (num >= GameSaveData.GetSelectMultiPlayerNum())
				{
					return;
				}
				if (IsKickCancelButton(num, ButtonStateType.DOWN))
				{
					controllerData[num].ResetTouchData(_bkReset: true);
				}
				else if (IsKickButton(num, ButtonStateType.DOWN))
				{
					touchPosTemp = Vector3.zero;
					playerNoTemp = num;
					if (!controllerData[playerNoTemp].touchData.isTouch)
					{
						break;
					}
				}
				if (IsKickButton(num, ButtonStateType.UP) && controllerData[num].touchData.isTouch)
				{
					controllerData[num].isTap = true;
					controllerData[num].touchData.touchTimeResetTime = 0f;
					controllerData[num].touchData.isTouch = false;
				}
				controllerData[num].isChangeChara_L = false;
				if (IsCharaChangeButton(num, ButtonStateType.DOWN))
				{
					controllerData[num].isChangeChara_L = true;
				}
				controllerData[num].isChangeChara_R = false;
				if (IsCharaChangeButton(num, ButtonStateType.DOWN))
				{
					controllerData[num].isChangeChara_R = true;
				}
				num++;
			}
			controllerData[playerNoTemp].touchData.isTouch = true;
			controllerData[playerNoTemp].touchData.touchTime = 0f;
			controllerData[playerNoTemp].touchData.touchTimeBk = 0f;
		}
		public ControllerData GetTouchData(int _no)
		{
			return controllerData[_no];
		}
		public bool IsMove(int _no)
		{
			return GetMoveLength(_no) > 0.01f;
		}
		public bool IsTap(int _no)
		{
			return controllerData[_no].IsTap();
		}
		public void ResetTapData(int _no)
		{
			controllerData[_no].ResetTapData();
		}
		public void ResetTouchData(int _no, bool _bkReset)
		{
			controllerData[_no].ResetTouchData(_bkReset);
		}
		public bool IsChangeChara_L(int _no)
		{
			return controllerData[_no].IsChangeChara_L();
		}
		public bool IsChangeChara_R(int _no)
		{
			return controllerData[_no].IsChangeChara_R();
		}
		public bool IsChangeChara(int _no)
		{
			if (!controllerData[_no].IsChangeChara_R())
			{
				return controllerData[_no].IsChangeChara_L();
			}
			return true;
		}
		public void ResetChangeChara(int _no)
		{
			controllerData[_no].ResetChangeChara_L();
			controllerData[_no].ResetChangeChara_R();
		}
		private bool IsKickButton(int _no, ButtonStateType _type)
		{
			int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
			if ((!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A) || _type != 0) && (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A) || _type != ButtonStateType.HOLD) && (!SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A) || _type != ButtonStateType.UP))
			{
				if ((!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y) || _type != 0) && (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y) || _type != ButtonStateType.HOLD))
				{
					if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y))
					{
						return _type == ButtonStateType.UP;
					}
					return false;
				}
				return true;
			}
			return true;
		}
		private bool IsKickCancelButton(int _no, ButtonStateType _type)
		{
			int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
			if ((!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B) || _type != 0) && (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B) || _type != ButtonStateType.HOLD))
			{
				if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B))
				{
					return _type == ButtonStateType.UP;
				}
				return false;
			}
			return true;
		}
		private bool IsCharaChangeButton(int _no, ButtonStateType _type)
		{
			int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
			if ((!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X) || _type != 0) && (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X) || _type != ButtonStateType.HOLD))
			{
				if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X))
				{
					return _type == ButtonStateType.UP;
				}
				return false;
			}
			return true;
		}
		public Vector3 GetMoveDir(int _no)
		{
			float num = 0f;
			float num2 = 0f;
			Vector3 mVector3Zero = CalcManager.mVector3Zero;
			int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
			JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
			num = axisInput.Stick_L.x;
			num2 = axisInput.Stick_L.y;
			mVector3Zero = new Vector3(num, 0f, num2);
			if (!GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
			{
				mVector3Zero = CalcManager.PosRotation2D(mVector3Zero, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
			}
			return mVector3Zero;
		}
		public float GetMoveLength(int _no)
		{
			return GetMoveDir(_no).magnitude;
		}
		public float GetTapTime(int _no)
		{
			return controllerData[_no].touchData.touchTime;
		}
	}
}
