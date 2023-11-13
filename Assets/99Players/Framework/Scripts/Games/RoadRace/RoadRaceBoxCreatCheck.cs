using UnityEngine;
public class RoadRaceBoxCreatCheck : MonoBehaviour
{
	[SerializeField]
	[Header("生存時間")]
	private float lifeTime = 7f;
	public RoadRaceWaters raceWaters;
	private float countTime;
	private int id;
	private void OnEnable()
	{
		countTime = 0f;
	}
	public void Update()
	{
		countTime += Time.deltaTime;
		if (countTime > lifeTime)
		{
			raceWaters.InitBox(id);
		}
	}
	public void SetID(int _id)
	{
		id = _id;
	}
	public float GetTimePer()
	{
		return Mathf.Max(countTime / lifeTime, 0f);
	}
	public void ResetTime()
	{
		countTime = 0f;
	}
	private void OnTriggerStay(Collider _collider)
	{
		if (_collider.tag == "Character")
		{
			RoadRaceCharacterScript chara = Scene_RoadRace.CM.GetChara(_collider.attachedRigidbody.gameObject);
			if (chara.transform != raceWaters.characterObject)
			{
				chara.AddWaterGage(GetTimePer());
			}
		}
	}
}
