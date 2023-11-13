using System.Collections.Generic;
using UnityEngine;
public class ShavedIce_Cup : MonoBehaviour
{
	private ShavedIce_Player player;
	private ShavedIce_IceObjectManager.CreateVecData nearIce;
	private List<ParticleCollisionEvent> colEvList = new List<ParticleCollisionEvent>();
	private ParticleSystem particle;
	private bool isStartHitParticle;
	public bool IsStartHitParticle => isStartHitParticle;
	public void Init(ShavedIce_Player _player)
	{
		player = _player;
	}
	private void CreateIce(Vector3 hitPos)
	{
		isStartHitParticle = true;
		nearIce = player.IOM.GetNearFirstIceObject(0, hitPos);
		if (nearIce.createIce == null)
		{
			player.IOM.CreateIceObject(0, nearIce.pointNo);
		}
		else if (!nearIce.createIce.IsFormIce)
		{
			nearIce.createIce.PileIce();
		}
	}
	private void OnParticleCollision(GameObject other)
	{
		if (particle == null)
		{
			particle = other.GetComponent<ParticleSystem>();
		}
		particle.GetCollisionEvents(base.gameObject, colEvList);
		foreach (ParticleCollisionEvent colEv in colEvList)
		{
			CreateIce(colEv.intersection);
		}
	}
}
