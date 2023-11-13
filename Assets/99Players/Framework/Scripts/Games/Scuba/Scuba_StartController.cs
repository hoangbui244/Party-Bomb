using UnityEngine;
public class Scuba_StartController : MonoBehaviour
{
	public void DirectionEnd()
	{
		SingletonCustom<Scuba_GameManager>.Instance.StartDirectionEnd();
	}
	public void DiveSePlay()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_scuba_dive");
	}
}
