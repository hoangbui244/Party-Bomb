using System;
using UnityEngine;
public class MorphingRace_Character : MonoBehaviour
{
	public enum MoveState
	{
		Walk,
		Run,
		Dash
	}
	private MorphingRace_Player player;
	private CharacterStyle style;
	private CharacterParts parts;
	private float animSpeed;
	private float animationTime;
	private bool isChangeAnimationNeutral;
	private float runAnimationSpeed = 50f;
	public void Init(MorphingRace_Player _player)
	{
		player = _player;
		style = base.gameObject.GetComponent<CharacterStyle>();
		parts = base.gameObject.GetComponent<CharacterParts>();
	}
	public void SetStyle(int _charaIdx)
	{
		style.SetGameStyle(GS_Define.GameType.RECEIVE_PON, _charaIdx);
	}
	public void SetLayer()
	{
		parts.Parts.SetLayer(LayerMask.NameToLayer("Character"), CharacterParts.BodyPartsList.HIP);
	}
	private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime, bool _isLoalPos = true, bool _isLocalEuler = true, float _delay = 0f)
	{
		if (_parts.gameObject != base.gameObject)
		{
			LeanTween.cancel(_parts.gameObject);
		}
		if (_isLoalPos)
		{
			LeanTween.moveLocal(_parts.gameObject, _pos, _animTime).setDelay(_delay);
		}
		else
		{
			LeanTween.move(_parts.gameObject, _pos, _animTime).setDelay(_delay);
		}
		if (_isLocalEuler)
		{
			LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime).setDelay(_delay);
		}
		else
		{
			LeanTween.rotate(_parts.gameObject, _euler, _animTime).setDelay(_delay);
		}
	}
	public void ResetAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void GameStartAnimation()
	{
		SetReadyMoveAnimation();
	}
	public void SetAnimationSpeed(int _characterType, float _intervalLerp)
	{
		animSpeed = SingletonCustom<MorphingRace_CharacterManager>.Instance.GetMoveAnimationSpeed(_characterType, _intervalLerp);
	}
	public void SetReadyMoveAnimation()
	{
	}
	public void MoveAnimation(int _characterType, float _intervalLerp, Vector3 _nowPos, Vector3 _prevPos)
	{
		float moveAnimationSpeed = SingletonCustom<MorphingRace_CharacterManager>.Instance.GetMoveAnimationSpeed(_characterType, _intervalLerp);
		MoveState moveState = (!SingletonCustom<MorphingRace_CharacterManager>.Instance.CheckWalkMoveAnimationSpeed(_characterType, moveAnimationSpeed)) ? (SingletonCustom<MorphingRace_CharacterManager>.Instance.CheckRunMoveAnimationSpeed(_characterType, moveAnimationSpeed) ? MoveState.Run : MoveState.Dash) : MoveState.Walk;
		switch (moveState)
		{
		case MoveState.Walk:
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalPositionY(Mathf.Sin(animationTime * (float)Math.PI) * 0.01f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) * 3f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesY(Mathf.Sin(animationTime * (float)Math.PI) * 2.5f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) * 2f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 20f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 30f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -30f);
			break;
		case MoveState.Run:
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalPositionY(Mathf.Sin(animationTime * (float)Math.PI) * 0.01f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) * 3f + 5f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesY(Mathf.Sin(animationTime * (float)Math.PI) * 2.5f + 2.5f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) + 5f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 40f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -40f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f);
			break;
		case MoveState.Dash:
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalPositionY(Mathf.Sin(animationTime * (float)Math.PI) * 0.01f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) * 3f + 15f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesY(Mathf.Sin(animationTime * (float)Math.PI) * 2.5f + 2.5f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI) + 10f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f);
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f);
			break;
		}
		if (CalcManager.Length(_nowPos, _prevPos) > 0.01f)
		{
			switch (moveState)
			{
			case MoveState.Walk:
				runAnimationSpeed = 20f;
				break;
			case MoveState.Run:
				runAnimationSpeed = 30f;
				break;
			case MoveState.Dash:
				runAnimationSpeed = 40f;
				break;
			}
			animationTime += CalcManager.Length(_nowPos, _prevPos) * runAnimationSpeed * Time.deltaTime;
			if (animationTime >= 1f)
			{
				animationTime = 0f;
			}
			isChangeAnimationNeutral = false;
		}
		else
		{
			if (isChangeAnimationNeutral)
			{
				return;
			}
			if (animationTime <= 0.25f)
			{
				animationTime -= 1f * Time.deltaTime;
				if (animationTime <= 0f)
				{
					animationTime = 0f;
					isChangeAnimationNeutral = true;
				}
			}
			else if (animationTime <= 0.5f)
			{
				animationTime += 1f * Time.deltaTime;
				if (animationTime >= 0.5f)
				{
					animationTime = 0.5f;
					isChangeAnimationNeutral = true;
				}
			}
			else if (animationTime <= 0.75f)
			{
				animationTime -= 1f * Time.deltaTime;
				if (animationTime <= 0.5f)
				{
					animationTime = 0.5f;
					isChangeAnimationNeutral = true;
				}
			}
			else
			{
				animationTime += 1f * Time.deltaTime;
				if (animationTime >= 1f)
				{
					animationTime = 1f;
					isChangeAnimationNeutral = true;
				}
			}
		}
	}
	public void MorphingFish_JumpAnimation(float _animTime, Vector3 _jumpPos, Vector3 _endPos, bool _isFishToHuman, Action _callBack = null)
	{
		ResetAnimation(0f);
		MorphingFish_JumpAnimation(_animTime);
		LeanTween.move(player.gameObject, _jumpPos, _animTime).setEaseOutCubic();
		LeanTween.delayedCall(player.gameObject, _animTime, (Action)delegate
		{
			LeanTween.move(player.gameObject, _endPos, _animTime).setEaseInCubic();
			if (_isFishToHuman)
			{
				LeanTween.delayedCall(player.gameObject, _animTime * 0.5f, (Action)delegate
				{
					ResetAnimation(_animTime * 0.5f);
				});
			}
			LeanTween.delayedCall(player.gameObject, _animTime, (Action)delegate
			{
				if (_callBack != null)
				{
					_callBack();
				}
			});
		});
	}
	private void MorphingFish_JumpAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(347.9209f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(87.82453f, 213.367f, 180f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.136f, 0.035f), new Vector3(313.2339f, 195.8934f, 180f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(294.2001f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.024f, 0.086f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.052f, -0.0507f), new Vector3(32.04827f, 0f, 0f), _animTime);
	}
	public void GoalRankAnimation(float _animTime, int _rank)
	{
		switch (_rank)
		{
		case 1:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.135f, 0.15f, 0f), new Vector3(0f, 0f, -165f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.135f, 0.15f, 0f), new Vector3(0f, 0f, 165f), _animTime);
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.HAPPY);
			LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
			{
				AfterRankGoalAnimation(_rank);
			});
			break;
		case 2:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.135f, 0.15f, 0f), new Vector3(0f, 0f, 165f), _animTime);
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.SMILE);
			LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
			{
				AfterRankGoalAnimation(_rank);
			});
			break;
		case 3:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, 0.002f), new Vector3(5.9f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.165f, 0.01f), new Vector3(24f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.NORMAL);
			break;
		case 4:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, 0.002f), new Vector3(5.9f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.165f, 0.01f), new Vector3(24f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.115f, 0.115f, 0.09f), new Vector3(315f, 0f, 0f), _animTime);
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.SAD);
			break;
		}
	}
	public void AfterRankGoalAnimation(int _rank)
	{
		switch (_rank)
		{
		case 1:
			LeanTween.rotateX(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setOnUpdate((Action<float>)delegate
			{
				parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localEulerAngles.x);
			}).setLoopPingPong();
			break;
		case 2:
			LeanTween.rotateX(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
			break;
		}
	}
	public void SetBlink()
	{
		parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).gameObject.SetActive(value: false);
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).gameObject.SetActive(value: true);
		});
		LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
		{
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).gameObject.SetActive(value: false);
		});
		LeanTween.delayedCall(base.gameObject, 0.75f, (Action)delegate
		{
			parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).gameObject.SetActive(value: true);
		});
	}
}
