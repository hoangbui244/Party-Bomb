using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_UI : SingletonMonoBehaviour<FlyingSquirrelRace_UI>
{
	[SerializeField]
	[DisplayName("シングルレイアウト")]
	private FlyingSquirrelRace_UILayout singleLayout;
	[SerializeField]
	[DisplayName("マルチレイアウト")]
	private FlyingSquirrelRace_UILayout multiLayout;
	private FlyingSquirrelRace_UILayout ActiveUILayout
	{
		get;
		set;
	}
	public void Initialize()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleLayout.Initialize();
			multiLayout.Disable();
			ActiveUILayout = singleLayout;
		}
		else
		{
			singleLayout.Disable();
			multiLayout.Initialize();
			ActiveUILayout = multiLayout;
		}
	}
	public void UpdateTime(float time, int playerNo)
	{
	}
	public void UpdateScore(int score, int playerNo)
	{
		ActiveUILayout.UpdateScore(score, playerNo);
	}
	public void UpdateDistance(int remaining, int playerNo)
	{
		ActiveUILayout.UpdateDistance(remaining, playerNo);
	}
	public void HideDistance(int playerNo)
	{
		ActiveUILayout.HideDistance(playerNo);
	}
	public void ShowResultPlacement(int placement, int playerNo)
	{
		ActiveUILayout.ShowResultPlacement(placement, playerNo);
	}
	public void HideResultPlacement(int playerNo)
	{
		ActiveUILayout.HideResultPlacement(playerNo);
	}
	public void HidePlayerIcon(int playerNo)
	{
		ActiveUILayout.HidePlayerIcon(playerNo);
	}
}
