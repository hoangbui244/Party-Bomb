using System;
using UnityEngine;
public class RoadRaceCameraManager : SingletonCustom<RoadRaceCameraManager>
{
	public enum DirType
	{
		LEFT,
		RIGHT,
		FRONT,
		BACK
	}
	public enum CameraPosType
	{
		START,
		GOAL,
		RUN_0,
		RUN_1,
		MAX
	}
	[Serializable]
	public struct CameraPosData
	{
		[SerializeField]
		private Vector3 offsetPos;
		public Vector3 OffsetPos => offsetPos;
	}
	[Serializable]
	public struct CameraData
	{
		public Camera camera;
		public Transform moveAnchor;
		public Transform root;
		public Vector3 offset;
		public float rot;
		public Vector3 lookPos;
		public float rotOffset;
		public Vector3 lookPosOffset;
		public bool isRearCamera;
		public CameraPosType posTypePrev;
		public CameraPosType posType;
		public Camera GetCamera(int _no = 0)
		{
			return camera;
		}
	}
	private Rect[][] cameraRect = new Rect[4][]
	{
		new Rect[1]
		{
			new Rect(0f, 0f, 1f, 1f)
		},
		new Rect[2]
		{
			new Rect(0f, 0f, 0.5f, 1f),
			new Rect(0.5f, 0f, 1f, 1f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		}
	};
	private Vector3 CAMERA_MOVE_SPEED = new Vector3(30f, 15f, 30f);
	private float CAMERA_ROT_OFFSET_SPEED = 2.5f;
	private float CAMERA_ROT_OFFSET = 10f;
	private float OFFSET_MOVE_SPEED = 5f;
	private Vector3 CAMERA_LOOK_SPEED = new Vector3(1f, 30f, 50f);
	private Vector3 CAMERA_LOOK_OFFSET = new Vector3(0f, 0.4f, 0f);
	private float CAMERA_ROT_SPEED = 5f;
	private Vector3 CAMERA_MOVE_SPEED_SET_PLAY = new Vector3(2.5f, 2f, 2.5f);
	private Vector3 calcVec3;
	private Vector3 gameShowSize = new Vector3(17f, 0f, 32f);
	[SerializeField]
	[Header("カメラ移動デ\u30fcタ")]
	private CameraPosData[] posData;
	private float CAMERA_DEF_Y = 30f;
	[SerializeField]
	[Header("カメラデ\u30fcタ")]
	private CameraData[] cameradData;
	private float[] cameraChangeInterval = new float[4];
	[SerializeField]
	[Header("オブジェクトアンカ\u30fc")]
	private Transform objAnchor;
	private RaycastHit raycastHit;
	private RoadRaceCharacterScript[] targetChara;
	public Vector3 GetCameraOffsetPos(CameraPosType _type)
	{
		return posData[(int)_type].OffsetPos;
	}
	public void Init(RoadRaceCharacterScript[] _charas)
	{
		targetChara = _charas;
		UnityEngine.Debug.Log("PlayerNum = " + SingletonCustom<GameSettingManager>.Instance.PlayerNum.ToString());
		int num = SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1;
		for (int i = 0; i < cameradData.Length; i++)
		{
			cameradData[i].root.gameObject.SetActive(i < cameraRect[num].Length);
			if (cameradData[i].root.gameObject.activeSelf)
			{
				UnityEngine.Debug.Log("カメラ設定 : " + i.ToString());
				cameradData[i].GetCamera().rect = cameraRect[num][i];
				Vector3 normalized = (targetChara[i].GetPos() - Scene_RoadRace.FM.StageData.transform.position).normalized;
				normalized.x = 0f - Mathf.Sign(normalized.x);
				normalized.z = 0f - Mathf.Sign(normalized.z);
				cameradData[i].moveAnchor.SetPosition(Scene_RoadRace.FM.StageData.transform.position.x + normalized.x * GetCameraOffsetPos(CameraPosType.START).x, Scene_RoadRace.FM.CharacterAnchor.position.y + GetCameraOffsetPos(CameraPosType.START).y, Scene_RoadRace.FM.StageData.transform.position.z + normalized.z * GetCameraOffsetPos(CameraPosType.START).z);
				cameradData[i].GetCamera().transform.LookAt(Scene_RoadRace.FM.CharacterAnchor.position);
				cameradData[i].posType = CameraPosType.RUN_0;
			}
		}
		for (int j = 0; j < cameraChangeInterval.Length; j++)
		{
			cameraChangeInterval[j] = 0f;
		}
	}
	public void FixedUpdateMethod()
	{
		if (Scene_RoadRace.CM.PlayerNum == 1)
		{
			UpdateCameraWorkSingle();
		}
		else
		{
			UpdateCameraWorkMulti();
		}
	}
	private void UpdateCameraWorkSingle()
	{
		for (int i = 0; i < cameradData.Length; i++)
		{
			if (!cameradData[i].root.gameObject.activeSelf)
			{
				continue;
			}
			if (Scene_RoadRace.GM.GetRaceState() != RoadRaceGameManager.RaceState.Game)
			{
				cameraChangeInterval[i] -= Time.fixedDeltaTime;
				if (cameraChangeInterval[i] <= 0f)
				{
					cameradData[i].offset = GetCameraOffsetPos(CameraPosType.GOAL);
					cameradData[i].lookPos = targetChara[i].GetPos();
					MoveCamera(i, 1, targetChara[i], _offsetLerp: true, cameradData[i].offset, targetChara[i].transform.rotation.eulerAngles.y, _moveLerp: true, CAMERA_MOVE_SPEED_SET_PLAY, _lookLerp: false, Vector3.zero, CAMERA_LOOK_OFFSET);
				}
				continue;
			}
			cameraChangeInterval[i] = 1f;
			cameradData[i].lookPosOffset = CAMERA_LOOK_OFFSET;
			if (targetChara[i].IsGoal)
			{
				CalcManager.mCalcVector3 = GetCameraOffsetPos(CameraPosType.GOAL);
			}
			else
			{
				CalcManager.mCalcVector3 = GetCameraOffsetPos(cameradData[i].posType);
				if (cameradData[i].posType == CameraPosType.RUN_1)
				{
					cameradData[i].lookPosOffset += Vector3.up * 0.1f;
				}
				else
				{
					CalcManager.mCalcVector3.z *= Mathf.Clamp(1f + targetChara[i].GetOverSpeedValue() * 0.1f, 1f, 1.5f);
				}
			}
			if (targetChara[i].transform.forward.y > 0.2f)
			{
				CalcManager.mCalcVector3.y += targetChara[i].GetCharaHeight() * 1f;
			}
			cameradData[i].offset = Vector3.Lerp(cameradData[i].offset, CalcManager.mCalcVector3, OFFSET_MOVE_SPEED * Time.fixedDeltaTime);
			CalcManager.mCalcVector3 = cameradData[i].offset;
			if (targetChara[i].IsRearLook)
			{
				CalcManager.mCalcVector3.z *= -1f;
			}
			Vector3 eulerAngles = targetChara[i].transform.rotation.eulerAngles;
			targetChara[i].transform.RotateAround(targetChara[i].transform.position, targetChara[i].transform.right, 0f - eulerAngles.x);
			targetChara[i].transform.RotateAround(targetChara[i].transform.position, targetChara[i].transform.forward, 0f - eulerAngles.z);
			float num = targetChara[i].transform.rotation.eulerAngles.y;
			targetChara[i].transform.RotateAround(targetChara[i].transform.position, targetChara[i].transform.right, eulerAngles.x);
			targetChara[i].transform.RotateAround(targetChara[i].transform.position, targetChara[i].transform.forward, eulerAngles.z);
			if (targetChara[i].IsGoal)
			{
				num += 140f;
			}
			bool flag = true;
			if (cameradData[i].isRearCamera != targetChara[i].IsRearLook)
			{
				cameradData[i].isRearCamera = targetChara[i].IsRearLook;
				flag = false;
			}
			MoveCamera(i, 2, targetChara[i], flag, CalcManager.mCalcVector3, num, flag, CAMERA_MOVE_SPEED, flag, CAMERA_LOOK_SPEED, cameradData[i].lookPosOffset);
		}
	}
	private void UpdateCameraWorkMulti()
	{
		UpdateCameraWorkSingle();
	}
	private void MoveCamera(int _no, int _type, RoadRaceCharacterScript _chara, bool _offsetLerp, Vector3 _offset, float _rot, bool _moveLerp, Vector3 _moveSpeed, bool _lookLerp, Vector3 _lookSpeed, Vector3 _lookPosOffset)
	{
		if (_offsetLerp)
		{
			cameradData[_no].rot = Mathf.LerpAngle(cameradData[_no].rot, _rot, CAMERA_ROT_SPEED * Time.fixedDeltaTime);
		}
		else
		{
			cameradData[_no].rot = _rot;
		}
		Vector3 vector = CalcManager.PosRotation2D(_offset, Vector3.zero, cameradData[_no].rot, CalcManager.AXIS.Y);
		vector.x = 0f - vector.x;
		vector.z = 0f - vector.z;
		Vector3 vector2 = _chara.GetPos() + vector;
		if (Physics.Raycast(vector2, Vector3.down, out raycastHit, 3f, RoadRaceDefine.ConvertLayerMask("Collision_Obj_1")) && raycastHit.point.y > _chara.GetPos().y + _chara.GetCharaHeight() * 1.5f && Mathf.Abs(raycastHit.point.y - vector2.y) < vector.y)
		{
			vector2.y = raycastHit.point.y + vector.y;
			UnityEngine.Debug.Log("No." + _no.ToString() + " : 地面までの距離が近い");
		}
		Vector3 vector3 = _chara.GetPos() + Vector3.up * _chara.GetCharaHeight() * 0.5f;
		Vector3 normalized = (vector2 - vector3).normalized;
		if (Physics.Raycast(vector3, normalized, out raycastHit, CalcManager.Length(vector2, vector3), RoadRaceDefine.ConvertLayerMask("Collision_Obj_1") | RoadRaceDefine.ConvertLayerMask("Object")))
		{
			vector2 = vector3 + normalized * CalcManager.Length(raycastHit.point, vector3);
			vector2.y = _chara.GetPos().y + vector.y;
		}
		if (_moveLerp)
		{
			cameradData[_no].moveAnchor.SetPositionX(Mathf.Lerp(cameradData[_no].moveAnchor.position.x, vector2.x, _moveSpeed.x * Time.fixedDeltaTime));
			cameradData[_no].moveAnchor.SetPositionY(Mathf.Lerp(cameradData[_no].moveAnchor.position.y, vector2.y, _moveSpeed.y * Time.fixedDeltaTime));
			cameradData[_no].moveAnchor.SetPositionZ(Mathf.Lerp(cameradData[_no].moveAnchor.position.z, vector2.z, _moveSpeed.z * Time.fixedDeltaTime));
		}
		else
		{
			cameradData[_no].moveAnchor.position = vector2;
		}
		if (_lookLerp)
		{
			if (Scene_RoadRace.CM.PlayerNum == 2)
			{
				bool flag = false;
				float b = 0f;
				if (_chara.GetTiltBodyRot() >= 180f)
				{
					if (_chara.GetTiltBodyRot() <= 359f)
					{
						b = Mathf.Min((360f - _chara.GetTiltBodyRot()) / _chara.TILT_BODY_ROT.z, 1f) * CAMERA_ROT_OFFSET;
						flag = true;
					}
				}
				else if (_chara.GetTiltBodyRot() >= 1f)
				{
					b = (0f - Mathf.Min(_chara.GetTiltBodyRot() / _chara.TILT_BODY_ROT.z, 1f)) * CAMERA_ROT_OFFSET;
					flag = true;
				}
				if (flag)
				{
					cameradData[_no].rotOffset = Mathf.Lerp(cameradData[_no].rotOffset, b, CAMERA_ROT_OFFSET_SPEED * Time.fixedDeltaTime);
					if (_no == 0)
					{
						UnityEngine.Debug.Log("傾け値" + cameradData[_no].rotOffset.ToString());
					}
				}
				else
				{
					cameradData[_no].rotOffset = Mathf.Lerp(cameradData[_no].rotOffset, 0f, CAMERA_ROT_OFFSET_SPEED * Time.fixedDeltaTime);
				}
			}
			CalcManager.mCalcVector3 = (_chara.GetPos() + _lookPosOffset - cameradData[_no].GetCamera().transform.position).normalized;
			CalcManager.mCalcVector3 = Quaternion.LookRotation(CalcManager.mCalcVector3).eulerAngles;
			CalcManager.mCalcVector3.y += cameradData[_no].rotOffset;
			CalcManager.mCalcVector3.x = Mathf.LerpAngle(cameradData[_no].GetCamera().transform.rotation.eulerAngles.x, CalcManager.mCalcVector3.x, _lookSpeed.x * Time.fixedDeltaTime);
			CalcManager.mCalcVector3.y = Mathf.LerpAngle(cameradData[_no].GetCamera().transform.rotation.eulerAngles.y, CalcManager.mCalcVector3.y, _lookSpeed.y * Time.fixedDeltaTime);
			CalcManager.mCalcVector3.z = Mathf.LerpAngle(cameradData[_no].GetCamera().transform.rotation.eulerAngles.z, CalcManager.mCalcVector3.z, _lookSpeed.z * Time.fixedDeltaTime);
			cameradData[_no].GetCamera().transform.rotation = Quaternion.Euler(CalcManager.mCalcVector3);
		}
		else
		{
			cameradData[_no].GetCamera().transform.LookAt(_chara.GetPos() + _lookPosOffset);
		}
	}
	public void CngCameraPosType(int _no)
	{
		switch (cameradData[_no].posType)
		{
		case CameraPosType.RUN_0:
			cameradData[_no].posType = CameraPosType.RUN_1;
			break;
		case CameraPosType.RUN_1:
			cameradData[_no].posType = CameraPosType.RUN_0;
			break;
		}
	}
	public float GetCameraHeight(int _no = 0)
	{
		return cameradData[_no].GetCamera().transform.position.y - objAnchor.transform.position.y;
	}
	public Camera Get3dCamera(int _no = 0)
	{
		return cameradData[_no].GetCamera();
	}
}
