using UnityEngine.U2D;

namespace UnityEngine.Extension.L10
{
	public class SpriteLocalizer : LocalizerBase
	{
		[SerializeField]
		[DisplayName("対象のレンダラ\u30fc")]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[DisplayName("スプライトアトラス")]
		private SpriteAtlas spriteAtlas;

		[SerializeField]
		[DisplayName("日本語のスプライト名")]
		private string jpSpriteName;

		[SerializeField]
		[DisplayName("英語のスプライト名")]
		private string enSpriteName;

		protected override void Localize()
		{
			switch (Localization.Language)
			{
			case Localization.SupportedLanguage.Japanese:
				spriteRenderer.sprite = spriteAtlas.GetSprite(jpSpriteName);
				break;
			case Localization.SupportedLanguage.English:
				spriteRenderer.sprite = spriteAtlas.GetSprite(enSpriteName);
				break;
			}
		}
	}
}
