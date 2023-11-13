using System;
using UnityEngine;
public class SmartBall_BallObject : MonoBehaviour
{
	[SerializeField]
	[Header("ボ\u30fcルのマテリアル")]
	private Material[] ballMats;
	[SerializeField]
	[Header("ボ\u30fcルのメッシュレンダラ\u30fc")]
	private MeshRenderer ballMeshRenderer;
	private int dataNo;
	private readonly float STOP_SPEED = 0.01f;
	private bool isStopped;
	private Rigidbody rigid;
	private int hasPoint;
	private readonly Vector3 START_BALL_LOCAL_POS = new Vector3(0f, 0f, -0.25f);
	private Vector3 gravityVec;
	private readonly float GRAVITY_FORCE_DOWN = -12f;
	private readonly float GRAVITY_FORCE_DROP = -12f;
	private bool shot;
	private bool holeIn;
	private bool addScore;
	private bool playerUseBall;
	public bool Shot => shot;
	public bool HoleIn => holeIn;
	public bool AddScore => addScore;
	public bool PlayerUseBall => playerUseBall;
	public void Init(int _dataNo, int _standNo, bool _isPlayer)
	{
		dataNo = _dataNo;
		playerUseBall = _isPlayer;
		isStopped = false;
		ballMeshRenderer.material = ballMats[_standNo];
		rigid = GetComponent<Rigidbody>();
		rigid.isKinematic = true;
		base.transform.localPosition = START_BALL_LOCAL_POS;
		gravityVec = new Vector3(0f, GRAVITY_FORCE_DOWN, GRAVITY_FORCE_DROP);
	}
	public void StockBallInit()
	{
		rigid = GetComponent<Rigidbody>();
		gravityVec = new Vector3(0f, GRAVITY_FORCE_DOWN, GRAVITY_FORCE_DROP);
	}
	private void FixedUpdate()
	{
		if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause)
		{
			rigid.AddForce(gravityVec, ForceMode.Force);
		}
	}
	public void BallAddFourse(float _power)
	{
		rigid.isKinematic = false;
		rigid.AddForce(Vector3.forward * _power, ForceMode.Impulse);
	}
	public int GetBallHasPoint()
	{
		addScore = true;
		return hasPoint;
	}
	public void BallHoleIn()
	{
		if (holeIn)
		{
			LeanTween.delayedCall(0.5f, (Action)delegate
			{
				rigid.isKinematic = true;
				base.gameObject.SetActive(value: false);
			});
		}
	}
	public void HoleInEffect(MeshRenderer getHoleRenderer)
	{
		Color color = getHoleRenderer.material.color;
		LeanTween.value(getHoleRenderer.gameObject, color.r, 1f, 0.3f).setLoopPingPong(2).setOnUpdate(delegate(float _value)
		{
			getHoleRenderer.material.SetColor("_Color", new Color(_value, _value, _value));
		});
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent.parent.GetComponent<SmartBall_StandObject>() != null)
		{
			int pointHoleDataNo = other.transform.parent.parent.GetComponent<SmartBall_StandObject>().GetPointHoleDataNo(other);
			hasPoint = other.transform.parent.parent.GetComponent<SmartBall_StandObject>().pointHoleDatas[pointHoleDataNo].holePoint;
			holeIn = true;
		}
		if (other.gameObject.name == "ShotJugeCollider")
		{
			shot = true;
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "StickShotParts")
		{
			rigid.isKinematic = true;
			base.transform.parent = collision.transform;
			base.transform.localPosition = START_BALL_LOCAL_POS;
		}
		if (collision.gameObject.tag == "Object" && playerUseBall)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_smartball_bound");
		}
	}
}
