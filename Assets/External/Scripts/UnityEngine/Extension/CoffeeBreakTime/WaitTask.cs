using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class WaitTask : CoffeeBreakTask
	{
		private readonly Func<bool> predicate;

		public WaitTask(Func<bool> predicate)
		{
			this.predicate = predicate;
		}

		protected override IEnumerator Run()
		{
			yield return new WaitWhile(base.Owner, predicate);
		}
	}
}
