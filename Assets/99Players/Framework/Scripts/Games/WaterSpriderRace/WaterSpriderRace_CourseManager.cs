using System;
using UnityEngine;
public class WaterSpriderRace_CourseManager : SingletonCustom<WaterSpriderRace_CourseManager>
{
	public GameObject[][] arrayCheckPointAuto;
	private GameObject goalPoint;
	public int[] currentCheckPointLength;
	public void Init()
	{
		arrayCheckPointAuto = new GameObject[WaterSpriderRace_Define.MEMBER_NUM][];
		currentCheckPointLength = new int[WaterSpriderRace_Define.MEMBER_NUM];
		GetCheckPointAnchorAuto();
	}
	public Transform GetGoalPoint()
	{
		goalPoint = GameObject.Find("GoalChecker");
		return goalPoint.transform;
	}
	public void GetCheckPointAnchorAuto()
	{
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			arrayCheckPointAuto[i] = GameObject.FindGameObjectsWithTag("CheckPoint_Player" + i.ToString());
			currentCheckPointLength[i] = arrayCheckPointAuto[i].Length;
			Array.Sort(arrayCheckPointAuto[i], ZPositionComparison);
		}
	}
	public int ZPositionComparison(GameObject a, GameObject b)
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
		float z = a.transform.position.z;
		float z2 = b.transform.position.z;
		return z.CompareTo(z2);
	}
	public Transform GetCheckPointAnchor(int player, int idx)
	{
		return arrayCheckPointAuto[player][idx].transform;
	}
}
