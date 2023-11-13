using System;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_GameMain : SingletonMonoBehaviour<Shuriken_GameMain>
{
	private Shuriken_Definition.GameState gameState;
	private float remainingTime;
	private Shuriken_Definition.ControlUser[] controlUsers;
	private int[] characterIndexes;
	[SerializeField]
	[Header("左扉")]
	private GameObject doorLeft;
	[SerializeField]
	[Header("右扉")]
	private GameObject doorRight;
	[SerializeField]
	[Header("追加タ\u30fcゲット")]
	private GameObject addTarget;
	[SerializeField]
	[Header("減少タ\u30fcゲット")]
	private GameObject subTarget;
	[SerializeField]
	[Header("エフェクト")]
	private ParticleSystem[] arrayPsRushEffect;
	public bool IsRushTime
	{
		get;
		set;
	}
	public float RemainingTime => remainingTime;
	public Shuriken_Definition.AIStrength AIStrength
	{
		get;
		private set;
	}
	public Shuriken_Definition.ControlUser[] ControlUsers => controlUsers;
	public int[] CharacterIndexes => characterIndexes;
	public int ControlPlayerNum
	{
		get;
		private set;
	}
	public bool IsDuringGame => gameState == Shuriken_Definition.GameState.DuringGame;
	public void Initialize()
	{
		AIStrength = (Shuriken_Definition.AIStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		gameState = Shuriken_Definition.GameState.Initialized;
		remainingTime = 60f;
		controlUsers = new Shuriken_Definition.ControlUser[4];
		characterIndexes = new int[4];
		for (int i = 0; i < controlUsers.Length; i++)
		{
			controlUsers[i] = (Shuriken_Definition.ControlUser)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
		}
		for (int j = 0; j < characterIndexes.Length; j++)
		{
			characterIndexes[j] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)controlUsers[j]];
		}
		ControlPlayerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
	}
	public void PlayGame()
	{
		SingletonMonoBehaviour<Shuriken_UI>.Instance.ShowThrowOpInformation();
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
	}
	public void UpdateMethod()
	{
		Shuriken_Definition.GameState gameState = this.gameState;
		if (gameState == Shuriken_Definition.GameState.DuringGame)
		{
			DuringGame();
		}
	}
	public int[] GetUserIds()
	{
		int[] array = new int[ControlUsers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (int)ControlUsers[i];
		}
		return array;
	}
	private void GameStart()
	{
		gameState = Shuriken_Definition.GameState.DuringGame;
		SingletonMonoBehaviour<Shuriken_UI>.Instance.HideThrowOpInformation();
	}
	private void DuringGame()
	{
		remainingTime -= Time.deltaTime;
		remainingTime = Mathf.Max(remainingTime, 0f);
		SingletonMonoBehaviour<Shuriken_UI>.Instance.SetRemainingTime(remainingTime);
		if (remainingTime <= 0f)
		{
			GameEnd();
		}
		if (remainingTime <= 20f && !IsRushTime)
		{
			RushAppear();
		}
	}
	private void GameEnd()
	{
		gameState = Shuriken_Definition.GameState.GameEnd;
		SingletonCustom<CommonEndSimple>.Instance.Show(GameResult);
	}
	private void GameResult()
	{
		gameState = Shuriken_Definition.GameState.GameResult;
		SingletonMonoBehaviour<Shuriken_GameResult>.Instance.ShowResult();
	}
	private void RushAppear()
	{
		IsRushTime = true;
		LeanTween.cancel(doorLeft);
		LeanTween.cancel(doorRight);
		SingletonCustom<AudioManager>.Instance.SePlay("se_battle_shell");
		addTarget.SetActive(value: true);
		addTarget.GetComponent<Shuriken_BasicTarget>().Show();
		subTarget.SetActive(value: false);
		for (int i = 0; i < arrayPsRushEffect.Length; i++)
		{
			arrayPsRushEffect[i].Play();
		}
		LeanTween.rotateY(doorLeft, -115f, 0.5f).setEaseOutQuart();
		LeanTween.rotateY(doorRight, 115f, 0.5f).setEaseOutQuart().setOnComplete((Action)delegate
		{
		});
	}
	private void OnDestroy()
	{
		LeanTween.cancel(doorLeft);
		LeanTween.cancel(doorRight);
	}
}
