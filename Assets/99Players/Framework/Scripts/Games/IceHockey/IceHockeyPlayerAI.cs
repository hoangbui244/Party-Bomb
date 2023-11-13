using SaveDataDefine;
using UnityEngine;
public class IceHockeyPlayerAI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private IceHockeyPlayer player;
	private float timeThrow;
	private Vector3 moveForce;
	private Vector3 moveOffset;
	private float passWaitTime;
	private float attackPosZ;
	private float faceOffTime;
	private Vector3 keeperOffset = Vector3.zero;
	private readonly float OFFSET_UPDATE = 0.4f;
	private bool isCheck;
	private bool isShoot;
	private bool isPass;
	private float updateTime;
	private Vector3 attackPos;
	private RaycastHit info;
	private Vector3 prevForce;
	private float tempScale = 0.1f;
	public Vector3 KeeperOffset => keeperOffset;
	public void Init(IceHockeyPlayer _player)
	{
		player = _player;
		timeThrow = 2.5f;
	}
	public Vector3 UpdateForceReferee()
	{
		prevForce = moveForce;
		moveForce = Vector3.zero;
		CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		if ((CalcManager.mCalcVector3 - base.transform.position).magnitude <= 4f)
		{
			moveForce = (base.transform.position - CalcManager.mCalcVector3).normalized * 0.45f;
		}
		else
		{
			if (SingletonCustom<IceHockeyPuck>.Instance.transform.position.z < SingletonCustom<IceHockeyRinkManager>.Instance.Center.position.z)
			{
				if (SingletonCustom<IceHockeyPuck>.Instance.transform.position.z >= SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[0].position.z * 0.9f)
				{
					CalcManager.mCalcVector3.z = SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[1].position.z;
				}
				else
				{
					CalcManager.mCalcVector3.z = SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[0].position.z;
				}
			}
			else if (SingletonCustom<IceHockeyPuck>.Instance.transform.position.z <= SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[1].position.z * 0.9f)
			{
				CalcManager.mCalcVector3.z = SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[0].position.z;
			}
			else
			{
				CalcManager.mCalcVector3.z = SingletonCustom<IceHockeyRinkManager>.Instance.ArrayMoveRefereeAnchor[1].position.z;
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
		updateTime += Time.deltaTime * UnityEngine.Random.Range(0.5f, 1f);
		if (updateTime >= OFFSET_UPDATE)
		{
			moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(0f, 2f) : UnityEngine.Random.Range(-2f, 0f));
			if (base.transform.position.z > SingletonCustom<IceHockeyRinkManager>.Instance.Center.position.z + IceHockeyRinkManager.RINK_Z_SIZE * 0.7f)
			{
				moveOffset.z = UnityEngine.Random.Range(-3f, 0f);
			}
			else if (base.transform.position.z < SingletonCustom<IceHockeyRinkManager>.Instance.Center.position.z - IceHockeyRinkManager.RINK_Z_SIZE * 0.7f)
			{
				moveOffset.z = UnityEngine.Random.Range(0f, 3f);
			}
			else
			{
				moveOffset.z = UnityEngine.Random.Range(-2f, 2f);
			}
			if (SingletonCustom<IceHockeyPuck>.Instance.Holder != null && SingletonCustom<IceHockeyPuck>.Instance.Holder == player)
			{
				moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(0f, 5f) : UnityEngine.Random.Range(-5f, 0f));
				if (UnityEngine.Random.Range(0, 100) <= 70)
				{
					attackPos = SingletonCustom<IceHockeyRinkManager>.Instance.GetAttackAnchor(player.TeamNo, IceHockeyPlayer.PositionNo.CF).position + moveOffset;
					attackPos.z += attackPosZ;
				}
				else
				{
					attackPos = base.transform.position + moveOffset * 2f;
				}
			}
			updateTime = 0f;
		}
		if (player.Position != IceHockeyPlayer.PositionNo.GK)
		{
			tempScale = 0.75f;
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				tempScale = 0.7f;
				break;
			case SystemData.AiStrength.STRONG:
				tempScale = 0.85f;
				break;
			}
			if (SingletonCustom<IceHockeyPuck>.Instance.Holder == null)
			{
				if (!player.IsPassDelay)
				{
					if (player.IsPuckChase)
					{
						CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position;
						CalcManager.mCalcVector3.y = base.transform.position.y;
						moveForce = (CalcManager.mCalcVector3 - base.transform.position).normalized * (tempScale + 0.1f);
					}
					else if (SingletonCustom<IceHockeyRinkManager>.Instance.IsOpponentPuck(player.TeamNo))
					{
						moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetAttackAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
					}
					else
					{
						moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
					}
				}
			}
			else if (SingletonCustom<IceHockeyPuck>.Instance.Holder.TeamNo == player.TeamNo)
			{
				if (SingletonCustom<IceHockeyPuck>.Instance.Holder == player)
				{
					moveForce = (attackPos - base.transform.position).normalized * 0.88f;
					passWaitTime = Mathf.Clamp(passWaitTime - Time.deltaTime, 0f, 1f);
					if (Vector3.Distance(SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(player.TeamNo).position, base.transform.position) <= 14.5f)
					{
						if (Physics.SphereCast(new Ray(base.transform.position, SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(player.TeamNo).position - base.transform.position), 0.15f, out info) && info.rigidbody != null)
						{
							IceHockeyPlayer component = info.rigidbody.GetComponent<IceHockeyPlayer>();
							if ((object)component == null || component.Position != IceHockeyPlayer.PositionNo.GK)
							{
								return moveForce;
							}
						}
						if (player.CurrentState != IceHockeyPlayer.State.SHOOT)
						{
							isShoot = true;
						}
					}
					else if (passWaitTime <= 0f && player.TempTarget != null && SingletonCustom<IceHockeyRinkManager>.Instance.IsTeamPlayerFront(base.transform.position, player.TempTarget) && UnityEngine.Random.Range(0, 100) <= (SingletonCustom<IceHockeyRinkManager>.Instance.IsOpponentArea(player) ? 1 : 3))
					{
						isPass = true;
					}
				}
				else if (SingletonCustom<IceHockeyPuck>.Instance.Holder.Position == IceHockeyPlayer.PositionNo.GK)
				{
					if (Vector3.Distance(SingletonCustom<IceHockeyRinkManager>.Instance.GetDefenceAnchor(player).position, base.transform.position) <= 4f && (player.Position == IceHockeyPlayer.PositionNo.LD || player.Position == IceHockeyPlayer.PositionNo.RD))
					{
						moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * 0.35f;
					}
					else
					{
						moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
					}
				}
				else
				{
					moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetAttackAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
				}
			}
			else if (SingletonCustom<IceHockeyPuck>.Instance.Holder.Position != IceHockeyPlayer.PositionNo.GK && player.IsPuckChase)
			{
				CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				if (Vector3.Distance(CalcManager.mCalcVector3, base.transform.position) <= 3f && Mathf.Abs(Vector3.Angle(player.MoveDir, CalcManager.mCalcVector3 - base.transform.position)) <= 10f && player.Rigid.velocity.magnitude >= 3f)
				{
					isCheck = true;
				}
				moveForce = (CalcManager.mCalcVector3 - base.transform.position).normalized * tempScale;
			}
			else
			{
				moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetDefenceAnchor(player).position + moveOffset - base.transform.position).normalized * tempScale;
			}
			moveForce = Vector3.Slerp(prevForce, moveForce, 0.25f);
		}
		else
		{
			tempScale = 0.1f;
			SystemData.AiStrength aiStrengthSetting = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting();
			if ((uint)aiStrengthSetting > 1u && aiStrengthSetting == SystemData.AiStrength.STRONG)
			{
				tempScale = 0.25f;
			}
			CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position + SingletonCustom<IceHockeyPuck>.Instance.Rigid.velocity * tempScale;
			CalcManager.mCalcVector3.y = base.transform.position.y;
			moveForce = (CalcManager.mCalcVector3 - (base.transform.position + player.KeeperOffset)).normalized * 1f;
			moveForce = Vector3.Slerp(prevForce, moveForce, 0.85f);
		}
		return moveForce;
	}
	public void SetFaceOff()
	{
		faceOffTime = UnityEngine.Random.Range(0.15f, 0.45f);
		keeperOffset.z = 0f;
	}
	public void HavePuck()
	{
		passWaitTime = UnityEngine.Random.Range(0.5f, 0.75f);
		switch (UnityEngine.Random.Range(0, 2))
		{
		case 0:
			attackPosZ = UnityEngine.Random.Range(-5f, -2.5f);
			break;
		case 1:
			attackPosZ = UnityEngine.Random.Range(2.5f, 5f);
			break;
		}
		moveOffset.x = ((player.TeamNo == 0) ? UnityEngine.Random.Range(0f, 1f) : UnityEngine.Random.Range(-1f, 0f));
		moveOffset.z = UnityEngine.Random.Range(-2.5f, 2.5f);
		if (UnityEngine.Random.Range(0, 100) <= 70)
		{
			attackPos = SingletonCustom<IceHockeyRinkManager>.Instance.GetAttackAnchor(player.TeamNo, IceHockeyPlayer.PositionNo.CF).position + moveOffset;
			attackPos.z += attackPosZ;
		}
		else if (SingletonCustom<IceHockeyRinkManager>.Instance.Center.position.z > base.transform.position.z)
		{
			attackPos = base.transform.position;
			attackPos.x = ((player.TeamNo == 0) ? 5f : (-5f));
			attackPos.z += UnityEngine.Random.Range(0f, 2.5f);
		}
		else
		{
			attackPos = base.transform.position;
			attackPos.x = ((player.TeamNo == 0) ? 5f : (-5f));
			attackPos.z += UnityEngine.Random.Range(-2.5f, 0f);
		}
	}
	public bool IsFaceOff()
	{
		faceOffTime -= Time.deltaTime;
		if (faceOffTime <= 0f)
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
			timeThrow = 2.5f;
			return true;
		}
		return false;
	}
	public bool IsCheck()
	{
		if (isCheck)
		{
			isCheck = false;
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
