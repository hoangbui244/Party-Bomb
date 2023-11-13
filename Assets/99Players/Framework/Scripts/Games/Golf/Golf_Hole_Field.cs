using UnityEngine;
public class Golf_Hole_Field : MonoBehaviour
{
	[SerializeField]
	[Header("必要なパワ\u30fc")]
	private float requiredPower;
	[SerializeField]
	[Header("カップアンカ\u30fc")]
	private Transform cupAnchor;
	public float GetRequiredPower()
	{
		return requiredPower;
	}
	public Vector3 GetCupPos()
	{
		return cupAnchor.position;
	}
}
