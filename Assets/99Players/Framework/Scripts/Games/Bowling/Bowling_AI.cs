using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bowling_AI : MonoBehaviour
{
	public struct ValueLimit<T>
	{
		public T min;
		public T max;
		public ValueLimit(T _min, T _max)
		{
			min = _min;
			max = _max;
		}
	}
	public struct RandomSeed<T>
	{
		public T minValue;
		public T maxValue;
		public RandomSeed(T _value1, T _value2, bool _mirror = false)
		{
			if (_mirror)
			{
				if (typeof(T) == typeof(int))
				{
					minValue = (T)Convert.ChangeType((int)Convert.ChangeType(_value1, typeof(int)) - (int)Convert.ChangeType(_value2, typeof(int)), typeof(T));
					maxValue = (T)Convert.ChangeType((int)Convert.ChangeType(_value1, typeof(int)) + (int)Convert.ChangeType(_value2, typeof(int)), typeof(T));
				}
				else if (typeof(T) == typeof(float))
				{
					minValue = (T)Convert.ChangeType((float)Convert.ChangeType(_value1, typeof(float)) - (float)Convert.ChangeType(_value2, typeof(float)), typeof(T));
					maxValue = (T)Convert.ChangeType((float)Convert.ChangeType(_value1, typeof(float)) + (float)Convert.ChangeType(_value2, typeof(float)), typeof(T));
				}
				else
				{
					minValue = _value1;
					maxValue = _value2;
				}
			}
			else
			{
				minValue = _value1;
				maxValue = _value2;
			}
		}
	}
	public struct StrengthParams
	{
		public int straightPer;
		public int straightThrowCenterPer;
		public int straightPerWhenFirstThrow;
		public int straightPerWhenOnePin;
		public RandomSeed<float> centerThrowRange;
		public int missThrowPer;
		public float missThrowRot;
		public float throwErrorRot;
		public RandomSeed<float> throwPowerRange;
		public StrengthParams(int _straightPer, int _straightThrowCenterPer, int _straightPerWhenFirstThrow, int _straightPerWhenOnePin, RandomSeed<float> _centerThrowRange, int _missThrowPer, float _missThrowRot, float _throwErrorRot, RandomSeed<float> _throwPowerRange)
		{
			straightPer = _straightPer;
			straightThrowCenterPer = _straightThrowCenterPer;
			straightPerWhenFirstThrow = _straightPerWhenFirstThrow;
			straightPerWhenOnePin = _straightPerWhenOnePin;
			centerThrowRange = _centerThrowRange;
			missThrowPer = _missThrowPer;
			missThrowRot = _missThrowRot;
			throwErrorRot = _throwErrorRot;
			throwPowerRange = _throwPowerRange;
		}
		public void Copy(StrengthParams _data)
		{
			straightPer = _data.straightPer;
			straightThrowCenterPer = _data.straightThrowCenterPer;
			straightPerWhenFirstThrow = _data.straightPerWhenFirstThrow;
			straightPerWhenOnePin = _data.straightPerWhenOnePin;
			centerThrowRange = _data.centerThrowRange;
			missThrowPer = _data.missThrowPer;
			missThrowRot = _data.missThrowRot;
			throwErrorRot = _data.throwErrorRot;
			throwPowerRange = _data.throwPowerRange;
		}
	}
	private const float MISS_THROW_ROT_MAX = 1f;
	private StrengthParams[] strengthParamsList = new StrengthParams[3]
	{
		new StrengthParams(80, 10, 95, 80, new RandomSeed<float>(0f, 2f), 100, 5f, 3f, new RandomSeed<float>(0.75f, 0.85f)),
		new StrengthParams(60, 40, 70, 20, new RandomSeed<float>(0f, 1f), 70, 3f, 1.5f, new RandomSeed<float>(0.85f, 1f)),
		new StrengthParams(30, 70, 80, 20, new RandomSeed<float>(0.4f, 0.6f), 5, 0.025f, 0f, new RandomSeed<float>(1f, 1f))
	};
	private StrengthParams strengthParams;
	private Bowling_Define.AiStrength aiStrength = Bowling_Define.AiStrength.COMMON;
	private List<Vector3> targetPosGizmos = new List<Vector3>();
	private Bowling_Player.ThrowData throwData;
	private Bowling_Player player;
	private bool isSkipMode;
	private float skipThrowVec;
	public bool IsSkipMode
	{
		get
		{
			return isSkipMode;
		}
		set
		{
			isSkipMode = value;
		}
	}
	public float RandomValue(RandomSeed<float> _seed)
	{
		return UnityEngine.Random.Range(_seed.minValue, _seed.maxValue);
	}
	public int RandomValue(RandomSeed<int> _seed)
	{
		return UnityEngine.Random.Range(_seed.minValue, _seed.maxValue);
	}
	private RandomSeed<float> RandomValue(RandomSeed<float> _seed, float _mag)
	{
		_seed.minValue *= _mag;
		_seed.maxValue *= _mag;
		return _seed;
	}
	private RandomSeed<int> RandomValue(RandomSeed<int> _seed, float _mag)
	{
		_seed.minValue = (int)((float)_seed.minValue * _mag);
		_seed.maxValue = (int)((float)_seed.maxValue * _mag);
		return _seed;
	}
	private int LimitValue(int _value, int _min, int _max)
	{
		return Mathf.Min(Mathf.Max(_value, _min), _max);
	}
	private float LimitValue(float _value, float _min, float _max)
	{
		return Mathf.Min(Mathf.Max(_value, _min), _max);
	}
	public void Init()
	{
		player = GetComponent<Bowling_Player>();
		aiStrength = (Bowling_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		strengthParams.Copy(strengthParamsList[(int)aiStrength]);
		isSkipMode = false;
	}
	public void Throw()
	{
		isSkipMode = false;
		StartCoroutine(_Throw());
	}
	private IEnumerator _Throw()
	{
		targetPosGizmos.Clear();
		yield return new WaitForEndOfFrame();
		Bowling_Pin bowling_Pin = null;
		for (int i = 0; i < Bowling_Define.MSM.PinList.Length; i++)
		{
			if (!Bowling_Define.MSM.PinList[i].IsFall)
			{
				bowling_Pin = Bowling_Define.MSM.PinList[i];
				break;
			}
		}
		this.throwData.touchLengthPer = RandomValue(strengthParams.throwPowerRange);
		UnityEngine.Debug.Log("<color=cyan>投球パワ\u30fc：</color>" + this.throwData.touchLengthPer.ToString());
		float throwOffset4;
		if ((Bowling_Define.MSM.GetPinFallNum() == 0 && CalcManager.IsPerCheck(strengthParams.straightPerWhenFirstThrow)) || CalcManager.IsPerCheck(strengthParams.straightPer))
		{
			this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
			throwOffset4 = SettingStraightThrow(bowling_Pin);
		}
		else
		{
			if ((bowling_Pin.gameObject.name.IndexOf("0") >= 0) ? CalcManager.IsPerCheck(50) : (bowling_Pin.transform.localPosition.x > 0f))
			{
				if (CalcManager.IsPerCheck(50))
				{
					if (player.GetBall().GetParam(Bowling_Define.ThrowType.RIGHT_S) != 0)
					{
						this.throwData.throwType = Bowling_Define.ThrowType.RIGHT_S;
					}
					else
					{
						this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
					}
				}
				else if (player.GetBall().GetParam(Bowling_Define.ThrowType.RIGHT_S) != 0)
				{
					this.throwData.throwType = Bowling_Define.ThrowType.RIGHT_S;
				}
				else
				{
					this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
				}
			}
			else if (CalcManager.IsPerCheck(50))
			{
				if (player.GetBall().GetParam(Bowling_Define.ThrowType.LEFT_S) != 0)
				{
					this.throwData.throwType = Bowling_Define.ThrowType.LEFT_S;
				}
				else
				{
					this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
				}
			}
			else if (player.GetBall().GetParam(Bowling_Define.ThrowType.LEFT_S) != 0)
			{
				this.throwData.throwType = Bowling_Define.ThrowType.LEFT_S;
			}
			else
			{
				this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
			}
			if (this.throwData.throwType == Bowling_Define.ThrowType.STRAIGHT)
			{
				throwOffset4 = SettingStraightThrow(bowling_Pin);
			}
			else
			{
				float num = bowling_Pin.transform.position.z - player.GetBall().GetBallPos().z;
				Vector3 position = bowling_Pin.transform.position;
				targetPosGizmos.Add(position);
				this.throwData.throwVec = Vector3.forward;
				this.throwData.throwVec.y = 0f;
				Bowling_Ball.ThrowData throwData = player.GetBall().CalcThrowData(this.throwData.throwVec, this.throwData.touchLengthPer, this.throwData.throwType);
				throwOffset4 = num * (0f - throwData.angularVelocity.z) / Mathf.Pow(throwData.velocity.magnitude, 2f) * 0.02975f;
				float num2 = Bowling_Define.MSM.LaneEndPos.z - position.z;
				num2 = 1f + Mathf.Min(0.927f - num2, 1f) / 0.927f * 0.9f;
				throwOffset4 *= num2;
				float pinRadius = Bowling_Define.MSM.PinList[0].PinRadius;
				throwOffset4 = bowling_Pin.transform.localPosition.x - throwOffset4;
				if ((bowling_Pin.gameObject.name.IndexOf("0") >= 0) ? CalcManager.IsPerCheck(50) : (bowling_Pin.transform.localPosition.x > 0f))
				{
					throwOffset4 -= pinRadius;
					targetPosGizmos[0] = -targetPosGizmos[0] + new Vector3(pinRadius, 0f, 0f);
				}
				else
				{
					throwOffset4 += pinRadius;
					List<Vector3> list = targetPosGizmos;
					list[0] += targetPosGizmos[0] + new Vector3(0f - pinRadius, 0f, 0f);
				}
				if (Mathf.Abs(throwOffset4) > Bowling_Define.MSM.GetApproachSize.x * 0.45f)
				{
					this.throwData.throwType = Bowling_Define.ThrowType.STRAIGHT;
					throwOffset4 = SettingStraightThrow(bowling_Pin);
				}
			}
		}
		if (CalcManager.IsPerCheck(strengthParams.missThrowPer))
		{
			this.throwData.throwVec = CalcManager.PosRotation2D(this.throwData.throwVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(0f - strengthParams.missThrowRot, strengthParams.missThrowRot), CalcManager.AXIS.Y);
		}
		else
		{
			this.throwData.throwVec = CalcManager.PosRotation2D(this.throwData.throwVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(0f - strengthParams.throwErrorRot, strengthParams.throwErrorRot), CalcManager.AXIS.Y);
		}
		float moveTime = 0f;
		float moveSpeed = UnityEngine.Random.Range(2f, 2.5f);
		player.OperationState = Bowling_Define.OperationState.BALL_MOVE;
		skipThrowVec = throwOffset4;
		yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1f));
		while (!isSkipMode)
		{
			moveTime += Time.deltaTime * moveSpeed;
			if (moveTime >= 1f)
			{
				moveTime = 1f;
			}
			else
			{
				yield return null;
			}
			player.GetBall().transform.SetLocalPositionX(throwOffset4 * moveTime);
			Bowling_Define.MSM.SetStageCameraPosX(player.GetBall().GetBallPos().x);
			if (!(moveTime < 1f))
			{
				break;
			}
		}
		if (!isSkipMode)
		{
			player.OperationState = Bowling_Define.OperationState.VECTOR_SELECT;
			player.GetBall().transform.SetLocalPositionX(throwOffset4);
			Bowling_Define.MSM.SetStageCameraPosX(player.GetBall().GetBallPos().x);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.5f));
			player.BallThrow(this.throwData.throwVec, this.throwData.touchLengthPer, this.throwData.throwType);
		}
		else
		{
			player.OperationState = Bowling_Define.OperationState.VECTOR_SELECT;
		}
	}
	private float SettingStraightThrow(Bowling_Pin _targetPin)
	{
		float num = 0f;
		if (Bowling_Define.MSM.GetPinFallNum() == 0)
		{
			num = Bowling_Define.MSM.PinList[0].PinRadius * RandomValue(strengthParams.centerThrowRange) * CalcManager.RandomPlusOrMinus();
			throwData.throwVec = Vector3.forward;
			throwData.throwVec.y = 0f;
		}
		else
		{
			num = ((!CalcManager.IsPerCheck(strengthParams.straightThrowCenterPer)) ? UnityEngine.Random.Range((0f - Bowling_Define.MSM.GetApproachSize.x) * 0.4f, Bowling_Define.MSM.GetApproachSize.x * 0.4f) : 0f);
			Vector3 ballPos = player.GetBall().GetBallPos();
			ballPos.x += num;
			throwData.throwVec = (_targetPin.transform.position - ballPos).normalized;
			throwData.throwVec.y = 0f;
		}
		targetPosGizmos.Add(_targetPin.transform.position);
		return num;
	}
	public void SkipThrow()
	{
		player.GetBall().transform.SetLocalPositionX(skipThrowVec);
		Bowling_Define.MSM.SetStageCameraPosX(player.GetBall().GetBallPos().x);
		player.BallThrow(throwData.throwVec, throwData.touchLengthPer, throwData.throwType);
		Time.timeScale = 20f;
		LeanTween.delayedCall(0.5f, (Action)delegate
		{
			Time.timeScale = 1f;
		});
	}
	private void OnDrawGizmos()
	{
		for (int i = 0; i < targetPosGizmos.Count; i++)
		{
			switch (i)
			{
			case 0:
				Gizmos.color = ColorPalet.yellow;
				break;
			case 1:
				Gizmos.color = ColorPalet.green;
				break;
			}
			Gizmos.DrawWireSphere(targetPosGizmos[i], 0.1f);
		}
	}
}
