using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class UnscaledDelayTask : CoffeeBreakTask
	{
		private readonly float delayTime;

		private readonly Func<float> delayTimeFunc;

		protected float DelayTime => delayTime + (delayTimeFunc?.Invoke() ?? 0f);

		protected internal UnscaledDelayTask(float delayTime)
		{
			this.delayTime = delayTime;
		}

		protected internal UnscaledDelayTask(Func<float> delayTimeFunc)
		{
			this.delayTimeFunc = delayTimeFunc;
		}

		protected internal UnscaledDelayTask(float delayTime, Func<float> delayTimeFunc)
		{
			this.delayTime = delayTime;
			this.delayTimeFunc = delayTimeFunc;
		}

		protected override IEnumerator Run()
		{
			yield return YieldCache.WaitForUnscaledSeconds(base.Owner, DelayTime);
		}
	}
}
