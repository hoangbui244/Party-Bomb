namespace UnityEngine.Extension.CoffeeBreakTime
{
	internal class WaitForSeconds : CustomYieldInstruction
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
					elapsed += Time.deltaTime;
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

		public WaitForSeconds(CoffeeBreak owner, float seconds)
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
