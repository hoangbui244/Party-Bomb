namespace UnityEngine.Extension.CoffeeBreakTime
{
	public class GlobalCoffeeBreakRunner : DecoratedMonoBehaviour
	{
		private static GlobalCoffeeBreakRunner instance;

		public static GlobalCoffeeBreakRunner Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				instance = new GameObject("[Global Coffee Break Runner]").AddComponent<GlobalCoffeeBreakRunner>();
				return instance;
			}
		}

		private void OnEnable()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (!(instance == this))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
