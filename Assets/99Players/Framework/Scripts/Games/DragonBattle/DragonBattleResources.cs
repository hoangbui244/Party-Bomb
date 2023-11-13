using System;
using UnityEngine;
public class DragonBattleResources : SingletonCustom<DragonBattleResources>
{
	[Serializable]
	public struct sEffectList
	{
		public ParticleSystem[] playerAttack;
		public ParticleSystem[] playerAttackHit;
		public ParticleSystem[] playerCharge;
		public ParticleSystem[] enemyAttack;
		public ParticleSystem[] enemyAttackHit;
		public ParticleSystem[] enemyAttackStart;
	}
	[Serializable]
	public struct sObjectList
	{
		public GameObject[] rocks;
		public GameObject[] jewel;
	}
	[Serializable]
	public struct sColorList
	{
		public Color[] jewel;
		public Color[] player;
	}
	[Serializable]
	public struct sMaterialList
	{
		public Material[] mats;
	}
	[Serializable]
	public struct sMonsterData
	{
		public sMaterialList[] matList;
		public int GetRandomMatNo(DragonBattleDefine.ModelType _type)
		{
			return UnityEngine.Random.Range(0, matList[(int)_type].mats.Length);
		}
		public Material GetMat(DragonBattleDefine.ModelType _type, int _no)
		{
			return matList[(int)_type].mats[_no];
		}
	}
	[SerializeField]
	private sEffectList effectList;
	[SerializeField]
	private GameObject[] enemyList;
	[SerializeField]
	private sObjectList objectList;
	[SerializeField]
	private sColorList colorList;
	[SerializeField]
	private sMonsterData monsterData;
	public sEffectList EffectList => effectList;
	public GameObject[] EnemyList => enemyList;
	public sObjectList ObjectList => objectList;
	public sColorList ColorList => colorList;
	public sMonsterData MonsterData => monsterData;
}
