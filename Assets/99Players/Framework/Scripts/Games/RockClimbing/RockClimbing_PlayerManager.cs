using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RockClimbing_PlayerManager : SingletonCustom<RockClimbing_PlayerManager>
{
	public enum State
	{
		MOVE,
		THROW,
		CLIMBING,
		CLIMB_ON
	}
	[SerializeField]
	[Header("プレイヤ\u30fcル\u30fcト配列")]
	private GameObject[] arrayPlayerRoot;
	[SerializeField]
	[Header("プレイヤ\u30fc配列")]
	private RockClimbing_Player[] arrayPlayer;
	[SerializeField]
	[Header("投げた鉤縄を格納するル－ト")]
	private Transform[] arrayThrowGrapplingHookRoot;
	[SerializeField]
	[Header("鉤縄を投げるアンカ\u30fc")]
	private Transform[] arrayThrowAnchor;
	[SerializeField]
	[Header("鉤縄のマテリアル配列")]
	private Material[] arrayGrapplingHookMat;
	private readonly float MOVE_SPEED = 2f;
	private readonly float BASE_MOVE_SPEED = 150f;
	private readonly float CORRECTION_MOVE_SPEED = 1.25f;
	private readonly float MAX_MOVE_SPEED = 1.5f;
	[SerializeField]
	[Header("鉤縄を投げた時の現在の高さからの最大距離")]
	private float THROW_GRAPPLING_HOOK_MAX_DISTANCE;
	[SerializeField]
	[Header("鉤縄を投げた時の飛ぶ速度")]
	private float THROW_GRAPPLING_HOOK_FLY_SPEED;
	private readonly float THROW_ANGLE_ON_ROOF = 12.5f;
	private readonly float COLLECT_ROPE_TIME = 0.5f;
	[SerializeField]
	[Header("鉤縄を登る時の補正座標")]
	private Vector3 CLIMBING_ROPE_DIFF_POS;
	private readonly float MAX_INPUT_INTERVAL = 0.5f;
	private readonly float MAX_CLIMBING_SPEED = 7f;
	private readonly float MIN_CLIMBING_SPEED = 5f;
	private readonly float CLIMBING_BASE_POWER = 25f;
	private readonly float HIT_OBSTACLE_INTERVAL = 1f;
	private readonly float HIT_OBSTACLE_DOWN_POWER = 50f;
	[SerializeField]
	[Header("土台の左右の端にならないようにする範囲")]
	private readonly float HOOK_POINT_COLLIDER_RANGE = 0.5f;
	private float GOAL_POS_MIN_DISTANCE = 2f;
	private float GOAL_POS_MAX_DISTANCE = 4.5f;
	private float GOAL_POS_MIN_MOVE_TIME = 0.5f;
	private float GOAL_POS_MAX_MOVE_TIME = 1f;
	public void Init()
	{
		Vector3[] array = new Vector3[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			array[i] = arrayPlayerRoot[i].transform.localPosition;
		}
		array.Shuffle();
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			arrayPlayerRoot[j].transform.localPosition = array[j];
			arrayPlayer[j].SetClimbingWall(SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetClimbingWall(0));
			arrayPlayer[j].Init(j, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0]);
		}
	}
	public void SetCpuFirstClimbOnFoundation()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		float num = Random.Range(RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] - 0.2f, RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] + 0.2f);
		int num2 = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		int num3 = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - num2;
		RockClimbing_ClimbOnFoundationObject_Group.ClimbOnFoundationAnchor climbOnFoundationAnchor = SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetClimbingWall(0).GetClimbOnFoundationObjectGroup().GetArrayClimbOnFoundationAnchor()[0];
		RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundation = climbOnFoundationAnchor.GetArrayClimbOnFoundation();
		if (Random.Range(0f, 1f) < num)
		{
			UnityEngine.Debug.Log("一番近くにある引っかけるポイントに移動する場合");
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < num2; i++)
			{
				dictionary.Clear();
				for (int j = 0; j < arrayClimbOnFoundation.Length; j++)
				{
					Collider collider = arrayClimbOnFoundation[j].GetGrapplingHookPoint(num3).GetCollider();
					float num4 = collider.bounds.min.x + HOOK_POINT_COLLIDER_RANGE;
					float num5 = collider.bounds.max.x - HOOK_POINT_COLLIDER_RANGE;
					if (arrayThrowAnchor[num3].position.x >= num4 && arrayThrowAnchor[num3].position.x <= num5)
					{
						dictionary.Add(j, 0f);
					}
					else if (arrayThrowAnchor[num3].position.x < num4)
					{
						float value = Mathf.Abs(arrayThrowAnchor[num3].position.x - num4);
						dictionary.Add(j, value);
					}
					else if (arrayThrowAnchor[num3].position.x > num5)
					{
						float value2 = Mathf.Abs(arrayThrowAnchor[num3].position.x - num5);
						dictionary.Add(j, value2);
					}
				}
				int num6 = 0;
				num6 = ((dictionary.Count <= 1) ? dictionary.Keys.FirstOrDefault() : (from v in dictionary
					orderby v.Value
					select v.Key).FirstOrDefault());
				arrayPlayer[num3].SetCpuFirstClimbOnFoundation(arrayClimbOnFoundation[num6]);
				num3++;
			}
			return;
		}
		UnityEngine.Debug.Log("ランダムで足場のIdxを設定する場合");
		arrayClimbOnFoundation.Shuffle();
		for (int k = 0; k < num2; k++)
		{
			if (k >= arrayClimbOnFoundation.Length)
			{
				arrayPlayer[num3].SetCpuFirstClimbOnFoundation(arrayClimbOnFoundation[Random.Range(0, arrayClimbOnFoundation.Length)]);
			}
			else
			{
				arrayPlayer[num3].SetCpuFirstClimbOnFoundation(arrayClimbOnFoundation[k]);
			}
			num3++;
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (!arrayPlayer[i].GetIsGoal())
			{
				arrayPlayer[i].UpdateMethod();
			}
		}
	}
	public RockClimbing_Player GetPlayer(int _playerNo)
	{
		return arrayPlayer[_playerNo];
	}
	public Transform GetThrowGrapplingHookRoot(int _playerNo)
	{
		return arrayThrowGrapplingHookRoot[_playerNo];
	}
	public Transform GetThrowAnchor(int _playerNo)
	{
		return arrayThrowAnchor[_playerNo];
	}
	public Material GetGrapplingHookMat(int _userType)
	{
		return arrayGrapplingHookMat[_userType];
	}
	public float GetMoveSpeed()
	{
		return MOVE_SPEED;
	}
	public float GetBaseMoveSpeed()
	{
		return BASE_MOVE_SPEED;
	}
	public float GetCorrectionMoveSpeed()
	{
		return CORRECTION_MOVE_SPEED;
	}
	public float GetMaxMoveSpeed()
	{
		return MAX_MOVE_SPEED;
	}
	public float GetThrowGrapplingHookDistance()
	{
		return THROW_GRAPPLING_HOOK_MAX_DISTANCE;
	}
	public float GetThrowGrapplingHookSpeed()
	{
		return THROW_GRAPPLING_HOOK_FLY_SPEED;
	}
	public float GetThrowAngleOnRoof()
	{
		return THROW_ANGLE_ON_ROOF;
	}
	public float GetCollectRopeTime()
	{
		return COLLECT_ROPE_TIME;
	}
	public Vector3 GetClimbingRopeDiffPos()
	{
		return CLIMBING_ROPE_DIFF_POS;
	}
	public float GetClimbingBasePower()
	{
		return CLIMBING_BASE_POWER;
	}
	public float GetIntervalLerp(float _inputInterval)
	{
		float num = Mathf.Clamp(_inputInterval, 0f, MAX_INPUT_INTERVAL);
		return (MAX_INPUT_INTERVAL - num) / MAX_INPUT_INTERVAL;
	}
	public float ClampClimbiingSpeed(float _intervalLerp)
	{
		return Mathf.Clamp(MAX_CLIMBING_SPEED * _intervalLerp, MIN_CLIMBING_SPEED, MAX_CLIMBING_SPEED);
	}
	public float GetHitObstacleInterval()
	{
		return HIT_OBSTACLE_INTERVAL;
	}
	public float GetHitObstacleDownPower()
	{
		return HIT_OBSTACLE_DOWN_POWER;
	}
	public float GetHookPointColliderRange()
	{
		return HOOK_POINT_COLLIDER_RANGE;
	}
	public void GameStartAnimation()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayPlayer[i].GameStartAnimation();
		}
	}
	public float GetGoalPosMinDistance()
	{
		return GOAL_POS_MIN_DISTANCE;
	}
	public float GetGoalPosMaxDistance()
	{
		return GOAL_POS_MAX_DISTANCE;
	}
	public float GetGoalPosMinMoveTime()
	{
		return GOAL_POS_MIN_MOVE_TIME;
	}
	public float GetGoalPosMaxMoveTime()
	{
		return GOAL_POS_MAX_MOVE_TIME;
	}
}
