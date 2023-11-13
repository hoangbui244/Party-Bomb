using System;
using UnityEngine;
public class MorphingRace_MorphingTarget_Eagle : MorphingRace_MorphingTarget
{
	[Serializable]
	private struct ThroughArea
	{
		public MorphingRace_MorphingTarget_Eagle_ThroughArea[] arrayAnchor;
	}
	[SerializeField]
	[Header("降下して着地する位置アンカ\u30fc")]
	private Transform[] arrayDescentLandingAnchor;
	[SerializeField]
	private ThroughArea[] arrayThroughArea;
	public override void Init()
	{
		base.Init();
		arrayThroughArea = new ThroughArea[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
	}
	public Transform GetDescentLandingAnchor(int _playerNo)
	{
		return arrayDescentLandingAnchor[_playerNo];
	}
	public void SetThroughArea(int _playerNo, GameObject _obstacleObj)
	{
		arrayThroughArea[_playerNo].arrayAnchor = _obstacleObj.GetComponentsInChildren<MorphingRace_MorphingTarget_Eagle_ThroughArea>();
		for (int i = 0; i < arrayThroughArea[_playerNo].arrayAnchor.Length; i++)
		{
			arrayThroughArea[_playerNo].arrayAnchor[i].SetThroughArea();
		}
	}
	public bool CheckPassThroughArea(int _playerNo, int _idx, Vector3 _pos)
	{
		if (arrayThroughArea[_playerNo].arrayAnchor[_idx].CheckPassThroughArea(_pos))
		{
			return true;
		}
		return false;
	}
	public Vector3 GetThroughAreaPos(int _playerNo, int _idx)
	{
		return arrayThroughArea[_playerNo].arrayAnchor[_idx].GetThroughArea().position;
	}
	public int GetThroughAnchorLength(int _playerNo)
	{
		return arrayThroughArea[_playerNo].arrayAnchor.Length;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0] < 4)
			{
				continue;
			}
			Vector3 from = Vector3.zero;
			Vector3 vector = Vector3.zero;
			for (int j = 0; j < arrayThroughArea[i].arrayAnchor.Length; j++)
			{
				if (j == 0)
				{
					from = base.transform.position;
					from.x = arrayThroughArea[i].arrayAnchor[j].GetThroughArea().position.x;
				}
				else
				{
					from = vector;
				}
				vector = arrayThroughArea[i].arrayAnchor[j].GetThroughArea().position;
				Gizmos.DrawLine(from, vector);
			}
		}
	}
}
