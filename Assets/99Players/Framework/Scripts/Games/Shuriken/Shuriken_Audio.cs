using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_Audio : SingletonMonoBehaviour<Shuriken_Audio>
{
	[SerializeField]
	[DisplayName("プレイヤ\u30fc用のクリップ")]
	private AudioClip[] playerClips;
	[SerializeField]
	[DisplayName("プレイヤ\u30fc以外のクリップ")]
	private AudioClip[] commonClips;
	private Dictionary<string, AudioClip> indexedClips;
	private Dictionary<string, AudioSource[]> playerAudioSources;
	private Dictionary<string, AudioSource> commonAudioSources;
	private List<AudioSource> listedSources;
	public void Initialize()
	{
		int num = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.ControlUsers.Length;
		indexedClips = new Dictionary<string, AudioClip>(playerClips.Length);
		playerAudioSources = new Dictionary<string, AudioSource[]>(playerClips.Length);
		AudioClip[] array = playerClips;
		foreach (AudioClip audioClip in array)
		{
			indexedClips[audioClip.name] = audioClip;
			playerAudioSources[audioClip.name] = new AudioSource[num];
		}
		listedSources = new List<AudioSource>(playerClips.Length * num);
		commonAudioSources = new Dictionary<string, AudioSource>(commonClips.Length);
		array = commonClips;
		foreach (AudioClip audioClip2 in array)
		{
			AudioSource audioSource = CreateAudioSource();
			audioSource.clip = audioClip2;
			commonAudioSources.Add(audioClip2.name, audioSource);
		}
	}
	public void UpdateMethod()
	{
	}
	public void PlaySfx(string sfxName, int playerNo)
	{
		if (!SingletonMonoBehaviour<Shuriken_GameMain>.Instance.IsDuringGame)
		{
			return;
		}
		if (!indexedClips.TryGetValue(sfxName, out AudioClip value))
		{
			UnityEngine.Debug.LogError("指定されたSEがありません。\u3000ＳＥ名:" + sfxName);
			return;
		}
		AudioSource audioSource = playerAudioSources[sfxName][playerNo];
		if (audioSource == null)
		{
			audioSource = CreateAudioSource();
			audioSource.clip = value;
		}
		audioSource.volume = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
		audioSource.Play();
	}
	public void PlaySfx(string sfxName)
	{
		if (SingletonMonoBehaviour<Shuriken_GameMain>.Instance.IsDuringGame)
		{
			if (!commonAudioSources.TryGetValue(sfxName, out AudioSource value))
			{
				UnityEngine.Debug.LogError("指定されたSEがありません。\u3000ＳＥ名:" + sfxName);
				return;
			}
			value.volume = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
			value.PlayOneShot(value.clip);
		}
	}
	private AudioSource CreateAudioSource()
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.volume = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
		listedSources.Add(audioSource);
		return audioSource;
	}
}
