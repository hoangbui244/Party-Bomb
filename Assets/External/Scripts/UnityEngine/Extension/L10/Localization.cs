using System;
using UnityEngine.EventSystems;

namespace UnityEngine.Extension.L10
{
	[DefaultExecutionOrder(-1)]
	public class Localization : SingletonMonoBehaviour<Localization>
	{
		public enum SupportedLanguage
		{
			Japanese,
			English
		}

		private static Func<SupportedLanguage> currentLanguageHandler;

		private static readonly ExecuteEvents.EventFunction<ILocalizeEventReceiver> ExecuteFunc = Execute;

		[SerializeField]
		[DisplayName("シ\u30fcンのル\u30fcトオブジェクト")]
		private GameObject sceneRoot;

		private SupportedLanguage previousLanguage;

		public static SupportedLanguage Language => currentLanguageHandler?.Invoke() ?? SupportedLanguage.Japanese;

		protected override void OnAwake()
		{
			previousLanguage = Language;
		}

		private void Update()
		{
			SupportedLanguage language = Language;
			if (language != previousLanguage)
			{
				ExecuteEvents.ExecuteHierarchy(sceneRoot, null, ExecuteFunc);
				previousLanguage = language;
			}
		}

		private static void Execute(ILocalizeEventReceiver handler, BaseEventData _)
		{
			handler.LocalizeByEvent();
		}

		public static void RegisterCurrentLanguageHandler(Func<SupportedLanguage> handler)
		{
			currentLanguageHandler = handler;
		}
	}
}
