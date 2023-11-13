using UnityEngine;
public class Surfing_WaveManager : MonoBehaviour
{
	[SerializeField]
	[Header("統括するSurfing_Waveスクリプト")]
	private Surfing_Wave[] waves;
	public void Init()
	{
		for (int i = 0; i < waves.Length; i++)
		{
			waves[i].Init();
		}
	}
}
