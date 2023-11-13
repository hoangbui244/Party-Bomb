using System.Collections;

namespace UnityEngine.Extension.CoffeeBreakTime
{
	public abstract class CoffeeBreakTask
	{
		protected CoffeeBreak Owner
		{
			get;
			private set;
		}

		public IEnumerator Run(CoffeeBreak owner)
		{
			Owner = owner;
			return Run();
		}

		protected abstract IEnumerator Run();
	}
}
