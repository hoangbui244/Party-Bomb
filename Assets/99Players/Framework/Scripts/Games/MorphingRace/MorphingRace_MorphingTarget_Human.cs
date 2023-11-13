using UnityEngine;
public class MorphingRace_MorphingTarget_Human : MorphingRace_MorphingTarget
{
	[SerializeField]
	[Header("人キャラの時の種類")]
	private MorphingRace_FieldManager.HumanTargetPrefType morphingHumanType;
	public MorphingRace_FieldManager.HumanTargetPrefType GetMorphingHumanType()
	{
		return morphingHumanType;
	}
}
