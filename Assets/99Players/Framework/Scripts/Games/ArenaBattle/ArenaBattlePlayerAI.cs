using UnityEngine;
public class ArenaBattlePlayerAI : MonoBehaviour
{
	public enum State
	{
		DEFAULT,
		SIDE_MOVE
	}
	public enum ThinkType
	{
		DISTANCE,
		HP,
		MAX
	}
	[SerializeField]
	[Header("本体")]
	private ArenaBattlePlayer rootPlayer;
	private GameObject targetPlayerObj;
	private ArenaBattlePlayer targetPlayer;
	private Vector3 calcPos;
	private float jumpTimeInterval;
	private float dodgeTimeInterval;
	private float magicTimeInterval;
	private float attackTimeInterval;
	private float thinkChangeTime;
	private State currentState;
	private ThinkType currentThinkType;
	private float stateTime;
	private Vector3 moveDir;
	private int checkProb;
	public void Init()
	{
		currentState = State.DEFAULT;
		if (CalcManager.GetHalfProbability())
		{
			magicTimeInterval = UnityEngine.Random.Range(0.5f, 3f);
		}
		else
		{
			magicTimeInterval = UnityEngine.Random.Range(2f, 5f);
		}
		thinkChangeTime = UnityEngine.Random.Range(3f, 6f);
		UnityEngine.Debug.Log("magic:" + magicTimeInterval.ToString());
	}
	public Vector3 UpdateMove()
	{
		switch (currentThinkType)
		{
		case ThinkType.DISTANCE:
			targetPlayer = SingletonCustom<ArenaBattlePlayerManager>.Instance.GetOpponentCharacter(rootPlayer.PlayerIdx);
			break;
		case ThinkType.HP:
			targetPlayer = SingletonCustom<ArenaBattlePlayerManager>.Instance.GetOpponentCharacterAtHp(rootPlayer.PlayerIdx);
			break;
		}
		if (targetPlayer == null)
		{
			return Vector3.zero;
		}
		targetPlayerObj = targetPlayer.gameObject;
		calcPos = targetPlayerObj.transform.position;
		switch (currentState)
		{
		case State.DEFAULT:
			if (CalcManager.Length(rootPlayer.transform.position, SingletonCustom<ArenaBattleFieldManager>.Instance.AnchorCenter.position) >= CalcManager.Length(targetPlayerObj.transform.position, SingletonCustom<ArenaBattleFieldManager>.Instance.AnchorCenter.position) && UnityEngine.Random.Range(0, 100) <= 10)
			{
				currentState = State.SIDE_MOVE;
				stateTime = UnityEngine.Random.Range(0.4f, 0.6f);
				moveDir = (Quaternion.Euler(0f, 50f, 0f) * (SingletonCustom<ArenaBattleFieldManager>.Instance.AnchorCenter.position - base.transform.position)).normalized;
			}
			break;
		case State.SIDE_MOVE:
			stateTime -= Time.deltaTime;
			if (stateTime >= 0f)
			{
				return moveDir;
			}
			SetCurrentState(State.DEFAULT);
			break;
		}
		return (calcPos - base.transform.position).normalized;
	}
	private void SetCurrentState(State _state)
	{
		currentState = _state;
	}
	public bool IsDodge()
	{
		checkProb = 0;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			checkProb = 1;
			break;
		case 1:
			checkProb = 2;
			break;
		case 2:
			checkProb = 3;
			break;
		}
		if (dodgeTimeInterval <= 0f && magicTimeInterval <= 0f && magicTimeInterval <= 0f && UnityEngine.Random.Range(0, 100) <= checkProb)
		{
			dodgeTimeInterval = UnityEngine.Random.Range(1f, 3f);
			return true;
		}
		return false;
	}
	public bool IsJump()
	{
		checkProb = 0;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			checkProb = 1;
			break;
		case 1:
			checkProb = 2;
			break;
		case 2:
			checkProb = 3;
			break;
		}
		if (jumpTimeInterval <= 0f && magicTimeInterval <= 0f && magicTimeInterval <= 0f && UnityEngine.Random.Range(0, 100) <= checkProb)
		{
			jumpTimeInterval = UnityEngine.Random.Range(3f, 7f);
			return true;
		}
		return false;
	}
	public bool IsMagic()
	{
		if (targetPlayerObj == null || targetPlayer == null)
		{
			return false;
		}
		if (Vector3.Angle(base.transform.forward, targetPlayerObj.transform.position - base.transform.position) <= 30f && CalcManager.Length(targetPlayerObj, base.gameObject) > 0.65f && magicTimeInterval <= 0f)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4 + (rootPlayer.PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)];
			if (num == 1 || num == 5)
			{
				magicTimeInterval = UnityEngine.Random.Range(2f, 5f);
			}
			else
			{
				magicTimeInterval = UnityEngine.Random.Range(1f, 2.5f);
			}
			return true;
		}
		return false;
	}
	public bool IsSwordStage1()
	{
		if (targetPlayerObj == null || targetPlayer == null)
		{
			return false;
		}
		if (Vector3.Angle(base.transform.forward, targetPlayerObj.transform.position - base.transform.position) <= 30f && CalcManager.Length(targetPlayerObj, base.gameObject) <= 0.85f && attackTimeInterval <= 0f)
		{
			attackTimeInterval = UnityEngine.Random.Range(0.1f, 1f);
			return true;
		}
		return false;
	}
	public bool IsSwordStage2()
	{
		if (targetPlayerObj == null || targetPlayer == null)
		{
			return false;
		}
		if (Vector3.Angle(base.transform.forward, targetPlayerObj.transform.position - base.transform.position) <= 35f)
		{
			checkProb = 0;
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
			{
			case 0:
				checkProb = 5;
				break;
			case 1:
				checkProb = 30;
				break;
			case 2:
				checkProb = 50;
				break;
			}
			if (CalcManager.Length(targetPlayerObj, base.gameObject) <= 1.65f && UnityEngine.Random.Range(0, 100) <= checkProb)
			{
				attackTimeInterval = UnityEngine.Random.Range(0.1f, 1f);
				return true;
			}
		}
		return false;
	}
	public bool IsSwordStage3()
	{
		if (targetPlayerObj == null || targetPlayer == null)
		{
			return false;
		}
		checkProb = 0;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			checkProb = 0;
			break;
		case 1:
			checkProb = 10;
			break;
		case 2:
			checkProb = 25;
			break;
		}
		if (Vector3.Angle(base.transform.forward, targetPlayerObj.transform.position - base.transform.position) <= 30f && CalcManager.Length(targetPlayerObj, base.gameObject) <= 1.7f && UnityEngine.Random.Range(0, 100) <= checkProb)
		{
			attackTimeInterval = UnityEngine.Random.Range(0.1f, 1f);
			return true;
		}
		return false;
	}
	public bool IsGrab()
	{
		if (!(targetPlayerObj == null))
		{
			bool flag = targetPlayer == null;
		}
		return false;
	}
	public bool IsAssembleAtk()
	{
		checkProb = 0;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			checkProb = 20;
			break;
		case 1:
			checkProb = 15;
			break;
		case 2:
			checkProb = 10;
			break;
		}
		if (Time.frameCount % checkProb == 0)
		{
			return true;
		}
		return false;
	}
	public bool IsAssembleDef()
	{
		checkProb = 0;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			checkProb = 25;
			break;
		case 1:
			checkProb = 20;
			break;
		case 2:
			checkProb = 15;
			break;
		}
		if (Time.frameCount % checkProb == 0)
		{
			return true;
		}
		return false;
	}
	public void UpdateMethod()
	{
		dodgeTimeInterval = Mathf.Clamp(dodgeTimeInterval - Time.deltaTime, 0f, 10f);
		jumpTimeInterval = Mathf.Clamp(jumpTimeInterval - Time.deltaTime, 0f, 10f);
		magicTimeInterval = Mathf.Clamp(magicTimeInterval - Time.deltaTime, 0f, 3f);
		attackTimeInterval = Mathf.Clamp(attackTimeInterval - Time.deltaTime, 0f, 3f);
		thinkChangeTime = Mathf.Clamp(thinkChangeTime - Time.deltaTime, 0f, 6f);
		if (thinkChangeTime <= 0f)
		{
			if (UnityEngine.Random.Range(0, 100) <= 70)
			{
				currentThinkType = ThinkType.HP;
			}
			else
			{
				currentThinkType = ThinkType.DISTANCE;
			}
			thinkChangeTime = UnityEngine.Random.Range(3f, 6f);
		}
	}
}
