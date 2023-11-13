using System;
using System.Collections.Generic;
using UnityEngine;
public class DragonBattleFieldManager : SingletonCustom<DragonBattleFieldManager>
{
	[Serializable]
	public struct FieldParts
	{
		public DragonBattleFieldParts[] parts;
	}
	[SerializeField]
	[Header("地形パ\u30fcツプレハブ：段差毎に設定")]
	private FieldParts[] arrayFieldPartsStep;
	[SerializeField]
	[Header("壁地形パ\u30fcツ")]
	private DragonBattleFieldParts[] arrayFieldWall;
	[SerializeField]
	[Header("特殊：ボス")]
	private DragonBattleFieldParts fieldBoss;
	[SerializeField]
	[Header("ゴ\u30fcルパ\u30fcツプレハブ")]
	private DragonBattleFieldParts fieldGoal;
	[SerializeField]
	[Header("生成ル\u30fcト")]
	private GameObject rootField;
	[SerializeField]
	[Header("マスク")]
	private LayerMask mask;
	private static int FIELD_NUM = 21;
	public readonly float CREATE_SPAN = 20f;
	private List<DragonBattleFieldParts> listParts = new List<DragonBattleFieldParts>();
	private bool isRespawnExec;
	private int checkPartsIdx = -1;
	private int partsIdx = -1;
	private int nowPartsNo;
	private DragonBattleFieldParts nowParts;
	public bool IsRespawnExec => isRespawnExec;
	public int NowPartsNo => nowPartsNo;
	public DragonBattleFieldParts NowParts => nowParts;
	public bool IsBossBattle => SingletonCustom<DragonBattleCameraMover>.Instance.CheckState(DragonBattleCameraMover.State.BossBattle);
	public void Init()
	{
		DragonBattleEnemyNinja.bossEnemyTypeBuf = DragonBattleDefine.EnemyType.MAX;
		int num = 0;
		float num2 = CREATE_SPAN;
		for (int i = 0; i < FIELD_NUM; i++)
		{
			DragonBattleFieldParts dragonBattleFieldParts = null;
			if (i == FIELD_NUM - 1)
			{
				dragonBattleFieldParts = UnityEngine.Object.Instantiate(fieldGoal, rootField.transform);
			}
			else if (i == FIELD_NUM - 2)
			{
				partsIdx = UnityEngine.Random.Range(0, arrayFieldPartsStep[arrayFieldPartsStep.Length - 1].parts.Length);
				dragonBattleFieldParts = UnityEngine.Object.Instantiate(arrayFieldPartsStep[arrayFieldPartsStep.Length - 1].parts[partsIdx], rootField.transform);
			}
			else if (i == 6 || i == 13)
			{
				dragonBattleFieldParts = UnityEngine.Object.Instantiate(fieldBoss, rootField.transform);
			}
			else
			{
				for (partsIdx = UnityEngine.Random.Range(0, arrayFieldPartsStep[num].parts.Length); partsIdx == checkPartsIdx; partsIdx = UnityEngine.Random.Range(0, arrayFieldPartsStep[num].parts.Length))
				{
				}
				dragonBattleFieldParts = UnityEngine.Object.Instantiate(arrayFieldPartsStep[num].parts[partsIdx], rootField.transform);
				checkPartsIdx = partsIdx;
			}
			if (dragonBattleFieldParts != null)
			{
				dragonBattleFieldParts.Init(num);
				dragonBattleFieldParts.transform.SetLocalPositionZ(num2);
				if (i == FIELD_NUM - 1)
				{
					dragonBattleFieldParts.transform.AddLocalPositionZ(CREATE_SPAN * 0.5f);
				}
				dragonBattleFieldParts.gameObject.SetActive(value: true);
				listParts.Add(dragonBattleFieldParts);
				if (dragonBattleFieldParts.IsBossField)
				{
					UnityEngine.Debug.Log("No." + i.ToString() + " : ボスステ\u30fcジ");
					dragonBattleFieldParts.transform.SetLocalPositionZ(num2);
					num2 += CREATE_SPAN * 0.5f;
				}
				num2 += CREATE_SPAN;
			}
			if (i > 0 && i % 5 == 0)
			{
				num++;
			}
		}
		SingletonCustom<DragonBattleCameraMover>.Instance.SetEndPointZ(GetGoal().transform.localPosition.z - CREATE_SPAN);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < listParts.Count; i++)
		{
			listParts[i].UpdateMethod();
		}
	}
	public void LateUpdateMethod()
	{
		if (isRespawnExec)
		{
			isRespawnExec = false;
		}
	}
	public Vector3 GetRespawnPoint()
	{
		isRespawnExec = true;
		Vector3 position = SingletonCustom<DragonBattleCameraMover>.Instance.transform.position;
		position.z += 17f;
		List<Transform> list = new List<Transform>();
		bool flag = false;
		bool flag2 = false;
		float num = 99f;
		Transform transform = null;
		while (!flag)
		{
			transform = null;
			num = 99f;
			for (int i = 0; i < listParts.Count; i++)
			{
				for (int j = 0; j < listParts[i].ArrayRespawnAnchor.Length; j++)
				{
					if (!(num > Vector3.Distance(position, listParts[i].ArrayRespawnAnchor[j].position)))
					{
						continue;
					}
					flag2 = false;
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k] == listParts[i].ArrayRespawnAnchor[j])
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						num = Vector3.Distance(position, listParts[i].ArrayRespawnAnchor[j].position);
						transform = listParts[i].ArrayRespawnAnchor[j];
					}
				}
			}
			if ((bool)transform)
			{
				if (Physics.OverlapSphere(transform.position, 1.7f, mask).Length == 0)
				{
					flag = true;
					CalcManager.mCalcVector3 = transform.position;
				}
				else
				{
					list.Add(transform);
				}
			}
		}
		return CalcManager.mCalcVector3;
	}
	public DragonBattleFieldParts GetGoal()
	{
		return listParts[listParts.Count - 1];
	}
	public DragonBattleEnemyNinja CheckEnemy(GameObject _col)
	{
		for (int i = 0; i < listParts.Count; i++)
		{
			for (int j = 0; j < listParts[i].Enemys.Length; j++)
			{
				if ((bool)listParts[i].Enemys[j] && listParts[i].Enemys[j].CheckMy(_col))
				{
					return listParts[i].Enemys[j];
				}
			}
		}
		return null;
	}
	public void SetNowParts(DragonBattleFieldParts _parts)
	{
		nowParts = _parts;
	}
}
