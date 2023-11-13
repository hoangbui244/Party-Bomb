using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_AudioConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("上昇ＳＥ")]
	private AudioClip riseClip;
	[SerializeField]
	[DisplayName("スコアゲットＳＥ")]
	private AudioClip collectCoinClip;
	[SerializeField]
	[DisplayName("スコアダウンSE")]
	private AudioClip contactObstacleClip;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップSE（接触音）")]
	private AudioClip getSpeedUpClip;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップSE（加速音）")]
	private AudioClip speedUpClip;
	[SerializeField]
	[DisplayName("上昇ＳＥボリュ\u30fcム")]
	private float riseVolume = 1f;
	[SerializeField]
	[DisplayName("スコアゲットＳＥボリュ\u30fcム")]
	private float collectCoinVolume = 1f;
	[SerializeField]
	[DisplayName("スコアダウンSE")]
	private float contactObstacleVolume = 1f;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップSEボリュ\u30fcム（接触音）")]
	private float getSpeedUpVolume = 1f;
	[SerializeField]
	[DisplayName("スピ\u30fcドアップSEボリュ\u30fcム（加速音）")]
	private float speedUpVolume = 1f;
	public AudioClip RiseClip => riseClip;
	public AudioClip CollectCoinClip => collectCoinClip;
	public AudioClip ContactObstacleClip => contactObstacleClip;
	public AudioClip GetSpeedUpClip => getSpeedUpClip;
	public AudioClip SpeedUpClip => speedUpClip;
	public float RiseVolume => riseVolume;
	public float CollectCoinVolume => collectCoinVolume;
	public float ContactObstacleVolume => contactObstacleVolume;
	public float GetSpeedUpVolume => getSpeedUpVolume;
	public float SpeedUpVolume => speedUpVolume;
}
