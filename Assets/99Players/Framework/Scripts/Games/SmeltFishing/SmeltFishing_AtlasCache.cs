using UnityEngine;
using UnityEngine.U2D;
public class SmeltFishing_AtlasCache : SingletonCustom<SmeltFishing_AtlasCache>
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
