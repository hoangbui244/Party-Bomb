using System.Collections.Generic;
using UnityEngine;
public class Shooting_BulletManager : SingletonCustom<Shooting_BulletManager>
{
	private readonly int MAX_LIST_COUNT = 30;
	private List<GameObject> bulletList = new List<GameObject>();
	private List<Shooting_Bullet>[] playerBulletList;
	private int mostRemainingGunNo;
	private int removeGunNo;
	private Shooting_Bullet removeBullet;
	[SerializeField]
	[Header("弾の親アンカ\u30fc")]
	private Transform bulletParentAnchor;
	public void Init()
	{
		mostRemainingGunNo = 0;
		playerBulletList = new List<Shooting_Bullet>[SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM];
		for (int i = 0; i < playerBulletList.Length; i++)
		{
			playerBulletList[i] = new List<Shooting_Bullet>();
		}
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < playerBulletList.Length; i++)
		{
			while (playerBulletList[i].Count > 0)
			{
				Remove(i);
			}
		}
	}
	public bool CheckMaxCount()
	{
		return bulletList.Count >= MAX_LIST_COUNT;
	}
	public void Add(int _gunNo, Shooting_Bullet _bullet)
	{
		playerBulletList[_gunNo].Add(_bullet);
		bulletList.Add(_bullet.gameObject);
		_bullet.transform.parent = bulletParentAnchor;
	}
	public void Remove(int _gunNo, int _idx = 0)
	{
		GameObject gameObject = bulletList[_idx].gameObject;
	}
}
