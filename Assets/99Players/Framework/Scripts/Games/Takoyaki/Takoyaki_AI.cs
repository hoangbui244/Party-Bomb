using System.Collections.Generic;
using UnityEngine;
public class Takoyaki_AI : MonoBehaviour
{
	private enum AIState
	{
		CursorMove,
		TakoMachineInteract,
		TakoBallBoxed
	}
	private Takoyaki_Player player;
	private Takoyaki_TakoyakiMachine takoMachine;
	private Takoyaki_Define.AiStrength aiStrength;
	private float cursorDelaySpeed;
	private readonly float[] SELECT_CURSOR_DELAY_SPEED = new float[3];
	private int checkHoleNum;
	private readonly int[] CHECK_HOLE_NUM = new int[3]
	{
		6,
		12,
		18
	};
	private float checkTakoyakiStatusPer;
	private readonly float[] CHECK_TAKOYAKI_STATUS_PER = new float[3]
	{
		30f,
		15f,
		10f
	};
	private const int TAKOYAKI_MACHINE_HOLE_NUM = 18;
	private AIState nowState;
	private bool duringAction;
	private List<int> targetHoleNoList = new List<int>();
	private int nowInteractHoleNo;
	private int[] checkTakoBallCnt;
	public void Init(Takoyaki_Player _player, Takoyaki_TakoyakiMachine _takoMachine)
	{
		player = _player;
		takoMachine = _takoMachine;
		aiStrength = (Takoyaki_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		cursorDelaySpeed = SELECT_CURSOR_DELAY_SPEED[(int)aiStrength];
		checkHoleNum = CHECK_HOLE_NUM[(int)aiStrength];
		checkTakoyakiStatusPer = CHECK_TAKOYAKI_STATUS_PER[(int)aiStrength];
		checkTakoBallCnt = new int[checkHoleNum];
		targetHoleNoList.Clear();
		if (aiStrength == Takoyaki_Define.AiStrength.WEAK)
		{
			while (targetHoleNoList.Count != checkHoleNum)
			{
				int item = UnityEngine.Random.Range(0, 18);
				if (!targetHoleNoList.Contains(item))
				{
					targetHoleNoList.Add(item);
				}
			}
		}
		else if (aiStrength == Takoyaki_Define.AiStrength.COMMON)
		{
			int num = UnityEngine.Random.Range(0, 18 - checkHoleNum);
			for (int i = 0; i < checkHoleNum; i++)
			{
				targetHoleNoList.Add(num);
				num++;
			}
		}
		else
		{
			for (int j = 0; j < checkHoleNum; j++)
			{
				targetHoleNoList.Add(j);
			}
		}
		nowInteractHoleNo = 0;
		nowState = AIState.CursorMove;
	}
	public void UpdateMethod()
	{
		if (!duringAction)
		{
			duringAction = true;
			switch (nowState)
			{
			case AIState.CursorMove:
				takoMachine.StartMoveCursor_AI(targetHoleNoList[nowInteractHoleNo], cursorDelaySpeed, EndCursorMove);
				break;
			case AIState.TakoMachineInteract:
				takoMachine.TakoyakiProcessAdvance_AI(EndInteract);
				break;
			case AIState.TakoBallBoxed:
				takoMachine.TakoBallBoxed_AI(EndTakoBallBoxed);
				break;
			}
		}
	}
	private void EndCursorMove()
	{
		duringAction = false;
		nowState = AIState.TakoMachineInteract;
	}
	private void EndInteract()
	{
		duringAction = false;
		switch (takoMachine.GetSelectTakoBallProcess())
		{
		case Takoyaki_TakoyakiMachine.TakoyakiProcessType.INGREDIENTS_PUT_IN:
			if (aiStrength == Takoyaki_Define.AiStrength.WEAK)
			{
				nowInteractHoleNo = UnityEngine.Random.Range(0, targetHoleNoList.Count);
			}
			else
			{
				nowInteractHoleNo++;
				if (nowInteractHoleNo == targetHoleNoList.Count)
				{
					nowInteractHoleNo = 0;
				}
			}
			nowState = AIState.CursorMove;
			break;
		case Takoyaki_TakoyakiMachine.TakoyakiProcessType.BALL_SPIN:
			if (takoMachine.GetSelectTakoBall().IsBakingTakoBallHalf)
			{
				if (aiStrength == Takoyaki_Define.AiStrength.WEAK)
				{
					nowInteractHoleNo = UnityEngine.Random.Range(0, targetHoleNoList.Count);
				}
				else
				{
					nowInteractHoleNo++;
					if (nowInteractHoleNo == targetHoleNoList.Count)
					{
						nowInteractHoleNo = 0;
					}
				}
				nowState = AIState.CursorMove;
				break;
			}
			if (checkTakoBallCnt[nowInteractHoleNo] == 0)
			{
				checkTakoBallCnt[nowInteractHoleNo]++;
				if (checkTakoyakiStatusPer > (float)UnityEngine.Random.Range(0, 100))
				{
					nowState = AIState.TakoBallBoxed;
					checkTakoBallCnt[nowInteractHoleNo] = 0;
				}
				break;
			}
			if (checkTakoBallCnt[nowInteractHoleNo] == 1 && takoMachine.GetSelectTakoBall().IsBakingPlaneBaked())
			{
				checkTakoBallCnt[nowInteractHoleNo]++;
				if (checkTakoyakiStatusPer < (float)UnityEngine.Random.Range(0, 100))
				{
					nowState = AIState.TakoBallBoxed;
					checkTakoBallCnt[nowInteractHoleNo] = 0;
				}
				break;
			}
			if (checkTakoBallCnt[nowInteractHoleNo] == 2 && takoMachine.GetSelectTakoBall().IsOverBakeAlert)
			{
				checkTakoBallCnt[nowInteractHoleNo]++;
				if (checkTakoyakiStatusPer < (float)UnityEngine.Random.Range(0, 100))
				{
					nowState = AIState.TakoBallBoxed;
					checkTakoBallCnt[nowInteractHoleNo] = 0;
				}
				break;
			}
			if (takoMachine.GetSelectTakoBall().GetTakoBallBakeStatus() == Takoyaki_Define.TakoBallBakeStatus.OverBake)
			{
				nowState = AIState.TakoBallBoxed;
				checkTakoBallCnt[nowInteractHoleNo] = 0;
				break;
			}
			if (aiStrength == Takoyaki_Define.AiStrength.WEAK)
			{
				nowInteractHoleNo = UnityEngine.Random.Range(0, targetHoleNoList.Count);
			}
			else
			{
				nowInteractHoleNo++;
				if (nowInteractHoleNo == targetHoleNoList.Count)
				{
					nowInteractHoleNo = 0;
				}
			}
			nowState = AIState.CursorMove;
			break;
		}
	}
	private void EndTakoBallBoxed()
	{
		duringAction = false;
		if (aiStrength == Takoyaki_Define.AiStrength.WEAK)
		{
			nowInteractHoleNo = UnityEngine.Random.Range(0, targetHoleNoList.Count);
		}
		else
		{
			nowInteractHoleNo++;
			if (nowInteractHoleNo == targetHoleNoList.Count)
			{
				nowInteractHoleNo = 0;
			}
		}
		nowState = AIState.CursorMove;
	}
}
