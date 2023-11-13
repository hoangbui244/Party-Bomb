using SaveDataDefine;
using UnityEngine;
public class BeachSoccerPlayerAI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private BeachSoccerPlayer player;
	[SerializeField]
	[Header("【デバッグ】移動停止")]
	private bool isMoveStop;
	private float timeThrow;
	private Vector3 moveForce;
	private Vector3 moveOffset;
	private float passWaitTime;
	private float attackPosZ;
	private float kickOffTime;
	private Vector3 keeperOffset = Vector3.zero;
	private readonly float OFFSET_UPDATE = 0.4f;
	private bool isSliding;
	private bool isShoot;
	private bool isPass;
	private float keeperBallChase;
	private float updateTime;
	private Vector3 attackPos;
	private RaycastHit info;
	private Vector3 prevForce;
	private float tempScale = 0.1f;
	private Vector3 targetDefencePos;
	private Vector3 targetAttackPos;
	public void Init()
	{
		timeThrow = UnityEngine.Random.Range(1.25f, 1.75f);
	}
	public Vector3 UpdateForceReferee()
	{
		prevForce = moveForce;
		moveForce = Vector3.zero;
		CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		if ((CalcManager.mCalcVector3 - base.transform.position).magnitude <= 4f)
		{
			moveForce = (base.transform.position - CalcManager.mCalcVector3).normalized * 0.45f;
		}
		else
		{
			if (SingletonCustom<BeachSoccerBall>.Instance.transform.position.z < SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z)
			{
				if (SingletonCustom<BeachSoccerBall>.Instance.transform.position.z >= SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[0].position.z * 0.9f)
				{
					CalcManager.mCalcVector3.z = SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[1].position.z;
				}
				else
				{
					CalcManager.mCalcVector3.z = SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[0].position.z;
				}
			}
			else if (SingletonCustom<BeachSoccerBall>.Instance.transform.position.z <= SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[1].position.z * 0.9f)
			{
				CalcManager.mCalcVector3.z = SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[0].position.z;
			}
			else
			{
				CalcManager.mCalcVector3.z = SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayMoveRefereeAnchor[1].position.z;
			}
			if ((CalcManager.mCalcVector3 - base.transform.position + (base.transform.position - CalcManager.mCalcVector3) * 0.15f).magnitude <= 1f)
			{
				moveForce = Vector3.zero;
			}
			else
			{
				moveForce = (CalcManager.mCalcVector3 - base.transform.position + (base.transform.position - CalcManager.mCalcVector3) * 0.15f).normalized * 0.45f;
			}
		}
		return moveForce;
	}
	public Vector3 UpdateForce()
	{
		prevForce = moveForce;
		moveForce = Vector3.zero;
		if (isMoveStop)
		{
			return moveForce;
		}
		updateTime += Time.deltaTime * UnityEngine.Random.Range(1f, 1.5f);
		if (updateTime >= OFFSET_UPDATE)
		{
			moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(moveOffset.x, 1f) : UnityEngine.Random.Range(-1f, moveOffset.x));
			if (base.transform.position.z > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z + BeachSoccerFieldManager.RINK_Z_SIZE * 0.85f)
			{
				moveOffset.z = UnityEngine.Random.Range(-2f, 0f);
			}
			else if (base.transform.position.z < SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z - BeachSoccerFieldManager.RINK_Z_SIZE * 0.85f)
			{
				moveOffset.z = UnityEngine.Random.Range(0f, 2f);
			}
			else
			{
				moveOffset.z = UnityEngine.Random.Range(-2f, 2f);
			}
			if (SingletonCustom<BeachSoccerBall>.Instance.Holder != null && SingletonCustom<BeachSoccerBall>.Instance.Holder == player)
			{
				moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(moveOffset.x, 3f) : UnityEngine.Random.Range(-3f, moveOffset.x));
				if (UnityEngine.Random.Range(0, 100) <= 70)
				{
					attackPos = SingletonCustom<BeachSoccerFieldManager>.Instance.GetAttackAnchor(player.TeamNo, BeachSoccerPlayer.PositionNo.FW).position + moveOffset;
					attackPos.z += attackPosZ;
				}
				else
				{
					attackPos = base.transform.position + moveOffset * 2f;
				}
			}
			updateTime = 0f;
		}
		if (player.Position != BeachSoccerPlayer.PositionNo.GK)
		{
			tempScale = 0.75f;
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				tempScale = 0.7f;
				break;
			case SystemData.AiStrength.NORAML:
				tempScale = 0.8f;
				break;
			case SystemData.AiStrength.STRONG:
				tempScale = 0.95f;
				break;
			}
			if (SingletonCustom<BeachSoccerBall>.Instance.Holder == null)
			{
				if (!player.IsPassDelay)
				{
					if (player.IsBallChase)
					{
						CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
						CalcManager.mCalcVector3.y = base.transform.position.y;
						moveForce = (CalcManager.mCalcVector3 - base.transform.position).normalized * (tempScale + 0.1f);
					}
					else if (SingletonCustom<BeachSoccerFieldManager>.Instance.IsOpponentPuck(player.TeamNo))
					{
						moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetAttackAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
					}
					else
					{
						moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
					}
				}
			}
			else if (SingletonCustom<BeachSoccerBall>.Instance.Holder.TeamNo == player.TeamNo)
			{
				if (SingletonCustom<BeachSoccerBall>.Instance.Holder == player)
				{
					moveForce = (attackPos - base.transform.position).normalized * 0.88f;
					passWaitTime = Mathf.Clamp(passWaitTime - Time.deltaTime, 0f, 1f);
					if (Vector3.Distance(SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(player.TeamNo).position, base.transform.position) <= 3.25f)
					{
						if (player.CurrentState != BeachSoccerPlayer.State.SHOOT)
						{
							isShoot = true;
						}
					}
					else if (passWaitTime <= 0f && player.TempTarget != null && SingletonCustom<BeachSoccerFieldManager>.Instance.IsTeamPlayerFront(base.transform.position, player.TempTarget) && UnityEngine.Random.Range(0, 100) <= (SingletonCustom<BeachSoccerFieldManager>.Instance.IsOpponentArea(player) ? 10 : 30))
					{
						isPass = true;
					}
				}
				else
				{
					targetDefencePos = SingletonCustom<BeachSoccerFieldManager>.Instance.GetDefenceAnchor(player).position + moveOffset;
					targetAttackPos = SingletonCustom<BeachSoccerFieldManager>.Instance.GetAttackAnchor(player).position + moveOffset;
					if (SingletonCustom<BeachSoccerBall>.Instance.Holder.Position == BeachSoccerPlayer.PositionNo.GK)
					{
						if (Vector3.Distance(SingletonCustom<BeachSoccerFieldManager>.Instance.GetDefenceAnchor(player).position, base.transform.position) <= 4f && player.Position == BeachSoccerPlayer.PositionNo.DF)
						{
							moveForce = (targetDefencePos - base.transform.position).normalized * 0.45f;
							if (Vector3.Distance(targetDefencePos, base.transform.position) <= 0.75f)
							{
								moveForce = Vector3.zero;
							}
						}
						else
						{
							moveForce = (targetDefencePos - base.transform.position).normalized * tempScale;
							if (Vector3.Distance(targetDefencePos, base.transform.position) <= 1f)
							{
								moveForce = Vector3.zero;
								updateTime = OFFSET_UPDATE;
							}
						}
					}
					else
					{
						moveForce = (targetAttackPos - base.transform.position).normalized * tempScale;
						if (Vector3.Distance(targetAttackPos, base.transform.position) <= 0.75f)
						{
							moveForce = Vector3.zero;
							updateTime = OFFSET_UPDATE;
						}
					}
				}
			}
			else if (SingletonCustom<BeachSoccerBall>.Instance.Holder.Position != BeachSoccerPlayer.PositionNo.GK && player.IsBallChase && SingletonCustom<BeachSoccerBall>.Instance.Holder.CurrentState != BeachSoccerPlayer.State.THROW_IN && SingletonCustom<BeachSoccerBall>.Instance.Holder.CurrentState != BeachSoccerPlayer.State.THROW_IN_WAIT && SingletonCustom<BeachSoccerBall>.Instance.Holder.CurrentState != BeachSoccerPlayer.State.CORNER_KICK && SingletonCustom<BeachSoccerBall>.Instance.Holder.CurrentState != BeachSoccerPlayer.State.CORNER_KICK_WAIT)
			{
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position + SingletonCustom<BeachSoccerBall>.Instance.Holder.Style.transform.forward * 0.1f + moveOffset;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				if (Vector3.Distance(CalcManager.mCalcVector3, base.transform.position) <= 2.05f && Vector3.Angle(player.Style.transform.forward, SingletonCustom<BeachSoccerBall>.Instance.transform.position - base.transform.position) <= 20f && player.MoveSpeed >= 1.05f)
				{
					if (UnityEngine.Random.Range(0, 100) <= 2)
					{
						isSliding = true;
					}
					else
					{
						CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position + SingletonCustom<BeachSoccerBall>.Instance.Holder.Style.transform.forward * 0.1f;
						CalcManager.mCalcVector3.y = base.transform.position.y;
						updateTime = OFFSET_UPDATE;
					}
				}
				UnityEngine.Debug.Log("距離:" + Vector3.Distance(CalcManager.mCalcVector3, base.transform.position).ToString());
				UnityEngine.Debug.Log("角度:" + Vector3.Angle(player.Style.transform.forward, SingletonCustom<BeachSoccerBall>.Instance.transform.position - base.transform.position).ToString());
				UnityEngine.Debug.Log("MoveSpeed:" + player.MoveSpeed.ToString());
				moveForce = (CalcManager.mCalcVector3 - base.transform.position).normalized * tempScale;
				UnityEngine.Debug.Log("Chase");
			}
			else
			{
				moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
			}
			moveForce = Vector3.Slerp(prevForce, moveForce, 0.25f);
		}
		else
		{
			CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerFieldManager>.Instance.GetDefenceAnchor(player.TeamNo, BeachSoccerPlayer.PositionNo.GK).position;
			CalcManager.mCalcVector3.z -= (SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z - SingletonCustom<BeachSoccerBall>.Instance.transform.position.z) * 0.25f;
			if (Vector3.Distance(SingletonCustom<BeachSoccerFieldManager>.Instance.GetGoalAnchor(player.TeamNo).position, base.transform.position) <= 1.25f && Vector3.Distance(SingletonCustom<BeachSoccerBall>.Instance.transform.position, base.transform.position) <= 1.15f)
			{
				keeperBallChase = 0.25f;
			}
			if (keeperBallChase > 0f)
			{
				keeperBallChase -= Time.deltaTime;
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
			}
			if ((CalcManager.mCalcVector3 - base.transform.position).magnitude <= 0.05f)
			{
				moveForce = Vector3.zero;
			}
			else
			{
				moveForce = (CalcManager.mCalcVector3 - base.transform.position).normalized * 0.85f;
			}
		}
		return moveForce;
	}
	public void SetKickOff()
	{
		kickOffTime = UnityEngine.Random.Range(0.15f, 0.45f);
		keeperOffset.z = 0f;
	}
	public void HaveBall()
	{
		passWaitTime = UnityEngine.Random.Range(0.5f, 0.75f);
		switch (UnityEngine.Random.Range(0, 2))
		{
		case 0:
			attackPosZ = UnityEngine.Random.Range(-3f, -2f);
			break;
		case 1:
			attackPosZ = UnityEngine.Random.Range(2f, 3f);
			break;
		}
		moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(0f, 0.5f) : UnityEngine.Random.Range(-0.5f, 0f));
		moveOffset.z = UnityEngine.Random.Range(-1f, 1f);
		if (UnityEngine.Random.Range(0, 100) <= 70)
		{
			attackPos = SingletonCustom<BeachSoccerFieldManager>.Instance.GetAttackAnchor(player.TeamNo, BeachSoccerPlayer.PositionNo.DF).position + moveOffset;
			attackPos.z += attackPosZ;
		}
		else if (SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z > base.transform.position.z)
		{
			attackPos = base.transform.position;
			attackPos.x = ((player.TeamNo == 0) ? 2f : (-2f));
			attackPos.z += UnityEngine.Random.Range(0f, 1f);
		}
		else
		{
			attackPos = base.transform.position;
			attackPos.x = ((player.TeamNo == 0) ? 2f : (-2f));
			attackPos.z += UnityEngine.Random.Range(-1f, 0f);
		}
		keeperBallChase = 0f;
	}
	public bool IsKickOff()
	{
		kickOffTime -= Time.deltaTime;
		if (kickOffTime <= 0f)
		{
			return true;
		}
		return false;
	}
	public bool IsThorw()
	{
		timeThrow -= Time.deltaTime;
		if (timeThrow <= 0f)
		{
			timeThrow = UnityEngine.Random.Range(0.75f, 1.25f);
			return true;
		}
		return false;
	}
	public bool IsSliding()
	{
		if (isSliding)
		{
			isSliding = false;
			return true;
		}
		return false;
	}
	public bool IsShoot()
	{
		if (isShoot)
		{
			isShoot = false;
			return true;
		}
		return false;
	}
	public bool IsPass()
	{
		if (isPass)
		{
			isPass = false;
			return true;
		}
		return false;
	}
}
