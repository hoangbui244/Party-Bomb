using System;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class WaitWhile : CustomYieldInstruction
	{
		private Func<bool> predicate;

		private bool IsPause => Owner?.IsPause ?? false;

		internal CoffeeBreak Owner
		{
			get;
			set;
		}

		internal Func<bool> Predicate
		{
			set
			{
				predicate = value;
			}
		}

		public override bool keepWaiting
		{
			get
			{
				if (IsPause || (predicate?.Invoke() ?? false))
				{
					return true;
				}
				Owner = null;
				return false;
			}
		}

		public WaitWhile(CoffeeBreak owner, Func<bool> predicate)
		{
			Owner = owner;
			Predicate = predicate;
		}
	}
}
