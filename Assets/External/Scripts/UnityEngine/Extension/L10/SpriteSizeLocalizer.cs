namespace UnityEngine.Extension.L10
{
	public class SpriteSizeLocalizer : LocalizerBase
	{
		[SerializeField]
		[DisplayName("対象のレンダラ\u30fc")]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[DisplayName("日本語のサイズ")]
		private Vector2 jpSpriteSize;

		[SerializeField]
		[DisplayName("英語のサイズ")]
		private Vector2 enSpriteSize;

		protected override void Localize()
		{
			switch (Localization.Language)
			{
			case Localization.SupportedLanguage.Japanese:
				spriteRenderer.size = jpSpriteSize;
				break;
			case Localization.SupportedLanguage.English:
				spriteRenderer.size = enSpriteSize;
				break;
			}
		}
	}
}
