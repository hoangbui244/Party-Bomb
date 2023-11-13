using UnityEngine;
public class RockClimbing_UIManager : SingletonCustom<RockClimbing_UIManager>
{
	public enum ControllerUIType
	{
		Move,
		Climbing,
		Skip
	}
	private Camera globalCamera;
	[SerializeField]
	[Header("【１人】の時のレイアウトオブジェクト")]
	private GameObject singleLayoutObjct;
	[SerializeField]
	[Header("【２人】の時のレイアウトオブジェクト")]
	private GameObject twoLayoutObject;
	[SerializeField]
	[Header("【３人】の時のレイアウトオブジェクト")]
	private GameObject threeLayoutObject;
	[SerializeField]
	[Header("【４人】の時のレイアウトオブジェクト")]
	private GameObject fourLayoutObject;
	[SerializeField]
	[Header("【１人】の時のレイアウトデ\u30fcタ")]
	private RockClimbing_LayoutData[] singleLayoutData;
	[SerializeField]
	[Header("【２人】の時のレイアウトデ\u30fcタ")]
	private RockClimbing_LayoutData[] multiLayoutData_Two;
	[SerializeField]
	[Header("【３人】の時のレイアウトデ\u30fcタ")]
	private RockClimbing_LayoutData[] multiLayoutData_Three;
	[SerializeField]
	[Header("【４人】の時のレイアウトデ\u30fcタ")]
	private RockClimbing_LayoutData[] multiLayoutData_Four;
	public void Init()
	{
		globalCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		singleLayoutObjct.SetActive(value: false);
		twoLayoutObject.SetActive(value: false);
		threeLayoutObject.SetActive(value: false);
		fourLayoutObject.SetActive(value: false);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleLayoutObjct.SetActive(value: true);
			for (int i = 0; i < singleLayoutData.Length; i++)
			{
				singleLayoutData[i].Init(i);
				if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(i))
				{
					SetPlayerIcon(i, (int)SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).GetUserType());
					SetObstacleCautionOrderInLayer(i);
					SetJoyConButtonPlayerType(i);
				}
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
		{
			twoLayoutObject.SetActive(value: true);
			for (int j = 0; j < multiLayoutData_Two.Length; j++)
			{
				multiLayoutData_Two[j].Init(j);
				if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(j))
				{
					SetPlayerIcon(j, (int)SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(j).GetUserType());
					SetObstacleCautionOrderInLayer(j);
					SetJoyConButtonPlayerType(j);
				}
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
		{
			threeLayoutObject.SetActive(value: true);
			for (int k = 0; k < multiLayoutData_Three.Length; k++)
			{
				multiLayoutData_Three[k].Init(k);
				if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(k))
				{
					SetPlayerIcon(k, (int)SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(k).GetUserType());
					SetObstacleCautionOrderInLayer(k);
					SetJoyConButtonPlayerType(k);
				}
			}
		}
		else
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 4)
			{
				return;
			}
			fourLayoutObject.SetActive(value: true);
			for (int l = 0; l < multiLayoutData_Four.Length; l++)
			{
				multiLayoutData_Four[l].Init(l);
				if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(l))
				{
					SetPlayerIcon(l, (int)SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(l).GetUserType());
					SetObstacleCautionOrderInLayer(l);
					SetJoyConButtonPlayerType(l);
				}
			}
		}
	}
	public void SetPlayerIconActive(int _playerNo, bool _isActive)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetPlayerIconActive(_isActive);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetPlayerIconActive(_isActive);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetPlayerIconActive(_isActive);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetPlayerIconActive(_isActive);
		}
	}
	public void SetPlayerIcon(int _playerNo, int _userType)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetPlayerIcon(_userType);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetPlayerIcon(_userType);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetPlayerIcon(_userType);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetPlayerIcon(_userType);
		}
	}
	public void SetTime(int _playerNo, float _time)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetTime(_time);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetTime(_time);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetTime(_time);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetTime(_time);
		}
	}
	public void ShowGoalRank(int _playerNo, int _rankNum)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].ShowGoalRankIcon(_rankNum);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].ShowGoalRankIcon(_rankNum);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].ShowGoalRankIcon(_rankNum);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].ShowGoalRankIcon(_rankNum);
		}
	}
	public void SetObstacleCautionOrderInLayer(int _playerNo)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetObstacleCautionOrderInLayer(_playerNo);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetObstacleCautionOrderInLayer(_playerNo);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetObstacleCautionOrderInLayer(_playerNo);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetObstacleCautionOrderInLayer(_playerNo);
		}
	}
	public void SetObstacleCautionIconPos(int _playerNo, Vector3 _pos)
	{
		Vector3 obstacleCautionIconPos = globalCamera.ScreenToWorldPoint(_pos);
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetObstacleCautionIconPos(obstacleCautionIconPos);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetObstacleCautionIconPos(obstacleCautionIconPos);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetObstacleCautionIconPos(obstacleCautionIconPos);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetObstacleCautionIconPos(obstacleCautionIconPos);
		}
	}
	public void ShowObstacleDropCautionIcon(int _playerNo)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].ShowObstacleDropCautionIcon();
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].ShowObstacleDropCautionIcon();
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].ShowObstacleDropCautionIcon();
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].ShowObstacleDropCautionIcon();
		}
	}
	public void StopObstacleDropCautionIcon(int _playerNo)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].StopObstacleDropCautionIcon();
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].StopObstacleDropCautionIcon();
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].StopObstacleDropCautionIcon();
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].StopObstacleDropCautionIcon();
		}
	}
	public void SetThrowUIPos(int _playerNo, Vector3 _pos)
	{
		Vector3 throwUIPos = globalCamera.ScreenToWorldPoint(_pos);
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetThrowUIPos(throwUIPos);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetThrowUIPos(throwUIPos);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetThrowUIPos(throwUIPos);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetThrowUIPos(throwUIPos);
		}
	}
	public void SetThrowControllerBalloonActive(int _playerNo, bool _isActive)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetThrowControllerBalloonActive(_isActive);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetThrowControllerBalloonActive(_isActive);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetThrowControllerBalloonActive(_isActive);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetThrowControllerBalloonActive(_isActive);
		}
	}
	public void SetControllerBalloonActive(int _playerNo, ControllerUIType _controllerUIType, bool _isFade, bool _isActive)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetControllerBalloonActive((int)_controllerUIType, _isFade, _isActive);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetControllerBalloonActive((int)_controllerUIType, _isFade, _isActive);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetControllerBalloonActive((int)_controllerUIType, _isFade, _isActive);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetControllerBalloonActive((int)_controllerUIType, _isFade, _isActive);
		}
	}
	public void ChangeControllerUIType(int _playerNo, ControllerUIType _controllerUIType)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].ChangeControllerUIType((int)_controllerUIType);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].ChangeControllerUIType((int)_controllerUIType);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].ChangeControllerUIType((int)_controllerUIType);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].ChangeControllerUIType((int)_controllerUIType);
		}
	}
	public void GameStartAnimation()
	{
	}
	public void SetJoyConButtonPlayerType(int _playerNo)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetJoyConButtonPlayerType(_playerNo);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetJoyConButtonPlayerType(_playerNo);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetJoyConButtonPlayerType(_playerNo);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetJoyConButtonPlayerType(_playerNo);
		}
	}
	public void HideUI(int _playerNo)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].HideUI();
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].HideUI();
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].HideUI();
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].HideUI();
		}
	}
}
