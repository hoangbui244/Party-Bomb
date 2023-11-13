using System.Runtime.InteropServices;
using UnityEngine;
[Guid("06FE09A9-81E0-48C1-89DC-A2C7BD6DBD6C")]
public class WaterSlider_CameraMover : MonoBehaviour
{
	private enum CAMERA_MODE
	{
		THIRDPERSON_FAR,
		THIRDPERSON_NEAR,
		FIRSTPERSON,
		GOAL,
		INTUNNEL,
		INBANK
	}
	private CAMERA_MODE cameraMode = CAMERA_MODE.THIRDPERSON_NEAR;
	private CAMERA_MODE cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
	[SerializeField]
	[Header("タ\u30fcゲット")]
	private GameObject objTarget;
	[SerializeField]
	[Header("ソリクラス")]
	private WaterSlider_Sled sled;
	[SerializeField]
	[Header("カメラ")]
	private Camera currentCamera;
	private float cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_UP;
	private float cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_CAMERABUCK;
	private float fpsCameraRot = 0.7f;
	private Vector3 targetPos;
	private float moveCompSpeed = 0.1f;
	private float rotCompSpeed = 1f;
	private float rotCompSpeed_Start = 0.001f;
	private Vector3 relativePos;
	private Quaternion rotation;
	private bool isAddSpeed;
	private bool isAccel;
	private float addSpeed;
	private Vector3 tempPos;
	private bool bank;
	public void Wakeup()
	{
		base.gameObject.SetActive(value: true);
		currentCamera.enabled = true;
		targetPos = objTarget.transform.position + objTarget.transform.forward;
		targetPos.y += 0.8599997f;
		base.transform.position = targetPos;
		relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
		base.transform.rotation = Quaternion.LookRotation(relativePos);
		base.transform.SetLocalEulerAnglesX(25f);
		addSpeed = 0f;
		bank = false;
	}
	private void FixedUpdate()
	{
		UnityEngine.Debug.Log(cameraUp);
		switch (cameraMode)
		{
		case CAMERA_MODE.THIRDPERSON_FAR:
		case CAMERA_MODE.THIRDPERSON_NEAR:
		case CAMERA_MODE.FIRSTPERSON:
			if (cameraModeKeep != CAMERA_MODE.FIRSTPERSON)
			{
				tempPos = objTarget.transform.position + objTarget.transform.forward * (0f - cameraBuck + addSpeed * 0.5f);
				tempPos.y += cameraUp;
				targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
				CameraMove();
			}
			else
			{
				tempPos = objTarget.transform.position;
				tempPos.y += cameraUp;
				targetPos = tempPos;
				CameraMove();
			}
			if (sled.CurrentState == WaterSlider_Sled.State.DRIVE)
			{
				ModeChange();
				InTunnelCamera();
				InBankCamera();
				GOAL();
			}
			break;
		case CAMERA_MODE.INTUNNEL:
			InTunnelModeChange();
			if (cameraModeKeep != CAMERA_MODE.FIRSTPERSON)
			{
				tempPos = objTarget.transform.position + objTarget.transform.forward * (0f - cameraBuck + addSpeed * 0.5f);
				tempPos.y += cameraUp;
				targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
				CameraMove();
			}
			else
			{
				tempPos = objTarget.transform.position;
				tempPos.y += cameraUp;
				targetPos = tempPos;
				CameraMove();
			}
			if (!sled.GetTunnel())
			{
				if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_FAR)
				{
					cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_UP;
					cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_CAMERABUCK;
				}
				else if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_NEAR)
				{
					cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_UP;
					cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_CAMERABUCK;
				}
				else if (cameraModeKeep == CAMERA_MODE.FIRSTPERSON)
				{
					cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
					cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				}
				cameraMode = cameraModeKeep;
			}
			break;
		case CAMERA_MODE.INBANK:
			InBankModeChange();
			if (cameraModeKeep != CAMERA_MODE.FIRSTPERSON)
			{
				tempPos = objTarget.transform.position + objTarget.transform.forward * (0f - cameraBuck + addSpeed * 0.5f);
				tempPos.y += cameraUp;
				targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
				CameraMove();
			}
			else
			{
				tempPos = objTarget.transform.position;
				tempPos.y += cameraUp;
				targetPos = tempPos;
				CameraMove();
			}
			if (!bank)
			{
				if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_FAR)
				{
					cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_UP;
					cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_CAMERABUCK;
				}
				else if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_NEAR)
				{
					cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_UP;
					cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_CAMERABUCK;
				}
				else if (cameraModeKeep == CAMERA_MODE.FIRSTPERSON)
				{
					cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
					cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				}
				cameraMode = cameraModeKeep;
			}
			break;
		case CAMERA_MODE.GOAL:
			currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
			moveCompSpeed = 0.065f;
			tempPos = objTarget.transform.position;
			tempPos.x -= 1.2f;
			tempPos.y += 1.2f;
			targetPos = Vector3.Slerp(targetPos, tempPos, 0.75f);
			CameraMove();
			break;
		}
	}
	public void Sleep()
	{
		base.gameObject.SetActive(value: false);
	}
	public void SetRect(Rect _rect)
	{
		currentCamera.rect = _rect;
	}
	public void AddSpeed()
	{
		isAddSpeed = true;
		isAccel = false;
		addSpeed = 0f;
	}
	private void CameraMove()
	{
		base.transform.position = Vector3.Slerp(base.transform.position, targetPos, moveCompSpeed);
		relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
		rotation = Quaternion.LookRotation(relativePos);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, rotCompSpeed);
	}
	private void GOAL()
	{
		if (sled.CurrentState == WaterSlider_Sled.State.GOAL)
		{
			cameraMode = CAMERA_MODE.GOAL;
		}
	}
	private void ModeChange()
	{
		if (cameraMode == CAMERA_MODE.THIRDPERSON_FAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_UP;
				cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
				cameraMode = CAMERA_MODE.THIRDPERSON_NEAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
				cameraMode = CAMERA_MODE.FIRSTPERSON;
			}
		}
		else if (cameraMode == CAMERA_MODE.THIRDPERSON_NEAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
				cameraMode = CAMERA_MODE.FIRSTPERSON;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_UP;
				cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
				cameraMode = CAMERA_MODE.THIRDPERSON_FAR;
			}
		}
		else if (cameraMode == CAMERA_MODE.FIRSTPERSON)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_UP;
				cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_FAR_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
				cameraMode = CAMERA_MODE.THIRDPERSON_FAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_UP;
				cameraBuck = WaterSlider_Define.THIRDPERSON_CAMERA_NEAR_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
				cameraMode = CAMERA_MODE.THIRDPERSON_NEAR;
			}
		}
	}
	private void InTunnelCamera()
	{
		if (sled.GetTunnel())
		{
			if (cameraMode == CAMERA_MODE.THIRDPERSON_FAR)
			{
				cameraUp = WaterSlider_Define.INTUNNEL_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_FARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
			else if (cameraMode == CAMERA_MODE.THIRDPERSON_NEAR)
			{
				cameraUp = WaterSlider_Define.INTUNNEL_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_NEARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
			else if (cameraMode == CAMERA_MODE.FIRSTPERSON)
			{
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
			cameraMode = CAMERA_MODE.INTUNNEL;
		}
	}
	private void InTunnelModeChange()
	{
		if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_FAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.INTUNNEL_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_NEARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
		}
		else if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_NEAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.INTUNNEL_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_FARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
		}
		else if (cameraModeKeep == CAMERA_MODE.FIRSTPERSON)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.INTUNNEL_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_FARCAMERA_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.INTUNNEL_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INTUNNEL_NEARCAMERA_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
		}
	}
	private void InBankCamera()
	{
		if (bank)
		{
			if (cameraMode == CAMERA_MODE.THIRDPERSON_FAR)
			{
				cameraUp = WaterSlider_Define.INBANK_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_FARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
			else if (cameraMode == CAMERA_MODE.THIRDPERSON_NEAR)
			{
				cameraUp = WaterSlider_Define.INBANK_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_NEARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
			else if (cameraMode == CAMERA_MODE.FIRSTPERSON)
			{
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
			cameraMode = CAMERA_MODE.INBANK;
		}
	}
	private void InBankModeChange()
	{
		if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_FAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.INBANK_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_NEARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
		}
		else if (cameraModeKeep == CAMERA_MODE.THIRDPERSON_NEAR)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.FIRSTPERSON_CAMERA_UP;
				cameraBuck = WaterSlider_Define.FIRSTPERSON_CAMERA_CAMERABUCK;
				currentCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString()));
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.FIRSTPERSON;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.INBANK_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_FARCAMERA_CAMERABUCK;
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
		}
		else if (cameraModeKeep == CAMERA_MODE.FIRSTPERSON)
		{
			if (sled.GetRideChara().GetCameraRight())
			{
				cameraUp = WaterSlider_Define.INBANK_FARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_FARCAMERA_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_FAR;
			}
			if (sled.GetRideChara().GetCameraLeft())
			{
				cameraUp = WaterSlider_Define.INBANK_NEARCAMERA_UP;
				cameraBuck = WaterSlider_Define.INBANK_NEARCAMERA_CAMERABUCK;
				currentCamera.cullingMask |= 1 << LayerMask.NameToLayer("Collision_Obj_" + (sled.GetRideChara().GetArrayIdx() + 1).ToString());
				LeanTween.move(base.gameObject, targetPos, 0.1f);
				cameraModeKeep = CAMERA_MODE.THIRDPERSON_NEAR;
			}
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.name.Contains("BankCameraCollider"))
		{
			switch (cameraMode)
			{
			case CAMERA_MODE.THIRDPERSON_FAR:
				bank = true;
				break;
			}
		}
		if (other.gameObject.name.Contains("BankCameraCollider_1"))
		{
			switch (cameraMode)
			{
			case CAMERA_MODE.THIRDPERSON_NEAR:
				bank = true;
				break;
			}
		}
		if (other.gameObject.name.Contains("BankCameraCollider_1"))
		{
			switch (cameraMode)
			{
			case CAMERA_MODE.THIRDPERSON_FAR:
			case CAMERA_MODE.THIRDPERSON_NEAR:
				break;
			case CAMERA_MODE.FIRSTPERSON:
				bank = true;
				break;
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Contains("BankCameraCollider"))
		{
			bank = false;
		}
		if (other.gameObject.name.Contains("BankCameraCollider_1"))
		{
			bank = false;
		}
		if (other.gameObject.name.Contains("BankCameraCollider_1"))
		{
			bank = false;
		}
	}
}
