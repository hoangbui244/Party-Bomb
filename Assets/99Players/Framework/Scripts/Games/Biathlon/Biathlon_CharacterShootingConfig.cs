using UnityEngine;
using UnityEngine.Extension;
public class Biathlon_CharacterShootingConfig : ScriptableObject
{
	[Header("レティクルの移動設定")]
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed;
	[SerializeField]
	[DisplayName("フレ\u30fcム辺りの移動速度")]
	private float deltaSpeed;
	[SerializeField]
	[DisplayName("ブレの大きさ")]
	private float noiseScale = 2f;
	[SerializeField]
	[DisplayName("ぶれる速度")]
	private float noiseSpeed = 2f;
	[SerializeField]
	[DisplayName("CPUのノイズへの対抗値")]
	private float cpuNoiseCanceler = 0.5f;
	[Header("射撃設定")]
	[SerializeField]
	[DisplayName("撃った球が到達するまでの時間")]
	private float shootTime = 0.1f;
	[SerializeField]
	[DisplayName("次の弾までに必要な時間")]
	private float shootInterval = 1f;
	[SerializeField]
	[DisplayName("外した場合に次の弾までに必要な時間")]
	private float shootIntervalByMiss = 1.5f;
	public float MoveSpeed => moveSpeed;
	public float DeltaSpeed => deltaSpeed;
	public float NoiseScale => noiseScale;
	public float NoiseSpeed => noiseSpeed;
	public float CpuNoiseCanceler => cpuNoiseCanceler;
	public float ShootTime => shootTime;
	public float ShootInterval => shootInterval;
	public float ShootIntervalByMiss => shootIntervalByMiss;
}
