using System;
using System.Collections;
using UnityEngine;
public class SmartBall_StandObject : MonoBehaviour
{
	[Serializable]
	public struct PointHoleData
	{
		[Header("ポイント")]
		public int holePoint;
		[Header("穴の位置")]
		public Transform holeTrans;
		[Header("当たり判定")]
		public Collider holeCollider;
		[Header("エフェクト用メッシュレンダラ\u30fc")]
		public MeshRenderer holeEffectMesh;
	}
	[SerializeField]
	[Header("ボ\u30fcル生成位置")]
	private Transform ballGenerateAnchor;
	[SerializeField]
	[Header("ボ\u30fcル発射開始位置")]
	private Transform ballShotAnchor;
	[Header("ポイントの穴デ\u30fcタ")]
	public PointHoleData[] pointHoleDatas;
	[SerializeField]
	[Header("玉を打つためのスティックパ\u30fcツ")]
	private SmartBall_ShotStickObj shotStick;
	[SerializeField]
	[Header("ボ\u30fcルストッパ\u30fcオブジェクト")]
	private GameObject ballStopperObj;
	[SerializeField]
	[Header("ギミックオブジェクト")]
	private SmartBall_Gimmick[] gimmicks;
	private bool stopperOpen;
	private readonly Vector3 STOPPER_CLOSE_ROT = new Vector3(0f, 65f, 0f);
	private readonly Vector3 STOPPER_OPEN_ROT = new Vector3(0f, -30f, 0f);
	private readonly float STOPPER_MOVE_TIME = 0.3f;
	public Transform BallGenerateAnchor => ballGenerateAnchor;
	public Transform BallShotAnchor => ballShotAnchor;
	public SmartBall_Gimmick[] Gimmicks => gimmicks;
	public bool StopperOpen => stopperOpen;
	public void Init(bool _isPlayer)
	{
		shotStick.CheckPlayerStand(_isPlayer);
		for (int i = 0; i < gimmicks.Length; i++)
		{
			gimmicks[i].CheckPlayerStand(_isPlayer);
		}
	}
	public void UpdateMethod()
	{
		if (gimmicks.Length != 0)
		{
			for (int i = 0; i < gimmicks.Length; i++)
			{
				gimmicks[i].UpdateMethod();
			}
		}
	}
	public void OpenCapGimmick(int _num)
	{
		gimmicks[_num].OpenCapGimmick();
	}
	public void CloseCapGimmick(int _num)
	{
		gimmicks[_num].CloseCapGimmick();
	}
	public void PullShotStick()
	{
		shotStick.PullShotStick();
	}
	public void BallShotStick()
	{
		shotStick.BallShotStick();
	}
	public void StoppeClose()
	{
		LeanTween.delayedCall(0.1f, (Action)delegate
		{
			LeanTween.rotateLocal(ballStopperObj, STOPPER_CLOSE_ROT, STOPPER_MOVE_TIME).setOnComplete((Action)delegate
			{
				stopperOpen = false;
			});
		});
	}
	public void StoppeOpen()
	{
		LeanTween.delayedCall(0.3f, (Action)delegate
		{
			LeanTween.rotateLocal(ballStopperObj, STOPPER_OPEN_ROT, STOPPER_MOVE_TIME).setOnComplete((Action)delegate
			{
				stopperOpen = true;
			});
		});
	}
	public IEnumerator _StopperMove()
	{
		LeanTween.rotateLocal(ballStopperObj, STOPPER_OPEN_ROT, STOPPER_MOVE_TIME).setOnComplete((Action)delegate
		{
			stopperOpen = true;
		});
		yield return new WaitForSeconds(STOPPER_MOVE_TIME * 2f);
		LeanTween.rotateLocal(ballStopperObj, STOPPER_CLOSE_ROT, STOPPER_MOVE_TIME).setOnComplete((Action)delegate
		{
			stopperOpen = false;
		});
	}
	public bool CheckSetBall()
	{
		return shotStick.CheckSetBall();
	}
	public bool CheckMoveGimmick(int _num)
	{
		return gimmicks[_num].IsMove;
	}
	public float GetStickShotTime()
	{
		return shotStick.StickShotTime;
	}
	public SmartBall_ShotStickObj GetStickObj()
	{
		return shotStick;
	}
	public SmartBall_BallObject GetShotBall()
	{
		return shotStick.GetShotBallObj();
	}
	public int GetPointHoleDataNo(Collider collider)
	{
		for (int i = 0; i < pointHoleDatas.Length; i++)
		{
			if (pointHoleDatas[i].holeCollider.gameObject == collider.gameObject)
			{
				return i;
			}
		}
		return -1;
	}
}
