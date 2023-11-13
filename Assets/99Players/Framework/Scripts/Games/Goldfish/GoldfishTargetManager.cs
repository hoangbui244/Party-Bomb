using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GoldfishTargetManager : SingletonCustom<GoldfishTargetManager>
{
	public enum FishType
	{
		Normal_Red,
		Normal_Colorful,
		Normal_Black,
		Eye_Red,
		Eye_Black,
		Big_Red,
		Big_White,
		Big_Colorful,
		Big_Black,
		King,
		Gold
	}
	private const int FISH_MAX_SHOW_NUM = 90;
	private const float CREATE_CHECK_INTERVAL = 5f;
	private const float CREATE_POS_OFFSET = -0.1f;
	private const float GOLD_TYPE_FISH_SHOW_TIME = 40f;
	private const float CAMERA_RANGE_OFFSET = 0.2f;
	private static readonly int[] FISH_TYPE_CREATE_NUMS = new int[11]
	{
		60,
		20,
		20,
		10,
		10,
		5,
		5,
		5,
		5,
		2,
		2
	};
	private float[] kingShowTimeData = new float[2]
	{
		20f,
		40f
	};
	private float[] goldShowTimeData = new float[2]
	{
		30f,
		50f
	};
	[SerializeField]
	private GoldfishTarget[] goldfishPrefabs;
	private GoldfishTarget[] goldfishArray;
	[SerializeField]
	private Transform goldfishAnchor;
	private Vector3 createRightTopPos;
	private Vector3 createLeftBottomPos;
	private int createNum;
	private float createTimer;
	private List<int> goldfishShowOrderList = new List<int>();
	private int nowShowOrderNo;
	private int showKingCount;
	private int showGoldCount;
	public int debugActiveCount;
	public int debugCameraInCount;
	public void Init()
	{
		createRightTopPos = SingletonCustom<GoldfishGameManager>.Instance.GetTubRightTopPos();
		createLeftBottomPos = SingletonCustom<GoldfishGameManager>.Instance.GetTubLeftBottomPos();
		createRightTopPos.x -= -0.1f;
		createLeftBottomPos.x += -0.1f;
		createLeftBottomPos.z += -0.1f;
		createNum = 0;
		for (int i = 0; i < FISH_TYPE_CREATE_NUMS.Length; i++)
		{
			createNum += FISH_TYPE_CREATE_NUMS[i];
		}
		goldfishArray = new GoldfishTarget[createNum];
		int num = 0;
		for (int j = 0; j < FISH_TYPE_CREATE_NUMS.Length; j++)
		{
			if (j != 9 && j != 10)
			{
				for (int k = 0; k < FISH_TYPE_CREATE_NUMS[j]; k++)
				{
					goldfishArray[num] = UnityEngine.Object.Instantiate(goldfishPrefabs[j], goldfishAnchor);
					goldfishArray[num].Init();
					goldfishShowOrderList.Add(num);
					num++;
				}
			}
		}
		goldfishShowOrderList = (from a in goldfishShowOrderList
			orderby Guid.NewGuid()
			select a).ToList();
		for (int l = 0; l < 90; l++)
		{
			goldfishArray[goldfishShowOrderList[l]].Show(_isStart: true);
		}
		nowShowOrderNo = 90;
		for (int m = 0; m < FISH_TYPE_CREATE_NUMS[9]; m++)
		{
			goldfishArray[num] = UnityEngine.Object.Instantiate(goldfishPrefabs[9], goldfishAnchor);
			goldfishArray[num].Init();
			goldfishShowOrderList.Add(num);
			num++;
		}
		for (int n = 0; n < FISH_TYPE_CREATE_NUMS[10]; n++)
		{
			goldfishArray[num] = UnityEngine.Object.Instantiate(goldfishPrefabs[10], goldfishAnchor);
			goldfishArray[num].Init();
			goldfishShowOrderList.Add(num);
			num++;
		}
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<GoldfishGameManager>.Instance.IsGameTitleClose)
		{
			return;
		}
		if (SingletonCustom<GoldfishGameManager>.Instance.IsGameNow)
		{
			createTimer += Time.deltaTime;
		}
		if (createTimer > 5f)
		{
			createTimer -= 5f;
			int num = 0;
			for (int i = 0; i < goldfishArray.Length; i++)
			{
				if (goldfishArray[i].IsShow)
				{
					num++;
				}
			}
			int num2 = 90 - num;
			int num3 = 0;
			for (int j = 0; j < num2; j++)
			{
				if (num3 >= 100)
				{
					break;
				}
				num3++;
				if (!TryShowFish())
				{
					j--;
				}
			}
		}
		if (showKingCount < kingShowTimeData.Length && SingletonCustom<GoldfishGameManager>.Instance.GameTime > kingShowTimeData[showKingCount])
		{
			ShowKingTypeFish(showKingCount);
			showKingCount++;
		}
		if (showGoldCount < goldShowTimeData.Length && SingletonCustom<GoldfishGameManager>.Instance.GameTime > goldShowTimeData[showGoldCount])
		{
			ShowGoldTypeFish(showGoldCount);
			showGoldCount++;
		}
		debugActiveCount = 0;
		debugCameraInCount = 0;
		for (int k = 0; k < goldfishArray.Length; k++)
		{
			goldfishArray[k].UpdateMethod();
			if (goldfishArray[k].IsShow)
			{
				debugActiveCount++;
				if (CheckInCameraRange(goldfishArray[k].transform.position))
				{
					debugCameraInCount++;
				}
			}
		}
	}
	private bool TryShowFish()
	{
		bool result = false;
		if (goldfishArray[goldfishShowOrderList[nowShowOrderNo]].IsHide && !goldfishArray[goldfishShowOrderList[nowShowOrderNo]].IsKing && !goldfishArray[goldfishShowOrderList[nowShowOrderNo]].IsGold)
		{
			goldfishArray[goldfishShowOrderList[nowShowOrderNo]].Show(_isStart: false);
			result = true;
		}
		nowShowOrderNo++;
		if (nowShowOrderNo == goldfishShowOrderList.Count)
		{
			nowShowOrderNo = 0;
		}
		return result;
	}
	private void ShowKingTypeFish(int _kingNo)
	{
		int num = FISH_TYPE_CREATE_NUMS[9] + FISH_TYPE_CREATE_NUMS[10];
		goldfishArray[goldfishArray.Length - num + _kingNo].Show(_isStart: false);
	}
	private void ShowGoldTypeFish(int _goldNo)
	{
		int num = FISH_TYPE_CREATE_NUMS[10];
		goldfishArray[goldfishArray.Length - num + _goldNo].Show(_isStart: false);
	}
	public Vector3 GetCenterCreatePos()
	{
		return (createLeftBottomPos + createRightTopPos) / 2f;
	}
	public Vector3 GetRandomCreatePos(bool _isEdge)
	{
		if (_isEdge)
		{
			if (UnityEngine.Random.Range(0, 3) == 1)
			{
				return new Vector3(UnityEngine.Random.Range(createLeftBottomPos.x, createRightTopPos.x), createRightTopPos.y, createLeftBottomPos.z);
			}
			if (UnityEngine.Random.Range(0, 2) == 1)
			{
				return new Vector3(createLeftBottomPos.x, createRightTopPos.y, UnityEngine.Random.Range(createLeftBottomPos.z, createRightTopPos.z));
			}
			return new Vector3(createRightTopPos.x, createRightTopPos.y, UnityEngine.Random.Range(createLeftBottomPos.z, createRightTopPos.z));
		}
		return new Vector3(UnityEngine.Random.Range(createLeftBottomPos.x, createRightTopPos.x), createRightTopPos.y, UnityEngine.Random.Range(createLeftBottomPos.z, createRightTopPos.z));
	}
	public bool CheckInTargetPosRange(Vector3 _pos)
	{
		if (createLeftBottomPos.x <= _pos.x && _pos.x <= createRightTopPos.x && createLeftBottomPos.z <= _pos.z)
		{
			return _pos.z <= createRightTopPos.z;
		}
		return false;
	}
	public bool CheckInCameraRange(Vector3 _pos)
	{
		if (createLeftBottomPos.x + 0.2f <= _pos.x && _pos.x <= createRightTopPos.x - 0.2f)
		{
			return createLeftBottomPos.z + 0.2f <= _pos.z;
		}
		return false;
	}
	public bool CheckUpperPosRange(Vector3 _pos)
	{
		if (createLeftBottomPos.x <= _pos.x && _pos.x <= createRightTopPos.x)
		{
			return _pos.z > createRightTopPos.z - 0.1f;
		}
		return false;
	}
	public Vector3 ClampTargetPos(Vector3 _pos)
	{
		_pos.x = Mathf.Clamp(_pos.x, createLeftBottomPos.x, createRightTopPos.x);
		_pos.z = Mathf.Clamp(_pos.z, createLeftBottomPos.z, createRightTopPos.z);
		return _pos;
	}
	public Transform GetGoldfishAnchor()
	{
		return goldfishAnchor;
	}
	public GoldfishTarget[] GetOnTriggerFishArray(Vector3 _center, float _radius, bool _isCheckDonut = false, float _donutHoleRadius = 0f)
	{
		List<GoldfishTarget> list = new List<GoldfishTarget>();
		for (int i = 0; i < goldfishArray.Length; i++)
		{
			if (goldfishArray[i].IsShow && goldfishArray[i].CheckOnTrigger(_center, _radius) && (!_isCheckDonut || !goldfishArray[i].CheckOnTriggerTail(_center, _donutHoleRadius)))
			{
				list.Add(goldfishArray[i]);
			}
		}
		return list.ToArray();
	}
	public Vector3 GetCameraRangeInDir(Vector3 _pos)
	{
		if (createLeftBottomPos.z + 0.2f > _pos.z)
		{
			if (_pos.x < createLeftBottomPos.x + 0.2f)
			{
				return new Vector3(1f, 0f, 1f).normalized;
			}
			if (createRightTopPos.x - 0.2f < _pos.x)
			{
				return new Vector3(-1f, 0f, 1f).normalized;
			}
			return Vector3.forward;
		}
		if (_pos.x < createLeftBottomPos.x + 0.2f)
		{
			return Vector3.right;
		}
		if (createRightTopPos.x - 0.2f < _pos.x)
		{
			return Vector3.left;
		}
		return Vector3.zero;
	}
	public void DangerCheck(Vector3 _dangerPoint, float _radius)
	{
		List<GoldfishTarget> list = new List<GoldfishTarget>();
		for (int i = 0; i < goldfishArray.Length; i++)
		{
			if (goldfishArray[i].IsShow && goldfishArray[i].CheckOnTrigger(_dangerPoint, _radius))
			{
				list.Add(goldfishArray[i]);
			}
		}
		list.Sort(delegate(GoldfishTarget a, GoldfishTarget b)
		{
			Vector3 vector = _dangerPoint - a.transform.position;
			Vector3 vector2 = _dangerPoint - b.transform.position;
			return (!(vector.sqrMagnitude > vector2.sqrMagnitude)) ? (-1) : 1;
		});
		for (int j = 1; j < list.Count; j++)
		{
			list[j].ForceMoveDanger(_dangerPoint);
		}
	}
}
