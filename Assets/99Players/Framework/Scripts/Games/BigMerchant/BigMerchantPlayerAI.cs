using SaveDataDefine;
using UnityEngine;
using UnityEngine.AI;
public class BigMerchantPlayerAI : MonoBehaviour
{
	public enum State
	{
		Check,
		SearchSupply,
		MoveSupply,
		SearchCustomer,
		MoveCustomer,
		WaitCustomer,
		ReturnSupply
	}
	[SerializeField]
	[Header("エ\u30fcジェント")]
	private NavMeshAgent agent;
	private SystemData.AiStrength aiStrength = SystemData.AiStrength.NORAML;
	private FireworksPlayer currentPlayer;
	private FireworksSlatbox currentTargetSupply;
	private BigMerchantCustomer currentTargetCustomer;
	private State currentState;
	private Vector3 prevForce;
	private Vector3 waitPoint;
	private int waitPointIdx;
	private bool isNear;
	public void Init(FireworksPlayer _player)
	{
		currentPlayer = _player;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting();
		agent.updatePosition = false;
		agent.updateRotation = false;
		switch (aiStrength)
		{
		case SystemData.AiStrength.WEAK:
			agent.speed = 1.85f;
			break;
		case SystemData.AiStrength.NORAML:
			agent.speed = 2.05f;
			isNear = true;
			break;
		case SystemData.AiStrength.STRONG:
			agent.speed = 2.35f;
			isNear = true;
			break;
		}
		waitPointIdx = UnityEngine.Random.Range(0, 3);
	}
	public void DoThink()
	{
		switch (currentState)
		{
		case State.Check:
			if (currentPlayer.HaveBallCount < FireworksPlayer.HAVE_ITEM_MAX)
			{
				SetState(State.SearchSupply);
			}
			else
			{
				SetState(State.SearchCustomer);
			}
			break;
		case State.SearchSupply:
			currentTargetSupply = SingletonCustom<BigMerchantSupplyManager>.Instance.GetNeedSupplyPos(base.transform.position, currentPlayer.ArrayBall, isNear);
			if (currentTargetSupply != null)
			{
				SetState(State.MoveSupply);
				agent.SetDestination(currentTargetSupply.transform.position);
			}
			break;
		case State.MoveSupply:
			if (currentPlayer.HaveBallCount > 0)
			{
				currentTargetCustomer = SingletonCustom<BigMerchantCustomerManager>.Instance.GetCustomer(base.transform.position, currentPlayer.ArrayBall, isNear);
				if (currentTargetCustomer != null && (currentTargetCustomer.transform.position - base.transform.position).sqrMagnitude < (currentTargetSupply.transform.position - base.transform.position).sqrMagnitude)
				{
					SetState(State.MoveCustomer);
					agent.SetDestination(currentTargetCustomer.transform.position);
				}
			}
			break;
		case State.SearchCustomer:
			currentTargetCustomer = SingletonCustom<BigMerchantCustomerManager>.Instance.GetCustomer(base.transform.position, currentPlayer.ArrayBall, isNear);
			if (currentTargetCustomer != null)
			{
				SetState(State.MoveCustomer);
				agent.SetDestination(currentTargetCustomer.transform.position);
				break;
			}
			if (currentPlayer.HaveBallCount < FireworksPlayer.HAVE_ITEM_MAX)
			{
				SetState(State.SearchSupply);
				break;
			}
			SetState(State.WaitCustomer);
			waitPoint = SingletonCustom<BigMerchantCustomerManager>.Instance.GetWaitPoint(waitPointIdx);
			agent.SetDestination(waitPoint);
			waitPointIdx = (waitPointIdx + 1) % 3;
			break;
		case State.MoveCustomer:
			if (!currentTargetCustomer.IsMatchList(currentPlayer.ArrayBall))
			{
				SetState(State.Check);
			}
			break;
		case State.WaitCustomer:
			currentTargetCustomer = SingletonCustom<BigMerchantCustomerManager>.Instance.GetCustomer(base.transform.position, currentPlayer.ArrayBall, isNear);
			if (currentTargetCustomer != null)
			{
				SetState(State.MoveCustomer);
				agent.SetDestination(currentTargetCustomer.transform.position);
				break;
			}
			currentTargetSupply = SingletonCustom<BigMerchantSupplyManager>.Instance.GetReturnPos(base.transform.position, currentPlayer.ArrayBall, isNear);
			if (currentTargetSupply != null)
			{
				SetState(State.ReturnSupply);
				agent.SetDestination(currentTargetSupply.transform.position);
			}
			break;
		case State.ReturnSupply:
			if (currentPlayer.HaveBallCount < FireworksPlayer.HAVE_ITEM_MAX)
			{
				SetState(State.Check);
			}
			break;
		}
	}
	public bool IsPathPending()
	{
		return agent.pathPending;
	}
	public Vector3 UpdateMoveForce()
	{
		if (agent.path.corners.Length != 0)
		{
			prevForce = (agent.path.corners[0] - base.transform.position).normalized;
			return prevForce;
		}
		return prevForce;
	}
	public bool IsLift()
	{
		if (currentTargetSupply == null)
		{
			return false;
		}
		if (currentState == State.MoveSupply)
		{
			return (currentPlayer.transform.position - currentTargetSupply.transform.position).sqrMagnitude <= 0.25f;
		}
		return false;
	}
	public void Lift()
	{
		SetState(State.Check);
	}
	public bool IsReturn()
	{
		if (currentTargetSupply == null)
		{
			return false;
		}
		if (currentState == State.ReturnSupply)
		{
			return (currentPlayer.transform.position - currentTargetSupply.transform.position).sqrMagnitude <= 0.25f;
		}
		return false;
	}
	public void Set()
	{
		if (currentPlayer.HaveBallCount > 0)
		{
			SetState(State.SearchCustomer);
		}
		else
		{
			SetState(State.SearchSupply);
		}
	}
	public void SetEnd()
	{
	}
	public void UpdateManualAgent(Vector3 _pos)
	{
		agent.SetDestination(_pos);
	}
	private void SetState(State _state)
	{
		currentState = _state;
	}
}
