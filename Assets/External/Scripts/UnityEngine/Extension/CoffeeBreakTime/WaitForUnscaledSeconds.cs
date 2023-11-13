namespace UnityEngine.Extension.CoffeeBreakTime
{
	internal class WaitForUnscaledSeconds : CustomYieldInstruction
	{
		private readonly float waitSeconds;

		private float elapsed;

		private bool IsPause => Owner?.IsPause ?? false;

		public override bool keepWaiting
		{
			get
			{
				if (!IsPause)
				{
					elapsed += Time.unscaledDeltaTime;
				}
				if (elapsed < waitSeconds)
				{
					return true;
				}
				Owner = null;
				return false;
			}
		}

		internal CoffeeBreak Owner
		{
			get;
			set;
		}

		public WaitForUnscaledSeconds(CoffeeBreak owner, float seconds)
		{
			Owner = owner;
			waitSeconds = seconds;
		}

		public override void Reset()
		{
			elapsed = 0f;
		}
	}
}
