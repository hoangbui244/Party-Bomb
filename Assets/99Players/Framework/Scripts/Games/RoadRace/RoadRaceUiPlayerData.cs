using UnityEngine;
public class RoadRaceUiPlayerData : MonoBehaviour
{
	[SerializeField]
	private GameObject[] playerAnchors;
	private int layoutIdx;
	[SerializeField]
	private GameObject[] hideThreePlayerObjs;
	public void Init(int _playerNum)
	{
		switch (_playerNum)
		{
		case 2:
			layoutIdx = 1;
			break;
		case 3:
		case 4:
			layoutIdx = 2;
			break;
		}
		playerAnchors[layoutIdx].SetActive(value: true);
		if (_playerNum == 3)
		{
			for (int i = 0; i < hideThreePlayerObjs.Length; i++)
			{
				hideThreePlayerObjs[i].SetActive(value: false);
			}
		}
	}
	public void SetTime(float _time)
	{
	}
	public void SetPlayerIconActive(bool _isActive)
	{
	}
	public void SetPlayerIcon(int _userType)
	{
	}
	public void SetThrowUIPos(Vector3 _pos)
	{
	}
	public void SetThrowControllerBalloonActive(bool _isActive)
	{
	}
	public void ShowGoalRankIcon(int _rankNum)
	{
	}
	public void SetObstacleCautionOrderInLayer(int _playerNo)
	{
	}
	public void SetObstacleCautionIconPos(Vector3 _pos)
	{
	}
	public void ShowObstacleDropCautionIcon()
	{
	}
	public void StopObstacleDropCautionIcon()
	{
	}
	public void SetControllerBalloonActive(int _balloonIdx, bool _isFade, bool _isActive)
	{
	}
	public void ChangeControllerUIType(int _controllerTypeIdx)
	{
	}
	public void SetJoyConButtonPlayerType(int _playerNo)
	{
	}
	public void HideUI()
	{
	}
}
