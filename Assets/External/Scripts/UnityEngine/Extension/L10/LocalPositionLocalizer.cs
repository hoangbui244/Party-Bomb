namespace UnityEngine.Extension.L10
{
	public class LocalPositionLocalizer : LocalizerBase
	{
		[SerializeField]
		[DisplayName("日本語の座標")]
		private Vector3 jpLocalPosition;

		[SerializeField]
		[DisplayName("英語の座標")]
		private Vector3 enLocalPosition;

		protected override void Localize()
		{
			switch (Localization.Language)
			{
			case Localization.SupportedLanguage.Japanese:
				base.transform.localPosition = jpLocalPosition;
				break;
			case Localization.SupportedLanguage.English:
				base.transform.localPosition = enLocalPosition;
				break;
			}
		}
	}
}
