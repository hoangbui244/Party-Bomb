using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.CoffeeBreakTime;
public class FlyingSquirrelRace_GameMain : SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>
{
	private const float GameEndDelayTime = 5f;
	private const float LastPlayerWaitTime = 20f;
	[SerializeField]
	[Disable(false)]
	[DisplayName("ゲ\u30fcム進行状態")]
	private FlyingSquirrelRace_Definition.GameState gameState;
	private bool isGameEndCountDown;
	private CoffeeBreak cb;
	private bool isFirstGoalPlayer;
	public bool IsSinglePlayerMode
	{
		get;
		private set;
	}
	public int ControlUserNum
	{
		get;
		private set;
	}
	public FlyingSquirrelRace_Definition.Controller[] Controllers
	{
		get;
		private set;
	}
	public FlyingSquirrelRace_Definition.GameState GameState => gameState;
	public FlyingSquirrelRace_Definition.AIStrength AIStrength
	{
		get;
		private set;
	}
	public void Initialize()
	{
		FlyingSquirrelRace_Definition.AIStrength aIStrength2 = AIStrength = (AIStrength = (FlyingSquirrelRace_Definition.AIStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength);
		IsSinglePlayerMode = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
		ControlUserNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		gameState = FlyingSquirrelRace_Definition.GameState.Initialized;
		Controllers = new FlyingSquirrelRace_Definition.Controller[4];
		for (int i = 0; i < Controllers.Length; i++)
		{
			Controllers[i] = (FlyingSquirrelRace_Definition.Controller)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
		}
	}
	public void PlayGame()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(delegate
		{
			GameStart();
			SingletonMonoBehaviour<FlyingSquirrelRace_Animation>.Instance.PlayStartAnimation();
			for (int i = 0; i < Controllers.Length; i++)
			{
				SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.HidePlayerIcon(i);
			}
		});
	}
	public void UpdateMethod()
	{
		FlyingSquirrelRace_Definition.GameState gameState = this.gameState;
		if (gameState == FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			DuringGame();
		}
	}
	public void GoalControlUserPlayer()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		if (!isFirstGoalPlayer)
		{
			isFirstGoalPlayer = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
		}
		StartGameEndCountDownIfNeeded();
	}
	private void GameStart()
	{
		gameState = FlyingSquirrelRace_Definition.GameState.DuringGame;
	}
	private void DuringGame()
	{
	}
	private void GameEnd()
	{
		gameState = FlyingSquirrelRace_Definition.GameState.GameEnd;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			SingletonMonoBehaviour<FlyingSquirrelRace_Animation>.Instance.StopGoalAnimation(i);
			SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.HideResultPlacement(i);
		}
		SingletonCustom<CommonEndSimple>.Instance.Show(GameResult);
	}
	private void GameResult()
	{
		gameState = FlyingSquirrelRace_Definition.GameState.GameResult;
		SingletonMonoBehaviour<FlyingSquirrelRace_GameResult>.Instance.ShowResult();
	}
	private void StartGameEndCountDownIfNeeded()
	{
		switch (SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GetUntilGoalUserControlPlayerNum())
		{
		case 0:
			DelayedGameEnd(5f);
			break;
		case 1:
			DelayedGameEnd(20f);
			break;
		}
	}
	private void DelayedGameEnd(float delay)
	{
		if (cb != null)
		{
			cb.Stop();
			cb = null;
		}
		if (gameState == FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			cb = this.CoffeeBreak().DelayCall(delay, GameEnd);
			cb.Start();
		}
	}
}
