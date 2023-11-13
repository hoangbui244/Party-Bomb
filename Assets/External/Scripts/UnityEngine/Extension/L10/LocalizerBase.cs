using UnityEngine.EventSystems;

namespace UnityEngine.Extension.L10
{
	public class LocalizerBase : DecoratedMonoBehaviour, ILocalizeEventReceiver, IEventSystemHandler
	{
		public void LocalizeByEvent()
		{
			Localize();
		}

		protected virtual void Localize()
		{
		}

		private void OnEnable()
		{
			Localize();
		}
	}
}
