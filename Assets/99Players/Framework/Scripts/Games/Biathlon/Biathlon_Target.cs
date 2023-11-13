using System.Collections;
using UnityEngine;
public class Biathlon_Target : MonoBehaviour
{
	[SerializeField]
	private Collider[] colliders;
	[SerializeField]
	private Transform[] guards;
	[SerializeField]
	private Transform relayPoint;
	[SerializeField]
	private Transform standingPoint;
	[SerializeField]
	private Transform escapePoint;
	[SerializeField]
	private Transform aimSupport;
	[SerializeField]
	private int number;
	public Transform AimSupport => aimSupport;
	public bool IsUsing
	{
		get;
		set;
	}
	public bool IsPrepare
	{
		get;
		set;
	}
	public int Number => number;
	public void Init()
	{
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}
	public void Activate()
	{
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		Transform[] array2 = guards;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		}
	}
	public bool TryBreak(Transform hitTarget)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			Collider collider = colliders[i];
			if (collider.transform == hitTarget)
			{
				collider.enabled = false;
				StartCoroutine(PullUpGuard(guards[i]));
				return true;
			}
		}
		return false;
	}
	private IEnumerator PullUpGuard(Transform guard)
	{
		float elapsed = 0f;
		while (elapsed < 0.3f)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / 0.3f;
			Quaternion a = Quaternion.Euler(0f, 0f, 90f);
			Quaternion identity = Quaternion.identity;
			guard.localRotation = Quaternion.Lerp(a, identity, t);
			yield return null;
		}
		guard.localRotation = Quaternion.identity;
	}
	public Vector3 GetRelayPoint()
	{
		return relayPoint.position;
	}
	public Vector3 GetStandingPoint()
	{
		return standingPoint.position;
	}
	public Vector3 GetEscapePoint()
	{
		return escapePoint.position;
	}
	public Collider GetHitCollider(int index)
	{
		return colliders[index];
	}
}
