using System;
using UnityEngine;
public class HandSeal_EnemyManager : SingletonCustom<HandSeal_EnemyManager>
{
	[Serializable]
	public struct EnemyData
	{
		public EnemyType enemyType;
		public int point;
		public float speed;
	}
	public enum EnemyType
	{
		COMMON,
		COMMON_2,
		SPEED,
		SPEED_2,
		STRONG,
		STRONG_2,
		TOUGH,
		TOUGH_2,
		BOSS,
		BOSS_2,
		MAX
	}
	[SerializeField]
	[Header("敵の情報")]
	private EnemyData[] enemyDatas;
	[SerializeField]
	[Header("敵の種類設定(1ライン目)")]
	private EnemyType[] enemyType_1Line;
	[SerializeField]
	[Header("敵の種類設定(2ライン目)")]
	private EnemyType[] enemyType_2Line;
	[SerializeField]
	[Header("敵の種類設定(3ライン目)")]
	private EnemyType[] enemyType_3Line;
	public EnemyData GetEnemyData(int _line, int _no)
	{
		switch (_line)
		{
		case 0:
			for (int j = 0; j < enemyDatas.Length; j++)
			{
				if (enemyType_1Line.Length <= _no)
				{
					break;
				}
				if (enemyType_1Line[_no] == enemyDatas[j].enemyType)
				{
					return enemyDatas[j];
				}
			}
			break;
		case 1:
			for (int k = 0; k < enemyDatas.Length; k++)
			{
				if (enemyType_2Line.Length <= _no)
				{
					break;
				}
				if (enemyType_2Line[_no] == enemyDatas[k].enemyType)
				{
					return enemyDatas[k];
				}
			}
			break;
		case 2:
			for (int i = 0; i < enemyDatas.Length; i++)
			{
				if (enemyType_3Line.Length <= _no)
				{
					break;
				}
				if (enemyType_3Line[_no] == enemyDatas[i].enemyType)
				{
					return enemyDatas[i];
				}
			}
			break;
		}
		return enemyDatas[10];
	}
	public int AllEnemyCount()
	{
		return enemyType_1Line.Length - 1 + enemyType_2Line.Length - 1 + enemyType_3Line.Length - 1;
	}
}
