using UnityEngine;
public class Hidden_FieldManager : SingletonCustom<Hidden_FieldManager>
{
	[SerializeField]
	private Transform rightTop;
	[SerializeField]
	private Transform leftBottom;
	[SerializeField]
	private Transform startLookAnchor;
	[SerializeField]
	private Hidden_FieldData[] fieldDatas;
	public bool CheckInField(Vector3 _pos)
	{
		if (_pos.x < rightTop.position.x && _pos.z < rightTop.position.z && _pos.x > leftBottom.position.x)
		{
			return _pos.z > leftBottom.position.z;
		}
		return false;
	}
	public bool JudgeTurnRight(Vector3 _pos, Vector3 _dir)
	{
		bool flag = rightTop.position.z - _pos.z < _pos.z - leftBottom.position.z;
		bool flag2 = rightTop.position.x - _pos.x < _pos.x - leftBottom.position.x;
		float num = 100000f;
		float num2 = 100000f;
		bool flag3 = false;
		bool flag4 = false;
		if (_dir.x > 0f)
		{
			num = Mathf.Abs((rightTop.position.x - _pos.x) / _dir.x);
			flag3 = true;
		}
		else if (_dir.x < 0f)
		{
			num = Mathf.Abs((leftBottom.position.x - _pos.x) / _dir.x);
		}
		if (_dir.z > 0f)
		{
			num2 = Mathf.Abs((rightTop.position.z - _pos.z) / _dir.z);
			flag4 = true;
		}
		else if (_dir.z < 0f)
		{
			num2 = Mathf.Abs((leftBottom.position.z - _pos.z) / _dir.z);
		}
		if (num < num2)
		{
			return flag == flag3;
		}
		return flag2 != flag4;
	}
	public Transform GetStartLookAnchor()
	{
		return startLookAnchor;
	}
	public bool CheckIsConnectTwoFieldNo(int _fieldNoA, int _fieldNoB)
	{
		return fieldDatas[_fieldNoA].CheckContainsConnectFieldNo(_fieldNoB);
	}
	public int SearchFieldNo(Vector3 _pos)
	{
		for (int i = 0; i < fieldDatas.Length; i++)
		{
			if (fieldDatas[i].CheckInField(_pos))
			{
				return i;
			}
		}
		UnityEngine.Debug.LogError("どのフィ\u30fcルド上にもいません");
		return 0;
	}
	public int SearchRandomConnectFieldNo(int _nowFieldNo, int _prevFieldNo = -1)
	{
		return fieldDatas[_nowFieldNo].GetRandomConnectFieldNo(_prevFieldNo);
	}
	public Transform SearchNearestConnectPoint(int _nowFieldNo, int _targetFieldNo, Vector3 _pos)
	{
		return fieldDatas[_nowFieldNo].SearchNearestConnectPoint(_targetFieldNo, _pos);
	}
	public int SearchEscapeConnectFieldNo(int _nowFieldNo, Vector3 _pos, Vector3 _dir)
	{
		int num = -1;
		float num2 = -1E+07f;
		for (int i = 0; i < fieldDatas[_nowFieldNo].connectFieldDataArray.Length; i++)
		{
			int connectFieldNo = fieldDatas[_nowFieldNo].connectFieldDataArray[i].connectFieldNo;
			Vector3 lhs = fieldDatas[connectFieldNo].center.position - _pos;
			lhs.y = 0f;
			float num3 = Vector3.Dot(lhs, _dir);
			if (num3 > num2)
			{
				num = connectFieldNo;
				num2 = num3;
			}
		}
		if (num == -1)
		{
			UnityEngine.Debug.LogError("逃げる先が見つかりませんでした");
			return 0;
		}
		return num;
	}
}
