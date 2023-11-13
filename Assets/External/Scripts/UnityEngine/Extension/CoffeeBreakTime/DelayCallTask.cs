using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class DelayCallTask : DelayTask
	{
		private readonly Action action;

		protected internal DelayCallTask(float delayTime, Action action)
			: base(delayTime)
		{
			this.action = action;
		}

		protected internal DelayCallTask(Func<float> delayTimeFunc, Action action)
			: base(delayTimeFunc)
		{
			this.action = action;
		}

		protected internal DelayCallTask(float delayTime, Func<float> delayTimeFunc, Action action)
			: base(delayTime, delayTimeFunc)
		{
			this.action = action;
		}

		protected override IEnumerator Run()
		{
			yield return base.Run();
			action?.Invoke();
		}
	}
}
