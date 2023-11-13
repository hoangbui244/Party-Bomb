using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Camera : SingletonMonoBehaviour<FlyingSquirrelRace_Camera>
{
	[SerializeField]
	[DisplayName("シングル用カメラ")]
	private Camera[] singleCameras;
	[SerializeField]
	[DisplayName("マルチ用カメラ")]
	private Camera[] multiCameras;
	private Camera[] activeCameras;
	public void Initialize()
	{
		bool isSinglePlay = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
		Camera[] array = singleCameras;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = isSinglePlay;
		}
		array = multiCameras;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = !isSinglePlay;
		}
		activeCameras = (isSinglePlay ? singleCameras : multiCameras);
	}
	public Camera GetCamera(int playerNo)
	{
		return activeCameras[playerNo];
	}
}
