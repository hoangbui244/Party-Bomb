using System;
using UnityEngine;
public class MonsterRace_Barrel : MonoBehaviour
{
	[SerializeField]
	[Header("破片エフェクト")]
	private ParticleSystem breakEffect;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private Collider collider;
	[SerializeField]
	[Header("メッシュレンダラ\u30fc")]
	private MeshRenderer meshRenderer;
	private bool active;
	private Quaternion startRot;
	public bool Active => active;
	private void Awake()
	{
		startRot = base.transform.rotation;
		SetViewActive(_active: false);
	}
	public void SetViewActive(bool _active)
	{
		active = _active;
		rigid.isKinematic = !_active;
		collider.enabled = _active;
		meshRenderer.enabled = _active;
	}
	public void ColliderDelayEnable(float _delayTime)
	{
		collider.enabled = false;
		LeanTween.delayedCall(base.gameObject, _delayTime, (Action)delegate
		{
			collider.enabled = true;
		});
	}
	public void PlayBreakEffect()
	{
		breakEffect.Play();
	}
	public void AddForce(Vector3 _vec, ForceMode _forceMode)
	{
		rigid.AddForce(_vec, _forceMode);
	}
	public void ResetRotation()
	{
		base.transform.rotation = startRot;
	}
	public void ResetVelocity()
	{
		rigid.velocity = Vector3.zero;
	}
	private void OnCollisionEnter(Collision _col)
	{
		if (_col.gameObject.tag == "Player" || _col.gameObject.tag == "Respawn")
		{
			ResetRotation();
			PlayBreakEffect();
			SetViewActive(_active: false);
		}
	}
	private void OnTriggerEnter(Collider _col)
	{
		if (_col.gameObject.tag == "Respawn")
		{
			ResetRotation();
			PlayBreakEffect();
			SetViewActive(_active: false);
		}
	}
}
