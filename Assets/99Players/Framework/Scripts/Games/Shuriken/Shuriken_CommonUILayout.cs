using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_CommonUILayout : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("残り時間UI")]
	private Shuriken_RemainingTimeUI remainingTime;
	[SerializeField]
	[DisplayName("プレイヤ\u30fcアイコンUI")]
	private Shuriken_PlayerIconUI[] playerIcons;
	[SerializeField]
	[DisplayName("レティクル")]
	private Shuriken_ReticleUI[] reticles;
	[SerializeField]
	[DisplayName("スコアUI")]
	private Shuriken_ScoreUI[] scores;
	[SerializeField]
	[DisplayName("操作説明")]
	private Shuriken_OperationInformationUI throwOperationInformation;
	[SerializeField]
	[DisplayName("スコア獲得UI")]
	private Shuriken_GetScoreUI[] getScores;
	public void Initialize()
	{
		remainingTime.Initialize();
		Shuriken_Definition.ControlUser[] controlUsers = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlUsers;
		for (int i = 0; i < controlUsers.Length; i++)
		{
			playerIcons[i].Initialize(controlUsers[i]);
			reticles[i].Initialize(i);
			scores[i].Initialize();
		}
		throwOperationInformation.Init();
		Shuriken_GetScoreUI[] array = getScores;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Initialize();
		}
	}
	public void SetRemainingTime(float time)
	{
		remainingTime.SetRemainingTime(time);
	}
	public void SetScore(int point, int playerNo)
	{
		scores[playerNo].SetPoint(point);
	}
	public void UpdatePosition(Vector2 position, int playerNo)
	{
		reticles[playerNo].UpdatePosition(position);
	}
	public void DisableReticle(int playerNo)
	{
		reticles[playerNo].Disable();
	}
	public void ShowThrowOpInformation()
	{
		throwOperationInformation.Show();
	}
	public void HideThrowOpInformation()
	{
		throwOperationInformation.Hide();
	}
	public void ShowGetScoreUI(int point, int playerNo, Vector3 worldPosition)
	{
		Shuriken_GetScoreUI[] array = getScores;
		int num = 0;
		Shuriken_GetScoreUI shuriken_GetScoreUI;
		while (true)
		{
			if (num < array.Length)
			{
				shuriken_GetScoreUI = array[num];
				if (!shuriken_GetScoreUI.IsUsing)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		shuriken_GetScoreUI.Show(point, playerNo, worldPosition);
	}
}
