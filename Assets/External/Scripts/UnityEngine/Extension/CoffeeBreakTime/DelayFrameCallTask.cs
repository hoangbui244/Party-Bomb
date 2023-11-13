using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class DelayFrameCallTask : DelayFrameTask
	{
		private readonly Action action;

		protected internal DelayFrameCallTask(int delayFrame, Action action)
			: base(delayFrame)
		{
			this.action = action;
		}

		protected internal DelayFrameCallTask(Func<int> delayFrameFunc, Action action)
			: base(delayFrameFunc)
		{
			this.action = action;
		}

		protected internal DelayFrameCallTask(int delayFrame, Func<int> delayFrameFunc, Action action)
			: base(delayFrame, delayFrameFunc)
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
