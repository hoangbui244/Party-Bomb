using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Surfing_OceanCollider : MonoBehaviour
{
	[SerializeField]
	[Header("更新するMeshCollider")]
	private MeshCollider col;
	[SerializeField]
	[Header("更新するMeshFilter")]
	private MeshFilter meshFilter;
	[SerializeField]
	[Header("生成する海面の一辺長さ[m]")]
	private int scale = 100;
	[SerializeField]
	[Header("メッシュの分割数(X方向)")]
	private int vertexXNum = 10;
	[SerializeField]
	[Header("メッシュの分割数(Z方向)")]
	private int vertexZNum = 10;
	[SerializeField]
	private float _WaveSpeed = 1f;
	[SerializeField]
	private Vector4 _Amplitude = new Vector4(0.78f, 0.81f, 0.6f, 0.27f);
	[SerializeField]
	private Vector4 _Frequency = new Vector4(0.16f, 0.18f, 0.21f, 0.27f);
	[SerializeField]
	private Vector4 _Steepness = new Vector4(1.7f, 1.6f, 1.2f, 1.8f);
	[SerializeField]
	private Vector4 _Speed = new Vector4(24f, 40f, 48f, 60f);
	[SerializeField]
	private Vector4 _Noise = new Vector4(0.39f, 0.31f, 0.27f, 0.57f);
	[SerializeField]
	private Vector4 _DirectionA = new Vector4(0.35f, 0.31f, 0.08f, 0.6f);
	[SerializeField]
	private Vector4 _DirectionB = new Vector4(-0.95f, -0.74f, 0.7f, -0.5f);
	[SerializeField]
	private Vector4 _Amplitude2 = new Vector4(0.17f, 0.12f, 0.21f, 0.06f);
	[SerializeField]
	private Vector4 _Frequency2 = new Vector4(0.7f, 0.84f, 0.54f, 0.8f);
	[SerializeField]
	private Vector4 _Steepness2 = new Vector4(1.56f, 2.18f, 2.8f, 1.9f);
	[SerializeField]
	private Vector4 _Speed2 = new Vector4(32f, 40f, 48f, 60f);
	[SerializeField]
	private Vector4 _Noise2 = new Vector4(0.33f, 0.81f, 0.39f, 0.45f);
	[SerializeField]
	private Vector4 _DirectionC = new Vector4(0.7f, 0.6f, 0.1f, 0.38f);
	[SerializeField]
	private Vector4 _DirectionD = new Vector4(0.43f, 0.07f, 0.42f, 0.61f);
	[SerializeField]
	[Range(0f, 0.5f)]
	private float _NoiseSizeLerp = 0.5f;
	[SerializeField]
	[Range(0f, 5f)]
	private float _NoiseStrength = 1.26f;
	private float[] amp = new float[8];
	private float[] freq = new float[8];
	private float[] steep = new float[8];
	private float[] speed = new float[8];
	private Vector2[] dir = new Vector2[8];
	private float[] noise_size = new float[8];
	private float[] foam_speed = new float[8];
	private Vector4 _LightColor0;
	private List<Vector3> vertexList = new List<Vector3>();
	private List<Vector3> vertexOriginList = new List<Vector3>();
	private List<int> triangleList = new List<int>();
	private List<Vector2> uvList = new List<Vector2>();
	private Mesh mesh;
	private int wave_number = 4;
	private int count = 2;
	private float time;
	private Vector3 p;
	private bool isFixedUpdate;
	private void Start()
	{
		SetUpWave();
		isFixedUpdate = true;
		mesh = meshFilter.mesh;
		for (int i = 0; i < vertexZNum; i++)
		{
			for (int j = 0; j < vertexXNum; j++)
			{
				vertexList.Add(new Vector3((float)j * (float)scale / (float)vertexXNum, 0f, (float)i * (float)scale / (float)vertexZNum));
				uvList.Add(new Vector2((float)j * 1f / (float)vertexXNum, (float)i * 1f / (float)vertexZNum));
			}
		}
		vertexOriginList = new List<Vector3>(vertexList);
		for (int k = 0; k < vertexZNum - 1; k++)
		{
			for (int l = 0; l < vertexXNum - 1; l++)
			{
				triangleList.Add(k * vertexXNum + l);
				triangleList.Add((k + 1) * vertexXNum + l);
				triangleList.Add((k + 1) * vertexXNum + l + 1);
				triangleList.Add(k * vertexXNum + l);
				triangleList.Add((k + 1) * vertexXNum + l + 1);
				triangleList.Add(k * vertexXNum + l + 1);
			}
		}
		mesh.SetVertices(vertexList);
		mesh.SetTriangles(triangleList, 0);
		mesh.RecalculateNormals();
		mesh.SetUVs(0, uvList);
		meshFilter.mesh = mesh;
		col.sharedMesh = null;
		col.sharedMesh = mesh;
	}
	private void FixedUpdate()
	{
		if (!isFixedUpdate)
		{
			return;
		}
		StartCoroutine(WaitOneFrame());
		time += Time.deltaTime * _WaveSpeed;
		for (int i = 0; i < vertexList.Count; i++)
		{
			p = vertexOriginList[i];
			for (int j = 0; j < count; j++)
			{
				p += GerstnerWave(new Vector2(amp[j], amp[j]), freq[j], steep[j], speed[j], noise_size[j], dir[j], new Vector2(vertexOriginList[i].x, vertexOriginList[i].z), time, j);
			}
			for (int k = wave_number - count; k < wave_number; k++)
			{
				p += GerstnerWave_Cross(new Vector2(amp[k], amp[k]), freq[k], steep[k], speed[k], noise_size[k], dir[k], new Vector2(vertexOriginList[i].x, vertexOriginList[i].z), time, k);
			}
			vertexList[i] = p;
		}
		mesh.SetVertices(vertexList);
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
		col.sharedMesh = mesh;
	}
	private void OnDistroy()
	{
		mesh.Clear();
	}
	public float Frac(float value)
	{
		return value - (float)Mathf.FloorToInt(value);
	}
	private void SetUpWave()
	{
		amp = new float[8]
		{
			_Amplitude.x,
			_Amplitude.y,
			_Amplitude.z,
			_Amplitude.w,
			_Amplitude2.x,
			_Amplitude2.y,
			_Amplitude2.z,
			_Amplitude2.w
		};
		freq = new float[8]
		{
			_Frequency.x,
			_Frequency.y,
			_Frequency.z,
			_Frequency.w,
			_Frequency2.x,
			_Frequency2.y,
			_Frequency2.z,
			_Frequency2.w
		};
		steep = new float[8]
		{
			_Steepness.x,
			_Steepness.y,
			_Steepness.z,
			_Steepness.w,
			_Steepness2.x,
			_Steepness2.y,
			_Steepness2.z,
			_Steepness2.w
		};
		speed = new float[8]
		{
			_Speed.x,
			_Speed.y,
			_Speed.z,
			_Speed.w,
			_Speed2.x,
			_Speed2.y,
			_Speed2.z,
			_Speed2.w
		};
		dir = new Vector2[8]
		{
			new Vector2(_DirectionA.x, _DirectionA.y),
			new Vector2(_DirectionA.z, _DirectionA.w),
			new Vector2(_DirectionB.x, _DirectionB.y),
			new Vector2(_DirectionB.z, _DirectionB.w),
			new Vector2(_DirectionC.x, _DirectionC.y),
			new Vector2(_DirectionC.z, _DirectionC.w),
			new Vector2(_DirectionD.x, _DirectionD.y),
			new Vector2(_DirectionD.z, _DirectionD.w)
		};
		noise_size = new float[8]
		{
			_Noise.x,
			_Noise.y,
			_Noise.z,
			_Noise.w,
			_Noise2.x,
			_Noise2.y,
			_Noise2.z,
			_Noise2.w
		};
	}
	private Vector3 GerstnerWave(Vector2 amp, float freq, float steep, float speed, float noise, Vector2 dir, Vector2 v, float time, int seed)
	{
		Vector2 normalized = dir.normalized;
		seed *= 3;
		float f = Vector2.Dot(normalized, v) * freq + time * speed;
		Vector3 result = default(Vector3);
		result.x = steep * amp.x * normalized.x * Mathf.Cos(f);
		result.z = steep * amp.y * normalized.y * Mathf.Cos(f);
		result.y = amp.x * Mathf.Sin(f);
		return result;
	}
	private Vector3 GerstnerWave_Cross(Vector2 amp, float freq, float steep, float speed, float noise, Vector2 dir, Vector2 v, float time, int seed)
	{
		Vector2 normalized = dir.normalized;
		float noiseStrength = _NoiseStrength;
		seed *= 3;
		Vector2 normalized2 = dir.normalized;
		Vector2 vector = new Vector2(0f - normalized.y, normalized.x);
		Vector2 vector2 = Vector2.Dot(normalized2, v) * new Vector2(freq, freq) + new Vector2(time * speed, time * speed);
		Vector2 vector3 = Vector2.Dot(vector, v) * new Vector2(freq, freq) + new Vector2(time * speed, time * speed);
		Vector3 vector4 = default(Vector3);
		vector4.x = steep * amp.x * normalized2.x * Mathf.Cos(vector2.x);
		vector4.z = steep * amp.y * normalized2.y * Mathf.Cos(vector2.y);
		vector4.y = amp.x * Mathf.Sin(vector2.x);
		Vector3 vector5 = default(Vector3);
		vector5.x = steep * amp.x * vector.x * Mathf.Cos(vector3.x);
		vector5.z = steep * amp.y * vector.y * Mathf.Cos(vector3.y);
		vector5.y = amp.x * Mathf.Sin(vector3.x);
		Vector3 result = default(Vector3);
		result.x = Mathf.Lerp(vector4.x, vector5.x, 0.5f);
		result.y = Mathf.Lerp(vector4.y, vector5.y, 0.5f);
		result.z = Mathf.Lerp(vector4.z, vector5.z, 0.5f);
		return result;
	}
	private Vector2 rand2d(Vector2 st, int seed)
	{
		Vector2 result = new Vector2(0f, 0f);
		result[0] = Vector2.Dot(st, new Vector2(127.1f, 311.7f)) + (float)seed;
		result[1] = Vector2.Dot(st, new Vector2(269.5f, 183.3f)) + (float)seed;
		result[0] = -1f + 2f * Frac(Mathf.Sin(result[0]) * 43758.5469f);
		result[1] = -1f + 2f * Frac(Mathf.Sin(result[1]) * 43758.5469f);
		return result;
	}
	private float noise2(Vector2 st, int seed)
	{
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		vector[0] = Mathf.FloorToInt(st[0]);
		vector[1] = Mathf.FloorToInt(st[1]);
		vector2[0] = Frac(st[0]);
		vector2[1] = Frac(st[1]);
		float a = Vector2.Dot(rand2d(vector, seed), vector2);
		float b = Vector2.Dot(rand2d(vector + new Vector2(1f, 0f), seed), vector2 - new Vector2(1f, 0f));
		float a2 = Vector2.Dot(rand2d(vector + new Vector2(0f, 1f), seed), vector2 - new Vector2(0f, 1f));
		float b2 = Vector2.Dot(rand2d(vector + new Vector2(1f, 1f), seed), vector2 - new Vector2(1f, 1f));
		Vector2 vector3 = new Vector2(0f, 0f);
		vector3 = Vector3.Scale(Vector3.Scale(vector2, vector2), new Vector2(3f, 3f) - 2f * vector2);
		return Mathf.Lerp(Mathf.Lerp(a, b, vector3.x), Mathf.Lerp(a2, b2, vector3.x), vector3.y);
	}
	private Vector3 rand3d(Vector3 p, int seed)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		result[0] = Vector3.Dot(p, new Vector3(127.1f, 311.7f, 74.7f)) + (float)seed;
		result[1] = Vector3.Dot(p, new Vector3(269.5f, 183.3f, 246.1f)) + (float)seed;
		result[2] = Vector3.Dot(p, new Vector3(113.5f, 271.9f, 124.6f)) + (float)seed;
		result[0] = -1f + 2f * Frac(Mathf.Sin(result[0]) * 43758.5469f);
		result[1] = -1f + 2f * Frac(Mathf.Sin(result[1]) * 43758.5469f);
		result[2] = -1f + 2f * Frac(Mathf.Sin(result[2]) * 43758.5469f);
		return result;
	}
	private float noise3(Vector3 st, int seed)
	{
		Vector3 a = new Vector3(0f, 0f, 0f);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		a[0] = Mathf.FloorToInt(st[0]);
		a[1] = Mathf.FloorToInt(st[1]);
		a[1] = Mathf.FloorToInt(st[2]);
		vector[0] = Frac(st[0]);
		vector[1] = Frac(st[1]);
		vector[1] = Frac(st[2]);
		float a2 = Vector3.Dot(rand3d(a, seed), vector);
		float b = Vector3.Dot(rand3d(a + new Vector3(1f, 0f, 0f), seed), vector - new Vector3(1f, 0f, 0f));
		float a3 = Vector3.Dot(rand3d(a + new Vector3(0f, 1f, 0f), seed), vector - new Vector3(0f, 1f, 0f));
		float b2 = Vector3.Dot(rand3d(a + new Vector3(1f, 1f, 0f), seed), vector - new Vector3(1f, 1f, 0f));
		float a4 = Vector3.Dot(rand3d(a + new Vector3(0f, 0f, 1f), seed), vector - new Vector3(0f, 0f, 1f));
		float b3 = Vector3.Dot(rand3d(a + new Vector3(1f, 0f, 1f), seed), vector - new Vector3(1f, 0f, 1f));
		float a5 = Vector3.Dot(rand3d(a + new Vector3(0f, 1f, 1f), seed), vector - new Vector3(0f, 1f, 1f));
		float b4 = Vector3.Dot(rand3d(a + new Vector3(1f, 1f, 1f), seed), vector - new Vector3(1f, 1f, 1f));
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		vector2 = Vector3.Scale(Vector3.Scale(vector, vector), new Vector3(3f, 3f, 3f) - 2f * vector);
		float a6 = Mathf.Lerp(Mathf.Lerp(a2, b, vector2.x), Mathf.Lerp(a3, b2, vector2.x), vector2.y);
		float b5 = Mathf.Lerp(Mathf.Lerp(a4, b3, vector2.x), Mathf.Lerp(a5, b4, vector2.x), vector2.y);
		return Mathf.Lerp(a6, b5, vector2.z);
	}
	private float fbm(Vector2 st, int seed)
	{
		float num = 0f;
		float num2 = 0.5f;
		for (int i = 0; i < 6; i++)
		{
			num += num2 * noise2(st, seed);
			st *= 2f;
			num2 *= 0.5f;
		}
		return num;
	}
	private IEnumerator WaitOneFrame()
	{
		isFixedUpdate = false;
		yield return null;
		isFixedUpdate = true;
	}
}
