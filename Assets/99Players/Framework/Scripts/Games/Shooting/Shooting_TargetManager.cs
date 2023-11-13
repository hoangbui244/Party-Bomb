using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Shooting_TargetManager : SingletonCustom<Shooting_TargetManager>
{
	public enum TargetType
	{
		TYPE_FALL,
		TYPE_KITE,
		TYPE_FIVE_KITES,
		TYPE_QUICK,
		STRING,
		TRAGET
	}
	public enum HitPointType
	{
		Weak,
		Normal,
		Bad
	}
	[SerializeField]
	private List<Shooting_Target> list;
	[SerializeField]
	[Header("的のプレファブ")]
	private GameObject[] targetPrefabs;
	private int quickKiteCount;
	private bool isFiveKite;
	private float quickKiteInstanceTime;
	private float parachuteInstanceTime = 5f;
	private float kiteInstanceTime;
	private float parachuteGenerateTime;
	private float kiteGenerateTime;
	[SerializeField]
	[Header("パラシュ\u30fcトの生成までの最長時間")]
	private float maxParachuteIntervalTime;
	[SerializeField]
	[Header("パラシュ\u30fcトの生成までの最短時間")]
	private float minParachuteIntervalTime;
	[SerializeField]
	[Header("凧の生成までの最長時間")]
	private float maxKiteIntervalTime;
	[SerializeField]
	[Header("凧の生成までの最短時間")]
	private float minKiteIntervalTime;
	[SerializeField]
	[Header("生成した的の親")]
	private GameObject parentObj;
	private float generationPos;
	private float memoryPos;
	public List<Shooting_Target> List => list;
	public int QuickKiteCount
	{
		get
		{
			return quickKiteCount;
		}
		set
		{
			quickKiteCount = value;
		}
	}
	public bool IsFiveKite
	{
		get
		{
			return isFiveKite;
		}
		set
		{
			isFiveKite = value;
		}
	}
	public void Init()
	{
		list = new List<Shooting_Target>();
		isFiveKite = false;
		memoryPos = -20f;
	}
	public void SecondGroupInit()
	{
	}
	public void UpdateMethod()
	{
		TargetGeneration();
	}
	private void TargetGeneration()
	{
		if (!isFiveKite)
		{
			ParachuteInstantiate();
			KiteInstantiate();
		}
		QuickKiteInstantiate();
		FiveKiteInstantiate();
	}
	private void ParachuteInstantiate()
	{
		if (parachuteInstanceTime >= parachuteGenerateTime)
		{
			generationPos = UnityEngine.Random.Range(-45f, 45f);
			if (memoryPos + 15f > generationPos && memoryPos - 15f < generationPos)
			{
				generationPos = UnityEngine.Random.Range(10, 45);
				return;
			}
			if (SingletonCustom<Shooting_GameManager>.Instance.Timer.RemainingTime <= 45f && 25f > generationPos && 25f < generationPos)
			{
				generationPos = UnityEngine.Random.Range(0, 45);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(targetPrefabs[0]);
			Shooting_Target component = gameObject.GetComponent<Shooting_Target>();
			list.Add(component);
			gameObject.transform.parent = parentObj.transform;
			gameObject.transform.localPosition = new Vector3(generationPos, 40f, UnityEngine.Random.Range(90f, 95f));
			memoryPos = generationPos;
			parachuteInstanceTime = 0f;
			parachuteGenerateTime = UnityEngine.Random.Range(minParachuteIntervalTime, maxParachuteIntervalTime);
		}
		else
		{
			parachuteInstanceTime += Time.deltaTime;
		}
	}
	private void KiteInstantiate()
	{
		if (kiteInstanceTime >= kiteGenerateTime)
		{
			generationPos = UnityEngine.Random.Range(-60f, -30f);
			GameObject gameObject = UnityEngine.Object.Instantiate(targetPrefabs[1]);
			Shooting_Target component = gameObject.GetComponent<Shooting_Target>();
			list.Add(component);
			gameObject.transform.parent = parentObj.transform;
			gameObject.transform.localPosition = new Vector3(generationPos, -60f, UnityEngine.Random.Range(130f, 150f));
			gameObject.transform.localRotation = Quaternion.Euler(350f, 5f, 0f);
			kiteInstanceTime = 0f;
			kiteGenerateTime = UnityEngine.Random.Range(minKiteIntervalTime, maxKiteIntervalTime);
		}
		else
		{
			kiteInstanceTime += Time.deltaTime;
		}
	}
	private void FiveKiteInstantiate()
	{
		if (!isFiveKite && SingletonCustom<Shooting_GameManager>.Instance.Timer.RemainingTime <= 20f)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(targetPrefabs[2]);
			Shooting_Target component = gameObject.GetComponent<Shooting_Target>();
			list.Add(component);
			gameObject.transform.parent = parentObj.transform;
			gameObject.transform.localPosition = new Vector3(0f, -150f, 80f);
			if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
			}
			isFiveKite = true;
		}
	}
	private void QuickKiteInstantiate()
	{
		if (quickKiteCount < 1)
		{
			if (quickKiteInstanceTime >= 15f)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(targetPrefabs[3]);
				gameObject.transform.parent = parentObj.transform;
				gameObject.transform.localPosition = new Vector3(0f, -80f, 200f);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(targetPrefabs[4]);
				gameObject2.transform.parent = parentObj.transform;
				Shooting_Target component = gameObject.GetComponent<Shooting_Target>();
				gameObject2.GetComponent<Shooting_Target>().Himo = component.Himo;
				gameObject2.transform.localPosition = new Vector3(0f, -80f, 200f);
				quickKiteInstanceTime = 0f;
			}
			else
			{
				quickKiteInstanceTime += Time.deltaTime;
			}
		}
	}
	public Shooting_Target SearchRandomTarget()
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].TargetOut)
			{
				list.RemoveAt(i);
			}
		}
		UnityEngine.Debug.Log("【list:】" + list.Count.ToString());
		if (list.Count > 0)
		{
			list = (from a in list
				orderby Guid.NewGuid()
				select a).ToList();
			return list[0];
		}
		return null;
	}
	internal bool CheckAllDropEnd()
	{
		throw new NotImplementedException();
	}
}
