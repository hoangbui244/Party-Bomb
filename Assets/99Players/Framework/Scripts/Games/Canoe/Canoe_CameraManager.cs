using UnityEngine;
public class Canoe_CameraManager : SingletonCustom<Canoe_CameraManager>
{
	[SerializeField]
	[Header("カメラ")]
	private Canoe_Camera[] arrayCamera;
	private Rect[][] CAMERA_VIEW_PORT_RECT = new Rect[4][]
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
	[SerializeField]
	[Header("カメラの追従する速度")]
	private float CAMERA_MOVE_SPEED;
	[SerializeField]
	[Header("カメラの回転する速度")]
	private float CAMERA_ROT_SPEED;
	[SerializeField]
	[Header("カメラを切り替える時間")]
	private float CAMERA_CHANGE_TIME;
	[SerializeField]
	[Header("ゴ\u30fcル時のカメラを切り替える時間")]
	private float CAMERA_GOAL_TIME;
	[SerializeField]
	[Header("ゴ\u30fcル時のゴ\u30fcルアンカ\u30fcのY座標")]
	private float CAMERA_GOAL_ANCHOR_POSY;
	[SerializeField]
	[Header("ゴ\u30fcル時のカメラの座標")]
	private Vector3 CAMERA_GOAL_CAMERA_POS;
	[SerializeField]
	[Header("ゴ\u30fcル時のカメラの角度")]
	private Vector3 CAMERA_GOAL_CAMERA_ROT;
	public void Init()
	{
		Rect[] array = CAMERA_VIEW_PORT_RECT[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1];
		for (int i = 0; i < arrayCamera.Length; i++)
		{
			if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].Init(SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i));
				arrayCamera[i].SetRect(array[i]);
				arrayCamera[i].gameObject.SetActive(value: true);
			}
			else
			{
				arrayCamera[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void FixedUpdateMethod()
	{
		for (int i = 0; i < arrayCamera.Length; i++)
		{
			if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].FixedUpdateMethod();
			}
		}
	}
	public void ChangeCameraType(int _playerNo, int _addCameraType)
	{
		arrayCamera[_playerNo].ChangeCameraType(_addCameraType);
	}
	public void SetGoalAnchor(int _playerNo)
	{
		arrayCamera[_playerNo].SetGoalAnchor();
	}
	public bool GetIsCameraChange(int _playerNo)
	{
		return arrayCamera[_playerNo].GetIsCameraChange();
	}
	public void PlayAddSpeedEffect(int _playerNo)
	{
		arrayCamera[_playerNo].PlayAddSpeedEffect();
	}
	public void StopAddSpeedEffect(int _playerNo)
	{
		arrayCamera[_playerNo].StopAddSpeedEffect();
	}
	public void PlayAddUseStaminaDashEffect(int _playerNo)
	{
		arrayCamera[_playerNo].PlayAddUseStaminaDashEffect();
	}
	public void StopAddUseStaminaDashEffect(int _playerNo)
	{
		arrayCamera[_playerNo].StopAddUseStaminaDashEffect();
	}
	public float GetCameraMoveSpeed()
	{
		return CAMERA_MOVE_SPEED;
	}
	public float GetCameraRotSpeed()
	{
		return CAMERA_ROT_SPEED;
	}
	public float GetCameraChangeTime()
	{
		return CAMERA_CHANGE_TIME;
	}
	public float GetCameraGoalTime()
	{
		return CAMERA_GOAL_TIME;
	}
	public float GetCameraGoalAnchorPosY()
	{
		return CAMERA_GOAL_ANCHOR_POSY;
	}
	public Vector3 GetCameraGoalCameraPos()
	{
		return CAMERA_GOAL_CAMERA_POS;
	}
	public Vector3 GetCameraGoalCameraRot()
	{
		return CAMERA_GOAL_CAMERA_ROT;
	}
}
