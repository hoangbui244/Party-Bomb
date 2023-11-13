using UnityEngine;
public class Biathlon_CharacterAudioConfig : ScriptableObject
{
	[SerializeField]
	private AudioClip glidingClip;
	[SerializeField]
	private AudioClip glidingNoiseClip;
	[SerializeField]
	private AudioClip windClip;
	[SerializeField]
	private AudioClip shotClip;
	[SerializeField]
	private AudioClip hitClip;
	[SerializeField]
	private AudioClip missClip;
	[SerializeField]
	private float glidingVolume = 1f;
	[SerializeField]
	private float glidingNoiseVolume = 1f;
	[SerializeField]
	private float windVolume = 1f;
	[SerializeField]
	private float shotVolume = 1f;
	[SerializeField]
	private float hitVolume = 1f;
	[SerializeField]
	private float missVolume = 1f;
	public AudioClip GlidingClip => glidingClip;
	public AudioClip GlidingNoiseClip => glidingNoiseClip;
	public AudioClip WindClip => windClip;
	public AudioClip ShotClip => shotClip;
	public AudioClip HitClip => hitClip;
	public AudioClip MissClip => missClip;
	public float GlidingVolume => glidingVolume;
	public float GlidingNoiseVolume => glidingNoiseVolume;
	public float WindVolume => windVolume;
	public float ShotVolume => shotVolume;
	public float HitVolume => hitVolume;
	public float MissVolume => missVolume;
}
