using UnityEngine.Extension.CoffeeBreakTime;

namespace UnityEngine.Extension
{
	public static class CoffeeBreakExtension
	{
		public static CoffeeBreak CoffeeBreak(this MonoBehaviour monoBehaviour)
		{
			return new CoffeeBreak(monoBehaviour);
		}

		public static CoffeeBreak Cb(this MonoBehaviour monoBehaviour)
		{
			return new CoffeeBreak(monoBehaviour);
		}
	}
}
