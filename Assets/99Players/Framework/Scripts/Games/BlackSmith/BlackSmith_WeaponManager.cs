using System;
using UnityEngine;
public class BlackSmith_WeaponManager : SingletonCustom<BlackSmith_WeaponManager>
{
	[SerializeField]
	[Header("フィ\u30fcルド")]
	private BlackSmith_Field[] arrayField;
	[SerializeField]
	[Header("作成する武器")]
	private BlackSmith_Weapon[] arrayWeapon;
	private int[] arraySortWeaponIdx;
	[SerializeField]
	[Header("評価に応じた加算されるポイント")]
	private int[] arrrayEvaluationPoint;
	public void Init()
	{
		for (int i = 0; i < arrayField.Length; i++)
		{
			arrayField[i].Init();
		}
		arraySortWeaponIdx = new int[arrayWeapon.Length];
		for (int j = 0; j < arraySortWeaponIdx.Length; j++)
		{
			arraySortWeaponIdx[j] = j;
		}
		arraySortWeaponIdx.Shuffle();
	}
	public BlackSmith_Weapon GetWeapon(int _idx)
	{
		return arrayWeapon[arraySortWeaponIdx[_idx % arrayWeapon.Length]];
	}
	public int GetEvaluationPoint(BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		return arrrayEvaluationPoint[(int)_evaluationType];
	}
	public Transform GetCreateWeaponAnchor(int _playerNo)
	{
		return arrayField[_playerNo].GetCreateWeaponAnchor();
	}
	public void PlayFadeInAnimation(int _playerNo, Action _gaugeFadeInCallBack = null, Action _callBack = null)
	{
		arrayField[_playerNo].PlayFadeInAnimation(_gaugeFadeInCallBack, _callBack);
	}
	public void PlayFadeOutAnimation(int _playerNo, Action _callBack = null)
	{
		arrayField[_playerNo].PlayFadeOutAnimation(_callBack);
	}
	public Transform GetFadeInAnchor(int _playerNo)
	{
		return arrayField[_playerNo].GetFadeInAnchor();
	}
	public Transform GetFadeOutAnchor(int _playerNo)
	{
		return arrayField[_playerNo].GetFadeOutAnchor();
	}
}
