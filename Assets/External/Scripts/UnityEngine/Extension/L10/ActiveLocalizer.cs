namespace UnityEngine.Extension.L10
{
	public class ActiveLocalizer : LocalizerBase
	{
		[SerializeField]
		[DisplayName("日本語での状態")]
		private bool jpActive;

		[SerializeField]
		[DisplayName("英語での状態")]
		private bool enActive;

		protected override void Localize()
		{
			switch (Localization.Language)
			{
			case Localization.SupportedLanguage.Japanese:
				base.gameObject.SetActive(jpActive);
				break;
			case Localization.SupportedLanguage.English:
				base.gameObject.SetActive(enActive);
				break;
			}
		}
	}
}
