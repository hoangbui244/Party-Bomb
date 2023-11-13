using System;
using UnityEngine;
public class Curling_AI : MonoBehaviour
{
	public enum ThrowTarget
	{
		Stone,
		House
	}
	private Curling_Player player;
	private int aiStrength;
	private float CPU_TARGET_ANGLE;
	private float CPU_TARGET_POS_DIFF;
	private float CPU_TARGET_POWER_DIFF;
	private bool TARGET_STONE_PROBABILITY;
	private float thinkTime;
	private bool isThrowSetting;
	private Vector3 finalTargetVec;
	private Vector3 targetVec;
	private bool isLookTargetVec;
	private Vector3 throw_CurveDir;
	private float finalTargetThrowPower;
	private float throwRandomPower;
	private int throwPowerDir;
	private float throw_Power;
	private readonly float THROW_TARGET_STONE_POWER = 1f;
	private readonly float THROW_TARGET_HOUSE_POWER = 0.7f;
	private bool isSetPower;
	private float THINK_TIME = 0.5f;
	private bool isSetRandomVec;
	private float DECISION_THROW_STONE_WAIT_TIME = 1.5f;
	private float decisionThrowStoneWaitTime;
	private bool isDecisionThrowStoneWait;
	private const float DIFF_Z_MIN = 0.5f;
	private const float DIFF_Z_MAX = 2f;
	private const float CURVE_DIR_DIFF = 1.05f;
	private Vector3 targetPos;
	private bool isHouseSweepCurve;
	private float houseSweepInterval;
	private float HOUSE_SWEEP_INTERVAL;
	public void Init(Curling_Player _player)
	{
		player = _player;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
	}
	public void InitPlay()
	{
		thinkTime = 0f;
		isThrowSetting = false;
		isLookTargetVec = false;
		throw_CurveDir = Vector3.zero;
		isSetPower = false;
		throw_Power = 0f;
		isSetRandomVec = false;
		decisionThrowStoneWaitTime = 0f;
		isDecisionThrowStoneWait = false;
		CPU_TARGET_ANGLE = UnityEngine.Random.Range(Curling_Define.CPU_TARGET_ANGLE[aiStrength] - 0.1f, Curling_Define.CPU_TARGET_ANGLE[aiStrength] + 0.1f);
		CPU_TARGET_ANGLE = ((CPU_TARGET_ANGLE < 0f) ? 0f : CPU_TARGET_ANGLE);
		CPU_TARGET_POS_DIFF = Curling_Define.CPU_TARGET_POS_DIFF[aiStrength];
		CPU_TARGET_POWER_DIFF = UnityEngine.Random.Range(Curling_Define.CPU_TARGET_POWER_DIFF[aiStrength] - 0.15f, Curling_Define.CPU_TARGET_POWER_DIFF[aiStrength] + 0.15f);
		CPU_TARGET_POWER_DIFF = ((CPU_TARGET_POWER_DIFF > 1f) ? 1f : CPU_TARGET_POWER_DIFF);
		isHouseSweepCurve = (UnityEngine.Random.Range(0f, 1f) <= Curling_Define.HOUSE_SWEEP_PROBABILITY[aiStrength]);
		if (isHouseSweepCurve)
		{
			HOUSE_SWEEP_INTERVAL = UnityEngine.Random.Range(Curling_Define.HOUSE_SWEEP_INTERVAL[aiStrength] - 0.05f, Curling_Define.HOUSE_SWEEP_INTERVAL[aiStrength] + 0.05f);
			HOUSE_SWEEP_INTERVAL = ((HOUSE_SWEEP_INTERVAL < 0f) ? 0f : HOUSE_SWEEP_INTERVAL);
			houseSweepInterval = 0f;
		}
	}
	public void SetThrowInfo()
	{
		if (isThrowSetting)
		{
			return;
		}
		isThrowSetting = true;
		Curling_GameManager.Team teamNo = (player.GetTeam() == Curling_GameManager.Team.TEAM_A) ? Curling_GameManager.Team.TEAM_B : Curling_GameManager.Team.TEAM_A;
		UnityEngine.Debug.Log("相手のチ\u30fcム" + teamNo.ToString());
		Curling_Stone[] arrayStone = SingletonCustom<Curling_GameManager>.Instance.GetArrayStone((int)teamNo);
		int num = -1;
		float num2 = 1000f;
		if (UnityEngine.Random.Range(0f, 1f) <= Curling_Define.TARGET_STONE_PROBABILITY[aiStrength])
		{
			float num3 = 0f;
			for (int i = 0; i < arrayStone.Length; i++)
			{
				Curling_Stone curling_Stone = arrayStone[i];
				if (curling_Stone.gameObject.activeSelf && !curling_Stone.GetRigid().isKinematic && !curling_Stone.GetIsFailure())
				{
					num3 = CalcManager.Length(new Vector3(curling_Stone.transform.position.x, 0f, curling_Stone.transform.position.z), SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos()) - curling_Stone.GetStoneObjectHalfSize();
					if (num3 < num2)
					{
						num = i;
						num2 = num3;
					}
				}
			}
		}
		if (num != -1)
		{
			if (num2 <= SingletonCustom<Curling_CurlingRinkManager>.Instance.GetTeaCircleRadius_Second())
			{
				UnityEngine.Debug.Log("相手の石がティ\u30fcから２番目に近い円以内にある場合");
				Curling_Stone curling_Stone = arrayStone[num];
				UnityEngine.Debug.Log("nearIdx " + num.ToString());
				UnityEngine.Debug.Log("stone " + curling_Stone.gameObject.name);
				UnityEngine.Debug.Log("stone.transform.position " + curling_Stone.transform.position.ToString());
				UnityEngine.Debug.Log("transform.position " + base.transform.position.ToString());
				targetPos = curling_Stone.transform.position;
				SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().SetTargetPos(targetPos);
				SetThrowTargetStone(ThrowTarget.Stone, curling_Stone.GetStoneObjectHalfSize());
				SetThrowTargetVectorDiff(curling_Stone.GetStoneObjectHalfSize(), curling_Stone);
			}
			else
			{
				UnityEngine.Debug.Log("相手の石がある場合 : 一定距離以上\u3000ティ\u30fcを狙う");
				targetPos = SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos();
				SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().SetTargetPos(targetPos);
				SetThrowTargetStone(ThrowTarget.House, SingletonCustom<Curling_CurlingRinkManager>.Instance.GetTeaCircleRadius_Second());
				SetThrowTargetVectorDiff(SingletonCustom<Curling_CurlingRinkManager>.Instance.GetTeaCircleRadius_Second());
			}
		}
		else
		{
			UnityEngine.Debug.Log("ティ\u30fcを狙う");
			targetPos = SingletonCustom<Curling_CurlingRinkManager>.Instance.GetBackTeaPos();
			SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().SetTargetPos(targetPos);
			SetThrowTargetStone(ThrowTarget.House, SingletonCustom<Curling_CurlingRinkManager>.Instance.GetTeaCircleRadius_Second());
			SetThrowTargetVectorDiff(SingletonCustom<Curling_CurlingRinkManager>.Instance.GetTeaCircleRadius_Second());
		}
		UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + finalTargetVec.normalized * 5f, Color.yellow, 30f);
	}
	public void Throw()
	{
		if (SingletonCustom<Curling_GameManager>.Instance.GetIsSkip())
		{
			return;
		}
		if (!isSetPower)
		{
			float num = isDecisionThrowStoneWait ? finalTargetThrowPower : throwRandomPower;
			throw_Power += (float)throwPowerDir * Time.deltaTime;
			throw_Power = Mathf.Clamp(throw_Power, (throwPowerDir == 1) ? 0f : num, (throwPowerDir == 1) ? num : 1f);
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: true);
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowPower(throw_Power);
			if (throw_Power == num)
			{
				if (isDecisionThrowStoneWait)
				{
					isSetPower = true;
				}
				else
				{
					throwRandomPower = UnityEngine.Random.Range(finalTargetThrowPower - 0.2f, finalTargetThrowPower + 0.2f);
					throwRandomPower = Mathf.Clamp(throwRandomPower, 0f, 1f);
					throwPowerDir = ((throw_Power < throwRandomPower) ? 1 : (-1));
				}
			}
		}
		decisionThrowStoneWaitTime += Time.deltaTime;
		player.SetThrowVel(throw_Power);
		if (decisionThrowStoneWaitTime >= DECISION_THROW_STONE_WAIT_TIME)
		{
			isDecisionThrowStoneWait = true;
			float num2 = Vector3.Angle(base.transform.forward, finalTargetVec);
			UnityEngine.Debug.Log("angle " + num2.ToString());
			if (num2 < CPU_TARGET_ANGLE)
			{
				isLookTargetVec = true;
			}
		}
		if (isLookTargetVec && isSetPower)
		{
			SingletonCustom<Curling_GameManager>.Instance.ThrowStone(throw_CurveDir, base.transform.forward, throw_Power);
		}
	}
	private void SetThrowTargetStone(ThrowTarget _throwTarget, float _targetRadius)
	{
		targetPos.x += UnityEngine.Random.Range(0f - _targetRadius, _targetRadius) * CPU_TARGET_POS_DIFF;
		finalTargetVec = targetPos - base.transform.position;
		finalTargetVec.y = 0f;
		finalTargetVec += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
		switch (_throwTarget)
		{
		case ThrowTarget.Stone:
			finalTargetThrowPower = THROW_TARGET_STONE_POWER * CPU_TARGET_POWER_DIFF;
			break;
		case ThrowTarget.House:
			finalTargetThrowPower = THROW_TARGET_HOUSE_POWER * CPU_TARGET_POWER_DIFF;
			break;
		}
		throwRandomPower = UnityEngine.Random.Range(finalTargetThrowPower - 0.2f, finalTargetThrowPower + 0.2f);
		throwRandomPower = Mathf.Clamp(throwRandomPower, 0f, 1f);
		throwPowerDir = ((throw_Power < throwRandomPower) ? 1 : (-1));
	}
	public void SetThrowTargetVectorDiff(float _targetRadius, Curling_Stone _targetStone = null)
	{
		float num = Mathf.Atan2(finalTargetVec.z, finalTargetVec.x) * 57.29578f;
		UnityEngine.Debug.Log("_targetPos.z " + targetPos.z.ToString());
		UnityEngine.Debug.Log("目的の座標の角度 finalVecAngle " + num.ToString());
		Vector3 vector = targetPos + _targetRadius * -base.transform.right;
		Vector3 vector2 = vector - base.transform.position;
		vector2.y = 0f;
		vector2 += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
		float num2 = Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
		Vector3 vector3 = vector;
		UnityEngine.Debug.Log("目的の座標の左側の座標 pos " + vector3.ToString());
		UnityEngine.Debug.Log("目的の座標の左側の角度 targetLeftAngle " + num2.ToString());
		vector = targetPos + _targetRadius * base.transform.right;
		vector2 = vector - base.transform.position;
		vector2.y = 0f;
		vector2 += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
		float num3 = Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
		vector3 = vector;
		UnityEngine.Debug.Log("目的の座標の右側の座標 pos " + vector3.ToString());
		UnityEngine.Debug.Log("目的の座標の右側の角度 targetRightAngle " + num3.ToString());
		float num4 = 1000f;
		Curling_Stone curling_Stone = null;
		UnityEngine.Debug.Log("_targetStone " + ((_targetStone != null) ? _targetStone.gameObject.name : "null"));
		for (int i = 0; i < SingletonCustom<Curling_GameManager>.Instance.GetArrayTeamInfo().Length; i++)
		{
			for (int j = 0; j < SingletonCustom<Curling_GameManager>.Instance.GetArrayStone(i).Length; j++)
			{
				Curling_Stone curling_Stone2 = SingletonCustom<Curling_GameManager>.Instance.GetArrayStone(i)[j];
				UnityEngine.Debug.Log("stone " + curling_Stone2.gameObject.name);
				if ((_targetStone != null && curling_Stone2 == _targetStone) || !curling_Stone2.gameObject.activeSelf || curling_Stone2.GetRigid().isKinematic || curling_Stone2.GetIsFailure())
				{
					continue;
				}
				vector = curling_Stone2.transform.position + curling_Stone2.GetStoneObjectHalfSize() * base.transform.right;
				vector2 = vector - base.transform.position;
				vector2.y = 0f;
				vector2 += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
				float num5 = Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
				vector = curling_Stone2.transform.position + curling_Stone2.GetStoneObjectHalfSize() * -base.transform.right;
				vector2 = vector - base.transform.position;
				vector2.y = 0f;
				vector2 += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
				float num6 = Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
				UnityEngine.Debug.Log("stoneRightAngle " + num5.ToString());
				UnityEngine.Debug.Log("stoneLeftAngle " + num6.ToString());
				float num7 = targetPos.z - curling_Stone2.transform.position.z - curling_Stone2.GetStoneObjectHalfSize();
				UnityEngine.Debug.Log("diffZ " + num7.ToString());
				if (num7 >= 0.5f && num7 <= 2f && num2 >= num5 && num3 <= num6)
				{
					UnityEngine.Debug.Log("一定角度以内に他の石がある");
					float num8 = CalcManager.Length(new Vector3(curling_Stone2.transform.position.x, 0f, curling_Stone2.transform.position.z), new Vector3(targetPos.x, 0f, targetPos.z)) - curling_Stone2.GetStoneObjectHalfSize();
					UnityEngine.Debug.Log("nearDistanceTmp " + num8.ToString());
					if (num8 < num4)
					{
						curling_Stone = curling_Stone2;
						num4 = num8;
					}
				}
			}
		}
		UnityEngine.Debug.Log("nearStone : " + ((curling_Stone != null) ? curling_Stone.gameObject.name : "null"));
		if (curling_Stone != null && curling_Stone.GetTeam() != SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().GetTeam())
		{
			if (_targetStone != null)
			{
				Vector3 vector4 = curling_Stone.transform.position - base.transform.position;
				vector4.y = 0f;
				vector4 += SingletonCustom<Curling_GameManager>.Instance.GetCharaToStoneVec();
				UnityEngine.Debug.Log("Vector3.Angle(stoneVec, transform.forward)  " + Vector3.Angle(vector4, base.transform.forward).ToString());
				if (Vector3.Angle(vector4, base.transform.forward) <= SingletonCustom<Curling_GameManager>.Instance.GetStoneThrowCurveVectorDiff())
				{
					float num9 = Mathf.Atan2(vector4.z, vector4.x) * 57.29578f;
					UnityEngine.Debug.Log("stoneAngle " + num9.ToString());
					if (num > num9)
					{
						throw_CurveDir = Vector3.right;
					}
					else if (num < num9)
					{
						throw_CurveDir = Vector3.left;
					}
				}
				else
				{
					throw_CurveDir = Vector3.zero;
				}
			}
			else
			{
				finalTargetThrowPower = THROW_TARGET_STONE_POWER * CPU_TARGET_POWER_DIFF;
			}
			num += throw_CurveDir.x * SingletonCustom<Curling_GameManager>.Instance.GetStoneThrowCurveVectorDiff() * 1.05f;
			float f = num * ((float)Math.PI / 180f);
			finalTargetVec = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
		if (throw_CurveDir == Vector3.zero)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.STRAIGHT);
		}
		else if (throw_CurveDir == Vector3.right)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.RIGHT_CURVE);
		}
		else if (throw_CurveDir == Vector3.left)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.LEFT_CURVE);
		}
	}
	public void HouseSweep()
	{
		player.HouseSweepRunAnimation();
		if (!isHouseSweepCurve)
		{
			return;
		}
		if (houseSweepInterval >= HOUSE_SWEEP_INTERVAL)
		{
			houseSweepInterval = 0f;
			Vector3 a = new Vector3(targetPos.x, 0f, targetPos.z);
			Vector3 b = new Vector3(player.GetStone().transform.position.x, 0f, player.GetStone().transform.position.z);
			Vector3 vector = a - b;
			UnityEngine.Debug.Log("vec x : " + vector.x.ToString() + " y : " + vector.y.ToString() + " z : " + vector.z.ToString());
			if (vector.z > 0f)
			{
				vector.y = 0f;
				vector.z = 0f;
				if (vector.x != 0f)
				{
					vector.x = Mathf.Sign(vector.x);
				}
				player.HouseSweepAnimation(vector);
			}
		}
		else
		{
			houseSweepInterval += Time.deltaTime;
		}
	}
	public void SetTargetPos(Vector3 _targetPos)
	{
		targetPos = _targetPos;
	}
	public void SkipThrowStone()
	{
		base.transform.rotation = Quaternion.LookRotation(finalTargetVec);
		throw_Power = finalTargetThrowPower;
		SingletonCustom<Curling_GameManager>.Instance.ThrowStone(throw_CurveDir, base.transform.forward, throw_Power);
	}
}
