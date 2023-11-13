using Cinemachine;
using UnityEngine;
public class HandSeal_Camera : MonoBehaviour
{
	[SerializeField]
	[Header("対応するHandスクリプト")]
	private HandSeal_Hand hand;
	[SerializeField]
	[Header("一人称_カメラ")]
	private CinemachineVirtualCamera cameraFP;
	[SerializeField]
	[Header("スタ\u30fcト_カメラ")]
	private CinemachineVirtualCamera cameraStart;
	[SerializeField]
	[Header("ゴ\u30fcル_カメラ")]
	private CinemachineVirtualCamera cameraGoal;
	private void Start()
	{
		cameraStart.enabled = true;
		cameraGoal.enabled = false;
		cameraFP.enabled = true;
		if (HandSeal_Define.PLAYER_NUM == 2)
		{
			cameraFP.gameObject.transform.SetLocalPosition(0f, 0.75f, 0.7f);
			cameraFP.m_Lens.FieldOfView = 80f;
		}
		else
		{
			cameraFP.gameObject.transform.SetLocalPosition(0f, 1f, 0.5f);
			cameraFP.m_Lens.FieldOfView = 52f;
		}
	}
	private void Update()
	{
		if (hand.processType == HandSeal_Hand.GameProcessType.STANDBY && HandSeal_Define.GM.IsStartCountDown && cameraStart.enabled)
		{
			cameraStart.enabled = false;
		}
	}
}
