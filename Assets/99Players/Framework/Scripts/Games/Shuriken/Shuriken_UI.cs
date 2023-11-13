using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_UI : SingletonMonoBehaviour<Shuriken_UI>
{
	[SerializeField]
	[DisplayName("共通UIレイアウト")]
	private Shuriken_CommonUILayout commonUILayout;
	[SerializeField]
	[DisplayName("ポ\u30fcズUI")]
	private GameObject pauseUI;
	public void Initialize()
	{
		commonUILayout.Initialize();
		pauseUI.SetActive(SingletonCustom<GameSettingManager>.Instance.IsSinglePlay);
	}
	public void SetRemainingTime(float time)
	{
		commonUILayout.SetRemainingTime(time);
	}
	public void SetScore(int point, int playerNo)
	{
		commonUILayout.SetScore(point, playerNo);
	}
	public void UpdatePosition(Vector2 position, int playerNo)
	{
		commonUILayout.UpdatePosition(position, playerNo);
	}
	public void DisableReticle(int playerNo)
	{
		commonUILayout.DisableReticle(playerNo);
	}
	public void ShowThrowOpInformation()
	{
		commonUILayout.ShowThrowOpInformation();
	}
	public void HideThrowOpInformation()
	{
		commonUILayout.HideThrowOpInformation();
	}
	public void ShowGetScoreUI(int point, int playerNo, Vector3 worldPosition)
	{
		commonUILayout.ShowGetScoreUI(point, playerNo, worldPosition);
	}
}
