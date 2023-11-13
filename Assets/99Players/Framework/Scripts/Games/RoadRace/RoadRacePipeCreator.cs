using UnityEngine;
public class RoadRacePipeCreator : MonoBehaviour
{
	[Header("作成するプレハブ")]
	public GameObject createPrefabS;
	[Header("作成するプレハブ")]
	public GameObject createPrefabM;
	[Header("作成するプレハブ")]
	public GameObject createPrefabL;
	[Header("作成したものをこのオブジェクトの子にする")]
	public Transform prefabParent;
	[Header("対象の名前")]
	public string targetName;
}
