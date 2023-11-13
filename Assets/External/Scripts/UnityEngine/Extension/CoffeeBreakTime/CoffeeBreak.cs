using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class CoffeeBreak
	{
		private Coroutine coroutine;

		private MonoBehaviour monoBehaviour;

		private readonly List<CoffeeBreakTask> tasks;

		private bool isReverse;

		private int currentTask;

		private int lastExecutedCount;

		private int MaxTaskIndex => Mathf.Max(0, tasks.Count);

		internal bool IsRepeat
		{
			get;
			private set;
		}

		internal bool IsPause
		{
			get;
			private set;
		}

		internal bool IsReverse => isReverse;

		internal bool IsPingPong
		{
			get;
			private set;
		}

		public CoffeeBreak Delay(float delayTime)
		{
			tasks.Add(new DelayTask(delayTime));
			return this;
		}

		public CoffeeBreak Delay(Func<float> delayTimeFunc)
		{
			tasks.Add(new DelayTask(delayTimeFunc));
			return this;
		}

		public CoffeeBreak Delay(float delayTime, Func<float> delayTimeFunc)
		{
			tasks.Add(new DelayTask(delayTime, delayTimeFunc));
			return this;
		}

		public CoffeeBreak DelayCall(float delayTime, Action action)
		{
			tasks.Add(new DelayCallTask(delayTime, action));
			return this;
		}

		public CoffeeBreak DelayCall(Func<float> delayTimeFunc, Action action)
		{
			tasks.Add(new DelayCallTask(delayTimeFunc, action));
			return this;
		}

		public CoffeeBreak DelayCall(float delayTime, Func<float> delayTimeFunc, Action action)
		{
			tasks.Add(new DelayCallTask(delayTime, delayTimeFunc, action));
			return this;
		}

		public CoffeeBreak DelayFrame(int delayFrame)
		{
			tasks.Add(new DelayFrameTask(delayFrame));
			return this;
		}

		public CoffeeBreak DelayFrame(Func<int> delayFrameFunc)
		{
			tasks.Add(new DelayFrameTask(delayFrameFunc));
			return this;
		}

		public CoffeeBreak DelayFrame(int delayFrame, Func<int> delayFrameFunc)
		{
			tasks.Add(new DelayFrameTask(delayFrame, delayFrameFunc));
			return this;
		}

		public CoffeeBreak DelayFrameCall(int delayFrame, Action action)
		{
			tasks.Add(new DelayFrameCallTask(delayFrame, action));
			return this;
		}

		public CoffeeBreak DelayFrameCall(Func<int> delayFrameFunc, Action action)
		{
			tasks.Add(new DelayFrameCallTask(delayFrameFunc, action));
			return this;
		}

		public CoffeeBreak DelayFrameCall(int delayFrame, Func<int> delayFrameFunc, Action action)
		{
			tasks.Add(new DelayFrameCallTask(delayFrame, delayFrameFunc, action));
			return this;
		}

		public CoffeeBreak If(Func<bool> condition, Action callback)
		{
			tasks.Add(new IfTask(condition, callback));
			return this;
		}

		public CoffeeBreak Keep(float keepTime, Action action)
		{
			tasks.Add(new KeepTask(keepTime, action));
			return this;
		}

		public CoffeeBreak Keep(Func<float> keepTimeFunc, Action action)
		{
			tasks.Add(new KeepTask(keepTimeFunc, action));
			return this;
		}

		public CoffeeBreak Keep(float keepTime, Func<float> keepTimeFunc, Action action)
		{
			tasks.Add(new KeepTask(keepTime, keepTimeFunc, action));
			return this;
		}

		public CoffeeBreak Keep(float keepTime, Action<float> progressReceiveAction)
		{
			tasks.Add(new KeepTask(keepTime, progressReceiveAction));
			return this;
		}

		public CoffeeBreak Keep(Func<float> keepTimeFunc, Action<float> progressReceiveAction)
		{
			tasks.Add(new KeepTask(keepTimeFunc, progressReceiveAction));
			return this;
		}

		public CoffeeBreak Keep(float keepTime, Func<float> keepTimeFunc, Action<float> progressReceiveAction)
		{
			tasks.Add(new KeepTask(keepTime, keepTimeFunc, progressReceiveAction));
			return this;
		}

		public CoffeeBreak AsGlobal()
		{
			if (coroutine != null)
			{
				return this;
			}
			monoBehaviour = GlobalCoffeeBreakRunner.Instance;
			return this;
		}

		public CoffeeBreak Repeat()
		{
			IsRepeat = true;
			return this;
		}

		public CoffeeBreak Reverse()
		{
			isReverse = !isReverse;
			if (currentTask != 0)
			{
				currentTask = tasks.Count - currentTask;
			}
			return this;
		}

		public CoffeeBreak Forward()
		{
			if (isReverse)
			{
				Reverse();
			}
			return this;
		}

		public CoffeeBreak Backward()
		{
			if (!isReverse)
			{
				Reverse();
			}
			return this;
		}

		public CoffeeBreak PingPong()
		{
			IsPingPong = true;
			return this;
		}

		public CoffeeBreak UnscaledDelay(float delayTime)
		{
			tasks.Add(new UnscaledDelayTask(delayTime));
			return this;
		}

		public CoffeeBreak UnscaledDelay(Func<float> delayTimeFunc)
		{
			tasks.Add(new UnscaledDelayTask(delayTimeFunc));
			return this;
		}

		public CoffeeBreak UnscaledDelay(float delayTime, Func<float> delayTimeFunc)
		{
			tasks.Add(new UnscaledDelayTask(delayTime, delayTimeFunc));
			return this;
		}

		public CoffeeBreak UnscaledDelayCall(float delayTime, Action action)
		{
			tasks.Add(new UnscaledDelayCallTask(delayTime, action));
			return this;
		}

		public CoffeeBreak UnscaledDelayCall(Func<float> delayTimeFunc, Action action)
		{
			tasks.Add(new UnscaledDelayCallTask(delayTimeFunc, action));
			return this;
		}

		public CoffeeBreak UnscaledDelayCall(float delayTime, Func<float> delayTimeFunc, Action action)
		{
			tasks.Add(new UnscaledDelayCallTask(delayTime, delayTimeFunc, action));
			return this;
		}

		public CoffeeBreak Wait(Func<bool> predicate)
		{
			tasks.Add(new WaitTask(predicate));
			return this;
		}

		public CoffeeBreak()
			: this(GlobalCoffeeBreakRunner.Instance)
		{
		}

		public CoffeeBreak(MonoBehaviour monoBehaviour)
		{
			this.monoBehaviour = monoBehaviour;
			tasks = new List<CoffeeBreakTask>();
		}

		public CoffeeBreak Start()
		{
			if (!monoBehaviour)
			{
				return this;
			}
			if (IsPause)
			{
				IsPause = false;
			}
			if (coroutine != null)
			{
				return this;
			}
			if (tasks.Count == 0)
			{
				return this;
			}
			if (currentTask >= tasks.Count)
			{
				currentTask = 0;
			}
			lastExecutedCount = 0;
			coroutine = monoBehaviour.StartCoroutine(Execute());
			return this;
		}

		public CoffeeBreak Stop()
		{
			if (!monoBehaviour)
			{
				return this;
			}
			if (coroutine != null)
			{
				monoBehaviour.StopCoroutine(coroutine);
			}
			return this;
		}

		public CoffeeBreak Pause()
		{
			IsPause = true;
			return this;
		}

		public CoffeeBreak Resume()
		{
			IsPause = false;
			return this;
		}

		private IEnumerator Execute()
		{
			int reverseIndex = -1;
			while (currentTask < MaxTaskIndex)
			{
				int taskIndex;
				CoffeeBreakTask task = GetTask(out taskIndex);
				if (taskIndex != reverseIndex)
				{
					reverseIndex = taskIndex;
					yield return ExecuteTask(task);
					while (IsPause)
					{
						yield return null;
					}
					PingPongIfNeeded(taskIndex, ref reverseIndex);
					RepeatIfNeeded(taskIndex, ref reverseIndex);
				}
			}
			coroutine = null;
		}

		private CoffeeBreakTask GetTask(out int taskIndex)
		{
			taskIndex = currentTask;
			if (isReverse)
			{
				taskIndex = MaxTaskIndex - 1 - taskIndex;
			}
			CoffeeBreakTask result = tasks[taskIndex];
			currentTask++;
			return result;
		}

		private IEnumerator ExecuteTask(CoffeeBreakTask task)
		{
			yield return task.Run(this);
			lastExecutedCount++;
		}

		private void PingPongIfNeeded(int taskIndex, ref int reverseIndex)
		{
			if (IsPingPong && taskIndex >= MaxTaskIndex && (lastExecutedCount <= MaxTaskIndex || IsRepeat))
			{
				Reverse();
				reverseIndex = -1;
			}
		}

		private void RepeatIfNeeded(int taskIndex, ref int reverseIndex)
		{
			if (IsRepeat && taskIndex >= MaxTaskIndex)
			{
				currentTask = 0;
				reverseIndex = -1;
			}
		}
	}
}
