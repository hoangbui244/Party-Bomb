using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Cameras;
public class Hidden_CharacterManager : SingletonCustom<Hidden_CharacterManager>
{
	public const int CHARACTER_NUM = 4;
	private const float CAMERA_SPHERECAST_RADIUS = 0.05f;
	private const float CAMERA_DISABLE_CHARA_DISTANCE = 1.2f;
	private const float CAMERA_DISABLE_CHARA_SQR_DISTANCE = 1.44f;
	private const int ONE_SECOND_ADD_SCORE = 100;
	private const int ONI_TOUCH_ADD_SCORE = 1000;
	private const float ESCAPER_TOUCH_REMOVE_MAG = 0.5f;
	private const float CAN_TOUCH_DISTANCE = 1f;
	private const float CAN_TOUCH_SQR_DISTANCE = 1f;
	private const float CAN_TOUCH_HEIGHT = 0.5f;
	private static int PrevOniCharaNo = -1;
	private float[] FieldScaleArray = new float[3]
	{
		1.25f,
		1.5f,
		1f
	};
	private int playerNum = 1;
	[SerializeField]
	private GameObject charaPrefab;
	[SerializeField]
	private Transform characterAnchor;
	[SerializeField]
	private Transform[] createAnchor;
	[SerializeField]
	private Transform fieldScaleAnchor;
	[SerializeField]
	private Transform freeLookCameraAnchor;
	[SerializeField]
	private FreeLookCam[] freeLookCam;
	[SerializeField]
	private Camera[] tpsCamera;
	private bool[] isCameraMaskChanges = new bool[4];
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
	private Hidden_CharacterScript[] charas;
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
		for (int i = 0; i < 30; i++)
		{
			int num = UnityEngine.Random.Range(0, createAnchor.Length);
			int num2 = UnityEngine.Random.Range(0, createAnchor.Length);
			Transform transform = createAnchor[num];
			createAnchor[num] = createAnchor[num2];
			createAnchor[num2] = transform;
		}
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		charas = new Hidden_CharacterScript[4];
		int num3 = 0;
		for (int j = 0; j < charas.Length; j++)
		{
			charas[j] = UnityEngine.Object.Instantiate(charaPrefab, createAnchor[j].transform.position, Quaternion.identity, characterAnchor).GetComponent<Hidden_CharacterScript>();
			charas[j].gameObject.SetActive(value: true);
			charas[j].Init(j, Hidden_CharacterScript.OniType.Escaper);
			charas[j].UpdateFieldNo();
			Vector3 vector = SingletonCustom<Hidden_FieldManager>.Instance.GetStartLookAnchor().position - charas[j].transform.position;
			float num4 = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			num4 += UnityEngine.Random.Range(-180f, 180f);
			if (j < playerNum)
			{
				charas[j].IsPlayer = true;
				charas[j].PlayerNo = playerGroupList[j][0];
				charas[j].SetCharaGameStyle(playerGroupList[j][0]);
				charas[j].SettingStartRotation(num4);
				charas[j].SettingActionMarkSprite();
				freeLookCam[j].SetPlayerNo(j);
				freeLookCam[j].SetTarget(charas[j].GetCameraTarget());
				charas[j].SetCameraRotAnchor(freeLookCam[j].transform);
				freeLookCam[j].Init(num4);
				freeLookCam[j].gameObject.SetActive(value: true);
				if (playerNum == 1)
				{
					tpsCamera[0].rect = cameraRect[0][0];
				}
				else
				{
					tpsCamera[j].rect = cameraRect[3][j];
				}
				if (playerNum == 1)
				{
					freeLookCam[0].SetIsCanButtonCtrl(_value: false);
				}
			}
			else
			{
				charas[j].IsPlayer = false;
				charas[j].PlayerNo = 4 + num3;
				charas[j].SetCharaGameStyle(charas[j].PlayerNo);
				charas[j].SettingCpuStrength();
				charas[j].SettingStartRotation(num4);
				charas[j].SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Standby);
				if (CheckViewCpuCarNo())
				{
					charas[j].IsCpuCamera = true;
					freeLookCam[j].enabled = false;
					freeLookCam[j].gameObject.SetActive(value: true);
					freeLookCam[j].transform.position = charas[j].GetCameraTarget().position;
					freeLookCam[j].transform.SetEulerAnglesY(charas[j].transform.eulerAngles.y);
					freeLookCam[j].SetPlayerNo(0);
					tpsCamera[j].rect = cameraRect[3][j];
				}
				else
				{
					freeLookCam[j].gameObject.SetActive(value: false);
				}
				num3++;
			}
			charas[j].SettingCtrlDirAnchor();
		}
		SetCharaMoveActive(_active: false);
		if (playerNum < 4)
		{
			int num5 = UnityEngine.Random.Range(playerNum, 4);
			charas[num5].SetOniType(Hidden_CharacterScript.OniType.Oni);
			charas[num5].SetAiMoveType(Hidden_CharacterScript.AiMoveType.Oni_Search);
			charas[num5].GetOniObj().SetActive(value: true);
		}
		else
		{
			List<int> list = new List<int>
			{
				0,
				1,
				2,
				3
			};
			list.Remove(PrevOniCharaNo);
			list = (from a in list
				orderby Guid.NewGuid()
				select a).ToList();
			charas[list[0]].SetOniType(Hidden_CharacterScript.OniType.Oni);
			charas[list[0]].SetAiMoveType(Hidden_CharacterScript.AiMoveType.Oni_Search);
			charas[list[0]].GetOniObj().SetActive(value: true);
			PrevOniCharaNo = list[0];
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < playerNum; i++)
		{
			if (isAutoCamera[i] && Mathf.Abs(freeLookCam[i].X) > 0.01f)
			{
				isAutoCamera[i] = false;
			}
			else if (!isAutoCamera[i] && Mathf.Abs(freeLookCam[i].X) <= 0.01f && charas[i].IsMove)
			{
				isAutoCamera[i] = true;
			}
		}
		if (!SingletonCustom<Hidden_GameManager>.Instance.IsGameStart)
		{
			for (int j = 0; j < freeLookCam.Length; j++)
			{
				if (freeLookCam[j].gameObject.activeSelf)
				{
					charas[j].UpdateObjRotAnchor();
				}
				if (charas[j].IsPlayer || CheckViewCpuCarNo())
				{
					CameraUpdate(j);
				}
			}
		}
		else
		{
			if (SingletonCustom<Hidden_GameManager>.Instance.IsGameEnd)
			{
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
				if (flag && !SingletonCustom<Hidden_GameManager>.Instance.IsGameEnd)
				{
					charas[k].AiMove();
				}
				charas[k].UpdateMethod();
				if (charas[k].IsPlayer || CheckViewCpuCarNo())
				{
					CameraUpdate(k);
				}
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
		tpsCamera[i].transform.localPosition = new Vector3(0f, 0f, -2f);
		Vector3 direction = tpsCamera[i].transform.position - position;
		if (Physics.SphereCast(position, 0.05f, direction, out RaycastHit hitInfo, direction.magnitude, 1048576))
		{
			tpsCamera[i].transform.position = hitInfo.point + hitInfo.normal * 0.05f;
			if (isCameraMaskChanges[i] == (tpsCamera[i].transform.position - position).sqrMagnitude > 1.44f)
			{
				SetCameraCullingMaskBit(i, isCameraMaskChanges[i], GetCharaLayerNo(i));
			}
		}
		else if (isCameraMaskChanges[i])
		{
			SetCameraCullingMaskBit(i, _isBit: true, GetCharaLayerNo(i));
		}
	}
	private void LateUpdate()
	{
		if (!SingletonCustom<Hidden_GameManager>.Instance.IsGameEnd)
		{
			for (int i = 0; i < charas.Length; i++)
			{
				charas[i].ActionMarkUpdate();
			}
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
	public void OneSecondScoreUp()
	{
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].CheckOniType(Hidden_CharacterScript.OniType.Escaper))
			{
				charas[i].AddScore(100);
				SingletonCustom<Hidden_UiManager>.Instance.SetScore(i, charas[i].Score);
			}
		}
	}
	public void OniTouchScoreUp(int _charaNo)
	{
		charas[_charaNo].AddScore(1000);
		SingletonCustom<Hidden_UiManager>.Instance.SetScore(_charaNo, charas[_charaNo].Score);
	}
	public void EscaperTouchScoreDown(int _charaNo)
	{
		charas[_charaNo].Score = Mathf.FloorToInt((float)charas[_charaNo].Score * 0.5f);
		SingletonCustom<Hidden_UiManager>.Instance.SetScore(_charaNo, charas[_charaNo].Score);
	}
	private void PlayerOperation(int _playerNo)
	{
		CharaMove(_playerNo);
	}
	private void CharaMove(int _no)
	{
		Vector3 moveDir = SingletonCustom<Hidden_ControllerManager>.Instance.GetMoveDir(_no);
		int moveTypeNum = SingletonCustom<Hidden_ControllerManager>.Instance.GetMoveTypeNum(_no);
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
	}
	public Hidden_CharacterScript SearchMeNearest(Hidden_CharacterScript _chara)
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
	public Hidden_CharacterScript SearchPosNearest(Vector3 _pos)
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
	public Hidden_CharacterScript SearchCanTouchChara(Hidden_CharacterScript _oniChara)
	{
		if (!_oniChara.CheckOniType(Hidden_CharacterScript.OniType.Oni))
		{
			return null;
		}
		int num = -1;
		float num2 = 1000f;
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].CheckOniType(Hidden_CharacterScript.OniType.Oni) || !charas[i].CheckCanTouchReceive())
			{
				continue;
			}
			Vector3 pos = charas[i].GetPos();
			Vector3 pos2 = _oniChara.GetPos();
			if (Mathf.Abs(pos.y - pos2.y) > 0.5f)
			{
				continue;
			}
			Vector3 lhs = pos - pos2;
			lhs.y = 0f;
			if (Vector3.Dot(lhs, _oniChara.GetDir()) < 0f)
			{
				continue;
			}
			pos.y += 0.5f;
			pos2.y += 0.5f;
			if (!Physics.Linecast(pos, pos2, 1048576))
			{
				float sqrMagnitude = lhs.sqrMagnitude;
				if (sqrMagnitude < 1f && sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					num = i;
				}
			}
		}
		if (num == -1)
		{
			return null;
		}
		return charas[num];
	}
	public bool CheckCanPlayerTarget()
	{
		return true;
	}
	public bool CheckViewCpuCarNo()
	{
		return playerNum > 1;
	}
	public Hidden_CharacterScript GetChara(int _charaNo)
	{
		return charas[_charaNo];
	}
	public Hidden_CharacterScript GetOniChara()
	{
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].CheckOniType(Hidden_CharacterScript.OniType.Oni))
			{
				return charas[i];
			}
		}
		return null;
	}
	public Hidden_CharacterScript GetControlChara(int _playerNo)
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
	public int GetOniCount()
	{
		return 1;
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
		List<int> list = new List<int>();
		int num = 0;
		for (int i = 0; i < charas.Length; i++)
		{
			if (charas[i].NowFieldNo == _fieldNo && charas[i].CheckOniType(Hidden_CharacterScript.OniType.Escaper))
			{
				num++;
				list.Add(i);
			}
		}
		_noArray = list.ToArray();
		return num;
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
