using Cinemachine;
using UnityEngine;
public class Surfing_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("対応するSurfing_Surfer")]
	private Surfing_Surfer surfer;
	[SerializeField]
	[Header("三人称_通常カメラ")]
	private CinemachineVirtualCamera cameraNormal;
	[SerializeField]
	[Header("三人称_チュ\u30fcブ(左サイド)")]
	private CinemachineVirtualCamera cameraTubeLeftSide;
	[SerializeField]
	[Header("三人称_チュ\u30fcブ(右サイド)")]
	private CinemachineVirtualCamera cameraTubeRightSide;
	[SerializeField]
	[Header("スタ\u30fcト_カメラ")]
	private CinemachineVirtualCamera cameraStart;
	[SerializeField]
	[Header("ゴ\u30fcル_カメラ")]
	private CinemachineVirtualCamera cameraGoal;
	public CinemachineVirtualCamera CameraTubeLeftSide => cameraTubeLeftSide;
	public CinemachineVirtualCamera CameraTubeRightSide => cameraTubeRightSide;
	private void Start()
	{
		cameraTubeLeftSide.enabled = false;
		cameraTubeRightSide.enabled = false;
		cameraGoal.enabled = false;
		cameraNormal.enabled = true;
		cameraStart.enabled = true;
	}
	public void Init()
	{
	}
	private void Update()
	{
		switch (surfer.processType)
		{
		case Surfing_Surfer.SurferProcessType.STANDBY:
			if (Surfing_Define.GM.IsStartCountDown && cameraStart.enabled)
			{
				cameraStart.enabled = false;
			}
			break;
		case Surfing_Surfer.SurferProcessType.START:
			if (surfer.IsTube)
			{
				if (cameraNormal.enabled)
				{
					cameraNormal.enabled = false;
				}
				if (!cameraTubeRightSide.enabled && !cameraTubeLeftSide.enabled)
				{
					if (surfer.IsDirRight())
					{
						if (!cameraTubeRightSide.enabled)
						{
							cameraTubeRightSide.enabled = true;
						}
						if (cameraTubeLeftSide.enabled)
						{
							cameraTubeLeftSide.enabled = false;
						}
					}
					else
					{
						if (!cameraTubeLeftSide.enabled)
						{
							cameraTubeLeftSide.enabled = true;
						}
						if (cameraTubeRightSide.enabled)
						{
							cameraTubeRightSide.enabled = false;
						}
					}
				}
				else if (cameraTubeRightSide.enabled)
				{
					if (!surfer.IsDirRightTubeRideing())
					{
						if (!cameraTubeLeftSide.enabled)
						{
							cameraTubeLeftSide.enabled = true;
						}
						if (cameraTubeRightSide.enabled)
						{
							cameraTubeRightSide.enabled = false;
						}
					}
				}
				else if (cameraTubeLeftSide.enabled && !surfer.IsDirLeftTubeRideing())
				{
					if (!cameraTubeRightSide.enabled)
					{
						cameraTubeRightSide.enabled = true;
					}
					if (cameraTubeLeftSide.enabled)
					{
						cameraTubeLeftSide.enabled = false;
					}
				}
			}
			else
			{
				if (cameraTubeLeftSide.enabled)
				{
					cameraTubeLeftSide.enabled = false;
				}
				if (cameraTubeRightSide.enabled)
				{
					cameraTubeRightSide.enabled = false;
				}
				if (!cameraNormal.enabled)
				{
					cameraNormal.enabled = true;
				}
			}
			break;
		case Surfing_Surfer.SurferProcessType.GOAL:
			if (!cameraGoal.enabled)
			{
				cameraGoal.enabled = true;
			}
			break;
		}
	}
}
