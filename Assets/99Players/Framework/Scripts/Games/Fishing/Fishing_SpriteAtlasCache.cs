using UnityEngine;
using UnityEngine.U2D;
public class Fishing_SpriteAtlasCache : SingletonCustom<Fishing_SpriteAtlasCache>
{
	[SerializeField]
	private SpriteAtlas commonAtlas;
	[SerializeField]
	private SpriteAtlas smeltFishingAtlas;
	public SpriteAtlas GetCommonAtlas()
	{
		return commonAtlas;
	}
	public SpriteAtlas GetSmeltFishingAtlas()
	{
		return smeltFishingAtlas;
	}
}
