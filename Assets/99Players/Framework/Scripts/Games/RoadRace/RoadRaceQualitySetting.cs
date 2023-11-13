using UnityEngine;
public class RoadRaceQualitySetting : MonoBehaviour
{
	[SerializeField]
	[Header("画面2分割or4分割")]
	private MeshRenderer[] shadowDisableRenderers_2;
	[SerializeField]
	private MeshRenderer[] shadowAndReceiveDisableRenderers_2;
	[SerializeField]
	private GameObject[] activeDisableObjs_2;
	[SerializeField]
	[Header("画面4分割")]
	private GameObject[] activeDisableObjs_4;
}
