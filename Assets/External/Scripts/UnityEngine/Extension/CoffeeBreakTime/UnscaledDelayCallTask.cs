using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class UnscaledDelayCallTask : UnscaledDelayTask
	{
		private readonly Action action;

		protected internal UnscaledDelayCallTask(float delayTime, Action action)
			: base(delayTime)
		{
			this.action = action;
		}

		protected internal UnscaledDelayCallTask(Func<float> delayTimeFunc, Action action)
			: base(delayTimeFunc)
		{
			this.action = action;
		}

		protected internal UnscaledDelayCallTask(float delayTime, Func<float> delayTimeFunc, Action action)
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
