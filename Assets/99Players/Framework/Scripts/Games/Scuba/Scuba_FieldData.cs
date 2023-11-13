using System;
using System.Collections.Generic;
using UnityEngine;
public class Scuba_FieldData : MonoBehaviour
{
	[Serializable]
	public class ConnectFieldData
	{
		public int connectFieldNo;
		public bool isNeedAction;
		public Transform[] connectPoints;
	}
	[SerializeField]
	[Header("フィ\u30fcルド番号：実際は配列の添字を番号として扱うため使用しない")]
	private int fieldNo;
	[Header("アクションでのみ入れる場所かどうか")]
	public bool isScubaRoom;
	[Header("フィ\u30fcルドの中心")]
	public Transform center;
	[Header("フィ\u30fcルドの範囲")]
	public Transform rightTop;
	public Transform leftBottom;
	[Header("繋がっているフィ\u30fcルド情報")]
	public ConnectFieldData[] connectFieldDataArray;
	public bool CheckInField(Vector3 _pos)
	{
		if (_pos.x <= rightTop.transform.position.x && _pos.z <= rightTop.transform.position.z && _pos.x > leftBottom.transform.position.x)
		{
			return _pos.z > leftBottom.transform.position.z;
		}
		return false;
	}
	public bool CheckContainsConnectFieldNo(int _fieldNo)
	{
		for (int i = 0; i < connectFieldDataArray.Length; i++)
		{
			if (connectFieldDataArray[i].connectFieldNo == _fieldNo)
			{
				return true;
			}
		}
		return false;
	}
	public int GetRandomConnectFieldNo(int _ignoreFieldNo = -1)
	{
		if (_ignoreFieldNo == -1)
		{
			return connectFieldDataArray[UnityEngine.Random.Range(0, connectFieldDataArray.Length)].connectFieldNo;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < connectFieldDataArray.Length; i++)
		{
			if (connectFieldDataArray[i].connectFieldNo != _ignoreFieldNo)
			{
				list.Add(connectFieldDataArray[i].connectFieldNo);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}
	public Transform SearchNearestConnectPoint(int _targetFieldNo, Vector3 _pos)
	{
		for (int i = 0; i < connectFieldDataArray.Length; i++)
		{
			if (connectFieldDataArray[i].connectFieldNo != _targetFieldNo)
			{
				continue;
			}
			int num = 0;
			float num2 = (connectFieldDataArray[i].connectPoints[0].position - _pos).sqrMagnitude;
			for (int j = 1; j < connectFieldDataArray[i].connectPoints.Length; j++)
			{
				float sqrMagnitude = (connectFieldDataArray[i].connectPoints[j].position - _pos).sqrMagnitude;
				if (num2 > sqrMagnitude)
				{
					num = j;
					num2 = sqrMagnitude;
				}
			}
			return connectFieldDataArray[i].connectPoints[num];
		}
		UnityEngine.Debug.LogError("_targetFieldNoが一致しませんでした");
		return connectFieldDataArray[0].connectPoints[0];
	}
}
