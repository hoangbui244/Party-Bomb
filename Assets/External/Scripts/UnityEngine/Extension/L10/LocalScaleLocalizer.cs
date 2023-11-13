namespace UnityEngine.Extension.L10
{
	public class LocalScaleLocalizer : LocalizerBase
	{
		[SerializeField]
		[DisplayName("日本語のスケ\u30fcル")]
		private Vector3 jpLocalScale;

		[SerializeField]
		[DisplayName("英語のスケ\u30fcル")]
		private Vector3 enLocalScale;

		protected override void Localize()
		{
			switch (Localization.Language)
			{
			case Localization.SupportedLanguage.Japanese:
				base.transform.localScale = jpLocalScale;
				break;
			case Localization.SupportedLanguage.English:
				base.transform.localScale = enLocalScale;
				break;
			}
		}
	}
}
