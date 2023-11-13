using UnityEngine;
public class SmeltFishing_FishingSpaceRegistry : SingletonCustom<SmeltFishing_FishingSpaceRegistry>
{
	[SerializeField]
	private SmeltFishing_FishingSpace[] fishingSpaces;
	public SmeltFishing_FishingSpace GetFishingSpace(int no)
	{
		return fishingSpaces[no];
	}
}
