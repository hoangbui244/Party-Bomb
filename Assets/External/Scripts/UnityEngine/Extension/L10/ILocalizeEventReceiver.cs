using UnityEngine.EventSystems;

namespace UnityEngine.Extension.L10
{
	public interface ILocalizeEventReceiver : IEventSystemHandler
	{
		void LocalizeByEvent();
	}
}
