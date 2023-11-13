using Cinemachine;
using UnityEngine;
public class WaterSpriderRace_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("対応する水蜘蛛")]
	private WaterSpriderRace_WaterSprider waterSprider;
	[SerializeField]
	[Header("三人称_通常カメラ")]
	private CinemachineVirtualCamera cameraNormal;
	[SerializeField]
	[Header("三人称_遠望カメラ")]
	private CinemachineVirtualCamera cameraDistant;
	[SerializeField]
	[Header("一人称_カメラ")]
	private CinemachineVirtualCamera cameraNear;
	[SerializeField]
	[Header("スタ\u30fcト_カメラ")]
	private CinemachineVirtualCamera cameraStart;
	[SerializeField]
	[Header("ゴ\u30fcル_カメラ")]
	private CinemachineVirtualCamera cameraGoal;
	private bool isGoalZoom;
	private void Start()
	{
		cameraDistant.enabled = false;
		cameraNear.enabled = false;
		cameraGoal.enabled = false;
		cameraNormal.enabled = true;
		cameraStart.enabled = true;
		isGoalZoom = false;
	}
	public void Init()
	{
	}
	private void Update()
	{
		switch (waterSprider.processType)
		{
		case WaterSpriderRace_WaterSprider.SkiBoardProcessType.STANDBY:
			if (WaterSpriderRace_Define.GM.IsStartCountDown && cameraStart.enabled)
			{
				cameraStart.enabled = false;
			}
			break;
		case WaterSpriderRace_WaterSprider.SkiBoardProcessType.START:
			switch (waterSprider.CameraPos)
			{
			case WaterSpriderRace_WaterSprider.CameraPosType.NEAR:
				if (!cameraNear.enabled)
				{
					cameraNear.enabled = true;
				}
				if (cameraNormal.enabled)
				{
					cameraNormal.enabled = false;
				}
				if (cameraDistant.enabled)
				{
					cameraDistant.enabled = false;
				}
				break;
			case WaterSpriderRace_WaterSprider.CameraPosType.NORMAL:
				if (cameraNear.enabled)
				{
					cameraNear.enabled = false;
				}
				if (!cameraNormal.enabled)
				{
					cameraNormal.enabled = true;
				}
				if (cameraDistant.enabled)
				{
					cameraDistant.enabled = false;
				}
				break;
			case WaterSpriderRace_WaterSprider.CameraPosType.DISTANT:
				if (cameraNear.enabled)
				{
					cameraNear.enabled = false;
				}
				if (cameraNormal.enabled)
				{
					cameraNormal.enabled = false;
				}
				if (!cameraDistant.enabled)
				{
					cameraDistant.enabled = true;
				}
				break;
			}
			break;
		case WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL:
			if (!cameraGoal.enabled)
			{
				cameraGoal.enabled = true;
			}
			break;
		}
	}
}
