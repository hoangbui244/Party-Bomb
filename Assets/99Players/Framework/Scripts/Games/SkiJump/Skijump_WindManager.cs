using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Skijump_WindManager : SingletonCustom<Skijump_WindManager>
{
	public struct SpeedChangeData
	{
		public int baseSpeed;
		public float nowSpeed;
		public float changeSpeed;
		public float changeInterval;
	}
	[SerializeField]
	[Header("表示ル\u30fcト")]
	private GameObject objRoot;
	[SerializeField]
	[Header("風マテリアル")]
	private Material[] windMats;
	[SerializeField]
	[Header("風速マ\u30fcク")]
	private Transform[] windSpeedMark;
	[SerializeField]
	[Header("風")]
	private MeshRenderer wind;
	[SerializeField]
	[Header("風速テキスト")]
	private TextMeshPro windSpeedText;
	private Vector3 POSITION_OUT = new Vector3(815f, 235f, 150f);
	private Vector3 POSITION_RESULT = new Vector3(1200f, 235f, 150f);
	private Vector3 POSITION_IN = new Vector3(327f, 4.8f, 150f);
	private float[,] changeIntervalData = new float[2, 2]
	{
		{
			1f,
			2f
		},
		{
			0.25f,
			1f
		}
	};
	private float[] changeInterval = new float[2];
	private float windDir;
	private float windDirChangeSpeed;
	private bool isWindDirChange;
	private float[] dirListData = new float[8]
	{
		0f,
		45f,
		90f,
		135f,
		180f,
		225f,
		270f,
		315f
	};
	private List<float> dirList = new List<float>();
	private float windSpeedMin = 10f;
	private float windSpeedAdd = 10f;
	private int changeRandRange = 3;
	private float[] changeSpeedData = new float[2]
	{
		0.25f,
		0.5f
	};
	private float changeSpeed = 2f;
	private SpeedChangeData speedChangeData;
	public void Init(float _speedMag)
	{
		UnityEngine.Debug.Log("_speedMag = " + _speedMag.ToString());
		windDir = Random.Range(0f, 360f);
		windDirChangeSpeed = 1f;
		changeInterval[0] = Random.Range(changeIntervalData[0, 0], changeIntervalData[0, 1]);
		isWindDirChange = true;
		speedChangeData.baseSpeed = (int)(windSpeedMin + windSpeedAdd * _speedMag);
		windSpeedText.text = speedChangeData.nowSpeed.ToString() + " m/s";
		ResetDirList();
		LeanTween.cancel(base.gameObject);
		base.transform.localPosition = POSITION_IN;
		base.transform.SetLocalScale(0f, 0f, 1f);
	}
	public void Show()
	{
		base.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.5f).setEaseOutBack();
	}
	public void UpdateMethod()
	{
		if (!isWindDirChange)
		{
			return;
		}
		for (int i = 0; i < windSpeedMark.Length; i++)
		{
			windSpeedMark[i].transform.SetLocalEulerAnglesZ(Mathf.LerpAngle(windSpeedMark[i].transform.localEulerAngles.z, windDir, windDirChangeSpeed * Time.deltaTime));
		}
		changeInterval[0] -= Time.deltaTime;
		if (changeInterval[0] <= 0f)
		{
			changeInterval[0] = Random.Range(changeIntervalData[0, 0], changeIntervalData[0, 1]);
			int index = Random.Range(0, dirList.Count);
			windDir = dirList[index] + Random.Range(-10f, 10f);
			dirList.RemoveAt(index);
			if (dirList.Count <= 0)
			{
				ResetDirList();
			}
		}
		changeInterval[1] -= Time.deltaTime;
		if (changeInterval[1] <= 0f)
		{
			UpdateSpeedData();
		}
		speedChangeData.nowSpeed = Mathf.Lerp(speedChangeData.nowSpeed, speedChangeData.changeSpeed, changeSpeed * Time.deltaTime);
		windSpeedText.text = ((int)speedChangeData.nowSpeed).ToString() + " m/s";
		if (CalcManager.CheckRange(windSpeedMark[0].transform.localEulerAngles.z, 90f, 270f))
		{
			wind.material = windMats[1];
		}
		else
		{
			wind.material = windMats[0];
		}
	}
	private void UpdateSpeedData()
	{
		changeInterval[1] = Random.Range(changeIntervalData[1, 0], changeIntervalData[1, 1]);
		speedChangeData.changeSpeed = speedChangeData.baseSpeed + Random.Range(-changeRandRange, changeRandRange);
		changeSpeed = Random.Range(changeSpeedData[0], changeSpeedData[1]);
	}
	public void ResetWindData()
	{
		ResetDirList();
		for (int i = 0; i < windSpeedMark.Length; i++)
		{
			windSpeedMark[i].transform.SetLocalEulerAnglesZ(Random.Range(0f, 360f));
		}
		LeanTween.cancel(base.gameObject);
		base.transform.SetLocalScale(0f, 0f, 1f);
		base.transform.localPosition = POSITION_IN;
	}
	private void ResetDirList()
	{
		dirList.Clear();
		for (int i = 0; i < dirListData.Length; i++)
		{
			dirList.Add(dirListData[i]);
		}
	}
	public void SetChangeWindDir(bool _flg)
	{
		isWindDirChange = _flg;
		if (!isWindDirChange)
		{
			LeanTween.moveLocal(base.gameObject, POSITION_OUT, 1f).setEaseOutQuart().setDelay(0.25f);
		}
	}
	public void SetSkipWind()
	{
		isWindDirChange = false;
		LeanTween.cancel(base.gameObject);
		base.transform.localPosition = POSITION_OUT;
		speedChangeData.nowSpeed = Random.Range(5f, 12f);
		for (int i = 0; i < windSpeedMark.Length; i++)
		{
			windSpeedMark[i].transform.SetLocalEulerAnglesZ(Random.Range(90f, 270f));
		}
		if (CalcManager.CheckRange(windSpeedMark[0].transform.localEulerAngles.z, 90f, 270f))
		{
			wind.material = windMats[1];
		}
		else
		{
			wind.material = windMats[0];
		}
	}
	public void ToResult()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.moveLocal(base.gameObject, POSITION_RESULT, 0.25f).setEaseOutQuart();
	}
	public float GetWindCorr()
	{
		return (0f - CalcManager.PosRotation2D(Vector3.forward, CalcManager.mVector3Zero, windSpeedMark[0].transform.localEulerAngles.z, CalcManager.AXIS.Y).z) * (speedChangeData.nowSpeed / windSpeedMin);
	}
	public float GetWindValue()
	{
		return 0f - CalcManager.PosRotation2D(Vector3.forward, CalcManager.mVector3Zero, windSpeedMark[0].transform.localEulerAngles.z, CalcManager.AXIS.Y).z;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
