using System.Collections.Generic;
using UnityEngine;
public class WhackMoleCharacterManager : SingletonCustom<WhackMoleCharacterManager>
{
	[SerializeField]
	private WhackMoleCharacterScript[] charas;
	[SerializeField]
	[Header("カ\u30fcソルテクスチャ")]
	private Texture[] cursorTextures;
	private const float AI_UPDATE_INTERVAL_WEAK = 0.8f;
	private const float AI_UPDATE_INTERVAL_NORMAL = 0.5f;
	private const float AI_UPDATE_INTERVAL_STRONG = 0.3f;
	private const float AI_INTERVAL_RANDOM = 0.2f;
	private float aiUpdateInterval;
	private float[] aiUpdateTimes;
	public void Init()
	{
		DataInit();
		for (int i = 0; i < charas.Length; i++)
		{
			charas[i].Init(i);
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
		if (SingletonCustom<CommonNotificationManager>.Instance.IsPause || !SingletonCustom<WhackMoleGameManager>.Instance.IsGameNow)
		{
			return;
		}
		for (int i = 0; i < charas.Length; i++)
		{
			bool flag = true;
			if (charas[i].IsPlayer)
			{
				if (!charas[i].IsWhackAnim)
				{
					charas[i].CursorControl(WhackMoleController.GetStickDir(charas[i].PlayerNo));
					if (WhackMoleController.GetWhackButtonDown(charas[i].PlayerNo))
					{
						WhackAction(i);
					}
				}
				flag = false;
			}
			if (flag && !charas[i].IsWhackAnim)
			{
				AiUpdate(i);
			}
			charas[i].UpdateMethod();
		}
	}
	public void GameStart()
	{
	}
	private void WhackAction(int _charaNo)
	{
		charas[_charaNo].WhackAnimation(delegate
		{
			SingletonCustom<WhackMoleTargetManager>.Instance.WhackReceive(_charaNo, charas[_charaNo].HoleNo);
		});
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
	public WhackMoleCharacterScript GetChara(int _charaNo)
	{
		return charas[_charaNo];
	}
	public Texture GetCursorTexture(int _playerNo)
	{
		return cursorTextures[_playerNo];
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
			aiUpdateTimes[i] = Random.Range(-0.2f, 0.2f);
		}
	}
	private void AiUpdate(int _charaNo)
	{
		aiUpdateTimes[_charaNo] += Time.deltaTime;
		if (!(aiUpdateTimes[_charaNo] > aiUpdateInterval))
		{
			return;
		}
		aiUpdateTimes[_charaNo] -= aiUpdateInterval + Random.Range(-0.2f, 0.2f);
		List<int> aiTargetNoList_Rare = SingletonCustom<WhackMoleTargetManager>.Instance.GetAiTargetNoList_Rare(_charaNo);
		List<int> aiTargetNoList_Normal = SingletonCustom<WhackMoleTargetManager>.Instance.GetAiTargetNoList_Normal(_charaNo);
		if (aiTargetNoList_Rare.Contains(charas[_charaNo].HoleNo))
		{
			WhackAction(_charaNo);
			return;
		}
		if (aiTargetNoList_Normal.Contains(charas[_charaNo].HoleNo))
		{
			WhackAction(_charaNo);
			return;
		}
		int num = SearchAiAdjacentHoleNo(charas[_charaNo].HoleNo, aiTargetNoList_Rare);
		if (num < 0)
		{
			num = SearchAiAdjacentHoleNo(charas[_charaNo].HoleNo, aiTargetNoList_Normal);
		}
		if (num < 0)
		{
			num = 4;
		}
		charas[_charaNo].SetHoleNo(num);
	}
	private int SearchAiAdjacentHoleNo(int _nowHoleNo, List<int> _targetHoleNoList)
	{
		List<int> list = new List<int>();
		list.AddRange(WhackMoleGameManager.GetAdjacentHoleNoArray(_nowHoleNo));
		for (int i = 0; i < _targetHoleNoList.Count; i++)
		{
			if (!list.Contains(_targetHoleNoList[i]))
			{
				_targetHoleNoList.RemoveAt(i);
				i--;
			}
		}
		if (_targetHoleNoList.Count == 0)
		{
			return -1;
		}
		return _targetHoleNoList[Random.Range(0, _targetHoleNoList.Count)];
	}
}
