using System;
using System.Collections.Generic;
using UnityEngine;
public class RingToss_RingManager : SingletonCustom<RingToss_RingManager>
{
	[Serializable]
	public class RingListData
	{
		public int nowRingNo;
		public List<RingToss_Ring> ringList;
	}
	[SerializeField]
	private RingToss_Ring ringPrefab;
	[SerializeField]
	private Material[] ringMaterials;
	[SerializeField]
	private Transform[] ringRemainAnchors_Move;
	[SerializeField]
	private Transform[] ringRemainAnchors_Rot;
	private Transform[] ringRemainAnchors;
	[SerializeField]
	private Transform ringTargetBottomAnchor;
	[SerializeField]
	private Transform ringThrowedAnchor;
	private RingListData[] ringListDatas;
	private int ringNum;
	public void Init()
	{
		ringRemainAnchors = ringRemainAnchors_Move;
		ringNum = SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum;
		ringListDatas = new RingListData[RingToss_Define.MAX_PLAYER_NUM];
		for (int i = 0; i < ringListDatas.Length; i++)
		{
			ringListDatas[i] = new RingListData();
			ringListDatas[i].nowRingNo = 0;
			ringListDatas[i].ringList = new List<RingToss_Ring>();
			SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(i);
			for (int j = 0; j < ringNum; j++)
			{
				Vector3 position = ringRemainAnchors[i].position;
				position.y += 0.2f * (float)(ringNum - j - 1);
				RingToss_Ring ringToss_Ring = UnityEngine.Object.Instantiate(ringPrefab, position, ringRemainAnchors[i].rotation, ringRemainAnchors[i]);
				ringListDatas[i].ringList.Add(ringToss_Ring);
				ringToss_Ring.Init(i);
				ringToss_Ring.SetMaterial(ringMaterials[i]);
			}
		}
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < ringListDatas.Length; i++)
		{
			ringListDatas[i].nowRingNo = 0;
			for (int j = 0; j < ringNum; j++)
			{
				ringListDatas[i].ringList[j].SecondGroupInit();
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < ringListDatas.Length; i++)
		{
			ringListDatas[i].ringList[ringListDatas[i].nowRingNo].UpdateMethod();
		}
	}
	private void FixedUpdate()
	{
		for (int i = 0; i < ringListDatas.Length; i++)
		{
			ringListDatas[i].ringList[ringListDatas[i].nowRingNo].FixedUpdateMethod();
		}
	}
	public Transform GetRingTargetBottomAnchor()
	{
		return ringTargetBottomAnchor;
	}
	public float GetRingTargetBottomPosY()
	{
		return ringTargetBottomAnchor.position.y;
	}
	public Transform GetRingThrowedAnchor()
	{
		return ringThrowedAnchor;
	}
	public RingToss_Ring GetRing(int _ctrlNo, int _ringNo)
	{
		return ringListDatas[_ctrlNo].ringList[_ringNo];
	}
	public RingToss_Ring GetNextRing(int _ctrlNo, bool _countUp = true)
	{
		int index = ringListDatas[_ctrlNo].nowRingNo + 1;
		if (_countUp)
		{
			ringListDatas[_ctrlNo].nowRingNo++;
		}
		return ringListDatas[_ctrlNo].ringList[index];
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < ringRemainAnchors_Move.Length; i++)
		{
			Vector3 position = ringRemainAnchors_Move[i].position;
			position.y += 0.2f;
			Gizmos.DrawWireSphere(position, 1.5f);
		}
		Gizmos.color = Color.yellow;
		for (int j = 0; j < ringRemainAnchors_Rot.Length; j++)
		{
			Vector3 position2 = ringRemainAnchors_Rot[j].position;
			position2.y += 0.2f;
			Gizmos.DrawWireSphere(position2, 1.5f);
		}
	}
}
