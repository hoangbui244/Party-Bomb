using UnityEngine;
public class Biathlon_FollowerCameraConfig : ScriptableObject
{
	[SerializeField]
	private Material[] gunMaterials;
	public Material[] GunMaterials => gunMaterials;
}
