using UnityEngine;
public class MorphingRace_MorphingTarget_Fish : MorphingRace_MorphingTarget
{
	[SerializeField]
	[Header("水に入った後の水上にいけない用のコライダ\u30fc")]
	private GameObject[] arrayLaneWaterUpCollider;
	[SerializeField]
	[Header("水に入る時のジャンプアンカ\u30fc")]
	private Transform[] arrayInWaterJumpAnchor;
	[SerializeField]
	[Header("水に入った時の開始位置アンカ\u30fc")]
	private Transform[] arrayInWaterStartAnchor;
	[SerializeField]
	[Header("水から出る時のジャンプアンカ\u30fc")]
	private Transform[] arrayOutWaterJumpAnchor;
	[SerializeField]
	[Header("水から出た時の着地位置アンカ\u30fc")]
	private Transform[] arrayOutWaterLandingAnchor;
	[SerializeField]
	[Header("水から出て着地したあと戻れないようにするためのコライダ\u30fc")]
	private GameObject[] arrayCantBackCollider;
	private MorphingRace_MorphingTarget_Fish_SwimArea[] arraySwimArea;
	public override void Init()
	{
		base.Init();
		arraySwimArea = new MorphingRace_MorphingTarget_Fish_SwimArea[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
	}
	public void SetLaneWaterUpColliderActive(int _playerNo, bool _isActive)
	{
		arrayLaneWaterUpCollider[_playerNo].SetActive(_isActive);
	}
	public Transform GetInWaterJumpAnchor(int _playerNo)
	{
		return arrayInWaterJumpAnchor[_playerNo];
	}
	public Transform GetInWaterStartAnchor(int _playerNo)
	{
		return arrayInWaterStartAnchor[_playerNo];
	}
	public Transform GetOutWaterJumpAnchor(int _playerNo)
	{
		return arrayOutWaterJumpAnchor[_playerNo];
	}
	public Transform GetOutWaterLandingAnchor(int _playerNo)
	{
		return arrayOutWaterLandingAnchor[_playerNo];
	}
	public void GetCantBackColliderActive(int _playerNo, bool _isActive)
	{
		arrayCantBackCollider[_playerNo].SetActive(_isActive);
	}
	public void SetSwimArea(int _playerNo, MorphingRace_GenerateObstacle _obstacleObj)
	{
		for (int i = 0; i < _obstacleObj.GetArrayGenerateAnchor().Length; i++)
		{
			MorphingRace_MorphingTarget_Fish_SwimArea componentInChildren = _obstacleObj.GetArrayGenerateAnchor()[i].GetComponentInChildren<MorphingRace_MorphingTarget_Fish_SwimArea>();
			arraySwimArea[_playerNo] = componentInChildren;
			arraySwimArea[_playerNo].Init();
		}
	}
	public bool CheckPassSwimArea(int _playerNo, int _idx, Vector3 _pos)
	{
		if (arraySwimArea[_playerNo].CheckPassSwimArea(_idx, _pos))
		{
			return true;
		}
		return false;
	}
	public Vector3[] GetSwimAreaPosList(int _playerNo)
	{
		return arraySwimArea[_playerNo].GetSwimAreaPosList();
	}
	public Vector3 GetSwimAreaPos(int _playerNo, int _idx)
	{
		return arraySwimArea[_playerNo].GetSwimAreaPos(_idx);
	}
	public int GetSwimAreaLength(int _playerNo)
	{
		return arraySwimArea[_playerNo].GetSwimAreaLength();
	}
}
