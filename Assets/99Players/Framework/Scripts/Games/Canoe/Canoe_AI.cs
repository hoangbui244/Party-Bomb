using UnityEngine;
using UnityStandardAssets.Utility;
public class Canoe_AI : MonoBehaviour
{
	private Canoe_Player player;
	private int aiStrength;
	private float colliderRadius;
	private WaypointProgressTracker wptAI;
	private float obstacleCheckTime;
	private readonly float OBSTACLE_CHECK_TIME = 3f;
	protected int wallLayerMask;
	private string[] wallLayerMaskNameList = new string[1]
	{
		"Wall"
	};
	private Collider[] arrayOverLapCollider = new Collider[4];
	private float MOVE_SPEED_MAG;
	private float ROT_SPEED_MAG;
	private float AVOID_OBSTACLE_TIME;
	private readonly float RELOTTERY_TIME = 3f;
	private float relotteryTime;
	public void Init(Canoe_Player _player)
	{
		player = _player;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		relotteryTime = 0f;
		SetLottery();
		CapsuleCollider collider = player.GetCollider();
		colliderRadius = collider.radius * collider.transform.localScale.x * base.transform.localScale.x;
		wallLayerMask = LayerMask.GetMask(wallLayerMaskNameList);
	}
	public void SetLottery()
	{
		float[] array = Canoe_Define.CPU_MOVE_SPEED_MAG[aiStrength];
		MOVE_SPEED_MAG = UnityEngine.Random.Range(array[0], array[1]);
		float[] array2 = Canoe_Define.CPU_ROT_SPEED_MAG[aiStrength];
		ROT_SPEED_MAG = UnityEngine.Random.Range(array2[0], array2[1]);
		float[] array3 = Canoe_Define.CPU_AVOID_OBSTACLE_TIME[aiStrength];
		AVOID_OBSTACLE_TIME = UnityEngine.Random.Range(array3[0], array3[1]);
	}
	public void SetWaypointCircuitAI(WaypointProgressTracker _wptAI)
	{
		wptAI = _wptAI;
		WaypointCircuit waypointCircuitAI = SingletonCustom<Canoe_CourseManager>.Instance.GetWaypointCircuitAI(player.GetPlayerNo());
		waypointCircuitAI.gameObject.SetActive(value: true);
		wptAI.setCircuit = waypointCircuitAI;
		wptAI.progressDistance = 0f;
	}
	public void UpdateMethod()
	{
		relotteryTime += Time.deltaTime;
		if (relotteryTime > RELOTTERY_TIME)
		{
			relotteryTime = 0f;
			SetLottery();
		}
		if (CheckObstacle())
		{
			obstacleCheckTime += Time.deltaTime;
		}
		else
		{
			obstacleCheckTime = 0f;
		}
		float x = (Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * wptAI.target.forward).x;
		Vector3 lhs = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
		Vector3 vector = wptAI.progressPoint.position - base.transform.position;
		vector.y = 0f;
		Vector3 vector2 = Vector3.Cross(lhs, vector.normalized);
		float x2 = (!(obstacleCheckTime > AVOID_OBSTACLE_TIME)) ? (Mathf.Clamp(x * 1.25f + vector2.y * 1f, -1f, 1f) * ROT_SPEED_MAG) : Mathf.Sign(vector2.y);
		player.AcceleMoveInput();
		player.SetMoveDir(new Vector3(x2, 0f, 0f));
		UnityEngine.Debug.Log("x : " + x.ToString());
		UnityEngine.Debug.Log("cross.y : " + vector2.y.ToString());
		UnityEngine.Debug.Log("steer : " + x2.ToString());
	}
	public bool CheckObstacle()
	{
		return Physics.OverlapSphereNonAlloc(player.GetObstacleCheckAnchor().position, colliderRadius, arrayOverLapCollider, wallLayerMask) > 0;
	}
	public float GetMoveSpeedMag()
	{
		return MOVE_SPEED_MAG;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(player.GetObstacleCheckAnchor().position, colliderRadius);
	}
}
