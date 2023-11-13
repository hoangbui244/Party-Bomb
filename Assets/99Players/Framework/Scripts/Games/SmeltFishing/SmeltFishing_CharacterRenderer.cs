using UnityEngine;
public class SmeltFishing_CharacterRenderer : MonoBehaviour
{
	[SerializeField]
	private GameObject[] bodyParts;
	private SmeltFishing_Character playingCharacter;
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
	}
	public void ChangeBodyPartsLayer(int _layer)
	{
		GameObject[] array = bodyParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].layer = _layer;
		}
	}
}
