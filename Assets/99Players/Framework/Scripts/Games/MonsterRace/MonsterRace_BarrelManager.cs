using UnityEngine;
public class MonsterRace_BarrelManager : MonoBehaviour
{
	[SerializeField]
	private MonsterRace_Barrel[] barrels;
	[SerializeField]
	private Transform[] createAnchors;
	[SerializeField]
	private float interval;
	[SerializeField]
	private Vector3 vec;
	[SerializeField]
	private float colliderDelay;
	private float timer;
	private int createNo;
	public void Update()
	{
		timer += Time.deltaTime;
		if (!(timer > interval))
		{
			return;
		}
		timer -= interval;
		int num = 0;
		while (true)
		{
			if (num < barrels.Length)
			{
				if (!barrels[num].Active)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		barrels[num].SetViewActive(_active: true);
		barrels[num].transform.position = createAnchors[createNo].position;
		barrels[num].ColliderDelayEnable(colliderDelay);
		barrels[num].ResetRotation();
		barrels[num].ResetVelocity();
		barrels[num].AddForce(vec, ForceMode.VelocityChange);
		createNo++;
		if (createNo >= createAnchors.Length)
		{
			createNo = 0;
		}
	}
}
