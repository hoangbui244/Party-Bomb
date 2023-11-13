using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Players : SingletonMonoBehaviour<FlyingSquirrelRace_Players>
{
	[SerializeField]
	[DisplayName("プレイヤ\u30fc")]
	private FlyingSquirrelRace_Player[] players;
	[SerializeField]
	[DisplayName("プレイヤ\u30fcの風呂敷のマテリアル")]
	private Material[] matFuroshiki;
	private int[] placements;
	private float[] times;
	public bool IsStartAnimationEnd
	{
		get;
		set;
	}
	public void Initialize()
	{
		FlyingSquirrelRace_Definition.Controller[] controllers = SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.Controllers;
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Initialize(i, controllers[i]);
		}
		times = new float[controllers.Length];
		placements = new int[controllers.Length];
	}
	public void UpdateMethod()
	{
		if (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState != FlyingSquirrelRace_Definition.GameState.DuringGame || !IsStartAnimationEnd)
		{
			return;
		}
		for (int i = 0; i < players.Length; i++)
		{
			players[i].UpdateMethod();
			if (!players[i].IsGoal)
			{
				times[i] += Time.deltaTime;
				SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.UpdateTime(times[i], i);
			}
		}
	}
	public void PreGoalPlayer(int playerNo)
	{
		SingletonMonoBehaviour<FlyingSquirrelRace_Animation>.Instance.PlayPreGoalAnimation(playerNo);
	}
	public int GoalPlayer(int playerNo)
	{
		int num = 0;
		float num2 = times[playerNo];
		for (int i = 0; i < players.Length; i++)
		{
			if (i != playerNo && times[i] < num2)
			{
				num++;
			}
		}
		placements[playerNo] = num;
		SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.ShowResultPlacement(num, playerNo);
		SingletonMonoBehaviour<FlyingSquirrelRace_Animation>.Instance.PlayGoalAnimation(playerNo);
		return num;
	}
	public int GetUntilGoalUserControlPlayerNum()
	{
		return players.Count((FlyingSquirrelRace_Player p) => !p.IsGoal && p.IsUserControl);
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
	public FlyingSquirrelRace_Player GetPlayer(int playerNo)
	{
		return players[playerNo];
	}
	public Material GetFuroshiki(int controller)
	{
		return matFuroshiki[controller];
	}
}
