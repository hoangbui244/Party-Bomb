using System;
using System.Collections;
using UnityEngine;
public class MonsterRace_WaveCheck : MonoBehaviour
{
	public enum WaveType
	{
		NORMAL,
		THREE_CONTINUITY
	}
	private static readonly float WAVE_TIME_INTERVAL = 1f;
	[SerializeField]
	[Header("波の種類")]
	private WaveType waveType;
	[SerializeField]
	[Header("起動する波")]
	private MonsterRace_Wave[] wave;
	private float waveTime;
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && !wave[0].isFlg)
		{
			switch (waveType)
			{
			case WaveType.NORMAL:
				wave[0].isFlg = true;
				break;
			case WaveType.THREE_CONTINUITY:
				ThreeContinuity(0);
				waveTime += WAVE_TIME_INTERVAL;
				ThreeContinuity(1);
				waveTime += WAVE_TIME_INTERVAL;
				ThreeContinuity(2);
				waveTime = 0f;
				break;
			}
		}
	}
	private IEnumerator TimeMove(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
	private void ThreeContinuity(int _no)
	{
		StartCoroutine(TimeMove(waveTime, delegate
		{
			wave[_no].isFlg = true;
		}));
	}
}
