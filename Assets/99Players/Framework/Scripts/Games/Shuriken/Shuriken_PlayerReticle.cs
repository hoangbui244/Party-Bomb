using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_PlayerReticle : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("投げるアクション")]
	private Shuriken_ThrowAction throwAction;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_ReticleConfig config;
	[Header("デバッグ用UI")]
	[SerializeField]
	[Disable(true)]
	[DisplayName("レティクル座標")]
	private Vector2 reticlePosition;
	[SerializeField]
	[Disable(true)]
	[DisplayName("目標の的(固定)")]
	private Shuriken_BasicTarget currentBasicTarget;
	[SerializeField]
	[Disable(true)]
	[DisplayName("目標の的(移動)")]
	private Shuriken_MoveTarget currentMoveTarget;
	private int playerNo;
	private Shuriken_Definition.ControlUser controlUser;
	private Vector2 previousVector;
	private readonly List<Shuriken_BasicTarget> targets = new List<Shuriken_BasicTarget>(4);
	public void Initialize(int no)
	{
		playerNo = no;
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			reticlePosition = Shuriken_Definition.SinglePlayerReticleDefaultPositions[no];
			break;
		case 2:
			reticlePosition = Shuriken_Definition.TwoPlayersReticleDefaultPositions[no];
			break;
		case 3:
			reticlePosition = Shuriken_Definition.ThreePlayersReticleDefaultPositions[no];
			break;
		case 4:
			reticlePosition = Shuriken_Definition.FourPlayersReticleDefaultPositions[no];
			break;
		}
		controlUser = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlUsers[playerNo];
	}
	public void PostInitialize()
	{
		SingletonMonoBehaviour<Shuriken_UI>.Instance.UpdatePosition(reticlePosition, playerNo);
		if (playerNo >= SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlPlayerNum && !config.ShowCpuReticle)
		{
			SingletonMonoBehaviour<Shuriken_UI>.Instance.DisableReticle(playerNo);
		}
	}
	public void UpdateMethod()
	{
		switch (controlUser)
		{
		case Shuriken_Definition.ControlUser.Player1:
		case Shuriken_Definition.ControlUser.Player2:
		case Shuriken_Definition.ControlUser.Player3:
		case Shuriken_Definition.ControlUser.Player4:
			MoveReticle(controlUser);
			break;
		case Shuriken_Definition.ControlUser.Cpu1:
		case Shuriken_Definition.ControlUser.Cpu2:
		case Shuriken_Definition.ControlUser.Cpu3:
		case Shuriken_Definition.ControlUser.AutoPlay:
			MoveReticleForAI();
			break;
		}
	}
	public Ray ScreenPointToRay()
	{
		return SingletonMonoBehaviour<Shuriken_Camera>.Instance.FullHdScreenPointToRay(reticlePosition);
	}
	public void DiscardTargetIfNeeded()
	{
		if (playerNo >= SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlPlayerNum)
		{
			currentBasicTarget = null;
		}
	}
	public bool IsAimTarget()
	{
		if (currentBasicTarget != null)
		{
			Vector2 b = SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(currentBasicTarget.AimHint);
			return Vector2.Distance(reticlePosition, b) < config.AimStickDistanceThreshold;
		}
		if (currentMoveTarget != null)
		{
			Vector3 predictionPosition = currentMoveTarget.GetPredictionPosition(throwAction.GetTimeRequiredToReach(currentMoveTarget.AimHint));
			Vector3 v = SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(predictionPosition);
			return Vector2.Distance(reticlePosition, v) < config.AimStickDistanceThreshold;
		}
		return false;
	}
	private void MoveReticle(Shuriken_Definition.ControlUser controlUser)
	{
		Vector2 normalized = SingletonMonoBehaviour<Shuriken_Input>.Instance.GetLStickVector(controlUser).normalized;
		Vector2 vector = normalized * config.ReticleSpeed * Time.deltaTime;
		reticlePosition += vector;
		Vector2 minReticlePosition = config.MinReticlePosition;
		Vector2 maxReticlePosition = config.MaxReticlePosition;
		reticlePosition.x = Mathf.Clamp(reticlePosition.x, minReticlePosition.x, maxReticlePosition.x);
		reticlePosition.y = Mathf.Clamp(reticlePosition.y, minReticlePosition.y, maxReticlePosition.y);
		SingletonMonoBehaviour<Shuriken_UI>.Instance.UpdatePosition(reticlePosition, playerNo);
	}
	private void MoveReticleForAI()
	{
		int takeTargetsCount = config.GetTakeTargetsCount(SingletonMonoBehaviour<Shuriken_GameMain>.Instance.AIStrength);
		targets.Clear();
		if (currentBasicTarget != null && currentBasicTarget.gameObject.activeSelf)
		{
			targets.Add(currentBasicTarget);
		}
		if (currentMoveTarget != null && currentMoveTarget.gameObject.activeSelf)
		{
			targets.Add(currentMoveTarget);
		}
		SingletonMonoBehaviour<Shuriken_TargetGenerator>.Instance.GetCloserTargets(reticlePosition, takeTargetsCount, targets);
		Shuriken_BasicTarget shuriken_BasicTarget = targets[Random.Range(0, targets.Count)];
		if (currentBasicTarget != null)
		{
			if (targets.Contains(currentBasicTarget))
			{
				shuriken_BasicTarget = currentBasicTarget;
			}
			else
			{
				currentBasicTarget.ReleaseTargeting();
			}
		}
		if (currentMoveTarget != null)
		{
			if (targets.Contains(currentMoveTarget))
			{
				shuriken_BasicTarget = currentMoveTarget;
			}
			else
			{
				currentMoveTarget.ReleaseTargeting();
			}
		}
		shuriken_BasicTarget.Targeting();
		Shuriken_MoveTarget shuriken_MoveTarget = shuriken_BasicTarget as Shuriken_MoveTarget;
		if ((object)shuriken_MoveTarget != null)
		{
			currentMoveTarget = shuriken_MoveTarget;
			currentBasicTarget = null;
			AimMoveTarget();
		}
		else
		{
			currentBasicTarget = shuriken_BasicTarget;
			currentMoveTarget = null;
			AimBasicTarget();
		}
	}
	private void AimBasicTarget()
	{
		if (!currentBasicTarget.IsActive)
		{
			currentBasicTarget = null;
			MoveReticleForAI();
			return;
		}
		if (!currentBasicTarget.gameObject.activeSelf)
		{
			currentBasicTarget = null;
			MoveReticleForAI();
			return;
		}
		Vector2 normalized = ((Vector2)SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(currentBasicTarget.AimHint) - reticlePosition).normalized;
		Vector2 a = MoveTowardsVector(normalized);
		Vector2 vector = config.ReticleSpeed * Time.deltaTime * a;
		reticlePosition += vector;
		Vector2 minReticlePosition = config.MinReticlePosition;
		Vector2 maxReticlePosition = config.MaxReticlePosition;
		reticlePosition.x = Mathf.Clamp(reticlePosition.x, minReticlePosition.x, maxReticlePosition.x);
		reticlePosition.y = Mathf.Clamp(reticlePosition.y, minReticlePosition.y, maxReticlePosition.y);
		SingletonMonoBehaviour<Shuriken_UI>.Instance.UpdatePosition(reticlePosition, playerNo);
	}
	private void AimMoveTarget()
	{
		if (!currentMoveTarget.IsActive)
		{
			currentMoveTarget = null;
			MoveReticleForAI();
			return;
		}
		Vector3 predictionPosition = currentMoveTarget.GetPredictionPosition(throwAction.GetTimeRequiredToReach(currentMoveTarget.AimHint));
		Vector2 vector = SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(predictionPosition);
		Vector2 normalized = (vector - reticlePosition).normalized;
		Vector2 a = MoveTowardsVector(normalized);
		Vector2 vector2 = config.ReticleSpeed * Time.deltaTime * a;
		reticlePosition += vector2;
		if (Vector2.Distance(reticlePosition, vector) < config.AimStickDistanceThreshold)
		{
			reticlePosition = vector;
		}
		Vector2 minReticlePosition = config.MinReticlePosition;
		Vector2 maxReticlePosition = config.MaxReticlePosition;
		reticlePosition.x = Mathf.Clamp(reticlePosition.x, minReticlePosition.x, maxReticlePosition.x);
		reticlePosition.y = Mathf.Clamp(reticlePosition.y, minReticlePosition.y, maxReticlePosition.y);
		SingletonMonoBehaviour<Shuriken_UI>.Instance.UpdatePosition(reticlePosition, playerNo);
	}
	private Vector2 MoveTowardsVector(Vector2 vector)
	{
		float aimVectorDeltaSpeed = config.GetAimVectorDeltaSpeed(SingletonMonoBehaviour<Shuriken_GameMain>.Instance.AIStrength);
		return previousVector = Vector2.MoveTowards(previousVector, vector, Time.deltaTime * aimVectorDeltaSpeed);
	}
}
