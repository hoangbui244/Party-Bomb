using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RingToss_TargetManager : SingletonCustom<RingToss_TargetManager>
{
	[Serializable]
	public class CreateAnchorGroup
	{
		public Transform[] createAnchors;
		public bool[] isUseAnchors;
		public void Init()
		{
			isUseAnchors = new bool[createAnchors.Length];
		}
		public bool IsCanCreate()
		{
			int num = 0;
			for (int i = 0; i < isUseAnchors.Length; i++)
			{
				if (isUseAnchors[i])
				{
					num++;
				}
			}
			return num < 4;
		}
		public int[] GetEmptyAnchorIndexes()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < isUseAnchors.Length; i++)
			{
				if (!isUseAnchors[i])
				{
					list.Add(i);
				}
			}
			return list.ToArray();
		}
		public int GetRandomEmptyAnchorIndex()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < isUseAnchors.Length; i++)
			{
				if (!isUseAnchors[i])
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				return -1;
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
	}
	[SerializeField]
	[Header("再出現エフェクトプレハブ")]
	private ParticleSystem appearEffectPrefab;
	[SerializeField]
	[Header("再出現エフェクト生成アンカ\u30fc")]
	private Transform appearEffectAnchor;
	[SerializeField]
	[Header("シ\u30fcン内のタ\u30fcゲット(動く的除く)")]
	private RingToss_Target[] arrayTarget;
	[SerializeField]
	[Header("シ\u30fcン内の電車用タ\u30fcゲット")]
	private RingToss_Target[] arrayTrainTarget;
	[SerializeField]
	[Header("タ\u30fcゲット生成位置")]
	private CreateAnchorGroup[] createAnchorGroups;
	[SerializeField]
	[Header("電車")]
	private RingToss_Train train;
	private int heroToyShowCount;
	public void Init()
	{
		for (int i = 0; i < createAnchorGroups.Length; i++)
		{
			createAnchorGroups[i].Init();
		}
		for (int j = 0; j < arrayTarget.Length; j++)
		{
			arrayTarget[j].Init(j);
			arrayTarget[j].gameObject.SetActive(value: false);
		}
		for (int k = 0; k < arrayTrainTarget.Length; k++)
		{
			arrayTrainTarget[k].Init(k);
			arrayTrainTarget[k].gameObject.SetActive(value: false);
		}
		InitShowTarget();
		train.Init();
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < createAnchorGroups.Length; i++)
		{
			createAnchorGroups[i].Init();
		}
		for (int j = 0; j < arrayTarget.Length; j++)
		{
			arrayTarget[j].SecondGroupInit();
		}
		for (int k = 0; k < arrayTrainTarget.Length; k++)
		{
			arrayTrainTarget[k].SecondGroupInit();
		}
		InitShowTarget();
		train.Init();
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayTarget.Length; i++)
		{
			arrayTarget[i].UpdateMethod();
		}
		for (int j = 0; j < arrayTrainTarget.Length; j++)
		{
			arrayTrainTarget[j].UpdateMethod();
		}
		train.UpdateMethod();
	}
	private void InitShowTarget()
	{
		List<RingToss_Target> list = new List<RingToss_Target>();
		list.AddRange(arrayTarget);
		list = (from a in list
			orderby Guid.NewGuid()
			select a).ToList();
		int num = Mathf.Min(12, list.Count);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int index = i + num2;
			if (list[index].IsHeroToys && heroToyShowCount >= 2)
			{
				num2++;
				i--;
				continue;
			}
			if (list[index].IsHeroToys)
			{
				heroToyShowCount++;
			}
			SetTargetRandomCreatePos(list[index]);
			list[index].Show(_isInit: true);
		}
		for (int j = 0; j < arrayTrainTarget.Length; j++)
		{
			arrayTrainTarget[j].Show(_isInit: true);
		}
	}
	public void ShowRandomTarget()
	{
		List<RingToss_Target> list = new List<RingToss_Target>();
		for (int i = 0; i < arrayTarget.Length; i++)
		{
			if ((!arrayTarget[i].IsHeroToys || heroToyShowCount < 2) && arrayTarget[i].IsWaitShow)
			{
				list.Add(arrayTarget[i]);
			}
		}
		if (list.Count != 0)
		{
			RingToss_Target ringToss_Target = list[UnityEngine.Random.Range(0, list.Count)];
			if (ringToss_Target.IsHeroToys)
			{
				heroToyShowCount++;
			}
			SetTargetRandomCreatePos(ringToss_Target);
			ringToss_Target.Show(_isInit: false);
		}
	}
	public void HideHeroToy()
	{
		heroToyShowCount--;
	}
	private void SetTargetRandomCreatePos(RingToss_Target _target)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < createAnchorGroups.Length; i++)
		{
			if (createAnchorGroups[i].IsCanCreate())
			{
				list.Add(i);
			}
		}
		if (list.Count != 0)
		{
			int num = list[UnityEngine.Random.Range(0, list.Count)];
			int randomEmptyAnchorIndex = createAnchorGroups[num].GetRandomEmptyAnchorIndex();
			Vector3 position = createAnchorGroups[num].createAnchors[randomEmptyAnchorIndex].position;
			position += Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward * UnityEngine.Random.Range(0f, 1f);
			position = KeepDisntanceCreatePos(_target.TargetNo, position, 1.5f);
			_target.SetCreatePos(position);
			_target.CreateGroupNo = num;
			_target.CreateAnchorNo = randomEmptyAnchorIndex;
			SetCreateGroupUseFlag(num, randomEmptyAnchorIndex, _flag: true);
		}
	}
	public ParticleSystem GetAppearEffectPrefab()
	{
		return appearEffectPrefab;
	}
	public Transform GetAppearEffectAnchor()
	{
		return appearEffectAnchor;
	}
	public RingToss_Target GetTarget(int _targetNo)
	{
		return arrayTarget[_targetNo];
	}
	public void SetCreateGroupUseFlag(int _groupNo, int _anchorNo, bool _flag)
	{
		createAnchorGroups[_groupNo].isUseAnchors[_anchorNo] = _flag;
	}
	private Vector3 KeepDisntanceCreatePos(int _targetNo, Vector3 _createPos, float _keepDis)
	{
		float y = _createPos.y;
		for (int i = 0; i < arrayTarget.Length; i++)
		{
			if (i != _targetNo && arrayTarget[i].IsShow)
			{
				Vector3 pos = arrayTarget[i].GetPos();
				Vector3 vector = _createPos - pos;
				vector.y = 0f;
				if (vector.sqrMagnitude < _keepDis * _keepDis)
				{
					_createPos = pos + vector.normalized * _keepDis;
					_createPos.y = y;
					break;
				}
			}
		}
		return _createPos;
	}
	public RingToss_Target SearchRandomTarget()
	{
		List<RingToss_Target> list = new List<RingToss_Target>();
		for (int i = 0; i < arrayTarget.Length; i++)
		{
			if (arrayTarget[i].IsCanAiTarget)
			{
				list.Add(arrayTarget[i]);
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}
	public RingToss_Target SearchNearestTarget(Vector3 _pos, bool _isIgnoreGet = true)
	{
		int num = 0;
		Vector3 vector = arrayTarget[0].GetPos() - _pos;
		vector.y = 0f;
		float num2 = vector.sqrMagnitude;
		if (!arrayTarget[0].IsCanAiTarget)
		{
			num2 = 100000f;
		}
		for (int i = 1; i < arrayTarget.Length; i++)
		{
			if (arrayTarget[i].IsCanAiTarget)
			{
				vector = arrayTarget[i].GetPos() - _pos;
				vector.y = 0f;
				float sqrMagnitude = vector.sqrMagnitude;
				if (num2 > sqrMagnitude)
				{
					num2 = sqrMagnitude;
					num = i;
				}
			}
		}
		bool flag = false;
		for (int j = 0; j < arrayTrainTarget.Length; j++)
		{
			if (arrayTrainTarget[j].IsCanAiTarget)
			{
				vector = arrayTrainTarget[j].GetPos() - _pos;
				vector.y = 0f;
				float sqrMagnitude2 = vector.sqrMagnitude;
				if (num2 > sqrMagnitude2)
				{
					num2 = sqrMagnitude2;
					num = j;
					flag = true;
				}
			}
		}
		if (flag)
		{
			return arrayTrainTarget[num];
		}
		return arrayTarget[num];
	}
}
