using System;
using System.Collections;
using UnityEngine;
public class MonsterRace_WaveManager : MonoBehaviour
{
	private static readonly float INTERVAL_TIME = 2f;
	[SerializeField]
	private MonsterRace_Wave[] waves;
	private float startWavesTime;
	private void Start()
	{
		Init();
	}
	public void Init()
	{
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[0].IsMove = true;
		}));
		startWavesTime += INTERVAL_TIME;
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[1].IsMove = true;
		}));
		startWavesTime += INTERVAL_TIME;
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[2].IsMove = true;
		}));
		startWavesTime += INTERVAL_TIME;
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[3].IsMove = true;
		}));
		startWavesTime += INTERVAL_TIME;
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[4].IsMove = true;
		}));
		startWavesTime += INTERVAL_TIME;
		StartCoroutine(TimeMove(startWavesTime, delegate
		{
			waves[5].IsMove = true;
		}));
	}
	private IEnumerator TimeMove(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
