using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Player : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("移動管理")]
	private FlyingSquirrelRace_Movement movement;
	[SerializeField]
	[DisplayName("描画管理")]
	private FlyingSquirrelRace_Renderer renderer;
	[SerializeField]
	[DisplayName("オ\u30fcディオ")]
	private FlyingSquirrelRace_Audio audio;
	[SerializeField]
	[DisplayName("アニメ\u30fcション")]
	private FlyingSquirrelRace_PlayerAnimation animation;
	[SerializeField]
	[DisplayName("CPU[AI]")]
	private FlyingSquirrelRace_AI ai;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private FlyingSquirrelRace_PlayerConfig config;
	[Header("デバッグ表示")]
	[SerializeField]
	[Disable(false)]
	[DisplayName("ID")]
	private int id;
	[SerializeField]
	[Disable(false)]
	[DisplayName("操作ユ\u30fcザ\u30fc")]
	private FlyingSquirrelRace_Definition.Controller ctrl;
	[SerializeField]
	[Disable(true)]
	[DisplayName("スコア")]
	private int score;
	private float lastContactTime = float.NegativeInfinity;
	public int Id => id;
	public FlyingSquirrelRace_Definition.Controller Controller => ctrl;
	public int Score => score;
	public bool IsUserControl => ctrl < FlyingSquirrelRace_Definition.Controller.Cpu1;
	public bool IsGodTime => Time.time < lastContactTime + config.GodTimeWhenAfterMiss;
	public bool IsPreGoal
	{
		get;
		private set;
	}
	public bool IsGoal
	{
		get;
		private set;
	}
	public float Offset => base.transform.localPosition.x;
	public void Initialize(int playerNo, FlyingSquirrelRace_Definition.Controller controller)
	{
		id = playerNo;
		ctrl = controller;
		movement.Initialize(this);
		renderer.Initialize(this);
		audio.Initialize(this);
		ai.Initialize(this);
		animation.Initialize(this);
	}
	public void UpdateMethod()
	{
		if (!IsGoal)
		{
			if (IsUserControl)
			{
				movement.UpdateMethod();
				renderer.UpdateMethod();
			}
			else
			{
				ai.UpdateMethod();
				movement.UpdateForAIMethod(ai);
				renderer.UpdateForAIMethod(ai);
			}
		}
	}
	public void Rise()
	{
		audio.PlayRiseSfx();
	}
	public void CollectCoin(FlyingSquirrelRace_OvalCoin coin)
	{
		score += coin.Score;
		renderer.PlayCoinCollectEffect();
		audio.PlayCollectCoinSfx();
		SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.UpdateScore(score, Id);
	}
	public void CollectScroll()
	{
		audio.PlaySpeedUpSfx();
		renderer.PlaySpeedUpEffect();
	}
	public void ContactObstacle(FlyingSquirrelRace_ObstacleObject obstacle)
	{
		if (!IsGodTime)
		{
			lastContactTime = Time.time;
			renderer.PlayFlash(config.GodTimeWhenAfterMiss);
			audio.PlayContactObstacleSfx();
			renderer.PlayobstacleBadEffect();
			if (obstacle.GetObstacleType() == FlyingSquirrelRace_ObstacleObject.ObstacleType.Pile)
			{
				renderer.PlayObstaclePileHitEffect();
			}
			score -= obstacle.PenaltyScore;
			score = Mathf.Max(score, 0);
			SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.UpdateScore(score, Id);
			if (IsUserControl)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(Id);
			}
		}
	}
	public void PreGoal()
	{
		if (!IsPreGoal)
		{
			IsPreGoal = true;
			SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.PreGoalPlayer(Id);
		}
	}
	public void Goal()
	{
		if (!IsGoal)
		{
			IsGoal = true;
			int num = SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GoalPlayer(Id);
			score += config.GoalScore[num];
			SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.UpdateScore(score, Id);
			if (IsUserControl)
			{
				SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GoalControlUserPlayer();
			}
		}
	}
}
