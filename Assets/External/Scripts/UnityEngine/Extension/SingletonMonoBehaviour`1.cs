namespace UnityEngine.Extension
{
	public abstract class SingletonMonoBehaviour<T> : DecoratedMonoBehaviour where T : DecoratedMonoBehaviour
	{
		private static T instance;

		protected virtual bool Persistence
		{
			get;
			set;
		}

		public static T Instance
		{
			get
			{
				if ((bool)(Object)instance)
				{
					return instance;
				}
				instance = Object.FindObjectOfType<T>();
				if (!(Object)instance)
				{
					UnityEngine.Debug.LogError(typeof(T)?.ToString() + " is nothing.");
				}
				return instance;
			}
		}

		private void Awake()
		{
			if (this != Instance)
			{
				UnityEngine.Object.Destroy(this);
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (Persistence)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			OnAwake();
		}

		protected virtual void OnAwake()
		{
		}
	}
}
