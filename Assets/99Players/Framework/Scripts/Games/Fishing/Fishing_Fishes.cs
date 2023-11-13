using System.Collections.Generic;
using UnityEngine;
public class Fishing_Fishes : SingletonCustom<Fishing_Fishes>
{
	[SerializeField]
	[Header("魚のプレハブ")]
	private Fishing_FishData[] fishPrefab;
	private List<Fishing_FishData> randomSelectFishDataList = new List<Fishing_FishData>();
	public GameObject GetFishObject(FishingDefinition.FishType _fishType)
	{
		return fishPrefab[(int)_fishType].gameObject;
	}
	public FishingDefinition.FishSizeType GetFishSizeType(FishingDefinition.FishType _fishType)
	{
		return fishPrefab[(int)_fishType].fishSizeType;
	}
	public int GetFishPoint(FishingDefinition.FishType _fishType)
	{
		return fishPrefab[(int)_fishType].fishPoint;
	}
	public string GetFishName(FishingDefinition.FishType _fishType)
	{
		return fishPrefab[(int)_fishType].fishName;
	}
	public Fishing_FishData GetRandomSelectFishData(bool _isMidiumContain, bool _isLargeContain, bool _isGarbage)
	{
		randomSelectFishDataList.Clear();
		for (int i = 0; i < fishPrefab.Length; i++)
		{
			switch (fishPrefab[i].fishSizeType)
			{
			case FishingDefinition.FishSizeType.Small:
				randomSelectFishDataList.Add(fishPrefab[i]);
				break;
			case FishingDefinition.FishSizeType.Medium:
				if (_isMidiumContain)
				{
					randomSelectFishDataList.Add(fishPrefab[i]);
				}
				break;
			case FishingDefinition.FishSizeType.Large:
				if (_isLargeContain)
				{
					randomSelectFishDataList.Add(fishPrefab[i]);
				}
				break;
			case FishingDefinition.FishSizeType.Garbage:
				if (_isGarbage)
				{
					randomSelectFishDataList.Add(fishPrefab[i]);
				}
				break;
			}
		}
		return randomSelectFishDataList[Random.Range(0, randomSelectFishDataList.Count)];
	}
}
