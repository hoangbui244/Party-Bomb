using UnityEngine;
namespace BeachSoccer
{
	public class BallManager : SingletonCustom<BallManager>
	{
		public enum BallState
		{
			FREE,
			KEEP,
			THROW_IN,
			GOAL_KICK,
			CORNER_KICK,
			GOAL,
			KICK_OFF,
			MAX
		}
		private BallState ballState;
		private float[] ballStateTime = new float[7];
		[SerializeField]
		[Header("ボ\u30fcル")]
		private BallScript ball;
		private float passSpeed = 1.5f;
		private float shootSpeed = 0.4f;
		private Vector3 ballPosOffset;
		private float STEAL_INTERVAL = 0.5f;
		private float PASS_POWER_BORDER = 0.5f;
		private float stealInterval;
		public void Init()
		{
			ball.Init();
		}
		public void UpdateMethod()
		{
			ball.UpdateMethod();
			ballStateTime[(int)ballState] += Time.deltaTime;
			stealInterval -= Time.deltaTime;
		}
		public void Catch(CharacterScript _chara)
		{
			ball.Catch(_chara);
		}
		public void Release(CharacterScript _releaseChara = null)
		{
			ball.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			ball.GetRigid().isKinematic = false;
			SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
			if (_releaseChara != null)
			{
				SingletonCustom<MainCharacterManager>.Instance.HaveBallCharaBallRelease();
			}
			else
			{
				SingletonCustom<MainCharacterManager>.Instance.ResetHaveBallChara();
			}
			SetBallState(BallState.FREE);
		}
		private void Kick(Vector3 kickDir, float _power, float _kickPowerCorr)
		{
			Vector3 vector = kickDir * (passSpeed + shootSpeed * Mathf.Pow(_power, 2f)) * _kickPowerCorr;
			float num = CalcManager.Rot(vector, CalcManager.AXIS.Y);
			vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - num, CalcManager.AXIS.Y);
			if (_power <= PASS_POWER_BORDER)
			{
				_power *= 2f;
				vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - _power * 20f, CalcManager.AXIS.X);
			}
			else
			{
				_power = (_power - 0.5f) * 2f;
				vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - (20f + _power * 45f), CalcManager.AXIS.X);
			}
			vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, num, CalcManager.AXIS.Y);
			ball.GetRigid().AddForce(vector, ForceMode.Impulse);
		}
		public void Kick(float _power, CharacterScript _kickChara, CharacterScript _passAlly)
		{
			ball.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			ball.GetRigid().isKinematic = false;
			Vector3 vector = _kickChara.transform.forward;
			if (_passAlly != null && _power <= PASS_POWER_BORDER * 0.5f)
			{
				Vector3 normalized = (_passAlly.GetPos() - _kickChara.GetPos()).normalized;
				float num = CalcManager.Rot(CalcManager.mVector3Zero, vector, normalized, Vector3.up);
				if (Mathf.Abs(num) >= 30f)
				{
					num *= 0.5f;
				}
				vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, num, CalcManager.AXIS.Y);
			}
			Kick(vector, _power, _kickChara.GetKickPowerCorr());
			if (_power >= PASS_POWER_BORDER && _power <= 0.75f)
			{
				ball.SetAfterEffectColor(ColorPalet.yellow);
			}
			ball.SetLastShootChara(_kickChara);
			if (_passAlly != null)
			{
				SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_kickChara.TeamNo, _kickChara.PlayerNo, _passAlly);
			}
		}
		public void CornerKick(float _power, CharacterScript _kickChara, CharacterScript _passAlly)
		{
			ball.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			ball.GetRigid().isKinematic = false;
			ball.GetCollider().isTrigger = false;
			Kick(_kickChara.transform.forward, _power, 1f);
			ball.SetLastShootChara(_kickChara);
			if (_passAlly != null)
			{
				SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_kickChara.TeamNo, _kickChara.PlayerNo, _passAlly);
			}
		}
		public void GoalKick(float _power, CharacterScript _kickChara, CharacterScript _passAlly)
		{
			ball.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			ball.GetRigid().isKinematic = false;
			ball.GetCollider().isTrigger = false;
			Kick(_kickChara.transform.forward, _power, 1f);
			ball.SetLastShootChara(_kickChara);
			if (_passAlly != null)
			{
				SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_kickChara.TeamNo, _kickChara.PlayerNo, _passAlly);
			}
		}
		public void Heading(float _power, CharacterScript _headingChara, CharacterScript _passAlly)
		{
			float value = CalcManager.Rot(_headingChara.ConvertLocalVec(_headingChara.transform.forward), CalcManager.AXIS.Y);
			if (Mathf.Abs(_headingChara.GetPos().z - SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_headingChara.TeamNo).position.z) <= SingletonCustom<FieldManager>.Instance.GetFieldData().penaltyAreaSize.z + _headingChara.GetCharaBodySize() * 3f && (CalcManager.CheckRange(value, 0f, 125f) || CalcManager.CheckRange(value, 135f, 360f)))
			{
				Vector3 vector = SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_headingChara.TeamNo).position - GetBallPos(_offset: false);
				float num = 5f + Mathf.Max(10f - (float)_headingChara.GetCharaParam().offense, 0f);
				vector = CalcManager.PosRotation2D(vector.normalized, CalcManager.mVector3Zero, Random.Range(0f - num, num), CalcManager.AXIS.Y);
				ball.GetRigid().AddForce(vector * (passSpeed + shootSpeed * _power) + Vector3.down * _headingChara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_headingChara.name + "がヘディング");
			}
			else
			{
				ball.GetRigid().AddForce(_headingChara.transform.forward * (passSpeed + shootSpeed * _power) * 0.5f * _headingChara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_headingChara.name + "がヘディング");
				if (_passAlly != null)
				{
					SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_headingChara.TeamNo, _headingChara.PlayerNo, _passAlly);
				}
			}
		}
		public void DivingHead(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			float value = CalcManager.Rot(_chara.ConvertLocalVec(_chara.transform.forward), CalcManager.AXIS.Y);
			if (Mathf.Abs(_chara.GetPos().z - SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position.z) <= SingletonCustom<FieldManager>.Instance.GetFieldData().penaltyAreaSize.z + _chara.GetCharaBodySize() * 3f && (CalcManager.CheckRange(value, 0f, 125f) || CalcManager.CheckRange(value, 135f, 360f)))
			{
				Vector3 vector = SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position - GetBallPos(_offset: false);
				float num = 5f + Mathf.Max(10f - (float)_chara.GetCharaParam().offense, 0f);
				vector = CalcManager.PosRotation2D(vector.normalized, CalcManager.mVector3Zero, Random.Range(0f - num, num), CalcManager.AXIS.Y);
				ball.GetRigid().AddForce(vector * (passSpeed + shootSpeed * _power) + Vector3.down * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がダイビングヘッド");
			}
			else
			{
				ball.GetRigid().AddForce(_chara.transform.forward * (passSpeed + shootSpeed * _power) * 0.5f * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がダイビングヘッド");
				if (_passAlly != null)
				{
					SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_chara.TeamNo, _chara.PlayerNo, _passAlly);
				}
			}
		}
		public void OverHeadKick(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			float value = CalcManager.Rot(_chara.ConvertLocalVec(_chara.transform.forward), CalcManager.AXIS.Y);
			if (Mathf.Abs(_chara.GetPos().z - SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position.z) <= SingletonCustom<FieldManager>.Instance.GetFieldData().penaltyAreaSize.z + _chara.GetCharaBodySize() * 3f && (CalcManager.CheckRange(value, 0f, 125f) || CalcManager.CheckRange(value, 135f, 360f)))
			{
				Vector3 vector = SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position - GetBallPos(_offset: false);
				float num = 5f + Mathf.Max(10f - (float)_chara.GetCharaParam().offense, 0f);
				vector = CalcManager.PosRotation2D(vector.normalized, CalcManager.mVector3Zero, Random.Range(0f - num, num), CalcManager.AXIS.Y);
				ball.GetRigid().AddForce(vector * (passSpeed + shootSpeed * _power) + Vector3.down * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がオ\u30fcバ\u30fcヘッドキック");
			}
			else
			{
				ball.GetRigid().AddForce(_chara.transform.forward * (passSpeed + shootSpeed * _power) * 0.5f * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がオ\u30fcバ\u30fcヘッドキック");
				if (_passAlly != null)
				{
					SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_chara.TeamNo, _chara.PlayerNo, _passAlly);
				}
			}
		}
		public void JumpingVolley(float _power, CharacterScript _chara, CharacterScript _passAlly)
		{
			float value = CalcManager.Rot(_chara.ConvertLocalVec(_chara.transform.forward), CalcManager.AXIS.Y);
			if (Mathf.Abs(_chara.GetPos().z - SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position.z) <= SingletonCustom<FieldManager>.Instance.GetFieldData().penaltyAreaSize.z + _chara.GetCharaBodySize() * 3f && (CalcManager.CheckRange(value, 0f, 125f) || CalcManager.CheckRange(value, 135f, 360f)))
			{
				Vector3 vector = SingletonCustom<FieldManager>.Instance.GetOpponentGoal(_chara.TeamNo).position - GetBallPos(_offset: false);
				float num = 5f + Mathf.Max(10f - (float)_chara.GetCharaParam().offense, 0f);
				vector = CalcManager.PosRotation2D(vector.normalized, CalcManager.mVector3Zero, Random.Range(0f - num, num), CalcManager.AXIS.Y);
				ball.GetRigid().AddForce(vector * (passSpeed + shootSpeed * _power) + Vector3.down * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がジャンピングボレ\u30fc");
			}
			else
			{
				ball.GetRigid().AddForce(_chara.transform.forward * (passSpeed + shootSpeed * _power) * 0.5f * _chara.GetKickPowerCorr(), ForceMode.Impulse);
				UnityEngine.Debug.Log(_chara.name + "がジャンピングボレ\u30fc");
				if (_passAlly != null)
				{
					SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_chara.TeamNo, _chara.PlayerNo, _passAlly);
				}
			}
		}
		public void ThrowIn(float _power, CharacterScript _throwInChara, CharacterScript _passAlly)
		{
			ball.transform.parent = SingletonCustom<FieldManager>.Instance.GetObjAnchor();
			ball.GetRigid().isKinematic = false;
			ball.GetCollider().isTrigger = false;
			ball.GetRigid().AddForce(_throwInChara.transform.forward * (1f + passSpeed + shootSpeed * _power) * 0.15f + Vector3.up * (0.5f + 0.5f * _power), ForceMode.Impulse);
			if (_passAlly != null)
			{
				SingletonCustom<MainCharacterManager>.Instance.ChangeControlChara(_throwInChara.TeamNo, _throwInChara.PlayerNo, _passAlly);
			}
		}
		public void SetBallState(BallState _state)
		{
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
		public BallScript GetBall()
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
		public Vector3 GetBallPos(bool _offset = true, bool _local = false)
		{
			if (_local)
			{
				ballPosOffset = ball.transform.localPosition;
				if (_offset)
				{
					ballPosOffset.y = 0f;
				}
				return ballPosOffset;
			}
			ballPosOffset = ball.transform.position;
			if (_offset)
			{
				ballPosOffset.y = SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position.y;
			}
			return ballPosOffset;
		}
		public float GetBallDistance(Vector3 _pos)
		{
			return CalcManager.Length(GetBallPos(), _pos);
		}
		public float GetBallSize()
		{
			return ball.GetCollider().radius;
		}
		public BallState GetBallState()
		{
			return ballState;
		}
		public float GetStateTime(BallState _state = BallState.MAX)
		{
			if (_state == BallState.MAX)
			{
				_state = ballState;
			}
			return ballStateTime[(int)_state];
		}
		public bool CheckBallState(BallState _state)
		{
			return ballState == _state;
		}
		public bool CheckMoveBall()
		{
			if (ballState != 0)
			{
				return ballState == BallState.KEEP;
			}
			return true;
		}
		public bool CheckStealInterval()
		{
			return stealInterval <= 0f;
		}
	}
}
