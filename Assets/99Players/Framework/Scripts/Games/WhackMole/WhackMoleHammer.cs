using System;
using UnityEngine;
public class WhackMoleHammer : MonoBehaviour
{
	private const float HAMMER_MOVE_SPEED = 30f;
	private const float HAMMER_STOP_DISTANCE = 0.01f;
	private const float DEFAULT_ANGLE_Z = 45f;
	private const float WHACK_ANGLE_Z = 90f;
	private const float WHACK_ANIMATION_TIME = 0.12f;
	[SerializeField]
	private Transform hammerAnchor;
	private bool isAnimation;
	private bool isHit;
	private Vector3 targetPos;
	private Action callback;
	public bool IsAnimation => isAnimation;
	public void Init()
	{
		hammerAnchor.SetLocalEulerAnglesZ(45f);
		isAnimation = false;
		targetPos = hammerAnchor.position;
		LeanTween.cancel(base.gameObject);
	}
	public void UpdateMethod()
	{
		Vector3 vector = targetPos - hammerAnchor.position;
		if (vector.sqrMagnitude < 0.0001f)
		{
			hammerAnchor.position = targetPos;
			return;
		}
		hammerAnchor.position += vector * 30f * Time.deltaTime;
		Vector3 rhs = targetPos - hammerAnchor.position;
		if (Vector3.Dot(vector, rhs) < 0f)
		{
			hammerAnchor.position = targetPos;
		}
	}
	private void OnTriggerEnter(Collider _col)
	{
		if (_col.gameObject.tag == "Object" && !isHit)
		{
			isHit = true;
			if (callback != null)
			{
				callback();
			}
		}
	}
	public void SetPos(Vector3 _pos, bool _immediate)
	{
		targetPos = _pos;
		if (_immediate)
		{
			hammerAnchor.position = _pos;
		}
	}
	public void AnimationStart(Action _callback)
	{
		callback = _callback;
		isHit = false;
		isAnimation = true;
		DownAnimation();
	}
	private void DownAnimation()
	{
		LeanTween.value(base.gameObject, 0f, 1f, 0.12f).setOnUpdate(delegate(float _value)
		{
			hammerAnchor.SetLocalEulerAnglesZ(Mathf.Lerp(45f, 90f, _value));
		}).setOnComplete((Action)delegate
		{
			UpAnimation();
		});
	}
	private void UpAnimation()
	{
		float angleZ = hammerAnchor.localEulerAngles.z;
		float time = (angleZ - 45f) / 45f * 0.12f;
		LeanTween.value(base.gameObject, 0f, 1f, time).setOnUpdate(delegate(float _value)
		{
			hammerAnchor.SetLocalEulerAnglesZ(Mathf.Lerp(angleZ, 45f, _value));
		}).setOnComplete((Action)delegate
		{
			hammerAnchor.SetLocalEulerAnglesZ(45f);
			isAnimation = false;
		});
	}
}
