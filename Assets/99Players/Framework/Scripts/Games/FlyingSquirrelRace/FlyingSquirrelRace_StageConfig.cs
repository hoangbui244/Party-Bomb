using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Serialization;
public class FlyingSquirrelRace_StageConfig : DecoratedScriptableObject
{
	public const int AdditionalStageSize = 2;
	[SerializeField]
	[DisplayName("通常時の速度")]
	private float baseSpeed = 2f;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップ時の速度")]
	private float boostedSpeed = 3f;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップ持続時間")]
	private float boostDuration = 5f;
	[SerializeField]
	[DisplayName("スピ\u30fcドダウン量")]
	private float speedDebuff = 0.25f;
	[SerializeField]
	[DisplayName("スピ\u30fcドダウン持続時間")]
	private float speedDebuffDuration = 1f;
	[SerializeField]
	[DisplayName("最低速度")]
	private float minSpeed = 1f;
	[FormerlySerializedAs("speedBuff")]
	[SerializeField]
	[DisplayName("スピ\u30fcドアップスピ\u30fcド")]
	private float speedUpBuff = 0.1f;
	[SerializeField]
	[DisplayName("ステ\u30fcジサイズ")]
	private int stageSize = 10;
	public float BaseSpeed => baseSpeed;
	public float BoostedSpeed => boostedSpeed;
	public float BoostDuration => boostDuration;
	public float SpeedDebuff => speedDebuff;
	public float SpeedDebuffDuration => speedDebuffDuration;
	public float MinSpeed => minSpeed;
	public float SpeedUpBuff => speedUpBuff;
	public int StageSize => stageSize + 2;
}
