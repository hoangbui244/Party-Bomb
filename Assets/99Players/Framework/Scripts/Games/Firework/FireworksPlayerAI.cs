using System.Collections.Generic;
using UnityEngine;
public class FireworksPlayerAI : MonoBehaviour
{
	private enum MoveState
	{
		DEFAULT,
		DETOUR
	}
	private readonly int LAUNCH_TUBE_RACK_IDX_DIFF = 4;
	private readonly float CONTACE_DISTANCE = 0.25f;
	private readonly int LAYER_FIELD = 20;
	private readonly int LAYER_CHARACTER = 30;
	private FireworksPlayer currentPlayer;
	private Vector3 moveForce;
	private int currentWayIdx;
	private List<FireworksWayAnchor> listWay = new List<FireworksWayAnchor>();
	private MoveState moveState;
	private RaycastHit raycastHit;
	private Vector3 moveDiff;
	private float diffupdateTime;
	private Vector3 prevPos;
	private Vector3 prevForce;
	public void Init(FireworksPlayer _player)
	{
		currentPlayer = _player;
		FireworksWayAnchor[] arrayWay = SingletonCustom<FireworksWayAnchorManager>.Instance.ArrayWay;
		float num = 99f;
		for (int i = 0; i < arrayWay.Length; i++)
		{
			if (Vector3.Distance(base.transform.position, arrayWay[i].transform.position) <= num)
			{
				num = Vector3.Distance(base.transform.position, arrayWay[i].transform.position);
				currentWayIdx = i;
			}
		}
	}
	public Vector3 UpdateMoveForce()
	{
		return Vector3.zero;
	}
	public void LiftBall()
	{
	}
	public void SetBall()
	{
		if (listWay.Count > 0)
		{
			currentWayIdx = listWay[listWay.Count - 1].no;
		}
		listWay.Clear();
	}
	private void SearchTargetLaunchTubeRack()
	{
		int num = (int)(currentPlayer.ArrayBall[0].Color + LAUNCH_TUBE_RACK_IDX_DIFF);
		int num2 = currentWayIdx;
		FireworksWayAnchor[] arrayWay = SingletonCustom<FireworksWayAnchorManager>.Instance.ArrayWay;
		float num3 = 99f;
		int num4 = -1;
		bool flag = false;
		int num5 = 1000;
		listWay.Clear();
		do
		{
			if (num2 == num)
			{
				return;
			}
			num3 = 99f;
			num4 = -1;
			for (int i = 0; i < arrayWay[num2].ConnectWay.Length; i++)
			{
				flag = false;
				for (int j = 0; j < listWay.Count; j++)
				{
					if (i == listWay[j].no)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (arrayWay[num2].ConnectWay[i].no == num)
					{
						num4 = num;
						break;
					}
					if (Vector3.Distance(arrayWay[num2].ConnectWay[i].transform.position, arrayWay[num].transform.position) <= num3)
					{
						num3 = Vector3.Distance(arrayWay[num2].ConnectWay[i].transform.position, arrayWay[num].transform.position);
						num4 = arrayWay[num2].ConnectWay[i].no;
					}
				}
			}
			listWay.Add(arrayWay[num4]);
			num2 = num4;
			num5--;
		}
		while (num5 != 0);
		UnityEngine.Debug.LogError("★break");
	}
	private void SearchTargetSlatBox()
	{
		int num = (int)SingletonCustom<FireworksUIManager>.Instance.GetOrderList(currentPlayer).ArrayColorList[currentPlayer.HaveBallCount];
		int num2 = currentWayIdx;
		FireworksWayAnchor[] arrayWay = SingletonCustom<FireworksWayAnchorManager>.Instance.ArrayWay;
		float num3 = 99f;
		int num4 = 0;
		bool flag = false;
		int num5 = 1000;
		List<FireworksBall.ItemType> list = new List<FireworksBall.ItemType>();
		listWay.Clear();
		for (int i = 0; i < SingletonCustom<FireworksUIManager>.Instance.GetOrderList(currentPlayer).ArrayColorList.Length; i++)
		{
			list.Add(SingletonCustom<FireworksUIManager>.Instance.GetOrderList(currentPlayer).ArrayColorList[i]);
		}
		for (int j = 0; j < currentPlayer.HaveBallCount; j++)
		{
			list.Remove(currentPlayer.ArrayBall[j].Color);
		}
		num = (int)list[Random.Range(0, list.Count)];
		if (num2 == num)
		{
			num3 = 99f;
			for (int k = 0; k < arrayWay.Length; k++)
			{
				if (Vector3.Distance(base.transform.position, arrayWay[k].transform.position) <= num3)
				{
					num3 = Vector3.Distance(base.transform.position, arrayWay[k].transform.position);
					currentWayIdx = k;
				}
			}
			num2 = currentWayIdx;
		}
		do
		{
			if (num2 == num)
			{
				return;
			}
			num3 = 99f;
			num4 = -1;
			for (int l = 0; l < arrayWay[num2].ConnectWay.Length; l++)
			{
				flag = false;
				for (int m = 0; m < listWay.Count; m++)
				{
					if (arrayWay[num2].ConnectWay[l].no == listWay[m].no)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (arrayWay[num2].ConnectWay[l].no == num)
					{
						num4 = num;
						break;
					}
					if (Vector3.Distance(arrayWay[num2].ConnectWay[l].transform.position, arrayWay[num].transform.position) <= num3)
					{
						num3 = Vector3.Distance(arrayWay[num2].ConnectWay[l].transform.position, arrayWay[num].transform.position);
						num4 = arrayWay[num2].ConnectWay[l].no;
					}
				}
			}
			UnityEngine.Debug.Log("target:" + num.ToString());
			UnityEngine.Debug.Log("current:" + currentWayIdx.ToString());
			UnityEngine.Debug.Log("temp:" + num4.ToString());
			listWay.Add(arrayWay[num4]);
			num2 = num4;
			num5--;
		}
		while (num5 != 0);
		UnityEngine.Debug.LogError("★break");
	}
}
