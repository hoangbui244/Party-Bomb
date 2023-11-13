using UnityEngine;
public class SmeltFishing_CameraRegistry : SingletonCustom<SmeltFishing_CameraRegistry>
{
	[SerializeField]
	private SmeltFishing_CharacterCamera[] playerCameras;
	public SmeltFishing_CharacterCamera GetCamera(int no)
	{
		return playerCameras[no];
	}
}
