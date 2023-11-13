using System.Collections.Generic;
using UnityEngine;
public class ShavedIce_AI : MonoBehaviour
{
	private enum AIState
	{
		Idle,
		MoveWait,
		MoveCup
	}
	[SerializeField]
	[Header("プレイヤ\u30fc処理")]
	private ShavedIce_Player player;
	private readonly float[] MOVE_CUP_RANDOM_VALUE = new float[3]
	{
		0.09f,
		0.06f,
		0.03f
	};
	private ShavedIce_Define.AiStrength aiStrength;
	private AIState currentAIState;
	private float duringMoveStartTime;
	private List<float> movePointList = new List<float>();
	private List<float> moveTimeList = new List<float>();
	private bool isMoveFinish;
	private int iceFXMoveXPosId;
	private float aiAdjustCupTime;
	private float aiAdjustPoint;
	private Vector3 topVec;
	private Transform cup;
	private float moveCupSpeed;
	public void Init()
	{
		aiStrength = (ShavedIce_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		currentAIState = AIState.Idle;
		cup = player.GetCup().transform;
		moveCupSpeed = player.GET_MOVE_CUP_SPEED;
	}
	public void UpdateMethod()
	{
		switch (currentAIState)
		{
		case AIState.Idle:
			IdleProcess();
			break;
		case AIState.MoveCup:
			CupMoveProcess();
			break;
		}
	}
	private void IdleProcess()
	{
		if (player.GetCup().IsStartHitParticle)
		{
			if (duringMoveStartTime > ShavedIce_Define.ICE_FX_MOVE_START_TIME)
			{
				currentAIState = AIState.MoveCup;
				duringMoveStartTime = 0f;
			}
			else
			{
				duringMoveStartTime += Time.deltaTime;
			}
		}
	}
	private void CupMoveProcess()
	{
		if (movePointList.Count <= iceFXMoveXPosId)
		{
			return;
		}
		List<float> list = moveTimeList;
		int index = iceFXMoveXPosId;
		list[index] -= Time.deltaTime;
		if (moveTimeList[iceFXMoveXPosId] <= 0f)
		{
			moveTimeList[iceFXMoveXPosId] = 0f;
			isMoveFinish = false;
			iceFXMoveXPosId++;
			if (movePointList.Count <= iceFXMoveXPosId)
			{
				return;
			}
		}
		if (!isMoveFinish && AI_MoveCup(movePointList[iceFXMoveXPosId], _adjustCup: true, 5f))
		{
			isMoveFinish = true;
		}
	}
	private bool AI_MoveCup(float _movePosX, bool _adjustCup, float _adjustTime)
	{
		if (_adjustCup)
		{
			aiAdjustCupTime += Time.deltaTime;
			if (aiAdjustCupTime > _adjustTime)
			{
				topVec = player.IOM.GetNowTopIceVec();
				aiAdjustPoint = topVec.x;
				aiAdjustCupTime = 0f;
			}
			if (cup.localPosition.x <= _movePosX)
			{
				cup.SetLocalPositionX(cup.localPosition.x + Time.deltaTime * moveCupSpeed);
				if (cup.localPosition.x + aiAdjustPoint > _movePosX)
				{
					return true;
				}
			}
			else
			{
				cup.SetLocalPositionX(cup.localPosition.x - Time.deltaTime * moveCupSpeed);
				if (cup.localPosition.x + aiAdjustPoint < _movePosX)
				{
					return true;
				}
			}
		}
		else if (cup.localPosition.x <= _movePosX)
		{
			cup.SetLocalPositionX(cup.localPosition.x + Time.deltaTime * moveCupSpeed);
			if (cup.localPosition.x > _movePosX)
			{
				cup.SetLocalPositionX(_movePosX);
				return true;
			}
		}
		else
		{
			cup.SetLocalPositionX(cup.localPosition.x - Time.deltaTime * moveCupSpeed);
			if (cup.localPosition.x < _movePosX)
			{
				cup.SetLocalPositionX(_movePosX);
				return true;
			}
		}
		return false;
	}
	public void AddCupMovePoint(float _movePoint)
	{
		if (movePointList.Count == 0)
		{
			float num = Vector3.Distance(new Vector3(0f, 0f, 0f), new Vector3(_movePoint, 0f, 0f));
			moveTimeList.Add(num / moveCupSpeed);
		}
		else
		{
			float num2 = Vector3.Distance(new Vector3(movePointList[movePointList.Count - 1], 0f, 0f), new Vector3(_movePoint, 0f, 0f));
			moveTimeList.Add(num2 / moveCupSpeed);
		}
		movePointList.Add(_movePoint + UnityEngine.Random.Range(0f - MOVE_CUP_RANDOM_VALUE[(int)aiStrength], MOVE_CUP_RANDOM_VALUE[(int)aiStrength]));
	}
}
