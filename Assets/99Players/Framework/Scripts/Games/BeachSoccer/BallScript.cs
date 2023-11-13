using System;
using UnityEngine;
namespace BeachSoccer
{
	public class BallScript : MonoBehaviour
	{
		public struct CatchPos
		{
			public Vector3 run;
			public Vector3 keeper;
			public Vector3 keeperCatch;
			public Vector3 throwIn;
			public CatchPos(Vector3 _run, Vector3 _keeper, Vector3 _keeperCatch, Vector3 _throwIn)
			{
				run = _run;
				keeper = _keeper;
				keeperCatch = _keeperCatch;
				throwIn = _throwIn;
			}
		}
		private static float FAST_BALL_BORDER = 0.55f;
		private static float SLOW_BALL_BORDER = 0.2f;
		private Rigidbody rigid;
		[SerializeField]
		[Header("オブジェクト")]
		private Transform obj;
		private Vector3 prevPos;
		private Vector3 nowPos;
		private Vector3 startPos;
		private Vector3 lineOutPos;
		private Vector3[] calcVec = new Vector3[2];
		private float BALL_ROT_SPEED = 5000f;
		private CharacterScript lastHitChara;
		private CharacterScript lastShootChara;
		[SerializeField]
		[Header("ボ\u30fcルレンダラ\u30fc")]
		private MeshRenderer ballRenderer;
		[SerializeField]
		[Header("軌跡エフェクト")]
		private TrailRenderer afterEffect;
		private Vector3 SHADOW_SIZE_MIN = new Vector3(0.5f, 1f, 0.5f);
		private float SHADOW_ALPHA_DFE = 0.75f;
		private CatchPos catchPos = new CatchPos(new Vector3(0f, 0.275f, 0.6f), new Vector3(0f, 0.275f, 0.6f), new Vector3(0f, 0.5f, 0.58f), new Vector3(0f, 0.275f, 0.675f));
		private SphereCollider collider;
		private float boundTime;
		private float BOUND_SPEED = 10f;
		private float BOUND_HEIGHT = 0.1f;
		public static float FastBallBorder => FAST_BALL_BORDER;
		public static float SlowBallBorder => SLOW_BALL_BORDER;
		public void Init()
		{
			rigid = GetComponent<Rigidbody>();
			collider = GetComponent<SphereCollider>();
			startPos = (nowPos = (prevPos = base.transform.position));
			boundTime = (float)Math.PI;
		}
		public void UpdateMethod()
		{
		}
		private void FixedUpdate()
		{
			afterEffect.enabled = SingletonCustom<BallManager>.Instance.CheckBallState(BallManager.BallState.FREE);
			prevPos = nowPos;
			nowPos = base.transform.position;
			if (SingletonCustom<BallManager>.Instance.CheckBallState(BallManager.BallState.FREE))
			{
				MoveRot();
				obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
				boundTime = (float)Math.PI;
			}
			else if (SingletonCustom<BallManager>.Instance.CheckBallState(BallManager.BallState.KEEP))
			{
				if (!SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().IsBallCatch)
				{
					MoveRot();
				}
				if (CalcManager.Length(nowPos, prevPos) >= 0.05f && !SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().IsBallCatch)
				{
					boundTime += BOUND_SPEED * Time.deltaTime;
					obj.SetPosition(base.transform.position.x, base.transform.position.y + Mathf.Max((Mathf.Cos(boundTime) + 0.5f) * BOUND_HEIGHT, 0f), base.transform.position.z);
				}
				else
				{
					obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
					boundTime = (float)Math.PI;
				}
			}
			else
			{
				MoveRot();
				obj.SetPosition(base.transform.position.x, base.transform.position.y, base.transform.position.z);
				boundTime = (float)Math.PI;
			}
		}
		private void MoveRot()
		{
			obj.Rotate(CalcManager.PosRotation2D((nowPos - prevPos).normalized, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y) * CalcManager.Length(nowPos, prevPos) * BALL_ROT_SPEED * Time.deltaTime, Space.World);
		}
		public void ResetPos()
		{
			base.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			rigid.isKinematic = true;
			rigid.velocity = CalcManager.mVector3Zero;
			rigid.angularVelocity = CalcManager.mVector3Zero;
			switch (SingletonCustom<BallManager>.Instance.GetBallState())
			{
			case BallManager.BallState.KICK_OFF:
				base.transform.position = startPos;
				break;
			case BallManager.BallState.THROW_IN:
				base.transform.position = lineOutPos;
				break;
			case BallManager.BallState.CORNER_KICK:
			{
				Vector3 vector = SingletonCustom<FieldManager>.Instance.CheckCornerPos(lineOutPos, _ballPos: true);
				base.transform.SetPosition(vector.x, startPos.y, vector.z);
				break;
			}
			case BallManager.BallState.GOAL_KICK:
			{
				Vector3 goalKickPos = SingletonCustom<FieldManager>.Instance.GetGoalKickPos(SingletonCustom<MainGameManager>.Instance.GetSetPlayTeamNo(), _ballPos: true);
				base.transform.SetPosition(goalKickPos.x, startPos.y, goalKickPos.z);
				break;
			}
			}
			afterEffect.Clear();
		}
		public Vector3 GetResetPos(BallManager.BallState _state)
		{
			switch (_state)
			{
			case BallManager.BallState.KICK_OFF:
				return startPos;
			case BallManager.BallState.THROW_IN:
				return lineOutPos;
			case BallManager.BallState.CORNER_KICK:
			{
				Vector3 result = SingletonCustom<FieldManager>.Instance.CheckCornerPos(lineOutPos, _ballPos: true);
				result.y = startPos.y;
				return result;
			}
			case BallManager.BallState.GOAL_KICK:
			{
				Vector3 goalKickPos = SingletonCustom<FieldManager>.Instance.GetGoalKickPos(SingletonCustom<MainGameManager>.Instance.GetSetPlayTeamNo(), _ballPos: true);
				goalKickPos.y = startPos.y;
				return goalKickPos;
			}
			default:
				return startPos;
			}
		}
		public void Catch(CharacterScript _chara)
		{
			rigid.velocity = CalcManager.mVector3Zero;
			rigid.isKinematic = true;
			base.transform.parent = _chara.transform;
			switch (SingletonCustom<BallManager>.Instance.GetBallState())
			{
			case BallManager.BallState.KEEP:
				if (_chara.CheckPositionType(GameDataParams.PositionType.GK))
				{
					if (_chara.IsBallCatch)
					{
						base.transform.localPosition = catchPos.keeperCatch;
					}
					else
					{
						base.transform.localPosition = catchPos.keeper;
					}
				}
				else
				{
					base.transform.localPosition = catchPos.run;
				}
				break;
			case BallManager.BallState.THROW_IN:
				base.transform.localPosition = catchPos.throwIn;
				break;
			}
			SetAfterEffectColor(ColorPalet.white);
			SetLastHitChara(_chara);
		}
		public void Release()
		{
		}
		private void OnCollisionEnter(Collision _col)
		{
			if (_col.gameObject.tag == "Goalpost")
			{
				if (GetMoveVec().magnitude >= SLOW_BALL_BORDER)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_goalpost");
				}
			}
			else if (_col.gameObject.tag == "GoalNet")
			{
				rigid.velocity *= 0.1f;
				if (GetMoveVec().magnitude >= SLOW_BALL_BORDER)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_goal_net");
				}
			}
		}
		private void OnTriggerEnter(Collider _col)
		{
			if (!SingletonCustom<MainGameManager>.Instance.CheckInPlay())
			{
				return;
			}
			if (GameSaveData.GetSelectArea() != FieldManager.OUT_DISABLE_STAGE_NO && (_col.tag == "VerticalWall" || _col.tag == "HorizontalWall"))
			{
				if (nowPos != base.transform.position && (!CalcManager.CheckRange(base.transform.position.x, SingletonCustom<FieldManager>.Instance.GetAnchors().left.position.x, SingletonCustom<FieldManager>.Instance.GetAnchors().right.position.x) || !CalcManager.CheckRange(base.transform.position.z, SingletonCustom<FieldManager>.Instance.GetAnchors().front.position.z, SingletonCustom<FieldManager>.Instance.GetAnchors().back.position.z)))
				{
					prevPos = nowPos;
					nowPos = base.transform.position;
				}
				switch (SingletonCustom<FieldManager>.Instance.CheckLineOutDir(nowPos, prevPos, ref lineOutPos))
				{
				case FieldManager.DirType.LEFT:
					UnityEngine.Debug.Log("左サイドラインをわった : " + _col.name);
					SingletonCustom<MainGameManager>.Instance.OutSideLine();
					break;
				case FieldManager.DirType.RIGHT:
					UnityEngine.Debug.Log("右サイドラインをわった : " + _col.name);
					SingletonCustom<MainGameManager>.Instance.OutSideLine();
					break;
				case FieldManager.DirType.FRONT:
					UnityEngine.Debug.Log("手前ゴ\u30fcルラインをわった : " + _col.name);
					SingletonCustom<MainGameManager>.Instance.OutGoalLine();
					break;
				case FieldManager.DirType.BACK:
					UnityEngine.Debug.Log("奥ゴ\u30fcルラインをわった : " + _col.name);
					SingletonCustom<MainGameManager>.Instance.OutGoalLine();
					break;
				}
				Vector3 vector = nowPos;
				string str = vector.ToString();
				vector = prevPos;
				UnityEngine.Debug.Log("ラインをわった座標 = " + str + " : 前の座標 = " + vector.ToString());
				SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
			}
			else if (_col.tag == "Goal")
			{
				UnityEngine.Debug.Log("ゴ\u30fcル");
				SingletonCustom<MainGameManager>.Instance.Goal((!(base.transform.position.z > SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position.z)) ? 1 : 0);
			}
		}
		public Vector3 GetLineOutPos()
		{
			return lineOutPos;
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
		public void SetMaterial(Material _mat)
		{
			ballRenderer.material = _mat;
		}
		public void SetAfterEffectColor(Color _color)
		{
			_color.a = 0.01f;
			afterEffect.startColor = _color;
			_color.a = 0f;
			afterEffect.endColor = _color;
		}
		public void SetLastHitChara(CharacterScript _lastHitChara)
		{
			lastHitChara = _lastHitChara;
		}
		public CharacterScript GetLastHitChara()
		{
			return lastHitChara;
		}
		public void SetLastShootChara(CharacterScript _lastShootChara)
		{
			lastShootChara = _lastShootChara;
		}
		public CharacterScript GetLastShootChara()
		{
			return lastShootChara;
		}
		public bool CheckLastShootChara(int _teamNo)
		{
			if (lastShootChara == null)
			{
				return false;
			}
			return lastShootChara.TeamNo == _teamNo;
		}
		public bool CheckOpponentLastShootChara(int _teamNo)
		{
			if (lastShootChara == null)
			{
				return false;
			}
			return lastShootChara.TeamNo != _teamNo;
		}
		public bool CheckMyLastShootChara(int _teamNo)
		{
			if (lastShootChara == null)
			{
				return false;
			}
			return lastShootChara.TeamNo == _teamNo;
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
	}
}
