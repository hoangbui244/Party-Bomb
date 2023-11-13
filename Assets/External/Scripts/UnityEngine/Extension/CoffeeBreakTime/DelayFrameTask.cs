using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class DelayFrameTask : CoffeeBreakTask
	{
		private readonly int delayFrame;

		private readonly Func<int> delayFrameFunc;

		protected int DelayFrame => delayFrame + (delayFrameFunc?.Invoke() ?? 0);

		protected internal DelayFrameTask(int delayFrame)
		{
			this.delayFrame = delayFrame;
		}

		protected internal DelayFrameTask(Func<int> delayFrameFunc)
		{
			this.delayFrameFunc = delayFrameFunc;
		}

		protected internal DelayFrameTask(int delayFrame, Func<int> delayFrameFunc)
		{
			this.delayFrame = delayFrame;
			this.delayFrameFunc = delayFrameFunc;
		}

		protected override IEnumerator Run()
		{
			int elapsed = 0;
			while (elapsed < DelayFrame)
			{
				if (!base.Owner.IsPause)
				{
					elapsed++;
				}
				yield return null;
			}
		}
	}
}
