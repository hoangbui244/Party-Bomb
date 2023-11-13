using UnityEngine;
public class Golf_WindManager : SingletonCustom<Golf_WindManager>
{
	[Header("デバッグ：風の影響を受けないようにするかどうか")]
	public bool isDebugNoneWind;
	[Header("デバッグ：風速を設定するかどうか")]
	public bool isDebugWindSpeed;
	[Header("デバッグ：風速（最大10）")]
	public int debugWindSpeed;
	[Header("デバッグ：風向きを設定するかどうか")]
	public bool isDebugWindDir;
	[Header("デバッグ：風向き")]
	public float debugWindDir;
	[SerializeField]
	[Header("WindZone")]
	private WindZone windZone;
	[SerializeField]
	[Header("風矢印のマテリアル")]
	private Material[] arrayWindArrowMaterial;
	private int windSpeed;
	private int windSpeedIdx;
	private readonly int WIND_MAX_SPEED = 5;
	private int[] arrayWindSpeedData;
	private float windDir;
	private int windDirIdx;
	private readonly float WIND_DIR_ANGLE = 15f;
	private float[] arrayWindDirData;
	[SerializeField]
	[Header("空気抵抗係数（風速 x 係数）")]
	private float WIND_COEFFICIENT;
	public void Init()
	{
		isDebugNoneWind = false;
		isDebugWindSpeed = false;
		isDebugWindDir = false;
		arrayWindSpeedData = new int[WIND_MAX_SPEED];
		for (int i = 0; i < WIND_MAX_SPEED; i++)
		{
			arrayWindSpeedData[i] = i + 1;
		}
		int num = (int)(360f / WIND_DIR_ANGLE);
		arrayWindDirData = new float[num];
		for (int j = 0; j < num; j++)
		{
			arrayWindDirData[j] = (float)j * WIND_DIR_ANGLE;
		}
		InitPlay(_isInit: true);
	}
	public void InitPlay(bool _isInit = false)
	{
		if (_isInit)
		{
			windSpeedIdx = Random.Range(0, arrayWindSpeedData.Length);
			windSpeed = arrayWindSpeedData[windSpeedIdx];
			windDirIdx = Random.Range(0, arrayWindDirData.Length);
			windDir = arrayWindDirData[windDirIdx];
		}
		else
		{
			windSpeedIdx += Random.Range(-1, 2);
			windSpeedIdx = Mathf.Clamp(windSpeedIdx, 0, arrayWindSpeedData.Length - 1);
			windSpeed = arrayWindSpeedData[windSpeedIdx];
			windDirIdx += Random.Range(-1, 2);
			if (windDirIdx < 0)
			{
				windDirIdx = arrayWindDirData.Length - 1;
			}
			else if (windDirIdx > arrayWindDirData.Length - 1)
			{
				windDirIdx = 0;
			}
			windDir = arrayWindDirData[windDirIdx];
		}
		windZone.windMain = windSpeed;
		windZone.transform.rotation = Quaternion.Euler(GetWindVec());
	}
	public int GetWindSpeed()
	{
		return windSpeed;
	}
	public int GetMinWindSpeed()
	{
		return arrayWindSpeedData[0];
	}
	public int GetMaxWindSpeed()
	{
		return arrayWindSpeedData[arrayWindSpeedData.Length - 1];
	}
	public Material GetWindArrowMaterial()
	{
		int num = arrayWindSpeedData[arrayWindSpeedData.Length - 1];
		int num2 = (!((float)windSpeed / (float)num <= 0.79f)) ? 1 : 0;
		return arrayWindArrowMaterial[num2];
	}
	public float GetWindDir()
	{
		return windDir;
	}
	public Vector3 GetWindVec()
	{
		return CalcManager.PosRotation2D(Vector3.forward, CalcManager.mVector3Zero, windDir, CalcManager.AXIS.Y);
	}
	public float GetWindCoefficient()
	{
		return WIND_COEFFICIENT;
	}
	public Vector3 GetWindVelocity()
	{
		return GetWindVec() * ((float)windSpeed * WIND_COEFFICIENT);
	}
}
