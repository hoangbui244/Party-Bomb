using System.Collections.Generic;
using UnityEngine;
public class Golf_PredictionBallLine : MonoBehaviour
{
	[SerializeField]
	[Header("予測線のボ\u30fcル")]
	private GameObject dummyBallPref;
	[SerializeField]
	[Header("予測線のボ\u30fcルの数")]
	private int dummyCount;
	[SerializeField]
	[Header("予測線のボ\u30fcルを表示する間隔の秒数")]
	private float dummySecInterval;
	[SerializeField]
	[Header("予測線のボ\u30fcルが移動する速度")]
	private float dummyOffsetSpeed = 0.5f;
	private float offset;
	private List<GameObject> dummyBallList = new List<GameObject>();
	private List<MeshRenderer> dummyBallMeshList = new List<MeshRenderer>();
	public void Init()
	{
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		base.transform.position = readyBallPos;
		for (int i = 0; i < dummyCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(dummyBallPref, base.transform);
			dummyBallList.Add(gameObject);
			dummyBallMeshList.Add(gameObject.GetComponent<MeshRenderer>());
		}
	}
	public void InitPlay()
	{
		for (int i = 0; i < dummyCount; i++)
		{
			dummyBallList[i].SetActive(value: true);
		}
	}
	public void UpdateMethod()
	{
		Golf_Player turnPlayer = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
		Golf_Ball turnPlayerBall = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		Vector3 a = Quaternion.Euler(new Vector3(SingletonCustom<Golf_PlayerManager>.Instance.GetShotAngle(), 0f, 0f)) * -turnPlayer.transform.right;
		float d = SingletonCustom<Golf_PlayerManager>.Instance.GetBaseShotPower() * (1f / turnPlayerBall.GetRigidbodyMass());
		Vector3 vector = a * d;
		offset = Mathf.Repeat(Time.time * dummyOffsetSpeed, dummySecInterval);
		for (int i = 0; i < dummyCount; i++)
		{
			float num = (float)i * dummySecInterval + offset;
			float x = num * vector.x;
			float z = num * vector.z;
			float y = vector.y * num - 0.5f * (0f - Physics.gravity.y) * Mathf.Pow(num, 2f);
			dummyBallList[i].transform.localPosition = new Vector3(x, y, z);
			dummyBallMeshList[i].material.SetAlpha(Mathf.Lerp(0.8f, 0.2f, (float)i / (float)dummyCount));
		}
	}
	public void Hide()
	{
		for (int i = 0; i < dummyCount; i++)
		{
			dummyBallList[i].SetActive(value: false);
		}
	}
}
