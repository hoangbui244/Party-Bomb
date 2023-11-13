using UnityEngine;
public class SnowBoard_ParticleController : MonoBehaviour
{
	[SerializeField]
	[Header("管理するParticleSystem")]
	private ParticleSystem[] effects;
	[SerializeField]
	[Header("ゲ\u30fcム開始時から再生するか")]
	private bool isStartPlay;
	private bool isPlay;
	private void Start()
	{
		for (int i = 0; i < effects.Length; i++)
		{
			effects[i].Stop();
			if (isStartPlay)
			{
				effects[i].Play();
			}
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!isPlay && other.gameObject.tag == "Player")
		{
			isPlay = true;
			for (int i = 0; i < effects.Length; i++)
			{
				effects[i].Play();
			}
		}
	}
}
