using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class DelayTask : CoffeeBreakTask
	{
		private readonly float delayTime;

		private readonly Func<float> delayTimeFunc;

		protected float DelayTime => delayTime + (delayTimeFunc?.Invoke() ?? 0f);

		protected internal DelayTask(float delayTime)
		{
			this.delayTime = delayTime;
		}

		protected internal DelayTask(Func<float> delayTimeFunc)
		{
			this.delayTimeFunc = delayTimeFunc;
		}

		protected internal DelayTask(float delayTime, Func<float> delayTimeFunc)
		{
			this.delayTime = delayTime;
			this.delayTimeFunc = delayTimeFunc;
		}

		protected override IEnumerator Run()
		{
			yield return YieldCache.WaitForSeconds(base.Owner, DelayTime);
		}
	}
}
