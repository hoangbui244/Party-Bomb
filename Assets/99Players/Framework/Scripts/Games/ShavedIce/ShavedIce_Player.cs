using System;
using UnityEngine;
public class ShavedIce_Player : MonoBehaviour
{
	public enum MoveType
	{
		Left,
		Right
	}
	[SerializeField]
	[Header("AI")]
	private ShavedIce_AI characterAI;
	[SerializeField]
	[Header("かき氷機の処理")]
	private ShavedIce_IceMachine iceMachine;
	[SerializeField]
	[Header("カップの処理")]
	private ShavedIce_Cup cup;
	[SerializeField]
	[Header("キャスト用アンカ\u30fc")]
	private Transform castAnchor;
	[SerializeField]
	[Header("メインカメラ")]
	private Camera mainCamera;
	[SerializeField]
	[Header("サブカメラ")]
	private Camera subCamera;
	[SerializeField]
	[Header("メインカメラの視野内に収める処理")]
	private SimpleCamera_FitInFOV fitInFOVCam;
	[SerializeField]
	[Header("高さを計測するポイント")]
	private Transform heightCalcPoint;
	[SerializeField]
	[Header("氷の管理")]
	private ShavedIce_IceObjectManager iceObjectManager;
	private ShavedIce_Define.UserType userType;
	private int dataNo;
	private bool isPlayer;
	private float currentHeight;
	private float defaultUserAnchorHeight;
	private RaycastHit hit;
	private Vector3 boxCastSize = new Vector3(1f, 1f, 1f);
	private bool isHit;
	private bool isTowerHeightCalcEnd;
	private float towerTopYPos;
	private bool duringTowerHeightCalc;
	private const int CONVERT_METER_SCALE = 100;
	private float defCameraZoom;
	private float cameraZoomInLimit;
	private float cameraZoomOutLimit;
	private const float CAMERA_ZOOM_SPEED = 0.25f;
	private const float CAMERA_AUTO_MOVE_START_HEIGHT = 40f;
	private float[] CAMERA_DEFAULT_ZOOM = new float[4]
	{
		0.2f,
		0f,
		-0.4f,
		-0.5f
	};
	private float[] CAMERA_ZOOM_IN_LIMIT = new float[4]
	{
		0.3f,
		0.3f,
		0.3f,
		0.3f
	};
	private float[] CAMERA_ZOOM_OUT_LIMIT = new float[4]
	{
		-100f,
		-100f,
		-100f,
		-100f
	};
	private float[] SUB_CAMERA_YPOS = new float[4]
	{
		0.27f,
		0.27f,
		0.27f,
		0.27f
	};
	private float[] SUB_CAMERA_CAM_SIZE = new float[4]
	{
		0.3f,
		0.3f,
		0.3f,
		0.3f
	};
	private const float CPU_CAMERA_DEFAULT_ZOOM = -1.2f;
	private const float CPU_SUB_CAMERA_YPOS = 0.285f;
	private const float CPU_SUB_CAMERA_CAM_SIZE = 0.3f;
	private float limitCameraHeight;
	private const float CAMERA_MOVE_SPEED = 0.75f;
	private const float AUTO_MOVE_CAMERA_SPEED = 0.75f;
	private const float DEF_PLAYER_CAMERA_YPOS = 1f;
	private const float DEF_CPU_CAMERA_YPOS = 1.5f;
	private float cameraDefaultYPos;
	private bool isAutoMoveStart;
	private bool isAutoCameraMove = true;
	private float autoMoveCameraHeight;
	private float currentCameraResetIntervalTime;
	private const float CAMERA_RESET_INTERVAL_TIME = 0.5f;
	private const float CAMERA_UP_MOVE_LIMIT_OFFSET = 0.1f;
	private const float AUTO_CAMERA_MOVE_LIMIT_OFFSET = -0.2f;
	private float moveCupLimit = 0.45f;
	private const float DEF_MOVE_CUP_LIMIT = 0.45f;
	private const float MOVE_CUP_SPEED = 1.25f;
	private const float HEIGHT_CALC_LINE_UP_SPEED = 1f;
	public ShavedIce_AI AI => characterAI;
	public ShavedIce_IceMachine IM => iceMachine;
	public ShavedIce_IceObjectManager IOM => iceObjectManager;
	public ShavedIce_Define.UserType UserType => userType;
	public bool IsTowerHeightCalcEnd => isTowerHeightCalcEnd;
	public float GET_MOVE_CUP_SPEED => 1.25f;
	private void Awake()
	{
		isAutoCameraMove = true;
	}
	public void Init(int _dataNo, ShavedIce_Define.UserType _userType, bool _isPlayer)
	{
		dataNo = _dataNo;
		userType = _userType;
		isPlayer = _isPlayer;
		defaultUserAnchorHeight = iceMachine.GetCrashIceFXRootAnchor().localPosition.y;
		iceMachine.Init();
		cup.Init(this);
		iceObjectManager.Init(this);
		fitInFOVCam.IsStopFitInFOW = true;
		cameraDefaultYPos = 1f;
		defCameraZoom = CAMERA_DEFAULT_ZOOM[3];
		cameraZoomInLimit = CAMERA_ZOOM_IN_LIMIT[3];
		cameraZoomOutLimit = CAMERA_ZOOM_OUT_LIMIT[3];
		subCamera.transform.SetLocalPositionY(SUB_CAMERA_YPOS[3]);
		subCamera.orthographicSize = SUB_CAMERA_CAM_SIZE[3];
		characterAI.Init();
		autoMoveCameraHeight = currentHeight + cameraDefaultYPos;
		mainCamera.transform.SetLocalPositionY(autoMoveCameraHeight);
		subCamera.backgroundColor = ShavedIce_Define.GetUserColor(_userType);
		mainCamera.transform.SetLocalPositionZ(defCameraZoom);
	}
	public void UpdateMethod()
	{
		iceObjectManager.UpdateMethod();
		if (isAutoCameraMove && isAutoMoveStart)
		{
			AutoMoveCamera();
		}
		if (isPlayer)
		{
			if (ShavedIce_Define.CM.IsStickTiltDirection((int)userType, ShavedIce_ControllerManager.StickType.L, ShavedIce_ControllerManager.StickDirType.RIGHT))
			{
				MoveCup(MoveType.Right);
			}
			else if (ShavedIce_Define.CM.IsStickTiltDirection((int)userType, ShavedIce_ControllerManager.StickType.L, ShavedIce_ControllerManager.StickDirType.LEFT))
			{
				MoveCup(MoveType.Left);
			}
			if (!ShavedIce_Define.CM.IsStickTilt((int)userType, ShavedIce_ControllerManager.StickType.L))
			{
				if (ShavedIce_Define.CM.IsPushCrossKey((int)userType, ShavedIce_ControllerManager.CrossKeyType.RIGHT, ShavedIce_ControllerManager.ButtonPushType.DOWN) || ShavedIce_Define.CM.IsPushCrossKey((int)userType, ShavedIce_ControllerManager.CrossKeyType.RIGHT, ShavedIce_ControllerManager.ButtonPushType.HOLD))
				{
					MoveCup(MoveType.Right);
				}
				else if (ShavedIce_Define.CM.IsPushCrossKey((int)userType, ShavedIce_ControllerManager.CrossKeyType.LEFT, ShavedIce_ControllerManager.ButtonPushType.DOWN) || ShavedIce_Define.CM.IsPushCrossKey((int)userType, ShavedIce_ControllerManager.CrossKeyType.LEFT, ShavedIce_ControllerManager.ButtonPushType.HOLD))
				{
					MoveCup(MoveType.Left);
				}
			}
			if (!IsCameraControleActive())
			{
			}
		}
		else
		{
			characterAI.UpdateMethod();
		}
		if (GetTowerHeightCovertCentiMeter() > 40f)
		{
			isAutoMoveStart = true;
		}
		AddjustCameraHeight();
		moveCupLimit = ((iceObjectManager.GetIceTowerWidth() > 0.45f) ? iceObjectManager.GetIceTowerWidth() : 0.45f);
	}
	private void MoveCup(MoveType _moveType)
	{
		switch (_moveType)
		{
		case MoveType.Left:
			if (cup.transform.localPosition.x <= 0f - moveCupLimit)
			{
				cup.transform.SetLocalPositionX(0f - moveCupLimit);
			}
			else
			{
				cup.transform.SetLocalPositionX(cup.transform.localPosition.x - Time.deltaTime * 1.25f);
			}
			break;
		case MoveType.Right:
			if (cup.transform.localPosition.x >= moveCupLimit)
			{
				cup.transform.SetLocalPositionX(moveCupLimit);
			}
			else
			{
				cup.transform.SetLocalPositionX(cup.transform.localPosition.x + Time.deltaTime * 1.25f);
			}
			break;
		}
	}
	private void CameraZoom(bool _isZoomIn)
	{
		float z = mainCamera.transform.localPosition.z;
		z = ((!_isZoomIn) ? (z - Time.deltaTime * 0.25f) : (z + Time.deltaTime * 0.25f));
		z = Mathf.Clamp(z, cameraZoomOutLimit, cameraZoomInLimit);
		mainCamera.transform.SetLocalPositionZ(z);
	}
	private void CameraReset()
	{
		mainCamera.transform.SetLocalPositionZ(defCameraZoom);
		mainCamera.transform.SetLocalPositionY(autoMoveCameraHeight);
		isAutoCameraMove = true;
		currentCameraResetIntervalTime = 0.5f;
	}
	private void MoveCamera(bool _isUpMove)
	{
		float y = mainCamera.transform.localPosition.y;
		y = ((!_isUpMove) ? (y - Time.deltaTime * 0.75f) : (y + Time.deltaTime * 0.75f));
		y = Mathf.Clamp(y, cameraDefaultYPos, limitCameraHeight);
		mainCamera.transform.SetLocalPositionY(y);
		isAutoCameraMove = false;
	}
	private void AddjustCameraHeight()
	{
		currentHeight = GetTowerHeight();
		iceMachine.SetCrashIceFXRootAnchorPosY(defaultUserAnchorHeight + currentHeight);
		if (autoMoveCameraHeight < currentHeight + cameraDefaultYPos + -0.2f)
		{
			autoMoveCameraHeight = currentHeight + cameraDefaultYPos + -0.2f;
		}
		limitCameraHeight = currentHeight + cameraDefaultYPos + 0.1f;
	}
	private void AutoMoveCamera()
	{
		mainCamera.transform.SetLocalPositionY(Mathf.MoveTowards(mainCamera.transform.localPosition.y, autoMoveCameraHeight, 0.75f * Time.deltaTime));
	}
	public bool CheckObj(GameObject _obj)
	{
		return base.gameObject == _obj;
	}
	private bool IsCameraControleActive()
	{
		if (currentCameraResetIntervalTime < 0f)
		{
			return true;
		}
		currentCameraResetIntervalTime -= Time.deltaTime;
		return false;
	}
	public float GetTowerHeight()
	{
		isHit = Physics.BoxCast(castAnchor.position, boxCastSize, Vector3.down, out hit, castAnchor.rotation, float.PositiveInfinity, LayerMask.GetMask(ShavedIce_Define.LAYER_FIELD));
		if (isHit)
		{
			heightCalcPoint.SetPositionY(hit.point.y);
			return heightCalcPoint.localPosition.y;
		}
		return 0f;
	}
	public float GetTowerHeightCovertCentiMeter()
	{
		return CalcManager.ConvertDecimalFirst(GetTowerHeight() * 100f);
	}
	public Camera GetMainCamera()
	{
		return mainCamera;
	}
	public ShavedIce_Cup GetCup()
	{
		return cup;
	}
	public void SetCameraRect_Main(Rect _rect)
	{
		mainCamera.rect = _rect;
	}
	public void SetCameraRect_Sub(Rect _rect)
	{
		subCamera.rect = _rect;
	}
	public void GameFinishProcess()
	{
		iceMachine.StopIceMachineAnimation();
	}
	public void HideCamera()
	{
		mainCamera.gameObject.SetActive(value: false);
		subCamera.gameObject.SetActive(value: false);
	}
	public void InitHeightCalc(bool _isGroup1)
	{
		mainCamera.transform.SetLocalPositionY(cameraDefaultYPos);
		mainCamera.transform.SetLocalPositionZ(defCameraZoom);
		cup.transform.SetLocalPositionX(0f);
		iceObjectManager.GameEndProcess();
		towerTopYPos = GetTowerHeight();
		heightCalcPoint.SetLocalPositionY(0f);
		iceObjectManager.SetShavedIcePourSyrup();
		fitInFOVCam.IsStopFitInFOW = false;
		fitInFOVCam.MoveCameraSpeed = 99f;
		LeanTween.delayedCall(0.1f, (Action)delegate
		{
			ShavedIce_Define.UIM.SetTowerHeightCalcLinePos(dataNo, heightCalcPoint.position, _isGroup1);
		});
	}
	public void StartHeightCalc()
	{
		duringTowerHeightCalc = true;
		fitInFOVCam.MoveCameraSpeed = 1f;
	}
	public void UpdateHeightCalcLine(bool _isGroup1)
	{
		if (duringTowerHeightCalc)
		{
			heightCalcPoint.AddLocalPositionY(Time.deltaTime * 1f);
			if (heightCalcPoint.localPosition.y >= towerTopYPos)
			{
				heightCalcPoint.SetLocalPositionY(towerTopYPos);
				duringTowerHeightCalc = false;
				isTowerHeightCalcEnd = true;
				ShavedIce_Define.UIM.ShowTowerHeight(dataNo);
			}
			float y = heightCalcPoint.localPosition.y;
			ShavedIce_Define.PM.SetHeightCalc(dataNo, CalcManager.ConvertDecimalFirst(heightCalcPoint.localPosition.y * 100f));
			ShavedIce_Define.UIM.SetTowerHeightCalcLinePos(dataNo, heightCalcPoint.position, _isGroup1);
			autoMoveCameraHeight = y + cameraDefaultYPos;
			if (ShavedIce_Define.PM.GetTowerHeight(dataNo, _isGroup1) > 40f)
			{
				AutoMoveCamera();
			}
		}
	}
}
