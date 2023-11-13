using System.Collections;
using UnityEngine;
public class SmeltFishing_Fishing : MonoBehaviour
{
	public enum Status
	{
		Waiting,
		WaitingBiteSmelt,
		FishFight,
		Cancelled,
		Caught,
		Escaped
	}
	[SerializeField]
	private SmeltFishing_FishingConfig config;
	private SmeltFishing_Character playingCharacter;
	private Coroutine fishingCoroutine;
	private Status currentStatus;
	private bool rollUpped;
	private float properBaitDepth;
	private float baitDepth;
	private Vector3 sitDownPosition;
	private SmeltFishing_IcePlate currentIcePlate;
	public Status CurrentStatus => currentStatus;
	public int CaughtSmeltCount
	{
		get;
		private set;
	}
	public float BaitDepth => baitDepth;
	public float ProperBaitDepth => properBaitDepth;
	public float EscapeTime
	{
		get;
		private set;
	}
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
		currentStatus = Status.Waiting;
	}
	public void SetFishingPosition(SmeltFishing_IcePlate icePlate)
	{
		currentIcePlate = icePlate;
	}
	public void UpdateMethod()
	{
		if (currentStatus == Status.WaitingBiteSmelt)
		{
			baitDepth += config.BaitFallSpeed * Time.deltaTime;
			baitDepth = Mathf.Min(baitDepth, config.MaxBaitDepth);
			SingletonCustom<SmeltFishing_UI>.Instance.SetDepth(playingCharacter.PlayerNo, baitDepth);
		}
	}
	public void CastLine()
	{
		if (fishingCoroutine != null)
		{
			UnityEngine.Debug.LogError("釣り中です");
			return;
		}
		rollUpped = false;
		properBaitDepth = Mathf.Lerp(config.MinBaitProperDepth, config.MaxBaitProperDepth, UnityEngine.Random.value);
		baitDepth = config.DefaultBaitDepth;
		fishingCoroutine = StartCoroutine(Fishing());
		SingletonCustom<SmeltFishing_UI>.Instance.SetProperDepth(playingCharacter.PlayerNo, properBaitDepth);
		SingletonCustom<SmeltFishing_UI>.Instance.SetDepthImmediate(playingCharacter.PlayerNo, baitDepth);
		float smeltValue = Mathf.Clamp01(playingCharacter.UseIcePlate.SmeltValue);
		SingletonCustom<SmeltFishing_UI>.Instance.ShowBaitDepth(playingCharacter.PlayerNo, smeltValue);
	}
	public void PullUp()
	{
		baitDepth = Mathf.Max(baitDepth - config.BaitPullPower, config.MinBaitDepth);
		SingletonCustom<SmeltFishing_UI>.Instance.SetDepth(playingCharacter.PlayerNo, baitDepth);
	}
	public void RollUp()
	{
		rollUpped = true;
		SingletonCustom<SmeltFishing_UI>.Instance.HideBaitDepth(playingCharacter.PlayerNo);
	}
	private IEnumerator Fishing()
	{
		float elapsed2 = 0f;
		currentStatus = Status.WaitingBiteSmelt;
		float biteTime = config.GetRandomBiteTime();
		while (elapsed2 < biteTime)
		{
			if (rollUpped)
			{
				Cancel();
				yield break;
			}
			float num = Mathf.Abs(baitDepth - properBaitDepth);
			float num2 = (num < config.BonusDepthRange) ? 2f : ((num < config.HalfBonusDepthRange) ? 1.5f : 1f);
			elapsed2 += Time.deltaTime * num2;
			yield return null;
		}
		currentStatus = Status.FishFight;
		int biteSmeltCount = config.GetRandomBiteSmeltCount(currentIcePlate.SmeltValue);
		float escapeTime = EscapeTime = config.GetRandomSmeltEscapeTime(biteSmeltCount);
		elapsed2 = 0f;
		while (elapsed2 < escapeTime)
		{
			if (rollUpped)
			{
				int bonusBiteSmeltCount = config.GetRandomBonusBiteSmeltCount(biteSmeltCount);
				if (elapsed2 >= escapeTime * config.BonusBiteSmeltTimeLimitRate)
				{
					bonusBiteSmeltCount = 0;
				}
				Catch(biteSmeltCount, bonusBiteSmeltCount);
				yield break;
			}
			elapsed2 += Time.deltaTime;
			yield return null;
		}
		Escape();
	}
	public void Cancel()
	{
		currentStatus = Status.Cancelled;
		fishingCoroutine = null;
	}
	public void Catch(int biteSmeltCount, int bonusBiteSmeltCount = 0)
	{
		currentStatus = Status.Caught;
		CaughtSmeltCount = biteSmeltCount + bonusBiteSmeltCount;
		fishingCoroutine = null;
		currentIcePlate.SubtractValue(CaughtSmeltCount);
	}
	private void Escape()
	{
		currentStatus = Status.Escaped;
		CaughtSmeltCount = 0;
		fishingCoroutine = null;
		SingletonCustom<SmeltFishing_UI>.Instance.HideBaitDepth(playingCharacter.PlayerNo);
	}
}
