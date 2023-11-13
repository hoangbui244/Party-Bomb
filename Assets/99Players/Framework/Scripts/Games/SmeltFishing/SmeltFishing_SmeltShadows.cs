using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_SmeltShadows : MonoBehaviour
{
	[SerializeField]
	[DisplayName("設定")]
	private SmeltFishing_SmeltShadowsConfig config;
	[SerializeField]
	[DisplayName("魚影")]
	private SmeltFishing_SmeltShadow[] shadows;
	private int current;
	private int shadowLimit;
	private int[] indexes;
	private SmeltFishing_Character playingCharacter;
	private SmeltFishing_IcePlate currentIcePlate;
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
		SetShadowLimit(0);
		indexes = (from i in Enumerable.Range(0, shadows.Length)
			orderby Guid.NewGuid()
			select i).ToArray();
		SmeltFishing_SmeltShadow[] array = shadows;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Init();
		}
	}
	public void UpdateMethod()
	{
		SmeltFishing_SmeltShadow[] array;
		if (currentIcePlate == null)
		{
			array = shadows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateMethod();
			}
			return;
		}
		int limit = GetLimit();
		SetShadowLimit(limit);
		int num = 0;
		array = shadows;
		foreach (SmeltFishing_SmeltShadow obj in array)
		{
			obj.UpdateMethod();
			if (obj.IsShowing)
			{
				num++;
			}
		}
		if (num >= shadowLimit)
		{
			return;
		}
		do
		{
			shadows[indexes[current]].Show();
			num++;
			current++;
			if (current >= indexes.Length)
			{
				current = 0;
			}
		}
		while (num < shadowLimit);
	}
	public void Activate(SmeltFishing_IcePlate icePlate)
	{
		currentIcePlate = icePlate;
		SmeltFishing_SmeltShadow[] array = shadows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Activate();
		}
	}
	public void Deactivate()
	{
		currentIcePlate = null;
		SmeltFishing_SmeltShadow[] array = shadows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Deactivate();
		}
	}
	private int GetLimit()
	{
		return Mathf.RoundToInt(Mathf.Lerp(config.MinShadows, config.MaxShadows, currentIcePlate.SmeltValue));
	}
	private void SetShadowLimit(int limit)
	{
		shadowLimit = Mathf.Clamp(limit, 0, Mathf.Min(config.MaxShadows, shadows.Length));
	}
}
