using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Biathlon_Characters : SingletonCustom<Biathlon_Characters>
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();
		public static Func<Biathlon_Character, bool> _003C_003E9__3_0;
		public static Func<Biathlon_Character, bool> _003C_003E9__5_0;
		public static Func<Biathlon_Character, bool> _003C_003E9__7_0;
		internal bool _003Cget_IsCharacterAllGoal_003Eb__3_0(Biathlon_Character c)
		{
			return c.IsGoal;
		}
		internal bool _003Cget_IsPlayerAllGoal_003Eb__5_0(Biathlon_Character c)
		{
			if (c.IsPlayer)
			{
				return c.IsGoal;
			}
			return false;
		}
		internal bool _003Cget_IsLastOnePlayer_003Eb__7_0(Biathlon_Character c)
		{
			if (c.IsPlayer)
			{
				return c.IsGoal;
			}
			return false;
		}
	}
	[SerializeField]
	private Biathlon_Character[] characters;
	private int[] placements;
	public bool IsCharacterAllGoal => characters.All((Biathlon_Character c) => c.IsGoal);
	public bool IsPlayerAllGoal => characters.Count((Biathlon_Character c) => c.IsPlayer && c.IsGoal) == SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum;
	public bool IsLastOnePlayer => characters.Count((Biathlon_Character c) => c.IsPlayer && c.IsGoal) == SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum - 1;
	public void Init()
	{
		for (int i = 0; i < characters.Length; i++)
		{
			characters[i].Init(i, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
		}
		placements = Enumerable.Range(0, characters.Length).ToArray();
	}
	public void UpdateMethod()
	{
		Biathlon_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateMethod();
		}
		UpdatePlacements();
	}
	public void UpdateAudio()
	{
		Biathlon_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateAudio();
		}
	}
	public void FixedUpdateMethod()
	{
		Biathlon_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FixedUpdateMethod();
		}
	}
	public void GameEnd()
	{
		Biathlon_Character[] array = characters;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GameEnd();
		}
	}
	public void ForceGoal(float time)
	{
		Biathlon_Course current = SingletonCustom<Biathlon_Courses>.Instance.Current;
		float length = current.Length;
		int raceLap = current.RaceLap;
		float num = length * (float)raceLap;
		for (int i = 0; i < characters.Length; i++)
		{
			Biathlon_Character biathlon_Character = characters[i];
			if (biathlon_Character.IsGoal)
			{
				continue;
			}
			if (biathlon_Character.CurrentRunDistance <= 0f)
			{
				SingletonCustom<Biathlon_GameMain>.Instance.CharacterForceGoal(biathlon_Character.PlayerNo, (int)biathlon_Character.ControlUser, 599.99f);
				continue;
			}
			float num2 = num / biathlon_Character.CurrentRunDistance * time;
			if (num2 < 0f)
			{
				SingletonCustom<Biathlon_GameMain>.Instance.CharacterForceGoal(biathlon_Character.PlayerNo, (int)biathlon_Character.ControlUser, 599.99f);
			}
			else
			{
				SingletonCustom<Biathlon_GameMain>.Instance.CharacterForceGoal(biathlon_Character.PlayerNo, (int)biathlon_Character.ControlUser, num2);
			}
		}
	}
	public int[] GetUsers()
	{
		int[] array = new int[characters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (int)characters[i].ControlUser;
		}
		return array;
	}
	public Biathlon_Character GetCharacter(int no)
	{
		return characters[no];
	}
	private void UpdatePlacements()
	{
		int num = 0;
		int[] array = new int[characters.Length];
		for (int i = 0; i < characters.Length; i++)
		{
			Biathlon_Character biathlon_Character = characters[i];
			if (biathlon_Character.IsLockPlacement)
			{
				array[num] = biathlon_Character.PlayerNo;
				num++;
			}
		}
		int num2 = 0;
		for (int j = 0; j < characters.Length; j++)
		{
			if (!characters[j].IsLockPlacement)
			{
				Biathlon_Character biathlon_Character2 = characters[j];
				array[num + num2] = biathlon_Character2.PlayerNo;
				num2++;
			}
		}
		for (int k = num; k < characters.Length - 1; k++)
		{
			for (int l = k + 1; l < characters.Length; l++)
			{
				Biathlon_Character obj = characters[array[k]];
				Biathlon_Character biathlon_Character3 = characters[array[l]];
				if (obj.CurrentRunDistance < biathlon_Character3.CurrentRunDistance)
				{
					_003CUpdatePlacements_003Eg__Swap_007C16_0(ref array[k], ref array[l]);
				}
			}
		}
		for (int m = num; m < characters.Length; m++)
		{
			placements[array[m]] = m;
			characters[array[m]].UpdatePlacement(m);
		}
	}
	[CompilerGenerated]
	private static void _003CUpdatePlacements_003Eg__Swap_007C16_0(ref int p1, ref int p2)
	{
		int num = p2;
		int num2 = p1;
		p1 = num;
		p2 = num2;
	}
}
