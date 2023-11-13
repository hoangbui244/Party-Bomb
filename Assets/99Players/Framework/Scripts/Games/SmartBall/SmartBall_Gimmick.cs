using System;
using UnityEngine;
public class SmartBall_Gimmick : MonoBehaviour
{
	public enum GIMMICK_TYPE
	{
		TIME_CAP_TYPE,
		SWITCH_CAP_TYPE,
		SEESAW_TYPE,
		SPIN_TYPE
	}
	[SerializeField]
	[Header("ギミックの種類")]
	private GIMMICK_TYPE gimmickType;
	[SerializeField]
	[Header("動かすオブジェクト")]
	private GameObject moveParts;
	[SerializeField]
	[Header("ギミックの開始スイッチ")]
	private SmartBall_GimmickSwitch[] gimmickSwitch;
	private readonly float GIMMICK_OPEN_TIME = 0.1f;
	private readonly float GIMMICK_OPEN_LIMIT = 7f;
	private readonly float GIMMICK_OPEN_ANGLE = 70f;
	private readonly float SEESAW_ANGLE = 60f;
	private readonly float SEESAW_SPEED = 0.6f;
	private bool isRight;
	private bool isLeft;
	private readonly float GIMMICK_SPIN_ANGLE = 90f;
	private readonly float GIMMICK_SPIN_TIME = 0.4f;
	private readonly float SPIN_INTERVAL = 0.5f;
	private bool isOpen;
	private bool isMove;
	private bool isPlayerStand;
	public GIMMICK_TYPE GimmickType => gimmickType;
	public bool IsOpen => isOpen;
	public bool IsMove => isMove;
	public void UpdateMethod()
	{
		switch (gimmickType)
		{
		case GIMMICK_TYPE.TIME_CAP_TYPE:
			if (!isMove && gimmickSwitch[0].GimmickStartUp)
			{
				OpenCapGimmick();
			}
			if (isOpen)
			{
				LeanTween.delayedCall(GIMMICK_OPEN_LIMIT, (Action)delegate
				{
					CloseCapGimmick();
				});
				isOpen = false;
			}
			break;
		case GIMMICK_TYPE.SWITCH_CAP_TYPE:
			if (!isMove && gimmickSwitch[0].GimmickStartUp)
			{
				OpenCapGimmick();
			}
			if (gimmickSwitch[gimmickSwitch.Length - 1].GimmickStartUp && isOpen)
			{
				CloseCapGimmick();
			}
			break;
		case GIMMICK_TYPE.SEESAW_TYPE:
			if (gimmickSwitch[0].GimmickStartUp)
			{
				SeesawTiltRight();
			}
			else if (gimmickSwitch[1].GimmickStartUp)
			{
				SeesawTiltLeft();
			}
			ReturnSeesawAngle();
			break;
		case GIMMICK_TYPE.SPIN_TYPE:
			SpinGimmickProsecc();
			break;
		}
	}
	public void OpenCapGimmick()
	{
		isMove = true;
		LeanTween.rotateAroundLocal(moveParts, Vector3.right, GIMMICK_OPEN_ANGLE, GIMMICK_OPEN_TIME).setOnComplete((Action)delegate
		{
			isOpen = true;
		});
		if (isPlayerStand)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_smartball_gimmick_open");
		}
	}
	public void CloseCapGimmick()
	{
		LeanTween.rotateAroundLocal(moveParts, Vector3.right, 0f - GIMMICK_OPEN_ANGLE, GIMMICK_OPEN_TIME).setOnComplete((Action)delegate
		{
			isMove = false;
		});
		gimmickSwitch[gimmickSwitch.Length - 1].EndGimmickMove();
		gimmickSwitch[0].EndGimmickMove();
	}
	public void SeesawTiltRight()
	{
		if (!isMove)
		{
			isMove = true;
			LeanTween.rotateAroundLocal(moveParts, moveParts.transform.forward, SEESAW_ANGLE, SEESAW_SPEED).setOnComplete((Action)delegate
			{
				isRight = true;
			});
		}
	}
	public void SeesawTiltLeft()
	{
		if (!isMove)
		{
			isMove = true;
			LeanTween.rotateAroundLocal(moveParts, moveParts.transform.forward, 0f - SEESAW_ANGLE, SEESAW_SPEED).setOnComplete((Action)delegate
			{
				isLeft = true;
			});
		}
	}
	public void ReturnSeesawAngle()
	{
		if (isRight)
		{
			LeanTween.rotateAroundLocal(moveParts, moveParts.transform.forward, 0f - SEESAW_ANGLE, SEESAW_SPEED).setOnComplete((Action)delegate
			{
				ResetSeesaw();
			});
			isRight = false;
		}
		else if (isLeft)
		{
			LeanTween.rotateAroundLocal(moveParts, moveParts.transform.forward, SEESAW_ANGLE, SEESAW_SPEED).setOnComplete((Action)delegate
			{
				ResetSeesaw();
			});
			isLeft = false;
		}
	}
	private void ResetSeesaw()
	{
		isMove = false;
		if (gimmickSwitch[0].GimmickStartUp)
		{
			gimmickSwitch[0].EndGimmickMove();
		}
		if (gimmickSwitch[1].GimmickStartUp)
		{
			gimmickSwitch[1].EndGimmickMove();
		}
	}
	private void SpinGimmickProsecc()
	{
		if (!isMove)
		{
			if (gimmickSwitch[0].GimmickStartUp)
			{
				SpinGimmick();
				gimmickSwitch[0].EndGimmickMove();
			}
			if (gimmickSwitch[1].GimmickStartUp)
			{
				ReturnSpinGimmick();
				gimmickSwitch[1].EndGimmickMove();
			}
		}
	}
	private void SpinGimmick()
	{
		if (!(moveParts.transform.localEulerAngles.x > GIMMICK_SPIN_ANGLE / 2f))
		{
			isMove = true;
			LeanTween.rotateAroundLocal(moveParts, Vector3.right, GIMMICK_SPIN_ANGLE, GIMMICK_SPIN_TIME).setOnComplete((Action)delegate
			{
				isMove = false;
			});
		}
	}
	private void ReturnSpinGimmick()
	{
		if (!(moveParts.transform.localEulerAngles.x < GIMMICK_SPIN_ANGLE / 2f))
		{
			isMove = true;
			LeanTween.rotateAroundLocal(moveParts, Vector3.right, 0f - GIMMICK_SPIN_ANGLE, GIMMICK_SPIN_TIME).setOnComplete((Action)delegate
			{
				isMove = false;
			});
		}
	}
	public void CheckPlayerStand(bool _isPlayer)
	{
		isPlayerStand = _isPlayer;
	}
}
