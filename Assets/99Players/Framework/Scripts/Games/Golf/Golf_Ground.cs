using UnityEngine;
public class Golf_Ground : MonoBehaviour
{
	[SerializeField]
	[Header("地面の種類")]
	private Golf_FieldManager.GroundType groundType;
	public Golf_FieldManager.GroundType GetGroundType()
	{
		return groundType;
	}
}
