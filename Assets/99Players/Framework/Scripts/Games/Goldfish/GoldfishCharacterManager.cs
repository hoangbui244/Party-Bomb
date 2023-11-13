using UnityEngine;
public class GoldfishCharacterManager : SingletonCustom<GoldfishCharacterManager>
{
	[SerializeField]
	private GoldfishCharacterScript[] charas;
	[SerializeField]
	private Material[] poiMaterials;
	private const float AI_UPDATE_INTERVAL_WEAK = 1f;
	private const float AI_UPDATE_INTERVAL_NORMAL = 0.6f;
	private const float AI_UPDATE_INTERVAL_STRONG = 0.3f;
	private const float AI_INTERVAL_RANDOM = 0.2f;
	private float aiUpdateInterval;
	private float[] aiUpdateTimes;
	public void Init()
	{
		DataInit();
		if (SingletonCustom<GoldfishGameManager>.Instance.CharaNum == 4)
		{
			for (int i = 4; i < charas.Length; i++)
			{
				charas[i].gameObject.SetActive(value: false);
				charas[i].PoiObj.SetActive(value: false);
			}
			charas = new GoldfishCharacterScript[4]
			{
				charas[0],
				charas[1],
				charas[2],
				charas[3]
			};
		}
		for (int j = 0; j < charas.Length; j++)
		{
			charas[j].Init(j);
		}
	}
	public void SecondGroupInit()
	{
		DataInit();
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].SecondGroupInit();
		}
	}
	private void DataInit()
	{
		AiDataInit();
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<GoldfishGameManager>.Instance.IsGameNow)
		{
			return;
		}
		for (int i = 0; i < charas.Length; i++)
		{
			bool flag = true;
			if (charas[i].IsPlayer)
			{
				if (!charas[i].IsEnd)
				{
					if (!charas[i].IsScoopAnim)
					{
						charas[i].CursorControl(GoldfishController.GetStickDir(charas[i].PlayerNo));
					}
					if (GoldfishController.GetScoopButtonDown(charas[i].PlayerNo))
					{
						WaterInAction(i);
					}
					else if (GoldfishController.GetScoopButtonUp(charas[i].PlayerNo))
					{
						ScoopAction(i);
					}
				}
				flag = false;
			}
			if (flag && !charas[i].IsEnd && !charas[i].IsScoopAnim)
			{
				AiUpdate(i);
			}
			charas[i].UpdateMethod();
		}
	}
	public void GameStart()
	{
	}
	private void WaterInAction(int _charaNo)
	{
		charas[_charaNo].WaterInAnimation();
	}
	private void ScoopAction(int _charaNo)
	{
		charas[_charaNo].ScoopAnimation();
	}
	public bool GetIsPlayer(int _charaNo)
	{
		return charas[_charaNo].IsPlayer;
	}
	public int GetPlayerNo(int _charaNo)
	{
		return charas[_charaNo].PlayerNo;
	}
	public int[] GetPlayerNoArray()
	{
		int[] array = new int[charas.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = charas[i].PlayerNo;
		}
		return array;
	}
	public GoldfishCharacterScript GetChara(int _charaNo)
	{
		return charas[_charaNo];
	}
	public Material GetPoiMaterial(int _charaNo)
	{
		return poiMaterials[_charaNo];
	}
	public bool CheckBreakEnd()
	{
		bool result = true;
		for (int i = 0; i < charas.Length; i++)
		{
			if (!charas[i].IsEnd && charas[i].IsPlayer)
			{
				result = false;
			}
		}
		return result;
	}
	public void CpuFutureScoreCalc()
	{
		for (int i = 0; i < charas.Length; i++)
		{
			if (!charas[i].IsPlayer && !charas[i].IsEnd)
			{
				int score = SingletonCustom<GoldfishGameManager>.Instance.GetScore(i);
				int num = Mathf.FloorToInt((float)score * 60f / SingletonCustom<GoldfishGameManager>.Instance.GameTime);
				int num2 = Mathf.FloorToInt((float)score / charas[i].GetPoi().HpLerp);
				score = ((num < num2) ? num : num2);
				score = score / 10 * 10;
				SingletonCustom<GoldfishGameManager>.Instance.SetScore(i, score);
			}
		}
	}
	private void AiDataInit()
	{
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			aiUpdateInterval = 1f;
			break;
		case 1:
			aiUpdateInterval = 0.6f;
			break;
		case 2:
			aiUpdateInterval = 0.3f;
			break;
		}
		aiUpdateTimes = new float[SingletonCustom<GoldfishGameManager>.Instance.CharaNum];
		for (int i = 0; i < aiUpdateTimes.Length; i++)
		{
			aiUpdateTimes[i] = Random.Range(-0.2f, 0.2f);
			charas[i].AiStickDirSetting();
		}
	}
	private void AiUpdate(int _charaNo)
	{
		charas[_charaNo].AiCursorUpdate();
		aiUpdateTimes[_charaNo] += Time.deltaTime;
		if (aiUpdateTimes[_charaNo] > aiUpdateInterval)
		{
			aiUpdateTimes[_charaNo] -= aiUpdateInterval + Random.Range(-0.2f, 0.2f);
			charas[_charaNo].AiStickDirSetting();
			charas[_charaNo].AiCheckScoop();
		}
	}
}
