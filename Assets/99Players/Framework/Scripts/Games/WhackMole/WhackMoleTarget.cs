using System;
using UnityEngine;
public class WhackMoleTarget : MonoBehaviour
{
	private enum State
	{
		Stay,
		Up,
		Down,
		WhackAnimation,
		Hide
	}
	private const float MOVE_UP_VALUE_Y = 2f;
	private const float WHACK_ANIMATION_TIME = 0.2f;
	private const float WHACK_ANIM_DIRECTION_TIME = 1f;
	private WhackMoleTargetManager.MoleData data;
	private State state;
	[SerializeField]
	private GameObject usualFaceObj;
	[SerializeField]
	private GameObject whackedFaceObj;
	[SerializeField]
	private Transform leftHandAnchor;
	[SerializeField]
	private Transform rightHandAnchor;
	[SerializeField]
	private Transform pointUiAnchor;
	[SerializeField]
	private ParticleSystem hitEffect;
	private bool isWhacked;
	private Vector3 initLocalPos;
	private Vector3 initLeftHandLocalPos;
	private Vector3 initRightHandLocalPos;
	private float stayTime;
	private float whackAnimTime;
	public bool IsWhacked => isWhacked;
	public bool IsCanAiTarget
	{
		get
		{
			if (state != State.Up)
			{
				return state == State.Stay;
			}
			return true;
		}
	}
	public void Init()
	{
		initLocalPos = base.transform.localPosition;
		if (leftHandAnchor != null)
		{
			initLeftHandLocalPos = leftHandAnchor.localPosition;
			initRightHandLocalPos = rightHandAnchor.localPosition;
		}
		base.gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		switch (state)
		{
		case State.Hide:
			break;
		case State.Stay:
			StayTimer();
			break;
		case State.Up:
			MoveUp();
			break;
		case State.Down:
			MoveDown();
			break;
		case State.WhackAnimation:
			WhackAnimDirection();
			break;
		}
	}
	public void Show(WhackMoleTargetManager.MoleData _data)
	{
		ResetHitInfo();
		data = _data;
		base.gameObject.SetActive(value: true);
		state = State.Up;
	}
	public bool CheckCanWhack()
	{
		if (!isWhacked)
		{
			return base.gameObject.activeSelf;
		}
		return false;
	}
	public void Whack()
	{
		isWhacked = true;
		state = State.WhackAnimation;
		WhackAnimStart();
		usualFaceObj.SetActive(value: false);
		whackedFaceObj.SetActive(value: true);
	}
	private void ResetHitInfo()
	{
		isWhacked = false;
		stayTime = 0f;
		whackAnimTime = 0f;
		usualFaceObj.SetActive(value: true);
		whackedFaceObj.SetActive(value: false);
	}
	private void StayTimer()
	{
		stayTime += Time.deltaTime;
		if (stayTime > data.stayTime)
		{
			state = State.Down;
		}
	}
	private void MoveUp()
	{
		CalcManager.mCalcVector3 = base.transform.localPosition;
		CalcManager.mCalcVector3.y += data.moveSpeed * Time.deltaTime;
		if (CalcManager.mCalcVector3.y > initLocalPos.y + 2f)
		{
			CalcManager.mCalcVector3.y = initLocalPos.y + 2f;
			state = State.Stay;
		}
		base.transform.localPosition = CalcManager.mCalcVector3;
		MoveHandPos();
	}
	private void MoveDown()
	{
		CalcManager.mCalcVector3 = base.transform.localPosition;
		CalcManager.mCalcVector3.y -= data.moveSpeed * Time.deltaTime;
		if (CalcManager.mCalcVector3.y < initLocalPos.y)
		{
			CalcManager.mCalcVector3.y = initLocalPos.y;
			base.gameObject.SetActive(value: false);
			state = State.Hide;
		}
		base.transform.localPosition = CalcManager.mCalcVector3;
		MoveHandPos();
	}
	private void MoveHandPos()
	{
		if (!(leftHandAnchor == null))
		{
			float num = 0.15f;
			float num2 = 0.25f;
			float num3 = 0.8f;
			float moleTopPosY = SingletonCustom<WhackMoleTargetManager>.Instance.GetMoleTopPosY();
			float num4 = base.transform.position.y + num3;
			if (num4 > moleTopPosY)
			{
				float t = (num4 - moleTopPosY) / num3;
				leftHandAnchor.localPosition = new Vector3(Mathf.Lerp(initLeftHandLocalPos.x + num, initLeftHandLocalPos.x, t), 0f, Mathf.Lerp(initLeftHandLocalPos.z - num2, initLeftHandLocalPos.z, t));
				rightHandAnchor.localPosition = new Vector3(Mathf.Lerp(initRightHandLocalPos.x - num, initRightHandLocalPos.x, t), 0f, Mathf.Lerp(initRightHandLocalPos.z - num2, initRightHandLocalPos.z, t));
				leftHandAnchor.SetPositionY(moleTopPosY);
				rightHandAnchor.SetPositionY(moleTopPosY);
			}
			else
			{
				leftHandAnchor.localPosition = new Vector3(initLeftHandLocalPos.x + num, 0f, initLeftHandLocalPos.z - num2);
				rightHandAnchor.localPosition = new Vector3(initRightHandLocalPos.x - num, 0f, initRightHandLocalPos.z - num2);
				leftHandAnchor.SetPositionY(num4);
				rightHandAnchor.SetPositionY(num4);
			}
		}
	}
	private void WhackAnimStart()
	{
		LeanTween.delayedCall(0.2f, (Action)delegate
		{
			hitEffect.Play();
		});
		LeanTween.value(base.gameObject, 0f, 1f, 0.2f).setOnUpdate(delegate(float _value)
		{
			base.transform.SetLocalScaleY(1f - Mathf.Sin(_value * (float)Math.PI) * 0.3f);
		}).setOnComplete((Action)delegate
		{
			base.transform.SetLocalScaleY(1f);
		});
	}
	private void WhackAnimDirection()
	{
		whackAnimTime += Time.deltaTime;
		if (whackAnimTime > 1f)
		{
			state = State.Down;
		}
	}
	public Vector3 GetPointUiPos()
	{
		return pointUiAnchor.position;
	}
	public void SetHandActive(bool _active)
	{
		leftHandAnchor.gameObject.SetActive(_active);
		rightHandAnchor.gameObject.SetActive(_active);
	}
}
