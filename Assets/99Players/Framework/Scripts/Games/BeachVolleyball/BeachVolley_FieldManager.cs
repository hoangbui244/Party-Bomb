using System;
using System.Collections;
using UnityEngine;
public class BeachVolley_FieldManager : SingletonCustom<BeachVolley_FieldManager>
{
	public struct CameraWorkData
	{
		public Vector3 defLocalPos;
		public float multiOffsetY;
		public float[] offsetZ;
		public float[] rot;
		public float moveSpeed;
		public float moveSpeedWhenSetPlay;
		public Vector3 gameShowSize;
		public Vector3 gameShowSizeMulti;
		public float approachDistance;
		public float[] approachDistanceMulti;
		public CameraWorkData(Vector3 _defLocalPos, float _multiOffsetY, float[] _offsetZ, float[] _rot, float _moveSpeed, float _moveSpeedWhenSetPlay, Vector3 _gameShowSize, Vector3 _gameShowSizeMulti, float _approachDistance, float[] _approachDistanceMulti)
		{
			defLocalPos = _defLocalPos;
			multiOffsetY = _multiOffsetY;
			offsetZ = _offsetZ;
			rot = _rot;
			moveSpeed = _moveSpeed;
			moveSpeedWhenSetPlay = _moveSpeedWhenSetPlay;
			gameShowSize = _gameShowSize;
			gameShowSizeMulti = _gameShowSizeMulti;
			approachDistance = _approachDistance;
			approachDistanceMulti = _approachDistanceMulti;
		}
	}
	public enum DirType
	{
		LEFT,
		RIGHT,
		FRONT,
		BACK
	}
	[Serializable]
	public struct FieldData
	{
		[Header("ネット")]
		public GameObject net;
		[Header("フィ\u30fcルドアンカ\u30fc")]
		public Transform fieldAnchor;
		[Header("右手前")]
		public Transform frontLeft;
		[Header("左奥")]
		public Transform backRight;
		[Header("ネット上部")]
		public Transform netTopAnchor;
		[SerializeField]
		[Header("センタ\u30fcサ\u30fcクル")]
		private Transform centerCircle;
		[Header("アンテナアンカ\u30fc")]
		public Transform antennaAnchor;
		private Vector3 halfCourtSize;
		public Transform CenterCircle => centerCircle;
		public Vector3 HalfCourtSize => halfCourtSize;
		public void SetHalfCourtSize(float _x, float _z)
		{
			halfCourtSize.x = _x;
			halfCourtSize.z = _z;
		}
		public Vector3 GetCenterPos()
		{
			return centerCircle.transform.position;
		}
		public Vector3 GetNetTopPos()
		{
			return netTopAnchor.transform.position;
		}
	}
	[Serializable]
	public struct AnchorList
	{
		[Header("フォ\u30fcメ\u30fcション")]
		public Transform[] formationAnchor;
		[Header("ビ\u30fcチバレ\u30fcフォ\u30fcメ\u30fcション")]
		public Transform[] formationAnchorBeach;
		[Header("サ\u30fcブ")]
		public Transform serveAnchor;
		[Header("サ\u30fcブビ\u30fcチ")]
		public Transform serveAnchorBeach;
		[Header("ベンチ")]
		public Transform benchAnchor;
		[Header("チ\u30fcムアンカ\u30fc")]
		public Transform teamAnchor;
		[Header("アタックライン")]
		public Transform attackLineAnchor;
		[Header("入口アンカ\u30fc")]
		public Transform enterAnchor;
		[Header("出口アンカ\u30fc")]
		public Transform exitAnchor;
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
	private CameraWorkData cameraWorkData = new CameraWorkData(new Vector3(8f, 20f, -9.5f), 4f, new float[2]
	{
		0f,
		0.7f
	}, new float[2]
	{
		70f,
		46f
	}, 0.25f, 3f, new Vector3(5f, 0f, 30f), new Vector3(40f, 0f, 20f), 12.5f, new float[2]
	{
		7.5f,
		9f
	});
	[SerializeField]
	[Header("オブジェクトアンカ\u30fc")]
	private Transform objAnchor;
	[SerializeField]
	[Header("背景アンカ\u30fc")]
	private Transform backgroundAnchor;
	[SerializeField]
	[Header("ネットのアニメ\u30fcション")]
	private Animator netAnimation;
	[SerializeField]
	[Header("コ\u30fcトのライン壁")]
	private GameObject courtLineWall;
	[SerializeField]
	[Header("ネット壁")]
	private GameObject netWall;
	[SerializeField]
	[Header("フィ\u30fcルド情報")]
	private FieldData fieldData;
	[SerializeField]
	[Header("アンカ\u30fcリスト")]
	private AnchorList[] anchorList;
	[SerializeField]
	[Header("プレイヤ\u30fc守備時のカメラタ\u30fcゲット")]
	private Transform playerDefenseCameraTarget;
	private float cameraOffsetZ;
	[SerializeField]
	[Header("カメラデ\u30fcタ")]
	private CameradData cameradData;
	[SerializeField]
	[Header("カメラ二つ")]
	private Camera[] cameradDatas = new Camera[2];
	private int[] formationNo = new int[2];
	[SerializeField]
	[Header("教室")]
	private Transform classRoom;
	[SerializeField]
	[Header("机")]
	private BoxCollider desk;
	[SerializeField]
	[Header("ライト")]
	private Light fieldLight;
	[SerializeField]
	[Header("フィ\u30fcルド")]
	private MeshRenderer field;
	private Transform fieldTransform;
	[SerializeField]
	[Header("床")]
	private MeshRenderer floor;
	[SerializeField]
	[Header("フィ\u30fcルドオブジェクト")]
	private BeachVolley_StageData fieldObj;
	private float cameraChangeInterval;
	private bool isReverseField;
	private Vector3 startPos;
	[SerializeField]
	[Header("ビ\u30fcチバレ\u30fc用フィ\u30fcルド")]
	private MeshRenderer fieldBeach;
	[Header("右手前")]
	public Transform frontLeftBeach;
	[Header("左奥")]
	public Transform backRightBeach;
	[SerializeField]
	[Header("フォ\u30fcメ\u30fcションデ\u30fcタ")]
	private BeachVolley_FormationData formationData;
	private float ObjAnchorPositionZ;
	private bool changeCort;
	private float zPrev;
	private float angleXPrev;
	private float angleXPrev2;
	private float CAMERA_DEFENCE_OFFSET_Y = -12.5f;
	private float CAMERA_DEFENCE_OFFSET_Z = -16f;
	public GameObject NetWall => netWall;
	public int[] FormationNo => formationNo;
	public Transform GetCameraRoot()
	{
		return cameradData.root;
	}
	public Transform GetDeskTransform()
	{
		return desk.transform;
	}
	public Light GetFieldLight()
	{
		return fieldLight;
	}
	public Vector3 GetFieldPos()
	{
		return fieldTransform.position;
	}
	public void Init()
	{
		fieldTransform = field.transform;
		fieldData.frontLeft.gameObject.SetActive(value: false);
		fieldData.frontLeft = frontLeftBeach;
		fieldData.backRight.gameObject.SetActive(value: false);
		fieldData.backRight = backRightBeach;
		field.gameObject.SetActive(value: false);
		field = fieldBeach;
		anchorList[0].serveAnchor.transform.SetPositionZ(anchorList[0].serveAnchorBeach.transform.position.z);
		anchorList[1].serveAnchor.transform.SetPositionZ(anchorList[1].serveAnchorBeach.transform.position.z);
		anchorList[0].teamAnchor.transform.SetPositionZ(frontLeftBeach.transform.position.z);
		anchorList[1].teamAnchor.transform.SetPositionZ(backRightBeach.transform.position.z);
		InitCamera();
		InitStage();
		InitFormationAnchor();
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			fieldLight.transform.SetLocalEulerAngles(54.766f, 87f, 2.582f);
			cameradData.root.SetLocalPositionY(30f);
			cameradDatas[0].gameObject.SetActive(value: true);
			cameradData.camera = cameradDatas[0];
		}
		else
		{
			fieldLight.transform.SetLocalEulerAngles(54.766f, 272f, 2.582f);
			cameradData.root.SetLocalPositionY(26.4f);
			cameradDatas[1].gameObject.SetActive(value: true);
			cameradData.camera = cameradDatas[1];
		}
		ObjAnchorPositionZ = BeachVolley_Define.BM.GetBall().transform.position.z;
		startPos = cameradData.camera.transform.position;
		if (fieldObj.fixField != null)
		{
			field.enabled = false;
		}
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			if (fieldObj.arrayHiddenVerticalObj != null)
			{
				for (int i = 0; i < fieldObj.arrayHiddenVerticalObj.Length; i++)
				{
					fieldObj.arrayHiddenVerticalObj[i].SetActive(value: false);
				}
			}
			if (fieldObj.arrayHiddenVerticalObjFirstHalf != null)
			{
				for (int j = 0; j < fieldObj.arrayHiddenVerticalObjFirstHalf.Length; j++)
				{
					fieldObj.arrayHiddenVerticalObjFirstHalf[j].SetActive(value: false);
				}
			}
			if (fieldObj.arrayHiddenVerticalObjSecondHalf != null)
			{
				for (int k = 0; k < fieldObj.arrayHiddenVerticalObjSecondHalf.Length; k++)
				{
					fieldObj.arrayHiddenVerticalObjSecondHalf[k].SetActive(value: true);
				}
			}
		}
		else
		{
			if (fieldObj.arrayHiddenHorizontalObj != null)
			{
				for (int l = 0; l < fieldObj.arrayHiddenHorizontalObj.Length; l++)
				{
					fieldObj.arrayHiddenHorizontalObj[l].SetActive(value: false);
				}
			}
			if (fieldObj.arrayHiddenHorizontalFirstHalfObj != null)
			{
				for (int m = 0; m < fieldObj.arrayHiddenHorizontalFirstHalfObj.Length; m++)
				{
					fieldObj.arrayHiddenHorizontalFirstHalfObj[m].SetActive(value: false);
				}
			}
			if (fieldObj.arrayHiddenHorizontalSecondHalfObj != null)
			{
				for (int n = 0; n < fieldObj.arrayHiddenHorizontalSecondHalfObj.Length; n++)
				{
					fieldObj.arrayHiddenHorizontalSecondHalfObj[n].SetActive(value: true);
				}
			}
		}
		fieldLight.color = fieldObj.stageLightColor;
	}
	private void InitCamera()
	{
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			cameradData.camera.transform.SetLocalEulerAnglesX(cameraWorkData.rot[0]);
			cameradData.root.transform.SetLocalPosition(0f, cameraWorkData.defLocalPos.y, cameraWorkData.defLocalPos.z);
		}
		else
		{
			for (int i = 0; i < cameraWorkData.offsetZ.Length; i++)
			{
				cameraWorkData.offsetZ[i] = 0f;
			}
			cameraWorkData.gameShowSize = cameraWorkData.gameShowSizeMulti;
			cameradData.root.transform.SetLocalPosition(cameraWorkData.defLocalPos.x, cameraWorkData.defLocalPos.y + cameraWorkData.multiOffsetY, 0f);
		}
		cameradData.fieldOfViewHalf = cameradData.camera.fieldOfView * 0.5f;
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			cameradData.defPos = cameradData.camera.transform.localPosition + Vector3.down * 5f;
		}
		else
		{
			cameradData.camera.transform.SetLocalPosition(-1f, -14.2f, 0.9f);
			cameradData.camera.transform.SetLocalEulerAngles(20f, 260f, 0f);
			cameradData.defPos = cameradData.camera.transform.localPosition;
		}
		cameradData.viewSize = CalcManager.CalcCameraViewSize(cameradData.camera.fieldOfView, cameradData.camera.aspect, GetCameraHeight());
		CalcManager.mCalcVector3 = cameradData.defPos;
		cameraOffsetZ = cameraWorkData.offsetZ[0];
		CalcManager.mCalcVector3.z -= cameradData.viewSize.y * cameraOffsetZ;
		cameradData.camera.transform.localPosition = CalcManager.mCalcVector3;
	}
	public bool GetChangeCort()
	{
		return changeCort;
	}
	public void ChangeCameraRootPos(int n)
	{
		if (n == 0)
		{
			changeCort = false;
		}
		else
		{
			changeCort = true;
		}
	}
	private void InitStage()
	{
		fieldData.SetHalfCourtSize(fieldData.backRight.position.x - fieldData.frontLeft.position.x, fieldData.CenterCircle.position.z - fieldData.frontLeft.position.z);
		fieldObj.transform.parent = fieldData.fieldAnchor.transform;
		BeachVolley_Define.BM.GetBall().SetChangeOutlineColor(fieldObj.GetBallOutLineColor());
	}
	private void InitFormationAnchor()
	{
		for (int i = 0; i < anchorList.Length; i++)
		{
			GetBenchAnchor(i).position = GetBenchPos(i, 0, _benchCenter: true);
		}
	}
	public void UpdateMethod()
	{
		float num = 0f;
		cameraChangeInterval -= Time.deltaTime;
		float num2 = -2f;
		Vector3 vector;
		switch (SingletonCustom<BeachVolley_MainGameManager>.Instance.GetGameState())
		{
		case BeachVolley_MainGameManager.GameState.SET_INTERVAL:
		case BeachVolley_MainGameManager.GameState.GAME_END_WAIT:
		case BeachVolley_MainGameManager.GameState.GAME_END:
			if (cameraChangeInterval <= 0f)
			{
				num = cameraWorkData.approachDistance;
				MoveCamera(new Vector3(0f, 0f, 0f), cameraWorkData.moveSpeedWhenSetPlay, 0, _changeY: false, num);
			}
			break;
		case BeachVolley_MainGameManager.GameState.SERVE_STANDBY:
		case BeachVolley_MainGameManager.GameState.SERVE:
			if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
			{
				num = cameraWorkData.approachDistance;
				if (BeachVolley_Define.MGM.GetSetPlayTeamNo() == 0)
				{
					vector = GetServeAnchor(0).position;
				}
				else
				{
					vector = GetMyEndLine(0).position;
					vector.z += GetFieldData().HalfCourtSize.z * 0.5f;
				}
				vector.z += 3f;
				vector.x = BeachVolley_Define.MCM.GetControlChara(BeachVolley_Define.MGM.GetSetPlayTeamNo()).GetPos().x;
			}
			else
			{
				num = cameraWorkData.approachDistanceMulti[0];
				vector = GetAttackLineAnchor(BeachVolley_Define.MGM.GetSetPlayTeamNo()).position;
				vector = ((!changeCort) ? (vector + Vector3.right * (9.5f + num2)) : (vector - Vector3.right * (9.5f + num2)));
			}
			MoveCamera(cameradData.root.InverseTransformPoint(vector), cameraWorkData.moveSpeedWhenSetPlay, 1, _changeY: true, num);
			break;
		default:
		{
			cameraChangeInterval = 1f;
			float num3 = 0f;
			if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
			{
				num = cameraWorkData.approachDistance;
				vector = BeachVolley_Define.BM.GetBallDropPrediPosGround();
				if (CheckInFrontZone(ConvertLocalPos(vector, 0), 0))
				{
					if (BeachVolley_Define.MCM.CheckLastTouch())
					{
						vector.z = GetAttackLineAnchor(0).position.z;
					}
					else
					{
						vector.z = GetMyEndLine(0).position.z + GetFieldData().HalfCourtSize.z * 0.3f + 0.5f;
					}
				}
				if (BeachVolley_Define.MCM.BallControllTeam == 1 || (BeachVolley_Define.Ball.GetLastHitChara() != null && BeachVolley_Define.Ball.GetLastHitChara().TeamNo == 1))
				{
					vector.z = GetMyEndLine(0).position.z + GetFieldData().HalfCourtSize.z * 0.3f + 0.5f;
				}
				float num4 = BeachVolley_Define.BM.GetBallPos().z - GetFieldData().GetCenterPos().z - 2f;
				num4 = ((!(zPrev < num4)) ? (BeachVolley_Define.BM.GetBallPos().z - GetFieldData().GetCenterPos().z - 2f) : (BeachVolley_Define.BM.GetBallPos().z - GetFieldData().GetCenterPos().z - 3f));
				if (num4 > 0f)
				{
					if (num4 > 4f)
					{
						num4 = 4f;
					}
					num3 = 6f * (num4 / 4f);
				}
				zPrev = num4;
			}
			else
			{
				num = cameraWorkData.approachDistanceMulti[1];
				vector = BeachVolley_Define.BM.GetBallDropPrediPosGround();
				vector = ((!changeCort) ? (vector + Vector3.right * (9.5f + num2)) : (vector - Vector3.right * (9.5f + num2)));
			}
			MoveCamera(cameradData.root.InverseTransformPoint(vector), cameraWorkData.moveSpeed + BeachVolley_Define.Ball.GetRigid().velocity.magnitude * 0.1f, 1, _changeY: true, num, _lerp: true, 0f - num3);
			break;
		}
		case BeachVolley_MainGameManager.GameState.GAME_START:
		case BeachVolley_MainGameManager.GameState.GAME_START_STANDBY:
		case BeachVolley_MainGameManager.GameState.NONE:
			break;
		}
		cameradData.viewSize = CalcManager.CalcCameraViewSize(cameradData.camera.fieldOfView, cameradData.camera.aspect, GetCameraHeight());
	}
	public float CameraAngleXRotate()
	{
		float num = Vector3.Angle(Vector3.down, (BeachVolley_Define.BM.GetBall().transform.position - cameradData.camera.transform.position).normalized);
		if (angleXPrev < num)
		{
			return num;
		}
		return -1f;
	}
	public float CameraAngleXRotate2()
	{
		float num = Vector3.Angle(Vector3.down, (BeachVolley_Define.BM.GetBall().transform.position - cameradData.camera.transform.position).normalized);
		if (angleXPrev + 25f < num)
		{
			return num;
		}
		return -1f;
	}
	private void MoveCamera(Vector3 _targetPos, float _moveSpeed, int _offsetNo, bool _changeY, float _approachDistance, bool _lerp = true, float _plusZ = 0f)
	{
		CalcManager.mCalcVector3 = cameradData.camera.transform.localEulerAngles;
		CalcManager.mCalcVector3.x = cameraWorkData.rot[_offsetNo];
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			CalcManager.mCalcVector3.x -= 6f;
		}
		float num = 0f;
		if (!BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			float num2 = CameraAngleXRotate();
			if (num2 > 0f)
			{
				float num3 = Vector3.Angle(Vector3.down, cameradData.camera.transform.forward);
				float num4 = (num2 - num3) * 0.5f;
				if (num4 > 0f)
				{
					CalcManager.mCalcVector3.x -= num4;
				}
				else
				{
					if (num4 < -1f)
					{
						num4 = -1f;
					}
					CalcManager.mCalcVector3.x -= num4;
				}
			}
			else
			{
				angleXPrev = Vector3.Angle(Vector3.down, cameradData.camera.transform.forward);
			}
			num = BeachVolley_Define.BM.GetBall().transform.position.z - ObjAnchorPositionZ;
			if (num > 10f)
			{
				num = 10f;
			}
			else if (num < -10f)
			{
				num = -10f;
			}
			num *= 0.1f;
			float num5 = 270f;
			CalcManager.mCalcVector3.y = num5 + num * -1f;
		}
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
			cameraOffsetZ = Mathf.Lerp(cameraOffsetZ, cameraWorkData.offsetZ[_offsetNo], _moveSpeed * Time.deltaTime);
		}
		else
		{
			cameraOffsetZ = cameraWorkData.offsetZ[_offsetNo];
		}
		if (_changeY)
		{
			CalcManager.mCalcVector3.y = 0f - _approachDistance;
		}
		CalcManager.mCalcVector3.z -= cameradData.viewSize.y * cameraOffsetZ + _plusZ;
		float num6 = CameraAngleXRotate2();
		if (num6 > 0f && BeachVolley_Define.MGM.GetGameState() != BeachVolley_MainGameManager.GameState.SCORE_UP)
		{
			float num7 = Vector3.Angle(Vector3.down, cameradData.camera.transform.forward);
			float num8 = (num6 - (num7 + 25f)) * 1f;
			float num9 = 1f;
			if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
			{
				num9 = 0.8f;
			}
			if (num8 > 0f)
			{
				CalcManager.mCalcVector3.y += num8 * num9;
			}
			else
			{
				if (num8 < -1f)
				{
					num8 = -1f;
				}
				CalcManager.mCalcVector3.y += num8 * num9;
			}
		}
		else
		{
			angleXPrev2 = Vector3.Angle(Vector3.down, cameradData.camera.transform.forward);
		}
		if (CheckCameraMoveLimitArea(CalcManager.mCalcVector3, cameraOffsetZ))
		{
			CalcManager.mCalcVector3 = GetCameraMoveLimitAreaPos(CalcManager.mCalcVector3, cameraOffsetZ);
		}
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			CalcManager.mCalcVector3.x *= 0.3f;
		}
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
		if (cameradData.viewSize.y < cameraWorkData.gameShowSize.z)
		{
			if (cameradData.offsetPos.z - cameradData.viewSize.y * 0.5f < (0f - cameraWorkData.gameShowSize.z) * 0.5f - cameradData.viewSize.y * _offset)
			{
				return true;
			}
			if (cameradData.offsetPos.z + cameradData.viewSize.y * 0.5f > cameraWorkData.gameShowSize.z * 0.5f - cameradData.viewSize.y * _offset)
			{
				return true;
			}
		}
		if (cameradData.viewSize.x < cameraWorkData.gameShowSize.x)
		{
			if (cameradData.offsetPos.x - cameradData.viewSize.x * 0.5f < (0f - cameraWorkData.gameShowSize.x) * 0.5f)
			{
				return true;
			}
			if (cameradData.offsetPos.x + cameradData.viewSize.x * 0.5f > cameraWorkData.gameShowSize.x * 0.5f)
			{
				return true;
			}
		}
		return false;
	}
	private Vector3 GetCameraMoveLimitAreaPos(Vector3 _localPos, float _offset)
	{
		cameradData.offsetPos = _localPos;
		if (cameradData.viewSize.y < cameraWorkData.gameShowSize.z)
		{
			if (cameradData.offsetPos.z - cameradData.viewSize.y * 0.5f < (0f - cameraWorkData.gameShowSize.z) * 0.5f - cameradData.viewSize.y * _offset)
			{
				cameradData.offsetPos.z = (0f - cameraWorkData.gameShowSize.z) * 0.5f + cameradData.viewSize.y * 0.5f - cameradData.viewSize.y * _offset;
			}
			else if (cameradData.offsetPos.z + cameradData.viewSize.y * 0.5f > cameraWorkData.gameShowSize.z * 0.5f - cameradData.viewSize.y * _offset)
			{
				cameradData.offsetPos.z = cameraWorkData.gameShowSize.z * 0.5f - cameradData.viewSize.y * 0.5f - cameradData.viewSize.y * _offset;
			}
		}
		if (cameradData.viewSize.x < cameraWorkData.gameShowSize.x)
		{
			if (cameradData.offsetPos.x - cameradData.viewSize.x * 0.5f < (0f - cameraWorkData.gameShowSize.x) * 0.5f)
			{
				cameradData.offsetPos.x = (0f - cameraWorkData.gameShowSize.x) * 0.5f + cameradData.viewSize.x * 0.5f;
			}
			else if (cameradData.offsetPos.x + cameradData.viewSize.x * 0.5f > cameraWorkData.gameShowSize.x * 0.5f)
			{
				cameradData.offsetPos.x = cameraWorkData.gameShowSize.x * 0.5f - cameradData.viewSize.x * 0.5f;
			}
		}
		return cameradData.offsetPos;
	}
	public bool CheckInCourt(Vector3 _pos, float _objRadius = 0f)
	{
		if (CalcManager.CheckRange(_pos.x, fieldData.frontLeft.position.x - _objRadius, fieldData.backRight.position.x + _objRadius))
		{
			return CalcManager.CheckRange(_pos.z, fieldData.frontLeft.position.z - _objRadius, fieldData.backRight.position.z + _objRadius);
		}
		return false;
	}
	public bool CheckInCourt(Vector3 _pos, float _teamNo, float _objRadius = 0f)
	{
		if (!CalcManager.CheckRange(_pos.x, fieldData.frontLeft.position.x - _objRadius, fieldData.backRight.position.x + _objRadius))
		{
			return false;
		}
		if (_teamNo == 0f)
		{
			if (!CalcManager.CheckRange(_pos.z, fieldData.frontLeft.position.z - _objRadius, fieldData.GetCenterPos().z + _objRadius))
			{
				return false;
			}
		}
		else if (!CalcManager.CheckRange(_pos.z, fieldData.GetCenterPos().z - _objRadius, fieldData.backRight.position.z + _objRadius))
		{
			return false;
		}
		return true;
	}
	public bool CheckInFrontZone(BeachVolley_Character _chara)
	{
		return _chara.GetPos(_isLocal: true).z > GetAttackLineAnchor(_chara.TeamNo).localPosition.z;
	}
	public bool CheckInFrontZone(Vector3 _localPos, int _teamNo)
	{
		return _localPos.z > GetAttackLineAnchor(_teamNo).localPosition.z;
	}
	public bool CheckInBackZone(BeachVolley_Character _chara)
	{
		return !CheckInFrontZone(_chara);
	}
	public bool CheckOnDesk(Vector3 _pos, float _objRadius = 0f)
	{
		if (CalcManager.CheckRange(_pos.x, desk.transform.position.x - desk.size.z * desk.transform.localScale.z * 0.5f + _objRadius, desk.transform.position.x + desk.size.z * desk.transform.localScale.z * 0.5f - _objRadius))
		{
			return CalcManager.CheckRange(_pos.z, desk.transform.position.z - desk.size.x * desk.transform.localScale.x * 0.5f + _objRadius, desk.transform.position.z + desk.size.x * desk.transform.localScale.x * 0.5f - _objRadius);
		}
		return false;
	}
	public bool CheckAntennaInsideArea(Vector3 _pos, float _objRadius = 0f)
	{
		if (CalcManager.CheckRange(_pos.x, fieldData.CenterCircle.position.x - fieldData.antennaAnchor.localPosition.x + _objRadius, fieldData.CenterCircle.position.x + fieldData.antennaAnchor.localPosition.x - _objRadius))
		{
			return _pos.y > fieldData.GetNetTopPos().y;
		}
		return false;
	}
	public Vector3 ConvertLocalPos(Vector3 _pos, int _teamNo)
	{
		return GetTeamAnchor(_teamNo).InverseTransformPoint(_pos);
	}
	public Vector3 ConvertWorldPos(Vector3 _localPos, int _teamNo)
	{
		return GetTeamAnchor(_teamNo).TransformPoint(_localPos);
	}
	public Vector3 ConvertLocalPosPer(Vector3 _pos, int _teamNo)
	{
		Vector3 vector = GetTeamAnchor(_teamNo).InverseTransformPoint(_pos);
		Vector3 result = default(Vector3);
		result.x = vector.x / fieldData.HalfCourtSize.x;
		result.z = vector.z / (fieldData.HalfCourtSize.z * 2f);
		result.y = 0f;
		return result;
	}
	public Vector3 ConvertPosPerToWorld(Vector3 _posPer, int _teamNo)
	{
		_posPer.x *= fieldData.HalfCourtSize.x;
		_posPer.z *= fieldData.HalfCourtSize.z * 2f;
		_posPer.y = 0f;
		return GetTeamAnchor(_teamNo).TransformPoint(_posPer);
	}
	public Vector3 ConvertPosPerToLocal(Vector3 _posPer)
	{
		_posPer.x *= fieldData.HalfCourtSize.x;
		_posPer.z *= fieldData.HalfCourtSize.z * 2f;
		_posPer.y = 0f;
		return _posPer;
	}
	public void ReverseField(int _setNo)
	{
		isReverseField = !isReverseField;
		if (isReverseField)
		{
			UnityEngine.Debug.Log("反転する");
			GetFieldData().net.transform.SetLocalEulerAnglesY(0f);
			int num3 = _setNo % 2;
			field.transform.SetLocalEulerAnglesY(180f);
			floor.transform.SetLocalEulerAnglesY(180f);
			fieldObj.transform.SetLocalEulerAnglesY(180f);
			desk.transform.SetLocalEulerAnglesY(270f);
			backgroundAnchor.SetLocalEulerAnglesY(180f);
			classRoom.SetLocalEulerAnglesY(90f);
			cameradData.rotationAnchor.SetEulerAnglesY(180f);
			if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
			{
				if (fieldObj.arrayHiddenVerticalObjFirstHalf != null)
				{
					for (int i = 0; i < fieldObj.arrayHiddenVerticalObjFirstHalf.Length; i++)
					{
						fieldObj.arrayHiddenVerticalObjFirstHalf[i].SetActive(value: true);
					}
				}
				if (fieldObj.arrayHiddenVerticalObjSecondHalf != null)
				{
					for (int j = 0; j < fieldObj.arrayHiddenVerticalObjSecondHalf.Length; j++)
					{
						fieldObj.arrayHiddenVerticalObjSecondHalf[j].SetActive(value: false);
					}
				}
				return;
			}
			if (fieldObj.arrayHiddenHorizontalFirstHalfObj != null)
			{
				for (int k = 0; k < fieldObj.arrayHiddenHorizontalFirstHalfObj.Length; k++)
				{
					fieldObj.arrayHiddenHorizontalFirstHalfObj[k].SetActive(value: true);
				}
			}
			if (fieldObj.arrayHiddenHorizontalSecondHalfObj != null)
			{
				for (int l = 0; l < fieldObj.arrayHiddenHorizontalSecondHalfObj.Length; l++)
				{
					fieldObj.arrayHiddenHorizontalSecondHalfObj[l].SetActive(value: false);
				}
			}
			return;
		}
		UnityEngine.Debug.Log("もとに戻す");
		field.transform.SetLocalEulerAnglesY(0f);
		floor.transform.SetLocalEulerAnglesY(0f);
		fieldObj.transform.SetLocalEulerAnglesY(0f);
		desk.transform.SetLocalEulerAnglesY(90f);
		backgroundAnchor.SetLocalEulerAnglesY(0f);
		classRoom.SetLocalEulerAnglesY(270f);
		cameradData.rotationAnchor.SetEulerAnglesY(0f);
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			if (fieldObj.arrayHiddenVerticalObjFirstHalf != null)
			{
				for (int m = 0; m < fieldObj.arrayHiddenVerticalObjFirstHalf.Length; m++)
				{
					fieldObj.arrayHiddenVerticalObjFirstHalf[m].SetActive(value: false);
				}
			}
			if (fieldObj.arrayHiddenVerticalObjSecondHalf != null)
			{
				for (int n = 0; n < fieldObj.arrayHiddenVerticalObjSecondHalf.Length; n++)
				{
					fieldObj.arrayHiddenVerticalObjSecondHalf[n].SetActive(value: true);
				}
			}
			return;
		}
		if (fieldObj.arrayHiddenHorizontalFirstHalfObj != null)
		{
			for (int num = 0; num < fieldObj.arrayHiddenHorizontalFirstHalfObj.Length; num++)
			{
				fieldObj.arrayHiddenHorizontalFirstHalfObj[num].SetActive(value: false);
			}
		}
		if (fieldObj.arrayHiddenHorizontalSecondHalfObj != null)
		{
			for (int num2 = 0; num2 < fieldObj.arrayHiddenHorizontalSecondHalfObj.Length; num2++)
			{
				fieldObj.arrayHiddenHorizontalSecondHalfObj[num2].SetActive(value: true);
			}
		}
	}
	public void StartCameraReverseAnimation(float _time)
	{
		LeanTween.rotateY(cameradData.rotationAnchor.gameObject, 0f, _time).setEaseOutCubic().setOnComplete((Action)delegate
		{
			cameradData.rotationAnchor.SetEulerAnglesY(0f);
		});
		StartCoroutine(_NetRotate(_time * 0.5f));
	}
	private IEnumerator _NetRotate(float _time)
	{
		yield return _time;
		GetFieldData().net.transform.AddLocalEulerAnglesY(180f);
	}
	public void StartNetAnimation(bool right)
	{
		if (right)
		{
			if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.HORIZONTAL) && BeachVolley_Define.MGM.GetChangeCort())
			{
				netAnimation.SetTrigger("OnLeftTrigger");
			}
			else
			{
				netAnimation.SetTrigger("OnRightTrigger");
			}
		}
		else if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.HORIZONTAL) && BeachVolley_Define.MGM.GetChangeCort())
		{
			netAnimation.SetTrigger("OnRightTrigger");
		}
		else
		{
			netAnimation.SetTrigger("OnLeftTrigger");
		}
	}
	public int CheckBallWhichTeamArea()
	{
		if (BeachVolley_Define.BM.GetBallPos().z < fieldData.CenterCircle.position.z)
		{
			return 0;
		}
		return 1;
	}
	public bool CheckBallWhichTeamArea(int _teamNo)
	{
		if (BeachVolley_Define.BM.GetBallPos().z < fieldData.CenterCircle.position.z)
		{
			return _teamNo == 0;
		}
		return 1 == _teamNo;
	}
	public bool CheckPosWhichTeamArea(Vector3 _pos, int _teamNo)
	{
		if (_pos.z < fieldData.CenterCircle.position.z)
		{
			return _teamNo == 0;
		}
		return 1 == _teamNo;
	}
	public bool CheckBallThrowAirTeam(int _teamNo)
	{
		if (Mathf.Abs(BeachVolley_Define.BM.GetBall().GetRigid().velocity.z) <= 1f)
		{
			return false;
		}
		if (BeachVolley_Define.BM.GetBall().GetRigid().velocity.z < 0f)
		{
			return _teamNo == 0;
		}
		return 1 == _teamNo;
	}
	public bool CheckBallAir()
	{
		return BeachVolley_Define.BM.GetBallPos(_offset: false, _local: true).y > BeachVolley_Define.BM.GetBallSize() * 1.5f;
	}
	public Transform GetObjAnchor()
	{
		return objAnchor;
	}
	public Transform GetBackgroundAnchor()
	{
		return backgroundAnchor;
	}
	public Transform GetFormationAnchor(int _teamNo, int _no)
	{
		return anchorList[_teamNo].formationAnchorBeach[_no];
	}
	public Vector3 GetFormationPos(int _teamNo, int _no)
	{
		return anchorList[_teamNo].formationAnchorBeach[_no].position;
	}
	public Transform GetServeAnchor(int _teamNo)
	{
		return anchorList[_teamNo].serveAnchor;
	}
	public Transform GetBenchAnchor(int _teamNo)
	{
		return anchorList[_teamNo].benchAnchor;
	}
	public Transform GetTeamAnchor(int _teamNo)
	{
		return anchorList[_teamNo].teamAnchor;
	}
	public Transform GetAttackLineAnchor(int _teamNo)
	{
		return anchorList[_teamNo].attackLineAnchor;
	}
	public Transform GetEnterAnchor(int _teamNo)
	{
		return anchorList[_teamNo].enterAnchor;
	}
	public Transform GetExitAnchor(int _teamNo)
	{
		return anchorList[_teamNo].exitAnchor;
	}
	public GameObject GetCourtLineWall()
	{
		return courtLineWall;
	}
	public Transform GetMyEndLine(int _teamNo)
	{
		return GetTeamAnchor(_teamNo);
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
	public Vector3 GetBenchPos(int _teamNo, int _setNo = 0, bool _benchCenter = false)
	{
		Vector3 position = GetFieldData().CenterCircle.position;
		Vector3 benchPosition = BeachVolley_Define.FM.fieldObj.GetBenchPosition(_teamNo);
		position.z += GetFieldData().HalfCourtSize.z * benchPosition.z * (float)((_teamNo != 0) ? 1 : (-1));
		position.x += GetFieldData().HalfCourtSize.x * benchPosition.x;
		if (_setNo > 1)
		{
			position.x = GetFieldData().CenterCircle.position.x + (GetFieldData().CenterCircle.position.x - position.x);
		}
		if (!_benchCenter)
		{
			position.z += UnityEngine.Random.Range(-2, 2);
		}
		return position;
	}
	public Vector3 GetTargetPos(int _teamNo, Vector3 _vec, float _offsetX = 0f, float _offsetZ = 0f)
	{
		Vector3 position = GetFieldData().CenterCircle.transform.position;
		if (_teamNo == 0)
		{
			position.z -= GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		else
		{
			position.z += GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		float num = (Mathf.Abs(_vec.x) > Mathf.Abs(_vec.z)) ? Mathf.Abs(_vec.x) : Mathf.Abs(_vec.z);
		float magnitude = _vec.magnitude;
		if (magnitude != 0f)
		{
			position.x += _vec.x / num * magnitude * (GetFieldData().HalfCourtSize.x * 0.5f - _offsetX);
			position.z += _vec.z / num * magnitude * (GetFieldData().HalfCourtSize.z * 0.5f - _offsetZ);
		}
		return position;
	}
	public Vector3 GetTargetPosServe(int _teamNo, Vector3 _vec, float _offsetX = 0f, float _offsetZ = 0f, BeachVolley_Character _chara = null)
	{
		Vector3 position = GetFieldData().CenterCircle.transform.position;
		if (_teamNo == 0)
		{
			position.z -= GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		else
		{
			position.z += GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		Vector3 vector = _chara.GetPos() - BeachVolley_Define.FM.GetServeAnchor(1 - _teamNo).position;
		float num = 0.7f;
		float num2 = GetFieldData().HalfCourtSize.x * (1f - num) * 0.5f;
		float num3 = GetFieldData().HalfCourtSize.z * 0.5f * (1f - num) * 0.5f;
		Vector3 b = new Vector3(vector.x / 4.4f * num2, 0f, vector.z / 1.2f * num3);
		float num4 = (Mathf.Abs(_vec.x) > Mathf.Abs(_vec.z)) ? Mathf.Abs(_vec.x) : Mathf.Abs(_vec.z);
		float magnitude = _vec.magnitude;
		if (magnitude != 0f)
		{
			position.x += _vec.x / num4 * magnitude * (GetFieldData().HalfCourtSize.x * 0.5f - _offsetX) * num;
			position.z += _vec.z / num4 * magnitude * (GetFieldData().HalfCourtSize.z * 0.5f - _offsetZ) * num;
		}
		return position + b;
	}
	public Vector3 GetTargetPosByPer(int _teamNo, Vector3 _posPer, float _offsetX = 0f, float _offsetZ = 0f)
	{
		Vector3 position = GetFieldData().CenterCircle.transform.position;
		if (_teamNo == 0)
		{
			position.z -= GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		else
		{
			position.z += GetFieldData().HalfCourtSize.z * 0.5f + _offsetZ;
		}
		position.x += _posPer.x * (GetFieldData().HalfCourtSize.x * 0.5f - _offsetX);
		position.z += _posPer.z * (GetFieldData().HalfCourtSize.z * 0.5f - _offsetZ);
		return position;
	}
	public Vector3 GetTargetPos(Vector3 _centerPos, Vector3 _size, Vector3 _vec)
	{
		Vector3 result = _centerPos;
		result.x += _vec.x * _size.x;
		result.z += _vec.z * _size.z;
		return result;
	}
	public Vector3 GetFrontZonePos(int _teamNo)
	{
		Vector3 vector = GetAttackLineAnchor(_teamNo).localPosition;
		vector.z += (GetFieldData().HalfCourtSize.z - vector.x) * 0.5f;
		vector = ConvertWorldPos(vector, _teamNo);
		return vector;
	}
	public bool IsMulti()
	{
		return !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
	}
	public void TutorialInit()
	{
		TutorialCameraInit();
	}
	public void TutorialCameraInit()
	{
	}
	public void TutorialCameraPosReset()
	{
		cameradData.camera.transform.localPosition = new Vector3(0f, CAMERA_DEFENCE_OFFSET_Y, CAMERA_DEFENCE_OFFSET_Z);
		cameraOffsetZ = 0.9f;
	}
	public void TutorialUpdate()
	{
		UpdateMethod();
	}
}
