using System;
using System.Collections;
using UnityEngine;
public class Surfing_Wave : MonoBehaviour
{
	[SerializeField]
	[Header("波全体のオブジェクト")]
	private GameObject allWaveObj;
	[SerializeField]
	[Header("高波の頂上判定トリガ\u30fcオブジェクト")]
	private Collider waveTriObj;
	[SerializeField]
	[Header("チュ\u30fcブ波のオブジェクト")]
	private GameObject tubeWaveObj;
	[SerializeField]
	[Header("チュ\u30fcブ波の判定トリガ\u30fcオブジェクト")]
	private Collider tubeWaveTriObj;
	[SerializeField]
	[Header("波の開始位置")]
	private Transform startPoint;
	[SerializeField]
	[Header("波が巻き始める位置")]
	private Transform tubePoint;
	[SerializeField]
	[Header("波の進行速度(Z軸)")]
	private float waveSpeed = 1f;
	[SerializeField]
	[Header("チュ\u30fcブライディング用の波を出すか")]
	private bool isTubeWave;
	[SerializeField]
	[Header("高波の水しぶきエフェクト")]
	private ParticleSystem psWave;
	[SerializeField]
	[Header("チュ\u30fcブ波の水しぶきエフェクト")]
	private ParticleSystem psTubeWave;
	private bool isTubeStart;
	private Rigidbody rb;
	private Vector3 originPos;
	private void Start()
	{
		originPos = allWaveObj.transform.localPosition;
		Init();
		rb = allWaveObj.GetComponent<Rigidbody>();
	}
	private void FixedUpdate()
	{
		rb.MovePosition(allWaveObj.transform.position + allWaveObj.transform.forward * Time.deltaTime);
		if (!isTubeStart && allWaveObj.transform.localPosition.z > -50f)
		{
			isTubeStart = true;
			StartCoroutine(TubeAnime());
		}
	}
	public void Init()
	{
		LeanTween.cancel(allWaveObj);
		LeanTween.cancel(tubeWaveObj);
		allWaveObj.transform.localPosition = originPos;
		tubeWaveObj.transform.SetLocalPosition(0f, -0.1f, 0f);
		tubeWaveObj.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
		waveTriObj.enabled = true;
		tubeWaveTriObj.enabled = false;
		isTubeStart = false;
		psWave.Play();
		psTubeWave.Stop();
	}
	public void Resporn()
	{
		LeanTween.cancel(allWaveObj);
		LeanTween.cancel(tubeWaveObj);
		allWaveObj.transform.localPosition = new Vector3(0f, -5f * allWaveObj.transform.localScale.y, startPoint.localPosition.z);
		tubeWaveObj.transform.SetLocalPosition(0f, -0.1f, 0f);
		tubeWaveObj.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
		waveTriObj.enabled = true;
		tubeWaveTriObj.enabled = false;
		isTubeStart = false;
		psWave.Play();
		psTubeWave.Stop();
		LeanTween.moveLocalY(allWaveObj, 0f, 10f);
	}
	private IEnumerator TubeAnime()
	{
		if (isTubeWave)
		{
			LeanTween.moveLocalY(tubeWaveObj, 2.253f, 10f);
			LeanTween.moveLocalZ(tubeWaveObj, 2.15f, 8f);
			psWave.Stop();
			psTubeWave.Play();
		}
		yield return new WaitForSeconds(10f);
		if (isTubeWave)
		{
			waveTriObj.enabled = false;
			tubeWaveTriObj.enabled = true;
			LeanTween.rotateAroundLocal(tubeWaveObj, -tubeWaveObj.transform.right, 90f, 5f);
		}
		LeanTween.moveLocalY(allWaveObj, -5f * allWaveObj.transform.localScale.y, 10f).setOnComplete((Action)delegate
		{
			Resporn();
		});
	}
}
