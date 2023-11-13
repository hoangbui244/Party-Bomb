using UnityEngine;
public class Shooting_AimingCursor : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer cursorSprite;
	private int gunNo;
	private float sensitivityValue;
	private bool isSpeedUp;
	private float inputTime;
	private Vector3 moveTargetPos;
	private Vector3 nowMovePos;
	private Vector3 nowMoveVec;
	private bool isSingle;
	private float singleDefaultScale;
	private float singleZoomScale;
	private Vector3 startPos;
	[SerializeField]
	[Header("カ\u30fcソルの左の最大")]
	private Vector3 leftEndPos;
	[SerializeField]
	[Header("カ\u30fcソルの右の最大")]
	private Vector3 rightEndPos;
	[SerializeField]
	[Header("カ\u30fcソルの上の最大")]
	private Vector3 upEndPos;
	private Vector3 ObjPos;
	private Vector3 sidePos;
	private Vector3 upDownPos;
	[SerializeField]
	[Header("プレイヤ\u30fcコントロ\u30fcラ\u30fc")]
	private Shooting_Controller controller;
	public void Init(int _no)
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			startPos = cursorSprite.transform.localPosition;
			ObjPos = cursorSprite.transform.localPosition;
		}
		gunNo = _no;
		moveTargetPos = (nowMovePos = base.transform.position);
		PositionUpdate();
		isSingle = (gunNo == 0 && SingletonCustom<Shooting_GameManager>.Instance.IsSingle);
		if (isSingle)
		{
			SingleSetting();
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<Shooting_GameManager>.Instance.GetIsPlayer(gunNo))
		{
			cursorSprite.color = Shooting_Define.CHARA_COLORS[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[controller.PlayerNo]];
			cursorSprite.gameObject.SetActive(value: true);
		}
		else if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			cursorSprite.color = Shooting_Define.CHARA_COLORS[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[controller.PlayerNo]];
			cursorSprite.gameObject.SetActive(value: false);
		}
		else
		{
			cursorSprite.color = Shooting_Define.CHARA_COLORS[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[controller.PlayerNo]];
			cursorSprite.gameObject.SetActive(value: true);
		}
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<Shooting_ControllerManager>.Instance.ArrayController[gunNo].IsShot)
		{
			if (!controller.IsPlayer || !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				PositionUpdate();
			}
			if (controller.RemainingBulletNim == 0)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
	private void PositionUpdate()
	{
		base.transform.position = nowMovePos;
	}
	public void Move(Vector2 _dir, bool _isSpeedUp)
	{
		float x = _dir.x;
		float y = _dir.y;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		isSpeedUp = _isSpeedUp;
		mVector3Zero = new Vector3(x, y, 0f) * (isSpeedUp ? 2f : 1f);
		nowMoveVec = Vector3.Lerp(nowMoveVec, mVector3Zero * SingletonCustom<Shooting_UIManager>.Instance.CursorMoveSpeed, Time.deltaTime * SingletonCustom<Shooting_UIManager>.Instance.CursorInputTimeValue);
		Vector3 vector = nowMovePos;
		nowMovePos += nowMoveVec * Time.deltaTime * SingletonCustom<Shooting_UIManager>.Instance.CursorMoveValue;
		nowMovePos.x = SingletonCustom<Shooting_UIManager>.Instance.GetClampMoveTargetPosX(nowMovePos.x);
		nowMovePos.y = SingletonCustom<Shooting_UIManager>.Instance.GetClampMoveTargetPosY(nowMovePos.y);
		if (vector.x == nowMovePos.x)
		{
			nowMoveVec.x = 0f;
		}
		if (vector.y == nowMovePos.y)
		{
			nowMoveVec.y = 0f;
		}
	}
	public void Stop()
	{
		nowMoveVec = Vector3.zero;
	}
	public Vector3 GetPos()
	{
		return base.transform.position;
	}
	public Vector3 GetMoveVec()
	{
		return nowMoveVec;
	}
	private void SingleSetting()
	{
		singleDefaultScale = base.transform.localScale.x;
		singleZoomScale = singleDefaultScale * 1.5f;
	}
	public void SetSingleScaleLerp(float _lerp)
	{
		float num = Mathf.Lerp(singleDefaultScale, singleZoomScale, _lerp);
		base.transform.SetLocalScaleX(num);
		base.transform.SetLocalScaleY(num);
	}
	public void SingleMove(float _leftTime, float _upTime, float _rightTime, float _vertical, float _horizontal)
	{
		if (SingletonCustom<Shooting_ControllerManager>.Instance.LeftMoveFlg)
		{
			sidePos = Vector3.Lerp(startPos, leftEndPos, _leftTime);
		}
		if (SingletonCustom<Shooting_ControllerManager>.Instance.RightMoveFlg)
		{
			sidePos = Vector3.Lerp(startPos, rightEndPos, _rightTime);
		}
		if (SingletonCustom<Shooting_ControllerManager>.Instance.UpMoveFlg)
		{
			upDownPos = Vector3.Lerp(startPos, upEndPos, _upTime);
		}
		cursorSprite.transform.localPosition = upDownPos + sidePos;
	}
}
