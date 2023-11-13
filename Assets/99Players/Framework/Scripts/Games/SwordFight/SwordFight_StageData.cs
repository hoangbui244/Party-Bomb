using UnityEngine;
public class SwordFight_StageData : MonoBehaviour
{
	public Material fieldMaterial;
	public Material floorMaterial;
	public Material goalMaterial;
	public Material[] ballMaterial;
	public Material deskMaterial;
	public Material GetFieldMaterial()
	{
		return fieldMaterial;
	}
	public Material GetFloorMaterial()
	{
		return floorMaterial;
	}
	public Material GetGoalMaterial()
	{
		return goalMaterial;
	}
	public Material GetRandomBallMaterial()
	{
		return ballMaterial[Random.Range(0, ballMaterial.Length)];
	}
	public Material GetBallMaterial(int _no)
	{
		return ballMaterial[_no];
	}
	public Material GetDeskMaterial()
	{
		return deskMaterial;
	}
}
