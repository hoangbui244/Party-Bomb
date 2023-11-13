using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Audio : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("コンフィグ")]
	private FlyingSquirrelRace_AudioConfig config;
	private AudioSource riseSource;
	private AudioSource collectCoinSource;
	private AudioSource getSpeedUpSource;
	private AudioSource speedUpSource;
	private AudioSource contactObstacleSource;
	private FlyingSquirrelRace_Player owner;
	private bool AllowCpuSfx => SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.ControlUserNum == 3;
	private bool IsDuringGame => SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState == FlyingSquirrelRace_Definition.GameState.DuringGame;
	public void Initialize(FlyingSquirrelRace_Player player)
	{
		riseSource = AddAudioSource();
		collectCoinSource = AddAudioSource();
		getSpeedUpSource = AddAudioSource();
		speedUpSource = AddAudioSource();
		contactObstacleSource = AddAudioSource();
		owner = player;
	}
	public void PlayRiseSfx()
	{
		if (!owner.IsGoal && IsDuringGame && (owner.IsUserControl || AllowCpuSfx))
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			riseSource.volume = num * config.RiseVolume;
			riseSource.PlayOneShot(config.RiseClip);
		}
	}
	public void PlayCollectCoinSfx()
	{
		if (!owner.IsGoal && IsDuringGame && (owner.IsUserControl || AllowCpuSfx))
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			collectCoinSource.volume = num * config.CollectCoinVolume;
			collectCoinSource.PlayOneShot(config.CollectCoinClip);
		}
	}
	public void PlaySpeedUpSfx()
	{
		if (!owner.IsGoal && IsDuringGame && (owner.IsUserControl || AllowCpuSfx))
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			getSpeedUpSource.volume = num * config.GetSpeedUpVolume;
			getSpeedUpSource.PlayOneShot(config.GetSpeedUpClip);
			speedUpSource.volume = num * config.SpeedUpVolume;
			speedUpSource.PlayOneShot(config.SpeedUpClip);
		}
	}
	public void PlayContactObstacleSfx()
	{
		if (!owner.IsGoal && IsDuringGame && (owner.IsUserControl || AllowCpuSfx))
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			contactObstacleSource.volume = num * config.ContactObstacleVolume;
			contactObstacleSource.PlayOneShot(config.ContactObstacleClip);
		}
	}
	private AudioSource AddAudioSource()
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		return audioSource;
	}
}
