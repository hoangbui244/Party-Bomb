using System;
using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class KeepTask : CoffeeBreakTask
	{
		private readonly float keepTime;

		private readonly Func<float> keepTimeFunc;

		private readonly Action action;

		private readonly Action<float> progressReceiveAction;

		protected float KeepTime => keepTime + (keepTimeFunc?.Invoke() ?? 0f);

		protected internal KeepTask(float keepTime, Action action)
			: this(keepTime, null, action)
		{
		}

		protected internal KeepTask(Func<float> keepTimeFunc, Action action)
			: this(0f, keepTimeFunc, action)
		{
		}

		protected internal KeepTask(float keepTime, Func<float> keepTimeFunc, Action action)
		{
			this.keepTime = keepTime;
			this.keepTimeFunc = keepTimeFunc;
			this.action = action;
			progressReceiveAction = null;
		}

		protected internal KeepTask(float keepTime, Action<float> progressReceiveAction)
			: this(keepTime, null, progressReceiveAction)
		{
		}

		protected internal KeepTask(Func<float> keepTimeFunc, Action<float> progressReceiveAction)
			: this(0f, keepTimeFunc, progressReceiveAction)
		{
		}

		protected internal KeepTask(float keepTime, Func<float> keepTimeFunc, Action<float> progressReceiveAction)
		{
			this.keepTime = keepTime;
			this.keepTimeFunc = keepTimeFunc;
			this.progressReceiveAction = progressReceiveAction;
			action = null;
		}

		protected override IEnumerator Run()
		{
			if (KeepTime < 0f)
			{
				yield return InfiniteLoop();
			}
			float elapsed = 0f;
			while (elapsed < KeepTime)
			{
				if (base.Owner.IsPause)
				{
					yield return null;
				}
				elapsed += Time.deltaTime;
				Invoke(Mathf.Clamp01(elapsed / KeepTime));
				yield return null;
			}
			Invoke(1f);
		}

		private IEnumerator InfiniteLoop()
		{
			while (true)
			{
				if (base.Owner.IsPause)
				{
					yield return null;
				}
				action?.Invoke();
				progressReceiveAction?.Invoke(base.Owner.IsReverse ? 1 : 0);
				yield return null;
			}
		}

		private void Invoke(float t)
		{
			if (base.Owner.IsReverse)
			{
				t = 1f - t;
			}
			action?.Invoke();
			progressReceiveAction?.Invoke(t);
		}
	}
}
