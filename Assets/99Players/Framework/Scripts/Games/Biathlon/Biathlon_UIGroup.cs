using UnityEngine;
public class Biathlon_UIGroup : MonoBehaviour
{
	[SerializeField]
	private GameObject[] elements;
	public void Activate()
	{
		GameObject[] array = elements;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void Deactivate()
	{
		GameObject[] array = elements;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}
}
