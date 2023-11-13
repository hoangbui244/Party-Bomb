using System;
using UnityEngine;
public class SmeltFishing_CharacterFishingAction : MonoBehaviour
{
	private enum FishingState
	{
		ReadyFishingRod,
		CastLine,
		WaitRollUp,
		RollUp,
		RollUpped,
		CaughtDelay
	}
	private SmeltFishing_CharacterFishingRod fishingRod;
	private SmeltFishing_Character playingCharacter;
	private SmeltFishing_CharacterSfx sfx;
	private SmeltFishing_FishingSpace fishingSpace;
	private SmeltFishing_Fishing fishing;
	[SerializeField]
	private SmeltFishing_FishingConfig config;
	private FishingState state;
	private bool showHit;
	private float caughtTime;
	private float canRollUpTime;
	private readonly float CAN_ROLL_UP_TIME = 0.5f;
	private int caughtSmeltCount;
	private readonly float CHECK_HIT_DIFF_TIME = 3f;
	private readonly float CHECK_HIT_MIN_TIME = 1f;
	private float defCheckHitTime;
	private float checkHitTime;
	private bool isRollUp;
	private bool isMoveNextSport;
	private bool isCaught;
	private float previousActionTime;
	private readonly float CHECK_ARROW_PROPER_CHANGE_TIME = 2f;
	private float checkArrowProperChangeTime;
	private float ARROW_PROPER_DIFF_RANGE;
	private float ARROW_PROPER_BRING_DIFF_VALUE;
	private float NEXT_SPOT_SMELT_VALUE;
	private float NEXT_SPOT_PROBABILITY;
	private float ROLL_UP_SMELT_VALUE;
	private int CAUGHT_SMELT_COUNT;
	private float ROLL_UP_NOT_HIT_TIME;
	private float rollUpNotHitTime;
	private int continueSameSpotCnt;
	private SmeltFishing_Definition.AIStrength AIStrength => SingletonCustom<SmeltFishing_GameMain>.Instance.AIStrength;
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
		sfx = GetComponent<SmeltFishing_CharacterSfx>();
		fishing = playingCharacter.GetComponent<SmeltFishing_Fishing>();
		fishingRod = playingCharacter.GetComponent<SmeltFishing_CharacterFishingRod>();
		fishingSpace = SingletonCustom<SmeltFishing_FishingSpaceRegistry>.Instance.GetFishingSpace(playingCharacter.PlayerNo);
		fishing.Init(playingCharacter);
		fishingRod.Init(playingCharacter);
		state = FishingState.ReadyFishingRod;
	}
	public void GameEnd()
	{
		fishingRod.StopAnimation();
		fishing.StopAllCoroutines();
		fishing.RollUp();
	}
	public void UpdateMethod()
	{
		fishingRod.UpdateMethod();
		switch (state)
		{
		case FishingState.ReadyFishingRod:
			ReadyFishingRod();
			break;
		case FishingState.CastLine:
			CastLine();
			break;
		case FishingState.WaitRollUp:
			WaitRollUp();
			break;
		case FishingState.RollUp:
			RollUp();
			break;
		case FishingState.RollUpped:
			RollUpped();
			break;
		case FishingState.CaughtDelay:
			CaughtDelay();
			break;
		}
	}
	public void SetupFishingRod(SmeltFishing_IcePlate icePlate)
	{
		fishingRod.Activate();
		fishing.SetFishingPosition(icePlate);
	}
	public void TakeInFishingRod()
	{
		fishingRod.Deactivate();
	}
	private void ReadyFishingRod()
	{
		if (playingCharacter.Camera.InTransition)
		{
			return;
		}
		if (!playingCharacter.IsPlayer)
		{
			ReadyFishingRodForAI();
			return;
		}
		if (!SingletonCustom<SmeltFishing_UI>.Instance.IsOnceOperationInformation(playingCharacter.PlayerNo, 1))
		{
			SingletonCustom<SmeltFishing_UI>.Instance.ShowOperationInformation(playingCharacter.PlayerNo, 1);
		}
		if (SingletonCustom<SmeltFishing_Input>.Instance.IsPressButtonA(playingCharacter.ControlUser))
		{
			fishingRod.CastLine();
			SetCastLineStatus();
		}
		else if (SingletonCustom<SmeltFishing_Input>.Instance.IsPressButtonB(playingCharacter.ControlUser))
		{
			SingletonCustom<SmeltFishing_UI>.Instance.HideOperationInformation(playingCharacter.PlayerNo);
			playingCharacter.StandUp();
		}
	}
	private void CastLine()
	{
		if (!fishingRod.IsPlayingAnimation)
		{
			if (!playingCharacter.IsPlayer)
			{
				CastLineForAI();
				return;
			}
			showHit = false;
			fishingRod.PlayIdleAnimation();
			state = FishingState.WaitRollUp;
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeRollUp(playingCharacter.PlayerNo);
		}
	}
	private void SetCastLineStatus()
	{
		float smeltValue = Mathf.Clamp01(playingCharacter.UseIcePlate.SmeltValue);
		SingletonCustom<SmeltFishing_UI>.Instance.SetProperSize(playingCharacter.PlayerNo, smeltValue, 0f);
		SingletonCustom<SmeltFishing_UI>.Instance.ShowBaitDepth(playingCharacter.PlayerNo, smeltValue);
		if (playingCharacter.IsPlayer)
		{
			if (!SingletonCustom<SmeltFishing_UI>.Instance.IsOnceAssistComment(playingCharacter.PlayerNo, 1))
			{
				SingletonCustom<SmeltFishing_UI>.Instance.ShowAssistComment(playingCharacter.PlayerNo, 1, isForceHide: true);
			}
			else
			{
				SingletonCustom<SmeltFishing_UI>.Instance.ForceHideAssistComment(playingCharacter.PlayerNo);
			}
			SingletonCustom<SmeltFishing_UI>.Instance.HideOperationInformation((int)playingCharacter.ControlUser);
			canRollUpTime = CAN_ROLL_UP_TIME;
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeRollUp(playingCharacter.PlayerNo);
		}
		showHit = false;
		fishingRod.PlayIdleAnimation();
		state = FishingState.WaitRollUp;
		float num = Mathf.Lerp(0f, 1f, playingCharacter.UseIcePlate.SmeltValue);
		checkHitTime = (1f - num) * CHECK_HIT_DIFF_TIME + CHECK_HIT_MIN_TIME;
		defCheckHitTime = checkHitTime;
		caughtSmeltCount = 0;
		isRollUp = false;
		isCaught = false;
		if (!playingCharacter.IsPlayer)
		{
			ARROW_PROPER_DIFF_RANGE = UnityEngine.Random.Range(SmeltFishing_Definition.ARROW_PROPER_DIFF_RANGE[(int)AIStrength] - 30f, SmeltFishing_Definition.ARROW_PROPER_DIFF_RANGE[(int)AIStrength] + 30f);
			NEXT_SPOT_SMELT_VALUE = UnityEngine.Random.Range(SmeltFishing_Definition.NEXT_SPOT_SMELT_VALUE[(int)AIStrength] - 0.2f, SmeltFishing_Definition.NEXT_SPOT_SMELT_VALUE[(int)AIStrength] + 0.2f);
			NEXT_SPOT_SMELT_VALUE = Mathf.Clamp01(NEXT_SPOT_SMELT_VALUE);
			NEXT_SPOT_PROBABILITY = UnityEngine.Random.Range(SmeltFishing_Definition.NEXT_SPOT_PROBABILITY[(int)AIStrength] - 0.2f, SmeltFishing_Definition.NEXT_SPOT_PROBABILITY[(int)AIStrength] + 0.2f);
			NEXT_SPOT_PROBABILITY += SmeltFishing_Definition.SAME_SPOT_DIFF_NEXT_SPOT_VALUE[(int)AIStrength] * (float)continueSameSpotCnt;
			NEXT_SPOT_PROBABILITY = Mathf.Clamp01(NEXT_SPOT_PROBABILITY);
			ARROW_PROPER_BRING_DIFF_VALUE = UnityEngine.Random.Range(SmeltFishing_Definition.ARROW_PROPER_BRING_DIFF_VALUE[(int)AIStrength] - 0.1f, SmeltFishing_Definition.ARROW_PROPER_BRING_DIFF_VALUE[(int)AIStrength] + 0.1f);
			ARROW_PROPER_BRING_DIFF_VALUE = Mathf.Clamp01(ARROW_PROPER_BRING_DIFF_VALUE);
			checkArrowProperChangeTime = CHECK_ARROW_PROPER_CHANGE_TIME;
			ROLL_UP_SMELT_VALUE = UnityEngine.Random.Range(SmeltFishing_Definition.ROLL_UP_SMELT_VALUE[(int)AIStrength] - 0.1f, SmeltFishing_Definition.ROLL_UP_SMELT_VALUE[(int)AIStrength] + 0.1f);
			CAUGHT_SMELT_COUNT = UnityEngine.Random.Range(SmeltFishing_Definition.CAUGHT_SMELT_COUNT[(int)AIStrength] - 1, SmeltFishing_Definition.CAUGHT_SMELT_COUNT[(int)AIStrength]);
			ROLL_UP_NOT_HIT_TIME = UnityEngine.Random.Range(SmeltFishing_Definition.ROLL_UP_NOT_HIT_TIME[(int)AIStrength] - 1f, SmeltFishing_Definition.ROLL_UP_NOT_HIT_TIME[(int)AIStrength] + 1f);
			rollUpNotHitTime = ROLL_UP_NOT_HIT_TIME;
			isMoveNextSport = false;
		}
	}
	private void WaitRollUp()
	{
		float smeltValue = Mathf.Clamp01(playingCharacter.UseIcePlate.SmeltValue - (float)caughtSmeltCount * 0.1f);
		SingletonCustom<SmeltFishing_UI>.Instance.SetProperSize(playingCharacter.PlayerNo, smeltValue);
		if (!isMoveNextSport)
		{
			if (caughtSmeltCount >= config.MaxLimitBiteSmeltCount)
			{
				isRollUp = true;
			}
			else if (playingCharacter.IsPlayer)
			{
				if (canRollUpTime < 0f)
				{
					if (SingletonCustom<SmeltFishing_Input>.Instance.IsPressButtonA(playingCharacter.ControlUser))
					{
						SingletonCustom<SmeltFishing_UI>.Instance.HideOperationInformation(playingCharacter.PlayerNo);
						isRollUp = !isRollUp;
					}
				}
				else
				{
					canRollUpTime -= Time.deltaTime;
				}
				if (!SingletonCustom<SmeltFishing_UI>.Instance.IsOnceOperationInformation(playingCharacter.PlayerNo, 2) && SingletonCustom<SmeltFishing_UI>.Instance.IsArrowGaugeCenterDown(playingCharacter.PlayerNo))
				{
					SingletonCustom<SmeltFishing_UI>.Instance.ShowOperationInformation(playingCharacter.PlayerNo, 2);
				}
			}
			else
			{
				rollUpNotHitTime -= Time.deltaTime;
				if (rollUpNotHitTime < 0f)
				{
					isRollUp = true;
					isMoveNextSport = true;
				}
				else
				{
					checkArrowProperChangeTime -= Time.deltaTime;
					if (checkArrowProperChangeTime < 0f)
					{
						checkArrowProperChangeTime = CHECK_ARROW_PROPER_CHANGE_TIME;
						ARROW_PROPER_DIFF_RANGE *= ARROW_PROPER_BRING_DIFF_VALUE;
					}
					if (caughtSmeltCount < config.MaxLimitBiteSmeltCount && !SingletonCustom<SmeltFishing_UI>.Instance.CheckHitRange(playingCharacter.PlayerNo, ARROW_PROPER_DIFF_RANGE))
					{
						if (SingletonCustom<SmeltFishing_UI>.Instance.IsDownDepthArrowToProperDepth(playingCharacter.PlayerNo))
						{
							if (!isRollUp)
							{
								isRollUp = true;
							}
						}
						else if (isRollUp)
						{
							isRollUp = false;
						}
					}
				}
			}
		}
		SingletonCustom<SmeltFishing_UI>.Instance.SetDepthArrowMoveDir(playingCharacter.PlayerNo, isRollUp);
		if (isRollUp)
		{
			fishingRod.HandleUpdateMethod(isRollUp);
			sfx.SetVolumeCache("se_fishing_roll_up", 1f);
		}
		else if (!SingletonCustom<SmeltFishing_UI>.Instance.IsArrowMaxDepthPosition(playingCharacter.PlayerNo))
		{
			fishingRod.HandleUpdateMethod(isRollUp);
		}
		else
		{
			sfx.SetVolumeCache("se_fishing_roll_up", 0f);
		}
		if (caughtSmeltCount < config.MaxLimitBiteSmeltCount && SingletonCustom<SmeltFishing_UI>.Instance.CheckHitRange(playingCharacter.PlayerNo))
		{
			if (playingCharacter.IsPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playingCharacter.PlayerNo);
			}
			CheckHit();
		}
		else
		{
			checkHitTime = defCheckHitTime;
		}
		fishing.UpdateMethod();
		if (!SingletonCustom<SmeltFishing_UI>.Instance.IsRollUpComplete(playingCharacter.PlayerNo))
		{
			return;
		}
		if (caughtSmeltCount > 0)
		{
			fishing.Catch(caughtSmeltCount);
		}
		else
		{
			fishing.Cancel();
			if (playingCharacter.IsPlayer)
			{
				SingletonCustom<SmeltFishing_UI>.Instance.HideAssistComment(playingCharacter.PlayerNo);
			}
		}
		fishing.RollUp();
		state = FishingState.RollUp;
	}
	private void CheckHit()
	{
		checkHitTime -= Time.deltaTime;
		if (!(checkHitTime < 0f))
		{
			return;
		}
		checkHitTime = defCheckHitTime;
		caughtSmeltCount++;
		UnityEngine.Debug.Log("Hit!!");
		sfx.PlayBiteSmeltSfx();
		SingletonCustom<SmeltFishing_UI>.Instance.ShowHit(playingCharacter.PlayerNo);
		SingletonCustom<SmeltFishing_UI>.Instance.PlayHitEffect(playingCharacter.PlayerNo);
		fishingSpace.PlayBubble();
		if (caughtSmeltCount == config.MaxLimitBiteSmeltCount)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.SetProperDepthRangeActive(playingCharacter.PlayerNo, _isActive: false);
			return;
		}
		if (playingCharacter.IsPlayer)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.ForceHideAssistComment(playingCharacter.PlayerNo);
			return;
		}
		checkArrowProperChangeTime = CHECK_ARROW_PROPER_CHANGE_TIME;
		ARROW_PROPER_DIFF_RANGE = UnityEngine.Random.Range(SmeltFishing_Definition.ARROW_PROPER_DIFF_RANGE[(int)AIStrength] - 15f, SmeltFishing_Definition.ARROW_PROPER_DIFF_RANGE[(int)AIStrength] + 15f);
		rollUpNotHitTime = ROLL_UP_NOT_HIT_TIME;
		if (caughtSmeltCount >= CAUGHT_SMELT_COUNT && playingCharacter.UseIcePlate.SmeltValue <= ROLL_UP_SMELT_VALUE && UnityEngine.Random.Range(0, 2) == 0)
		{
			isRollUp = true;
			isMoveNextSport = true;
		}
	}
	private void RollUp()
	{
		switch (fishing.CurrentStatus)
		{
		case SmeltFishing_Fishing.Status.Cancelled:
			fishingRod.RollUp();
			state = FishingState.RollUpped;
			break;
		case SmeltFishing_Fishing.Status.Caught:
			sfx.StopBiteSmeltSfx();
			fishingRod.ShowSmelts(fishing.CaughtSmeltCount);
			fishingRod.RollUp();
			state = FishingState.RollUpped;
			break;
		}
	}
	private void RollUpped()
	{
		if (fishingRod.IsPlayingAnimation)
		{
			return;
		}
		switch (fishing.CurrentStatus)
		{
		case SmeltFishing_Fishing.Status.Cancelled:
			sfx.StopRollupSfx();
			state = FishingState.ReadyFishingRod;
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeCastLine(playingCharacter.PlayerNo);
			break;
		case SmeltFishing_Fishing.Status.Caught:
			sfx.StopRollupSfx();
			fishingSpace.StopBubble();
			fishingRod.PlayIdleAnimation();
			sfx.PlayCaughtSmeltSfx(fishing.CaughtSmeltCount);
			caughtTime = Time.time;
			fishingRod.StopAnimation();
			playingCharacter.AddScore(fishing.CaughtSmeltCount);
			SingletonCustom<SmeltFishing_UI>.Instance.ShowGet(playingCharacter.PlayerNo, fishing.CaughtSmeltCount);
			UnityEngine.Debug.Log($"ワカサギゲット!! {fishing.CaughtSmeltCount}匹");
			state = FishingState.CaughtDelay;
			if (playingCharacter.IsPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playingCharacter.PlayerNo);
			}
			break;
		}
	}
	private void CaughtDelay()
	{
		if (caughtTime + 3f > Time.time)
		{
			return;
		}
		isCaught = true;
		state = FishingState.ReadyFishingRod;
		fishingRod.HideSmelts();
		UnityEngine.Debug.Log("caughtSmeltCount\u3000" + caughtSmeltCount.ToString());
		fishingSpace.AddSmelt(caughtSmeltCount);
		if (playingCharacter.IsPlayer)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeCastLine(playingCharacter.PlayerNo);
			if (!SingletonCustom<SmeltFishing_UI>.Instance.IsOnceAssistComment(playingCharacter.PlayerNo, 2) && playingCharacter.UseIcePlate.SmeltValue <= 0.5f)
			{
				SingletonCustom<SmeltFishing_UI>.Instance.ShowAssistComment(playingCharacter.PlayerNo, 2, isForceHide: true);
			}
		}
	}
	private void ReadyFishingRodForAI()
	{
		UnityEngine.Debug.Log("ReadyFishingRodForAI");
		if (!isCaught)
		{
			continueSameSpotCnt = 0;
			fishingRod.CastLine();
			SetCastLineStatus();
			previousActionTime = Time.time;
		}
		else if (isMoveNextSport || playingCharacter.UseIcePlate.SmeltValue <= 0.4f || (playingCharacter.UseIcePlate.SmeltValue <= NEXT_SPOT_SMELT_VALUE && UnityEngine.Random.Range(0f, 1f) <= NEXT_SPOT_PROBABILITY))
		{
			playingCharacter.StandUp();
			isCaught = false;
		}
		else
		{
			continueSameSpotCnt++;
			fishingRod.CastLine();
			SetCastLineStatus();
			previousActionTime = Time.time;
		}
	}
	private void CastLineForAI()
	{
		if (!(previousActionTime + 1f > Time.time))
		{
			showHit = false;
			fishingRod.PlayIdleAnimation();
			state = FishingState.WaitRollUp;
			previousActionTime = Time.time;
		}
	}
	private void WaitRollUpForAI()
	{
		if (fishing.CurrentStatus == SmeltFishing_Fishing.Status.FishFight)
		{
			switch (AIStrength)
			{
			case SmeltFishing_Definition.AIStrength.Easy:
				LeanTween.delayedCall(fishing.EscapeTime * UnityEngine.Random.Range(0.25f, 1.5f), (Action)delegate
				{
					if (fishing.CurrentStatus == SmeltFishing_Fishing.Status.FishFight)
					{
						fishing.RollUp();
						state = FishingState.RollUp;
					}
				});
				break;
			case SmeltFishing_Definition.AIStrength.Normal:
				LeanTween.delayedCall(fishing.EscapeTime * UnityEngine.Random.Range(0.25f, 1.5f), (Action)delegate
				{
					if (fishing.CurrentStatus == SmeltFishing_Fishing.Status.FishFight)
					{
						fishing.RollUp();
						state = FishingState.RollUp;
					}
				});
				break;
			case SmeltFishing_Definition.AIStrength.Hard:
				LeanTween.delayedCall(fishing.EscapeTime * UnityEngine.Random.Range(0.1f, 0.75f), (Action)delegate
				{
					if (fishing.CurrentStatus == SmeltFishing_Fishing.Status.FishFight)
					{
						fishing.RollUp();
						state = FishingState.RollUp;
					}
				});
				break;
			}
			return;
		}
		float baitDepth = fishing.BaitDepth;
		float properBaitDepth = fishing.ProperBaitDepth;
		switch (AIStrength)
		{
		case SmeltFishing_Definition.AIStrength.Easy:
		{
			float num2 = properBaitDepth + (float)UnityEngine.Random.Range(-1, 1);
			if (baitDepth > num2)
			{
				fishing.PullUp();
			}
			break;
		}
		case SmeltFishing_Definition.AIStrength.Normal:
		{
			float num3 = properBaitDepth + UnityEngine.Random.Range(0.5f, 1.5f);
			if (baitDepth > num3)
			{
				fishing.PullUp();
			}
			break;
		}
		case SmeltFishing_Definition.AIStrength.Hard:
		{
			float num = properBaitDepth + UnityEngine.Random.Range(1.25f, 1.75f);
			if (baitDepth > num)
			{
				fishing.PullUp();
			}
			break;
		}
		}
	}
}
