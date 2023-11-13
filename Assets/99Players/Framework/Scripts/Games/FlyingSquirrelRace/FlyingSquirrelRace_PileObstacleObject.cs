using UnityEngine;
public class FlyingSquirrelRace_PileObstacleObject : FlyingSquirrelRace_ObstacleObject
{
	[SerializeField]
	[Header("オブジェクト")]
	private GameObject obj;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private Collider collider;
	protected override void OnInitialize()
	{
		float y = Random.Range(0f, 359.9f);
		obj.transform.SetLocalEulerAnglesY(y);
		collider.transform.SetLocalEulerAnglesY(y);
	}
}
