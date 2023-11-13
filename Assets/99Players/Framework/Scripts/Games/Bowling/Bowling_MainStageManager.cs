using System;
using System.Collections.Generic;
using UnityEngine;
public class Bowling_MainStageManager : SingletonCustom<Bowling_MainStageManager>
{
	[Serializable]
	public struct LaneCollider
	{
		[Header("レ\u30fcン")]
		public BoxCollider center;
		[Header("アプロ\u30fcチ")]
		public BoxCollider approach;
	}
	private enum PinChangeMatPattern
	{
		HorizontalLine,
		VerticalLine,
		Gathers
	}
	[SerializeField]
	[Header("ピンのオブジェクト")]
	private Bowling_Pin pinObject;
	[SerializeField]
	[Header("ボ\u30fcルの開始地点アンカ\u30fc")]
	private Transform ballStartPointAnchor;
	[SerializeField]
	[Header("ピンル\u30fcトアンカ\u30fc")]
	private Transform pinRootAnchor;
	[SerializeField]
	[Header("ピンアンカ\u30fcリスト")]
	private Transform[] pinAnchorList;
	[SerializeField]
	[Header("ピンのマテリアル")]
	private Material[] pinMaterials;
	[SerializeField]
	[Header("3D空間用カメラRootアンカ\u30fc")]
	private Transform stageCameraRoot;
	[SerializeField]
	[Header("3D空間用カメラ")]
	private Camera stageCamera;
	[SerializeField]
	[Header("カメラ移動X角度")]
	private float cameraMoveRotX = -1f;
	[SerializeField]
	[Header("レ\u30fcンコライダ\u30fc")]
	private LaneCollider laneCollider;
	[SerializeField]
	[Header("カメラ停止場所オフセット")]
	private float cameraStopOffsetCorr = 1.75f;
	private Bowling_Pin[] pinList;
	private bool[] isPinFall;
	private Vector3[] pinDefLocalPos;
	private Vector3 cameraPosDef;
	private float cameraOffset;
	private Vector3 cameraMoveStartPos;
	private float cameraMoveStartRotX;
	private Vector3 cameraMovePos;
	private Vector3 laneEndPos;
	private const float CAMERA_MOVE_INTERVAL = 0f;
	private float cameraMoveInterval;
	private bool isCameraMove;
	private Vector3 ballPosWhenCameraMove;
	private List<int> pinMatIdList = new List<int>();
	private PinChangeMatPattern pinMatPattern;
	public Transform BallStartPointAnchor => ballStartPointAnchor;
	public Bowling_Pin[] PinList => pinList;
	public Camera StageCamera => stageCamera;
	public Vector3 LaneEndPos => laneEndPos;
	public Vector3 GetLaneSize => laneCollider.center.size;
	public Vector3 GetLanePos => laneCollider.center.transform.position;
	public Vector3 GetApproachSize => laneCollider.approach.size;
	public Transform GetCameraRoot => stageCameraRoot;
	public void Init()
	{
		cameraMoveInterval = 0f;
		pinList = new Bowling_Pin[pinAnchorList.Length];
		for (int i = 0; i < pinList.Length; i++)
		{
			pinList[i] = UnityEngine.Object.Instantiate(pinObject, pinAnchorList[i].position, Quaternion.identity, pinRootAnchor);
			GameObject gameObject = pinList[i].gameObject;
			gameObject.name = gameObject.name + "_" + i.ToString();
			pinList[i].Init();
		}
		isPinFall = new bool[pinAnchorList.Length];
		pinDefLocalPos = new Vector3[pinList.Length];
		for (int j = 0; j < pinDefLocalPos.Length; j++)
		{
			pinDefLocalPos[j] = pinList[j].transform.localPosition;
		}
		laneEndPos = laneCollider.center.transform.position;
		laneEndPos.z += laneCollider.center.size.z * 0.5f;
		laneEndPos.y += laneCollider.center.size.y * 0.5f;
		cameraOffset = stageCamera.transform.position.z - ballStartPointAnchor.position.z;
		cameraPosDef = stageCameraRoot.position;
		cameraMovePos = cameraPosDef;
		if (cameraStopOffsetCorr < 0f)
		{
			cameraMovePos.z = stageCameraRoot.position.z;
		}
		else
		{
			cameraMovePos.z = laneEndPos.z + cameraOffset * cameraStopOffsetCorr;
		}
		if (cameraMoveRotX < 0f)
		{
			cameraMoveRotX = stageCamera.transform.localEulerAngles.x;
		}
		for (int k = 0; k < pinMaterials.Length; k++)
		{
			pinMatIdList.Add(k);
		}
		SetPinMaterial();
	}
	public void UpdateMethod()
	{
		CheckPinStop();
	}
	public void GameStartProcess()
	{
	}
	public void GameEndProcess()
	{
	}
	public void SetStageCameraPosX(float _pos)
	{
		stageCameraRoot.SetPositionX(_pos);
	}
	public void SetCameraMoveStartData()
	{
		cameraMoveStartPos = stageCameraRoot.position;
		cameraMoveStartRotX = stageCamera.transform.localEulerAngles.x;
	}
	public void MoveCameraWork()
	{
		if (isCameraMove)
		{
			float value = Mathf.InverseLerp(ballPosWhenCameraMove.z, ballPosWhenCameraMove.z + (GetLaneSize.z - Mathf.Abs(ballStartPointAnchor.position.z - ballPosWhenCameraMove.z)), Bowling_Define.MPM.GetNowThrowUserBall().GetBallPos().z);
			float x = Mathf.LerpAngle(cameraMoveStartRotX, cameraMoveRotX, Mathf.Clamp(Mathf.InverseLerp(ballPosWhenCameraMove.z, ballPosWhenCameraMove.z + (GetLaneSize.z - Mathf.Abs(ballStartPointAnchor.transform.position.z - ballPosWhenCameraMove.z)), Bowling_Define.MPM.GetNowThrowUserBall().GetBallPos().z), 0f, 1f));
			stageCameraRoot.position = Vector3.Lerp(cameraMoveStartPos, cameraMovePos, Mathf.Clamp(value, 0f, 1f));
			stageCamera.transform.SetLocalEulerAnglesX(x);
			return;
		}
		cameraMoveInterval -= Time.deltaTime;
		if (cameraMoveInterval <= 0f)
		{
			ballPosWhenCameraMove = Bowling_Define.MPM.GetNowThrowUserBall().GetBallPos();
			isCameraMove = true;
		}
	}
	public void ResetCameraPos(bool _animationMove = false, float _aniTime = 1f)
	{
		cameraMoveInterval = 0f;
		isCameraMove = false;
		if (_animationMove)
		{
			LeanTween.move(stageCameraRoot.gameObject, cameraPosDef, _aniTime).setEaseOutCubic();
			LeanTween.rotateX(stageCamera.gameObject, cameraMoveStartRotX, _aniTime).setEaseOutCubic();
		}
		else
		{
			stageCameraRoot.SetPosition(cameraPosDef.x, cameraPosDef.y, cameraPosDef.z);
			stageCamera.transform.SetLocalEulerAnglesX(cameraMoveStartRotX);
		}
	}
	public void PinResetPos(bool _all = true)
	{
		if (_all)
		{
			for (int i = 0; i < isPinFall.Length; i++)
			{
				isPinFall[i] = false;
			}
			Bowling_Define.MSM.SetPinMaterial();
		}
		for (int j = 0; j < pinList.Length; j++)
		{
			if (_all || !isPinFall[j])
			{
				pinList[j].ResetPos(pinRootAnchor);
			}
			else
			{
				pinList[j].Hide();
			}
		}
	}
	public void CheckPinStop()
	{
		for (int i = 0; i < pinList.Length; i++)
		{
			pinList[i].CheckFall();
			pinList[i].CheckMoveStop();
		}
	}
	public bool IsPinAllStop()
	{
		for (int i = 0; i < pinList.Length; i++)
		{
			if (!pinList[i].IsStop() && pinList[i].gameObject.activeSelf)
			{
				return false;
			}
		}
		return true;
	}
	public void CheckPinFall(bool _gutter)
	{
		if (!_gutter)
		{
			for (int i = 0; i < isPinFall.Length; i++)
			{
				isPinFall[i] = pinList[i].IsFall;
			}
		}
	}
	public bool IsPinAllFall()
	{
		for (int i = 0; i < isPinFall.Length; i++)
		{
			if (!isPinFall[i])
			{
				return false;
			}
		}
		return true;
	}
	public bool[] IsPinStand()
	{
		bool[] array = new bool[isPinFall.Length];
		for (int i = 0; i < isPinFall.Length; i++)
		{
			array[i] = !isPinFall[i];
		}
		return array;
	}
	public int GetPinFallNum()
	{
		int num = 0;
		for (int i = 0; i < isPinFall.Length; i++)
		{
			if (isPinFall[i])
			{
				num++;
			}
		}
		return num;
	}
	public void SetPinMaterial()
	{
		if (pinMatPattern == PinChangeMatPattern.HorizontalLine)
		{
			pinMatPattern = ((UnityEngine.Random.Range(0, 2) == 0) ? PinChangeMatPattern.VerticalLine : PinChangeMatPattern.Gathers);
		}
		else if (pinMatPattern == PinChangeMatPattern.VerticalLine)
		{
			pinMatPattern = ((UnityEngine.Random.Range(0, 2) != 0) ? PinChangeMatPattern.Gathers : PinChangeMatPattern.HorizontalLine);
		}
		else if (pinMatPattern == PinChangeMatPattern.Gathers)
		{
			pinMatPattern = ((UnityEngine.Random.Range(0, 2) != 0) ? PinChangeMatPattern.VerticalLine : PinChangeMatPattern.HorizontalLine);
		}
		pinMatIdList.Shuffle();
		for (int i = 0; i < pinList.Length; i++)
		{
			if (pinMatPattern == PinChangeMatPattern.HorizontalLine)
			{
				switch (i)
				{
				case 0:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[0]]);
					continue;
				case 1:
				case 2:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[1]]);
					continue;
				}
				if (i >= 3 && i <= 5)
				{
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[2]]);
				}
				else if (i >= 6 && i <= 9)
				{
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[3]]);
				}
			}
			else if (pinMatPattern == PinChangeMatPattern.VerticalLine)
			{
				switch (i)
				{
				case 0:
				case 4:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[0]]);
					break;
				case 1:
				case 2:
				case 7:
				case 8:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[1]]);
					break;
				case 3:
				case 5:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[2]]);
					break;
				case 6:
				case 9:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[3]]);
					break;
				}
			}
			else if (pinMatPattern == PinChangeMatPattern.Gathers)
			{
				switch (i)
				{
				case 0:
				case 4:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[0]]);
					break;
				case 1:
				case 3:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[1]]);
					break;
				case 2:
				case 5:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[2]]);
					break;
				case 6:
				case 7:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[3]]);
					break;
				case 8:
				case 9:
					pinList[i].SetPinMaterial(pinMaterials[pinMatIdList[4]]);
					break;
				}
			}
		}
	}
}
