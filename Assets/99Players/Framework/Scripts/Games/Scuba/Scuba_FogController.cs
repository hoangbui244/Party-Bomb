using UnityEngine;
public class Scuba_FogController : MonoBehaviour
{
	public void FogOn()
	{
		RenderSettings.fog = true;
	}
	public void FogOff()
	{
		RenderSettings.fog = false;
	}
}
