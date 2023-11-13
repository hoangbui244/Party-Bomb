using UnityEngine;
public class MonsterRace_CarAudio : MonoBehaviour
{
	private const float MIN_PITCH = 0.5f;
	private const float MAX_PITCH = 0.9f;
	private const float PITCH_BLUR_VALUE = 0.1f;
	private const float END_VOLUME_FADE_SPEED = 0.3f;
	private const float MAX_VOLUME = 1f;
	private const float CPU_VOLUME_MAG = 0.1f;
	private const float TIRE_VOLUME_MAG = 0.6f;
	private const float DRIFT_VOLUME_MAG = 2f;
	private const float DIRT_VOLUME_MAG = 2f;
	[SerializeField]
	private MonsterRace_CarScript car;
	[SerializeField]
	private AudioClip motorAccelClip;
	[SerializeField]
	private AudioClip edgeDirtClip;
	private AudioSource motorAccelSource;
	private AudioSource edgeDirtSource;
	private bool isPlay = true;
	private bool isEnd;
	private bool isPause;
	private Vector2 pitchPerlinVec2;
	private float pitchPerlinSpeed = 2f;
	public void Init()
	{
		if (!car.IsPlayer && SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum > 1)
		{
			isPlay = false;
			return;
		}
		motorAccelSource = SetUpEngineAudioSource(motorAccelClip);
		edgeDirtSource = SetUpEngineAudioSource(edgeDirtClip);
		pitchPerlinVec2 = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
	}
	public void UpdateMethod()
	{
		if (!isPlay)
		{
			return;
		}
		if (isEnd)
		{
			float volume = motorAccelSource.volume;
			volume -= Time.deltaTime * 0.3f;
			if (volume < 0f)
			{
				motorAccelSource.mute = true;
				edgeDirtSource.mute = true;
				isPlay = false;
				return;
			}
			motorAccelSource.volume = volume;
			if (car.IsEdgeDirt && car.IsGrounded)
			{
				edgeDirtSource.volume = volume * 2f;
			}
			else
			{
				edgeDirtSource.volume = 0f;
			}
			FixSystemVolume();
			return;
		}
		if (isPause && Time.timeScale > 0.1f)
		{
			isPause = false;
			motorAccelSource.enabled = true;
			edgeDirtSource.enabled = true;
		}
		if (Time.timeScale < 0.1f)
		{
			isPause = true;
			motorAccelSource.enabled = false;
			edgeDirtSource.enabled = false;
			return;
		}
		float num = Mathf.Lerp(0.5f, 0.9f, car.SpeedLerp);
		pitchPerlinVec2.x += pitchPerlinSpeed * Time.deltaTime;
		num += Mathf.Lerp(-0.1f, 0.1f, Mathf.PerlinNoise(pitchPerlinVec2.x, pitchPerlinVec2.y));
		motorAccelSource.pitch = num;
		float num2 = Mathf.Lerp(0f, 1f, car.SpeedLerp);
		if (!car.IsPlayer)
		{
			num2 *= 0.1f;
		}
		motorAccelSource.volume = num2;
		if (car.IsEdgeDirt && car.IsGrounded)
		{
			edgeDirtSource.volume = num2 * 2f;
		}
		else
		{
			edgeDirtSource.volume = 0f;
		}
		FixSystemVolume();
	}
	private AudioSource SetUpEngineAudioSource(AudioClip clip)
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
	private void FixSystemVolume()
	{
		float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
		motorAccelSource.volume *= num;
		edgeDirtSource.volume *= num;
	}
	public void EndAudio()
	{
		isEnd = true;
	}
}
