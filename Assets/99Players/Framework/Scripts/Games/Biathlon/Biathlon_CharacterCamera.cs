using Cinemachine;
using UnityEngine;
public class Biathlon_CharacterCamera : MonoBehaviour
{
	private static readonly Rect[] SinglePlayerRects = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(0f, 0f, 0f, 0f),
		new Rect(0f, 0f, 0f, 0f),
		new Rect(0f, 0f, 0f, 0f)
	};
	private static readonly Rect[] DuoPlayerRects = new Rect[4]
	{
		new Rect(0f, 0f, 0.5f, 1f),
		new Rect(0.5f, 0f, 0.5f, 1f),
		new Rect(0f, 0f, 0f, 0f),
		new Rect(0f, 0f, 0f, 0f)
	};
	private static readonly Rect[] MultiPlayerRects = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private CinemachineVirtualCamera crossCountryCamera;
	[SerializeField]
	private CinemachineVirtualCamera crossCountryCamera2;
	[SerializeField]
	private CinemachineVirtualCamera shootingCamera;
	[SerializeField]
	private CinemachineVirtualCamera downwardCamera;
	[SerializeField]
	private CinemachineVirtualCamera upwardCamera;
	[SerializeField]
	private CinemachineVirtualCamera goalCamera;
	[SerializeField]
	private Biathlon_CinemachineCameraZoom cameraZoom;
	[SerializeField]
	private Transform shootingFollowTarget;
	[SerializeField]
	private GameObject gun;
	[SerializeField]
	private MeshRenderer gunRenderer;
	[SerializeField]
	private Biathlon_SpeedEffect speedEffect;
	[SerializeField]
	private Transform shootPosition;
	[SerializeField]
	private Biathlon_FollowerCameraConfig config;
	private Biathlon_Character playingCharacter;
	private Vector3 targetPosition;
	private int crossCountryCameraIndex;
	public CinemachineVirtualCamera ShootingCamera => shootingCamera;
	public Biathlon_CinemachineCameraZoom CameraZoom => cameraZoom;
	public Transform ShootingFollowTarget => shootingFollowTarget;
	private CinemachineVirtualCamera CurrentCrossCountryCamera
	{
		get
		{
			if (crossCountryCameraIndex != 0)
			{
				return crossCountryCamera2;
			}
			return crossCountryCamera;
		}
	}
	public void Init()
	{
		camera.enabled = false;
		gun.SetActive(value: false);
		speedEffect.Init();
		crossCountryCameraIndex = 0;
		ActivateCrossCountryCamera();
	}
	public void UpdateMethod()
	{
		speedEffect.UpdateMethod();
		if (!playingCharacter.IsGoal && IsChangeCamera())
		{
			ChangeCrossCountryCamera();
		}
	}
	public void SetCharacter(Biathlon_Character character)
	{
		playingCharacter = character;
		SetGunMaterial();
		UpdateViewPort();
		ActivateCrossCountryCamera();
	}
	public void UpdateViewPort()
	{
		switch (SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum)
		{
		case 1:
			camera.enabled = (playingCharacter.PlayerNo < SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum);
			camera.rect = SinglePlayerRects[playingCharacter.PlayerNo];
			break;
		case 2:
			camera.enabled = (playingCharacter.PlayerNo < SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum);
			camera.rect = DuoPlayerRects[playingCharacter.PlayerNo];
			break;
		case 3:
		case 4:
			camera.enabled = true;
			camera.rect = MultiPlayerRects[playingCharacter.PlayerNo];
			break;
		}
	}
	public void ActivateCrossCountryCamera()
	{
		if (CurrentCrossCountryCamera.Priority != 100 && goalCamera.Priority != 100)
		{
			CurrentCrossCountryCamera.Priority = 100;
			shootingCamera.Priority = 10;
			downwardCamera.Priority = 10;
			upwardCamera.Priority = 10;
			goalCamera.Priority = 10;
			gun.SetActive(value: false);
		}
	}
	public void ActivateRifleShootingCamera()
	{
		if (goalCamera.Priority != 100)
		{
			shootingCamera.Priority = 100;
			cameraZoom.Fov = 60f;
			CurrentCrossCountryCamera.Priority = 10;
			downwardCamera.Priority = 10;
			upwardCamera.Priority = 10;
			goalCamera.Priority = 10;
			gun.SetActive(value: true);
		}
	}
	public void ActivateDownwardCamera()
	{
		if (downwardCamera.Priority != 100 && goalCamera.Priority != 100)
		{
			downwardCamera.Priority = 100;
			upwardCamera.Priority = 10;
			CurrentCrossCountryCamera.Priority = 10;
			shootingCamera.Priority = 10;
			goalCamera.Priority = 10;
		}
	}
	public void ActivateUpwardCamera()
	{
		if (upwardCamera.Priority != 100 && goalCamera.Priority != 100)
		{
			upwardCamera.Priority = 100;
			downwardCamera.Priority = 10;
			CurrentCrossCountryCamera.Priority = 10;
			shootingCamera.Priority = 10;
			goalCamera.Priority = 10;
		}
	}
	public void ActivateGoalCamera()
	{
		if (goalCamera.Priority != 100)
		{
			goalCamera.Priority = 100;
			upwardCamera.Priority = 10;
			downwardCamera.Priority = 10;
			CurrentCrossCountryCamera.Priority = 10;
			shootingCamera.Priority = 10;
		}
	}
	public Vector3 GetScaledScreenPoint(Vector3 rawScreenPoint)
	{
		int pixelWidth = camera.pixelWidth;
		int pixelHeight = camera.pixelHeight;
		float num = (float)pixelWidth / 1920f;
		float num2 = (float)pixelHeight / 1080f;
		Rect rect = camera.rect;
		float num3 = (float)pixelWidth / rect.width * rect.x;
		float num4 = (float)pixelHeight / rect.height * rect.y;
		float x = rawScreenPoint.x * num + num3;
		float y = rawScreenPoint.y * num2 + num4;
		return new Vector3(x, y, rawScreenPoint.z);
	}
	public Ray ScreenPointToRay(Vector3 position)
	{
		return camera.ScreenPointToRay(position);
	}
	public void ShowSpeedEffect()
	{
		if (!speedEffect.IsShow)
		{
			speedEffect.Show();
		}
	}
	public void HideSpeedEffect()
	{
		if (speedEffect.IsShow)
		{
			speedEffect.Hide();
		}
	}
	private void SetGunMaterial()
	{
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playingCharacter.PlayerNo];
		gunRenderer.sharedMaterial = config.GunMaterials[num];
	}
	private bool IsChangeCamera()
	{
		if (!playingCharacter.IsPlayer)
		{
			return false;
		}
		Biathlon_Definition.ControlUser controlUser = playingCharacter.ControlUser;
		if (!SingletonCustom<Biathlon_Input>.Instance.IsPressDownButtonL(controlUser) && !SingletonCustom<Biathlon_Input>.Instance.IsPressDownButtonZL(controlUser) && !SingletonCustom<Biathlon_Input>.Instance.IsPressDownButtonR(controlUser))
		{
			return SingletonCustom<Biathlon_Input>.Instance.IsPressDownButtonZR(controlUser);
		}
		return true;
	}
	private void ChangeCrossCountryCamera()
	{
		if (CurrentCrossCountryCamera.Priority == 100)
		{
			CurrentCrossCountryCamera.Priority = 10;
			crossCountryCameraIndex = 1 - crossCountryCameraIndex;
			CurrentCrossCountryCamera.Priority = 100;
		}
	}
}
