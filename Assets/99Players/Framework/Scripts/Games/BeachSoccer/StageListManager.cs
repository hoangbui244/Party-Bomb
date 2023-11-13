using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class StageListManager : SingletonCustom<StageListManager>
	{
		[SerializeField]
		[Header("ステ\u30fcジ名リスト")]
		public List<string> objNameList;
		[SerializeField]
		[Header("ステ\u30fcジプレハブリスト")]
		public StageData[] objListPref;
		private void Awake()
		{
			objListPref = new StageData[objNameList.Count];
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
		private StageData GetStagePref(int _no)
		{
			if (objListPref[_no] == null)
			{
				UnityEngine.Debug.Log("読み込み:" + objNameList[_no] + " _no:" + _no.ToString());
				objListPref[_no] = (Resources.Load("Stage/" + objNameList[_no]) as GameObject).GetComponent<StageData>();
			}
			return objListPref[_no];
		}
		public StageData GetStageData(int _no)
		{
			return GetStagePref(_no);
		}
		public int GetStageNum()
		{
			return objNameList.Count;
		}
	}
}
