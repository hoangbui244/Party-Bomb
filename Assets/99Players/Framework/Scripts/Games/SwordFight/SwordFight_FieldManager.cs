using System;
using UnityEngine;
public class SwordFight_FieldManager : SingletonCustom<SwordFight_FieldManager>
{
	public enum DirType
	{
		LEFT,
		RIGHT,
		FRONT,
		BACK
	}
	[Serializable]
	public struct AnchorList
	{
		public Transform[] anchor;
	}
	[Serializable]
	public struct CameradData
	{
		public Camera camera;
		public Transform rotationAnchor;
		public Transform root;
		public Vector3 movePos;
		public Vector3 defPos;
		public Vector3 offsetPos;
		public Vector3 viewSize;
		public float fieldOfViewHalf;
	}
	[Serializable]
	public struct FieldData
	{
		[Header("フィ\u30fcルドアンカ\u30fc")]
		public Transform fieldAnchor;
		[SerializeField]
		[Header("センタ\u30fcアンカ\u30fc")]
		private Transform centerAnchor;
		private Vector3 areaSize;
		public Transform CenterAnchor => centerAnchor;
		public Vector3 AreaSize => areaSize;
		public void SetAreaSize(float _x, float _z)
		{
			areaSize.x = _x;
			areaSize.z = _z;
		}
	}
	private float CAMERA_MOVE_SPEED = 3f;
	private float CAMERA_MOVE_SPEED_SET_PLAY = 2f;
	private float CAMERA_MOVE_SPEED_LIMIT_IN = 0.05f;
	[SerializeField]
	[Header("オブジェクトアンカ\u30fc")]
	private Transform objAnchor;
	private Vector3 calcVec3;
	[SerializeField]
	[Header("初期配置アンカ\u30fc")]
	private AnchorList originAnchorList;
	private Vector3 gameShowSize = new Vector3(18f, 0f, 32f);
	private float[] CAMERA_DEF_Y = new float[16]
	{
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f,
		37f
	};
	private float[] CAMERA_APPROACH_LENGTH = new float[16]
	{
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f,
		14f
	};
	private float[] CAMERA_ZOOM_OUT_OFFSET_Z = new float[16]
	{
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f,
		0.38f
	};
	private Vector3[] CAMERA_OFFSET = new Vector3[2]
	{
		new Vector3(0f, 0f, 0.58f),
		new Vector3(0f, 0f, 1f)
	};
	private Vector3[] CAMERA_ROT = new Vector3[2]
	{
		new Vector3(70f, 0f, 0f),
		new Vector3(45f, 0f, 0f)
	};
	[SerializeField]
	[Header("カメラデ\u30fcタ")]
	private CameradData cameradData;
	private int[] formationNo = new int[2];
	[SerializeField]
	[Header("フィ\u30fcルド情報")]
	private FieldData fieldData;
	[SerializeField]
	[Header("机")]
	private BoxCollider desk;
	[SerializeField]
	[Header("ライト")]
	private Light fieldLight;
	[SerializeField]
	[Header("フィ\u30fcルド")]
	private MeshRenderer field;
	[SerializeField]
	[Header("フィ\u30fcルドオブジェクト")]
	private SwordFight_StageData fieldObj;
	private float cameraChangeInterval;
	private bool isReverseField;
	public static int[] DebugNo = new int[2]
	{
		-1,
		-1
	};
	private Vector3 cameraOffset;
	public Transform[] OriginAnchorList => originAnchorList.anchor;
	public int[] FormationNo => formationNo;
	public void Init()
	{
	}
	private void SetVerticalCamera()
	{
		cameradData.camera.transform.SetLocalEulerAngles(CAMERA_ROT[0].x, CAMERA_ROT[0].y, CAMERA_ROT[0].z);
		cameradData.root.transform.SetLocalPosition(0f, CAMERA_DEF_Y[0], -1.5f);
		fieldLight.transform.SetLocalEulerAnglesY(0f);
	}
	private void SetHorizontalCamera()
	{
		CAMERA_OFFSET[0].z = 0f;
		CAMERA_OFFSET[0].x = -0.9f;
		CAMERA_OFFSET[1].z = 0f;
		CAMERA_OFFSET[1].x = -0.8f;
		for (int i = 0; i < CAMERA_ROT.Length; i++)
		{
			CAMERA_ROT[i].x = 35f;
			CAMERA_ROT[i].y = 270f;
			CAMERA_ROT[i].z = 0f;
		}
		cameradData.camera.transform.SetLocalEulerAngles(CAMERA_ROT[0].x, CAMERA_ROT[0].y, CAMERA_ROT[0].z);
		CAMERA_DEF_Y[0] -= 19.77f;
		cameradData.root.transform.SetLocalPosition(0f, CAMERA_DEF_Y[0], 0f);
		CAMERA_APPROACH_LENGTH[0] -= 14.5f;
		fieldLight.transform.SetLocalEulerAnglesY(-90f);
	}
	public void UpdateMethod()
	{
	}
	private void MoveCamera(Vector3 _targetPos, float _moveSpeed, int _offsetNo, bool _changeY, bool _lerp = true)
	{
		CalcManager.mCalcVector3 = cameradData.camera.transform.localEulerAngles;
		CalcManager.mCalcVector3.x = CAMERA_ROT[_offsetNo].x;
		if (_lerp)
		{
			cameradData.camera.transform.localEulerAngles = Vector3.Lerp(cameradData.camera.transform.localEulerAngles, CalcManager.mCalcVector3, _moveSpeed * Time.deltaTime);
		}
		else
		{
			cameradData.camera.transform.localEulerAngles = CalcManager.mCalcVector3;
		}
		cameradData.viewSize = CalcManager.CalcCameraViewSize(cameradData.camera.fieldOfView, cameradData.camera.aspect, GetCameraHeight());
		CalcManager.mCalcVector3 = _targetPos;
		if (_lerp)
		{
			cameraOffset.x = Mathf.Lerp(cameraOffset.x, CAMERA_OFFSET[_offsetNo].x, CAMERA_MOVE_SPEED * Time.deltaTime);
			cameraOffset.z = Mathf.Lerp(cameraOffset.z, CAMERA_OFFSET[_offsetNo].z, CAMERA_MOVE_SPEED * Time.deltaTime);
		}
		else
		{
			cameraOffset.x = CAMERA_OFFSET[_offsetNo].x;
			cameraOffset.z = CAMERA_OFFSET[_offsetNo].z;
		}
		if (_changeY)
		{
			if (SingletonCustom<SwordFight_MainGameManager>.Instance.CheckGameState(SwordFight_MainGameManager.GameState.DURING_GAME))
			{
				CalcManager.mCalcVector3.y = 0f - CAMERA_APPROACH_LENGTH[0] - 1.5f;
			}
			else
			{
				CalcManager.mCalcVector3.y = 0f - CAMERA_APPROACH_LENGTH[0];
			}
		}
		CalcManager.mCalcVector3.x -= cameradData.viewSize.x * cameraOffset.x;
		CalcManager.mCalcVector3.z -= cameradData.viewSize.z * cameraOffset.z;
		if (_lerp)
		{
			cameradData.camera.transform.localPosition = Vector3.Lerp(cameradData.camera.transform.localPosition, CalcManager.mCalcVector3, _moveSpeed * Time.deltaTime);
		}
		else
		{
			cameradData.camera.transform.localPosition = CalcManager.mCalcVector3;
		}
	}
	private bool CheckCameraMoveLimitArea(float _offset)
	{
		return CheckCameraMoveLimitArea(cameradData.camera.transform.localPosition, _offset);
	}
	private bool CheckCameraMoveLimitArea(Vector3 _localPos, float _offset)
	{
		cameradData.offsetPos = _localPos;
		if (cameradData.viewSize.y < gameShowSize.z)
		{
			if (cameradData.offsetPos.z - cameradData.viewSize.y * 0.5f < (0f - gameShowSize.z) * 0.5f - cameradData.viewSize.y * _offset)
			{
				return true;
			}
			if (cameradData.offsetPos.z + cameradData.viewSize.y * 0.5f > gameShowSize.z * 0.5f - cameradData.viewSize.y * _offset)
			{
				return true;
			}
		}
		if (cameradData.viewSize.x < gameShowSize.x)
		{
			if (cameradData.offsetPos.x - cameradData.viewSize.x * 0.5f < (0f - gameShowSize.x) * 0.5f)
			{
				return true;
			}
			if (cameradData.offsetPos.x + cameradData.viewSize.x * 0.5f > gameShowSize.x * 0.5f)
			{
				return true;
			}
		}
		return false;
	}
	private Vector3 GetCameraMoveLimitAreaPos(float _offset)
	{
		return GetCameraMoveLimitAreaPos(cameradData.camera.transform.localPosition, _offset);
	}
	private Vector3 GetCameraMoveLimitAreaPos(Vector3 _localPos, float _offset)
	{
		cameradData.offsetPos = _localPos;
		if (cameradData.viewSize.y < gameShowSize.z)
		{
			if (cameradData.offsetPos.z - cameradData.viewSize.y * 0.5f < (0f - gameShowSize.z) * 0.5f - cameradData.viewSize.y * _offset)
			{
				cameradData.offsetPos.z = (0f - gameShowSize.z) * 0.5f + cameradData.viewSize.y * 0.5f - cameradData.viewSize.y * _offset;
			}
			else if (cameradData.offsetPos.z + cameradData.viewSize.y * 0.5f > gameShowSize.z * 0.5f - cameradData.viewSize.y * _offset)
			{
				cameradData.offsetPos.z = gameShowSize.z * 0.5f - cameradData.viewSize.y * 0.5f - cameradData.viewSize.y * _offset;
			}
		}
		if (cameradData.viewSize.x < gameShowSize.x)
		{
			if (cameradData.offsetPos.x - cameradData.viewSize.x * 0.5f < (0f - gameShowSize.x) * 0.5f)
			{
				cameradData.offsetPos.x = (0f - gameShowSize.x) * 0.5f + cameradData.viewSize.x * 0.5f;
			}
			else if (cameradData.offsetPos.x + cameradData.viewSize.x * 0.5f > gameShowSize.x * 0.5f)
			{
				cameradData.offsetPos.x = gameShowSize.x * 0.5f - cameradData.viewSize.x * 0.5f;
			}
		}
		return cameradData.offsetPos;
	}
	public DirType CheckLineOutDir(Vector3 _pos, ref Vector3 _lineOutPos)
	{
		return DirType.BACK;
	}
	public Transform GetFormationAnchor(int _no)
	{
		return originAnchorList.anchor[_no];
	}
	public Vector3 GetFormationPos(int _no)
	{
		return originAnchorList.anchor[_no].position;
	}
	public FieldData GetAnchors()
	{
		return fieldData;
	}
	public FieldData GetFieldData()
	{
		return fieldData;
	}
	public float GetCameraHeight()
	{
		return cameradData.camera.transform.position.y - objAnchor.transform.position.y;
	}
	public Camera Get3dCamera()
	{
		return cameradData.camera;
	}
	public Vector3 ConvertLocalPos(Vector3 _pos, int _no)
	{
		return originAnchorList.anchor[_no].InverseTransformPoint(_pos);
	}
	public Vector3 ConvertWorldPos(Vector3 _localPos, int _no)
	{
		return originAnchorList.anchor[_no].TransformPoint(_localPos);
	}
	public Vector3 ConvertLocalPosPer(Vector3 _pos, int _no)
	{
		Vector3 vector = originAnchorList.anchor[_no].InverseTransformPoint(_pos);
		Vector3 result = default(Vector3);
		result.x = vector.x / fieldData.AreaSize.x;
		result.z = vector.z / (fieldData.AreaSize.z * 2f);
		result.y = 0f;
		return result;
	}
	public Vector3 ConvertPosPerToWorld(Vector3 _posPer, int _no)
	{
		_posPer.x *= fieldData.AreaSize.x;
		_posPer.z *= fieldData.AreaSize.z * 2f;
		_posPer.y = 0f;
		return originAnchorList.anchor[_no].TransformPoint(_posPer);
	}
}
