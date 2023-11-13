using System;
using System.Collections.Generic;
using UnityEngine;
public class MakingPotion_PlayerManager : SingletonCustom<MakingPotion_PlayerManager>
{
	public const float ONE_CREATE_TIME = 15f;
	public const float SUGAR_SELECT_TIME = 3f;
	[SerializeField]
	private MakingPotion_PlayerScript[] players;
	[SerializeField]
	private Material[] machineBodyMaterials_Opaque;
	[SerializeField]
	private Material[] machineBodyMaterials_Transparent;
	[SerializeField]
	private Material[] dropSugarEffectMaterials;
	[SerializeField]
	private Material[] cauldronMaterials;
	[SerializeField]
	private Color[] threadColors;
	[SerializeField]
	private Color[] cottonColors;
	private bool[] isReds = new bool[4];
	private bool[] isYellows = new bool[4];
	private bool[] isGreens = new bool[4];
	private bool[] isBlues = new bool[4];
	private int[] colorTypeNo = new int[4];
	private List<MakingPotion_PlayerScript.SugarColorType>[] ctrlColorLists = new List<MakingPotion_PlayerScript.SugarColorType>[4]
	{
		new List<MakingPotion_PlayerScript.SugarColorType>(),
		new List<MakingPotion_PlayerScript.SugarColorType>(),
		new List<MakingPotion_PlayerScript.SugarColorType>(),
		new List<MakingPotion_PlayerScript.SugarColorType>()
	};
	private MakingPotion_TargetManager.TargetData targetData;
	private float createTimer;
	private int createCount;
	private int createSpeedCount;
	private int createSugarCount;
	private int createTurnCount;
	private bool isNowCreate;
	private bool isSugarForceEndWait;
	private float sugarForceEndTimer;
	private float sugarForceEndSecond;
	private const float AI_UPDATE_INTERVAL_WEAK = 0.8f;
	private const float AI_UPDATE_INTERVAL_NORMAL = 0.5f;
	private const float AI_UPDATE_INTERVAL_STRONG = 0.3f;
	private const float AI_INTERVAL_RANDOM = 0.2f;
	private float aiUpdateInterval;
	private float[] aiUpdateTimes;
	public int[] ColorTypeNo => colorTypeNo;
	public MakingPotion_TargetManager.TargetData TargetData => targetData;
	public float TargetNowRotSpeed => targetData.rotSpeedCurve.Evaluate(createTimer);
	public float TargetAverageRotSpeed => (targetData.rotSpeedCurve.Evaluate(createSpeedCount) + targetData.rotSpeedCurve.Evaluate(createSpeedCount + 1)) / 2f;
	public float CreateTimer => createTimer;
	public int CreateCount => createCount;
	public void Init()
	{
		DataInit();
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Init(i);
			players[i].GetMachine().SetCauldronMaterial(cauldronMaterials[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[players[i].PlayerNo]]);
		}
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].SecondGroupInit();
		}
		DataInit();
	}
	private void DataInit()
	{
		for (int i = 0; i < players.Length; i++)
		{
			TargetUpdate(i);
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.SetTime(15f);
		AiDataInit();
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<CommonNotificationManager>.Instance.IsPause || !SingletonCustom<MakingPotion_GameManager>.Instance.IsGameNow || !isNowCreate)
		{
			return;
		}
		createTimer += Time.deltaTime;
		SingletonCustom<MakingPotion_UiManager>.Instance.SetTime(15f - createTimer);
		if ((float)(createSpeedCount + 1) * 1f < createTimer)
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].SpinCheck();
			}
			createSpeedCount++;
		}
		if (createSugarCount < targetData.sugarTimeDataArray.Length && targetData.sugarTimeDataArray[createSugarCount].time < createTimer)
		{
			for (int j = 0; j < players.Length; j++)
			{
				players[j].SetTargetSugarTimeData(targetData.sugarTimeDataArray[createSugarCount]);
			}
			createSugarCount++;
			isSugarForceEndWait = true;
			if (createSugarCount < targetData.sugarTimeDataArray.Length)
			{
				sugarForceEndSecond = targetData.sugarTimeDataArray[createSugarCount].time - targetData.sugarTimeDataArray[createSugarCount - 1].time - 0.2f;
			}
			else
			{
				sugarForceEndSecond = 10f;
			}
		}
		if (isSugarForceEndWait)
		{
			sugarForceEndTimer += Time.deltaTime;
			if (sugarForceEndTimer > sugarForceEndSecond)
			{
				sugarForceEndTimer = 0f;
				isSugarForceEndWait = false;
				for (int k = 0; k < players.Length; k++)
				{
					players[k].SugarOperationEnd();
				}
			}
		}
		if (createTurnCount < targetData.turnTimeDataArray.Length && targetData.turnTimeDataArray[createTurnCount].time < createTimer)
		{
			for (int l = 0; l < players.Length; l++)
			{
				players[l].SetTargetTurnDir(targetData.turnTimeDataArray[createTurnCount].isRight);
			}
			SingletonCustom<MakingPotion_UiManager>.Instance.ShowArrow(targetData.turnTimeDataArray[createTurnCount].isRight);
			createTurnCount++;
		}
		if (createTimer > 15f)
		{
			sugarForceEndTimer = 0f;
			isSugarForceEndWait = false;
			for (int m = 0; m < players.Length; m++)
			{
				players[m].SugarOperationEnd();
			}
			CreateEnd();
			SingletonCustom<MakingPotion_UiManager>.Instance.HideArrow();
			return;
		}
		for (int n = 0; n < players.Length; n++)
		{
			bool flag = true;
			if (players[n].IsPlayer)
			{
				players[n].RodControl(MakingPotion_Controller.GetEditorStickDir(SingletonCustom<MakingPotion_GameManager>.Instance.GetCamera(n), players[n].GetMachineCenterPos()));
				flag = false;
			}
			if (flag)
			{
				AiUpdate(n);
			}
			players[n].UpdateMethod();
		}
	}
	public void ColorButtonOperation(int _charaNo, int _playerNo)
	{
		bool flag = false;
		if (!isReds[_playerNo] && MakingPotion_Controller.GetRedButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Add(MakingPotion_PlayerScript.SugarColorType.Red);
			isReds[_playerNo] = true;
			flag = true;
			colorTypeNo[_playerNo] = 1;
		}
		if (!isYellows[_playerNo] && MakingPotion_Controller.GetYellowButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Add(MakingPotion_PlayerScript.SugarColorType.Yellow);
			isYellows[_playerNo] = true;
			flag = true;
			colorTypeNo[_playerNo] = 4;
		}
		if (!isGreens[_playerNo] && MakingPotion_Controller.GetGreenButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Add(MakingPotion_PlayerScript.SugarColorType.Green);
			isGreens[_playerNo] = true;
			flag = true;
			colorTypeNo[_playerNo] = 2;
		}
		if (!isBlues[_playerNo] && MakingPotion_Controller.GetBlueButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Add(MakingPotion_PlayerScript.SugarColorType.Blue);
			isBlues[_playerNo] = true;
			flag = true;
			colorTypeNo[_playerNo] = 3;
		}
		if (isReds[_playerNo] && !MakingPotion_Controller.GetRedButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Remove(MakingPotion_PlayerScript.SugarColorType.Red);
			isReds[_playerNo] = false;
			flag = true;
			colorTypeNo[_playerNo] = 0;
		}
		if (isYellows[_playerNo] && !MakingPotion_Controller.GetYellowButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Remove(MakingPotion_PlayerScript.SugarColorType.Yellow);
			isYellows[_playerNo] = false;
			flag = true;
			colorTypeNo[_playerNo] = 0;
		}
		if (isGreens[_playerNo] && !MakingPotion_Controller.GetGreenButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Remove(MakingPotion_PlayerScript.SugarColorType.Green);
			isGreens[_playerNo] = false;
			flag = true;
			colorTypeNo[_playerNo] = 0;
		}
		if (isBlues[_playerNo] && !MakingPotion_Controller.GetBlueButton(_playerNo))
		{
			ctrlColorLists[_playerNo].Remove(MakingPotion_PlayerScript.SugarColorType.Blue);
			isBlues[_playerNo] = false;
			flag = true;
			colorTypeNo[_playerNo] = 0;
		}
		if (flag)
		{
			if (ctrlColorLists[_playerNo].Count > 0)
			{
				players[_charaNo].SetSugarColorType(ctrlColorLists[_playerNo][ctrlColorLists[_playerNo].Count - 1]);
			}
			else
			{
				players[_charaNo].SetSugarColorType(MakingPotion_PlayerScript.SugarColorType.White);
			}
		}
	}
	public void GameStartColorButtonOperation(int _charaNo)
	{
	}
	public void CreateStart()
	{
		isNowCreate = true;
		targetData = SingletonCustom<MakingPotion_TargetManager>.Instance.GetData(createCount);
		for (int i = 0; i < players.Length; i++)
		{
			players[i].CreateStart();
			players[i].SetTargetTurnDir(targetData.turnTimeDataArray[0].isRight);
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.SetTime(15f);
		SingletonCustom<MakingPotion_UiManager>.Instance.ShowArrow(targetData.turnTimeDataArray[0].isRight);
		SingletonCustom<MakingPotion_UiManager>.Instance.SetCreateNum(createCount + 1);
	}
	public void CreateEnd()
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].CreateEnd();
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.SetTime(0f);
		isNowCreate = false;
		createCount++;
		if (createCount < 5)
		{
			LeanTween.delayedCall(base.gameObject, 3.5f, (Action)delegate
			{
				createTimer = 0f;
				createSpeedCount = 0;
				createSugarCount = 0;
				createTurnCount = 0;
				CreateStart();
			});
		}
		else
		{
			GameEnd();
		}
	}
	public void GameStart()
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].GameStart();
		}
		CreateStart();
	}
	public void GameEnd()
	{
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].GameEnd();
			}
		});
		SingletonCustom<MakingPotion_GameManager>.Instance.GameEnd();
	}
	public void TargetUpdate(int _charaNo)
	{
		SingletonCustom<MakingPotion_UiManager>.Instance.ViewTarget(_charaNo, players[_charaNo].CreateCount % 5);
	}
	public bool GetIsPlayer(int _charaNo)
	{
		return players[_charaNo].IsPlayer;
	}
	public int GetPlayerNo(int _charaNo)
	{
		return players[_charaNo].PlayerNo;
	}
	public int[] GetPlayerNoArray()
	{
		int[] array = new int[players.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = players[i].PlayerNo;
		}
		return array;
	}
	public MakingPotion_PlayerScript GetPlayer(int _charaNo)
	{
		return players[_charaNo];
	}
	public Material GetMachineOpaqueMaterial(int _charaNo)
	{
		return machineBodyMaterials_Opaque[_charaNo];
	}
	public Material GetMachineTransparentMaterial(int _charaNo)
	{
		return machineBodyMaterials_Transparent[_charaNo];
	}
	public Material GetDropSugarEffectMaterial(MakingPotion_PlayerScript.SugarColorType _colorType)
	{
		return dropSugarEffectMaterials[(int)_colorType];
	}
	public Color GetThreadColor(MakingPotion_PlayerScript.SugarColorType _colorType)
	{
		return threadColors[(int)_colorType];
	}
	public Color GetCottonColor(MakingPotion_PlayerScript.SugarColorType _colorType)
	{
		return cottonColors[(int)_colorType];
	}
	private void AiDataInit()
	{
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			aiUpdateInterval = 0.8f;
			break;
		case 1:
			aiUpdateInterval = 0.5f;
			break;
		case 2:
			aiUpdateInterval = 0.3f;
			break;
		}
		aiUpdateTimes = new float[4];
		for (int i = 0; i < aiUpdateTimes.Length; i++)
		{
			aiUpdateTimes[i] = UnityEngine.Random.Range(-0.2f, 0.2f);
		}
	}
	private void AiUpdate(int _charaNo)
	{
		players[_charaNo].AiControl();
	}
}
