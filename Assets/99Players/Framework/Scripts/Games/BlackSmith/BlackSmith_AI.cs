using SaveDataDefine;
using System.Collections.Generic;
using UnityEngine;
public class BlackSmith_AI : MonoBehaviour
{
	private struct InputList
	{
		public BlackSmith_PlayerManager.EvaluationType evaluationType;
		public bool isInput;
		public float inputTiming;
	}
	private BlackSmith_Player player;
	private int aiStrength;
	private int originAIStrength;
	private Dictionary<int, InputList> inputEvaluationList = new Dictionary<int, InputList>();
	private InputList inputList;
	private float[] arrayProbability;
	private float probability;
	private bool inputBad;
	private bool inputBetterEvaluation;
	private float changeStrengthDelayTime;
	public void Init(BlackSmith_Player _player)
	{
		player = _player;
		this.aiStrength = ((player.GetUserType() != SingletonCustom<BlackSmith_PlayerManager>.Instance.GetChangeAIStrengthUserType()) ? SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength : (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength - 1));
		originAIStrength = this.aiStrength;
		SystemData.AiStrength aiStrength = (SystemData.AiStrength)this.aiStrength;
		if ((uint)(aiStrength - 1) <= 1u && SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
		{
			this.aiStrength = 0;
			int idx = (int)(player.GetUserType() - 4);
			changeStrengthDelayTime = SingletonCustom<BlackSmith_PlayerManager>.Instance.GetChangeAIStrengthDelayTime(idx);
		}
	}
	public void SetInputEvaluationList()
	{
		inputEvaluationList.Clear();
		for (int i = 0; i < player.GetHammerStrileInputCnt(); i++)
		{
			arrayProbability = BlackSmith_Define.CPU_INPUT_PERFECT_PROBABILITY[aiStrength];
			probability = UnityEngine.Random.Range(arrayProbability[0], arrayProbability[1]);
			if (UnityEngine.Random.Range(0f, 1f) < probability)
			{
				inputList.evaluationType = BlackSmith_PlayerManager.EvaluationType.Perfect;
				inputList.isInput = false;
				inputList.inputTiming = SingletonCustom<BlackSmith_UIManager>.Instance.GetInputTiming(player.GetPlayerNo(), i, BlackSmith_PlayerManager.EvaluationType.Perfect);
				inputEvaluationList.Add(i, inputList);
				continue;
			}
			arrayProbability = BlackSmith_Define.CPU_INPUT_NICE_PROBABILITY[aiStrength];
			probability = UnityEngine.Random.Range(arrayProbability[0], arrayProbability[1]);
			if (UnityEngine.Random.Range(0f, 1f) < probability)
			{
				inputList.evaluationType = BlackSmith_PlayerManager.EvaluationType.Nice;
				inputList.isInput = false;
				inputList.inputTiming = SingletonCustom<BlackSmith_UIManager>.Instance.GetInputTiming(player.GetPlayerNo(), i, BlackSmith_PlayerManager.EvaluationType.Nice);
				inputEvaluationList.Add(i, inputList);
			}
			else
			{
				inputList.evaluationType = BlackSmith_PlayerManager.EvaluationType.Good;
				inputList.isInput = false;
				inputList.inputTiming = SingletonCustom<BlackSmith_UIManager>.Instance.GetInputTiming(player.GetPlayerNo(), i, BlackSmith_PlayerManager.EvaluationType.Good);
				inputEvaluationList.Add(i, inputList);
			}
		}
		arrayProbability = BlackSmith_Define.CPU_INPUT_BAD_PROBABILITY[aiStrength];
		probability = UnityEngine.Random.Range(arrayProbability[0], arrayProbability[1]);
		inputBad = (UnityEngine.Random.Range(0f, 1f) < probability);
		arrayProbability = BlackSmith_Define.CPU_INPUT_BETTER_EVALUATION_PROBABILITY[aiStrength];
		probability = UnityEngine.Random.Range(arrayProbability[0], arrayProbability[1]);
		inputBetterEvaluation = (UnityEngine.Random.Range(0f, 1f) < probability);
	}
	public void UpdateMethod()
	{
		if (aiStrength != originAIStrength)
		{
			changeStrengthDelayTime -= Time.deltaTime;
			if (changeStrengthDelayTime < 0f)
			{
				aiStrength = originAIStrength;
			}
		}
		int barMoveDir = SingletonCustom<BlackSmith_UIManager>.Instance.GetBarMoveDir(player.GetPlayerNo());
		int barIdx = SingletonCustom<BlackSmith_UIManager>.Instance.GetBarIdx(player.GetPlayerNo());
		switch (barMoveDir)
		{
		case 1:
			if (inputBetterEvaluation)
			{
				for (int i = 0; i < inputEvaluationList.Count; i++)
				{
					if (inputEvaluationList[i].isInput || i >= barIdx)
					{
						continue;
					}
					for (int j = barIdx; j < inputEvaluationList.Count; j++)
					{
						if (inputEvaluationList[i].evaluationType > inputEvaluationList[j].evaluationType)
						{
							inputList.isInput = true;
							inputEvaluationList[i] = inputList;
							inputList.evaluationType = inputEvaluationList[i].evaluationType;
							inputList.isInput = false;
							inputList.inputTiming = inputEvaluationList[i].inputTiming;
							inputEvaluationList[j] = inputList;
							break;
						}
					}
				}
			}
			for (int k = 0; k < inputEvaluationList.Count; k++)
			{
				if (k >= barIdx && !inputEvaluationList[k].isInput && SingletonCustom<BlackSmith_UIManager>.Instance.IsInputTiming(player.GetPlayerNo(), k, inputEvaluationList[k].evaluationType, inputEvaluationList[k].inputTiming))
				{
					player.HammerStrike();
					inputList.isInput = true;
					inputEvaluationList[k] = inputList;
					return;
				}
			}
			if (inputBad && barIdx == inputEvaluationList.Count - 1 && !SingletonCustom<BlackSmith_UIManager>.Instance.IsCanGoodInput(player.GetPlayerNo()) && player.GetHammerStrileInputCnt() > 0)
			{
				player.HammerStrike();
			}
			break;
		case -1:
			if (inputBetterEvaluation)
			{
				for (int num = inputEvaluationList.Count - 1; num >= 0; num--)
				{
					if (!inputEvaluationList[num].isInput && num > barIdx)
					{
						for (int num2 = barIdx; num2 >= 0; num2--)
						{
							if (inputEvaluationList[num].evaluationType > inputEvaluationList[num2].evaluationType)
							{
								inputList.isInput = true;
								inputEvaluationList[num] = inputList;
								inputList.evaluationType = inputEvaluationList[num].evaluationType;
								inputList.isInput = false;
								inputList.inputTiming = inputEvaluationList[num].inputTiming;
								inputEvaluationList[num2] = inputList;
								break;
							}
						}
					}
				}
			}
			for (int num3 = inputEvaluationList.Count - 1; num3 >= 0; num3--)
			{
				if (num3 <= barIdx && !inputEvaluationList[num3].isInput && SingletonCustom<BlackSmith_UIManager>.Instance.IsInputTiming(player.GetPlayerNo(), num3, inputEvaluationList[num3].evaluationType, inputEvaluationList[num3].inputTiming))
				{
					player.HammerStrike();
					inputList.isInput = true;
					inputEvaluationList[num3] = inputList;
					return;
				}
			}
			if (inputBad && barIdx == 0 && !SingletonCustom<BlackSmith_UIManager>.Instance.IsCanGoodInput(player.GetPlayerNo()) && player.GetHammerStrileInputCnt() > 0)
			{
				player.HammerStrike();
			}
			break;
		}
	}
}
