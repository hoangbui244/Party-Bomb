using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Extension;
public class SmeltFishing_Field : SingletonCustom<SmeltFishing_Field>
{
	private int MaxTrialHoleGenerate = 1000;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private SmeltFishing_FieldConfig config;
	[SerializeField]
	[DisplayName("氷の横のタイリング数")]
	private int width;
	[SerializeField]
	[DisplayName("氷の縦のタイリング数")]
	private int height;
	[SerializeField]
	[DisplayName("タイリングされた氷")]
	private SmeltFishing_IcePlate[] icePlates;
	[SerializeField]
	private NavMeshSurface surface;
	private int holeCount;
	[NonReorderable]
	private List<SmeltFishing_IcePlate> holedIcePlates = new List<SmeltFishing_IcePlate>();
	private float RANDOM_SELECT_ICE_PLATE_PROBABILITY;
	public void Init()
	{
		InitIcePlates();
		GenerateHole();
		surface.BuildNavMesh();
		for (int i = 0; i < icePlates.Length; i++)
		{
			if (icePlates[i].CanMakeHole)
			{
				icePlates[i].InitShowSmeltSadowCnt(holedIcePlates.Contains(icePlates[i]));
			}
		}
	}
	public void UpdateMethod()
	{
		foreach (SmeltFishing_IcePlate holedIcePlate in holedIcePlates)
		{
			holedIcePlate.UpdateMethod();
		}
		for (int i = 0; i < icePlates.Length; i++)
		{
			if (icePlates[i].CanMakeHole && !icePlates[i].IsUsing)
			{
				icePlates[i].UpdateMethodSmeltShadow(holedIcePlates.Contains(icePlates[i]));
			}
		}
	}
	public SmeltFishing_IcePlate GetIcePlate(Vector2Int position)
	{
		int num = position.y * width + position.x;
		return icePlates[num];
	}
	public SmeltFishing_IcePlate GetRandomIcePlateInit(SmeltFishing_IcePlate _beforeTargetIcePlate)
	{
		SmeltFishing_IcePlate smeltFishing_IcePlate;
		do
		{
			int index = UnityEngine.Random.Range(0, holedIcePlates.Count);
			smeltFishing_IcePlate = holedIcePlates[index];
		}
		while (smeltFishing_IcePlate.IsUsing || smeltFishing_IcePlate == _beforeTargetIcePlate);
		return smeltFishing_IcePlate;
	}
	public SmeltFishing_IcePlate GetRandomIcePlate(SmeltFishing_IcePlate _beforeTargetIcePlate, Vector3 _pos)
	{
		RANDOM_SELECT_ICE_PLATE_PROBABILITY = UnityEngine.Random.Range(SmeltFishing_Definition.RANDOM_SELECT_ICE_PLATE_PROBABILITY[(int)SingletonCustom<SmeltFishing_GameMain>.Instance.AIStrength] - 0.1f, SmeltFishing_Definition.RANDOM_SELECT_ICE_PLATE_PROBABILITY[(int)SingletonCustom<SmeltFishing_GameMain>.Instance.AIStrength] + 0.1f);
		RANDOM_SELECT_ICE_PLATE_PROBABILITY = Mathf.Clamp01(RANDOM_SELECT_ICE_PLATE_PROBABILITY);
		SmeltFishing_IcePlate[] array;
		if (UnityEngine.Random.Range(0f, 1f) <= RANDOM_SELECT_ICE_PLATE_PROBABILITY)
		{
			UnityEngine.Debug.Log("優先度が高い穴場を選ぶ場合 ");
			array = (from rec in holedIcePlates
				orderby rec.SmeltValue descending, CalcManager.Length(new Vector3(rec.transform.position.x, 0f, rec.transform.position.z), new Vector3(_pos.x, 0f, _pos.z))
				select rec).ToArray();
		}
		else
		{
			UnityEngine.Debug.Log("ランダムな場所を選ぶ場合 ");
			array = holedIcePlates.ToArray();
			array = CalcManager.ShuffleList(array);
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsUsing && !(array[i] == _beforeTargetIcePlate))
			{
				UnityEngine.Debug.Log("sortList[i] " + array[i]?.ToString());
				return array[i];
			}
		}
		return null;
	}
	private void InitIcePlates()
	{
		SmeltFishing_IcePlate[] array = icePlates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}
	private void GenerateHole()
	{
		int num = 0;
		int num2 = 0;
		holeCount = config.GetRandomHoleCount();
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
		while (num2 < holeCount)
		{
			num++;
			if (num > MaxTrialHoleGenerate)
			{
				break;
			}
			int x = UnityEngine.Random.Range(0, width);
			int y = UnityEngine.Random.Range(0, height);
			Vector2Int vector2Int = new Vector2Int(x, y);
			if (!hashSet.Contains(vector2Int))
			{
				SmeltFishing_IcePlate icePlate = GetIcePlate(vector2Int);
				if (icePlate.CanMakeHole)
				{
					RegisterBlackList(hashSet, vector2Int);
					icePlate.SetAsHoledIcePlate();
					holedIcePlates.Add(icePlate);
					num2++;
				}
			}
		}
		if (num >= MaxTrialHoleGenerate)
		{
			if (num2 <= config.MinHoleCount)
			{
				throw new Exception($"既定の回数まで生成を繰り返しましたが最低限必要な穴の生成が出来ませんでした\u3000生成した穴の数:{num2}");
			}
			UnityEngine.Debug.Log($"既定の回数まで生成を繰り返したため強制停止しました。 生成した穴の数:{num2} 生成予定だった穴の数:{holeCount}");
			holeCount = num2;
		}
	}
	private void RegisterBlackList(HashSet<Vector2Int> blackList, Vector2Int position)
	{
		blackList.Add(position);
		blackList.Add(new Vector2Int(position.x + 1, position.y));
		blackList.Add(new Vector2Int(position.x, position.y + 1));
		blackList.Add(new Vector2Int(position.x, position.y - 1));
		blackList.Add(new Vector2Int(position.x - 1, position.y));
	}
}
