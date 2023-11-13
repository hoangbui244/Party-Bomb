using UnityEngine;
public class RockClimbing_GrapplingHookPoint_Group : MonoBehaviour
{
	[SerializeField]
	[Header("GrapplingHookPoint配列")]
	private RockClimbing_GrapplingHookPoint[] arrayGrapplingHookPoint;
	public void Init(RockClimbing_ClimbOnFoundation _climbOnFoundation)
	{
		for (int i = 0; i < arrayGrapplingHookPoint.Length; i++)
		{
			if (_climbOnFoundation.GetClimbOnFoundationType() != RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_4 && i >= 1)
			{
				arrayGrapplingHookPoint[i].gameObject.SetActive(value: false);
				continue;
			}
			arrayGrapplingHookPoint[i].Init(_climbOnFoundation);
			arrayGrapplingHookPoint[i].gameObject.SetActive(value: true);
		}
	}
	public RockClimbing_GrapplingHookPoint[] GetArrayGrapplingHookPoint()
	{
		return arrayGrapplingHookPoint;
	}
}
