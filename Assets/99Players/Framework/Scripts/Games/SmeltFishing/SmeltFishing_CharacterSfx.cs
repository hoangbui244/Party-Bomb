using System;
using System.Collections.Generic;
using UnityEngine;
public class SmeltFishing_CharacterSfx : MonoBehaviour
{
	[SerializeField]
	private List<AudioClip> audioClips = new List<AudioClip>();
	private readonly Dictionary<string, AudioClip> clipIndex = new Dictionary<string, AudioClip>();
	private readonly Dictionary<string, AudioSource> sources = new Dictionary<string, AudioSource>();
	private readonly Dictionary<string, float> volumeCache = new Dictionary<string, float>();
	private bool isActive;
	public void Init()
	{
		foreach (AudioClip audioClip in audioClips)
		{
			clipIndex[audioClip.name] = audioClip;
		}
	}
	public void GameStart()
	{
		isActive = true;
	}
	public void GameEnd()
	{
		isActive = false;
		foreach (AudioSource value in sources.Values)
		{
			value.Stop();
		}
	}
	public void UpdateMethod()
	{
		if (isActive)
		{
			foreach (AudioSource value in sources.Values)
			{
				value.volume = volumeCache[value.clip.name] * ((float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f);
			}
		}
	}
	public void Disable()
	{
		isActive = false;
	}
	public void PlayWalkSfx()
	{
		if (isActive)
		{
			PlaySfx("se_run", 0.2f);
		}
	}
	public void PlayWalkSfxWithInterval(float walkTime)
	{
		if (isActive && walkTime > 1f)
		{
			PlayWalkSfx();
		}
	}
	public void PlayCastLineSfx()
	{
		if (isActive)
		{
			PlaySfx("se_fishing_roll_up", 1f, isLoop: true);
			PlaySfx("se_fishing_water_sink", 1f, isLoop: false, 0.05f);
		}
	}
	public void StopCastLineSfx()
	{
		StopSfx("se_fishing_roll_up");
	}
	public void PlayBiteSmeltSfx()
	{
		if (isActive)
		{
			PlaySfx("se_fishing_bite_1", 1f, isLoop: true);
			PlaySfx("se_fishing_bite_2", 1f, isLoop: true);
			PlaySfx("se_fishing_bite_3");
		}
	}
	public void StopBiteSmeltSfx()
	{
		if (isActive)
		{
			StopSfx("se_fishing_bite_1");
			StopSfx("se_fishing_bite_2");
		}
	}
	public void PlayPullUpSfx()
	{
		if (isActive)
		{
			PlaySfx("se_fishing_roll_up_short");
		}
	}
	public void PlayRollUpSfx()
	{
		if (isActive)
		{
			PlaySfx("se_fishing_roll_up", 1f, isLoop: true);
		}
	}
	public void StopRollupSfx()
	{
		if (isActive)
		{
			StopSfx("se_fishing_roll_up");
		}
	}
	public void PlayCaughtSmeltSfx(int smeltCount)
	{
		if (isActive)
		{
			if (smeltCount >= 5)
			{
				PlaySfx("se_fishing_caught_many");
			}
			else if (smeltCount >= 3)
			{
				PlaySfx("se_fishing_caught_some");
			}
			else if (smeltCount >= 1)
			{
				PlaySfx("se_fishing_caught_few");
			}
			else
			{
				PlaySfx("se_fishing_caught_empty");
			}
		}
	}
	private void PlaySfx(string clipName, float volume = 1f, bool isLoop = false, float delay = 0f)
	{
		UnityEngine.Debug.Log("ＳE再生:" + clipName);
		if (!sources.TryGetValue(clipName, out AudioSource value))
		{
			value = base.gameObject.AddComponent<AudioSource>();
			value.playOnAwake = false;
			value.clip = GetClip(clipName);
			sources[clipName] = value;
		}
		volumeCache[clipName] = volume;
		value.volume = volume * ((float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f);
		value.loop = isLoop;
		value.PlayDelayed(delay);
	}
	private void StopSfx(string clipName, float delay = 0f)
	{
		if (sources.TryGetValue(clipName, out AudioSource source))
		{
			if (Mathf.Approximately(delay, 0f))
			{
				source.Stop();
			}
			else
			{
				LeanTween.delayedCall(delay, (Action)delegate
				{
					source.Stop();
				});
			}
		}
	}
	public void SetVolumeCache(string clipName, float volume)
	{
		volumeCache[clipName] = volume;
	}
	private AudioClip GetClip(string clipName)
	{
		return clipIndex[clipName];
	}
}
