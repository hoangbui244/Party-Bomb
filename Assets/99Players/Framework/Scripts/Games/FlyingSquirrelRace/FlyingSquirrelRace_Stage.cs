using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Stage : DecoratedMonoBehaviour
{
	private static readonly string[] Layers = new string[4]
	{
		"Collision_Obj_1",
		"Collision_Obj_2",
		"Collision_Obj_3",
		"Collision_Obj_4"
	};
	[SerializeField]
	[DisplayName("ステ\u30fcジ背景")]
	private FlyingSquirrelRace_Backgrounds backgrounds;
	[SerializeField]
	[DisplayName("ル\u30fcトオブジェクト")]
	private Transform stageObjectsRoot;
	[SerializeField]
	[DisplayName("スタ\u30fcト足場")]
	private Transform startStand;
	[SerializeField]
	[DisplayName("ゴ\u30fcル足場")]
	private Transform goalStand;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private FlyingSquirrelRace_StageConfig config;
	[Header("デバッグ表示")]
	[SerializeField]
	[Disable(true)]
	[DisplayName("現在の速度")]
	private float speed = 2f;
	private int layer;
	private int index;
	private int owner;
	private float boostUpEndTime;
	private float speedDownEndTime;
	private bool stopStageScroll;
	private FlyingSquirrelRace_StagePattern[] stagePatterns;
	private const float FIRST_ADD_STAGE_PATTERN_DISTANCE = -5f;
	private float nextAddStagePatternDistance;
	public FlyingSquirrelRace_Player Player
	{
		get;
		private set;
	}
	public void Initialize(int playerNo)
	{
		owner = playerNo;
		layer = LayerMask.NameToLayer(Layers[playerNo]);
		backgrounds.Initialize();
		speed = config.BaseSpeed;
		stagePatterns = new FlyingSquirrelRace_StagePattern[config.StageSize];
		Player = SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GetPlayer(playerNo);
	}
	public void PostInitialize()
	{
	}
	public void AddPattern(FlyingSquirrelRace_StagePattern prefab)
	{
		FlyingSquirrelRace_StagePattern flyingSquirrelRace_StagePattern = Object.Instantiate(prefab, stageObjectsRoot);
		flyingSquirrelRace_StagePattern.transform.Identity();
		flyingSquirrelRace_StagePattern.transform.LocalPosition((Vector3 p) => p.X(index * 10));
		flyingSquirrelRace_StagePattern.Initialize(this, layer, index);
		stagePatterns[index] = flyingSquirrelRace_StagePattern;
		nextAddStagePatternDistance = ((index == 0) ? (-5f) : (-5f + (float)index * -10f));
		index++;
	}
	public void StopStageScroll()
	{
		stopStageScroll = true;
	}
	public void UpdateMethod()
	{
		if (!stopStageScroll)
		{
			FixSpeedIfNeeded();
			backgrounds.UpdateMethod(speed);
		}
	}
	public void FixedUpdateMethod()
	{
		if (Player.IsGoal)
		{
			return;
		}
		if (CheckNextAddPattern())
		{
			FlyingSquirrelRace_StagePattern stagePattern = SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.GetStagePattern(index);
			AddPattern(stagePattern);
		}
		Move(speed);
		FlyingSquirrelRace_StagePattern[] array = stagePatterns;
		foreach (FlyingSquirrelRace_StagePattern flyingSquirrelRace_StagePattern in array)
		{
			if (!(flyingSquirrelRace_StagePattern == null) && !flyingSquirrelRace_StagePattern.IsDeactivateDistance)
			{
				flyingSquirrelRace_StagePattern.FixedUpdateMethod(speed);
			}
		}
	}
	private void Move(float speed)
	{
		Vector3 v3 = new Vector3(speed, 0f, 0f) * Time.fixedDeltaTime;
		stageObjectsRoot.LocalPosition(v3, (Vector3 v1, Vector3 v2) => v1 - v2);
	}
	private bool CheckNextAddPattern()
	{
		if (index == stagePatterns.Length)
		{
			return false;
		}
		return stageObjectsRoot.localPosition.x <= nextAddStagePatternDistance;
	}
	public Transform GetStageObjectsRoot()
	{
		return stageObjectsRoot;
	}
	public void CollectScroll()
	{
		speed = config.BoostedSpeed;
		speedDownEndTime = -1f;
		boostUpEndTime = config.BoostDuration + Time.time;
	}
	public void ContactObstacle(FlyingSquirrelRace_ObstacleObject obstacle)
	{
		if (obstacle.IsSpeedDown)
		{
			if (Mathf.Approximately(speed, config.BoostedSpeed))
			{
				speed = config.BaseSpeed;
				boostUpEndTime = -1f;
			}
			else
			{
				speed -= config.SpeedDebuff;
				speedDownEndTime = config.SpeedDebuffDuration + Time.time;
			}
			speed = Mathf.Max(speed, config.MinSpeed);
		}
	}
	public bool GetTargetHeight(float offset, out float height)
	{
		for (int i = 0; i < stagePatterns.Length; i++)
		{
			FlyingSquirrelRace_StagePattern flyingSquirrelRace_StagePattern = stagePatterns[i];
			if (!(flyingSquirrelRace_StagePattern == null) && flyingSquirrelRace_StagePattern.IsCurrent)
			{
				height = flyingSquirrelRace_StagePattern.GetTargetHeight(offset);
				return true;
			}
		}
		height = 0f;
		return false;
	}
	public bool TargetBaseSpeed()
	{
		if (speed > config.BaseSpeed)
		{
			speed -= config.SpeedUpBuff * Time.deltaTime;
		}
		else if (speed < config.BaseSpeed)
		{
			speed += config.SpeedUpBuff * Time.deltaTime;
		}
		return Mathf.Approximately(speed, config.BaseSpeed);
	}
	private void FixSpeedIfNeeded()
	{
		FixBoostSpeedIfNeeded();
		FixSpeedDownIfNeeded();
	}
	private void FixBoostSpeedIfNeeded()
	{
		if (!(Time.time < boostUpEndTime) && !(speed <= config.BaseSpeed))
		{
			speed -= config.SpeedUpBuff * Time.fixedDeltaTime;
			Mathf.Max(speed, config.BaseSpeed);
		}
	}
	private void FixSpeedDownIfNeeded()
	{
		if (!(Time.time > speedDownEndTime) && !(speed >= config.BaseSpeed))
		{
			speed += config.SpeedUpBuff * Time.fixedDeltaTime;
			Mathf.Min(speed, config.BaseSpeed);
		}
	}
}
