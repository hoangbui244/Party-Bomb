using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_AI : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("未来予知強度")]
	private float forwardOffset;
	[SerializeField]
	[DisplayName("入力切替間隔(秒)")]
	private float inputInterval;
	private FlyingSquirrelRace_Player owner;
	private float nextTime;
	private float latestTargetHeight;
	private float secondFromLatestTargetHeight;
	private float targetHeight;
	private float offset;
	private const float RELOTTERY_TIME = 3f;
	private float relotteryTime;
	public bool IsPressButtonA
	{
		get;
		private set;
	}
	public void Initialize(FlyingSquirrelRace_Player player)
	{
		owner = player;
		relotteryTime = 3f;
		SetLottery();
	}
	public void UpdateMethod()
	{
		if (!owner.IsUserControl)
		{
			relotteryTime -= Time.deltaTime;
			if (relotteryTime < 0f)
			{
				relotteryTime = 3f;
				SetLottery();
			}
			FlyingSquirrelRace_Stage stage = SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.GetStage(owner.Id);
			secondFromLatestTargetHeight = latestTargetHeight;
			latestTargetHeight = targetHeight;
			if (stage.GetTargetHeight(owner.Offset + offset, out float height))
			{
				targetHeight = height;
			}
			if (IsWrap() || nextTime <= Time.time)
			{
				IsPressButtonA = (base.transform.localPosition.y < targetHeight);
				nextTime = Time.time + inputInterval;
			}
		}
	}
	private bool IsWrap()
	{
		if (targetHeight < latestTargetHeight)
		{
			return secondFromLatestTargetHeight < latestTargetHeight;
		}
		if (targetHeight > latestTargetHeight)
		{
			return secondFromLatestTargetHeight > latestTargetHeight;
		}
		return false;
	}
	private void SetLottery()
	{
		switch (SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.AIStrength)
		{
		case FlyingSquirrelRace_Definition.AIStrength.Easy:
			forwardOffset = Random.Range(1f, 1.75f);
			inputInterval = Random.Range(0.2f, 0.4f);
			offset = forwardOffset + Random.Range(-0.3f, 0.3f);
			break;
		case FlyingSquirrelRace_Definition.AIStrength.Normal:
			forwardOffset = Random.Range(1f, 1.5f);
			inputInterval = Random.Range(0.15f, 0.3f);
			offset = forwardOffset + Random.Range(-0.2f, 0.2f);
			break;
		case FlyingSquirrelRace_Definition.AIStrength.Hard:
			forwardOffset = Random.Range(1f, 1.25f);
			inputInterval = Random.Range(0.1f, 0.2f);
			offset = forwardOffset + Random.Range(-0.1f, 0.1f);
			break;
		}
	}
}
