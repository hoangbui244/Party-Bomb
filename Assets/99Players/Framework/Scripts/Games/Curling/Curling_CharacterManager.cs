using UnityEngine;
public class Curling_CharacterManager : SingletonCustom<Curling_CharacterManager>
{
	public enum MotionType
	{
		Stand,
		Throw,
		Sweep
	}
	[SerializeField]
	[Header("キャラクタ\u30fcモ\u30fcション用プレハブ")]
	private Curling_CharacterParts[] arrayCharcterPartsPref;
	public void SetMotion(MotionType _motionType, Curling_CharacterParts _characterParts)
	{
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L).localEulerAngles;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R).localPosition = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R).localPosition;
		_characterParts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R).localEulerAngles = arrayCharcterPartsPref[(int)_motionType].Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R).localEulerAngles;
	}
	public void SetOptionAnchor(MotionType _motionType, Curling_CharacterParts _characterParts)
	{
		_characterParts.GetBrushObj().transform.localPosition = arrayCharcterPartsPref[(int)_motionType].GetBrushObj().transform.localPosition;
		_characterParts.GetBrushObj().transform.localEulerAngles = arrayCharcterPartsPref[(int)_motionType].GetBrushObj().transform.localEulerAngles;
	}
}
