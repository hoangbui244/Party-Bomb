using UnityEngine;
public class Canoe_UIManager : SingletonCustom<Canoe_UIManager>
{
	public enum ControllerUIType
	{
		Common
	}
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
	private Canoe_LayoutData[] singleLayoutData;
	[SerializeField]
	[Header("【２人】の時のレイアウトデ\u30fcタ")]
	private Canoe_LayoutData[] multiLayoutData_Two;
	[SerializeField]
	[Header("【３人】の時のレイアウトデ\u30fcタ")]
	private Canoe_LayoutData[] multiLayoutData_Three;
	[SerializeField]
	[Header("【４人】の時のレイアウトデ\u30fcタ")]
	private Canoe_LayoutData[] multiLayoutData_Four;
	public void Init()
	{
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
				SetPlayerIconActive(i, _isActive: false);
				SetJoyConButtonPlayerType(i);
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
		{
			twoLayoutObject.SetActive(value: true);
			for (int j = 0; j < multiLayoutData_Two.Length; j++)
			{
				multiLayoutData_Two[j].Init(j);
				SetPlayerIconActive(j, _isActive: true);
				SetPlayerIcon(j, (int)SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(j).GetUserType());
				SetJoyConButtonPlayerType(j);
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
		{
			threeLayoutObject.SetActive(value: true);
			for (int k = 0; k < multiLayoutData_Three.Length; k++)
			{
				multiLayoutData_Three[k].Init(k);
				SetPlayerIconActive(k, _isActive: true);
				SetPlayerIcon(k, (int)SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(k).GetUserType());
				SetJoyConButtonPlayerType(k);
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4)
		{
			fourLayoutObject.SetActive(value: true);
			for (int l = 0; l < multiLayoutData_Four.Length; l++)
			{
				multiLayoutData_Four[l].Init(l);
				SetPlayerIconActive(l, _isActive: true);
				SetPlayerIcon(l, (int)SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(l).GetUserType());
				SetJoyConButtonPlayerType(l);
			}
		}
	}
	public void UpdateMethod()
	{
		SetCurrentRankIconScaling(Time.deltaTime * 4f);
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
	public void SetStaminaGauge(int _playerNo, float _staminaLerp, bool _isUseUpStamina)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData[_playerNo].SetStaminaGauge(_staminaLerp, _isUseUpStamina);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_Two[_playerNo].SetStaminaGauge(_staminaLerp, _isUseUpStamina);
		}
		else if (threeLayoutObject.activeSelf)
		{
			multiLayoutData_Three[_playerNo].SetStaminaGauge(_staminaLerp, _isUseUpStamina);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_Four[_playerNo].SetStaminaGauge(_staminaLerp, _isUseUpStamina);
		}
	}
	public void SetCurrentRankIconScaling(float _scaleTime)
	{
		if (singleLayoutObjct.activeSelf)
		{
			for (int i = 0; i < singleLayoutData.Length; i++)
			{
				singleLayoutData[i].SetCurrentRankIconScaling(_scaleTime);
			}
		}
		else if (twoLayoutObject.activeSelf)
		{
			for (int j = 0; j < multiLayoutData_Two.Length; j++)
			{
				multiLayoutData_Two[j].SetCurrentRankIconScaling(_scaleTime);
			}
		}
		else if (threeLayoutObject.activeSelf)
		{
			for (int k = 0; k < multiLayoutData_Three.Length; k++)
			{
				multiLayoutData_Three[k].SetCurrentRankIconScaling(_scaleTime);
			}
		}
		else if (fourLayoutObject.activeSelf)
		{
			for (int l = 0; l < multiLayoutData_Four.Length; l++)
			{
				multiLayoutData_Four[l].SetCurrentRankIconScaling(_scaleTime);
			}
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
