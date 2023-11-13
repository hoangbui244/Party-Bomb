using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;

namespace UnityEngine.Extension.L10
{
	public static class SpriteAtlasLocalizationExtension
	{
		private static readonly Dictionary<SpriteAtlas, HashSet<string>> SpriteNamesCache = new Dictionary<SpriteAtlas, HashSet<string>>();

		public static Sprite GetSprite(this SpriteAtlas sa, string name, Localization.SupportedLanguage language)
		{
			switch (language)
			{
			case Localization.SupportedLanguage.Japanese:
				return sa.GetSprite(name);
			case Localization.SupportedLanguage.English:
			{
				string text = name;
				if (text.StartsWith("_"))
				{
					text = "en" + text;
				}
				if (!name.StartsWith("en_"))
				{
					text = "en_" + text;
				}
				HashSet<string> cache = GetCache(sa);
				return sa.GetSprite(cache.Contains(text) ? text : name);
			}
			default:
				return null;
			}
		}

		private static HashSet<string> GetCache(SpriteAtlas sa)
		{
			if (SpriteNamesCache.TryGetValue(sa, out HashSet<string> value))
			{
				return value;
			}
			Sprite[] array = new Sprite[sa.spriteCount];
			sa.GetSprites(array);
			value = new HashSet<string>((from sp in array
				select sp.name).Distinct());
			SpriteNamesCache.Add(sa, value);
			return value;
		}
	}
}
