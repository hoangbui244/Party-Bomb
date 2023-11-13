using UnityEngine;
public class MorphingRace_MorphingTarget_Collider : MonoBehaviour
{
	private MorphingRace_MorphingTarget morphingTarget;
	private MorphingRace_MorphingTarget_MorphingPoint morphingPoint;
	public void Init(MorphingRace_MorphingTarget _morphingTarget, MorphingRace_MorphingTarget_MorphingPoint _morphingPoint)
	{
		morphingTarget = _morphingTarget;
		morphingPoint = _morphingPoint;
	}
	public MorphingRace_MorphingTarget GetMorphingTarget()
	{
		return morphingTarget;
	}
	public MorphingRace_MorphingTarget_MorphingPoint GetMorphingPoint()
	{
		return morphingPoint;
	}
	public void HideMorphingPoint()
	{
		morphingPoint.Hide();
	}
}
