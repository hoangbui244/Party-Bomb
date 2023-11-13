using System;
using UnityEngine;
public class MorphingRace_MorphingTarget_Dog : MorphingRace_MorphingTarget
{
	[Serializable]
	private struct JumpArea
	{
		public MorphingRace_MorphingTarget_Dog_JumpArea[] arrayAnchor;
	}
	[SerializeField]
	private JumpArea[] arrayJumpArea;
	public override void Init()
	{
		base.Init();
		arrayJumpArea = new JumpArea[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
	}
	public void SetJumpArea(int _playerNo, GameObject _obstacleObj)
	{
		arrayJumpArea[_playerNo].arrayAnchor = _obstacleObj.GetComponentsInChildren<MorphingRace_MorphingTarget_Dog_JumpArea>();
	}
	public void SetJumpAreaCPU(int _playerNo)
	{
		for (int i = 0; i < arrayJumpArea[_playerNo].arrayAnchor.Length; i++)
		{
			arrayJumpArea[_playerNo].arrayAnchor[i].SetCPUJumpPos();
		}
	}
	public bool CheckIsCanJumpArea(int _playerNo, int _idx, Vector3 _pos, bool _isPlayer)
	{
		if (_isPlayer)
		{
			if (arrayJumpArea[_playerNo].arrayAnchor[_idx].CheckIsCanJumpArea(_pos))
			{
				return true;
			}
		}
		else if (arrayJumpArea[_playerNo].arrayAnchor[_idx].CheckIsCanJumpCPU(_pos))
		{
			return true;
		}
		return false;
	}
	public bool CheckIsCanJumpViewUI(int _playerNo, int _idx, Vector3 _pos)
	{
		return arrayJumpArea[_playerNo].arrayAnchor[_idx].CheckIsCanJumpViewUI(_pos);
	}
	public int GetJumpAnchorLength(int _playerNo)
	{
		return arrayJumpArea[_playerNo].arrayAnchor.Length;
	}
	public Vector3 GetJumpAnchorPos(int _playerNo, int _idx)
	{
		return arrayJumpArea[_playerNo].arrayAnchor[_idx].transform.position;
	}
	public int GetJumpAreaIdx(int _playerNo, Vector3 _pos)
	{
		for (int i = 0; i < arrayJumpArea[_playerNo].arrayAnchor.Length; i++)
		{
			if (arrayJumpArea[_playerNo].arrayAnchor[i].GetJumpPassAnchorPosZ() > _pos.z)
			{
				return i;
			}
		}
		return arrayJumpArea[_playerNo].arrayAnchor.Length;
	}
	public bool CheckPassJumpArea(int _playerNo, int _idx, Vector3 _pos)
	{
		return arrayJumpArea[_playerNo].arrayAnchor[_idx].GetJumpPassAnchorPosZ() <= _pos.z;
	}
	public void SetJumpObstacleColliderActive(int _playerNo, int _idx, bool _isActive)
	{
		arrayJumpArea[_playerNo].arrayAnchor[_idx].SetJumpObstacleColliderActive(_isActive);
	}
}
