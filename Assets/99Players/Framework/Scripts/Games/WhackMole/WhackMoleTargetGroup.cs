using System;
using System.Collections.Generic;
using UnityEngine;
public class WhackMoleTargetGroup : MonoBehaviour
{
	public WhackMoleTarget[] normalTargets;
	public WhackMoleTarget[] rareTargets;
	public ParticleSystem[] holeEffects;
	private bool isPlayer;
	private int playerNo;
	private int charaNo;
	private bool IsPlayer => isPlayer;
	public int PlayerNo => playerNo;
	public int CharaNo => charaNo;
	public void Init(int _playerNo, int _charaNo)
	{
		isPlayer = (_playerNo < 4);
		playerNo = _playerNo;
		charaNo = _charaNo;
		for (int i = 0; i < 9; i++)
		{
			normalTargets[i].Init();
			rareTargets[i].Init();
		}
	}
	public void SecondGroupInit(int _playerNo, int _charaNo)
	{
		isPlayer = (_playerNo < 4);
		playerNo = _playerNo;
		charaNo = _charaNo;
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < 9; i++)
		{
			normalTargets[i].UpdateMethod();
			rareTargets[i].UpdateMethod();
		}
	}
	public void ShowMole(WhackMoleTargetManager.MoleData _data)
	{
		switch (_data.moleType)
		{
		case WhackMoleTargetManager.MoleType.N:
			normalTargets[_data.holeNo].Show(_data);
			break;
		case WhackMoleTargetManager.MoleType.R:
			rareTargets[_data.holeNo].Show(_data);
			break;
		}
	}
	public void WhackCheck(int _holeNo)
	{
		if (isPlayer)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
		}
		if (normalTargets[_holeNo].CheckCanWhack())
		{
			normalTargets[_holeNo].Whack();
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_whack_hit");
			}
			SingletonCustom<WhackMoleGameManager>.Instance.AddScore(charaNo, 50);
			SingletonCustom<WhackMoleUiManager>.Instance.ScoreUpdate(charaNo);
			LeanTween.delayedCall(0.2f, (Action)delegate
			{
				SingletonCustom<WhackMoleUiManager>.Instance.ViewGetPoint(charaNo, 50, normalTargets[_holeNo].GetPointUiPos());
			});
		}
		else if (rareTargets[_holeNo].CheckCanWhack())
		{
			rareTargets[_holeNo].Whack();
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_whack_hit_rare");
			}
			SingletonCustom<WhackMoleGameManager>.Instance.AddScore(charaNo, 150);
			SingletonCustom<WhackMoleUiManager>.Instance.ScoreUpdate(charaNo);
			LeanTween.delayedCall(0.2f, (Action)delegate
			{
				SingletonCustom<WhackMoleUiManager>.Instance.ViewGetPoint(charaNo, 150, rareTargets[_holeNo].GetPointUiPos());
			});
		}
	}
	public void PlayHoleEffect()
	{
		for (int i = 0; i < holeEffects.Length; i++)
		{
			holeEffects[i].Play();
		}
	}
	public List<int> SearchAiTargetNoList_Normal()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < normalTargets.Length; i++)
		{
			if (normalTargets[i].IsCanAiTarget)
			{
				list.Add(i);
			}
		}
		return list;
	}
	public List<int> SearchAiTargetNoList_Rare()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < rareTargets.Length; i++)
		{
			if (rareTargets[i].IsCanAiTarget)
			{
				list.Add(i);
			}
		}
		return list;
	}
}
