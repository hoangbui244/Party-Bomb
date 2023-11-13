using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Animation : SingletonMonoBehaviour<FlyingSquirrelRace_Animation>
{
	[SerializeField]
	private FlyingSquirrelRace_PlayerAnimation[] playerAnimations;
	[SerializeField]
	private FlyingSquirrelRace_StageAnimation[] stageAnimations;
	public void Initialize()
	{
		FlyingSquirrelRace_StageAnimation[] array = stageAnimations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Initialize();
		}
	}
	public void PlayStartAnimation()
	{
		for (int i = 0; i < SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.Controllers.Length; i++)
		{
			PlayStartAnimation(i);
		}
	}
	public void PlayPreGoalAnimation(int playerNo)
	{
		stageAnimations[playerNo].PlayEndStandPreGoalAnimation();
	}
	public void PlayGoalAnimation(int playerNo)
	{
		StartCoroutine(PlayGoalAnimationInternal(playerNo));
	}
	private void PlayStartAnimation(int playerNo)
	{
		FlyingSquirrelRace_PlayerAnimation obj = playerAnimations[playerNo];
		FlyingSquirrelRace_StageAnimation flyingSquirrelRace_StageAnimation = stageAnimations[playerNo];
		obj.PlayStartAnimation();
		StartCoroutine(flyingSquirrelRace_StageAnimation.PlayStartAnimation());
	}
	private IEnumerator PlayGoalAnimationInternal(int playerNo)
	{
		FlyingSquirrelRace_PlayerAnimation player = playerAnimations[playerNo];
		FlyingSquirrelRace_StageAnimation stage = stageAnimations[playerNo];
		stage.StopEndStandPreGoalAnimation();
		StartCoroutine(stage.PlayPreGoalAnimation());
		StartCoroutine(player.PlayPreGoalAnimation());
		yield return stage.PlayGoalAnimation0();
		StartCoroutine(stage.PlayGoalAnimation1());
		StartCoroutine(player.PlayGoalAnimation());
	}
	public void StopGoalAnimation(int playerNo)
	{
		FlyingSquirrelRace_PlayerAnimation flyingSquirrelRace_PlayerAnimation = playerAnimations[playerNo];
		FlyingSquirrelRace_StageAnimation flyingSquirrelRace_StageAnimation = stageAnimations[playerNo];
		flyingSquirrelRace_StageAnimation.StopEndStandPreGoalAnimation();
		StopCoroutine(flyingSquirrelRace_StageAnimation.PlayPreGoalAnimation());
		StopCoroutine(flyingSquirrelRace_PlayerAnimation.PlayPreGoalAnimation());
		StopCoroutine(flyingSquirrelRace_StageAnimation.PlayGoalAnimation1());
		StopCoroutine(flyingSquirrelRace_PlayerAnimation.PlayGoalAnimation());
	}
}
