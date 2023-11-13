using System.Collections.Generic;
using UnityEngine;
public class ShavedIce_DeskCollision : MonoBehaviour
{
	[SerializeField]
	[Header("かき氷機の降ってくる氷のエフェクト")]
	private ParticleSystem iceMachineFX;
	[SerializeField]
	[Header("氷の生成時のエフェクトのプレハブ")]
	private GameObject iceSpawnFXPrefab;
	[SerializeField]
	[Header("カメラオブジェクト")]
	private GameObject cameraObj;
	private List<ParticleCollisionEvent> collisionEventList = new List<ParticleCollisionEvent>();
	private void Update()
	{
		if (cameraObj.transform.localPosition.y > 2f)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
	private void OnParticleCollision()
	{
		iceMachineFX.GetCollisionEvents(base.gameObject, collisionEventList);
		foreach (ParticleCollisionEvent collisionEvent in collisionEventList)
		{
			Vector3 intersection = collisionEvent.intersection;
			GameObject gameObject = UnityEngine.Object.Instantiate(iceSpawnFXPrefab, intersection, Quaternion.identity, base.transform);
			gameObject.transform.AddLocalPositionY(0.03f);
			gameObject.transform.localScale = 0.5f * Vector3.one;
		}
	}
}
