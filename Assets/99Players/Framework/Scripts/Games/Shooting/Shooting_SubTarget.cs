using UnityEngine;
public class Shooting_SubTarget : MonoBehaviour, Shooting_IHitCallable
{
	[SerializeField]
	private Shooting_Target mainTarget;
	private float mass;
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	[Header("HitPoint情報")]
	private Shooting_HitPointAnchor hitPointAnchor;
	private Vector3 initPos;
	private Quaternion initRot;
	public void Init()
	{
		initPos = base.transform.position;
		initRot = base.transform.rotation;
		hitPointAnchor.Init();
	}
	public void SecondGroupInit()
	{
		base.transform.position = initPos;
		base.transform.rotation = initRot;
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
	}
	public void SetMass(float _mass)
	{
		mass = _mass;
		rigid.mass = mass;
	}
	public void CallHit(int _gunNo, Vector3 _vec, Vector3 _pos)
	{
		if (mainTarget != null)
		{
			mainTarget.CallHit(_gunNo, _vec, _pos);
		}
		else
		{
			Hit(_vec, _pos);
		}
	}
	public void Hit(Vector3 _vec, Vector3 _pos)
	{
		float num = 1000f;
		int num2 = 0;
		for (int i = 0; i < hitPointAnchor.ArrayHitPointAnchor.Length; i++)
		{
			float num3 = CalcManager.Length(hitPointAnchor.ArrayHitPointAnchor[i].transform.position, _pos);
			UnityEngine.Debug.Log("len " + num3.ToString());
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		UnityEngine.Debug.Log("nearIdx " + num2.ToString());
		UnityEngine.Debug.Log("hitPointAnchor.ArrayHitPointAnchor[nearIdx].HitPointType : " + hitPointAnchor.ArrayHitPointAnchor[num2].HitPointType.ToString());
		float num4 = 2f;
		UnityEngine.Debug.Log("power 調整前 : " + num4.ToString());
		switch (hitPointAnchor.ArrayHitPointAnchor[num2].HitPointType)
		{
		case Shooting_TargetManager.HitPointType.Weak:
			UnityEngine.Debug.Log("Shooting_Define.WEAK_HIT_POWER : " + 1f.ToString());
			num4 *= 1f;
			break;
		case Shooting_TargetManager.HitPointType.Normal:
			UnityEngine.Debug.Log("Shooting_Define.NORMAL_HIT_POWER : " + 0.6f.ToString());
			num4 *= 0.6f;
			break;
		case Shooting_TargetManager.HitPointType.Bad:
			UnityEngine.Debug.Log("Shooting_Define.BAD_HIT_POWER : " + 0.2f.ToString());
			num4 *= 0.2f;
			break;
		}
		UnityEngine.Debug.Log("power 調整後 : " + num4.ToString());
		rigid.AddForce(_vec * num4, ForceMode.Impulse);
	}
}
