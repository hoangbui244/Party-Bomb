using UnityEngine;
public class MikoshiRacePointSpaceData : MonoBehaviour
{
	[SerializeField]
	private MikoshiRacePointSpaceScript[] pointSpaces;
	private int[] spaceInCharaCounts;
	private bool isInit;
	public void DataInit()
	{
		spaceInCharaCounts = new int[pointSpaces.Length];
		if (!isInit)
		{
			isInit = true;
			for (int i = 0; i < pointSpaces.Length; i++)
			{
				pointSpaces[i].spaceData = this;
				pointSpaces[i].spaceNo = i;
			}
		}
	}
	public void AddCount(int _spaceNo)
	{
		spaceInCharaCounts[_spaceNo]++;
	}
	public void RemoveCount(int _spaceNo)
	{
		spaceInCharaCounts[_spaceNo]--;
	}
	public Transform GetSpaceTrans(int _spaceNo)
	{
		return pointSpaces[_spaceNo].transform;
	}
	public int SearchNearestEmptySpaceNo(Vector3 _pos)
	{
		int num = 0;
		float num2 = (pointSpaces[num].transform.position - _pos).sqrMagnitude;
		if (spaceInCharaCounts[0] > 0)
		{
			num2 = 100000f;
		}
		for (int i = 1; i < pointSpaces.Length; i++)
		{
			if (spaceInCharaCounts[i] <= 0)
			{
				float sqrMagnitude = (pointSpaces[i].transform.position - _pos).sqrMagnitude;
				if (num2 > sqrMagnitude)
				{
					num = i;
					num2 = sqrMagnitude;
				}
			}
		}
		if (spaceInCharaCounts[num] > 0)
		{
			return 0;
		}
		return num;
	}
	public Transform SearchNearestEmptySpaceTrans(Vector3 _pos)
	{
		int num = 0;
		float num2 = (pointSpaces[num].transform.position - _pos).sqrMagnitude;
		if (spaceInCharaCounts[0] > 0)
		{
			num2 = 100000f;
		}
		for (int i = 1; i < pointSpaces.Length; i++)
		{
			if (spaceInCharaCounts[i] <= 0)
			{
				float sqrMagnitude = (pointSpaces[i].transform.position - _pos).sqrMagnitude;
				if (num2 > sqrMagnitude)
				{
					num = i;
					num2 = sqrMagnitude;
				}
			}
		}
		if (spaceInCharaCounts[num] > 0)
		{
			return pointSpaces[0].transform;
		}
		return pointSpaces[num].transform;
	}
}
