using UnityEngine;
public class MorphingRace_MorphingTarget_Dog_JumpArea : MonoBehaviour
{
	[SerializeField]
	[Header("ジャンプ可能エリアの左上アンカ\u30fc")]
	private Transform jumpAnchor_LeftUp;
	[SerializeField]
	[Header("ジャンプ可能エリアの右下アンカ\u30fc")]
	private Transform jumpAnchor_RightDown;
	[SerializeField]
	[Header("「ジャンプ」UIを表示するエリアの右下アンカ\u30fc")]
	private Transform jumpAnchor_RightDown_ViewUI;
	[SerializeField]
	[Header("ジャンプ可能エリアの右下アンカ\u30fc(CPU用)")]
	private Transform jumpAnchor_RightDown_CPU;
	private Vector3 cpuJumpPos;
	[SerializeField]
	[Header("ジャンプ通過の判定用のアンカ\u30fc")]
	private Transform jumpPassAnchor;
	[SerializeField]
	[Header("ジャンプする障害物のコライダ\u30fc")]
	private Collider[] arrayJumpObstacleCollider;
	public bool CheckIsCanJumpArea(Vector3 _pos)
	{
		if (jumpAnchor_LeftUp.position.x <= _pos.x && jumpAnchor_RightDown.position.x >= _pos.x && jumpAnchor_LeftUp.position.z >= _pos.z)
		{
			return jumpAnchor_RightDown.position.z <= _pos.z;
		}
		return false;
	}
	public bool CheckIsCanJumpViewUI(Vector3 _pos)
	{
		if (jumpAnchor_LeftUp.position.x <= _pos.x && jumpAnchor_RightDown_ViewUI.position.x >= _pos.x && jumpAnchor_LeftUp.position.z >= _pos.z)
		{
			return jumpAnchor_RightDown_ViewUI.position.z <= _pos.z;
		}
		return false;
	}
	public bool CheckIsCanJumpCPU(Vector3 _pos)
	{
		if (jumpAnchor_LeftUp.position.x <= _pos.x && cpuJumpPos.x >= _pos.x && jumpAnchor_LeftUp.position.z >= _pos.z)
		{
			return cpuJumpPos.z <= _pos.z;
		}
		return false;
	}
	public void SetCPUJumpPos()
	{
		cpuJumpPos = jumpAnchor_RightDown_CPU.position;
		float maxInclusive = (jumpAnchor_LeftUp.position.z - jumpAnchor_RightDown_CPU.position.z) / 3f * 2f;
		cpuJumpPos.z = jumpAnchor_RightDown_CPU.position.z + UnityEngine.Random.Range(0f, maxInclusive);
	}
	public float GetJumpPassAnchorPosZ()
	{
		return jumpPassAnchor.position.z;
	}
	public void SetJumpObstacleColliderActive(bool _isActive)
	{
		for (int i = 0; i < arrayJumpObstacleCollider.Length; i++)
		{
			arrayJumpObstacleCollider[i].enabled = _isActive;
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Vector3 position = jumpAnchor_LeftUp.position;
		position.x = jumpAnchor_RightDown.position.x;
		Vector3 position2 = jumpAnchor_RightDown.position;
		position2.x = jumpAnchor_LeftUp.position.x;
		Gizmos.DrawLine(jumpAnchor_LeftUp.position, position);
		Gizmos.DrawLine(position, jumpAnchor_RightDown.position);
		Gizmos.DrawLine(jumpAnchor_RightDown.position, position2);
		Gizmos.DrawLine(position2, jumpAnchor_LeftUp.position);
		Gizmos.color = Color.green;
		position = jumpAnchor_LeftUp.position;
		position.x = jumpAnchor_RightDown_ViewUI.position.x;
		position2 = jumpAnchor_RightDown_ViewUI.position;
		position2.x = jumpAnchor_LeftUp.position.x;
		Gizmos.DrawLine(jumpAnchor_LeftUp.position, position);
		Gizmos.DrawLine(position, jumpAnchor_RightDown_ViewUI.position);
		Gizmos.DrawLine(jumpAnchor_RightDown_ViewUI.position, position2);
		Gizmos.DrawLine(position2, jumpAnchor_LeftUp.position);
		Gizmos.color = Color.white;
		position = jumpAnchor_LeftUp.position;
		position.x = jumpAnchor_RightDown_CPU.position.x;
		position2 = jumpAnchor_RightDown_CPU.position;
		position2.x = jumpAnchor_LeftUp.position.x;
		Gizmos.DrawLine(jumpAnchor_LeftUp.position, position);
		Gizmos.DrawLine(position, jumpAnchor_RightDown_CPU.position);
		Gizmos.DrawLine(jumpAnchor_RightDown_CPU.position, position2);
		Gizmos.DrawLine(position2, jumpAnchor_LeftUp.position);
	}
}
