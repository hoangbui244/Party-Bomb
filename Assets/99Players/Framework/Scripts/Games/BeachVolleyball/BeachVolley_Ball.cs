using UnityEngine;
public class BeachVolley_Ball : MonoBehaviour
{
	public enum BallArea
	{
		IN_1P,
		OUT_1P,
		IN_2P,
		OUT_2P,
		COURT_OUT
	}
	public struct CatchPos
	{
		public Vector3 stand;
		public Vector3 shoot;
		public Vector3 charge;
		public Vector3 catchSuccess;
		public CatchPos(Vector3 _stand, Vector3 _shoot, Vector3 _charge, Vector3 _catchSuccess)
		{
			stand = _stand;
			shoot = _shoot;
			charge = _charge;
			catchSuccess = _catchSuccess;
		}
	}
	public enum OutLineColorType
	{
		ATTACK,
		TOSS
	}
	protected static float FAST_BALL_BORDER = 0.75f;
	protected static float SLOW_BALL_BORDER = 0.1f;
	protected Rigidbody rigid;
	[SerializeField]
	[Header("オブジェクト")]
	protected Transform obj;
	protected Vector3 defPos;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	protected Vector3 startPos;
	protected Vector3 lineOutPos;
	protected BallArea lastBoundBallArea;
	protected BallArea nowBallArea;
	protected float BALL_ROT_SPEED = 3000f;
	protected BeachVolley_Character lastHitChara;
	protected BeachVolley_Character lastControlChara;
	protected bool isAir;
	protected bool isServeBall;
	protected bool isBound;
	protected bool isPassingOutAntenna;
	[SerializeField]
	[Header("ボ\u30fcルレンダラ\u30fc")]
	protected MeshRenderer ballRenderer;
	[SerializeField]
	[Header("軌跡エフェクト")]
	protected TrailRenderer afterEffect;
	[SerializeField]
	[Header("バウンドエフェクト")]
	protected ParticleSystem boundEffect;
	[SerializeField]
	[Header("拾う判定の拡大用コライダ\u30fc")]
	protected SphereCollider pickUpCollider;
	protected float EXPANSION_TIME = 5f;
	protected float EXPANSION_RADIUS = 1.5f;
	protected bool usePassGravity;
	protected float passAddGravity;
	protected CatchPos catchPos = new CatchPos(new Vector3(0f, 0.75f, 0.8f), new Vector3(0f, 0.75f, 0.8f), new Vector3(0f, 0.75f, 0.8f), new Vector3(0f, 0.75f, 0.8f));
	private Vector3 catchPosPlusSize;
	protected SphereCollider collider;
	protected Color[] outlineChangeColor = new Color[2]
	{
		ColorPalet.yellow,
		ColorPalet.yellow
	};
	protected float outlineDefSize;
	protected float outlineChangeSize;
	protected bool isOverlap;
	protected float OVERLAP_CHECK_TIME = 0.05f;
	protected float overlapCheckTime;
	private Vector3 startLocalPos;
	private bool waitFlg;
	private float lastHitTime;
	public static float FastBallBorder => FAST_BALL_BORDER;
	public static float SlowBallBorder => SLOW_BALL_BORDER;
	public bool IsAir
	{
		get
		{
			return isAir;
		}
		set
		{
			isAir = value;
		}
	}
	public bool IsServeBall
	{
		get
		{
			return isServeBall;
		}
		set
		{
			isServeBall = value;
		}
	}
	public bool IsBound
	{
		get
		{
			return isBound;
		}
		set
		{
			isBound = value;
		}
	}
	public bool IsPassingOutAntenna
	{
		get
		{
			return isPassingOutAntenna;
		}
		set
		{
			isPassingOutAntenna = value;
		}
	}
	public void SetCatchPosPlusSize(float _size)
	{
		catchPosPlusSize = new Vector3(0f, 1f * (GetCollider().radius * _size), 1f * (GetCollider().radius * _size));
		catchPosPlusSize += new Vector3(0f, -0.4f, -0.4f);
	}
	public Vector3 GetStartLocalPos()
	{
		return startLocalPos;
	}
	public void Init()
	{
		rigid = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		outlineDefSize = ballRenderer.material.GetFloat("_Outline");
		outlineChangeSize = outlineDefSize * 3f;
		defPos = (startPos = (nowPos = (prevPos = base.transform.position)));
		startLocalPos = base.transform.localPosition;
		rigid.maxAngularVelocity = 20f;
	}
	public void UpdateMethod()
	{
		lastHitTime += Time.deltaTime;
		if (base.transform.position.y <= BeachVolley_Define.FM.GetFieldData().CenterCircle.position.y - BallSize())
		{
			isAir = false;
			if (BeachVolley_Define.MGM.CheckInPlay())
			{
				BeachVolley_Define.MGM.OutCourt();
			}
		}
		if (base.transform.localPosition.y >= 8f && rigid.velocity.y > 0f && !isServeBall)
		{
			rigid.velocity *= 0.8f;
		}
		if (BeachVolley_Define.MGM.CheckInPlay())
		{
			if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE) && rigid.velocity.magnitude <= 0.1f)
			{
				rigid.velocity = rigid.velocity.normalized * 0.1f;
			}
			if (!BeachVolley_Define.FM.CheckOnDesk(base.transform.position))
			{
				BeachVolley_Define.MGM.OutCourt();
			}
		}
		CheckOverlap();
		afterEffect.enabled = BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE);
		prevPos = nowPos;
		nowPos = base.transform.position;
		if (BeachVolley_Define.MGM.CheckInPlay() && BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE) && BeachVolley_Define.MCM.teamUserList[BeachVolley_Define.MCM.BallControllTeam][0] <= 3)
		{
			BeachVolley_Character controlChara = BeachVolley_Define.MCM.GetControlChara(BeachVolley_Define.MCM.BallControllTeam);
			if (controlChara != null)
			{
				float max = BeachVolley_Define.FM.GetFieldData().GetCenterPos().y + controlChara.GetStatusData().jumpHeight + controlChara.GetCharaHeight() + BallSize() * 1f;
				if (!isServeBall && (CalcManager.CheckRange(base.transform.position.y, BeachVolley_Define.BM.GetUpperNetBorder(), max, _include: false) || (BeachVolley_Define.MCM.CheckLastTouch() && base.transform.position.y < BeachVolley_Define.BM.GetUpperNetBorder())) && !BeachVolley_Define.MCM.GetControlChara(BeachVolley_Define.MCM.BallControllTeam).CheckPositionType(BeachVolley_Define.PositionType.LIBERO))
				{
					ChangeOutline(outlineChangeColor[0], _thickLine: true);
				}
				else if (CalcManager.CheckRange(base.transform.position.y, controlChara.GetPos().y + controlChara.GetCharaHeight(), BeachVolley_Define.BM.GetUpperNetBorder()))
				{
					ChangeOutline(outlineChangeColor[1], _thickLine: true);
				}
				else
				{
					ChangeOutline(ColorPalet.black);
				}
			}
			else
			{
				ChangeOutline(ColorPalet.black);
			}
		}
		else
		{
			ChangeOutline(ColorPalet.black);
		}
		if (!BeachVolley_Define.MGM.CheckInPlayOrEndWait())
		{
			return;
		}
		if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
		{
			obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			if (rigid.velocity.magnitude <= 0.04f)
			{
				rigid.velocity = rigid.velocity.normalized * 0.04f;
			}
		}
		else if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.KEEP))
		{
			if (BeachVolley_Define.MCM.GetHaveBallChara() != null && BeachVolley_Define.MCM.GetHaveBallChara().CheckActionState(BeachVolley_Character.ActionState.STANDARD))
			{
				obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			}
			else
			{
				obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			}
		}
		else
		{
			obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		}
	}
	public void JumpBallTos()
	{
		rigid.AddForce(Vector3.up * UnityEngine.Random.Range(1.35f, 1.44f), ForceMode.Impulse);
	}
	protected void MoveRot(bool _toss = false)
	{
		if (_toss)
		{
			obj.Rotate((BeachVolley_Define.MCM.BallControllTeam == 0) ? Vector3.forward : Vector3.back, Space.World);
		}
		else
		{
			obj.Rotate(CalcManager.PosRotation2D((nowPos - prevPos).normalized, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y) * CalcManager.Length(nowPos, prevPos) * BALL_ROT_SPEED * Time.deltaTime, Space.World);
		}
	}
	public void ResetPos()
	{
		base.transform.parent = BeachVolley_Define.FM.GetObjAnchor();
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.angularVelocity = CalcManager.mVector3Zero;
		switch (BeachVolley_Define.BM.GetBallState())
		{
		case BeachVolley_BallManager.BallState.JUMP_BALL:
			base.transform.position = startPos;
			rigid.isKinematic = false;
			break;
		case BeachVolley_BallManager.BallState.THROW_IN:
			rigid.isKinematic = true;
			base.transform.position = lineOutPos;
			break;
		}
		afterEffect.Clear();
	}
	public void ResetVelocity()
	{
		bool isKinematic = GetRigid().isKinematic;
		if (!isKinematic)
		{
			GetRigid().isKinematic = true;
		}
		GetRigid().velocity = Vector3.zero;
		GetRigid().angularVelocity = Vector3.zero;
		if (!isKinematic)
		{
			GetRigid().isKinematic = false;
		}
	}
	public void SettingStandPosition()
	{
		SetOverlapCheck();
		base.transform.localPosition = catchPos.stand + catchPosPlusSize;
	}
	public void SettingThrowPosition()
	{
		SetOverlapCheck();
		base.transform.localPosition = catchPos.shoot + catchPosPlusSize;
	}
	public void SettingShootPosition()
	{
		SetOverlapCheck();
		base.transform.localPosition = catchPos.shoot + catchPosPlusSize;
	}
	public void SettingChargePosition()
	{
		SetOverlapCheck();
		base.transform.localPosition = catchPos.charge + catchPosPlusSize;
	}
	public void SettingCatchSuccessPosition()
	{
		SetOverlapCheck();
		base.transform.localPosition = catchPos.catchSuccess + catchPosPlusSize;
	}
	public void SetLineOutPos(Vector3 _lineOutPos)
	{
		lineOutPos = _lineOutPos;
	}
	public Vector3 GetResetPos(BeachVolley_BallManager.BallState _state)
	{
		if (_state != BeachVolley_BallManager.BallState.THROW_IN)
		{
			return startPos;
		}
		return lineOutPos;
	}
	public void Catch(BeachVolley_Character _chara)
	{
		SetOverlapCheck();
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.isKinematic = true;
		base.transform.parent = _chara.transform;
		isBound = false;
		switch (BeachVolley_Define.BM.GetBallState())
		{
		case BeachVolley_BallManager.BallState.KEEP:
			base.transform.localPosition = catchPos.stand;
			break;
		case BeachVolley_BallManager.BallState.THROW_IN:
			base.transform.localPosition = catchPos.stand + catchPosPlusSize;
			break;
		}
		if (_chara.CheckActionState(BeachVolley_Character.ActionState.CATCH_SUCCESS))
		{
			base.transform.localPosition = catchPos.catchSuccess + catchPosPlusSize;
		}
		SetAfterEffectColor(ColorPalet.white);
		SetLastHitChara(_chara);
		SetLastControlChara(_chara);
	}
	public void MiniGameCatch(BeachVolley_Character _chara)
	{
		SetOverlapCheck();
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.isKinematic = true;
		base.transform.parent = _chara.transform;
		isBound = false;
		base.transform.localPosition = catchPos.stand + catchPosPlusSize;
		if (_chara.CheckActionState(BeachVolley_Character.ActionState.CATCH_SUCCESS))
		{
			base.transform.localPosition = catchPos.catchSuccess + catchPosPlusSize;
		}
		SetAfterEffectColor(ColorPalet.white);
		SetLastHitChara(_chara);
		SetLastControlChara(_chara);
	}
	protected void SetOverlapCheck()
	{
		overlapCheckTime = OVERLAP_CHECK_TIME;
		isOverlap = true;
		SetIsTrigger(_flg: true);
	}
	protected void CheckOverlap()
	{
		if (!isOverlap)
		{
			return;
		}
		overlapCheckTime -= Time.deltaTime;
		if (overlapCheckTime <= 0f)
		{
			overlapCheckTime = OVERLAP_CHECK_TIME;
			if (Physics.OverlapSphere(base.transform.position, BallSize() * 0.5f, LayerMask.GetMask(BeachVolley_Define.LAYER_CHARACTER, BeachVolley_Define.LAYER_GOAL, BeachVolley_Define.LAYER_OBJECT)).Length == 0)
			{
				isOverlap = false;
				SetIsTrigger(_flg: false);
			}
		}
	}
	protected virtual void CheckOutOfBounds()
	{
		if (!BeachVolley_Define.MGM.CheckInPlay())
		{
			return;
		}
		if (BeachVolley_Define.FM.CheckInCourt(base.transform.position, BeachVolley_Define.BM.GetBallSize() * 0.4f))
		{
			if (IsPassingOutAntenna)
			{
				BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.OUT_OF_BOUNDS, (!(base.transform.position.z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)) ? 1 : 0);
				return;
			}
			int teamNo = (base.transform.position.z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z) ? 1 : 0;
			BeachVolley_Define.MGM.InCourt(teamNo);
		}
		else
		{
			BeachVolley_Define.MGM.OutCourt();
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
	}
	protected void CreateBoundEffect()
	{
		Vector3 position = base.transform.position;
		position.y = BeachVolley_Define.FM.GetObjAnchor().position.y + 0.15f;
		Object.Instantiate(boundEffect, position, Quaternion.identity, BeachVolley_Define.FM.GetObjAnchor());
	}
	protected virtual void OnCollisionEnter(Collision _col)
	{
		if (!BeachVolley_Define.MGM.CheckInPlay() || !BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
		{
			return;
		}
		if (_col.gameObject.tag == BeachVolley_Define.TAG_CHARACTER)
		{
			UnityEngine.Debug.Log("最後に触ったGetLastHitChara():" + GetLastHitChara().gameObject.name);
			if (_col.gameObject == GetLastHitChara().gameObject)
			{
				UnityEngine.Debug.Log("最後に触ったキャラに当たった");
				return;
			}
		}
		BeachVolley_Define.BM.ResetGravity();
		isAir = false;
		if (_col.gameObject.tag == BeachVolley_Define.TAG_CHARACTER)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_ball_bound");
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_ball_bound");
		}
		if (_col.gameObject.tag == BeachVolley_Define.TAG_NET)
		{
			UnityEngine.Debug.Log("ａｎｉｍａｔｉｏｎスタ\u30fcト");
			BeachVolley_Define.FM.StartNetAnimation(base.transform.localPosition.z > 0f);
			Vector3 velocity = rigid.velocity;
			velocity *= 0.9f;
			rigid.velocity = velocity;
		}
		if (_col.gameObject.tag == BeachVolley_Define.TAG_ANTENNA)
		{
			BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.OUT_OF_BOUNDS, GetLastHitChara().TeamNo);
		}
		else if (_col.gameObject.tag == BeachVolley_Define.TAG_FIELD || (_col.gameObject.tag == BeachVolley_Define.TAG_OBJECT && !BeachVolley_Define.FM.CheckInCourt(base.transform.position, (0f - BeachVolley_Define.BM.GetBallSize()) * 0.4f)))
		{
			if (_col.gameObject.tag == BeachVolley_Define.TAG_FIELD && !isBound)
			{
				CreateBoundEffect();
			}
			isBound = true;
			CheckOutOfBounds();
		}
	}
	public void ResetFlg()
	{
		waitFlg = false;
	}
	private void OnTriggerEnter(Collider _col)
	{
		if (BeachVolley_Define.MGM.CheckInPlay() && BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE) && !waitFlg)
		{
			if (_col.tag == BeachVolley_Define.TAG_ANTENNA)
			{
				BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.OUT_OF_BOUNDS, GetLastHitChara().TeamNo);
			}
			else if (_col.tag == BeachVolley_Define.TAG_HORIZONTAL_WALL || _col.tag == BeachVolley_Define.TAG_VERTICAL_WALL)
			{
				UnityEngine.Debug.Log("得点入った");
				waitFlg = true;
				CheckOutOfBounds();
			}
		}
	}
	public Vector3 GetLineOutPos()
	{
		return lineOutPos;
	}
	public Vector3 GetDefPos()
	{
		return defPos;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
	}
	public SphereCollider GetCollider()
	{
		return collider;
	}
	public Vector3 GetStartPos()
	{
		return startPos;
	}
	public float BallSize()
	{
		return collider.radius * 2f;
	}
	public Vector3 GetMoveVec(bool _rigid = false)
	{
		if (_rigid)
		{
			return GetRigid().velocity * 0.075f;
		}
		return nowPos - prevPos;
	}
	public float GetBallGravity(bool _isMinus = true)
	{
		float num = Physics.gravity.y;
		if (usePassGravity)
		{
			num -= passAddGravity / rigid.mass;
		}
		if (!_isMinus)
		{
			num = 0f - num;
		}
		return num;
	}
	public BeachVolley_Character GetLastHitChara()
	{
		return lastHitChara;
	}
	public BeachVolley_Character GetLastControlChara()
	{
		return lastControlChara;
	}
	public void SetGhost(bool _flg)
	{
		GetCollider().isTrigger = _flg;
		rigid.useGravity = !_flg;
	}
	public void Show(bool _flg)
	{
		base.gameObject.SetActive(_flg);
	}
	public void SetIsTrigger(bool _flg)
	{
		GetCollider().isTrigger = _flg;
	}
	public void SetMesh(Mesh _mesh)
	{
		MeshFilter componentInChildren = ballRenderer.GetComponentInChildren<MeshFilter>();
		if ((bool)componentInChildren)
		{
			componentInChildren.mesh = _mesh;
		}
		else
		{
			UnityEngine.Debug.Log("MeshFilterが見つかりません");
		}
	}
	public void SetMaterial(Material _mat)
	{
		ballRenderer.material = _mat;
	}
	public void SetChangeOutlineColor(Color[] _color)
	{
		outlineChangeColor = _color;
	}
	public void SetChangeOutlineColor(OutLineColorType _type, Color _color)
	{
		outlineChangeColor[(int)_type] = _color;
	}
	public void ChangeOutline(Color _color, bool _thickLine = false)
	{
		ballRenderer.material.SetColor("_OutlineColor", _color);
		ballRenderer.material.SetFloat("_Outline", _thickLine ? outlineChangeSize : outlineDefSize);
	}
	public void SetAfterEffectColor(Color _color)
	{
		_color.a = 0.01f;
		afterEffect.startColor = _color;
		_color.a = 0f;
		afterEffect.endColor = _color;
	}
	public void SetLastHitChara(BeachVolley_Character _chara)
	{
		lastHitChara = _chara;
		lastHitTime = 0f;
	}
	public float GetLastHitTime()
	{
		return lastHitTime;
	}
	public void SetLastControlChara(BeachVolley_Character _chara)
	{
		lastControlChara = _chara;
	}
	public void SetPassGravity(float _addGravity)
	{
		passAddGravity = _addGravity;
		usePassGravity = true;
	}
	private void OnDrawGizmos()
	{
		if ((bool)collider)
		{
			Gizmos.color = ColorPalet.lightblue;
			Gizmos.DrawWireSphere(prevPos, collider.radius);
			Gizmos.color = ColorPalet.blue;
			Gizmos.DrawWireSphere(nowPos, collider.radius);
			Gizmos.color = ColorPalet.red;
			Gizmos.DrawWireSphere(lineOutPos, collider.radius);
		}
	}
	public void TutorialInit()
	{
		Init();
		base.transform.position += Vector3.down * 100f;
		rigid.isKinematic = true;
	}
	public void TutorialUpdate()
	{
	}
}
