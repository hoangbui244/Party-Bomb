using System;
using System.Collections.Generic;
using UnityEngine;
public class Fishing_FishShadows : SingletonCustom<Fishing_FishShadows>
{
	public enum FishingAreaType
	{
		SmallWaterFall,
		DeepPool,
		SoftFlow,
		SoftFlow_2
	}
	[Serializable]
	public struct FishingAreaData
	{
		[Header("釣り場の範囲開始座標")]
		public Vector3 fishArea_Start;
		[Header("釣り場の範囲終了座標")]
		public Vector3 fishArea_End;
		[Header("釣り場の種類")]
		public FishingAreaType fishingAreaType;
		[Header("魚生成範囲間隔")]
		public int fishArea_Interval;
		[Header("魚の生成限界値")]
		public int fishCreateLimitCount;
		[Header("残り時間30秒以内での魚の生成限界値")]
		public int fishFewTimeCreateLimitCount;
		[Header("魚影生成範囲リスト")]
		public List<Vector3> fishCreateAreaPosList;
		[Header("魚影の管理用リスト")]
		public List<Fishing_FishShadow> createFishShadowList;
		[Header("ギズモ")]
		public bool debugGizmos;
	}
	[SerializeField]
	[Header("魚影オブジェクト")]
	private Fishing_FishShadow fishShadow;
	[SerializeField]
	[Header("魚影のアンカ\u30fc")]
	private Transform fishShadowAnchor;
	[SerializeField]
	[Header("魚が釣れる範囲デ\u30fcタ")]
	private FishingAreaData[] fishingAreaDatas;
	[SerializeField]
	[Header("魚影からの立ち位置を計算するオブジェクト")]
	private Fishing_CalcStandFishPoint[] fishing_CalcStandFishPoints;
	private const int LARGE_SIZE_FISH_LIMIT = 3;
	private int largeSizeFishCnt;
	private const int GARBAGE_FISH_LIMIT = 7;
	private int garbageFishCnt;
	private const float FEW_TIME_LIMIT = 30f;
	private int instantiateFishIDSettingNo;
	private float createAreaSize_X;
	private float createAreaSize_Z;
	private float createAreaInterval_X;
	private float createAreaInterval_Z;
	private RaycastHit hit;
	private float boxCastsize = 0.1f;
	private float boxCastDistance = 30f;
	private int randomIdx;
	private Vector3[] fishShadowPointArray = new Vector3[4];
	private List<Vector3>[] fishShadowStandPointList = new List<Vector3>[4];
	public void Init()
	{
		SetCreateAreaData();
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			for (int j = 0; j < fishingAreaDatas[i].createFishShadowList.Count; j++)
			{
				if (fishingAreaDatas[i].createFishShadowList[j] != null)
				{
					fishingAreaDatas[i].createFishShadowList[j].UpdateMethod();
					if (!fishingAreaDatas[i].createFishShadowList[j].IsActive())
					{
						UnityEngine.Object.Destroy(fishingAreaDatas[i].createFishShadowList[j].gameObject);
						fishingAreaDatas[i].createFishShadowList.RemoveAt(j);
					}
				}
			}
		}
		CheckGarbageFishCount();
		CheckLargeFishCount();
		CreateFishShadow();
	}
	private void SetCreateAreaData()
	{
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			fishingAreaDatas[i].fishCreateAreaPosList = new List<Vector3>();
			UnityEngine.Debug.Log("検索間隔：" + fishingAreaDatas[i].fishArea_Interval.ToString());
			if ((fishingAreaDatas[i].fishArea_Start.x > 0f && fishingAreaDatas[i].fishArea_End.x > 0f) || (fishingAreaDatas[i].fishArea_Start.x < 0f && fishingAreaDatas[i].fishArea_End.x < 0f))
			{
				createAreaSize_X = fishingAreaDatas[i].fishArea_Start.x - fishingAreaDatas[i].fishArea_End.x;
				createAreaSize_X *= Mathf.Sign(createAreaSize_X);
			}
			else
			{
				createAreaSize_X = fishingAreaDatas[i].fishArea_Start.x * Mathf.Sign(fishingAreaDatas[i].fishArea_Start.x) + fishingAreaDatas[i].fishArea_End.x * Mathf.Sign(fishingAreaDatas[i].fishArea_End.x);
			}
			if ((fishingAreaDatas[i].fishArea_Start.z > 0f && fishingAreaDatas[i].fishArea_End.z > 0f) || (fishingAreaDatas[i].fishArea_Start.z < 0f && fishingAreaDatas[i].fishArea_End.z < 0f))
			{
				createAreaSize_Z = fishingAreaDatas[i].fishArea_Start.z - fishingAreaDatas[i].fishArea_End.z;
				createAreaSize_Z *= Mathf.Sign(createAreaSize_Z);
			}
			else
			{
				createAreaSize_Z = fishingAreaDatas[i].fishArea_Start.z * Mathf.Sign(fishingAreaDatas[i].fishArea_Start.z) + fishingAreaDatas[i].fishArea_End.z * Mathf.Sign(fishingAreaDatas[i].fishArea_End.z);
			}
			UnityEngine.Debug.Log("X範囲：" + createAreaSize_X.ToString());
			UnityEngine.Debug.Log("Z範囲：" + createAreaSize_Z.ToString());
			createAreaInterval_X = createAreaSize_X / (float)fishingAreaDatas[i].fishArea_Interval;
			createAreaInterval_Z = createAreaSize_Z / (float)fishingAreaDatas[i].fishArea_Interval;
			UnityEngine.Debug.Log("X間隔：" + createAreaInterval_X.ToString());
			UnityEngine.Debug.Log("Z間隔：" + createAreaInterval_Z.ToString());
			for (int j = 0; j < fishingAreaDatas[i].fishArea_Interval + 1; j++)
			{
				for (int k = 0; k < fishingAreaDatas[i].fishArea_Interval + 1; k++)
				{
					CalcManager.mCalcVector3 = base.transform.position + new Vector3(fishingAreaDatas[i].fishArea_Start.x + createAreaInterval_X * (float)k, 5f, fishingAreaDatas[i].fishArea_Start.z - createAreaInterval_Z * (float)j);
					if (Physics.BoxCast(CalcManager.mCalcVector3, Vector3.one * boxCastsize, Vector3.down, out hit, base.transform.rotation, boxCastDistance, LayerMask.GetMask(FishingDefinition.LayerField)) && hit.collider.gameObject.tag == FishingDefinition.TagObject)
					{
						fishingAreaDatas[i].fishCreateAreaPosList.Add(CalcManager.mCalcVector3);
					}
				}
			}
		}
		FishShadowInit();
	}
	private void FishShadowInit()
	{
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			if (fishingAreaDatas[i].createFishShadowList.Count != 0)
			{
				for (int j = 0; j < fishingAreaDatas[i].createFishShadowList.Count; j++)
				{
					UnityEngine.Object.Destroy(fishingAreaDatas[i].createFishShadowList[j].gameObject);
					fishingAreaDatas[i].createFishShadowList.RemoveAt(j);
				}
			}
		}
		for (int k = 0; k < fishingAreaDatas.Length; k++)
		{
			fishingAreaDatas[k].createFishShadowList = new List<Fishing_FishShadow>();
			for (int l = 0; l < fishingAreaDatas[k].fishCreateLimitCount; l++)
			{
				if (fishingAreaDatas[k].fishCreateAreaPosList.Count != 0)
				{
					fishingAreaDatas[k].createFishShadowList.Add(UnityEngine.Object.Instantiate(fishShadow, Vector3.zero, Quaternion.identity, fishShadowAnchor));
					randomIdx = UnityEngine.Random.Range(0, fishingAreaDatas[k].fishCreateAreaPosList.Count);
					fishingAreaDatas[k].createFishShadowList[l].transform.position = new Vector3(fishingAreaDatas[k].fishCreateAreaPosList[randomIdx].x, fishingAreaDatas[k].fishCreateAreaPosList[randomIdx].y, fishingAreaDatas[k].fishCreateAreaPosList[randomIdx].z);
					fishingAreaDatas[k].createFishShadowList[l].transform.SetLocalPositionY(fishingAreaDatas[k].fishArea_Start.y);
					fishingAreaDatas[k].createFishShadowList[l].gameObject.SetActive(value: true);
					fishingAreaDatas[k].createFishShadowList[l].Init(instantiateFishIDSettingNo, FishingDefinition.FDM.GetRandomSelectFishData(_isMidiumContain: true, _isLargeContain: false, _isGarbage: false), fishingAreaDatas[k].fishArea_Start + base.transform.position, fishingAreaDatas[k].fishArea_End + base.transform.position);
					fishingAreaDatas[k].createFishShadowList[l].name = "FishShadow_ID[" + instantiateFishIDSettingNo.ToString() + "]";
					instantiateFishIDSettingNo++;
				}
			}
		}
	}
	private void CreateFishShadow()
	{
		if (FishingDefinition.MGM.GetGameTime() < 30f)
		{
			for (int i = 0; i < fishingAreaDatas.Length; i++)
			{
				if (fishingAreaDatas[i].createFishShadowList.Count < fishingAreaDatas[i].fishFewTimeCreateLimitCount)
				{
					fishingAreaDatas[i].createFishShadowList.Add(UnityEngine.Object.Instantiate(fishShadow, Vector3.zero, Quaternion.identity, fishShadowAnchor));
					randomIdx = UnityEngine.Random.Range(0, GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishCreateAreaPosList.Count);
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].transform.position = new Vector3(GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishCreateAreaPosList[randomIdx].x, GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishCreateAreaPosList[randomIdx].y, GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishCreateAreaPosList[randomIdx].z);
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].transform.SetLocalPositionY(fishingAreaDatas[i].fishArea_Start.y);
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].gameObject.SetActive(value: true);
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].Init(instantiateFishIDSettingNo, FishingDefinition.FDM.GetRandomSelectFishData(_isMidiumContain: true, largeSizeFishCnt < 3, garbageFishCnt < 7), GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishArea_Start + base.transform.position, GetFishingAreaData(fishingAreaDatas[i].fishingAreaType).fishArea_End + base.transform.position);
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].name = "FishShadow_ID[" + instantiateFishIDSettingNo.ToString() + "]";
					fishingAreaDatas[i].createFishShadowList[fishingAreaDatas[i].createFishShadowList.Count - 1].FishShadowFadeIn();
					instantiateFishIDSettingNo++;
				}
			}
			return;
		}
		for (int j = 0; j < fishingAreaDatas.Length; j++)
		{
			if (fishingAreaDatas[j].createFishShadowList.Count != 0 && fishingAreaDatas[j].createFishShadowList.Count < fishingAreaDatas[j].fishCreateLimitCount)
			{
				fishingAreaDatas[j].createFishShadowList.Add(UnityEngine.Object.Instantiate(fishShadow, Vector3.zero, Quaternion.identity, fishShadowAnchor));
				randomIdx = UnityEngine.Random.Range(0, GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishCreateAreaPosList.Count);
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].transform.position = new Vector3(GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishCreateAreaPosList[randomIdx].x, GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishCreateAreaPosList[randomIdx].y, GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishCreateAreaPosList[randomIdx].z);
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].transform.SetLocalPositionY(fishingAreaDatas[j].fishArea_Start.y);
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].gameObject.SetActive(value: true);
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].Init(instantiateFishIDSettingNo, FishingDefinition.FDM.GetRandomSelectFishData(_isMidiumContain: true, largeSizeFishCnt < 3, garbageFishCnt < 7), GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishArea_Start + base.transform.position, GetFishingAreaData(fishingAreaDatas[j].fishingAreaType).fishArea_End + base.transform.position);
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].name = "FishShadow_ID[" + instantiateFishIDSettingNo.ToString() + "]";
				fishingAreaDatas[j].createFishShadowList[fishingAreaDatas[j].createFishShadowList.Count - 1].FishShadowFadeIn();
				instantiateFishIDSettingNo++;
			}
		}
	}
	private void CheckGarbageFishCount()
	{
		garbageFishCnt = 0;
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			for (int j = 0; j < fishingAreaDatas[i].createFishShadowList.Count; j++)
			{
				if (fishingAreaDatas[i].createFishShadowList[j].GetFishSizeType() == FishingDefinition.FishSizeType.Garbage)
				{
					garbageFishCnt++;
				}
			}
		}
	}
	private void CheckLargeFishCount()
	{
		largeSizeFishCnt = 0;
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			for (int j = 0; j < fishingAreaDatas[i].createFishShadowList.Count; j++)
			{
				if (fishingAreaDatas[i].createFishShadowList[j].GetFishSizeType() == FishingDefinition.FishSizeType.Large)
				{
					largeSizeFishCnt++;
				}
			}
		}
	}
	private FishingAreaData GetFishingAreaData(FishingAreaType _fishingAreaType)
	{
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			if (fishingAreaDatas[i].fishingAreaType == _fishingAreaType)
			{
				return fishingAreaDatas[i];
			}
		}
		UnityEngine.Debug.LogError("指定された釣り場の情報は存在しません！");
		return fishingAreaDatas[0];
	}
	public bool CheckFishShadowAllowFishing(int _userDataNo, FishingDefinition.AiStrength _aiStrength)
	{
		int num = UnityEngine.Random.Range(0, 100);
		switch (_aiStrength)
		{
		case FishingDefinition.AiStrength.Weak:
			if (num > 90)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList.Count)].GetPos();
			}
			else if (num > 70)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList.Count)].GetPos();
			}
			else
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList.Count)].GetPos();
			}
			break;
		case FishingDefinition.AiStrength.Normal:
			if (num > 75)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList.Count)].GetPos();
			}
			else if (num > 50)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList.Count)].GetPos();
			}
			else
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList.Count)].GetPos();
			}
			break;
		case FishingDefinition.AiStrength.Strong:
			if (num > 50)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SmallWaterFall).createFishShadowList.Count)].GetPos();
			}
			else if (num > 25)
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.DeepPool).createFishShadowList.Count)].GetPos();
			}
			else
			{
				fishShadowPointArray[_userDataNo] = GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList[UnityEngine.Random.Range(0, GetFishingAreaData(FishingAreaType.SoftFlow).createFishShadowList.Count)].GetPos();
			}
			break;
		}
		if (CheckFishShadowStandPoint(_userDataNo, fishShadowPointArray[_userDataNo]))
		{
			return true;
		}
		return false;
	}
	private bool CheckFishShadowStandPoint(int _userDataNo, Vector3 _targetFishPoint)
	{
		fishing_CalcStandFishPoints[_userDataNo].Init(_userDataNo, _targetFishPoint);
		fishShadowStandPointList[_userDataNo] = fishing_CalcStandFishPoints[_userDataNo].GetStandFishPoint();
		return fishShadowStandPointList[_userDataNo].Count != 0;
	}
	public bool CheckObstacleStandPoint(int _userDataNo, Vector3 _fishStandPoint)
	{
		return fishing_CalcStandFishPoints[_userDataNo].CheckObstacleStandPoint(_fishStandPoint);
	}
	public Vector3 GetFishShadowPoint(int _userDataNo)
	{
		return fishShadowPointArray[_userDataNo];
	}
	public List<Vector3> GetFishShadowStandPoint(int _userDataNo)
	{
		return fishShadowStandPointList[_userDataNo];
	}
	private void OnDrawGizmos()
	{
		if (fishingAreaDatas == null)
		{
			return;
		}
		for (int i = 0; i < fishingAreaDatas.Length; i++)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position + fishingAreaDatas[i].fishArea_Start, 0.05f);
			Gizmos.DrawWireSphere(base.transform.position + fishingAreaDatas[i].fishArea_End, 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(base.transform.position + (fishingAreaDatas[i].fishArea_Start + fishingAreaDatas[i].fishArea_End) * 0.5f, fishingAreaDatas[i].fishArea_Start - fishingAreaDatas[i].fishArea_End);
			Gizmos.color = Color.white;
			if (fishingAreaDatas[i].fishCreateAreaPosList != null && fishingAreaDatas[i].debugGizmos)
			{
				for (int j = 0; j < fishingAreaDatas[i].fishCreateAreaPosList.Count; j++)
				{
					Gizmos.DrawWireCube(fishingAreaDatas[i].fishCreateAreaPosList[j], Vector3.one * boxCastsize);
				}
			}
		}
	}
}
