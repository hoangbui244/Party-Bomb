using System;
using UnityEngine;
public class MorphingRace_CameraManager : SingletonCustom<MorphingRace_CameraManager>
{
	[Serializable]
	private struct CameraFollowOffset
	{
		public MorphingRace_FieldManager.TargetPrefType charaType;
		public float angle;
		public Vector3 followOffset;
	}
	[SerializeField]
	[Header("カメラ")]
	private MorphingRace_Camera[] arrayCamera;
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
	[Header("カメラの追従情報")]
	private CameraFollowOffset[] arrayCameraFollowOffset;
	private Vector3 CAMERA_AFTER_GOAL_ROT = new Vector3(0f, 180f, 0f);
	private Vector3 CAMERA_AFTER_GOAL_FOLLOW_OFF_SET = new Vector3(0f, 1.25f, 2.5f);
	public void Init()
	{
		Rect[] array = CAMERA_VIEW_PORT_RECT[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1];
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].Init();
				arrayCamera[i].SetCameraLayer(i);
				arrayCamera[i].SetRect(array[i]);
				arrayCamera[i].gameObject.SetActive(value: true);
			}
			else
			{
				arrayCamera[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCamera[i].UpdateMethod();
			}
		}
	}
	public MorphingRace_Camera GetCamera(int _playerNo)
	{
		return arrayCamera[_playerNo];
	}
	public void SetFollowOffset(int _playerNo, int _charaType)
	{
		arrayCamera[_playerNo].SetFollowOffset(arrayCameraFollowOffset[_charaType].angle, arrayCameraFollowOffset[_charaType].followOffset);
	}
	public void AfterGoalAnimation(int _playerNo)
	{
		arrayCamera[_playerNo].AfterGoalAnimation(CAMERA_AFTER_GOAL_ROT, CAMERA_AFTER_GOAL_FOLLOW_OFF_SET);
	}
}
