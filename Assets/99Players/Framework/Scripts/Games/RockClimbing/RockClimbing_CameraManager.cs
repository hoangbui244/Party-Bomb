using UnityEngine;
public class RockClimbing_CameraManager : SingletonCustom<RockClimbing_CameraManager>
{
	[SerializeField]
	[Header("カメラ")]
	private RockClimbing_Camera[] arrayCamera;
	[SerializeField]
	[Header("カメラがキャラを追従する時の速度")]
	private float CAMERA_SPEED;
	private readonly Rect[] CAMERA_VIEW_PORT_RECT_SINGLE = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.334f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private readonly Rect[] CAMERA_VIEW_PORT_RECT_MULTI = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	private readonly float SINGLE_PLAYER_GAME_POS_Z = -9.5f;
	private readonly float OHTER_PLAYER_GAME_POS_Z = -8f;
	private readonly float GOAL_POS_DIFF_Y = -0.915f;
	private readonly float GOAL_POS_DIFF_Z = 2.545f;
	public void Init()
	{
		Rect[] array = null;
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			array = CAMERA_VIEW_PORT_RECT_SINGLE;
			break;
		case 2:
		case 3:
		case 4:
			array = CAMERA_VIEW_PORT_RECT_MULTI;
			break;
		}
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].SetStartAnchorPositionZ((SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && i == 0) ? SINGLE_PLAYER_GAME_POS_Z : OHTER_PLAYER_GAME_POS_Z);
				arrayCamera[i].SetDiffVec(i);
				arrayCamera[i].SetRect(array[i]);
			}
			else
			{
				arrayCamera[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void GameStartAnimation()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].GameStartAnimation();
			}
		}
	}
	public void GoalAnimation(int _playerNo)
	{
		Vector3 localPosition = arrayCamera[_playerNo].transform.localPosition;
		localPosition.y += GOAL_POS_DIFF_Y;
		localPosition.z += GOAL_POS_DIFF_Z;
		arrayCamera[_playerNo].GoalAnimation(localPosition);
	}
	public void LateUpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(i) && !arrayCamera[i].GetIsStop())
			{
				arrayCamera[i].LateUpdateMethod();
			}
		}
	}
	public RockClimbing_Camera GetCamera(int _playerNo)
	{
		return arrayCamera[_playerNo];
	}
	public float GetCameraSpeed()
	{
		return CAMERA_SPEED;
	}
}
