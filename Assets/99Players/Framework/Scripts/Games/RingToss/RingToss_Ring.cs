using System;
using UnityEngine;
public class RingToss_Ring : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] renderers;
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	private Transform[] ringCheckPoints;
	private RingToss_Target nearestTarget;
	private int ctrlNo;
	private bool isThrow;
	private float throwTime;
	private float stopTime;
	private bool isFixTable;
	private bool isTrainParent;
	private Transform initParent;
	private Vector3 initPos;
	private Quaternion initRot;
	private Action callback;
	public int CtrlNo => ctrlNo;
	public void Init(int _ctrlNo)
	{
		ctrlNo = _ctrlNo;
		rigid.isKinematic = true;
		isThrow = false;
		initParent = base.transform.parent;
		initPos = base.transform.position;
		initRot = base.transform.rotation;
	}
	public void SecondGroupInit()
	{
		rigid.isKinematic = true;
		isThrow = false;
		throwTime = 0f;
		stopTime = 0f;
		isFixTable = false;
		isTrainParent = false;
		base.gameObject.SetActive(value: true);
		base.transform.parent = initParent;
		base.transform.position = initPos;
		base.transform.rotation = initRot;
	}
	public void UpdateMethod()
	{
		if (!isThrow)
		{
			return;
		}
		if (CheckStop())
		{
			if (stopTime == 0f)
			{
				UpdateNearestTarget();
			}
			stopTime += Time.deltaTime;
			if (stopTime > 0.5f)
			{
				if (CheckInTarget())
				{
					nearestTarget.TargetGet(ctrlNo);
				}
				stopTime = 0f;
				isThrow = false;
				rigid.isKinematic = true;
				callback();
				Hide();
			}
		}
		else
		{
			stopTime = 0f;
			if (throwTime > 5f)
			{
				isThrow = false;
				throwTime = 0f;
				rigid.isKinematic = true;
				callback();
				Hide();
			}
		}
		throwTime += Time.deltaTime;
	}
	public void FixedUpdateMethod()
	{
		rigid.AddForce(Vector3.up * -98.1f, ForceMode.Acceleration);
	}
	private void UpdateNearestTarget()
	{
		RingToss_Target y = SingletonCustom<RingToss_TargetManager>.Instance.SearchNearestTarget(base.transform.position);
		if (nearestTarget != y)
		{
			nearestTarget = y;
			stopTime = 0f;
		}
	}
	public void Throw(Vector3 _vec, Action _callback = null)
	{
		rigid.isKinematic = false;
		rigid.velocity = _vec;
		isThrow = true;
		callback = _callback;
	}
	public bool CheckStop()
	{
		if (isTrainParent)
		{
			return true;
		}
		return rigid.velocity.sqrMagnitude < 0.0100000007f;
	}
	public bool CheckInTarget()
	{
		if (nearestTarget == null)
		{
			return false;
		}
		if (nearestTarget.IsGet)
		{
			return false;
		}
		if (isFixTable && nearestTarget.IsSpecialObject)
		{
			return false;
		}
		for (int i = 0; i < ringCheckPoints.Length; i++)
		{
			Vector3 lhs = base.transform.position - ringCheckPoints[i].position;
			lhs.y = 0f;
			Vector3 rhs = nearestTarget.GetPos() - ringCheckPoints[i].position;
			rhs.y = 0f;
			if (Vector3.Dot(lhs, rhs) < 0f)
			{
				return false;
			}
		}
		float num = Vector3.Angle(base.transform.up, Vector3.up);
		if (45f < num && num < 135f)
		{
			return false;
		}
		return true;
	}
	private bool CheckFixTable()
	{
		float num = Vector3.Angle(base.transform.up, Vector3.up);
		if (2f < num && num < 178f)
		{
			return false;
		}
		return true;
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	public void SetMaterial(Material _mat)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sharedMaterial = _mat;
		}
	}
	private void OnCollisionStay(Collision _col)
	{
		if (_col.gameObject.tag == "School")
		{
			if (!isFixTable && CheckFixTable())
			{
				isFixTable = true;
			}
		}
		else
		{
			if (!(_col.gameObject.tag == "Airplane"))
			{
				return;
			}
			if (isFixTable)
			{
				rigid.isKinematic = true;
			}
			else
			{
				if (isTrainParent)
				{
					return;
				}
				for (int i = 0; i < ringCheckPoints.Length; i++)
				{
					Vector3 lhs = base.transform.position - ringCheckPoints[i].position;
					lhs.y = 0f;
					Vector3 rhs = _col.transform.position - ringCheckPoints[i].position;
					rhs.y = 0f;
					if (Vector3.Dot(lhs, rhs) < 0f)
					{
						return;
					}
				}
				base.transform.parent = _col.transform.parent;
				isTrainParent = true;
			}
		}
	}
}
