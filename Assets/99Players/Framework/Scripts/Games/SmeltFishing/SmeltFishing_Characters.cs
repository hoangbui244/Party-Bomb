using System;
using UnityEngine;
public class SmeltFishing_Characters : SingletonCustom<SmeltFishing_Characters>
{
	[SerializeField]
	private SmeltFishing_Character[] characters = Array.Empty<SmeltFishing_Character>();
	public void Init()
	{
		for (int i = 0; i < characters.Length; i++)
		{
			characters[i].Init(i, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
		}
	}
	public void GameStart()
	{
		SmeltFishing_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GameStart();
		}
	}
	public void GameEnd()
	{
		SmeltFishing_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GameEnd();
		}
	}
	public void UpdateMethod()
	{
		SmeltFishing_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateMethod();
		}
	}
	public void FixedUpdateMethod()
	{
		SmeltFishing_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FixedUpdateMethod();
		}
	}
	public SmeltFishing_Character GetPlayer(int no)
	{
		return characters[no];
	}
	public (int[], int[]) GetResult()
	{
		int[] array = new int[characters.Length];
		int[] array2 = new int[characters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = characters[i].Score;
			array2[i] = (int)characters[i].ControlUser;
		}
		return (array, array2);
	}
}
