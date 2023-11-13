using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
public class Scuba_CharacterManager : SingletonCustom<Scuba_CharacterManager>
{
	public class PictureViewPointData
	{
		public enum ColorType
		{
			Purple,
			Red,
			Orange,
			Yellow,
			Green,
			Blue
		}
		public Vector3 viewport;
		public int point;
		public ColorType colorType;
		public PictureViewPointData(Vector3 _viewport, int _point)
		{
			viewport = _viewport;
			point = _point;
			colorType = Point2ColorType(_point);
		}
		public ColorType Point2ColorType(int _point)
		{
			if (_point > 200)
			{
				return ColorType.Purple;
			}
			if (_point > 150)
			{
				return ColorType.Red;
			}
			if (_point > 100)
			{
				return ColorType.Orange;
			}
			if (_point > 50)
			{
				return ColorType.Yellow;
			}
			if (_point > 20)
			{
				return ColorType.Green;
			}
			return ColorType.Blue;
		}
		public int GetColorTypeIndex()
		{
			return (int)colorType;
		}
		public bool GetIsRanbow()
		{
			return colorType == ColorType.Purple;
		}
	}
	public const int CHARACTER_NUM = 4;
	private const float CAMERA_SPHERECAST_RADIUS = 0.05f;
	private const float CAMERA_DISABLE_CHARA_DISTANCE = 0.5f;
	private const float CAMERA_DISABLE_CHARA_SQR_DISTANCE = 0.25f;
	private const float START_CHARA_VELOCITY_Y = -1f;
	private int playerNum = 1;
	[SerializeField]
	private GameObject charaPrefab;
	[SerializeField]
	private Transform characterAnchor;
	[SerializeField]
	private Transform[] createAnchor;
	[SerializeField]
	private Transform freeLookCameraAnchor;
	[SerializeField]
	private FreeLookCam[] freeLookCam;
	[SerializeField]
	private Camera[] tpsCamera;
	[SerializeField]
	private Camera[] startCamera;
	[SerializeField]
	private RenderTexture[] renderTextures;
	[SerializeField]
	private Material[] cameraViewMaterials;
	private bool[] isCameraMaskChanges = new bool[4];
	private bool[] isCameraFps = new bool[4];
	private bool[] isCameraViewFrames = new bool[4];
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
		new Rect[3]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		}
	};
	private bool[] isAutoCamera = new bool[4];
	private PictureViewPointData[][] pictureViewPointDatasArray = new PictureViewPointData[4][];
	[SerializeField]
	private Material[] cameraModelMaterials;
	[SerializeField]
	private CharacterStyle[] startCharaStyles;
	[SerializeField]
	private MeshRenderer[] startCharaCameraModels;
	private Scuba_CharacterScript[] charas;
	public int PlayerNum
	{
		get
		{
			return playerNum;
		}
		set
		{
			playerNum = value;
		}
	}
	public void Init()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		charas = new Scuba_CharacterScript[4];
		int num = 0;
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i] = UnityEngine.Object.Instantiate(charaPrefab, createAnchor[i].transform.position, Quaternion.identity, characterAnchor).GetComponent<Scuba_CharacterScript>();
			charas[i].gameObject.SetActive(value: true);
			charas[i].Init(i);
			Vector3 forward = createAnchor[i].transform.forward;
			float num2 = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
			if (i < playerNum)
			{
				charas[i].IsPlayer = true;
				charas[i].PlayerNo = playerGroupList[i][0];
				charas[i].StyleCharaNo = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerGroupList[i][0]];
				charas[i].SetCharaGameStyle(playerGroupList[i][0]);
				startCharaStyles[i].SetGameStyle(GS_Define.GameType.BLOCK_WIPER, playerGroupList[i][0]);
				charas[i].SetCameraModelMaterial(cameraModelMaterials[charas[i].StyleCharaNo]);
				startCharaCameraModels[i].sharedMaterial = cameraModelMaterials[charas[i].StyleCharaNo];
				charas[i].SettingStartRotation(num2);
				charas[i].SettingActionMarkSprite();
				freeLookCam[i].SetPlayerNo(i);
				freeLookCam[i].SetTarget(charas[i].GetCameraTarget());
				charas[i].SetCameraRotAnchor(freeLookCam[i].transform);
				freeLookCam[i].Init(num2);
				freeLookCam[i].gameObject.SetActive(value: true);
				if (playerNum == 1)
				{
					tpsCamera[0].rect = cameraRect[0][0];
					startCamera[i].enabled = true;
					startCamera[i].rect = cameraRect[0][0];
				}
				else
				{
					tpsCamera[i].rect = cameraRect[3][i];
					startCamera[i].enabled = true;
					startCamera[i].rect = cameraRect[3][i];
				}
				if (playerNum == 1)
				{
					freeLookCam[0].SetIsCanButtonCtrl(_value: false);
				}
				isCameraViewFrames[i] = true;
				ChangeCameraPerson(i, _isFps: true);
			}
			else
			{
				charas[i].IsPlayer = false;
				charas[i].PlayerNo = 4 + num;
				charas[i].StyleCharaNo = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[charas[i].PlayerNo];
				charas[i].SetCharaGameStyle(charas[i].PlayerNo);
				startCharaStyles[i].SetGameStyle(GS_Define.GameType.BLOCK_WIPER, charas[i].PlayerNo);
				charas[i].SetCameraModelMaterial(cameraModelMaterials[charas[i].StyleCharaNo]);
				startCharaCameraModels[i].sharedMaterial = cameraModelMaterials[charas[i].StyleCharaNo];
				charas[i].SettingCpuStrength();
				charas[i].SettingStartRotation(num2);
				if (CheckViewCpuCarNo())
				{
					charas[i].IsCpuCamera = true;
					freeLookCam[i].enabled = false;
					freeLookCam[i].gameObject.SetActive(value: true);
					freeLookCam[i].transform.position = charas[i].GetCameraTarget().position;
					freeLookCam[i].transform.SetEulerAnglesY(charas[i].transform.eulerAngles.y);
					freeLookCam[i].SetPlayerNo(0);
					tpsCamera[i].rect = cameraRect[3][i];
					startCamera[i].enabled = true;
					startCamera[i].rect = cameraRect[3][i];
					isCameraViewFrames[i] = true;
					ChangeCameraPerson(i, _isFps: true);
				}
				else
				{
					freeLookCam[i].gameObject.SetActive(value: false);
					startCamera[i].enabled = false;
				}
				num++;
			}
			charas[i].SettingCtrlDirAnchor();
			charas[i].SetCameraViewMaterial(cameraViewMaterials[charas[i].StyleCharaNo]);
		}
		SetCharaMoveActive(_active: false);
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<Scuba_GameManager>.Instance.IsGameStart)
		{
			for (int i = 0; i < freeLookCam.Length; i++)
			{
				if (charas[i].IsPlayer)
				{
					charas[i].UpdateObjRotAnchor();
				}
				if (charas[i].IsCameraChara)
				{
					CameraUpdate(i);
				}
			}
			return;
		}
		if (SingletonCustom<Scuba_GameManager>.Instance.IsGameEnd)
		{
			for (int j = 0; j < charas.Length; j++)
			{
				charas[j].Stay();
			}
			return;
		}
		for (int k = 0; k < charas.Length; k++)
		{
			bool flag = true;
			if (charas[k].IsPlayer)
			{
				PlayerOperation(k);
				flag = false;
			}
			if (flag && !SingletonCustom<Scuba_GameManager>.Instance.IsGameEnd)
			{
				charas[k].AiMove();
			}
			charas[k].UpdateMethod();
			if (charas[k].IsCameraChara)
			{
				CameraUpdate(k);
			}
		}
	}
	private void CameraUpdate(int i)
	{
		if (charas[i].IsCpuCamera || (charas[i].IsPlayer && isAutoCamera[i]))
		{
			if (!charas[i].IsPlayer)
			{
				freeLookCam[i].transform.position = charas[i].GetCameraTarget().position;
			}
			float y = freeLookCam[i].transform.eulerAngles.y;
			Vector3 lhs = Quaternion.Euler(0f, y, 0f) * Vector3.forward;
			Vector3 dir = charas[i].GetDir();
			float num = Vector3.Dot(lhs, dir);
			Vector3 vector = Vector3.Cross(lhs, dir);
			float num2 = Mathf.Clamp01(num * -1f + 1f);
			if (!charas[i].IsPlayer)
			{
				TransformExtension.SetEulerAnglesY(y: (!(vector.y > 0f)) ? (y - 180f * num2 * Time.deltaTime) : (y + 180f * num2 * Time.deltaTime), transform: freeLookCam[i].transform);
			}
			else if (vector.y > 0f)
			{
				freeLookCam[i].SetLookAngle(freeLookCam[i].GetLookAngle() + 180f * num2 * Time.deltaTime);
			}
			else
			{
				freeLookCam[i].SetLookAngle(freeLookCam[i].GetLookAngle() - 180f * num2 * Time.deltaTime);
			}
		}
		Vector3 position = charas[i].GetCameraTarget().position;
		if (isCameraFps[i])
		{
			tpsCamera[i].transform.position = position;
			return;
		}
		tpsCamera[i].transform.localPosition = new Vector3(0f, 0f, -2f);
		Vector3 direction = tpsCamera[i].transform.position - position;
		if (Physics.SphereCast(position, 0.05f, direction, out RaycastHit hitInfo, direction.magnitude, 1048576))
		{
			tpsCamera[i].transform.position = hitInfo.point + hitInfo.normal * 0.05f;
			if (isCameraMaskChanges[i] == (tpsCamera[i].transform.position - position).sqrMagnitude > 0.25f)
			{
				SetCameraCullingMaskBit(i, isCameraMaskChanges[i], GetCharaLayerNo(i));
			}
		}
		else if (isCameraMaskChanges[i])
		{
			SetCameraCullingMaskBit(i, _isBit: true, GetCharaLayerNo(i));
		}
	}
	private void ChangeCameraPerson(int _charaNo, bool _isFps)
	{
		isCameraFps[_charaNo] = _isFps;
		if (_isFps)
		{
			SetCameraCullingMaskBit(_charaNo, _isBit: false, GetCharaLayerNo(_charaNo));
		}
		else
		{
			SetCameraCullingMaskBit(_charaNo, _isBit: true, GetCharaLayerNo(_charaNo));
		}
		if (charas[_charaNo].IsCameraChara)
		{
			SingletonCustom<Scuba_UiManager>.Instance.SetCameraFrameView(_charaNo, _isFps);
		}
	}
	public void StartChara()
	{
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].SetVelocityY(-1f);
			if (charas[i].IsCameraChara)
			{
				SingletonCustom<Scuba_UiManager>.Instance.SetCameraFrameView(i, _active: true);
			}
		}
		SingletonCustom<Scuba_ItemManager>.Instance.IsUpdateStart = true;
		StartCoroutine(_StartChara());
	}
	private IEnumerator _StartChara()
	{
		yield return new WaitForSeconds(0.2f);
		while (!SingletonCustom<Scuba_GameManager>.Instance.IsGameStart)
		{
			for (int i = 0; i < charas.Length; i++)
			{
				charas[i].Stay();
			}
			yield return null;
		}
	}
	public void EndChara()
	{
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].MarkObjEnd();
		}
	}
	public void EndCamera()
	{
		for (int i = 0; i < tpsCamera.Length; i++)
		{
			tpsCamera[i].gameObject.SetActive(value: false);
		}
	}
	private void PlayerOperation(int _playerNo)
	{
		if (!charas[_playerNo].GetRigid().isKinematic)
		{
			if (SingletonCustom<Scuba_ControllerManager>.Instance.GetCameraFrameViewButtonDown(_playerNo) && isCameraFps[_playerNo])
			{
				isCameraViewFrames[_playerNo] = !isCameraViewFrames[_playerNo];
				SingletonCustom<Scuba_UiManager>.Instance.SetCameraFrameView(0, isCameraViewFrames[_playerNo]);
			}
			if (SingletonCustom<Scuba_ControllerManager>.Instance.GetCameraAngleLeftButtonDown(_playerNo))
			{
				ChangeCameraPerson(_playerNo, !isCameraFps[_playerNo]);
			}
			if (SingletonCustom<Scuba_ControllerManager>.Instance.GetTakePictureButtonDown(_playerNo))
			{
				TakePicture(_playerNo);
			}
			CharaMove(_playerNo);
		}
	}
	private void CharaMove(int _no)
	{
		Vector3 moveDir = SingletonCustom<Scuba_ControllerManager>.Instance.GetMoveDir(_no);
		int moveTypeNum = SingletonCustom<Scuba_ControllerManager>.Instance.GetMoveTypeNum(_no);
		float moveSpeed = 0f;
		switch (moveTypeNum)
		{
		case 1:
			moveSpeed = 0.25f;
			break;
		case 2:
			moveSpeed = 0.5f;
			break;
		case 3:
			moveSpeed = 0.9f;
			break;
		}
		charas[_no].Move(moveDir, moveSpeed);
		bool flag = false;
		if (SingletonCustom<Scuba_ControllerManager>.Instance.GetRiseButton(_no))
		{
			flag = true;
			charas[_no].Rise();
		}
		if (SingletonCustom<Scuba_ControllerManager>.Instance.GetDiveButton(_no))
		{
			flag = !flag;
			charas[_no].Dive();
		}
		if (!flag)
		{
			charas[_no].Stay();
		}
	}
	public void TakePicture(int _no)
	{
		charas[_no].IsTakePicture = true;
		charas[_no].GetRigid().isKinematic = true;
		charas[_no].ShotAnimTrigger();
		if (isCameraFps[_no])
		{
			charas[_no].MoveRot(tpsCamera[_no].transform.forward, _immediate: true, _isForce: true);
		}
		charas[_no].CameraFlashView();
		SetCameraMoveActive(_no, _active: false);
		if (charas[_no].IsPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_scuba_shutter", _loop: false, 0f, 1f, 1f, 0.2f);
		}
		if (charas[_no].IsCameraChara && isCameraFps[_no])
		{
			SingletonCustom<Scuba_UiManager>.Instance.ShutterAnimationPlay(_no);
		}
		else
		{
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				if (charas[_no].IsCameraChara)
				{
					RenderTextureDirection(_no);
				}
				else
				{
					Scuba_ItemObject[] array = SingletonCustom<Scuba_ItemManager>.Instance.SearchFoundRangeItems(_no, _isNotFoundYet: true, _isViewport: false);
					for (int i = 0; i < array.Length; i++)
					{
						charas[_no].AddScore(array[i].GetScore());
						array[i].SetIsFound(_no, _value: true);
					}
					SingletonCustom<Scuba_UiManager>.Instance.SetScore(_no, charas[_no].Score);
					LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
					{
						charas[_no].IsTakePicture = false;
						charas[_no].GetRigid().isKinematic = false;
						SetCameraMoveActive(_no, _active: true);
					});
				}
			});
		}
	}
	public void RenderTextureDirection(int _no)
	{
		if (charas[_no].IsPlayer)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_no);
		}
		StartCoroutine(_RenderTextureDirection(_no));
	}
	private IEnumerator _RenderTextureDirection(int _no)
	{
		bool isFps = isCameraFps[_no];
		if (isFps)
		{
			tpsCamera[_no].targetTexture = renderTextures[_no];
		}
		else
		{
			charas[_no].TpsPictureCamera.gameObject.SetActive(value: true);
			charas[_no].TpsPictureCamera.targetTexture = renderTextures[_no];
		}
		yield return null;
		if (isFps)
		{
			tpsCamera[_no].targetTexture = null;
		}
		else
		{
			charas[_no].TpsPictureCamera.gameObject.SetActive(value: false);
			charas[_no].TpsPictureCamera.targetTexture = null;
		}
		Scuba_ItemObject[] items = SingletonCustom<Scuba_ItemManager>.Instance.SearchFoundRangeItems(_no, _isNotFoundYet: true, charas[_no].IsCameraChara && isCameraFps[_no]);
		bool isBigGet = false;
		pictureViewPointDatasArray[_no] = new PictureViewPointData[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			pictureViewPointDatasArray[_no][i] = new PictureViewPointData(isFps ? items[i].GetViewportPoint(_no) : items[i].GetCharaViewportPoint(_no), items[i].GetScore());
			if (pictureViewPointDatasArray[_no][i].GetIsRanbow())
			{
				isBigGet = true;
			}
		}
		SingletonCustom<Scuba_UiManager>.Instance.PictureView(_no);
		yield return new WaitForSeconds(1f);
		for (int j = 0; j < items.Length; j++)
		{
			charas[_no].AddScore(items[j].GetScore());
			items[j].SetIsFound(_no, _value: true);
		}
		if (charas[_no].IsPlayer)
		{
			if (isBigGet)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
			}
			else if (items.Length != 0)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
			else
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
			}
		}
		SingletonCustom<Scuba_UiManager>.Instance.SetScore(_no, charas[_no].Score);
		SingletonCustom<Scuba_UiManager>.Instance.ShutterAnimationEnd(_no);
		yield return new WaitForSeconds(1f);
		charas[_no].IsTakePicture = false;
		charas[_no].GetRigid().isKinematic = false;
		SetCameraMoveActive(_no, _active: true);
	}
	public Scuba_CharacterScript SearchMeNearest(Scuba_CharacterScript _chara)
	{
		float num = 10000f;
		int num2 = -1;
		for (int i = 0; i < charas.Length; i++)
		{
			if (!(charas[i] == _chara) && !charas[i].IsPlayer)
			{
				CalcManager.mCalcFloat = (charas[i].GetPos() - _chara.GetPos()).sqrMagnitude;
				if (CalcManager.mCalcFloat < num)
				{
					num = CalcManager.mCalcFloat;
					num2 = i;
				}
			}
		}
		return charas[num2];
	}
	public Scuba_CharacterScript SearchPosNearest(Vector3 _pos)
	{
		float num = 10000f;
		int num2 = -1;
		for (int i = 0; i < charas.Length; i++)
		{
			CalcManager.mCalcFloat = (charas[i].GetPos() - _pos).sqrMagnitude;
			if (CalcManager.mCalcFloat < num)
			{
				num = CalcManager.mCalcFloat;
				num2 = i;
			}
		}
		return charas[num2];
	}
	public Scuba_CharacterScript SearchCanTouchChara(Scuba_CharacterScript _oniChara)
	{
		return null;
	}
	public bool CheckCanPlayerTarget()
	{
		return true;
	}
	public bool CheckViewCpuCarNo()
	{
		return playerNum > 1;
	}
	public Scuba_CharacterScript GetChara(int _charaNo)
	{
		return charas[_charaNo];
	}
	public Scuba_CharacterScript GetControlChara(int _playerNo)
	{
		return charas[_playerNo];
	}
	public int GetCharaLayerNo(int _charaNo)
	{
		switch (_charaNo)
		{
		case 0:
			return 28;
		case 1:
			return 29;
		case 2:
			return 30;
		case 3:
			return 31;
		default:
			return 1;
		}
	}
	public Transform GetCharacterAnchor()
	{
		return characterAnchor;
	}
	public int GetInFieldEscaperCount(int _fieldNo)
	{
		int num = 0;
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].NowFieldNo == _fieldNo)
			{
				num++;
			}
		}
		return num;
	}
	public int GetInFieldEscaperCount(int _fieldNo, out int[] _noArray)
	{
		_noArray = new int[0];
		return 0;
	}
	public int GetScore(int _charaNo)
	{
		return charas[_charaNo].Score;
	}
	public int[] GetScoreArray()
	{
		int[] array = new int[charas.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = charas[i].Score;
		}
		return array;
	}
	public Vector2[] GetPlayerCharaViewportPos()
	{
		Vector2[] array = new Vector2[playerNum];
		int num = playerNum - 1;
		for (int i = 0; i < playerNum; i++)
		{
			array[i].x = (cameraRect[num][i].x + cameraRect[num][i].width) / 2f;
			float num2 = 0.08f;
			if (playerNum >= 3)
			{
				num2 = 0.04f;
			}
			array[i].y = cameraRect[num][i].y + num2;
		}
		return array;
	}
	public Camera GetCamera(int _playerNo)
	{
		return tpsCamera[_playerNo];
	}
	public bool GetIsCameraFps(int _playerNo)
	{
		return isCameraFps[_playerNo];
	}
	public PictureViewPointData GetPictureViewPointData(int _playerNo, int _idx)
	{
		return pictureViewPointDatasArray[_playerNo][_idx];
	}
	public PictureViewPointData[] GetPictureViewPointDataArray(int _playerNo)
	{
		return pictureViewPointDatasArray[_playerNo];
	}
	public void SetCharaMoveActive(bool _active)
	{
		for (int i = 0; i < charas.Length; i++)
		{
			if (_active)
			{
				charas[i].GetRigid().constraints = RigidbodyConstraints.FreezeRotation;
			}
			else if (!charas[i].IsPlayer || charas[i].CheckCanGameEndStop())
			{
				charas[i].GetRigid().constraints = (RigidbodyConstraints)122;
			}
		}
	}
	public void SetCameraMoveActive(bool _active)
	{
		for (int i = 0; i < freeLookCam.Length; i++)
		{
			freeLookCam[i].IsFix = !_active;
		}
	}
	public void SetCameraMoveActive(int _cameraNo, bool _active)
	{
		freeLookCam[_cameraNo].IsFix = !_active;
	}
	public void SetCameraCullingMaskBit(int _cameraNo, bool _isBit, int _layerNo)
	{
		int num = tpsCamera[_cameraNo].cullingMask;
		int num2 = 1 << _layerNo;
		if (_isBit)
		{
			if ((num & num2) != num2)
			{
				num += num2;
				isCameraMaskChanges[_cameraNo] = false;
			}
		}
		else if ((num & num2) == num2)
		{
			num -= num2;
			isCameraMaskChanges[_cameraNo] = true;
		}
		tpsCamera[_cameraNo].cullingMask = num;
	}
	private void OnDrawGizmos()
	{
	}
}
