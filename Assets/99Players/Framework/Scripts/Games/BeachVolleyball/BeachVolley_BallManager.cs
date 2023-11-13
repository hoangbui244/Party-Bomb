using System;
using UnityEngine;
public class BeachVolley_BallManager : SingletonCustom<BeachVolley_BallManager>
{
	public enum BallState
	{
		NONE,
		FREE,
		SERVE,
		KEEP,
		THROW_IN,
		GOAL,
		JUMP_BALL,
		MAX
	}
	public struct ActionTimeData
	{
		public float standServeTime;
		public float JumpServeTime;
		public float tossTime;
		public float spikeTime;
		public float spikeHeightCorr;
		public float attackTime;
		public float missTouchTime;
		public float blockTime;
		public float niceBlockTime;
		public float timeParamCorr;
		public float timeDistanceCorr;
		public float timeDistanceCorrBack;
		public float timeDistanceCorrFront;
		public float serveTargetOffsetCorr;
		public ActionTimeData(float _standServeTime, float _JumpServeTime, float _tossTime, float _spikeTime, float _attackTime, float _missTouchTime, float _blockTime, float _niceBlockTime, float _spikeHeightCorr, float _timeParamCorr, float _timeDistanceCorr, float _timeDistanceCorrBack, float _timeDistanceCorrFront, float _serveTargetOffsetCorr)
		{
			standServeTime = _standServeTime;
			JumpServeTime = _JumpServeTime;
			tossTime = _tossTime;
			spikeTime = _spikeTime;
			attackTime = _attackTime;
			missTouchTime = _missTouchTime;
			blockTime = _blockTime;
			niceBlockTime = _niceBlockTime;
			spikeHeightCorr = _spikeHeightCorr;
			timeParamCorr = _timeParamCorr;
			timeDistanceCorr = _timeDistanceCorr;
			timeDistanceCorrBack = _timeDistanceCorrBack;
			timeDistanceCorrFront = _timeDistanceCorrFront;
			serveTargetOffsetCorr = _serveTargetOffsetCorr;
		}
	}
	private BallState ballState;
	private float[] ballStateTime = new float[7];
	[SerializeField]
	[Header("ボ\u30fcル")]
	private BeachVolley_Ball ball;
	private Vector3 dropPrediPos;
	private Vector3 dropPrediPosGround;
	private Vector3 dropPrediPosAir;
	private Vector3 lastTouchPos;
	private int touchCount;
	private float tossPower = 0.75f;
	private float blockPowerBase = 3f;
	private float blockPower = 5f;
	private float ballTypeSpeedMag;
	private float serveTossRotBase = 5f;
	private float serveTossRotAdd = 15f;
	private ActionTimeData actionTimeData = new ActionTimeData(1f, 0.275f, 1.4f, 0.27f, 1.8f, 0.5f, 2f, 0.3f, 0.75f, 0.025f, 0.01f, 0.0175f, 0.055f, 2f);
	private Vector3 ballPosOffset;
	private float STEAL_INTERVAL = 0.5f;
	private float TRAP_INTERVAL = 0.2f;
	public const float OVER_POWER_BORDER = 1.1f;
	public const float PASS_POWER_BORDER = 0.25f;
	private float stealInterval;
	private float trapInterval;
	private float ballControlTime;
	private Vector3 throwFallPos;
	private Vector3 defGravity;
	private float GRAVITY_MAG_SERVE = 2.75f;
	private float GRAVITY_MAG_SPIKE_DEF = 1.25f;
	private float GRAVITY_MAG_SPIKE_ADD = 0.5f;
	private float GRAVITY_MAG_SPIKE_MAX = 2f;
	[SerializeField]
	[Header("落下予測地点マ\u30fcク")]
	private SpriteRenderer[] dropPrediPosMark;
	private float dropPrediPosMarkRotSpeed = 80f;
	private float dropPrediPosMarkDefAlpha = 1f;
	private Color dropPrediPosMarkDefColor;
	public void PlusTouchCount()
	{
		touchCount++;
	}
	private Vector3 GetGap()
	{
		return Vector3.zero;
	}
	public Transform GetDropPrediPosMark()
	{
		return dropPrediPosMark[0].transform;
	}
	public void BoundFlgReset()
	{
		ball.ResetFlg();
	}
	public void Init()
	{
		ball.Init();
		defGravity = Physics.gravity;
		ballTypeSpeedMag = BeachVolley_Define.GetBallSpeedMag();
		if (dropPrediPosMark.Length != 0 && dropPrediPosMark[0] != null)
		{
			dropPrediPosMarkDefAlpha = dropPrediPosMark[0].color.a;
			dropPrediPosMarkDefColor = dropPrediPosMark[0].color;
		}
		float ballSize = BeachVolley_Define.GetBallSize();
		ball.transform.SetLocalScale(ballSize, ballSize, ballSize);
		ball.SetCatchPosPlusSize(ballSize);
		ballSize = BeachVolley_Define.GetBallAngleDrag();
		ball.GetRigid().angularDrag = ballSize;
		ballSize = BeachVolley_Define.GetBallDrag();
		ball.GetRigid().drag = ballSize;
	}
	public void UpdateMethod()
	{
		ball.UpdateMethod();
		if (!ball.IsBound && BeachVolley_Define.MCM.GetHaveBallChara() == null)
		{
			throwFallPos = CalcManager.GetVelocityFallPositionY(ball.GetRigid().velocity, GetBallPos(_offset: false), BeachVolley_Define.FM.GetFieldData().CenterCircle.position.y + GetBallSize(), ball.GetBallGravity());
		}
		ballStateTime[(int)ballState] += Time.deltaTime;
		ballControlTime += Time.deltaTime;
		stealInterval -= Time.deltaTime;
		trapInterval -= Time.deltaTime;
	}
	public void MoveServePos(int _teamNo)
	{
		ball.SetGhost(_flg: true);
		ball.transform.position = BeachVolley_Define.FM.GetServeAnchor(_teamNo).position + Vector3.up * (GetBallSize() * GetBall().transform.localScale.x);
		ball.ResetVelocity();
		touchCount = 0;
	}
	public void Catch(BeachVolley_Character _chara)
	{
		SetBallState(BallState.KEEP);
		ball.Catch(_chara);
	}
	public void Release(BeachVolley_Character _releaseChara = null)
	{
		ball.transform.parent = SingletonCustom<BeachVolley_FieldManager>.Instance.GetObjAnchor();
		ball.GetRigid().isKinematic = false;
		ball.IsBound = false;
		if (_releaseChara != null)
		{
			SingletonCustom<BeachVolley_GameUiManager>.Instance.FinishTimeLimit(_releaseChara.playerNo);
			SingletonCustom<BeachVolley_MainCharacterManager>.Instance.HaveBallCharaBallRelease();
		}
		else
		{
			SingletonCustom<BeachVolley_MainCharacterManager>.Instance.ResetHaveBallChara();
		}
		ball.SetLastHitChara(null);
		ball.SetLastControlChara(null);
		SetBallState(BallState.FREE);
		ball.ChangeOutline(ColorPalet.black);
	}
	public void ServeToss(BeachVolley_Character _actionChara, Vector3 _dir, float _gaugeValue, bool _isStand)
	{
		ball.transform.parent = _actionChara.transform;
		ball.SettingThrowPosition();
		ball.transform.parent = SingletonCustom<BeachVolley_FieldManager>.Instance.GetObjAnchor();
		ball.GetRigid().isKinematic = false;
		Vector3 a = Vector3.up * tossPower;
		if (!_isStand)
		{
			a *= 1f + (float)_actionChara.GetCharaParam().jump * 0.025f;
		}
		ball.GetRigid().angularVelocity = _actionChara.transform.right * (serveTossRotBase + serveTossRotAdd * _gaugeValue);
		a += _dir * 0.1f;
		ball.GetRigid().AddForce(a, ForceMode.Impulse);
	}
	public void StandServe(BeachVolley_Character _actionChara, Vector3 _vec)
	{
		Vector3 targetPos = CalcServeTagetPos(_actionChara, _vec);
		float num = CalcTimeDistanceCorr(_actionChara.TeamNo, _actionChara.GetPos(), targetPos);
		Physics.gravity = defGravity * ballTypeSpeedMag;
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), targetPos, (actionTimeData.standServeTime + num) / ballTypeSpeedMag, Physics.gravity.y);
		ball.GetRigid().velocity = velocityPositionVec;
		ball.GetRigid().angularVelocity = CalcManager.PosRotation2D(velocityPositionVec.normalized, Vector3.zero, 90f, CalcManager.AXIS.Y) * ball.GetRigid().angularVelocity.sqrMagnitude * 0.2f;
		BeachVolley_Define.Ball.SetLastControlChara(_actionChara);
		BeachVolley_Define.Ball.SetLastHitChara(_actionChara);
		UpdateDropPrediPos();
		BeachVolley_Define.MCM.AutoPlayChangeControlleChara(_actionChara);
	}
	public void JumpServe(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec)
	{
		Vector3 a = CalcServeTagetPos(_actionChara, _vec);
		UnityEngine.Debug.Log("ブレた");
		float num = UnityEngine.Random.Range(0f, 360f);
		a += new Vector3(Mathf.Cos(num * (float)Math.PI / 180f), 0f, Mathf.Sin(num * (float)Math.PI / 180f)).normalized * UnityEngine.Random.Range(0f, GetBallSize() * _actionChara.GetStatusData().jumpServeShufflePer * 0.4f);
		float num2 = CalcTimeDistanceCorr(_actionChara.TeamNo, _actionChara.GetPos(), a);
		_gaugeValue = 1f + _gaugeValue * 0.2f;
		Physics.gravity = defGravity * ballTypeSpeedMag * _gaugeValue * (1f + (float)_actionChara.GetCharaParam().offense * actionTimeData.timeParamCorr) * GRAVITY_MAG_SERVE;
		float num3 = (actionTimeData.JumpServeTime + num2) / _gaugeValue / (1f + (float)_actionChara.GetCharaParam().offense * actionTimeData.timeParamCorr);
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), a, num3 / ballTypeSpeedMag, Physics.gravity.y);
		ball.GetRigid().velocity = velocityPositionVec;
		ball.GetRigid().angularVelocity = CalcManager.PosRotation2D(velocityPositionVec.normalized, Vector3.zero, 90f, CalcManager.AXIS.Y) * ball.GetRigid().angularVelocity.sqrMagnitude * 0.2f;
		BeachVolley_Define.Ball.SetLastControlChara(_actionChara);
		BeachVolley_Define.Ball.SetLastHitChara(_actionChara);
		UpdateDropPrediPos();
		BeachVolley_Define.MCM.AutoPlayChangeControlleChara(_actionChara);
	}
	public Vector3 CalcServeTagetPos(BeachVolley_Character _actionChara, Vector3 _vec)
	{
		return BeachVolley_Define.FM.GetTargetPosServe(1 - _actionChara.TeamNo, _vec, 0f, GetBallSize(), _actionChara);
	}
	public void Toss(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec, bool _noRotation = true)
	{
		Vector3 vector = GetTossTargetPos(_actionChara, _vec) + GetGap();
		float num = CalcManager.Length(_actionChara.GetPos(), vector) * actionTimeData.timeDistanceCorrFront;
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), vector, actionTimeData.tossTime + num * 0.75f + _gaugeValue * 0.3f, Physics.gravity.y);
		ball.GetRigid().velocity = velocityPositionVec;
		if (_noRotation)
		{
			ball.GetRigid().angularVelocity = CalcManager.mVector3Zero;
		}
		else
		{
			ball.GetRigid().angularVelocity = CalcManager.PosRotation2D(velocityPositionVec.normalized, Vector3.zero, 90f, CalcManager.AXIS.Y) * (ball.GetRigid().angularVelocity.sqrMagnitude + velocityPositionVec.sqrMagnitude) * 0.2f;
		}
		BeachVolley_Define.Ball.SetLastControlChara(_actionChara);
		UpdateDropPrediPos();
	}
	private float CalcTimeDistanceCorr(int _teamNo, Vector3 _charaPos, Vector3 _targetPos)
	{
		_charaPos.y = _targetPos.y;
		return (BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z - BeachVolley_Define.FM.ConvertLocalPos(_charaPos, _teamNo).z) * actionTimeData.timeDistanceCorrFront + (BeachVolley_Define.FM.ConvertLocalPos(_targetPos, _teamNo).z - BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z) * actionTimeData.timeDistanceCorrBack + CalcManager.Length(_charaPos, _targetPos) * actionTimeData.timeDistanceCorr;
	}
	public Vector3 GetTossTargetPos(BeachVolley_Character _actionChara, Vector3 _vec)
	{
		_vec = _vec.normalized;
		if (_actionChara.TeamNo == 0)
		{
			if (!(_vec.z < -0.75f))
			{
				_vec = ((!(Mathf.Abs(_vec.x) <= 0.25f)) ? (Vector3.forward + Vector3.right * 0.5f * Mathf.Sign(_vec.x)).normalized : Vector3.forward);
			}
			else
			{
				_vec.z = 0.2f;
			}
		}
		else if (!(_vec.z > 0.75f))
		{
			_vec = ((!(Mathf.Abs(_vec.x) <= 0.25f)) ? (Vector3.back + Vector3.right * 0.5f * Mathf.Sign(_vec.x)).normalized : Vector3.back);
		}
		else
		{
			_vec.z = -0.2f;
		}
		return BeachVolley_Define.FM.GetTargetPos(_actionChara.TeamNo, _vec, 0f, GetBallSize());
	}
	public void Spike(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec)
	{
		Vector3 targetPos = CalcSpikeTargetPos(_actionChara, _gaugeValue, _vec) + GetGap();
		float num = CalcTimeDistanceCorr(_actionChara.TeamNo, _actionChara.GetPos(), targetPos);
		if (_actionChara.IsPlayer)
		{
			num = 0.27f;
		}
		float num2 = Mathf.Max(BeachVolley_Define.BM.GetUpperNetBorder() - BeachVolley_Define.BM.GetBallPos(_offset: false).y, 0f);
		if (_actionChara.IsPlayer)
		{
			num2 = 0f;
		}
		num += num2 * actionTimeData.spikeHeightCorr;
		_gaugeValue = 1f + _gaugeValue * 0.2f;
		float num3 = Mathf.Min(GRAVITY_MAG_SPIKE_DEF + Mathf.Max((BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z - BeachVolley_Define.FM.ConvertLocalPos(_actionChara.GetPos(), _actionChara.TeamNo).z) * 0.2f, num2) * GRAVITY_MAG_SPIKE_ADD * _gaugeValue, GRAVITY_MAG_SPIKE_MAX);
		if (_actionChara.CheckPositionState(BeachVolley_Character.PositionState.BACK_ZONE))
		{
			num3 *= 1.25f;
		}
		Physics.gravity = defGravity * num3 * ballTypeSpeedMag * (1f + (float)_actionChara.GetCharaParam().offense * actionTimeData.timeParamCorr);
		ball.ResetVelocity();
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), targetPos, (actionTimeData.spikeTime + num * 0.8f) / _gaugeValue / ballTypeSpeedMag / (1f + (float)_actionChara.GetCharaParam().offense * actionTimeData.timeParamCorr), Physics.gravity.y);
		ball.GetRigid().velocity = velocityPositionVec;
		ball.GetRigid().angularVelocity = CalcManager.PosRotation2D(velocityPositionVec.normalized, Vector3.zero, 90f, CalcManager.AXIS.Y) * velocityPositionVec.sqrMagnitude * 0.2f;
		BeachVolley_Define.Ball.SetLastControlChara(_actionChara);
		UpdateDropPrediPos();
	}
	public Vector3 CalcSpikeTargetPos(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec)
	{
		if (_vec.z <= -0.8f)
		{
			_vec.z = -0.8f;
		}
		if (_actionChara.CheckPositionState(BeachVolley_Character.PositionState.BACK_ZONE))
		{
			_vec = _actionChara.ConvertWordVec(_vec);
			float num = 1f - BeachVolley_Define.FM.GetAttackLineAnchor(_actionChara.TeamNo).localPosition.z / BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z;
			if (_vec.z < 0f - num)
			{
				_vec.z = 0f - num;
			}
			_vec = _actionChara.ConvertWordVec(_vec);
		}
		if (_gaugeValue >= 1.1f)
		{
			UnityEngine.Debug.Log("オ\u30fcバ\u30fcパワ\u30fc");
			_vec.z = _actionChara.transform.forward.z * UnityEngine.Random.Range(1.1f, 1.2f);
		}
		return BeachVolley_Define.FM.GetTargetPosByPer(1 - _actionChara.TeamNo, _vec, GetBallSize(), GetBallSize());
	}
	public void Attack(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec)
	{
		_vec = _vec.normalized;
		if (_gaugeValue >= 1.1f)
		{
			_vec.z = _actionChara.transform.forward.z * UnityEngine.Random.Range(1.1f, 1.2f);
		}
		else
		{
			float d = (_gaugeValue - 0.25f) / 0.7f;
			_vec *= d;
		}
		Vector3 targetPos = BeachVolley_Define.FM.GetTargetPos(1 - _actionChara.TeamNo, _vec, 0f, GetBallSize()) + GetGap();
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), targetPos, actionTimeData.attackTime, Physics.gravity.y);
		Vector3 vector = velocityPositionVec;
		UnityEngine.Debug.Log("ボ\u30fcル飛ばしたよ " + vector.ToString());
		ball.GetRigid().velocity = velocityPositionVec;
		BeachVolley_Define.Ball.SetLastControlChara(_actionChara);
		UpdateDropPrediPos();
	}
	public void Block(BeachVolley_Character _actionChara, float _gaugeValue, Vector3 _vec, bool _nice, bool _miss)
	{
		_vec.z = Mathf.Abs(_vec.z) * _actionChara.transform.forward.z;
		Vector3 normalized = (_actionChara.transform.forward + _vec.normalized).normalized;
		if (_miss)
		{
			normalized = (normalized + BeachVolley_Define.Ball.GetRigid().velocity).normalized;
		}
		normalized *= BeachVolley_Define.Ball.GetRigid().velocity.magnitude * 0.3f;
		if (_nice)
		{
			normalized += Vector3.down * (blockPowerBase + blockPower * _gaugeValue);
		}
		ball.GetRigid().velocity = normalized;
		UpdateDropPrediPos();
	}
	public void MissTouch(BeachVolley_Character _actionChara)
	{
		Vector3 pos = _actionChara.GetPos();
		pos.x += BeachVolley_Define.FM.GetFieldData().HalfCourtSize.x * UnityEngine.Random.Range(0.25f, 0.75f) * CalcManager.RandomPlusOrMinus();
		pos.z += BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z * UnityEngine.Random.Range(0.25f, 0.75f) * CalcManager.RandomPlusOrMinus();
		Vector3 velocityPositionVec = CalcManager.GetVelocityPositionVec(BeachVolley_Define.BM.GetBallPos(_offset: false), pos, actionTimeData.missTouchTime * UnityEngine.Random.Range(0f, 1f) + CalcManager.Length(BeachVolley_Define.BM.GetBallPos(_offset: false), pos) * 0.25f, Physics.gravity.y);
		ball.GetRigid().velocity = velocityPositionVec;
		UpdateDropPrediPos();
	}
	public void SetBallState(BallState _state)
	{
		ball.IsAir = (_state == BallState.FREE);
		ballStateTime[(int)_state] = 0f;
		ballState = _state;
	}
	public void SettingStealInterval(float _time = -1f)
	{
		if (_time < 0f)
		{
			_time = STEAL_INTERVAL;
		}
		stealInterval = _time;
	}
	public void SettingTrapInterval(float _time = -1f)
	{
		if (_time < 0f)
		{
			_time = TRAP_INTERVAL;
		}
		trapInterval = _time;
	}
	public void SetDropPrediPos(Vector3 _pos)
	{
		dropPrediPos = _pos;
	}
	public void SetDropPrediPosAir(Vector3 _pos)
	{
		dropPrediPosAir = _pos;
	}
	public void SetDropPrediPosGround(Vector3 _pos)
	{
		dropPrediPosGround = _pos;
	}
	public BeachVolley_Ball GetBall()
	{
		return ball;
	}
	public float GetBallSpeed(bool _rigid = false)
	{
		if (_rigid)
		{
			return ball.GetRigid().velocity.magnitude * 0.075f;
		}
		return ball.GetMoveVec().magnitude;
	}
	public float GetBallControlTime()
	{
		return ballControlTime;
	}
	public Vector3 GetBallPos(bool _offset = true, bool _local = false, bool _haveCheck = false)
	{
		if (_local)
		{
			ballPosOffset = ball.transform.localPosition;
			if (_haveCheck && CheckBallState(BallState.KEEP))
			{
				ballPosOffset = ball.transform.parent.localPosition;
			}
			if (_offset)
			{
				ballPosOffset.y = 0f;
			}
			return ballPosOffset;
		}
		ballPosOffset = ball.transform.position;
		if (_haveCheck && CheckBallState(BallState.KEEP))
		{
			ballPosOffset = ball.transform.parent.position;
		}
		if (_offset)
		{
			ballPosOffset.y = SingletonCustom<BeachVolley_FieldManager>.Instance.GetFieldData().CenterCircle.position.y;
		}
		return ballPosOffset;
	}
	public Vector3 GetBallDropPrediPos()
	{
		return dropPrediPos;
	}
	public Vector3 GetBallDropPrediPosGround()
	{
		return dropPrediPosGround;
	}
	public Vector3 GetBallDropPrediPosAir()
	{
		return dropPrediPosAir;
	}
	public float GetBallDistance(Vector3 _pos)
	{
		return CalcManager.Length(GetBallPos(_offset: false), _pos);
	}
	public float GetBallDropPrediPosDistance(Vector3 _pos, bool _isPlane = true)
	{
		if (_isPlane)
		{
			_pos.y = GetBallDropPrediPos().y;
		}
		return CalcManager.Length(GetBallDropPrediPos(), _pos);
	}
	public float GetBallDropPrediPosGroundDistance(Vector3 _pos, bool _isPlane = true)
	{
		if (_isPlane)
		{
			_pos.y = GetBallDropPrediPosGround().y;
		}
		return CalcManager.Length(GetBallDropPrediPosGround(), _pos);
	}
	public float GetBallSize()
	{
		return ball.GetCollider().radius;
	}
	public BallState GetBallState()
	{
		return ballState;
	}
	public Vector3 GetBallThrowFallPos()
	{
		return throwFallPos;
	}
	public float GetStateTime(BallState _state = BallState.MAX)
	{
		if (_state == BallState.MAX)
		{
			_state = ballState;
		}
		return ballStateTime[(int)_state];
	}
	public float GetUpperNetBorder()
	{
		return BeachVolley_Define.FM.GetFieldData().GetNetTopPos().y + GetBallSize() * 3f;
	}
	public bool CheckBallState(BallState _state)
	{
		return ballState == _state;
	}
	public bool CheckMoveBall()
	{
		if (ballState != BallState.FREE)
		{
			return ballState == BallState.KEEP;
		}
		return true;
	}
	public bool CheckStealInterval()
	{
		return stealInterval <= 0f;
	}
	public bool CheckTrapInterval()
	{
		return trapInterval <= 0f;
	}
	public bool CheckLastHitTeam(int _teamNo)
	{
		if (ball.GetLastHitChara() != null)
		{
			return ball.GetLastHitChara().TeamNo == _teamNo;
		}
		return false;
	}
	public bool CheckLastHitChara(BeachVolley_Character _chara)
	{
		if (ball.GetLastHitChara() != null)
		{
			return ball.GetLastHitChara() == _chara;
		}
		return false;
	}
	public bool CheckBallUpperNet()
	{
		return GetBallPos(_offset: false).y > GetUpperNetBorder();
	}
	public void ResetStateTime(BallState _state = BallState.MAX)
	{
		if (_state == BallState.MAX)
		{
			_state = ballState;
		}
		ballStateTime[(int)_state] = 0f;
	}
	public void ResetLastBallData()
	{
		ball.SetLastControlChara(null);
		ball.SetLastHitChara(null);
	}
	public void ResetBallControlTime()
	{
		ballControlTime = 0f;
	}
	public void UpdateDropPrediPos()
	{
		float num = BeachVolley_Define.FM.GetObjAnchor().position.y + BeachVolley_Define.Ball.GetCollider().radius;
		float num2 = num + 1.3f;
		if (GetBallPos(_offset: false).y > num2)
		{
			dropPrediPosAir = CalcManager.GetVelocityFallPositionY(ball.GetRigid().velocity, GetBallPos(_offset: false), num2, Physics.gravity.y);
			dropPrediPosAir.y = num;
		}
		else
		{
			dropPrediPosAir = GetBallPos(_offset: false);
		}
		dropPrediPosAir.y = num;
		if (GetBallPos(_offset: false).y > num)
		{
			dropPrediPosGround = CalcManager.GetVelocityFallPositionY(ball.GetRigid().velocity, GetBallPos(_offset: false), num, Physics.gravity.y);
		}
		else
		{
			dropPrediPosGround = GetBallPos(_offset: false);
			dropPrediPosGround.y = num;
		}
		if (GetBallPos(_offset: false).y <= num2)
		{
			dropPrediPos = dropPrediPosGround;
		}
		else
		{
			dropPrediPos = dropPrediPosAir;
		}
	}
	public void ShowDropPrediPos(bool _show = true)
	{
		if (!dropPrediPosMark[0].gameObject.activeSelf)
		{
			dropPrediPosMark[1].color = new Color(1f, 1f, 1f);
		}
		dropPrediPosMark[0].gameObject.SetActive(_show);
		if (!dropPrediPosMark[0].gameObject.activeSelf)
		{
			return;
		}
		dropPrediPosMark[0].transform.SetPositionX(dropPrediPosGround.x);
		dropPrediPosMark[0].transform.SetPositionZ(dropPrediPosGround.z);
		float num = (1f + Mathf.Min((GetBallPos(_offset: false).y - BeachVolley_Define.FM.GetFieldPos().y) / 6f, 1f) * 1.5f) * 0.5f;
		dropPrediPosMark[1].transform.SetLocalScaleX(num);
		dropPrediPosMark[1].transform.SetLocalScaleY(num);
		dropPrediPosMark[0].transform.Rotate(0f, 0f, (0f - dropPrediPosMarkRotSpeed) * Time.deltaTime);
		float num2 = Vector3.Distance(dropPrediPosMark[0].transform.position, BeachVolley_Define.BM.GetBallPos());
		float num3 = 6f;
		if (num2 < num3)
		{
			dropPrediPosMark[1].color = new Color(num2 / num3 * (1f - dropPrediPosMarkDefColor.r) + dropPrediPosMarkDefColor.r, num2 / num3 * (1f - dropPrediPosMarkDefColor.g) + dropPrediPosMarkDefColor.g, num2 / num3 * (1f - dropPrediPosMarkDefColor.b) + dropPrediPosMarkDefColor.b, (1f - num2 / num3) * (1f - dropPrediPosMarkDefColor.a) + dropPrediPosMarkDefColor.a);
		}
		else
		{
			dropPrediPosMark[1].color = new Color(1f, 1f, 1f, dropPrediPosMarkDefColor.a);
		}
		if (BeachVolley_Define.FM.CheckInCourt(dropPrediPosGround, 0f, BeachVolley_Define.BM.GetBallSize() * 0.4f))
		{
			dropPrediPosMark[0].color = Color.green;
		}
		else if (BeachVolley_Define.FM.CheckInCourt(dropPrediPosGround, 1f, BeachVolley_Define.BM.GetBallSize() * 0.4f))
		{
			dropPrediPosMark[0].color = Color.red;
		}
		else if (!(BeachVolley_Define.Ball.GetLastHitChara() == null))
		{
			if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo == 0)
			{
				dropPrediPosMark[0].color = Color.green;
			}
			else
			{
				dropPrediPosMark[0].color = Color.red;
			}
		}
	}
	public void UpdateLastTouchPos()
	{
	}
	public void ResetGravity()
	{
		Physics.gravity = defGravity;
	}
	public void TutorialInit()
	{
		ball.TutorialInit();
	}
	public void TutorialUpdate()
	{
		UpdateMethod();
	}
	private void OnDrawGizmos()
	{
		if (Application.isPlaying && ball.GetCollider() != null)
		{
			Gizmos.color = ColorPalet.lightgreen;
			Gizmos.DrawWireSphere(GetBallDropPrediPos(), GetBallSize());
		}
	}
}
