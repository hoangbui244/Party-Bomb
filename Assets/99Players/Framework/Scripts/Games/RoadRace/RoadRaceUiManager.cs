using UnityEngine;
public class RoadRaceUiManager : SingletonCustom<RoadRaceUiManager>
{
	private Camera globalCamera;
	[SerializeField]
	private RoadRaceUiRaceData raceData;
	[SerializeField]
	private RoadRaceUiRaceOptionData raceOptionData;
	[SerializeField]
	private RoadRaceUiPlayerData playerData;
	public bool IsCanChangeRank
	{
		get
		{
			return raceData.IsCanChangeRank;
		}
		set
		{
			raceData.IsCanChangeRank = value;
		}
	}
	public void Init(int _playerNum)
	{
		raceData.Init(_playerNum);
		raceOptionData.Init(_playerNum);
		playerData.Init(_playerNum);
	}
	public void UpdateMethod()
	{
		raceData.UpdateMethod();
		raceOptionData.UpdateMethod();
	}
	public void SetPlayerIcon(int _playerNo, int _userType)
	{
	}
	public void ShowRankSprite(bool _isFade)
	{
		raceData.ShowRankSprite(_isFade);
	}
	public void SetTime(int _playerNo, float _time)
	{
		raceData.SetTime(_playerNo, _time);
	}
	public void ChangeRankNum(int _no, int _rank)
	{
		raceData.ChangeRankNum(_no, _rank);
	}
	public void ShowGoalRank(int _playerNo, int _rank)
	{
		raceData.SetResultRankSprite(_playerNo, _rank);
	}
	public void HideUI(int _playerNo)
	{
	}
	public void SetLap(int _no, int _lap)
	{
	}
	public void PlayRapEffect(int _no, int _lap)
	{
	}
	public void PlayLastOneEffect(int _no)
	{
	}
	public void ShowReverseRun(int _no, bool _isShow)
	{
		if (_isShow)
		{
			raceData.ReverseRunON(_no);
		}
		else
		{
			raceData.ReverseRunOFF(_no);
		}
	}
}
