using UnityEngine;
public class RingToss_ControllerManager : SingletonCustom<RingToss_ControllerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc操作クラス")]
	private RingToss_Controller[] arrayController;
	private int[] arrayCtrlNo;
	[SerializeField]
	[Header("アンカ\u30fc")]
	private Transform controllerAnchor;
	[SerializeField]
	[Header("輪の移動範囲")]
	private Transform ringRightEdgeAnchor;
	[SerializeField]
	private Transform ringLeftEdgeAnchor;
	[SerializeField]
	[Header("落下地点の移動範囲")]
	private Transform aimNearLeftEdgeAnchor;
	[SerializeField]
	private Transform aimFarRightEdgeAnchor;
	private Vector3 nowSingleMoveVec;
	public RingToss_Controller[] ArrayController => arrayController;
	public Vector3 NowSingleMoveVec => nowSingleMoveVec;
	public int RemainingRingNum => 10;
	public void Init()
	{
		arrayCtrlNo = new int[RingToss_Define.MAX_PLAYER_NUM];
		for (int i = 0; i < arrayCtrlNo.Length; i++)
		{
			arrayCtrlNo[i] = i;
		}
		SetController();
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < arrayCtrlNo.Length; i++)
		{
			arrayController[arrayCtrlNo[i]].SecondGroupInit();
		}
	}
	public void SetController()
	{
		for (int i = 0; i < arrayCtrlNo.Length; i++)
		{
			arrayController[arrayCtrlNo[i]].Init(arrayCtrlNo[i]);
		}
	}
	public void SetStartFutureLineView()
	{
		for (int i = 0; i < arrayCtrlNo.Length; i++)
		{
			arrayController[arrayCtrlNo[i]].SetFutureLineActive(_active: true);
		}
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<RingToss_GameManager>.Instance.IsGameStart || SingletonCustom<RingToss_GameManager>.Instance.IsGameEnd)
		{
			return;
		}
		for (int i = 0; i < RingToss_Define.MAX_PLAYER_NUM; i++)
		{
			bool flag = true;
			if (arrayController[i].IsPlayer)
			{
				arrayController[i].PlayerControl();
				flag = false;
			}
			if (flag)
			{
				arrayController[i].AiUpdate();
			}
			arrayController[i].UpdateMethod();
		}
	}
	public int[] GetArrayCtrlNo()
	{
		return arrayCtrlNo;
	}
	public bool CheckPlayerEnd()
	{
		bool result = true;
		for (int i = 0; i < arrayController.Length; i++)
		{
			if (arrayController[i].IsPlayer && !arrayController[i].IsRingEnd)
			{
				result = false;
			}
		}
		return result;
	}
	public bool CheckAllRingEnd()
	{
		for (int i = 0; i < arrayController.Length; i++)
		{
			if (!arrayController[i].IsRingEnd)
			{
				return false;
			}
		}
		return true;
	}
	public Vector3 ClampRingPosition(Vector3 _ringPos)
	{
		_ringPos.x = Mathf.Clamp(_ringPos.x, ringLeftEdgeAnchor.position.x, ringRightEdgeAnchor.position.x);
		return _ringPos;
	}
	public Vector3 ClampAimPosition(Vector3 _aimPos)
	{
		_aimPos.z = Mathf.Clamp(_aimPos.z, aimNearLeftEdgeAnchor.position.z, aimFarRightEdgeAnchor.position.z);
		return _aimPos;
	}
	public float GetLeapAimPositionZ(float _lerp)
	{
		return Mathf.Lerp(aimNearLeftEdgeAnchor.position.z, aimFarRightEdgeAnchor.position.z, _lerp);
	}
}
