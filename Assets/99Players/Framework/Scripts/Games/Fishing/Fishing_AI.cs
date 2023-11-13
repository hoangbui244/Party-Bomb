using System;
using UnityEngine;
public class Fishing_AI : MonoBehaviour
{
	private enum AIActionState
	{
		NONE,
		STANDBY,
		MOVE,
		ROD_CAST,
		ROD_SET_UP,
		FISHING,
		ROD_CANCEL
	}
	[Serializable]
	private struct AIStrengthData
	{
		public float defFishingSucessPer;
		public float defFishingFailPer;
	}
	[SerializeField]
	[Header("BoxCastのレイを飛ばすアンカ\u30fc")]
	private Transform boxCastAnchor;
	[SerializeField]
	[Header("BoxCastのギズモ表示")]
	private bool isEnable;
	private bool isHitRayCast_BetweenToTarget;
	private bool isHitBoxCast_BetweenToTarget;
	private bool isHitSearchBoxCast;
	private bool isClearBetweenToTarget;
	private RaycastHit hit_RayCast;
	private RaycastHit hit_BoxCast;
	private RaycastHit hit_Character;
	private const float BOX_CAST_DISTANCE = 1f;
	private float boxCastScale;
	private float searchAngle_End;
	private float nowSearchAngle;
	private Vector3 searchDirection = Vector3.zero;
	private Vector3 lastSearchHitPoint = Vector3.zero;
	private Vector3 lastSearchDirection = Vector3.zero;
	private Vector3 originToTargetDirection = Vector3.zero;
	private float targetDistance;
	private float currentMoveTargetRemainTime;
	private const float MOVE_TARGET_REMAIN_TIME = 5f;
	private int duringPathRootId = -1;
	private bool avoidCharacter;
	private Vector3 avoidCharacterDirection = Vector3.zero;
	private Vector3 pathMoveDirection = Vector3.zero;
	private Fishing_Character moveObstacleCharacter;
	private float obstacleCharaDist;
	private const float CHECK_OBSTACLE_CHARA_DIST = 0.5f;
	private AIStrengthData strengthData;
	private AIActionState actionState;
	private Fishing_Character character;
	private Vector3[] calcVec = new Vector3[2];
	private Vector3 rot;
	private FishingDefinition.AiStrength aiStrength;
	private float STOP_CHECK_DISTANCE = 0.1f;
	private float runTime;
	private Vector3 targetFishPoint = Vector3.zero;
	private Vector3 targetFishStandPoint = Vector3.zero;
	private int userDataNo = -1;
	private float currentFishingTime;
	private float fishingContinueTime;
	private const float FISHING_CONTINUE_TIME_WEAK = 7.5f;
	private const float FISHING_CONTINUE_TIME_COMMON = 5f;
	private const float FISHING_CONTINUE_TIME_STRONG = 2.5f;
	private float nowTargetToAngle;
	private Fishing_Field.PathRootData pathRootData;
	private bool pathRootMove;
	private bool pathRootStartPointEnd;
	private Vector3 tempVecOrigin;
	private Vector3 tempVecTarget;
	public void Init(Fishing_Character _character, int _userDataNo)
	{
		aiStrength = (FishingDefinition.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		character = _character;
		userDataNo = _userDataNo;
		actionState = AIActionState.STANDBY;
		boxCastScale = base.transform.lossyScale.x * 0.2f;
		switch (aiStrength)
		{
		case FishingDefinition.AiStrength.Weak:
			strengthData.defFishingSucessPer = 75f;
			strengthData.defFishingFailPer = 25f;
			fishingContinueTime = 7.5f;
			break;
		case FishingDefinition.AiStrength.Normal:
			strengthData.defFishingSucessPer = 95f;
			strengthData.defFishingFailPer = 5f;
			fishingContinueTime = 5f;
			break;
		case FishingDefinition.AiStrength.Strong:
			strengthData.defFishingSucessPer = 100f;
			strengthData.defFishingFailPer = 0f;
			fishingContinueTime = 2.5f;
			break;
		}
	}
	public void UpdateMethod()
	{
		switch (actionState)
		{
		case AIActionState.MOVE:
			break;
		case AIActionState.STANDBY:
			AiStandBy();
			break;
		case AIActionState.ROD_CAST:
			AiRodCast();
			break;
		case AIActionState.ROD_SET_UP:
			AiRodSetUp();
			break;
		case AIActionState.FISHING:
			AiFishing();
			break;
		case AIActionState.ROD_CANCEL:
			AiRodCancel();
			break;
		}
	}
	public void FixedUpdateMethod()
	{
		if (actionState == AIActionState.MOVE)
		{
			AiMove();
			targetDistance = GetTargetDistance();
		}
	}
	private void AiMove()
	{
		if (currentMoveTargetRemainTime < 0f)
		{
			actionState = AIActionState.STANDBY;
			return;
		}
		currentMoveTargetRemainTime -= Time.deltaTime;
		if (FishingDefinition.FSM.CheckObstacleStandPoint(userDataNo, targetFishStandPoint))
		{
			actionState = AIActionState.STANDBY;
		}
		else if (RunTowardTarget(targetFishStandPoint))
		{
			character.transform.position = targetFishStandPoint;
			LookTargetImmediately();
			character.RodAction();
			if (character.GetAnimationType() == Fishing_Character.AnimationType.RodCast)
			{
				currentFishingTime = fishingContinueTime;
				actionState = AIActionState.ROD_CAST;
			}
		}
	}
	private void AiStandBy()
	{
		SetFishPointCalculate();
	}
	private void AiRodCast()
	{
		if (character.GetAnimationType() == Fishing_Character.AnimationType.RodSetUp)
		{
			actionState = AIActionState.ROD_SET_UP;
		}
	}
	private void AiRodSetUp()
	{
		if (!character.IsStartBite())
		{
			if (currentFishingTime < 0f)
			{
				actionState = AIActionState.ROD_CANCEL;
			}
			else
			{
				currentFishingTime -= Time.deltaTime;
			}
		}
		if (character.IsBite())
		{
			if ((float)UnityEngine.Random.Range(0, 100) > strengthData.defFishingFailPer)
			{
				character.RodAction();
				actionState = AIActionState.FISHING;
			}
			else
			{
				actionState = AIActionState.ROD_CANCEL;
			}
		}
	}
	private void AiFishing()
	{
		if (character.GetAnimationType() == Fishing_Character.AnimationType.Wait)
		{
			actionState = AIActionState.STANDBY;
		}
	}
	private void AiRodCancel()
	{
		if (character.GetAnimationType() == Fishing_Character.AnimationType.RodSetUp)
		{
			if (!character.IsBite())
			{
				character.RodAction();
			}
		}
		else if (character.GetAnimationType() == Fishing_Character.AnimationType.Wait)
		{
			actionState = AIActionState.STANDBY;
		}
	}
	private bool RunTowardTarget(Vector3 _pos)
	{
		if (Vector3.Distance(_pos, character.GetPos(_isLocal: false)) > STOP_CHECK_DISTANCE)
		{
			character.Move(GetMoveToTargetDirection().normalized);
			return false;
		}
		return true;
	}
	private bool RunTargetCertainTime(Vector3 _pos, float _time)
	{
		runTime += Time.deltaTime;
		if (runTime <= _time)
		{
			character.Move(_pos - character.GetPos(_isLocal: false));
			return true;
		}
		runTime = 0f;
		return false;
	}
	private void LookForward()
	{
		character.GetRigid().MoveRotation(Quaternion.Euler(0f, 180f, 0f));
	}
	private void LookTargetCharacter()
	{
		calcVec[0] = (targetFishPoint - character.GetPos(_isLocal: false)).normalized;
		rot.x = 0f;
		rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
		rot.z = 0f;
		character.GetRigid().MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
	}
	private void LookTargetImmediately()
	{
		calcVec[0] = (targetFishPoint - character.GetPos(_isLocal: false)).normalized;
		rot.x = 0f;
		rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
		rot.z = 0f;
		character.transform.eulerAngles = rot;
	}
	private bool IsTargetCharaActive()
	{
		Vector3 targetFishStandPoint2 = targetFishStandPoint;
		return true;
	}
	private float GetTargetDistance()
	{
		tempVecOrigin = base.transform.position;
		tempVecTarget = targetFishStandPoint;
		tempVecOrigin.y = 0f;
		tempVecTarget.y = 0f;
		return Vector3.Distance(tempVecOrigin, tempVecTarget);
	}
	private Vector3 GetMoveToTargetDirection()
	{
		if (!isClearBetweenToTarget)
		{
			originToTargetDirection = targetFishStandPoint - base.transform.position;
			isHitRayCast_BetweenToTarget = Physics.Raycast(boxCastAnchor.position, originToTargetDirection, out hit_RayCast, targetDistance, LayerMask.GetMask(FishingDefinition.LayerWall));
			isHitBoxCast_BetweenToTarget = Physics.BoxCast(boxCastAnchor.position, Vector3.one * boxCastScale, originToTargetDirection, out hit_BoxCast, base.transform.rotation, targetDistance, LayerMask.GetMask(FishingDefinition.LayerWall));
			if (!isHitRayCast_BetweenToTarget && !isHitBoxCast_BetweenToTarget)
			{
				isClearBetweenToTarget = true;
				return originToTargetDirection;
			}
			if (!pathRootMove)
			{
				pathRootData = FishingDefinition.FM.GetNearStartPathRoot(base.transform.position, targetFishStandPoint);
				pathRootStartPointEnd = false;
				pathRootMove = true;
				duringPathRootId = 0;
			}
			if (pathRootData.startPoint == null || pathRootData.endPoint == null)
			{
				pathRootStartPointEnd = false;
				pathRootMove = false;
				duringPathRootId = -1;
				return originToTargetDirection;
			}
			if (Vector3.Distance(base.transform.position, pathRootData.pathPoint[duringPathRootId].position) < STOP_CHECK_DISTANCE && !pathRootStartPointEnd)
			{
				duringPathRootId++;
			}
			if (duringPathRootId == pathRootData.pathPoint.Length)
			{
				pathRootStartPointEnd = false;
				pathRootMove = false;
				duringPathRootId = -1;
				return originToTargetDirection;
			}
			pathMoveDirection = pathRootData.pathPoint[duringPathRootId].position - base.transform.position;
			return pathMoveDirection;
		}
		isHitRayCast_BetweenToTarget = Physics.Raycast(boxCastAnchor.position, originToTargetDirection, out hit_RayCast, targetDistance, LayerMask.GetMask(FishingDefinition.LayerWall));
		if (isHitRayCast_BetweenToTarget)
		{
			isClearBetweenToTarget = false;
		}
		return originToTargetDirection;
	}
	private void InitFishPointCalculateData()
	{
		isClearBetweenToTarget = false;
		isHitRayCast_BetweenToTarget = false;
		isHitBoxCast_BetweenToTarget = false;
		isHitSearchBoxCast = false;
		targetFishPoint = Vector3.zero;
		targetFishStandPoint = Vector3.zero;
		targetDistance = 0f;
		searchDirection = Vector3.zero;
		lastSearchHitPoint = Vector3.zero;
		lastSearchDirection = Vector3.zero;
		originToTargetDirection = Vector3.zero;
		pathRootMove = false;
	}
	private void SetFishPointCalculate()
	{
		InitFishPointCalculateData();
		if (!FishingDefinition.FSM.CheckFishShadowAllowFishing(userDataNo, aiStrength))
		{
			return;
		}
		targetFishPoint = FishingDefinition.FSM.GetFishShadowPoint(userDataNo);
		for (int i = 0; i < FishingDefinition.FSM.GetFishShadowStandPoint(userDataNo).Count; i++)
		{
			if (targetFishStandPoint == Vector3.zero)
			{
				targetFishStandPoint = FishingDefinition.FSM.GetFishShadowStandPoint(userDataNo)[i];
			}
			else if (!FishingDefinition.FSM.CheckObstacleStandPoint(userDataNo, FishingDefinition.FSM.GetFishShadowStandPoint(userDataNo)[i]) && Vector3.Distance(character.GetPos(_isLocal: false), targetFishStandPoint) > Vector3.Distance(character.GetPos(_isLocal: false), FishingDefinition.FSM.GetFishShadowStandPoint(userDataNo)[i]))
			{
				targetFishStandPoint = FishingDefinition.FSM.GetFishShadowStandPoint(userDataNo)[i];
			}
		}
		actionState = AIActionState.MOVE;
		currentMoveTargetRemainTime = 5f;
	}
	private void OnDrawGizmos()
	{
		if (isEnable)
		{
			if (isClearBetweenToTarget)
			{
				Gizmos.DrawRay(boxCastAnchor.position, originToTargetDirection.normalized * targetDistance);
				Gizmos.DrawWireCube(boxCastAnchor.position + originToTargetDirection.normalized * targetDistance, Vector3.one * boxCastScale);
			}
			else if (isHitSearchBoxCast)
			{
				Gizmos.DrawRay(boxCastAnchor.position, lastSearchDirection * hit_BoxCast.distance);
				Gizmos.DrawWireCube(boxCastAnchor.position + lastSearchDirection * 1f, Vector3.one * boxCastScale);
			}
			else
			{
				Gizmos.DrawRay(boxCastAnchor.position, searchDirection * 1f);
				Gizmos.DrawWireCube(boxCastAnchor.position + searchDirection * 1f, Vector3.one * boxCastScale);
			}
			switch (userDataNo)
			{
			case 1:
				Gizmos.color = Color.red;
				break;
			case 2:
				Gizmos.color = Color.blue;
				break;
			case 3:
				Gizmos.color = Color.yellow;
				break;
			}
			Gizmos.DrawWireCube(targetFishStandPoint, new Vector3(0.2f, 0.2f, 0.2f));
		}
	}
}
