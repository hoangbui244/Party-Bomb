using System;
using UnityEngine;
public class MonsterRace_Wave : MonoBehaviour
{
	[SerializeField]
	[Header("表示する波")]
	private GameObject[] waveObj;
	[SerializeField]
	[Header("波の開始位置")]
	private Vector3 waveStartPos;
	[SerializeField]
	[Header("波の終了位置")]
	private Vector3 waveEndPos;
	[SerializeField]
	[Header("何秒で沖まで行く？")]
	private float waveBeachMoveTime;
	[SerializeField]
	[Header("何秒で上がったり下がったりするか")]
	private float waveUpDownTime;
	[SerializeField]
	[Header("波はどれぐらいであがるか")]
	private Vector3 waveMaxScale;
	[SerializeField]
	[Header("波の動き方")]
	private AnimationCurve waveMoveAniCur;
	private int randomNo;
	private float waveMoveTime;
	private bool isUpDownFlg;
	private bool isMoveFlg;
	private float differenceTime;
	private float moveUpDownLerp;
	private float waveMoveTimeCount;
	[SerializeField]
	[Header("ギミックかどうか")]
	private bool isGimmick;
	[NonSerialized]
	public bool isFlg;
	private bool isRandomFlg;
	private bool isMove;
	public bool IsMove
	{
		get
		{
			return isMove;
		}
		set
		{
			isMove = value;
		}
	}
	private void Start()
	{
		differenceTime = waveBeachMoveTime - waveUpDownTime;
		for (int i = 0; i < waveObj.Length; i++)
		{
			waveObj[i].SetActive(value: false);
		}
	}
	private void Update()
	{
		if (isGimmick)
		{
			if (isFlg)
			{
				WaveMove();
			}
		}
		else if (isMove)
		{
			WaveMove();
		}
	}
	private void WaveMove()
	{
		if (!isRandomFlg)
		{
			randomNo = UnityEngine.Random.Range(0, waveObj.Length);
			waveObj[randomNo].SetActive(value: true);
			waveObj[randomNo].transform.transform.localPosition = waveStartPos;
			waveObj[randomNo].transform.transform.localScale = new Vector3(waveMaxScale.x, 0.001f, 1f);
			isRandomFlg = true;
		}
		if (isUpDownFlg)
		{
			moveUpDownLerp = Mathf.Clamp(moveUpDownLerp + Time.deltaTime / waveUpDownTime, 0f, 1f);
			if (moveUpDownLerp >= 1f)
			{
				isUpDownFlg = false;
			}
		}
		else
		{
			if (waveBeachMoveTime != 0f)
			{
				moveUpDownLerp = Mathf.Clamp(moveUpDownLerp - Time.deltaTime / differenceTime, 0f, 1f);
			}
			else
			{
				moveUpDownLerp = Mathf.Clamp(moveUpDownLerp - Time.deltaTime / waveUpDownTime, 0f, 1f);
			}
			if (moveUpDownLerp <= 0f)
			{
				isUpDownFlg = true;
			}
		}
		if (isMoveFlg)
		{
			waveMoveTimeCount = Mathf.Clamp(waveMoveTimeCount + Time.deltaTime / waveBeachMoveTime, 0f, 1f);
			if (waveMoveTimeCount >= 1f)
			{
				isMoveFlg = false;
			}
		}
		else
		{
			waveMoveTimeCount = 0f;
			moveUpDownLerp = 0f;
			if (isGimmick)
			{
				isFlg = false;
			}
			isRandomFlg = false;
			waveObj[randomNo].SetActive(value: false);
			isMoveFlg = true;
		}
		waveMoveTime = waveMoveAniCur.Evaluate(moveUpDownLerp);
		waveObj[randomNo].transform.localScale = Vector3.Lerp(new Vector3(waveMaxScale.x, 0.001f, 1f), waveMaxScale, waveMoveTime);
		waveObj[randomNo].transform.localPosition = Vector3.Lerp(waveStartPos, waveEndPos, waveMoveTimeCount);
	}
	private int WaveNo(int _randomNo)
	{
		return _randomNo = UnityEngine.Random.Range(0, waveObj.Length);
	}
}
