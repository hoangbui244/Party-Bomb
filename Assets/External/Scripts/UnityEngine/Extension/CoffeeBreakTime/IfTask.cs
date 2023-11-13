using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class IfTask : CoffeeBreakTask
	{
		private readonly Func<bool> conditionFunc;

		private readonly Action action;

		protected internal IfTask(Func<bool> conditionFunc, Action action)
		{
			this.conditionFunc = conditionFunc;
			this.action = action;
		}

		protected override IEnumerator Run()
		{
			if (conditionFunc?.Invoke() ?? true)
			{
				action?.Invoke();
			}
			yield return null;
		}
	}
}
