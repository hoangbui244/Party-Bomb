using UnityEngine;
using UnityEngine.Extension;
public class Biathlon_CharacterMovementConfig : ScriptableObject
{
	[Header("基本的な速度設定")]
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed = 7f;
	[SerializeField]
	[DisplayName("弱いCPUの速度補正")]
	private float easyCpuMoveSpeedMultiplier = 0.9f;
	[SerializeField]
	[DisplayName("普通CPUの速度補正")]
	private float normalCpuMoveSpeedMultiplier = 0.95f;
	[SerializeField]
	[DisplayName("強いCPUの速度補正")]
	private float hardCpuMoveSpeedMultiplier = 1f;
	[SerializeField]
	private float[] moveSpeedCorrectionsByPlacement;
	[Header("基本的な回転設定")]
	[SerializeField]
	[DisplayName("回転速度")]
	private float angularSpeed = 60f;
	[SerializeField]
	[DisplayName("速度補正の変更する速さ")]
	private float moveSpeedCorrectionChangeSpeed = 1f;
	[SerializeField]
	[DisplayName("地形に沿った回転速度")]
	private float rotationSpeed = 3f;
	[Header("スリップストリ\u30fcム(SS)の設定")]
	[SerializeField]
	[DisplayName("SSの速度ボ\u30fcナス")]
	private float slipStreamBonusSpeed = 1f;
	[SerializeField]
	[DisplayName("SSのチャ\u30fcジ速度")]
	private float slipstreamChargeSpeed = 1f;
	[SerializeField]
	[DisplayName("SSの最大チャ\u30fcジ値")]
	private float slipstreamChargeMax = 5f;
	[SerializeField]
	[DisplayName("SSの発生するチャ\u30fcジ値")]
	private float slipStreamBonusUseThreshold = 2f;
	[SerializeField]
	[DisplayName("SSの判定距離")]
	private float slipstreamSphereLength = 1f;
	[SerializeField]
	[DisplayName("SSの判定サイズ")]
	private float slipstreamSphereRadius = 1f;
	[Header("坂の補正設定")]
	[SerializeField]
	[DisplayName("下り坂の速度補正")]
	private float downwardSpeedCorrection = 1f;
	[SerializeField]
	[DisplayName("上り坂の速度補正")]
	private float upwardSpeedCorrection = -2f;
	public float MoveSpeed => moveSpeed;
	public float EasyCpuMoveSpeedMultiplier => easyCpuMoveSpeedMultiplier;
	public float NormalCpuMoveSpeedMultiplier => normalCpuMoveSpeedMultiplier;
	public float HardCpuMoveSpeedMultiplier => hardCpuMoveSpeedMultiplier;
	public float[] MoveSpeedCorrectionsByPlacement => moveSpeedCorrectionsByPlacement;
	public float MoveSpeedCorrectionChangeSpeed => moveSpeedCorrectionChangeSpeed;
	public float AngularSpeed => angularSpeed;
	public float RotationSpeed => rotationSpeed;
	public float SlipStreamBonusSpeed => slipStreamBonusSpeed;
	public float SlipstreamChargeSpeed => slipstreamChargeSpeed;
	public float SlipstreamChargeMax => slipstreamChargeMax;
	public float SlipStreamBonusUseThreshold => slipStreamBonusUseThreshold;
	public float SlipstreamSphereLength => slipstreamSphereLength;
	public float SlipstreamSphereRadius => slipstreamSphereRadius;
	public float DownwardSpeedCorrection => downwardSpeedCorrection;
	public float UpwardSpeedCorrection => upwardSpeedCorrection;
}
