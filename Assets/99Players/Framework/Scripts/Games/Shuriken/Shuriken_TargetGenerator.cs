using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_TargetGenerator : SingletonMonoBehaviour<Shuriken_TargetGenerator>
{
	[SerializeField]
	[DisplayName("ル\u30fcトオブジェクト")]
	private Transform root;
	[SerializeField]
	[DisplayName("出現する的")]
	private Shuriken_BasicTarget[] targets;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_TargetGeneratorConfig config;
	private List<int> deactivateTargets;
	private HashSet<int> activeTargets;
	private SimpleCircularQueue<int> generateHistories;
	private Dictionary<int, Vector3> targetScreenPointCache;
	private SortedList<float, Shuriken_BasicTarget> sorted = new SortedList<float, Shuriken_BasicTarget>();
	public void Initialize()
	{
		targets = root.GetComponentsInChildren<Shuriken_BasicTarget>();
		deactivateTargets = new List<int>(from x in Enumerable.Range(0, targets.Length)
			orderby Guid.NewGuid()
			select x);
		activeTargets = new HashSet<int>();
		generateHistories = new SimpleCircularQueue<int>(config.MinimumDisplayInterval);
		targetScreenPointCache = new Dictionary<int, Vector3>(config.MinimumDisplayTargets);
		Shuriken_BasicTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Initialize(this);
		}
		GameStart();
	}
	public void GameStart()
	{
		GenerateTargets();
	}
	public void NotifyHide(Shuriken_BasicTarget target)
	{
		int num = 0;
		while (true)
		{
			if (num < targets.Length)
			{
				if (!(targets[num] != target))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		generateHistories.Enqueue(num);
		activeTargets.Remove(num);
		deactivateTargets.Add(num);
		targetScreenPointCache.Remove(num);
	}
	public void GetCloserTargets(Vector3 point, int take, List<Shuriken_BasicTarget> list)
	{
		sorted.Clear();
		foreach (KeyValuePair<int, Vector3> item in targetScreenPointCache)
		{
			if (targets[item.Key].IsActive)
			{
				float key = Vector3.Distance(item.Value, point);
				sorted[key] = targets[item.Key];
			}
		}
		switch (SingletonMonoBehaviour<Shuriken_GameMain>.Instance.AIStrength)
		{
		case Shuriken_Definition.AIStrength.Easy:
			list.AddRange(from p in sorted.Take(Mathf.Min(take, sorted.Count))
				select p.Value);
			break;
		case Shuriken_Definition.AIStrength.Normal:
			if (UnityEngine.Random.value >= 0.5f)
			{
				list.AddRange(from p in sorted.Take(Mathf.Min(take, sorted.Count))
					select p.Value);
			}
			else
			{
				list.AddRange(from p in (from p in sorted
						where !p.Value.IsTargeted
						select p).Take(Mathf.Min(take, sorted.Count))
					select p.Value);
			}
			break;
		case Shuriken_Definition.AIStrength.Hard:
			list.AddRange(from p in (from p in sorted
					where !p.Value.IsTargeted
					select p).Take(Mathf.Min(take, sorted.Count))
				select p.Value);
			break;
		}
		sorted.Clear();
	}
	public void UpdateMethod()
	{
		GenerateTargets();
		CacheScreenPoint();
	}
	private void GenerateTargets()
	{
		int num = config.MinimumDisplayTargets - activeTargets.Count;
		for (int i = 0; i < num; i++)
		{
			int index = UnityEngine.Random.Range(0, deactivateTargets.Count);
			int num2 = deactivateTargets[index];
			int num3 = 0;
			while (generateHistories.Contains(num2) && num3 < 100)
			{
				index = UnityEngine.Random.Range(0, deactivateTargets.Count);
				num2 = deactivateTargets[index];
				num3++;
			}
			deactivateTargets.RemoveAt(index);
			activeTargets.Add(num2);
			targets[num2].Show();
		}
	}
	private void CacheScreenPoint()
	{
		targetScreenPointCache.Clear();
		foreach (int activeTarget in activeTargets)
		{
			Shuriken_BasicTarget shuriken_BasicTarget = targets[activeTarget];
			targetScreenPointCache.Add(activeTarget, SingletonMonoBehaviour<Shuriken_Camera>.Instance.WorldToFullHdScreenPoint(shuriken_BasicTarget.AimHint));
		}
	}
}
