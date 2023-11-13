using UnityEngine;
public class Biathlon_CharacterAudio : MonoBehaviour
{
	private const float VolumeFadeOutSpeed = 0.3f;
	private const float MinPitch = 0.2f;
	private const float MaxPitch = 0.8f;
	private const float PitchBlurValue = 0.1f;
	private const float PitchPerlinSpeed = 2f;
	private const float CpuVolumeScale = 0.1f;
	[SerializeField]
	private Biathlon_CharacterAudioConfig config;
	private Biathlon_Character playingCharacter;
	private AudioSource glidingSource;
	private AudioSource glidingNoiseSource;
	private AudioSource windSource;
	private AudioSource shotSource;
	private AudioSource hitSource;
	private AudioSource missSource;
	private float baseVolume;
	private bool allowPlay = true;
	private bool isPause;
	private bool isGameEnd;
	private bool disallowGlidingSfx;
	private bool allowWindSfx;
	private Vector2 pitchPerlinNoise;
	public void Init(Biathlon_Character character)
	{
		playingCharacter = character;
		if (!playingCharacter.IsPlayer && SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum != 3)
		{
			allowPlay = false;
			return;
		}
		glidingSource = SetLoopAudioSource(config.GlidingClip);
		glidingNoiseSource = SetLoopAudioSource(config.GlidingNoiseClip);
		windSource = SetLoopAudioSource(config.WindClip);
		shotSource = SetUpOneShotAudioSource();
		hitSource = SetUpOneShotAudioSource();
		missSource = SetUpOneShotAudioSource();
		pitchPerlinNoise = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
	}
	public void UpdateMethod()
	{
		if (!allowPlay)
		{
			return;
		}
		if (isGameEnd)
		{
			baseVolume -= Time.deltaTime * 0.3f;
			if (baseVolume < 0f)
			{
				glidingSource.mute = true;
				windSource.mute = true;
				allowPlay = false;
				return;
			}
			glidingSource.volume = baseVolume * config.GlidingVolume;
			glidingNoiseSource.volume = baseVolume * config.GlidingNoiseVolume;
			if (allowWindSfx)
			{
				windSource.volume = baseVolume * config.WindVolume;
			}
			else
			{
				windSource.volume = 0f;
			}
			FixSystemVolume();
			return;
		}
		if (isPause && Time.timeScale > 0.1f)
		{
			isPause = false;
			glidingSource.enabled = true;
			windSource.enabled = true;
		}
		if (Time.timeScale < 0.1f)
		{
			isPause = true;
			glidingSource.enabled = false;
			windSource.enabled = false;
			return;
		}
		baseVolume = playingCharacter.Speed;
		if (!playingCharacter.IsPlayer)
		{
			baseVolume *= 0.1f;
		}
		glidingSource.volume = baseVolume * config.GlidingVolume;
		glidingNoiseSource.volume = baseVolume * config.GlidingNoiseVolume;
		if (allowWindSfx)
		{
			windSource.volume = baseVolume * config.WindVolume;
		}
		else
		{
			windSource.volume = 0f;
		}
		FixSystemVolume();
	}
	public void GameEnd()
	{
		isGameEnd = true;
	}
	public void StartGlidingSfx()
	{
		disallowGlidingSfx = false;
	}
	public void StopGlidingSfx()
	{
		disallowGlidingSfx = true;
	}
	public void StartWindSfx()
	{
		allowWindSfx = true;
	}
	public void StopWindSfx()
	{
		allowWindSfx = false;
	}
	public void PlayShotSfx()
	{
		if (allowPlay)
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			shotSource.volume = num * config.ShotVolume;
			shotSource.PlayOneShot(config.ShotClip);
		}
	}
	public void PlayHitSfx()
	{
		if (allowPlay)
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			shotSource.volume = num * config.HitVolume;
			shotSource.PlayOneShot(config.HitClip);
		}
	}
	public void PlayMissHitSfx()
	{
		if (allowPlay)
		{
			float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			shotSource.volume = num * config.MissVolume;
			shotSource.PlayOneShot(config.MissClip);
		}
	}
	private AudioSource SetLoopAudioSource(AudioClip clip)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0f;
		audioSource.loop = true;
		audioSource.time = UnityEngine.Random.Range(0f, clip.length);
		audioSource.Play();
		audioSource.minDistance = 1f;
		audioSource.dopplerLevel = 0f;
		return audioSource;
	}
	private AudioSource SetUpOneShotAudioSource()
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.volume = 0f;
		audioSource.loop = false;
		audioSource.playOnAwake = false;
		audioSource.minDistance = 1f;
		audioSource.dopplerLevel = 0f;
		audioSource.Stop();
		return audioSource;
	}
	private void FixSystemVolume()
	{
		float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
		glidingSource.volume *= num;
		windSource.volume *= num;
	}
}
