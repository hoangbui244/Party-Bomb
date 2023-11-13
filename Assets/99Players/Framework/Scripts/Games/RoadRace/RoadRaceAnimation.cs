using System;
using UnityEngine;
public class RoadRaceAnimation : MonoBehaviour
{
	[SerializeField]
	[Header("パ\u30fcツ")]
	private CharacterParts bodyParts;
	[SerializeField]
	[Header("足サブパ\u30fcツ")]
	private Transform[] legSubParts;
	[SerializeField]
	[Header("足アンカ\u30fc")]
	private Transform[] legAnchors;
	[SerializeField]
	[Header("ル\u30fcトキャラ")]
	private RoadRaceCharacterScript rootCharacter;
	private Quaternion[] partsRotDef;
	private Vector3[] partsPosDef;
	[SerializeField]
	[Header("回転アンカ\u30fc")]
	private Transform rotAnchor;
	public CharacterParts.BodyParts Parts => bodyParts.Parts;
	public Transform[] LegSubParts => legSubParts;
	public Transform[] LegAnchors => legAnchors;
	public void Init()
	{
		partsRotDef = new Quaternion[Parts.GetRendererListLength()];
		partsPosDef = new Vector3[Parts.GetRendererListLength()];
		for (int i = 0; i < Parts.GetRendererListLength(); i++)
		{
			partsRotDef[i] = Parts.RendererParts(i).localRotation;
			partsPosDef[i] = Parts.RendererParts(i).localPosition;
		}
	}
	private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime)
	{
		LeanTween.cancel(_parts.gameObject);
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
		LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime);
	}
	private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime, float _delay)
	{
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime).setDelay(_delay);
		LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime).setDelay(_delay);
	}
	private void AnimationRotateAround(Transform _parts, Vector3 _pos, Vector3 _dir, float _angle, float _animTime)
	{
		LeanTween.cancel(_parts.gameObject);
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
		LeanTween.rotateAround(_parts.gameObject, _dir, _angle, _animTime);
	}
	public void ResetAnimation()
	{
		Part(CharacterParts.BodyPartsList.HIP).gameObject.SetActive(value: true);
		for (int i = 0; i < Parts.GetRendererListLength(); i++)
		{
			LeanTween.cancel(Parts.RendererParts(i).gameObject);
			Parts.RendererParts(i).localRotation = partsRotDef[i];
			Parts.RendererParts(i).localPosition = partsPosDef[i];
		}
		LeanTween.cancel(rotAnchor.gameObject);
		rotAnchor.SetLocalPosition(0f, 0f, 0f);
		rotAnchor.SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void ActionAnimation(int _no, float _animTime)
	{
		LeanTween.cancel(rotAnchor.gameObject);
		switch (_no)
		{
		case 1:
			break;
		case 0:
			Animation(Part(CharacterParts.BodyPartsList.HEAD), PosDef(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.BODY), PosDef(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), PosDef(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0f, 0f, 90f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), PosDef(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(0f, 0f, 270f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_R), PosDef(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_L), PosDef(CharacterParts.BodyPartsList.ARM_L), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case 2:
			Animation(Part(CharacterParts.BodyPartsList.HEAD), PosDef(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, -86f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.BODY), PosDef(CharacterParts.BodyPartsList.BODY), new Vector3(0f, -40f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), PosDef(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(320f, 70f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), PosDef(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(320f, -220f, 194f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_R), PosDef(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_L), PosDef(CharacterParts.BodyPartsList.ARM_L), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case 3:
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), PosDef(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(110f, -190f, 310f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_R), PosDef(CharacterParts.BodyPartsList.ARM_R), new Vector3(60f, 0f, 350f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_L), PosDef(CharacterParts.BodyPartsList.ARM_L), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case 4:
			Animation(Part(CharacterParts.BodyPartsList.HEAD), PosDef(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.BODY), PosDef(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), PosDef(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(225f, 90f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), PosDef(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(310f, 90f, 180f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_R), PosDef(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.ARM_L), PosDef(CharacterParts.BodyPartsList.ARM_L), new Vector3(0f, 0f, 0f), _animTime);
			break;
		}
	}
	public void GoalRankAnimation(float _animTime, int _rank)
	{
		switch (_rank)
		{
		case 1:
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.135f, 0.15f, 0f), new Vector3(0f, 0f, -165f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.135f, 0.15f, 0f), new Vector3(0f, 0f, 165f), _animTime);
			rootCharacter.Style.SetMainCharacterFaceDiff((int)rootCharacter.UserType, StyleTextureManager.MainCharacterFaceType.HAPPY);
			LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
			{
				AfterRankGoalAnimation(_rank);
			});
			break;
		case 2:
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.135f, 0.15f, 0f), new Vector3(0f, 0f, 165f), _animTime);
			rootCharacter.Style.SetMainCharacterFaceDiff((int)rootCharacter.UserType, StyleTextureManager.MainCharacterFaceType.SMILE);
			LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
			{
				AfterRankGoalAnimation(_rank);
			});
			break;
		case 3:
			Animation(Part(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, 0.002f), new Vector3(5.9f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.165f, 0.01f), new Vector3(24f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			rootCharacter.Style.SetMainCharacterFaceDiff((int)rootCharacter.UserType, StyleTextureManager.MainCharacterFaceType.NORMAL);
			break;
		case 4:
			Animation(Part(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, 0.002f), new Vector3(5.9f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.165f, 0.01f), new Vector3(24f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			Animation(Part(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			rootCharacter.Style.SetMainCharacterFaceDiff((int)rootCharacter.UserType, StyleTextureManager.MainCharacterFaceType.SAD);
			break;
		}
	}
	private void AfterRankGoalAnimation(int _rank)
	{
		switch (_rank)
		{
		case 1:
			LeanTween.rotateX(Part(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setOnUpdate((Action<float>)delegate
			{
				Part(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Part(CharacterParts.BodyPartsList.SHOULDER_R).transform.localEulerAngles.x);
			}).setLoopPingPong();
			break;
		case 2:
			LeanTween.rotateX(Part(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
			break;
		}
	}
	public Transform Part(CharacterParts.BodyPartsList _part)
	{
		return Parts.RendererParts(_part).transform;
	}
	public Transform Part(int _part)
	{
		return Parts.RendererParts(_part).transform;
	}
	private Vector3 RotDef(CharacterParts.BodyPartsList _part)
	{
		return partsRotDef[(int)_part].eulerAngles;
	}
	private Vector3 PosDef(CharacterParts.BodyPartsList _part)
	{
		return partsPosDef[(int)_part];
	}
}
