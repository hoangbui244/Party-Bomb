using System;
using UnityEngine;
public class MorphingRace_MorphingTarget : MonoBehaviour
{
	[Serializable]
	private struct MorphingTargetCollider
	{
		public MorphingRace_MorphingTarget_MorphingPoint startPoint;
		public MorphingRace_MorphingTarget_MorphingPoint endPoint;
	}
	[SerializeField]
	[Header("変身する動物の種類")]
	private MorphingRace_FieldManager.TargetPrefType morphingTargetType;
	[SerializeField]
	[Header("変身ポイントを非表示にするかどうかのフラグ")]
	private bool isHideMorphingPoint;
	[SerializeField]
	[Header("進行方向用のル\u30fcトを設定するかどうかのフラグ")]
	private bool isRootPos;
	[SerializeField]
	[Header("変身ポイント")]
	private MorphingTargetCollider[] arrayMorphingTargetPoint;
	[SerializeField]
	[Header("障害物を生成するアンカ\u30fc")]
	protected Transform[] arrayObstacleAnchor;
	[SerializeField]
	[Header("開始位置アンカ\u30fc")]
	private Transform startAnchor;
	[SerializeField]
	[Header("終了位置アンカ\u30fc")]
	private Transform endAnchor;
	[SerializeField]
	[Header("進行方向用のル\u30fcト")]
	private BezierPosition[] arrayBezierPosition;
	private MorphingRace_GenerateObstacle obstacleObj;
	[SerializeField]
	[Header("デバッグ用：ライン")]
	private Collider[] arrayDebugColliderLine;
	public virtual void Init()
	{
	}
	public void BezierPositionInit(int _playerNo)
	{
		arrayBezierPosition[_playerNo].Init();
	}
	public void InitMorphingTargetPoint(int _playerNo)
	{
		if (arrayMorphingTargetPoint[_playerNo].startPoint.GetIsHidePoint())
		{
			arrayMorphingTargetPoint[_playerNo].startPoint.Hide();
		}
		else
		{
			arrayMorphingTargetPoint[_playerNo].startPoint.Init(this);
		}
		if (arrayMorphingTargetPoint[_playerNo].endPoint.GetIsHidePoint())
		{
			arrayMorphingTargetPoint[_playerNo].endPoint.Hide();
		}
		else
		{
			arrayMorphingTargetPoint[_playerNo].endPoint.Init(this);
		}
	}
	public void GenerateObstacle(int _playerNo, MorphingRace_GenerateObstacle _obstacle)
	{
		MorphingRace_GenerateObstacle morphingRace_GenerateObstacle = null;
		if (obstacleObj == null)
		{
			morphingRace_GenerateObstacle = UnityEngine.Object.Instantiate(_obstacle, arrayObstacleAnchor[_playerNo]);
			morphingRace_GenerateObstacle.GenerateObstacle(morphingTargetType);
			obstacleObj = morphingRace_GenerateObstacle;
		}
		else
		{
			morphingRace_GenerateObstacle = UnityEngine.Object.Instantiate(obstacleObj, arrayObstacleAnchor[_playerNo]);
		}
		switch (morphingTargetType)
		{
		case MorphingRace_FieldManager.TargetPrefType.Mouse:
		{
			MorphingRace_MorphingTarget_Mouse morphingRace_MorphingTarget_Mouse = (MorphingRace_MorphingTarget_Mouse)this;
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_playerNo][0] >= 4)
			{
				morphingRace_MorphingTarget_Mouse.BuildNavMesh(morphingRace_GenerateObstacle);
			}
			break;
		}
		case MorphingRace_FieldManager.TargetPrefType.Eagle:
			((MorphingRace_MorphingTarget_Eagle)this).SetThroughArea(_playerNo, morphingRace_GenerateObstacle.gameObject);
			break;
		case MorphingRace_FieldManager.TargetPrefType.Fish:
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_playerNo][0] >= 4)
			{
				((MorphingRace_MorphingTarget_Fish)this).SetSwimArea(_playerNo, morphingRace_GenerateObstacle);
			}
			break;
		case MorphingRace_FieldManager.TargetPrefType.Dog:
		{
			MorphingRace_MorphingTarget_Dog morphingRace_MorphingTarget_Dog = (MorphingRace_MorphingTarget_Dog)this;
			morphingRace_MorphingTarget_Dog.SetJumpArea(_playerNo, morphingRace_GenerateObstacle.gameObject);
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_playerNo][0] >= 4)
			{
				morphingRace_MorphingTarget_Dog.SetJumpAreaCPU(_playerNo);
			}
			break;
		}
		}
	}
	public MorphingRace_FieldManager.TargetPrefType GetMorphingCharacterType()
	{
		return morphingTargetType;
	}
	public bool GetIsRootPos()
	{
		return isRootPos;
	}
	public Vector3 GetObstacleAnchorPos(int _playerNo)
	{
		return arrayObstacleAnchor[_playerNo].transform.position;
	}
	public Vector3 GetStartAnchorPos()
	{
		return startAnchor.position;
	}
	public Vector3 GetEndAnchorPos()
	{
		return endAnchor.position;
	}
	public BezierPosition GetBezierPosition(int _playerNo)
	{
		return arrayBezierPosition[_playerNo];
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 from = new Vector3(startAnchor.position.x, 0.1f, startAnchor.position.z);
		Vector3 to = new Vector3(endAnchor.position.x, 0.1f, endAnchor.position.z);
		Gizmos.DrawLine(from, to);
		Gizmos.color = Color.black;
		for (int i = 0; i < arrayDebugColliderLine.Length; i++)
		{
			if (i == 0)
			{
				from = new Vector3(arrayDebugColliderLine[i].bounds.max.x, 0.1f, startAnchor.position.z);
				to = new Vector3(arrayDebugColliderLine[i].bounds.max.x, 0.1f, endAnchor.position.z);
			}
			else if (i == arrayDebugColliderLine.Length - 1)
			{
				from = new Vector3(arrayDebugColliderLine[i].bounds.min.x, 0.1f, startAnchor.position.z);
				to = new Vector3(arrayDebugColliderLine[i].bounds.min.x, 0.1f, endAnchor.position.z);
			}
			else
			{
				from = new Vector3(arrayDebugColliderLine[i].transform.position.x, 0.1f, startAnchor.position.z);
				to = new Vector3(arrayDebugColliderLine[i].transform.position.x, 0.1f, endAnchor.position.z);
			}
			Gizmos.DrawLine(from, to);
		}
	}
}
