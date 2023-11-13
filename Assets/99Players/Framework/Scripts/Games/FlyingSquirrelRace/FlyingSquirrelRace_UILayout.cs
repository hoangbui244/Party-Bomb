using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_UILayout : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("タイマ\u30fc")]
	private FlyingSquirrelRace_Timer[] timers;
	[SerializeField]
	[DisplayName("スコア")]
	private FlyingSquirrelRace_Score[] scores;
	[SerializeField]
	[DisplayName("残り距離")]
	private FlyingSquirrelRace_Distance[] distances;
	[SerializeField]
	[DisplayName("リザルト順位")]
	private FlyingSquirrelRace_ResultPlacement[] resultPlacements;
	[SerializeField]
	[DisplayName("プレイヤ\u30fcアイコン")]
	private FlyingSquirrelRace_PlayerIcon[] playerIcon;
	[SerializeField]
	[DisplayName("画面右のコントロ\u30fcラ\u30fcUI")]
	private GameObject[] controllerUI;
	private JoyConButton[] arrayJoyConButton;
	public void Initialize()
	{
		base.gameObject.SetActive(value: true);
		arrayJoyConButton = GetComponentsInChildren<JoyConButton>(includeInactive: true);
		FlyingSquirrelRace_Definition.Controller[] controllers = SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.Controllers;
		for (int i = 0; i < controllers.Length; i++)
		{
			scores[i].Initialize();
			resultPlacements[i].Initialize();
			playerIcon[i].Initialize(i);
			if (i < controllerUI.Length)
			{
				controllerUI[i].SetActive(controllers[i] < FlyingSquirrelRace_Definition.Controller.Cpu1);
				SetJoyConButtonPlayerType(i);
			}
		}
	}
	public void Disable()
	{
		base.gameObject.SetActive(value: false);
	}
	public void UpdateTime(float time, int playerNo)
	{
	}
	public void UpdateScore(int score, int playerNo)
	{
		scores[playerNo].UpdateScore(score);
	}
	public void UpdateDistance(int remaining, int playerNo)
	{
		distances[playerNo].UpdateDistance(remaining);
	}
	public void HideDistance(int playerNo)
	{
		distances[playerNo].HideDistance();
	}
	public void ShowResultPlacement(int placement, int playerNo)
	{
		resultPlacements[playerNo].Show(placement);
	}
	public void HideResultPlacement(int playerNo)
	{
		resultPlacements[playerNo].Hide();
	}
	public void HidePlayerIcon(int playerNo)
	{
		playerIcon[playerNo].Hide();
	}
	public void SetJoyConButtonPlayerType(int playerNo)
	{
		arrayJoyConButton[playerNo].SetPlayerType((JoyConButton.PlayerType)playerNo);
		arrayJoyConButton[playerNo].CheckJoyconButton();
	}
}
