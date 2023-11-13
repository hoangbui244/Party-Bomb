using System;
using System.Collections.Generic;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class YieldCache
	{
		private static readonly Dictionary<float, List<WaitForSeconds>> WaitForSecondsCache = new Dictionary<float, List<WaitForSeconds>>();

		private static readonly Dictionary<float, List<WaitForUnscaledSeconds>> WaitForUnscaledSecondsCache = new Dictionary<float, List<WaitForUnscaledSeconds>>();

		private static readonly List<WaitWhile> WaitWhileCache = new List<WaitWhile>();

		public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

		public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

		internal static WaitForSeconds WaitForSeconds(CoffeeBreak coffeeBreak, float seconds)
		{
			if (!WaitForSecondsCache.TryGetValue(seconds, out List<WaitForSeconds> value))
			{
				value = new List<WaitForSeconds>(4);
				WaitForSecondsCache.Add(seconds, value);
			}
			foreach (WaitForSeconds item in value)
			{
				if (item.Owner == null)
				{
					item.Owner = coffeeBreak;
					item.Reset();
					return item;
				}
			}
			WaitForSeconds waitForSeconds = new WaitForSeconds(coffeeBreak, seconds);
			value.Add(waitForSeconds);
			return waitForSeconds;
		}

		internal static WaitWhile WaitWhile(CoffeeBreak coffeeBreak, Func<bool> predicate)
		{
			foreach (WaitWhile item in WaitWhileCache)
			{
				if (item.Owner == null)
				{
					item.Owner = coffeeBreak;
					item.Predicate = predicate;
				}
			}
			WaitWhile waitWhile = new WaitWhile(coffeeBreak, predicate);
			WaitWhileCache.Add(waitWhile);
			return waitWhile;
		}

		internal static WaitForUnscaledSeconds WaitForUnscaledSeconds(CoffeeBreak coffeeBreak, float seconds)
		{
			if (!WaitForUnscaledSecondsCache.TryGetValue(seconds, out List<WaitForUnscaledSeconds> value))
			{
				value = new List<WaitForUnscaledSeconds>(4);
				WaitForUnscaledSecondsCache.Add(seconds, value);
			}
			foreach (WaitForUnscaledSeconds item in value)
			{
				if (item.Owner == null)
				{
					item.Owner = coffeeBreak;
					item.Reset();
					return item;
				}
			}
			WaitForUnscaledSeconds waitForUnscaledSeconds = new WaitForUnscaledSeconds(coffeeBreak, seconds);
			value.Add(waitForUnscaledSeconds);
			return waitForUnscaledSeconds;
		}
	}
}
