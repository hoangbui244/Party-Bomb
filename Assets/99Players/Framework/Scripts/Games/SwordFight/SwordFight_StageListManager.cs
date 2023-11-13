using System;
using System.Collections.Generic;
using UnityEngine;
public class SwordFight_StageListManager : SingletonCustom<SwordFight_StageListManager>
{
	[SerializeField]
	[Header("ステ\u30fcジ名リスト")]
	public List<string> objNameList;
	[SerializeField]
	[Header("ステ\u30fcジプレハブリスト")]
	public SwordFight_StageData[] objListPref;
	private void Awake()
	{
		objListPref = new SwordFight_StageData[objNameList.Count];
	}
	public T InstantiateObj<T>(int _no, Vector3 _pos, Quaternion _rot)
	{
		UnityEngine.Object value = UnityEngine.Object.Instantiate(GetStagePref(_no).gameObject, _pos, _rot);
		if (typeof(T) == typeof(GameObject))
		{
			return (T)Convert.ChangeType(value, typeof(T));
		}
		return default(T);
	}
	private SwordFight_StageData GetStagePref(int _no)
	{
		if (objListPref[_no] == null)
		{
			objListPref[_no] = (Resources.Load("Stage/" + objNameList[_no]) as GameObject).GetComponent<SwordFight_StageData>();
		}
		return objListPref[_no];
	}
	public SwordFight_StageData GetStageData(int _no)
	{
		return GetStagePref(_no);
	}
	public int GetStageNum()
	{
		return objNameList.Count;
	}
}
