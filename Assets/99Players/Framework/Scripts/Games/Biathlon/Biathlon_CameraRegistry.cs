using UnityEngine;
public class Biathlon_CameraRegistry : SingletonCustom<Biathlon_CameraRegistry>
{
	[SerializeField]
	private Biathlon_CharacterCamera[] cameras;
	public void Init()
	{
		Biathlon_CharacterCamera[] array = cameras;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}
	public Biathlon_CharacterCamera GetCamera(int no)
	{
		return cameras[no];
	}
}
