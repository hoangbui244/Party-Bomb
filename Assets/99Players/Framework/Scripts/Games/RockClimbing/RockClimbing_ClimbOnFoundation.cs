using UnityEngine;
public class RockClimbing_ClimbOnFoundation : MonoBehaviour
{
	[SerializeField]
	[Header("土台の種類")]
	private RockClimbing_ClimbingWallManager.ClimbOnFoundationType climbOnFoundationType;
	[SerializeField]
	[Header("ゴ\u30fcル用の判定かどうかのフラグ")]
	private bool isGoal;
	[SerializeField]
	[Header("鉤縄を引っかけるポイントクラスグル\u30fcプ")]
	private RockClimbing_GrapplingHookPoint_Group[] arrayGrapplingHookPointGroup;
	[SerializeField]
	[Header("壁のコライダ\u30fc")]
	private Collider wallCollider;
	[SerializeField]
	[Header("奥行き移動制限アンカ\u30fc")]
	private Transform moveZLimitAnchor;
	[SerializeField]
	[Header("プレイヤ\u30fcが登る判定となるコライダ\u30fcクラス")]
	private RockClimbing_ClimbOnCollider[] climbOnCollider;
	[SerializeField]
	[Header("プレイヤ\u30fcが登る時のアニメ\u30fcションアンカ\u30fc")]
	private Transform[] arrayClimbOnAnchor;
	public void Init()
	{
		for (int i = 0; i < arrayGrapplingHookPointGroup.Length; i++)
		{
			arrayGrapplingHookPointGroup[i].Init(this);
		}
		for (int j = 0; j < climbOnCollider.Length; j++)
		{
			climbOnCollider[j].Init(this);
		}
	}
	public RockClimbing_ClimbingWallManager.ClimbOnFoundationType GetClimbOnFoundationType()
	{
		return climbOnFoundationType;
	}
	public bool GetIsGoal()
	{
		return isGoal;
	}
	public RockClimbing_GrapplingHookPoint_Group GetGrapplingHookPointGroup(int _playerNo)
	{
		return arrayGrapplingHookPointGroup[_playerNo];
	}
	public RockClimbing_GrapplingHookPoint GetGrapplingHookPoint(int _playerNo, int _idx = 0)
	{
		return arrayGrapplingHookPointGroup[_playerNo].GetArrayGrapplingHookPoint()[_idx];
	}
	public RockClimbing_GrapplingHookPoint GetNearGrapplingHookPoint(int _playerNo, Vector3 _pos)
	{
		int idx = 0;
		float num = CalcManager.Length(_pos, GetGrapplingHookPoint(_playerNo, idx).transform.position);
		for (int i = 1; i < arrayGrapplingHookPointGroup[_playerNo].GetArrayGrapplingHookPoint().Length; i++)
		{
			float num2 = CalcManager.Length(_pos, GetGrapplingHookPoint(_playerNo, i).transform.position);
			if (num2 < num)
			{
				num = num2;
				idx = i;
			}
		}
		return GetGrapplingHookPoint(_playerNo, idx);
	}
	public Collider GetWallCollider()
	{
		return wallCollider;
	}
	public Transform GetMoveZLimitAnchor()
	{
		return moveZLimitAnchor;
	}
	public RockClimbing_ClimbOnCollider GetClimbOnCollider(int _playerNo)
	{
		return climbOnCollider[_playerNo];
	}
	public Transform[] GetArrayClimbOnAnchor()
	{
		return arrayClimbOnAnchor;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Vector3 vector = new Vector3(wallCollider.bounds.min.x, wallCollider.bounds.max.y, moveZLimitAnchor.position.z);
		Vector3 vector2 = new Vector3(wallCollider.bounds.min.x, wallCollider.bounds.min.y, moveZLimitAnchor.position.z);
		Vector3 vector3 = new Vector3(wallCollider.bounds.max.x, wallCollider.bounds.max.y, moveZLimitAnchor.position.z);
		Vector3 vector4 = new Vector3(wallCollider.bounds.max.x, wallCollider.bounds.min.y, moveZLimitAnchor.position.z);
		Gizmos.DrawLine(vector, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector2);
		Gizmos.DrawLine(vector2, vector);
	}
}
