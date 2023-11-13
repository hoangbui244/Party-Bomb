using UnityEngine;
public class MonsterRace_StaminaGage : MonoBehaviour
{
	private MonsterRace_CarScript car;
	[SerializeField]
	private GameObject[] NoItemSprite;
	[SerializeField]
	private GameObject[] OnItemSprite;
	public MonsterRace_CarScript Car
	{
		get
		{
			return car;
		}
		set
		{
			car = value;
		}
	}
	public void Init()
	{
	}
	public void SetItem()
	{
		for (int i = 0; i < car.MaxStaminaCount; i++)
		{
			NoItemSprite[i].SetActive(i >= car.StaminaCount);
			OnItemSprite[i].SetActive(i < car.StaminaCount);
		}
	}
}
