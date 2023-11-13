using UnityEngine;
public class Canoe_AddSpeedUp : MonoBehaviour
{
	public enum Type
	{
		WATER_FALL,
		AMONG_ROCKS
	}
	[SerializeField]
	[Header("加速の種類")]
	private Type addSpeedUpType;
	public Type GetAddSpeedUpType()
	{
		return addSpeedUpType;
	}
}
