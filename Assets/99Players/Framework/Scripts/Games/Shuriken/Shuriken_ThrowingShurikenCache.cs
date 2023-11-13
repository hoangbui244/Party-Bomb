using System;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ThrowingShurikenCache : SingletonMonoBehaviour<Shuriken_ThrowingShurikenCache>
{
	[SerializeField]
	[DisplayName("オリジナル")]
	private Shuriken_ThrowingShuriken original;
	[SerializeField]
	[DisplayName("初期生成キャッシュ数")]
	private int generateCount = 20;
	private Shuriken_ThrowingShuriken[] shurikens;
	private int nextIndex;
	public void Initialize()
	{
		CacheShurikens();
	}
	public Shuriken_ThrowingShuriken GetShuriken()
	{
		return GetUnusedShuriken();
	}
	private void CacheShurikens()
	{
		shurikens = new Shuriken_ThrowingShuriken[generateCount];
		Transform parent = original.transform.parent;
		for (int i = 0; i < shurikens.Length; i++)
		{
			Shuriken_ThrowingShuriken shuriken_ThrowingShuriken = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false);
			shuriken_ThrowingShuriken.Initialize();
			shuriken_ThrowingShuriken.name = original.name;
			shurikens[i] = shuriken_ThrowingShuriken;
		}
		original.gameObject.SetActive(value: false);
	}
	private Shuriken_ThrowingShuriken GetUnusedShuriken()
	{
		int num = 0;
		do
		{
			Shuriken_ThrowingShuriken shuriken_ThrowingShuriken = shurikens[nextIndex];
			if (!shuriken_ThrowingShuriken.Used)
			{
				return shuriken_ThrowingShuriken;
			}
			nextIndex++;
			if (nextIndex >= shurikens.Length)
			{
				nextIndex = 0;
			}
			num++;
		}
		while (num >= shurikens.Length);
		nextIndex = shurikens.Length;
		ExtendCache();
		Shuriken_ThrowingShuriken result = shurikens[nextIndex];
		nextIndex++;
		return result;
	}
	private void ExtendCache()
	{
		Shuriken_ThrowingShuriken[] array = new Shuriken_ThrowingShuriken[shurikens.Length + generateCount];
		Array.Copy(shurikens, array, shurikens.Length);
		Transform parent = original.transform.parent;
		for (int i = shurikens.Length; i < array.Length; i++)
		{
			Shuriken_ThrowingShuriken shuriken_ThrowingShuriken = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false);
			shuriken_ThrowingShuriken.Initialize();
			shuriken_ThrowingShuriken.name = original.name;
			array[i] = shuriken_ThrowingShuriken;
		}
		shurikens = array;
	}
}
