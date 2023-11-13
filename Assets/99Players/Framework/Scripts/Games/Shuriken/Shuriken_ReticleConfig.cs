using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ReticleConfig : DecoratedScriptableObject
{
	[Header("操作設定")]
	[SerializeField]
	[DisplayName("レティクルの移動速度")]
	private float reticleSpeed = 10f;
	[SerializeField]
	[DisplayName("レティクルの左下座標")]
	private Vector2 minReticlePosition = Vector2.zero;
	[SerializeField]
	[DisplayName("レティクルの右上座標")]
	private Vector2 maxReticlePosition = new Vector2(1920f, 1080f);
	[Header("CPUに関する設定")]
	[SerializeField]
	[DisplayName("レティクルが吸い付く距離")]
	private float aimStickDistanceThreshold = 0.1f;
	[SerializeField]
	[DisplayName("[Easy]が取得する付近の的の数")]
	private int easyCpuTakeTargetsCount = 6;
	[SerializeField]
	[DisplayName("[Normal]が取得する付近の的の数")]
	private int normalCpuTakeTargetsCount = 5;
	[SerializeField]
	[DisplayName("[Hard]が取得する付近の的の数")]
	private int hardCpuTakeTargetsCount = 4;
	[SerializeField]
	[DisplayName("レティクル移動方向変更速度")]
	private float easyAimVectorSpeed = 2f;
	[SerializeField]
	[DisplayName("レティクル移動方向変更速度")]
	private float normalAimVectorSpeed = 2f;
	[SerializeField]
	[DisplayName("レティクル移動方向変更速度")]
	private float hardAimVectorSpeed = 2f;
	[Header("UI設定")]
	[SerializeField]
	[DisplayName("レティクルの色")]
	private Color[] colors;
	[SerializeField]
	[DisplayName("CPUでもレティクル表示")]
	private bool showCpuReticle;
	public float ReticleSpeed => reticleSpeed;
	public Vector2 MinReticlePosition => minReticlePosition;
	public Vector2 MaxReticlePosition => maxReticlePosition;
	public float AimStickDistanceThreshold => aimStickDistanceThreshold;
	public int EasyCpuTakeTargetsCount => easyCpuTakeTargetsCount;
	public int NormalCpuTakeTargetsCount => normalCpuTakeTargetsCount;
	public int HardCpuTakeTargetsCount => hardCpuTakeTargetsCount;
	public float EasyAimVectorSpeed => easyAimVectorSpeed;
	public float NormalAimVectorSpeed => normalAimVectorSpeed;
	public float HardAimVectorSpeed => hardAimVectorSpeed;
	public bool ShowCpuReticle => showCpuReticle;
	public Color GetColor(int playerNo)
	{
		return colors[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(playerNo >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) ? (4 + (playerNo - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerNo]];
	}
	public int GetTakeTargetsCount(Shuriken_Definition.AIStrength strength)
	{
		switch (strength)
		{
		case Shuriken_Definition.AIStrength.Easy:
			return easyCpuTakeTargetsCount;
		case Shuriken_Definition.AIStrength.Normal:
			return normalCpuTakeTargetsCount;
		case Shuriken_Definition.AIStrength.Hard:
			return hardCpuTakeTargetsCount;
		default:
			return normalCpuTakeTargetsCount;
		}
	}
	public float GetAimVectorDeltaSpeed(Shuriken_Definition.AIStrength strength)
	{
		switch (strength)
		{
		case Shuriken_Definition.AIStrength.Easy:
			return easyAimVectorSpeed;
		case Shuriken_Definition.AIStrength.Normal:
			return normalAimVectorSpeed;
		case Shuriken_Definition.AIStrength.Hard:
			return hardAimVectorSpeed;
		default:
			return normalAimVectorSpeed;
		}
	}
}
