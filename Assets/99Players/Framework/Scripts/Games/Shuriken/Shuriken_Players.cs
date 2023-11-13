using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_Players : SingletonMonoBehaviour<Shuriken_Players>
{
	[SerializeField]
	[DisplayName("プレイヤ\u30fcリスト")]
	private Shuriken_Player[] players;
	public void Initialize()
	{
		Shuriken_Definition.ControlUser[] controlUsers = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlUsers;
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Initialize(i, controlUsers[i]);
		}
	}
	public void PostInitialize()
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].PostInitialize();
		}
	}
	public void UpdateMethod()
	{
		if (SingletonMonoBehaviour<Shuriken_GameMain>.Instance.IsDuringGame)
		{
			Shuriken_Player[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateMethod();
			}
		}
	}
	public void AddScore(int playerNo, int score)
	{
		players[playerNo].AddScore(score);
		if (!players[playerNo].IsCpu)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
		}
	}
	public int[] GetScores()
	{
		int[] array = new int[players.Length];
		for (int i = 0; i < players.Length; i++)
		{
			array[i] = players[i].Score;
		}
		return array;
	}
}
