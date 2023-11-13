using System;
using UnityEngine;
public class GS_Bird : MonoBehaviour
{
	[SerializeField]
	[Header("鳥画像")]
	private SimpleAnim[] arrayBirdSp;
	private Vector3[] moveOffset;
	private bool isExec;
	private float checkTime;
	private DateTime dt;
	private int[] arrayUseBirdIdx;
	private int arrayUseBirdNum;
	public void SetMoveUnit()
	{
		checkTime = UnityEngine.Random.Range(3f, 7f);
		Shuffle(arrayUseBirdIdx);
		arrayUseBirdNum = UnityEngine.Random.Range(1, 5);
		for (int i = 0; i < arrayBirdSp.Length; i++)
		{
			moveOffset[i].x = UnityEngine.Random.Range(-1.25f, -1.55f);
			moveOffset[i].y = UnityEngine.Random.Range(0.75f, 1f);
			arrayBirdSp[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < arrayUseBirdNum; j++)
		{
			arrayBirdSp[arrayUseBirdIdx[j]].gameObject.SetActive(value: true);
		}
	}
	public void Shuffle(int[] arr)
	{
		for (int num = arr.Length - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			int num3 = arr[num];
			arr[num] = arr[num2];
			arr[num2] = num3;
		}
	}
	private void OnEnable()
	{
		arrayUseBirdIdx = new int[arrayBirdSp.Length];
		moveOffset = new Vector3[arrayBirdSp.Length];
		for (int i = 0; i < arrayBirdSp.Length; i++)
		{
			arrayBirdSp[i].Init();
			arrayUseBirdIdx[i] = i;
		}
		SetMoveUnit();
	}
	private void Update()
	{
		if (!isExec)
		{
			if (checkTime > 0f)
			{
				dt = SingletonCustom<GS_GameSelectManager>.Instance.GetDateTime();
				checkTime -= Time.deltaTime;
				return;
			}
			isExec = true;
			for (int i = 0; i < arrayBirdSp.Length; i++)
			{
				arrayBirdSp[i].Init();
				arrayBirdSp[i].gameObject.SetActive(value: true);
			}
			SetMoveUnit();
			return;
		}
		isExec = false;
		for (int j = 0; j < arrayBirdSp.Length; j++)
		{
			if (arrayBirdSp[j].gameObject.activeSelf)
			{
				isExec = true;
				arrayBirdSp[j].transform.AddLocalPositionX(moveOffset[j].x * Time.deltaTime);
				arrayBirdSp[j].transform.AddLocalPositionY(moveOffset[j].y * Time.deltaTime);
				if (arrayBirdSp[j].transform.localPosition.y > 6.8f)
				{
					arrayBirdSp[j].gameObject.SetActive(value: false);
				}
			}
		}
	}
}
