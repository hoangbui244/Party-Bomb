using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class FieldManager : SingletonCustom<FieldManager>
	{
		public enum DirType
		{
			LEFT,
			RIGHT,
			FRONT,
			BACK
		}
		[Serializable]
		public struct FieldAnchorList
		{
			[Header("フィ\u30fcルドアンカ\u30fc")]
			public Transform fieldAnchor;
			[Header("ゴ\u30fcルライン(奥)")]
			public Transform back;
			[Header("ゴ\u30fcルライン(手前)")]
			public Transform front;
			[Header("サイドライン(左)")]
			public Transform left;
			[Header("サイドライン(右)")]
			public Transform right;
			[Header("センタ\u30fcサ\u30fcクル")]
			public SphereCollider centerCircle;
			[Header("ペナルティ\u30fcエリア")]
			public BoxCollider penaltyArea;
			[Header("ゴ\u30fcルエリア")]
			public BoxCollider[] goalArea;
			[Header("ゴ\u30fcルサイズ")]
			public BoxCollider goalSize;
		}
		public struct FieldData
		{
			public Vector3 halfSize;
			public float centerCircleRadius;
			public Vector3 penaltyAreaSize;
			public Vector3 goalAreaSize;
			public Vector3 goalSize;
		}
		[Serializable]
		public struct FormationAnchorList
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
		public static int OUT_DISABLE_STAGE_NO = 18;
		private float CAMERA_MOVE_SPEED = 3f;
		private float CAMERA_MOVE_SPEED_SET_PLAY = 2f;
		private float CAMERA_MOVE_SPEED_LIMIT_IN = 3f;
		[SerializeField]
		[Header("オブジェクトアンカ\u30fc")]
		private Transform objAnchor;
		private Vector3 calcVec3;
		[SerializeField]
		[Header("フィ\u30fcルドアンカ\u30fcリスト")]
		private FieldAnchorList fieldAnchorList;
		private FieldData fieldData;
		[SerializeField]
		[Header("チ\u30fcムアンカ\u30fc")]
		private Transform[] teamAnchor;
		[SerializeField]
		[Header("フォ\u30fcメ\u30fcションアンカ\u30fc")]
		private FormationAnchorList[] formationAnchorList;
		private Vector3 gameShowSize = new Vector3(18f, 0f, 32f);
		[SerializeField]
		[Header("通常ゴ\u30fcル設定")]
		private BoxCollider[] arrayNormalGoal;
		[SerializeField]
		[Header("城内フィ\u30fcルドゴ\u30fcル設定")]
		private BoxCollider[] arrayCastleGoal;
		private float[] CAMERA_DEF_Y = new float[19]
		{
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f,
			30f
		};
		private float[] CAMERA_APPROACH_LENGTH = new float[19]
		{
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f,
			17f
		};
		private float[] CAMERA_ZOOM_OUT_OFFSET_Z = new float[19]
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
		private Vector3 cameraOffset;
		[SerializeField]
		[Header("カメラデ\u30fcタ")]
		private CameradData cameraData;
		private int[] formationNo = new int[2];
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
		[Header("床")]
		private MeshRenderer floor;
		[SerializeField]
		[Header("机")]
		private MeshRenderer[] chairs = new MeshRenderer[2];
		private StageData fieldObj;
		private float cameraChangeInterval;
		private bool isReverseField;
		private BallScript ballObj;
		private List<TranslucentObject> translucentObject = new List<TranslucentObject>();
		public static int[] DebugNo = new int[2]
		{
			-1,
			-1
		};
		private bool isVerticalCamera = true;
		public Transform[] TeamAnchor => teamAnchor;
		public int[] FormationNo => formationNo;
		public void Init()
		{
			CAMERA_OFFSET[0].z = CAMERA_ZOOM_OUT_OFFSET_Z[GameSaveData.GetSelectArea()];
			GameSaveData.SetSelectCameraMode(GameSaveData.CameraMode.HORIZONTAL);
			if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
			{
				SetVerticalCamera();
			}
			else
			{
				SetHorizontalCamera();
			}
			cameraData.fieldOfViewHalf = cameraData.camera.fieldOfView * 0.5f;
			fieldData.centerCircleRadius = fieldAnchorList.centerCircle.radius;
			fieldData.penaltyAreaSize = fieldAnchorList.penaltyArea.size;
			fieldData.goalAreaSize = fieldAnchorList.goalArea[0].size;
			fieldData.goalSize = fieldAnchorList.goalSize.size;
			fieldData.halfSize.x = fieldAnchorList.right.position.x - fieldAnchorList.left.position.x;
			fieldData.halfSize.z = fieldAnchorList.centerCircle.transform.position.z - fieldAnchorList.front.position.z;
			for (int i = 0; i < formationAnchorList.Length; i++)
			{
				formationNo[i] = SchoolData.GetCommonFormationNo(i);
				if (DebugNo[i] != -1)
				{
					formationNo[i] = DebugNo[i];
				}
				for (int j = 0; j < formationAnchorList[i].anchor.Length; j++)
				{
					if (j == 0)
					{
						formationAnchorList[i].anchor[j].SetLocalPosition(0f, 0f, 0.075f * fieldData.halfSize.z);
					}
					else
					{
						formationAnchorList[i].anchor[j].SetLocalPosition((SingletonCustom<FormationListManager>.Instance.GetData(formationNo[i]).playerData[j].pos.x - 0.5f) * fieldData.halfSize.x, 0f, SingletonCustom<FormationListManager>.Instance.GetData(formationNo[i]).playerData[j].pos.y * fieldData.halfSize.z);
					}
				}
			}
			for (int k = 0; k < DebugNo.Length; k++)
			{
				DebugNo[k] = -1;
			}
			cameraData.defPos = cameraData.camera.transform.localPosition;
			cameraData.viewSize = CalcManager.CalcCameraViewSize(cameraData.camera.fieldOfView, cameraData.camera.aspect, GetCameraHeight());
			CalcManager.mCalcVector3 = cameraData.defPos;
			cameraOffset.x = CAMERA_OFFSET[0].x;
			cameraOffset.z = CAMERA_OFFSET[0].z;
			CalcManager.mCalcVector3.x -= cameraData.viewSize.x * cameraOffset.x;
			CalcManager.mCalcVector3.z -= cameraData.viewSize.y * cameraOffset.z;
			cameraData.camera.transform.localPosition = CalcManager.mCalcVector3;
			field.material = SingletonCustom<StageListManager>.Instance.GetStageData(GameSaveData.GetSelectArea()).fieldMaterial;
			floor.material = SingletonCustom<StageListManager>.Instance.GetStageData(GameSaveData.GetSelectArea()).floorMaterial;
			desk.GetComponent<MeshRenderer>().material = SingletonCustom<StageListManager>.Instance.GetStageData(GameSaveData.GetSelectArea()).GetDeskMaterial();
			for (int l = 0; l < chairs.Length; l++)
			{
				chairs[l].material = SingletonCustom<StageListManager>.Instance.GetStageData(GameSaveData.GetSelectArea()).GetChairMaterial();
			}
			fieldObj = SingletonCustom<StageListManager>.Instance.InstantiateObj<GameObject>(GameSaveData.GetSelectArea(), fieldAnchorList.fieldAnchor.transform.position, Quaternion.identity).GetComponent<StageData>();
			fieldObj.transform.parent = fieldAnchorList.fieldAnchor.transform;
			SingletonCustom<BallManager>.Instance.GetBall().SetMaterial(fieldObj.GetRandomBallMaterial());
			if (GameSaveData.GetSelectArea() == 16)
			{
				desk.GetComponent<MeshRenderer>().enabled = false;
			}
			if (GameSaveData.GetSelectArea() == OUT_DISABLE_STAGE_NO)
			{
				for (int m = 0; m < arrayNormalGoal.Length; m++)
				{
					arrayNormalGoal[m].gameObject.SetActive(value: false);
				}
				for (int n = 0; n < arrayCastleGoal.Length; n++)
				{
					arrayCastleGoal[n].gameObject.SetActive(value: true);
				}
				if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
				{
					fieldObj.GetArrayTranslucentObject()[0].SetEnable(_isEnable: false);
					fieldObj.GetArrayTranslucentObject()[2].SetEnable(_isEnable: false);
				}
				else
				{
					fieldObj.GetArrayTranslucentObject()[0].SetEnable(_isEnable: false);
					fieldObj.GetArrayTranslucentObject()[1].SetEnable(_isEnable: false);
					fieldObj.GetArrayTranslucentObject()[2].SetEnable(_isEnable: false);
					fieldObj.GetArrayTranslucentObject()[3].SetEnable(_isEnable: false);
				}
				fieldData.goalAreaSize = arrayCastleGoal[0].size;
			}
			ballObj = SingletonCustom<BallManager>.Instance.GetBall();
		}
		private void SetVerticalCamera()
		{
			isVerticalCamera = true;
			cameraData.camera.transform.SetLocalEulerAngles(CAMERA_ROT[0].x, CAMERA_ROT[0].y, CAMERA_ROT[0].z);
			cameraData.root.transform.SetLocalPosition(0f, CAMERA_DEF_Y[GameSaveData.GetSelectArea()], -1.5f);
			fieldLight.transform.SetLocalEulerAnglesY(0f);
		}
		private void SetHorizontalCamera()
		{
			isVerticalCamera = false;
			CAMERA_OFFSET[0].z = 0f;
			CAMERA_OFFSET[0].x = -0.85f;
			CAMERA_OFFSET[1].z = 0f;
			CAMERA_OFFSET[1].x = -0.7f;
			for (int i = 0; i < CAMERA_ROT.Length; i++)
			{
				CAMERA_ROT[i].x = 40f;
				CAMERA_ROT[i].y = 270f;
				CAMERA_ROT[i].z = 0f;
			}
			cameraData.camera.transform.SetLocalEulerAngles(CAMERA_ROT[0].x, CAMERA_ROT[0].y, CAMERA_ROT[0].z);
			CAMERA_DEF_Y[GameSaveData.GetSelectArea()] -= 14f;
			cameraData.root.transform.SetLocalPosition(0f, CAMERA_DEF_Y[GameSaveData.GetSelectArea()], 0f);
			CAMERA_APPROACH_LENGTH[GameSaveData.GetSelectArea()] -= 14.5f;
			fieldLight.transform.SetLocalEulerAnglesY(-90f);
		}
		public void UpdateMethod()
		{
		}
		public void LateUpdate()
		{
			switch (SingletonCustom<MainGameManager>.Instance.GetGameState())
			{
			case MainGameManager.GameState.HALF_TIME:
			case MainGameManager.GameState.GOAL_KICK:
			case MainGameManager.GameState.GOAL:
				cameraChangeInterval -= Time.deltaTime;
				if (cameraChangeInterval <= 0f)
				{
					CalcManager.mCalcVector3 = cameraData.camera.transform.localEulerAngles;
					CalcManager.mCalcVector3 = CAMERA_ROT[0];
					cameraData.camera.transform.localEulerAngles = Vector3.Lerp(cameraData.camera.transform.localEulerAngles, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED_SET_PLAY * Time.deltaTime);
					cameraData.viewSize = CalcManager.CalcCameraViewSize(cameraData.camera.fieldOfView, cameraData.camera.aspect, GetCameraHeight());
					CalcManager.mCalcVector3 = cameraData.defPos;
					cameraOffset.x = Mathf.Lerp(cameraOffset.x, CAMERA_OFFSET[0].x, CAMERA_MOVE_SPEED_SET_PLAY * Time.deltaTime);
					cameraOffset.z = Mathf.Lerp(cameraOffset.z, CAMERA_OFFSET[0].z, CAMERA_MOVE_SPEED_SET_PLAY * Time.deltaTime);
					CalcManager.mCalcVector3.x -= cameraData.viewSize.x * cameraOffset.x;
					CalcManager.mCalcVector3.z -= cameraData.viewSize.y * cameraOffset.z;
					cameraData.camera.transform.localPosition = Vector3.Lerp(cameraData.camera.transform.localPosition, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED_SET_PLAY * Time.deltaTime);
				}
				break;
			case MainGameManager.GameState.CORNER_KICK:
				cameraChangeInterval = 1.5f;
				CalcManager.mCalcVector3 = cameraData.camera.transform.localEulerAngles;
				CalcManager.mCalcVector3 = CAMERA_ROT[1];
				cameraData.camera.transform.localEulerAngles = Vector3.Lerp(cameraData.camera.transform.localEulerAngles, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED * Time.deltaTime);
				cameraData.viewSize = CalcManager.CalcCameraViewSize(cameraData.camera.fieldOfView, cameraData.camera.aspect, GetCameraHeight());
				CalcManager.mCalcVector3 = cameraData.root.InverseTransformPoint(SingletonCustom<BallManager>.Instance.GetBallPos());
				cameraOffset.x = Mathf.Lerp(cameraOffset.x, CAMERA_OFFSET[1].x, CAMERA_MOVE_SPEED * Time.deltaTime);
				cameraOffset.z = Mathf.Lerp(cameraOffset.z, CAMERA_OFFSET[1].z, CAMERA_MOVE_SPEED * Time.deltaTime);
				if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
				{
					CalcManager.mCalcVector3.y = (0f - CAMERA_APPROACH_LENGTH[GameSaveData.GetSelectArea()]) * 0.8f;
				}
				else
				{
					CalcManager.mCalcVector3.y = (0f - CAMERA_APPROACH_LENGTH[GameSaveData.GetSelectArea()]) * 0.1f;
				}
				CalcManager.mCalcVector3.x -= cameraData.viewSize.x * cameraOffset.x;
				CalcManager.mCalcVector3.z -= cameraData.viewSize.y * cameraOffset.z;
				cameraData.camera.transform.localPosition = Vector3.Lerp(cameraData.camera.transform.localPosition, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED * Time.deltaTime);
				break;
			default:
				cameraChangeInterval = 1.5f;
				CalcManager.mCalcVector3 = cameraData.camera.transform.localEulerAngles;
				CalcManager.mCalcVector3 = CAMERA_ROT[1];
				cameraData.camera.transform.localEulerAngles = Vector3.Lerp(cameraData.camera.transform.localEulerAngles, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED * Time.deltaTime);
				cameraData.viewSize = CalcManager.CalcCameraViewSize(cameraData.camera.fieldOfView, cameraData.camera.aspect, GetCameraHeight());
				CalcManager.mCalcVector3 = cameraData.root.InverseTransformPoint(SingletonCustom<BallManager>.Instance.GetBallPos());
				cameraOffset.x = Mathf.Lerp(cameraOffset.x, CAMERA_OFFSET[1].x, CAMERA_MOVE_SPEED * Time.deltaTime);
				cameraOffset.z = Mathf.Lerp(cameraOffset.z, CAMERA_OFFSET[1].z, CAMERA_MOVE_SPEED * Time.deltaTime);
				CalcManager.mCalcVector3.y = 0f - CAMERA_APPROACH_LENGTH[GameSaveData.GetSelectArea()];
				CalcManager.mCalcVector3.x -= cameraData.viewSize.x * cameraOffset.x;
				CalcManager.mCalcVector3.z -= cameraData.viewSize.y * cameraOffset.z;
				cameraData.camera.transform.localPosition = Vector3.Lerp(cameraData.camera.transform.localPosition, CalcManager.mCalcVector3, CAMERA_MOVE_SPEED * Time.deltaTime);
				break;
			}
			cameraData.viewSize = CalcManager.CalcCameraViewSize(cameraData.camera.fieldOfView, cameraData.camera.aspect, GetCameraHeight());
			cameraData.camera.transform.localPosition = Vector3.Lerp(cameraData.camera.transform.localPosition, GetCameraMoveLimitAreaPos(cameraOffset), CAMERA_MOVE_SPEED_LIMIT_IN * Time.deltaTime);
			if (!GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI) && IsChangeCameraButton(TouchManager.TOUCH_TYPE.DOWN))
			{
				if (isVerticalCamera)
				{
					SetHorizontalCamera();
				}
				else
				{
					SetVerticalCamera();
				}
			}
			if (GameSaveData.GetSelectCameraMode() == GameSaveData.CameraMode.VERTICAL)
			{
				TranslucentStageObj();
			}
		}
		private void TranslucentStageObj()
		{
			for (int i = 0; i < translucentObject.Count; i++)
			{
				translucentObject[i].Reset();
			}
			translucentObject.Clear();
			float magnitude = (ballObj.transform.position - Get3dCamera().transform.position).magnitude;
			RaycastHit[] array = Physics.SphereCastAll(Get3dCamera().transform.position, 3f, (ballObj.transform.position - Get3dCamera().transform.position).normalized, magnitude);
			for (int j = 0; j < array.Length; j++)
			{
				if ((bool)array[j].collider.GetComponent<TranslucentObject>())
				{
					translucentObject.Add(array[j].collider.GetComponent<TranslucentObject>());
					translucentObject[translucentObject.Count - 1].Translucent();
				}
			}
		}
		private bool IsChangeCameraButton(TouchManager.TOUCH_TYPE _type)
		{
			return false;
		}
		private bool CheckCameraMoveLimitArea(Vector3 _offset)
		{
			cameraData.offsetPos = cameraData.camera.transform.localPosition;
			if (cameraData.viewSize.y < gameShowSize.z)
			{
				if (cameraData.offsetPos.z - cameraData.viewSize.y * 0.5f < (0f - gameShowSize.z) * 0.5f - cameraData.viewSize.y * _offset.z)
				{
					return true;
				}
				if (cameraData.offsetPos.z + cameraData.viewSize.y * 0.5f > gameShowSize.z * 0.5f - cameraData.viewSize.y * _offset.z)
				{
					return true;
				}
			}
			if (cameraData.viewSize.x < gameShowSize.x)
			{
				if (cameraData.offsetPos.x - cameraData.viewSize.x * 0.5f < (0f - gameShowSize.x) * 0.5f)
				{
					return true;
				}
				if (cameraData.offsetPos.x + cameraData.viewSize.x * 0.5f > gameShowSize.x * 0.5f)
				{
					return true;
				}
			}
			return false;
		}
		private Vector3 GetCameraMoveLimitAreaPos(Vector3 _offset)
		{
			cameraData.offsetPos = cameraData.camera.transform.localPosition;
			if (cameraData.viewSize.y < gameShowSize.z)
			{
				if (cameraData.offsetPos.z - cameraData.viewSize.y * 0.5f < (0f - gameShowSize.z) * 0.5f - cameraData.viewSize.y * _offset.z)
				{
					cameraData.offsetPos.z = (0f - gameShowSize.z) * 0.5f + cameraData.viewSize.y * 0.5f - cameraData.viewSize.y * _offset.z;
				}
				else if (cameraData.offsetPos.z + cameraData.viewSize.y * 0.5f > gameShowSize.z * 0.5f - cameraData.viewSize.y * _offset.z)
				{
					cameraData.offsetPos.z = gameShowSize.z * 0.5f - cameraData.viewSize.y * 0.5f - cameraData.viewSize.y * _offset.z;
				}
			}
			if (cameraData.viewSize.x < gameShowSize.x)
			{
				if (cameraData.offsetPos.x - cameraData.viewSize.x * 0.5f < (0f - gameShowSize.x) * 0.5f - cameraData.viewSize.x * _offset.x)
				{
					cameraData.offsetPos.x = (0f - gameShowSize.x) * 0.5f + cameraData.viewSize.x * 0.5f - cameraData.viewSize.x * _offset.x;
				}
				else if (cameraData.offsetPos.x + cameraData.viewSize.x * 0.5f > gameShowSize.x * 0.5f - cameraData.viewSize.x * _offset.x)
				{
					cameraData.offsetPos.x = gameShowSize.x * 0.5f - cameraData.viewSize.x * 0.5f - cameraData.viewSize.x * _offset.x;
				}
			}
			return cameraData.offsetPos;
		}
		public bool CheckInPenaltyArea(CharacterScript _chara, bool _my = true)
		{
			int teamNo = _my ? _chara.TeamNo : ((_chara.TeamNo == 0) ? 1 : 0);
			Vector3 vector = ConvertLocalPos(_chara.GetPos(), teamNo);
			if (CalcManager.CheckRange(vector.x, (0f - fieldAnchorList.penaltyArea.size.x) * 0.5f, fieldAnchorList.penaltyArea.size.x * 0.5f, _include: false))
			{
				return CalcManager.CheckRange(vector.z, 0f, fieldAnchorList.penaltyArea.size.z, _include: false);
			}
			return false;
		}
		public bool CheckInPenaltyArea(Vector3 _pos, int _teamNo, bool _half = false)
		{
			Vector3 vector = ConvertLocalPos(_pos, _teamNo);
			if (CalcManager.CheckRange(vector.x, (0f - fieldAnchorList.penaltyArea.size.x) * 0.5f, fieldAnchorList.penaltyArea.size.x * 0.5f, _include: false))
			{
				return CalcManager.CheckRange(vector.z, 0f, fieldAnchorList.penaltyArea.size.z * (_half ? 0.5f : 1f), _include: false);
			}
			return false;
		}
		public int CheckGoalWhichTeam()
		{
			return 0;
		}
		public int CheckBallWhichTeamArea()
		{
			if (SingletonCustom<BallManager>.Instance.GetBallPos().z < GetAnchors().centerCircle.transform.position.z)
			{
				return 0;
			}
			return 1;
		}
		public DirType CheckLineOutDir(Vector3 _nowPos, Vector3 _prevPos, ref Vector3 _lineOutPos)
		{
			_lineOutPos = _nowPos;
			if (CalcManager.CheckRange(_lineOutPos.x, fieldAnchorList.left.position.x, fieldAnchorList.right.position.x) || !CalcManager.CheckRange(_lineOutPos.z, fieldAnchorList.front.position.z, fieldAnchorList.back.position.z))
			{
				_lineOutPos.x = Mathf.Min(Mathf.Max(_lineOutPos.x, fieldAnchorList.left.position.x), fieldAnchorList.right.position.x);
				if (_nowPos.z < fieldAnchorList.centerCircle.transform.position.z)
				{
					UnityEngine.Debug.Log("手前のラインを出た");
					_lineOutPos.z = fieldAnchorList.front.position.z - SingletonCustom<BallManager>.Instance.GetBallSize();
					return DirType.FRONT;
				}
				UnityEngine.Debug.Log("奥のラインを出た");
				_lineOutPos.z = fieldAnchorList.back.position.z + SingletonCustom<BallManager>.Instance.GetBallSize();
				return DirType.BACK;
			}
			if (CalcManager.CheckRange(_lineOutPos.z, fieldAnchorList.front.position.z, fieldAnchorList.back.position.z) || !CalcManager.CheckRange(_lineOutPos.x, fieldAnchorList.left.position.x, fieldAnchorList.right.position.x))
			{
				_lineOutPos.z = Mathf.Min(Mathf.Max(_lineOutPos.z, fieldAnchorList.front.position.z), fieldAnchorList.back.position.z);
				if (_lineOutPos.x > fieldAnchorList.centerCircle.transform.position.x)
				{
					UnityEngine.Debug.Log("右のラインを出た");
					_lineOutPos.x = fieldAnchorList.right.position.x + SingletonCustom<BallManager>.Instance.GetBallSize();
					return DirType.RIGHT;
				}
				UnityEngine.Debug.Log("左のラインを出た");
				_lineOutPos.x = fieldAnchorList.left.position.x - SingletonCustom<BallManager>.Instance.GetBallSize();
				return DirType.LEFT;
			}
			if (!CalcManager.CheckRange(_lineOutPos.z, fieldAnchorList.front.position.z, fieldAnchorList.back.position.z))
			{
				_lineOutPos.x = Mathf.Min(Mathf.Max(_lineOutPos.x, fieldAnchorList.left.position.x), fieldAnchorList.right.position.x);
				if (_nowPos.z < fieldAnchorList.centerCircle.transform.position.z)
				{
					UnityEngine.Debug.Log("手前のラインを出た");
					_lineOutPos.z = fieldAnchorList.front.position.z - SingletonCustom<BallManager>.Instance.GetBallSize();
					return DirType.FRONT;
				}
				UnityEngine.Debug.Log("奥のラインを出た");
				_lineOutPos.z = fieldAnchorList.back.position.z + SingletonCustom<BallManager>.Instance.GetBallSize();
				return DirType.BACK;
			}
			_lineOutPos.z = Mathf.Min(Mathf.Max(_lineOutPos.z, fieldAnchorList.front.position.z), fieldAnchorList.back.position.z);
			if (_lineOutPos.x > fieldAnchorList.centerCircle.transform.position.x)
			{
				UnityEngine.Debug.Log("右のラインを出た");
				_lineOutPos.x = fieldAnchorList.right.position.x + SingletonCustom<BallManager>.Instance.GetBallSize();
				return DirType.RIGHT;
			}
			UnityEngine.Debug.Log("左のラインを出た");
			_lineOutPos.x = fieldAnchorList.left.position.x - SingletonCustom<BallManager>.Instance.GetBallSize();
			return DirType.LEFT;
		}
		public Vector3 CheckCornerPos(Vector3 _lineOutPos, bool _ballPos = false)
		{
			Vector3 result = default(Vector3);
			if (_lineOutPos.z > GetAnchors().centerCircle.transform.position.z)
			{
				if (_lineOutPos.x > GetAnchors().centerCircle.transform.position.x)
				{
					result.x = GetAnchors().right.position.x;
					result.y = 0f;
					result.z = GetAnchors().back.position.z;
					if (_ballPos)
					{
						result.x -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
						result.z -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					}
					else
					{
						result.x += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
						result.z += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					}
				}
				else
				{
					result.x = GetAnchors().left.position.x;
					result.y = 0f;
					result.z = GetAnchors().back.position.z;
					if (_ballPos)
					{
						result.x += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
						result.z -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					}
					else
					{
						result.x -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
						result.z += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					}
				}
			}
			else if (_lineOutPos.x > GetAnchors().centerCircle.transform.position.x)
			{
				result.x = GetAnchors().right.position.x;
				result.y = 0f;
				result.z = GetAnchors().front.position.z;
				if (_ballPos)
				{
					result.x -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					result.z += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
				}
				else
				{
					result.x += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					result.z -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
				}
			}
			else
			{
				result.x = GetAnchors().left.position.x;
				result.y = 0f;
				result.z = GetAnchors().front.position.z;
				if (_ballPos)
				{
					result.x += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					result.z += SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
				}
				else
				{
					result.x -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
					result.z -= SingletonCustom<BallManager>.Instance.GetBall().BallSize() * 0.5f;
				}
			}
			return result;
		}
		public Vector3 ConvertLocalPos(Vector3 _pos, int _teamNo)
		{
			return teamAnchor[_teamNo].InverseTransformPoint(_pos);
		}
		public Vector3 ConvertWorldPos(Vector3 _pos, int _teamNo)
		{
			return teamAnchor[_teamNo].TransformPoint(_pos);
		}
		public Vector3 ConvertLocalPosPer(Vector3 _pos, int _teamNo)
		{
			Vector3 vector = teamAnchor[_teamNo].InverseTransformPoint(_pos);
			Vector3 result = default(Vector3);
			result.x = vector.x / fieldData.halfSize.x;
			result.z = vector.z / (fieldData.halfSize.z * 2f);
			result.y = 0f;
			return result;
		}
		public Vector3 ConvertPosPerToWorld(Vector3 _posPer, int _teamNo)
		{
			_posPer.x *= fieldData.halfSize.x;
			_posPer.z *= fieldData.halfSize.z * 2f;
			_posPer.y = 0f;
			return teamAnchor[_teamNo].TransformPoint(_posPer);
		}
		public void ReverseField()
		{
			isReverseField = !isReverseField;
			if (isReverseField)
			{
				UnityEngine.Debug.Log("反転する");
				floor.transform.SetLocalEulerAnglesY(180f);
				fieldObj.transform.SetLocalEulerAnglesY(180f);
				field.transform.SetLocalEulerAnglesY(180f);
				cameraData.rotationAnchor.SetEulerAnglesY(180f);
				desk.transform.SetLocalEulerAnglesY(270f);
			}
			else
			{
				UnityEngine.Debug.Log("もとに戻す");
				if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
				{
					cameraData.rotationAnchor.SetEulerAnglesY(0f);
				}
			}
			if (GameSaveData.GetSelectArea() == OUT_DISABLE_STAGE_NO && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
			{
				fieldObj.GetArrayTranslucentObject()[0].SetEnable(_isEnable: true);
				fieldObj.GetArrayTranslucentObject()[1].SetEnable(_isEnable: false);
				fieldObj.GetArrayTranslucentObject()[1].Reset();
				fieldObj.GetArrayTranslucentObject()[2].SetEnable(_isEnable: true);
				fieldObj.GetArrayTranslucentObject()[3].SetEnable(_isEnable: false);
				fieldObj.GetArrayTranslucentObject()[3].Reset();
			}
		}
		public void StartCameraReverseAnimation(float _time)
		{
			if (!GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.HORIZONTAL))
			{
				LeanTween.rotateY(cameraData.rotationAnchor.gameObject, 0f, _time).setEaseOutCubic().setOnComplete((Action)delegate
				{
					cameraData.rotationAnchor.SetEulerAnglesY(0f);
				});
			}
		}
		public Vector3 GetGoalKickPos(int _teamNo, bool _ballPos = false)
		{
			Vector3 vector = CalcManager.mVector3Zero;
			if (_ballPos)
			{
				vector.z = fieldData.goalAreaSize.z;
			}
			else
			{
				vector.z = fieldData.goalAreaSize.z * 0.5f;
			}
			vector = GetAnchors().goalArea[_teamNo].transform.TransformPoint(vector);
			return vector;
		}
		public Transform GetObjAnchor()
		{
			return objAnchor;
		}
		public Transform GetFormationAnchor(int _teamNo, int _no)
		{
			return formationAnchorList[_teamNo].anchor[_no];
		}
		public Vector3 GetFormationPos(int _teamNo, int _no)
		{
			return formationAnchorList[_teamNo].anchor[_no].position;
		}
		public FieldAnchorList GetAnchors()
		{
			return fieldAnchorList;
		}
		public Transform GetMyGoal(int _teamNo)
		{
			return fieldAnchorList.goalArea[_teamNo].transform;
		}
		public Transform GetOpponentGoal(int _teamNo)
		{
			return fieldAnchorList.goalArea[(_teamNo == 0) ? 1 : 0].transform;
		}
		public FieldData GetFieldData()
		{
			return fieldData;
		}
		public float GetCameraHeight()
		{
			return cameraData.camera.transform.position.y - objAnchor.transform.position.y;
		}
		public Camera Get3dCamera()
		{
			return cameraData.camera;
		}
		public StageData GetStageData()
		{
			return fieldObj;
		}
	}
}
