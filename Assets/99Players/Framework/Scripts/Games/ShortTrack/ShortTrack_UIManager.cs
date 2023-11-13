using UnityEngine;
public class ShortTrack_UIManager : SingletonCustom<ShortTrack_UIManager>
{
	[SerializeField]
	[Header("【１人】の時のレイアウトオブジェクト")]
	private GameObject singleLayoutObjct;
	[SerializeField]
	[Header("【２人】の時のレイアウトオブジェクト")]
	private GameObject twoLayoutObject;
	[SerializeField]
	[Header("【３，４人】の時のレイアウトオブジェクト")]
	private GameObject fourLayoutObject;
	[SerializeField]
	[Header("【１人】の時のレイアウトデ\u30fcタ")]
	private ShortTrack_LayoutData singleLayoutData;
	[SerializeField]
	[Header("【２人】の時のレイアウトデ\u30fcタ")]
	private ShortTrack_LayoutData[] multiLayoutData_TwoDisplay;
	[SerializeField]
	[Header("【３，４人】の時のレイアウトデ\u30fcタ")]
	private ShortTrack_LayoutData[] multiLayoutData_FourDisplay;
	[SerializeField]
	[Header("２人用の分割ライン")]
	private GameObject multiPartition_Two;
	[SerializeField]
	[Header("４人用の分割ライン")]
	private GameObject multiPartition_Four;
	[SerializeField]
	[Header("直線の操作説明の表示")]
	private GameObject gameOperation;
	[SerializeField]
	[Header("カ\u30fcブ時の操作説明の表示")]
	private GameObject curveGameOperation;
	public GameObject ForLayoutObject => fourLayoutObject;
	public void Init()
	{
		singleLayoutObjct.SetActive(value: false);
		twoLayoutObject.SetActive(value: false);
		fourLayoutObject.SetActive(value: false);
		multiPartition_Two.SetActive(value: false);
		multiPartition_Four.SetActive(value: false);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleLayoutObjct.SetActive(value: true);
			singleLayoutData.Init();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
		{
			twoLayoutObject.SetActive(value: true);
			for (int i = 0; i < multiLayoutData_TwoDisplay.Length; i++)
			{
				multiLayoutData_TwoDisplay[i].Init();
				SetPlayerIcon(i, SHORTTRACK.MCM.PData[i].userType);
			}
			multiPartition_Two.SetActive(value: true);
		}
		else
		{
			fourLayoutObject.SetActive(value: true);
			for (int j = 0; j < multiLayoutData_FourDisplay.Length; j++)
			{
				multiLayoutData_FourDisplay[j].Init();
				SetPlayerIcon(j, SHORTTRACK.MCM.PData[j].userType);
			}
			multiPartition_Four.SetActive(value: true);
		}
	}
	public void UpdateMethed()
	{
		RankIconScaling();
	}
	public void InfoIconHide(int _playerNum)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData.RightUpInfoIconHide();
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_TwoDisplay[_playerNum].RightUpInfoIconHide();
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_FourDisplay[_playerNum].RightUpInfoIconHide();
		}
	}
	public void StraightLineInfo()
	{
		if (singleLayoutObjct.activeSelf)
		{
			gameOperation.SetActive(value: true);
			curveGameOperation.SetActive(value: false);
		}
	}
	public void SetPlayerIcon(int _playerNum, int _userType)
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.SetPlayerIcon(_userType);
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].SetPlayerIcon(_userType);
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].SetPlayerIcon(_userType);
			}
		}
	}
	public void SetTime(int _playerNum, float _time)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 || _playerNum <= 0) && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.SetTime(_time);
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].SetTime(_time);
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].SetTime(_time);
			}
		}
	}
	public ShortTrack_DashButtonPress GetDashButtonPress(int _playerNum)
	{
		if (singleLayoutObjct.activeSelf)
		{
			return singleLayoutData.GetDashButtonPressUI();
		}
		if (twoLayoutObject.activeSelf)
		{
			return multiLayoutData_TwoDisplay[_playerNum].GetDashButtonPressUI();
		}
		if (fourLayoutObject.activeSelf)
		{
			return multiLayoutData_FourDisplay[_playerNum].GetDashButtonPressUI();
		}
		return null;
	}
	public ShortTrack_StaminaGauge GetStaminaGauge(int _playerNum)
	{
		if (singleLayoutObjct.activeSelf)
		{
			return singleLayoutData.GetStaminaGaugeUI();
		}
		if (twoLayoutObject.activeSelf)
		{
			return multiLayoutData_TwoDisplay[_playerNum].GetStaminaGaugeUI();
		}
		if (fourLayoutObject.activeSelf)
		{
			return multiLayoutData_FourDisplay[_playerNum].GetStaminaGaugeUI();
		}
		return null;
	}
	public void ChangeRankIconShow(int _playerNum, bool flg)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData.ChangeRankIconShow(flg);
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_TwoDisplay[_playerNum].ChangeRankIconShow(flg);
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_FourDisplay[_playerNum].ChangeRankIconShow(flg);
		}
	}
	public void goalRankIcon(int _playerNum)
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData.GoalRankIconMove();
		}
		else if (twoLayoutObject.activeSelf)
		{
			multiLayoutData_TwoDisplay[_playerNum].GoalRankIconMove();
		}
		else if (fourLayoutObject.activeSelf)
		{
			multiLayoutData_FourDisplay[_playerNum].GoalRankIconMove();
		}
	}
	public void SetGoalRank(int _playerNum, int _rankNum)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 || _playerNum <= 0) && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.SetGoalRankIcon(_rankNum);
				singleLayoutData.ActiveGoalRankIcon();
				singleLayoutData.NonActiveChangeRankIcon();
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].SetGoalRankIcon(_rankNum);
				multiLayoutData_TwoDisplay[_playerNum].ActiveGoalRankIcon();
				multiLayoutData_TwoDisplay[_playerNum].NonActiveChangeRankIcon();
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].SetGoalRankIcon(_rankNum);
				multiLayoutData_FourDisplay[_playerNum].ActiveGoalRankIcon();
				multiLayoutData_FourDisplay[_playerNum].NonActiveChangeRankIcon();
			}
		}
	}
	public void SetChangeRank(int _playerNum, int _rankNum, bool _isInit = false)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 || _playerNum <= 0) && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.SetChangeRankIcon(_rankNum);
				singleLayoutData.SetRankIconScale(_isInit ? 1f : 0f);
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].SetChangeRankIcon(_rankNum);
				multiLayoutData_TwoDisplay[_playerNum].SetRankIconScale(_isInit ? 1f : 0f);
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].SetChangeRankIcon(_rankNum);
				multiLayoutData_FourDisplay[_playerNum].SetRankIconScale(_isInit ? 1f : 0f);
			}
		}
	}
	public void RankIconScaling()
	{
		if (singleLayoutObjct.activeSelf)
		{
			singleLayoutData.SetRankIconScale(Mathf.Clamp(singleLayoutData.GetRankIconScale().x + Time.deltaTime * 4f, 0f, 1f));
		}
		else if (twoLayoutObject.activeSelf)
		{
			for (int i = 0; i < multiLayoutData_TwoDisplay.Length; i++)
			{
				multiLayoutData_TwoDisplay[i].SetRankIconScale(Mathf.Clamp(singleLayoutData.GetRankIconScale().x + Time.deltaTime * 4f, 0f, 1f));
			}
		}
		else if (fourLayoutObject.activeSelf)
		{
			for (int j = 0; j < multiLayoutData_FourDisplay.Length; j++)
			{
				multiLayoutData_FourDisplay[j].SetRankIconScale(Mathf.Clamp(singleLayoutData.GetRankIconScale().x + Time.deltaTime * 4f, 0f, 1f));
			}
		}
	}
	public void SetCurrentLap(int _playerNum, int _lapNum)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 || _playerNum <= 0) && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			UnityEngine.Debug.Log("SetCurrentLap : " + _playerNum.ToString() + " _lapNum : " + _lapNum.ToString());
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.SetCurrentLap(_lapNum);
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].SetCurrentLap(_lapNum);
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].SetCurrentLap(_lapNum);
			}
		}
	}
	public void PlayLastLapAnimation(int _playerNum)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1 || _playerNum <= 0) && (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 || _playerNum <= 1))
		{
			if (singleLayoutObjct.activeSelf)
			{
				singleLayoutData.PlayLastLapAnimation();
			}
			else if (twoLayoutObject.activeSelf)
			{
				multiLayoutData_TwoDisplay[_playerNum].PlayLastLapAnimation();
			}
			else if (fourLayoutObject.activeSelf)
			{
				multiLayoutData_FourDisplay[_playerNum].PlayLastLapAnimation();
			}
		}
	}
}
