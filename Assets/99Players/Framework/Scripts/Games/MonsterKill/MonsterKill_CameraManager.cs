using UnityEngine;
public class MonsterKill_CameraManager : SingletonCustom<MonsterKill_CameraManager>
{
	[SerializeField]
	[Header("カメラ")]
	private MonsterKill_Camera[] arrayCamera;
	[SerializeField]
	[Header("追従速度")]
	private float FOLLOW_SPEED;
	[SerializeField]
	[Header("回転速度")]
	private float ROT_SPEED;
	[SerializeField]
	[Header("カメラの角度のリセットする最大時間（180度の場合）")]
	private float ROT_RESET_TIME;
	private readonly Rect[] SinglePlayerRects = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private readonly Rect[] SinglePlayerWithCpuRects = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private readonly Rect[] MultiPlayerRects = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	public void Init()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				if (i == 0)
				{
					arrayCamera[i].Init(i);
					arrayCamera[i].SetCameraRect(SinglePlayerRects[i]);
				}
				else
				{
					arrayCamera[i].gameObject.SetActive(value: false);
				}
			}
			else
			{
				arrayCamera[i].Init(i);
				arrayCamera[i].SetCameraRect(MultiPlayerRects[i]);
			}
		}
	}
	public void FixedUpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (arrayCamera[i].GetIsActive())
			{
				arrayCamera[i].FixedUpdateMethod();
			}
		}
	}
	public Camera GetCamera(int _playerNo)
	{
		return arrayCamera[_playerNo].GetCamera();
	}
	public bool GetIsCameraRotReset(int _playerNo)
	{
		return arrayCamera[_playerNo].GetIsCameraRotReset();
	}
	public void SetCameraRotReset(int _playerNo)
	{
		arrayCamera[_playerNo].SetCameraRotReset();
	}
	public void SetCameraRot(int _playerNo, Vector3 _vec, bool _isDirect = false)
	{
		arrayCamera[_playerNo].SetCameraRot(_vec, _isDirect);
	}
	public Quaternion GetCameraDir(int _playerNo)
	{
		return arrayCamera[_playerNo].GetCameraDir();
	}
	public bool GetIsActive(int _playerNo)
	{
		return arrayCamera[_playerNo].GetIsActive();
	}
	public float GetFollowSpeed()
	{
		return FOLLOW_SPEED;
	}
	public float GetRotSpeed()
	{
		return ROT_SPEED;
	}
	public float GetCameraRotResetTime()
	{
		return ROT_RESET_TIME;
	}
}
