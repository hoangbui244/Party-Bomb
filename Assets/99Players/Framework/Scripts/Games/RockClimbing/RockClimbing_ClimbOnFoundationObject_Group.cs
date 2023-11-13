using System;
using UnityEngine;
public class RockClimbing_ClimbOnFoundationObject_Group : MonoBehaviour
{
	[Serializable]
	public struct ClimbOnFoundationAnchor
	{
		public Transform climbOnFoundationAnchor;
		private RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundation;
		public RockClimbing_ClimbOnFoundation[] GetArrayClimbOnFoundation()
		{
			return arrayClimbOnFoundation;
		}
		public void SetArrayClimbOnFoundation(RockClimbing_ClimbOnFoundation[] _arrayClimbOnFoundation)
		{
			arrayClimbOnFoundation = _arrayClimbOnFoundation;
		}
	}
	[SerializeField]
	[Header("各アンカ\u30fc用のの構造体配列")]
	private ClimbOnFoundationAnchor[] arrayClimbOnFoundationAnchor;
	public void Init()
	{
		for (int i = 0; i < arrayClimbOnFoundationAnchor.Length; i++)
		{
			arrayClimbOnFoundationAnchor[i].SetArrayClimbOnFoundation(arrayClimbOnFoundationAnchor[i].climbOnFoundationAnchor.GetComponentsInChildren<RockClimbing_ClimbOnFoundation>());
		}
	}
	public ClimbOnFoundationAnchor[] GetArrayClimbOnFoundationAnchor()
	{
		return arrayClimbOnFoundationAnchor;
	}
}
