using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Scuba_ItemManager : SingletonCustom<Scuba_ItemManager>
{
	[Serializable]
	public class FoundEffectArray
	{
		public GameObject[] effectObjs;
		private int[] ids;
		private bool active = true;
		public bool Active => active;
		public bool TryGetIdMatchIndex(out int _index, int _id)
		{
			_index = 0;
			if (ids == null)
			{
				ids = new int[effectObjs.Length];
				return false;
			}
			for (int i = 0; i < ids.Length; i++)
			{
				if (_id == ids[i])
				{
					_index = i;
					return true;
				}
			}
			return false;
		}
		public bool TryGetEmptyIndex(out int _index)
		{
			_index = 0;
			if (ids == null)
			{
				ids = new int[effectObjs.Length];
				return true;
			}
			for (int i = 0; i < ids.Length; i++)
			{
				if (ids[i] == 0)
				{
					_index = i;
					return true;
				}
			}
			return false;
		}
		public void SetId(int _index, int _id)
		{
			if (ids == null)
			{
				ids = new int[effectObjs.Length];
			}
			ids[_index] = _id;
		}
		public void SetActive(bool _active)
		{
			if (!_active && active)
			{
				ids = new int[effectObjs.Length];
				for (int i = 0; i < effectObjs.Length; i++)
				{
					effectObjs[i].SetActive(value: false);
				}
			}
			active = _active;
		}
		public void DisableCheck(int[] _useIds)
		{
			if (ids == null)
			{
				ids = new int[effectObjs.Length];
				return;
			}
			for (int i = 0; i < ids.Length; i++)
			{
				if (ids[i] == 0)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < _useIds.Length; j++)
				{
					if (ids[i] == _useIds[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ids[i] = 0;
					effectObjs[i].SetActive(value: false);
				}
			}
		}
	}
	[Serializable]
	public class SquareNumbers
	{
		public List<int> noList;
	}
	private static readonly Vector3 VIEWPORT_FOUND_RANGE_MIN = new Vector3(0f, 0f, 0.1f);
	private static readonly Vector3 VIEWPORT_FOUND_RANGE_MAX = new Vector3(1f, 1f, 1.5f);
	private static readonly Vector3 SCREEN_FOUND_RANGE_MIN = new Vector3(0f, 0f, 0.1f);
	private static readonly Vector3 SCREEN_FOUND_RANGE_MAX = new Vector3(1920f, 1080f, 1.5f);
	private static readonly Vector3 CHARA_EYE_FOUND_RANGE_MIN = new Vector3(-2f, -1f, 0.1f);
	private static readonly Vector3 CHARA_EYE_FOUND_RANGE_MAX = new Vector3(2f, 1f, 1.5f);
	private static readonly Vector3 AI_VIEWPORT_RANGE_MIN = new Vector3(0f, 0f, 1f);
	private static readonly Vector3 AI_VIEWPORT_RANGE_MAX = new Vector3(1f, 1f, 5f);
	private static readonly int[] FISH_CREATE_SQUARE_ORDER_A = new int[9]
	{
		0,
		1,
		2,
		5,
		8,
		7,
		6,
		3,
		4
	};
	private static readonly int[] FISH_CREATE_SQUARE_ORDER_B = new int[9]
	{
		4,
		7,
		6,
		3,
		0,
		1,
		2,
		5,
		8
	};
	[SerializeField]
	private Scuba_ItemObject[] items;
	[SerializeField]
	private Scuba_Fish[] fishArray;
	[SerializeField]
	private GameObject[] fishPrefabs;
	[SerializeField]
	private Scuba_BigFish[] bigFishArray;
	[SerializeField]
	private FoundEffectArray[] foundEffectArrays;
	[SerializeField]
	private Transform rightTop;
	[SerializeField]
	private Transform leftBottom;
	private int playerNum;
	private bool isSingle;
	[SerializeField]
	private SquareNumbers[] squareArray;
	private bool isUpdateStart;
	public Vector3 RightTopPos => rightTop.position;
	public Vector3 LeftBottomPos => leftBottom.position;
	public bool IsUpdateStart
	{
		get
		{
			return isUpdateStart;
		}
		set
		{
			isUpdateStart = value;
		}
	}
	public void Init()
	{
		playerNum = SingletonCustom<Scuba_CharacterManager>.Instance.PlayerNum;
		isSingle = (playerNum == 1);
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Init();
		}
		List<int> list = new List<int>();
		for (int j = 0; j < fishPrefabs.Length; j++)
		{
			list.Add(j);
		}
		list = (from a in list
			orderby Guid.NewGuid()
			select a).ToList();
		int num = 0;
		int[] array = (UnityEngine.Random.Range(0, 2) == 1) ? FISH_CREATE_SQUARE_ORDER_A : FISH_CREATE_SQUARE_ORDER_B;
		for (int k = 0; k < squareArray.Length; k++)
		{
			for (int l = 0; l < squareArray[array[k]].noList.Count; l++)
			{
				int num2 = squareArray[array[k]].noList[l];
				fishArray[num2].CreateFishObj(fishPrefabs[list[num]]);
				fishArray[num2].Init();
				num++;
				if (num == list.Count)
				{
					num = 0;
				}
			}
		}
		for (int m = 0; m < bigFishArray.Length; m++)
		{
			bigFishArray[m].Init();
		}
	}
	public void UpdateMethod()
	{
		if (!isUpdateStart)
		{
			return;
		}
		for (int i = 0; i < fishArray.Length; i++)
		{
			fishArray[i].UpdateMethod();
		}
		for (int j = 0; j < bigFishArray.Length; j++)
		{
			bigFishArray[j].UpdateMethod();
		}
		for (int k = 0; k < 4; k++)
		{
			if (!isSingle || k == 0)
			{
				CalcCameraToViewportPoint(k);
			}
			CalcCharaCameraToScreenPoint(k);
		}
		if (isSingle)
		{
			SingletonCustom<Scuba_UiManager>.Instance.SetFoundView(0, SearchFoundRangeItems(0, _isNotFoundYet: false, SingletonCustom<Scuba_CharacterManager>.Instance.GetIsCameraFps(0)));
			return;
		}
		for (int l = 0; l < playerNum; l++)
		{
			SingletonCustom<Scuba_UiManager>.Instance.SetFoundView(l, SearchFoundRangeItems(l, _isNotFoundYet: false, SingletonCustom<Scuba_CharacterManager>.Instance.GetIsCameraFps(l)));
		}
	}
	private void FoundEffectView(int _charaNo, Scuba_ItemObject[] _items)
	{
		int[] array = new int[_items.Length];
		for (int i = 0; i < _items.Length; i++)
		{
			array[i] = _items[i].GetInstanceID();
			int _index = 0;
			if (foundEffectArrays[_charaNo].TryGetIdMatchIndex(out _index, array[i]))
			{
				foundEffectArrays[_charaNo].effectObjs[_index].transform.localScale = _items[i].GetFoundEffectScale();
				foundEffectArrays[_charaNo].effectObjs[_index].transform.position = _items[i].GetFoundEffectCenter();
			}
			else if (foundEffectArrays[_charaNo].TryGetEmptyIndex(out _index))
			{
				foundEffectArrays[_charaNo].effectObjs[_index].SetActive(value: true);
				foundEffectArrays[_charaNo].SetId(_index, array[i]);
				foundEffectArrays[_charaNo].effectObjs[_index].transform.localScale = _items[i].GetFoundEffectScale();
				foundEffectArrays[_charaNo].effectObjs[_index].transform.position = _items[i].GetFoundEffectCenter();
			}
		}
		foundEffectArrays[_charaNo].DisableCheck(array);
	}
	public void SetFoundEffectActive(int _charaNo, bool _active)
	{
		foundEffectArrays[_charaNo].SetActive(_active);
	}
	public Scuba_Fish[] GetFishArray()
	{
		return fishArray;
	}
	public void SetSquareArray(SquareNumbers[] _squares)
	{
		squareArray = _squares;
	}
	public Scuba_ItemObject[] SearchFoundRangeItems(int _charaNo, bool _isNotFoundYet, bool _isViewport)
	{
		List<Scuba_ItemObject> list = new List<Scuba_ItemObject>();
		for (int i = 0; i < items.Length; i++)
		{
			if (!_isNotFoundYet || !items[i].GetIsFound(_charaNo))
			{
				if (_isViewport && CheckInRangeViewport(items[i].GetViewportPoint(_charaNo)))
				{
					list.Add(items[i]);
				}
				else if (!_isViewport && CheckInRangeViewport(items[i].GetCharaViewportPoint(_charaNo)))
				{
					list.Add(items[i]);
				}
			}
		}
		return list.ToArray();
	}
	public Scuba_ItemObject SearchAiRandomItem(int _charaNo)
	{
		List<Scuba_ItemObject> list = new List<Scuba_ItemObject>();
		for (int i = 0; i < items.Length; i++)
		{
			if (!items[i].GetIsFound(_charaNo) && CheckInRangeAiViewport(items[i].GetCharaViewportPoint(_charaNo)))
			{
				list.Add(items[i]);
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return items[UnityEngine.Random.Range(0, items.Length)];
	}
	public Scuba_ItemObject SearchAiSortItem(int _charaNo, float _orderLerp)
	{
		List<Scuba_ItemObject> list = new List<Scuba_ItemObject>();
		for (int i = 0; i < items.Length; i++)
		{
			if (!items[i].GetIsFound(_charaNo) && CheckInRangeAiViewport(items[i].GetCharaViewportPoint(_charaNo)))
			{
				list.Add(items[i]);
			}
		}
		SortItemList_CharaViewportZ(_charaNo, list);
		SortItemList_Score(list);
		if (list.Count > 0)
		{
			return list[Mathf.RoundToInt((float)(list.Count - 1) * _orderLerp)];
		}
		return items[UnityEngine.Random.Range(0, items.Length)];
	}
	public Scuba_ItemObject SearchAiReverseItem(int _charaNo, float _orderLerp)
	{
		List<Scuba_ItemObject> list = new List<Scuba_ItemObject>();
		for (int i = 0; i < items.Length; i++)
		{
			if (!items[i].GetIsFound(_charaNo) && items[i].GetCharaViewportPoint(_charaNo).z < 0f)
			{
				list.Add(items[i]);
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return items[UnityEngine.Random.Range(0, items.Length)];
	}
	public void CalcCameraToViewportPoint(int _charaNo)
	{
		Camera camera = SingletonCustom<Scuba_CharacterManager>.Instance.GetCamera(_charaNo);
		for (int i = 0; i < items.Length; i++)
		{
			items[i].SetViewportPoint(_charaNo, camera.WorldToViewportPoint(items[i].GetCenterPos()));
		}
	}
	public void CalcCameraToScreenPoint(int _charaNo)
	{
		Camera camera = SingletonCustom<Scuba_CharacterManager>.Instance.GetCamera(_charaNo);
		for (int i = 0; i < items.Length; i++)
		{
			items[i].SetScreenPoint(_charaNo, camera.WorldToScreenPoint(items[i].GetCenterPos()));
		}
	}
	public void CalcCharaEyeVectors(int _charaNo)
	{
		Transform eyeAnchor = SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(_charaNo).GetEyeAnchor();
		Vector3 up = eyeAnchor.up;
		Vector3 right = eyeAnchor.right;
		Vector3 forward = eyeAnchor.forward;
		for (int i = 0; i < items.Length; i++)
		{
			Vector3 lhs = items[i].GetCenterPos() - eyeAnchor.position;
			items[i].SetCharaEyeVectors(_charaNo, new Vector3(Vector3.Dot(lhs, right), Vector3.Dot(lhs, up), Vector3.Dot(lhs, forward)));
		}
	}
	public void CalcCharaCameraToScreenPoint(int _charaNo)
	{
		Camera tpsPictureCamera = SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(_charaNo).TpsPictureCamera;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].SetCharaViewportPoint(_charaNo, tpsPictureCamera.WorldToViewportPoint(items[i].GetCenterPos()));
		}
	}
	public bool CheckInRangeViewport(Vector3 _viewportPoint)
	{
		if (VIEWPORT_FOUND_RANGE_MIN.x <= _viewportPoint.x && _viewportPoint.x <= VIEWPORT_FOUND_RANGE_MAX.x && VIEWPORT_FOUND_RANGE_MIN.y <= _viewportPoint.y && _viewportPoint.y <= VIEWPORT_FOUND_RANGE_MAX.y && VIEWPORT_FOUND_RANGE_MIN.z <= _viewportPoint.z)
		{
			return _viewportPoint.z <= VIEWPORT_FOUND_RANGE_MAX.z;
		}
		return false;
	}
	public bool CheckInRangeScreen(Vector3 _screenPoint)
	{
		if (SCREEN_FOUND_RANGE_MIN.x <= _screenPoint.x && _screenPoint.x <= SCREEN_FOUND_RANGE_MAX.x && SCREEN_FOUND_RANGE_MIN.y <= _screenPoint.y && _screenPoint.y <= SCREEN_FOUND_RANGE_MAX.y && SCREEN_FOUND_RANGE_MIN.z <= _screenPoint.z)
		{
			return _screenPoint.z <= SCREEN_FOUND_RANGE_MAX.z;
		}
		return false;
	}
	public bool CheckInRangeCharaEye(Vector3 _charaEyeVec)
	{
		if (CHARA_EYE_FOUND_RANGE_MIN.x <= _charaEyeVec.x && _charaEyeVec.x <= CHARA_EYE_FOUND_RANGE_MAX.x && CHARA_EYE_FOUND_RANGE_MIN.y <= _charaEyeVec.y && _charaEyeVec.y <= CHARA_EYE_FOUND_RANGE_MAX.y && CHARA_EYE_FOUND_RANGE_MIN.z <= _charaEyeVec.z)
		{
			return _charaEyeVec.z <= CHARA_EYE_FOUND_RANGE_MAX.z;
		}
		return false;
	}
	public bool CheckInRangeAiViewport(Vector3 _viewportPoint)
	{
		if (AI_VIEWPORT_RANGE_MIN.x <= _viewportPoint.x && _viewportPoint.x <= AI_VIEWPORT_RANGE_MAX.x && AI_VIEWPORT_RANGE_MIN.y <= _viewportPoint.y && _viewportPoint.y <= AI_VIEWPORT_RANGE_MAX.y && AI_VIEWPORT_RANGE_MIN.z <= _viewportPoint.z)
		{
			return _viewportPoint.z <= AI_VIEWPORT_RANGE_MAX.z;
		}
		return false;
	}
	public Transform GetRandomFishTrans()
	{
		return fishArray[UnityEngine.Random.Range(0, fishArray.Length)].transform;
	}
	public void AddLODSize(float _addSize)
	{
		for (int i = 0; i < fishArray.Length; i++)
		{
			fishArray[i].AddLODSize(_addSize);
		}
		for (int j = 0; j < bigFishArray.Length; j++)
		{
			bigFishArray[j].AddLODSize(_addSize);
		}
	}
	private void SortItemList_Score(List<Scuba_ItemObject> _list)
	{
		for (int i = 0; i < _list.Count - 1; i++)
		{
			for (int num = _list.Count - 1; num > i; num--)
			{
				if (_list[num - 1].GetScore() < _list[num].GetScore())
				{
					Scuba_ItemObject value = _list[num - 1];
					_list[num - 1] = _list[num];
					_list[num] = value;
				}
			}
		}
	}
	private void SortItemList_ViewportZ(int _charaNo, List<Scuba_ItemObject> _list)
	{
		for (int i = 0; i < _list.Count - 1; i++)
		{
			for (int num = _list.Count - 1; num > i; num--)
			{
				if (_list[num - 1].GetViewportPoint(_charaNo).z > _list[num].GetViewportPoint(_charaNo).z)
				{
					Scuba_ItemObject value = _list[num - 1];
					_list[num - 1] = _list[num];
					_list[num] = value;
				}
			}
		}
	}
	private void SortItemList_CharaViewportZ(int _charaNo, List<Scuba_ItemObject> _list)
	{
		for (int i = 0; i < _list.Count - 1; i++)
		{
			for (int num = _list.Count - 1; num > i; num--)
			{
				if (_list[num - 1].GetCharaViewportPoint(_charaNo).z > _list[num].GetCharaViewportPoint(_charaNo).z)
				{
					Scuba_ItemObject value = _list[num - 1];
					_list[num - 1] = _list[num];
					_list[num] = value;
				}
			}
		}
	}
}
