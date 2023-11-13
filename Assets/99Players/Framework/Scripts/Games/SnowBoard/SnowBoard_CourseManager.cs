using System;
using UnityEngine;
public class SnowBoard_CourseManager : SingletonCustom<SnowBoard_CourseManager>
{
	public GameObject[] arrayCheckPointAuto;
	private GameObject goalPoint;
	private int currentCheckPointLength;
	public int CheckPointLength => currentCheckPointLength;
	public void Init()
	{
		GetCheckPointAnchorAuto();
	}
	public Transform GetGoalPoint()
	{
		goalPoint = GameObject.Find("GoalChecker");
		return goalPoint.transform;
	}
	public void GetCheckPointAnchorAuto()
	{
		arrayCheckPointAuto = GameObject.FindGameObjectsWithTag("CheckPoint");
		currentCheckPointLength = arrayCheckPointAuto.Length;
		Array.Sort(arrayCheckPointAuto, YPositionComparison);
		Array.Reverse((Array)arrayCheckPointAuto);
		for (int i = 0; i < currentCheckPointLength; i++)
		{
			UnityEngine.Debug.Log("ソ\u30fcト後" + arrayCheckPointAuto[i]?.ToString());
		}
		UnityEngine.Debug.Log("arrayCheckPointAuto.Length=" + arrayCheckPointAuto.Length.ToString());
	}
	public int YPositionComparison(GameObject a, GameObject b)
	{
		if (a == null)
		{
			if (!(b == null))
			{
				return -1;
			}
			return 0;
		}
		if (b == null)
		{
			return 1;
		}
		float y = a.transform.position.y;
		float y2 = b.transform.position.y;
		return y.CompareTo(y2);
	}
	public Transform GetCheckPointAnchor(int idx)
	{
		return arrayCheckPointAuto[idx].transform;
	}
}
