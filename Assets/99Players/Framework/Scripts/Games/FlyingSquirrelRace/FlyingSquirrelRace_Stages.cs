using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Stages : SingletonMonoBehaviour<FlyingSquirrelRace_Stages>
{
	[SerializeField]
	[DisplayName("コンフィグ")]
	private FlyingSquirrelRace_StageConfig config;
	[SerializeField]
	[DisplayName("ステ\u30fcジ")]
	private FlyingSquirrelRace_Stage[] stages;
	[SerializeField]
	[DisplayName("スタ\u30fcト位置")]
	private FlyingSquirrelRace_StagePattern startPattern;
	[SerializeField]
	[DisplayName("ゴ\u30fcル位置")]
	private FlyingSquirrelRace_StagePattern endPattern;
	[SerializeField]
	[DisplayName("ステ\u30fcジパタ\u30fcン")]
	private FlyingSquirrelRace_StagePattern[] stagePatterns;
	private SimpleCircularQueue<int> usedHash = new SimpleCircularQueue<int>(4);
	private Dictionary<int, FlyingSquirrelRace_StagePattern> spHash = new Dictionary<int, FlyingSquirrelRace_StagePattern>();
	public void Initialize()
	{
		for (int i = 0; i < stages.Length; i++)
		{
			stages[i].Initialize(i);
		}
		SetStagePatternHash();
		CreateStage();
	}
	public void PostInitialize()
	{
		FlyingSquirrelRace_Stage[] array = stages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PostInitialize();
		}
	}
	public void UpdateMethod()
	{
		if (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState == FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			FlyingSquirrelRace_Stage[] array = stages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateMethod();
			}
		}
	}
	public void FixedUpdateMethod()
	{
		if (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState == FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			FlyingSquirrelRace_Stage[] array = stages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FixedUpdateMethod();
			}
		}
	}
	public FlyingSquirrelRace_Stage GetStage(int playerNo)
	{
		return stages[playerNo];
	}
	private void SetStagePatternHash()
	{
		spHash.Add(0, startPattern);
		FlyingSquirrelRace_StagePattern.ConnectionPoint connection = startPattern.EndConnectionPoint;
		for (int i = 1; i < config.StageSize - 2 + 1; i++)
		{
			FlyingSquirrelRace_StagePattern flyingSquirrelRace_StagePattern = (from sp in stagePatterns
				where sp.StartConnectionPoint == connection && !spHash.ContainsValue(sp)
				orderby Guid.NewGuid()
				select sp).FirstOrDefault();
			if (flyingSquirrelRace_StagePattern == null)
			{
				flyingSquirrelRace_StagePattern = (from sp in stagePatterns
					where sp.StartConnectionPoint == connection
					orderby Guid.NewGuid()
					select sp).FirstOrDefault();
			}
			spHash.Add(i, flyingSquirrelRace_StagePattern);
			connection = flyingSquirrelRace_StagePattern.EndConnectionPoint;
		}
		spHash.Add(config.StageSize - 2 + 1, endPattern);
	}
	private void CreateStage()
	{
		FlyingSquirrelRace_Stage[] array = stages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].AddPattern(GetStagePattern(0));
		}
	}
	public FlyingSquirrelRace_StagePattern GetStagePattern(int idx)
	{
		return spHash[idx];
	}
}
