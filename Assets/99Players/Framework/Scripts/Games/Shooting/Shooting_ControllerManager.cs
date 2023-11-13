using System;
using UnityEngine;
public class Shooting_ControllerManager : SingletonCustom<Shooting_ControllerManager>
{
	[Serializable]
	private struct AimingOffsetPosition
	{
		public Vector3[] pos;
	}
	[SerializeField]
	[Header("カ\u30fcソルの方向を向く用のアンカ\u30fc（Ｚ座標のみ使用）")]
	private Transform lookZAnchor;
	[SerializeField]
	[Header("プレイヤ\u30fcが操作する銃")]
	private Shooting_Controller[] arrayController;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した銃の座標（１人用）")]
	private Vector3[] arraySingleContollerPosition;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した銃の座標（１人用_スタ\u30fcト前）")]
	private Vector3[] arraySingleContollerStartPosition;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した銃の座標（マルチ用）")]
	private Vector3[] arrayMultiContollerPosition;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した銃の補正座標")]
	private AimingOffsetPosition[] arrayAimingOffsetPosition;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した銃の補正座標")]
	private Vector3[] aimingOffsetPosition;
	private int[] arrayGunNo;
	[SerializeField]
	[Header("プレイヤ\u30fcの弾の色（１人用）")]
	private Material singleRubberColor;
	[SerializeField]
	[Header("プレイヤ\u30fcの弾の色（マルチ用）")]
	private Material[] arrayRubberColor;
	[SerializeField]
	[Header("プレイヤ\u30fcの弾の軌道の色")]
	private Color[] arrayTrailColor;
	private float rot_y;
	private float rot_x;
	private bool flg;
	private bool leftMoveFlg;
	private bool rightMoveFlg;
	private bool upMoveFlg;
	private float upMoveTime;
	private float rightMoveTime;
	private float leftMoveTime;
	[SerializeField]
	[Header("１人用のカ\u30fcソルの参照")]
	private Shooting_AimingCursor cursor;
	private int remainingBulletNum;
	[SerializeField]
	[Header("アンカ\u30fc")]
	private Transform controllerAnchor;
	[SerializeField]
	[Header("1人時の移動範囲")]
	private Transform singleRightTopAnchor;
	[SerializeField]
	private Transform singleLeftBottomAnchor;
	private Vector3 nowSingleMoveVec;
	public Transform LookZAnchor => lookZAnchor;
	public Shooting_Controller[] ArrayController => arrayController;
	public Material SingleBulletColor => singleRubberColor;
	public Material[] ArrayBulletColor => arrayRubberColor;
	public Color[] ArrayTrailColor => arrayTrailColor;
	public Vector3 NowSingleMoveVec => nowSingleMoveVec;
	public bool LeftMoveFlg => leftMoveFlg;
	public bool RightMoveFlg => rightMoveFlg;
	public bool UpMoveFlg => upMoveFlg;
	public int RemainingBulletNum => 12;
	public void SetChangeItem(Shooting_Controller[] _arrayController, Transform _lookZAnchor)
	{
		arrayController = _arrayController;
		lookZAnchor = _lookZAnchor;
	}
	public void Init()
	{
		arrayGunNo = new int[SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM];
		for (int i = 0; i < arrayGunNo.Length; i++)
		{
			arrayGunNo[i] = i;
		}
		SetGunController(arrayGunNo);
		SingletonCustom<Shooting_GameManager>.Instance.SingleCameraMoveInit();
	}
	public void SecondGroupInit()
	{
		for (int i = 0; i < arrayGunNo.Length; i++)
		{
			arrayController[arrayGunNo[i]].SecondGroupInit();
			SingletonCustom<Shooting_UIManager>.Instance.StopCursor(i);
		}
	}
	public void SetGunController(int[] _tmp)
	{
		arrayGunNo = _tmp;
		for (int i = 0; i < arrayGunNo.Length; i++)
		{
			if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
			{
				arrayController[arrayGunNo[i]].transform.localPosition = arraySingleContollerStartPosition[i];
			}
			else
			{
				arrayController[arrayGunNo[i]].transform.localPosition = arrayMultiContollerPosition[i];
			}
			arrayController[arrayGunNo[i]].Init(arrayGunNo[i]);
		}
	}
	public void SetSingleControllerPositionLerp(float _lerp)
	{
		for (int i = 1; i < arrayGunNo.Length; i++)
		{
			arrayController[arrayGunNo[i]].transform.localPosition = Vector3.Lerp(arraySingleContollerStartPosition[i], arraySingleContollerPosition[i], _lerp);
		}
	}
	public void UpdateLookCursor()
	{
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; i++)
		{
			arrayController[i].UpdateLookCursor();
		}
	}
	public void UpdateMethod()
	{
		UpdateLookCursor();
		if (!SingletonCustom<Shooting_GameManager>.Instance.IsGameStart || SingletonCustom<Shooting_GameManager>.Instance.IsGameEnd)
		{
			return;
		}
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; i++)
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
	public int[] GetArrayGunNo()
	{
		return arrayGunNo;
	}
	public bool CheckPlayerEnd()
	{
		bool result = true;
		for (int i = 0; i < arrayController.Length; i++)
		{
			if (arrayController[i].IsPlayer && !arrayController[i].IsBulletEnd)
			{
				result = false;
			}
		}
		return result;
	}
	public bool CheckAllBulletEnd()
	{
		for (int i = 0; i < arrayController.Length; i++)
		{
			if (!arrayController[i].IsBulletEnd)
			{
				return false;
			}
		}
		return true;
	}
	public void SingleMove(Vector2 _dir, GameObject _player, bool _isShot)
	{
		float x = _dir.x;
		float y = _dir.y;
		Vector3 vector = controllerAnchor.transform.position;
		Vector3 vector2 = vector;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		nowSingleMoveVec = Vector3.Lerp(b: new Vector3(x, y, 0f) * SingletonCustom<Shooting_UIManager>.Instance.CursorMoveSpeed, a: nowSingleMoveVec, t: Time.deltaTime * SingletonCustom<Shooting_UIManager>.Instance.CursorInputTimeValue);
		if (leftMoveFlg || rightMoveFlg)
		{
			nowSingleMoveVec.x = 0f;
		}
		if (upMoveFlg)
		{
			nowSingleMoveVec.y = 0f;
		}
		if (!_isShot)
		{
			vector += nowSingleMoveVec * Time.deltaTime * 10f;
		}
		vector.x = Mathf.Clamp(vector.x, singleLeftBottomAnchor.position.x, singleRightTopAnchor.position.x);
		vector.y = Mathf.Clamp(vector.y, singleLeftBottomAnchor.position.y, singleRightTopAnchor.position.y);
		if (vector2.x == vector.x)
		{
			nowSingleMoveVec.x = 0f;
		}
		if (vector2.y == vector.y)
		{
			nowSingleMoveVec.y = 0f;
		}
		if (vector.x == singleRightTopAnchor.transform.position.x)
		{
			rightMoveFlg = true;
		}
		else
		{
			rightMoveFlg = false;
		}
		if (vector.x == singleLeftBottomAnchor.transform.position.x)
		{
			leftMoveFlg = true;
		}
		else
		{
			leftMoveFlg = false;
		}
		if (vector.y == singleRightTopAnchor.transform.position.y)
		{
			upMoveFlg = true;
		}
		else
		{
			upMoveFlg = false;
		}
		if (upMoveFlg)
		{
			rot_x = float.Parse(Mathf.Clamp(rot_x + Time.deltaTime * (0f - y) * 3f, -10.5f, 0f).ToString("f2"));
			upMoveTime = float.Parse(Mathf.Clamp(-1f * rot_x * 1.5f / 10.5f, 0f, 1f).ToString("f2"));
			if (upMoveTime >= 0.01f)
			{
				flg = true;
			}
			else if (upMoveTime <= 0f)
			{
				upMoveFlg = false;
			}
		}
		if (leftMoveFlg)
		{
			rot_y = float.Parse(Mathf.Clamp(rot_y + Time.deltaTime * x * 3.5f, -23.5f, 0f).ToString("f2"));
			leftMoveTime = float.Parse(Mathf.Clamp(-1f * rot_y * 1.7f / 23.5f, 0f, 1f).ToString("f3"));
			if (leftMoveTime >= 0.001f)
			{
				flg = true;
			}
			else if (leftMoveTime <= 0f)
			{
				leftMoveFlg = false;
			}
		}
		if (rightMoveFlg)
		{
			rot_y = float.Parse(Mathf.Clamp(rot_y + Time.deltaTime * x * 3.5f, 0f, 23.5f).ToString("f2"));
			rightMoveTime = float.Parse(Mathf.Clamp(rot_y * 1.7f / 23.5f, 0f, 1f).ToString("f3"));
			if (rightMoveTime >= 0.001f)
			{
				flg = true;
			}
			else if (rightMoveTime <= 0f)
			{
				rightMoveFlg = false;
			}
		}
		if (flg)
		{
			if (leftMoveTime <= 0f && leftMoveFlg)
			{
				vector.x = Mathf.Clamp(vector.x + 0.5f, singleLeftBottomAnchor.position.x, singleRightTopAnchor.position.x);
				leftMoveTime = 0f;
				flg = false;
			}
			if (rightMoveTime <= 0f && rightMoveFlg)
			{
				vector.x = Mathf.Clamp(vector.x - 0.5f, singleLeftBottomAnchor.position.x, singleRightTopAnchor.position.x);
				rightMoveTime = 0f;
				flg = false;
			}
			if (upMoveTime <= 0f && upMoveFlg)
			{
				vector.y = Mathf.Clamp(vector.y - 0.5f, singleLeftBottomAnchor.position.y, singleRightTopAnchor.position.y);
				upMoveTime = 0f;
				flg = false;
			}
		}
		cursor.SingleMove(leftMoveTime, upMoveTime, rightMoveTime, y, x);
		controllerAnchor.transform.position = vector;
	}
	public void SingleStop()
	{
		nowSingleMoveVec = Vector3.zero;
	}
	public float GetSinglePosXLerp()
	{
		return Mathf.InverseLerp(singleLeftBottomAnchor.position.x, singleRightTopAnchor.position.x, controllerAnchor.transform.position.x);
	}
	public float GetSinglePosYLerp()
	{
		return Mathf.InverseLerp(singleLeftBottomAnchor.position.y, singleRightTopAnchor.position.y, controllerAnchor.transform.position.y);
	}
}
