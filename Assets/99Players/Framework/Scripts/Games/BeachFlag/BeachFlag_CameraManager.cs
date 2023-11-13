using Cinemachine;
using System.Collections;
using UnityEngine;
public class BeachFlag_CameraManager : SingletonCustom<BeachFlag_CameraManager>
{
	public enum CameraState
	{
		STANDBY,
		START,
		DASH,
		DIVE,
		GOAL
	}
	[SerializeField]
	[Header("エイム対象のオブジェクト")]
	private GameObject[] CharacterAnchor = new GameObject[2];
	[SerializeField]
	[Header("競争中キャラのMyCinemachineDollyCart")]
	private MyCinemachineDollyCart[] dollyCart = new MyCinemachineDollyCart[2];
	[SerializeField]
	[Header("待機_カメラ")]
	private CinemachineVirtualCamera cameraStandby;
	[SerializeField]
	[Header("スタ\u30fcト_カメラ")]
	private CinemachineVirtualCamera cameraStart;
	[SerializeField]
	[Header("ダッシュ中_カメラ")]
	private CinemachineVirtualCamera cameraDash;
	[SerializeField]
	[Header("飛び込み_カメラ")]
	private CinemachineVirtualCamera cameraDive;
	[SerializeField]
	[Header("ゴ\u30fcル_カメラ")]
	private CinemachineVirtualCamera cameraGoal;
	private CinemachineVirtualCamera activeCam;
	private Quaternion dash_defaultangle;
	private CameraState state;
	private bool UIdisplay;
	public CinemachineVirtualCamera ActiveCamera => activeCam;
	public bool UIDisplay => UIdisplay;
	private void Start()
	{
		Init();
	}
	public void Init()
	{
		state = CameraState.STANDBY;
		cameraStandby.enabled = true;
		cameraStart.enabled = false;
		cameraDash.enabled = false;
		cameraDash.LookAt = null;
		dash_defaultangle = cameraDash.transform.rotation;
		cameraDive.enabled = false;
		cameraGoal.enabled = false;
		activeCam = cameraStandby;
	}
	public CameraState GetState()
	{
		return state;
	}
	private void LateUpdate()
	{
		CameraLookAtUpdate();
	}
	public void SetState(CameraState _state)
	{
		if (state == _state)
		{
			return;
		}
		state = _state;
		cameraStandby.enabled = false;
		cameraStart.enabled = false;
		cameraDash.enabled = false;
		cameraDive.enabled = false;
		cameraGoal.enabled = false;
		switch (state)
		{
		case CameraState.STANDBY:
			for (int j = 0; j < BeachFlag_Define.PM.Players.Length; j++)
			{
				dollyCart[j] = BeachFlag_Define.PM.Players[j].Chara.Cart;
			}
			cameraStandby.enabled = true;
			activeCam = cameraStandby;
			StartCoroutine(WaitForEase());
			break;
		case CameraState.START:
			cameraStart.enabled = true;
			activeCam = cameraStart;
			break;
		case CameraState.DASH:
			if (BeachFlag_Define.GM.signboard.activeSelf)
			{
				BeachFlag_Define.GM.signboard.SetActive(value: false);
			}
			for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
			{
				dollyCart[i] = BeachFlag_Define.PM.Players[i].Chara.Cart;
			}
			cameraDash.enabled = true;
			activeCam = cameraDash;
			break;
		case CameraState.DIVE:
			cameraDive.enabled = true;
			activeCam = cameraDive;
			break;
		case CameraState.GOAL:
			cameraGoal.enabled = true;
			activeCam = cameraGoal;
			break;
		}
	}
	private void CameraLookAtUpdate()
	{
		if (!(activeCam == cameraDash) && !(activeCam == cameraStart))
		{
			return;
		}
		Quaternion rotation = activeCam.transform.rotation;
		rotation.eulerAngles = dash_defaultangle.eulerAngles;
		activeCam.transform.rotation = rotation;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
		{
			if (BeachFlag_Define.PM.Players[i].UserType <= BeachFlag_Define.UserType.PLAYER_4)
			{
				num++;
				num2 = i;
			}
			else
			{
				num--;
			}
		}
		if (num == 0)
		{
			activeCam.Follow = CharacterAnchor[num2].transform;
		}
		else if (dollyCart[0].m_Position >= dollyCart[1].m_Position)
		{
			activeCam.Follow = CharacterAnchor[0].transform;
		}
		else
		{
			activeCam.Follow = CharacterAnchor[1].transform;
		}
	}
	private IEnumerator WaitForEase()
	{
		UIdisplay = false;
		yield return new WaitForSeconds(2f);
		UIdisplay = true;
	}
}
