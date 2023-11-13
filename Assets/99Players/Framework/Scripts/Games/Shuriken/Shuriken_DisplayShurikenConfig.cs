using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_DisplayShurikenConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("手裏剣の表示位置")]
	private Vector3[] displayShurikenPosition = new Vector3[4];
	[SerializeField]
	[DisplayName("再表示の基準位置")]
	private float popupBasePosition = -0.5f;
	[SerializeField]
	[DisplayName("手裏剣のマテリアル")]
	private Material[] shurikenMaterials = new Material[8];
	public Vector3[] DisplayShurikenPosition => displayShurikenPosition;
	public float PopupBasePosition => popupBasePosition;
	public Material[] ShurikenMaterials => shurikenMaterials;
}
