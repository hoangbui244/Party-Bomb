using GamepadInput;
using UnityEngine;
public class WaterSlider_Character : MonoBehaviour
{
	public enum State
	{
		STANDBY,
		IN_PLAY,
		GOAL
	}
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody parentRb;
	[SerializeField]
	[Header("CPU")]
	private WaterSlider_Cpu cpu;
	[SerializeField]
	[Header("段ボ\u30fcルソリ管理クラス")]
	private WaterSlider_Sled sled;
	[SerializeField]
	[Header("カメラ管理クラス")]
	private WaterSlider_CameraMover cameraMover;
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("重心")]
	private GameObject CenterOfBalance;
	[SerializeField]
	[Header("プレイヤ\u30fcの浮き輪")]
	private GameObject charaFloat;
	private int currentPlayerNo;
	private bool isCpu;
	private bool isInit;
	private int goalRank = -1;
	private Vector3 initPos;
	private State currentState;
	private CharacterStyle.StyleData[] arrayStyleData = new CharacterStyle.StyleData[9];
	private float goalTime;
	private int arrayIdx;
	public float GoalTime => goalTime;
	public int PlayerNo => currentPlayerNo;
	public State CurrentState => currentState;
	public bool IsReverse => cpu.IsReverse;
	public bool IsCpu => isCpu;
	public WaterSlider_Cpu Cpu => cpu;
	public WaterSlider_Sled Sled => sled;
	public int GoalRank
	{
		get
		{
			return goalRank;
		}
		set
		{
			goalRank = value;
		}
	}
	public int CurrentRank
	{
		get;
		set;
	}
	public WaterSlider_CameraMover CameraMover => cameraMover;
	public void Init(int _playerNo)
	{
		if (!isInit)
		{
			isInit = true;
			for (int i = 0; i < arrayStyleData.Length; i++)
			{
				arrayStyleData[i] = new CharacterStyle.StyleData();
			}
		}
		currentPlayerNo = _playerNo;
		isCpu = (currentPlayerNo > 3);
		SetCurrentState(State.STANDBY);
		cpu.Init();
		base.gameObject.transform.parent.gameObject.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		sled.gameObject.SetActive(value: true);
		sled.Init(_playerNo);
		parentRb.velocity = Vector3.zero;
		parentRb.angularVelocity = Vector3.zero;
		goalTime = 0f;
		cameraMover.Sleep();
		if (!isCpu)
		{
			cameraMover.Wakeup();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			cameraMover.Wakeup();
		}
		goalRank = -1;
		CurrentRank = 0;
		style.SetStyle(arrayStyleData[currentPlayerNo].gender, arrayStyleData[currentPlayerNo].face, arrayStyleData[currentPlayerNo].hairColor, arrayStyleData[currentPlayerNo].shape, arrayStyleData[currentPlayerNo].texId);
		CheckMainCharacter();
		style.SetGameStyle(GS_Define.GameType.RECEIVE_PON, currentPlayerNo);
	}
	public void SetFaceDiff(StyleTextureManager.MainCharacterFaceType _type)
	{
		style.SetMainCharacterFaceDiff(currentPlayerNo, _type);
	}
	public void CheckMainCharacter()
	{
		style.SetMainCharacterStyle(currentPlayerNo);
	}
	public int GetCorvePower()
	{
		return cpu.GetCorvePower();
	}
	public void SetDisable()
	{
		base.gameObject.transform.parent.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		sled.gameObject.SetActive(value: false);
	}
	public void SetCameraRect(Rect _rect)
	{
		cameraMover.SetRect(_rect);
	}
	public void GameStart()
	{
		SetCurrentState(State.IN_PLAY);
		sled.SetCurrentState(WaterSlider_Sled.State.DRIVE);
	}
	public void OnGoal()
	{
		if (currentState == State.IN_PLAY)
		{
			goalTime = SingletonCustom<WaterSlider_GameManager>.Instance.OnSetTime(PlayerNo);
			SetCurrentState(State.GOAL);
			cpu.OnGoal();
		}
	}
	public void SkipGoal()
	{
		if (currentState != State.GOAL)
		{
			if (!isCpu)
			{
				goalTime = SingletonCustom<WaterSlider_GameManager>.Instance.OnSetTime(PlayerNo, -1f);
			}
			else
			{
				goalTime = SingletonCustom<WaterSlider_GameManager>.Instance.OnSetTime(PlayerNo, CalcManager.Length(SingletonCustom<WaterSlider_CourseManager>.Instance.GetGoalPoint().position, base.transform.position) * 1f);
			}
			SetCurrentState(State.GOAL);
			cpu.OnGoal();
		}
	}
	public void SetCurrentState(State _state)
	{
		currentState = _state;
	}
	public float GetInputHorizontal()
	{
		if (isCpu)
		{
			return cpu.GetInputHorizontal();
		}
		return GetMoveDir(currentPlayerNo).x;
	}
	public Vector3 GetMoveDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return (Vector2)new Vector3(num, num2);
	}
	public bool GetCameraAngleRightButton(int _playerNo)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(_playerNo, SatGamePad.Button.RightShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_playerNo, SatGamePad.Button.RightTrigger);
		}
		return true;
	}
	public bool GetCameraAngleLeftButton(int _playerNo)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(_playerNo, SatGamePad.Button.LeftShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_playerNo, SatGamePad.Button.LeftTrigger);
		}
		return true;
	}
	public bool GetCameraRight()
	{
		return GetCameraAngleRightButton(currentPlayerNo);
	}
	public bool GetCameraLeft()
	{
		return GetCameraAngleLeftButton(currentPlayerNo);
	}
	public void SetArrayIdx(int _idx)
	{
		arrayIdx = _idx;
	}
	public int GetArrayIdx()
	{
		return arrayIdx;
	}
	public void SetLayer(int layer)
	{
		SetLayerRecursively(base.gameObject, layer);
		SetLayerRecursively(charaFloat, layer);
	}
	public void SetLayerRecursively(GameObject self, int layer)
	{
		self.layer = layer;
		foreach (Transform item in self.transform)
		{
			SetLayerRecursively(item.gameObject, layer);
		}
	}
}
