using UnityEngine;
public class BlackSmith_CameraManager : SingletonCustom<BlackSmith_CameraManager>
{
	[SerializeField]
	[Header("カメラ")]
	private BlackSmith_Camera[] arrayCamera;
	private static readonly Rect[] SinglePlayerRects = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] SinglePlayerWithCpuRects = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] MultiPlayerRects = new Rect[4]
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
				arrayCamera[i].SetCameraRect(SinglePlayerWithCpuRects[i]);
			}
			else
			{
				arrayCamera[i].SetCameraRect(MultiPlayerRects[i]);
			}
		}
	}
	public Vector3 GetCameraRot(int _playerNo)
	{
		return arrayCamera[_playerNo].GetCameraRot();
	}
}
