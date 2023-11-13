using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RockClimbing_AI : MonoBehaviour
{
	private RockClimbing_Player player;
	private RockClimbing_ClimbingWall climbingWall;
	private Transform throwAnchor;
	private Rigidbody rigid;
	private CapsuleCollider collider;
	private float collider_radius;
	private int wallCheckLayerMask;
	private string[] wallCheckMaskNameList = new string[1]
	{
		"Wall"
	};
	private int charaCheckLayerMask;
	private string[] charaCheckMaskNameList = new string[2]
	{
		"Character",
		"HitCharacterDefaultOnly"
	};
	private Collider[] arrayOverLapCollider = new Collider[4];
	private RaycastHit[] arrayRaycastHit = new RaycastHit[4];
	private RockClimbing_ClimbOnFoundation firstClimbOnFoundation;
	private RockClimbing_ClimbOnFoundationObject_Group.ClimbOnFoundationAnchor[] stoneWallClimbOnFoundationAnchor;
	private RockClimbing_GrapplingHookPoint targetHookPoint;
	private bool isSettingMove;
	private Vector3 targetPos;
	private Vector3 avoidBeforeTargetPos;
	private int moveTargetIdx;
	private Dictionary<int, float> canMoveFoundationDistanceDic = new Dictionary<int, float>();
	private bool isMoveEnd;
	private int aiStrength;
	private float climbingLerpTime;
	private float climbingInterval;
	private float CPU_CLIMBING_INTERVAL;
	private float CPU_CLIMBING_DEF_INTERVAL;
	private float CPU_CLIMBING_MIN_INTERVAL;
	private float CPU_CLIMBING_MAX_SPEED_TIME;
	private float GRAPPLINNG_HOOK_NEAR_POINT;
	private float GRAPPLINNG_HOOK_THROW_WAIT_TIME;
	private float CPU_AVOID_PROBABILITY;
	private bool isAvoid;
	private readonly float AVOID_CHARA_TIME = 0.25f;
	private readonly float AVOID_TIME = 0.5f;
	private float avoidTime;
	private bool isNotAvoid;
	private readonly float NOT_AVOID_TIME = 0.5f;
	private float notAvoidTime;
	private Vector3 Debug_Point0;
	private Vector3 Debug_Point1;
	private Vector3 Debug_Point0_CheckObstacle;
	private Vector3 Debug_Point1_CheckObstacle;
	public void Init(RockClimbing_Player _player)
	{
		player = _player;
		climbingWall = player.GetClimbingWall();
		throwAnchor = player.GetThrowAnchor();
		rigid = player.GetRigidbody();
		collider = player.GetCollider();
		collider_radius = player.GetColliderRadius();
		wallCheckLayerMask = LayerMask.GetMask(wallCheckMaskNameList);
		charaCheckLayerMask = LayerMask.GetMask(charaCheckMaskNameList);
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		CPU_CLIMBING_INTERVAL = UnityEngine.Random.Range(RockClimbing_Define.CPU_CLIMBING_INTERVAL[aiStrength] - 0.025f, RockClimbing_Define.CPU_CLIMBING_INTERVAL[aiStrength] + 0.025f);
		CPU_CLIMBING_DEF_INTERVAL = CPU_CLIMBING_INTERVAL;
		CPU_CLIMBING_MIN_INTERVAL = UnityEngine.Random.Range(RockClimbing_Define.CPU_CLIMBING_MIN_INTERVAL[aiStrength] - 0.01f, RockClimbing_Define.CPU_CLIMBING_MIN_INTERVAL[aiStrength] + 0.01f);
		CPU_CLIMBING_MAX_SPEED_TIME = UnityEngine.Random.Range(RockClimbing_Define.CPU_CLIMBING_MAX_SPEED_TIME[aiStrength] - 0.25f, RockClimbing_Define.CPU_CLIMBING_MAX_SPEED_TIME[aiStrength] + 0.25f);
		GRAPPLINNG_HOOK_NEAR_POINT = UnityEngine.Random.Range(RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] - 0.2f, RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] + 0.2f);
		GRAPPLINNG_HOOK_THROW_WAIT_TIME = UnityEngine.Random.Range(RockClimbing_Define.CPU_GRAPPLINNG_HOOK_THROW_WAIT_TIME[aiStrength] - 0.1f, RockClimbing_Define.CPU_GRAPPLINNG_HOOK_THROW_WAIT_TIME[aiStrength] + 0.1f);
		CPU_AVOID_PROBABILITY = UnityEngine.Random.Range(RockClimbing_Define.CPU_AVOID_PROBABILITY[aiStrength] - 0.2f, RockClimbing_Define.CPU_AVOID_PROBABILITY[aiStrength] + 0.2f);
	}
	public void SetFirstClimbOnFoundation(RockClimbing_ClimbOnFoundation _firstClimbOnFoundation)
	{
		firstClimbOnFoundation = _firstClimbOnFoundation;
	}
	public void ResetMoveSetting()
	{
		isMoveEnd = false;
		isSettingMove = false;
		GRAPPLINNG_HOOK_NEAR_POINT = UnityEngine.Random.Range(RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] - 0.2f, RockClimbing_Define.CPU_GRAPPLINNG_HOOK_NEAR_POINT[aiStrength] + 0.2f);
		GRAPPLINNG_HOOK_THROW_WAIT_TIME = UnityEngine.Random.Range(RockClimbing_Define.CPU_GRAPPLINNG_HOOK_THROW_WAIT_TIME[aiStrength] - 0.1f, RockClimbing_Define.CPU_GRAPPLINNG_HOOK_THROW_WAIT_TIME[aiStrength] + 0.1f);
		CPU_AVOID_PROBABILITY = UnityEngine.Random.Range(RockClimbing_Define.CPU_AVOID_PROBABILITY[aiStrength] - 0.2f, RockClimbing_Define.CPU_AVOID_PROBABILITY[aiStrength] + 0.2f);
		isAvoid = false;
		isNotAvoid = false;
		climbingInterval = 0f;
	}
	public void SetMove()
	{
		RockClimbing_ClimbingWallManager.ClimbOnFoundationType climbOnFoundationType = player.GetClimbOnFoundation().GetClimbOnFoundationType();
		float z = player.GetClimbOnFoundation().GetMoveZLimitAnchor().position.z;
		switch (climbOnFoundationType)
		{
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.None:
			if (!isSettingMove)
			{
				targetHookPoint = firstClimbOnFoundation.GetGrapplingHookPoint(player.GetPlayerNo());
				Collider collider = targetHookPoint.GetCollider();
				float num = collider.bounds.min.x + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
				float num2 = collider.bounds.max.x - SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
				float num4 = (num2 - num) / 2f / 2f;
				targetPos.x = ((UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(num, num + num4) : UnityEngine.Random.Range(num2 - num4, num2));
				targetPos.z = z - collider_radius;
				avoidBeforeTargetPos = targetPos;
				SetMoveDir();
				isSettingMove = true;
			}
			break;
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_1:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_2:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_3:
		{
			if (isSettingMove)
			{
				break;
			}
			if (stoneWallClimbOnFoundationAnchor == null)
			{
				stoneWallClimbOnFoundationAnchor = climbingWall.GetClimbOnFoundationObjectGroup().GetArrayClimbOnFoundationAnchor();
			}
			RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundation = stoneWallClimbOnFoundationAnchor[(int)climbOnFoundationType].GetArrayClimbOnFoundation();
			if (UnityEngine.Random.Range(0f, 1f) < GRAPPLINNG_HOOK_NEAR_POINT)
			{
				UnityEngine.Debug.Log("一番近くにある引っかけるポイントに移動する場合");
				float num6 = 1000f;
				Collider collider;
				for (int j = 0; j < arrayClimbOnFoundation.Length; j++)
				{
					targetHookPoint = arrayClimbOnFoundation[j].GetGrapplingHookPoint(player.GetPlayerNo());
					collider = targetHookPoint.GetCollider();
					float num = collider.bounds.min.x + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					float num2 = collider.bounds.max.x - SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					float num4 = (num2 - num) / 2f / 2f;
					if (throwAnchor.position.x >= num && throwAnchor.position.x <= num2)
					{
						moveTargetIdx = j;
						targetPos.x = UnityEngine.Random.Range(collider.transform.position.x - num4, collider.transform.position.x + num4);
						break;
					}
					if (throwAnchor.position.x < num)
					{
						float num7 = Mathf.Abs(throwAnchor.position.x - num);
						if (num6 > num7 && !Physics.Raycast(base.transform.position, Vector3.right, num7, wallCheckLayerMask))
						{
							num6 = num7;
							moveTargetIdx = j;
							targetPos.x = UnityEngine.Random.Range(num, num + num4);
						}
					}
					else if (throwAnchor.position.x > num2)
					{
						float num8 = Mathf.Abs(throwAnchor.position.x - num2);
						if (num6 > num8 && !Physics.Raycast(base.transform.position, Vector3.left, num8, wallCheckLayerMask))
						{
							num6 = num8;
							moveTargetIdx = j;
							targetPos.x = UnityEngine.Random.Range(num2 - num4, num2);
						}
					}
				}
				targetHookPoint = arrayClimbOnFoundation[moveTargetIdx].GetGrapplingHookPoint(player.GetPlayerNo());
				collider = targetHookPoint.GetCollider();
			}
			else
			{
				canMoveFoundationDistanceDic.Clear();
				Collider collider;
				float num;
				float num2;
				for (int k = 0; k < arrayClimbOnFoundation.Length; k++)
				{
					targetHookPoint = arrayClimbOnFoundation[k].GetGrapplingHookPoint(player.GetPlayerNo());
					collider = targetHookPoint.GetCollider();
					num = collider.bounds.min.x + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					num2 = collider.bounds.max.x - SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					if (throwAnchor.position.x >= num && throwAnchor.position.x <= num2)
					{
						canMoveFoundationDistanceDic.Add(k, 0f);
					}
					else if (throwAnchor.position.x < num)
					{
						float num9 = Mathf.Abs(throwAnchor.position.x - num);
						if (!Physics.Raycast(base.transform.position, Vector3.right, num9, wallCheckLayerMask))
						{
							canMoveFoundationDistanceDic.Add(k, num9);
						}
					}
					else if (throwAnchor.position.x > num2)
					{
						float num10 = Mathf.Abs(throwAnchor.position.x - num2);
						if (!Physics.Raycast(base.transform.position, Vector3.left, num10, wallCheckLayerMask))
						{
							canMoveFoundationDistanceDic.Add(k, num10);
						}
					}
				}
				if (canMoveFoundationDistanceDic.Count > 1)
				{
					moveTargetIdx = (from v in (from v in canMoveFoundationDistanceDic
							orderby v.Value
							select v.Key).Skip(1)
						orderby Guid.NewGuid()
						select v).FirstOrDefault();
				}
				else
				{
					moveTargetIdx = canMoveFoundationDistanceDic.Keys.FirstOrDefault();
				}
				targetHookPoint = arrayClimbOnFoundation[moveTargetIdx].GetGrapplingHookPoint(player.GetPlayerNo());
				collider = targetHookPoint.GetCollider();
				num = collider.bounds.min.x + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
				num2 = collider.bounds.max.x - SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
				float num4 = (num2 - num) / 2f / 2f;
				if (throwAnchor.position.x < num)
				{
					targetPos.x = UnityEngine.Random.Range(num, num + num4);
				}
				else if (throwAnchor.position.x > num2)
				{
					targetPos.x = UnityEngine.Random.Range(num2 - num4, num2);
				}
			}
			targetPos.z = z - collider_radius;
			avoidBeforeTargetPos = targetPos;
			SetMoveDir();
			isSettingMove = true;
			break;
		}
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_MAX:
			if (!isSettingMove)
			{
				climbOnFoundationType -= 4;
				targetHookPoint = climbingWall.GetArrayClimbOnFoundationRoof()[(int)climbOnFoundationType].GetGrapplingHookPoint(player.GetPlayerNo());
				Collider collider = targetHookPoint.GetCollider();
				isSettingMove = true;
			}
			break;
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_1:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_2:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_3:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_4:
		case RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_MAX:
		{
			bool flag = false;
			if (climbOnFoundationType == RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_3)
			{
				flag = true;
			}
			climbOnFoundationType -= 4;
			if (flag)
			{
				UnityEngine.Debug.Log("近いTargetHookPointを設定");
				targetHookPoint = climbingWall.GetArrayClimbOnFoundationRoof()[(int)climbOnFoundationType].GetNearGrapplingHookPoint(player.GetPlayerNo(), player.transform.position);
			}
			else
			{
				targetHookPoint = climbingWall.GetArrayClimbOnFoundationRoof()[(int)climbOnFoundationType].GetGrapplingHookPoint(player.GetPlayerNo());
			}
			Collider collider = targetHookPoint.GetCollider();
			float num = collider.bounds.min.x + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
			float num2 = collider.bounds.max.x - SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
			Vector3 position = targetHookPoint.transform.position;
			position.x = throwAnchor.transform.position.x;
			Vector3 normalized = (position - throwAnchor.transform.position).normalized;
			if (Vector3.Angle(Vector3.up, normalized) <= SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowAngleOnRoof() && throwAnchor.transform.position.x >= num && throwAnchor.transform.position.x <= num2)
			{
				UnityEngine.Debug.Log("鉤縄を登れる角度のところにいる");
				player.SetMoveDir(Vector3.zero);
				break;
			}
			if (UnityEngine.Random.Range(0f, 1f) < CPU_AVOID_PROBABILITY)
			{
				for (int i = 0; i < climbingWall.GetThrowObstacleGroup(player.GetPlayerNo()).GetThrowObjectList().Count; i++)
				{
					RockClimbing_Obstacle_Throw_Object rockClimbing_Obstacle_Throw_Object = climbingWall.GetThrowObstacleGroup(player.GetPlayerNo()).GetThrowObjectList()[i];
					if (!rockClimbing_Obstacle_Throw_Object.GetIsHit())
					{
						float num3 = CalcManager.Length(base.transform.position, rockClimbing_Obstacle_Throw_Object.transform.position);
						UnityEngine.Debug.DrawLine(rockClimbing_Obstacle_Throw_Object.transform.position, rockClimbing_Obstacle_Throw_Object.transform.position + rockClimbing_Obstacle_Throw_Object.GetThrowVec() * num3, Color.magenta, 10f);
						if (Physics.Raycast(rockClimbing_Obstacle_Throw_Object.transform.position, rockClimbing_Obstacle_Throw_Object.GetThrowVec(), out RaycastHit hitInfo, num3) && (hitInfo.collider.tag == "Player" || hitInfo.collider.tag == "Character"))
						{
							normalized = rockClimbing_Obstacle_Throw_Object.GetThrowVec();
							float f = (Mathf.Atan2(normalized.z, normalized.x) * 57.29578f + ((UnityEngine.Random.Range(0, 2) == 0) ? (-90f) : 90f)) * ((float)Math.PI / 180f);
							targetPos = base.transform.position + new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
							string str = player.GetPlayerNo().ToString();
							Vector3 vector = targetPos;
							UnityEngine.Debug.Log("playerno : " + str + " targetPos : " + vector.ToString());
							avoidBeforeTargetPos = targetPos;
							SetMoveDir();
							isAvoid = true;
							avoidTime = UnityEngine.Random.Range(AVOID_TIME - 0.1f, AVOID_TIME + 0.1f);
							isSettingMove = false;
							UnityEngine.Debug.Log("避ける");
							break;
						}
					}
				}
			}
			if (!isAvoid && !isSettingMove)
			{
				float num4 = (num2 - num) / 2f / 2f;
				float num5 = num4 / 2f;
				if (Mathf.Abs(throwAnchor.transform.position.x - collider.transform.position.x) < num5)
				{
					targetPos.x = UnityEngine.Random.Range(collider.transform.position.x - num5, collider.transform.position.x + num5);
				}
				else if (throwAnchor.transform.position.x < collider.transform.position.x)
				{
					targetPos.x = UnityEngine.Random.Range(collider.transform.position.x - num4 - num5, collider.transform.position.x - num4 + num5);
				}
				else
				{
					targetPos.x = UnityEngine.Random.Range(collider.transform.position.x + num4 - num5, collider.transform.position.x + num4 + num5);
				}
				targetPos.z = z - collider_radius;
				avoidBeforeTargetPos = targetPos;
				SetMoveDir();
				isSettingMove = true;
			}
			break;
		}
		}
	}
	public void Avoid()
	{
		avoidTime -= Time.deltaTime;
		if (avoidTime < 0f)
		{
			isAvoid = false;
			isNotAvoid = true;
			notAvoidTime = UnityEngine.Random.Range(NOT_AVOID_TIME - 0.1f, NOT_AVOID_TIME + 0.1f);
		}
	}
	public void NotAvoid()
	{
		notAvoidTime -= Time.deltaTime;
		if (notAvoidTime < 0f)
		{
			isNotAvoid = false;
		}
	}
	private void SetMoveDir()
	{
		player.SetMoveDir((new Vector3(targetPos.x, throwAnchor.position.y, targetPos.z) - throwAnchor.position).normalized);
	}
	public void StopMove()
	{
		isMoveEnd = true;
		rigid.velocity = Vector3.zero;
		player.SetMoveDir(Vector3.zero);
		isAvoid = false;
		isNotAvoid = false;
	}
	public void ChangeMoveTargetPos(Vector3 _velocity)
	{
		Vector3 position = collider.transform.position;
		position.y = collider.bounds.min.y + collider_radius;
		Vector3 position2 = collider.transform.position;
		position2.y = collider.bounds.max.y - collider_radius;
		Debug_Point0 = position;
		Debug_Point1 = position2;
		Collider x = null;
		int num = Physics.OverlapCapsuleNonAlloc(position, position2, collider_radius, arrayOverLapCollider, charaCheckLayerMask);
		if (num > 1)
		{
			for (int i = 0; i < num; i++)
			{
				if (!(collider == arrayOverLapCollider[i]))
				{
					x = arrayOverLapCollider[i];
					break;
				}
			}
		}
		if (x != null)
		{
			RockClimbing_Player component = collider.transform.parent.GetComponent<RockClimbing_Player>();
			UnityEngine.Debug.Log("colOtherPlayer " + component.GetPlayerNo().ToString());
			if (CheckAvoidMoveTargetPos(component))
			{
				SetAvoidMoveTargetPos(component);
				return;
			}
		}
		else
		{
			Debug_Point0_CheckObstacle = position + _velocity.normalized * collider_radius * 2.5f;
			Debug_Point1_CheckObstacle = position2 + _velocity.normalized * collider_radius * 2.5f;
			RaycastHit raycastHit = default(RaycastHit);
			num = Physics.CapsuleCastNonAlloc(position, position2, collider_radius, _velocity.normalized, arrayRaycastHit, collider_radius * 2.5f, charaCheckLayerMask);
			UnityEngine.Debug.Log("capsuleCast checkColCnt" + num.ToString());
			if (num > 1)
			{
				for (int j = 0; j < num; j++)
				{
					if (!(collider == arrayRaycastHit[j].collider))
					{
						raycastHit = arrayRaycastHit[j];
						break;
					}
				}
			}
			if (raycastHit.collider != null)
			{
				RockClimbing_Player component2 = raycastHit.collider.transform.parent.GetComponent<RockClimbing_Player>();
				UnityEngine.Debug.Log("colOtherPlayer " + component2.GetPlayerNo().ToString());
				if (CheckAvoidMoveTargetPos(component2))
				{
					SetAvoidMoveTargetPos(component2);
					return;
				}
			}
		}
		if (targetPos != avoidBeforeTargetPos)
		{
			targetPos = avoidBeforeTargetPos;
			SetMoveDir();
		}
	}
	private bool CheckAvoidMoveTargetPos(RockClimbing_Player _colOtherPlayer)
	{
		if (_colOtherPlayer.GetState() == RockClimbing_PlayerManager.State.MOVE)
		{
			if (_colOtherPlayer.GetIsCpu() && _colOtherPlayer.GetIsCpuNotAvoid())
			{
				return true;
			}
			CapsuleCollider capsuleCollider = _colOtherPlayer.GetCollider();
			float colliderRadius = _colOtherPlayer.GetColliderRadius();
			Vector3 forward = _colOtherPlayer.transform.forward;
			if (forward == Vector3.zero)
			{
				return true;
			}
			Vector3 position = capsuleCollider.transform.position;
			position.y = capsuleCollider.bounds.min.y + colliderRadius;
			Vector3 position2 = capsuleCollider.transform.position;
			position2.y = capsuleCollider.bounds.max.y - colliderRadius;
			Debug_Point0_CheckObstacle = position + forward * colliderRadius * 2.5f;
			Debug_Point1_CheckObstacle = position2 + forward * colliderRadius * 2.5f;
			int num = Physics.CapsuleCastNonAlloc(position, position2, colliderRadius, forward, arrayRaycastHit, colliderRadius * 2.5f, charaCheckLayerMask);
			UnityEngine.Debug.Log("capsuleCast checkColCnt" + num.ToString());
			if (num > 1)
			{
				for (int i = 0; i < num; i++)
				{
					if (collider == arrayRaycastHit[i].collider)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
		return true;
	}
	private void SetAvoidMoveTargetPos(RockClimbing_Player _colOtherPlayer)
	{
		Vector3 forward = base.transform.forward;
		float num = Mathf.Atan2(forward.z, forward.x) * 57.29578f;
		Vector3 vector = Vector3.Cross(base.transform.forward, Vector3.forward);
		UnityEngine.Debug.Log("cross Y " + vector.y.ToString());
		if ((_colOtherPlayer.GetIsCpu() && _colOtherPlayer.GetIsCpuMoveEnd()) || _colOtherPlayer.GetState() != 0)
		{
			if (vector.y < 0f)
			{
				UnityEngine.Debug.Log("右に進んでいる時に誰かに接触：外側に避ける");
				num += UnityEngine.Random.Range(-45f, 0f);
			}
			else if (vector.y > 0f)
			{
				UnityEngine.Debug.Log("左に進んでいる時に誰かに接触：外側に避ける");
				num += UnityEngine.Random.Range(45f, 90f);
			}
		}
		else if (vector.y < 0f)
		{
			UnityEngine.Debug.Log("右に進んでいる時に誰かに接触");
			num += UnityEngine.Random.Range(0f, 45f);
		}
		else if (vector.y > 0f)
		{
			UnityEngine.Debug.Log("左に進んでいる時に誰かに接触");
			num += UnityEngine.Random.Range(45f, 90f);
		}
		float f = num * ((float)Math.PI / 180f);
		targetPos = base.transform.position + new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		string[] obj = new string[6]
		{
			"playerno : ",
			player.GetPlayerNo().ToString(),
			" transform.forward.x : ",
			null,
			null,
			null
		};
		Vector3 forward2 = base.transform.forward;
		obj[3] = forward2.x.ToString();
		obj[4] = " targetPos : ";
		forward2 = targetPos;
		obj[5] = forward2.ToString();
		UnityEngine.Debug.Log(string.Concat(obj));
		SetMoveDir();
		isAvoid = true;
		avoidTime = UnityEngine.Random.Range(AVOID_CHARA_TIME - 0.1f, AVOID_CHARA_TIME + 0.1f);
	}
	public void SetThrowGrapplingHook()
	{
		GRAPPLINNG_HOOK_THROW_WAIT_TIME -= Time.deltaTime;
		if (GRAPPLINNG_HOOK_THROW_WAIT_TIME < 0f)
		{
			player.ThrowGrapplingHook();
		}
	}
	public void SetClimbing()
	{
		if (climbingLerpTime < CPU_CLIMBING_MAX_SPEED_TIME)
		{
			climbingLerpTime += Time.deltaTime;
			climbingLerpTime = Mathf.Clamp(climbingLerpTime, 0f, CPU_CLIMBING_MAX_SPEED_TIME);
			float num = climbingLerpTime / CPU_CLIMBING_MAX_SPEED_TIME;
			CPU_CLIMBING_INTERVAL = CPU_CLIMBING_DEF_INTERVAL - (CPU_CLIMBING_DEF_INTERVAL - CPU_CLIMBING_MIN_INTERVAL) * num;
			CPU_CLIMBING_INTERVAL = Mathf.Clamp(CPU_CLIMBING_INTERVAL, CPU_CLIMBING_MIN_INTERVAL, CPU_CLIMBING_DEF_INTERVAL);
		}
		climbingInterval += Time.deltaTime;
		if (climbingInterval > CPU_CLIMBING_INTERVAL)
		{
			climbingInterval = 0f;
			player.Climbing();
		}
	}
	public void ResetClimbingLerpTime()
	{
		climbingLerpTime = 0f;
	}
	public RockClimbing_GrapplingHookPoint GetTargetHookPoint()
	{
		return targetHookPoint;
	}
	public bool GetIsMoveEnd()
	{
		return isMoveEnd;
	}
	public bool GetIsAvoid()
	{
		return isAvoid;
	}
	public bool GetIsNotAvoid()
	{
		return isNotAvoid;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawWireCapsule(Debug_Point0, Debug_Point1, collider_radius);
		Gizmos.color = Color.blue;
		DrawWireCapsule(Debug_Point0_CheckObstacle, Debug_Point1_CheckObstacle, collider_radius);
	}
	public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
	{
	}
}
