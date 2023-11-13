using System;
using UnityEngine;
public class RockClimbing_Character : MonoBehaviour
{
	public enum AnimState
	{
		Anim1,
		Anim2,
		Anim3,
		Anim4,
		Anim5,
		Anim6
	}
	private RockClimbing_Player player;
	private CharacterStyle style;
	private CharacterParts parts;
	private Vector3[] arrayClimbOnPos;
	private bool isBlink;
	[SerializeField]
	[Header("頭頂部")]
	private Transform headTop;
	[SerializeField]
	[Header("縄の輪っかを持つ部分")]
	private Transform haveRopeRing;
	[SerializeField]
	[Header("鉤縄を持つ部分")]
	private Transform haveGrapplingHook;
	private float animationTime;
	private bool isChangeAnimationNeutral;
	private float runAnimationSpeed = 50f;
	private float collectRopeAnimationSpeed = 2f;
	public void Init(RockClimbing_Player _player)
	{
		player = _player;
		style = base.gameObject.GetComponent<CharacterStyle>();
		parts = base.gameObject.GetComponent<CharacterParts>();
	}
	public void SetArrayClimbOnPosElement()
	{
		arrayClimbOnPos = new Vector3[SingletonCustom<RockClimbing_CharacterManager>.Instance.GetArrayClimbOnAnchor().Length];
	}
	public void SetStyle(int _charaIdx)
	{
		style.SetGameStyle(GS_Define.GameType.MOLE_HAMMER, _charaIdx);
	}
	public Transform GetHeadTop()
	{
		return headTop;
	}
	public Transform GetHaveRopeRing()
	{
		return haveRopeRing;
	}
	public Transform GetHaveGrapplingHook()
	{
		return haveGrapplingHook;
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
		LeanTween.rotateY(base.gameObject, 0f, 0.5f).setOnComplete((Action)delegate
		{
			LeanTween.moveLocalZ(base.gameObject, 0.1f, 0.5f);
			ReadyClimbingAnimation(SingletonCustom<RockClimbing_CharacterManager>.Instance.GetReadyClimbingAnimationTime());
		});
	}
	public void ReadyClimbingAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(345.5f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.055f, 0f), new Vector3(355f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.0716f, 0.1621f, 0.2281f), new Vector3(325.1356f, 178.8642f, 187.2883f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.0635f, 0.2434f, 0.2121f), new Vector3(316.8468f, 159.9283f, 187.0467f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
		{
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
		}
		else
		{
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
		}
	}
	public void ClimbingAnimation(bool _isOdd, float _intervalLerp, Action _callBack)
	{
		float animSpeed = SingletonCustom<RockClimbing_CharacterManager>.Instance.ClampAnimationSpeed(_intervalLerp);
		ClimbingAnimation(_isOdd, animSpeed, AnimState.Anim1);
		LeanTween.delayedCall(base.gameObject, animSpeed, (Action)delegate
		{
			LeanTween.delayedCall(base.gameObject, animSpeed, (Action)delegate
			{
				if (_callBack != null)
				{
					_callBack();
				}
			});
		});
	}
	private void ClimbingAnimation(bool isOdd, float _animTime, AnimState _animState)
	{
		if (!isOdd)
		{
			switch (_animState)
			{
			case AnimState.Anim1:
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(350f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.0621457f, 0.2449935f, 0.2105448f), new Vector3(317.2044f, 206.4438f, 167.4856f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.07172774f, 0.16139f, 0.2281518f), new Vector3(324.04f, 181.6244f, 172.2831f), _animTime);
				if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
				}
				else
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				}
				break;
			case AnimState.Anim2:
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(350f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.217f, 0.103f), new Vector3(45f, 300f, 145f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.088f, -0.098f), new Vector3(315f, 240f, 145f), _animTime);
				if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
				}
				else
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				}
				break;
			case AnimState.Anim3:
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.025f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.217f, 0.108f), new Vector3(315f, 240f, 145f), _animTime);
				if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
				}
				else
				{
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
					Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				}
				break;
			}
			return;
		}
		switch (_animState)
		{
		case AnimState.Anim1:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.0716f, 0.1621f, 0.2281f), new Vector3(325.1356f, 178.8642f, 187.2883f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.0635f, 0.2434f, 0.2121f), new Vector3(316.8468f, 159.9283f, 187.0467f), _animTime);
			if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
			}
			else
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
			}
			break;
		case AnimState.Anim2:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.088f, -0.098f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.217f, 0.103f), new Vector3(315f, 240f, 145f), _animTime);
			if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
			}
			else
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
			}
			break;
		case AnimState.Anim3:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(350f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.217f, 0.108f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.025f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
			if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(player.GetClimbOnFoundation().GetClimbOnFoundationType()))
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
			}
			else
			{
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, -0.045f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.06f, 0f, 0.093f), new Vector3(0f, 0f, 0f), _animTime);
			}
			break;
		}
	}
	public void MoveAnimation()
	{
		parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 15f);
		parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -15f);
		parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * 15f);
		parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(animationTime * (float)Math.PI * 2f) * -15f);
		if (CalcManager.Length(player.GetNowPos(), player.GetPrevPos()) > 0.01f)
		{
			animationTime += CalcManager.Length(player.GetNowPos(), player.GetPrevPos()) * runAnimationSpeed * Time.deltaTime;
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
	public void ThrowAnimation(Action _callBack)
	{
		float animTime = SingletonCustom<RockClimbing_CharacterManager>.Instance.GetThrowGrapplingHookAnimationTime();
		ThrowAnimation(animTime, AnimState.Anim1);
		LeanTween.delayedCall(base.gameObject, animTime, (Action)delegate
		{
			LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
			{
				ThrowAnimation(animTime, AnimState.Anim2);
				LeanTween.delayedCall(base.gameObject, animTime * 0.75f, (Action)delegate
				{
					_callBack();
				});
			});
		});
	}
	private void ThrowAnimation(float _animTime, AnimState _animState)
	{
		switch (_animState)
		{
		case AnimState.Anim1:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735f, 0f), new Vector3(333.5f, 2.52f, 350f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(355f, 25f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.15f, 0.1f, -0.025f), new Vector3(285f, 330f, 15f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.12f, 0.111f, 0.042f), new Vector3(303f, 325f, 105f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L).transform.localPosition, new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R).transform.localPosition, new Vector3(0f, 0f, 0f), _animTime);
			break;
		case AnimState.Anim2:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(340f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.129f, 0f), new Vector3(290f, 180f, 180f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.155f, 0.18f, 0.08f), new Vector3(320f, 180f, 180f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0.021f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, -0.008f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		}
	}
	public void SetClimbOnPos()
	{
		Transform[] arrayClimbOnAnchor = player.GetClimbOnFoundation().GetArrayClimbOnAnchor();
		for (int i = 0; i < arrayClimbOnAnchor.Length; i++)
		{
			arrayClimbOnPos[i] = new Vector3(base.transform.position.x, arrayClimbOnAnchor[i].position.y, arrayClimbOnAnchor[i].position.z);
			Vector3 vector = arrayClimbOnPos[i];
			UnityEngine.Debug.Log("arrayClimbOnPos[i] " + vector.ToString());
		}
	}
	public void ClimbOnAnimation(bool _isGoal = false, Action _callBack = null)
	{
		float animTime = SingletonCustom<RockClimbing_CharacterManager>.Instance.GetClimbOnAnimationTime();
		ClimbOnAnimation(animTime, AnimState.Anim1);
		LeanTween.delayedCall(base.gameObject, animTime, (Action)delegate
		{
			ClimbOnAnimation(animTime, AnimState.Anim2);
			LeanTween.delayedCall(base.gameObject, animTime, (Action)delegate
			{
				ClimbOnAnimation(animTime, AnimState.Anim3, _isGoal);
				LeanTween.delayedCall(base.gameObject, animTime, (Action)delegate
				{
					if (_callBack != null)
					{
						_callBack();
					}
				});
			});
		});
	}
	private void ClimbOnAnimation(float _animTime, AnimState _animState, bool _isGoal = false)
	{
		switch (_animState)
		{
		case AnimState.Anim1:
			Animation(base.transform, arrayClimbOnPos[(int)_animState], new Vector3(0f, 0f, 0f), _animTime, _isLoalPos: false);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(349.2066f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.002f, -0.013f), new Vector3(359.3358f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05356598f, 0.01595052f), new Vector3(1.530573f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.127f, 0.04f), new Vector3(348.2313f, 63.27095f, 267.2055f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.123f, 0.052f), new Vector3(356.7374f, 301.0549f, 116.8664f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(316.0092f, 3.994534f, 6.859902f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(319.6358f, 30.74282f, 316.3231f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.068f, -0.04833f, 0f), new Vector3(348.5809f, 0.3663634f, 0.5334841f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case AnimState.Anim2:
			Animation(base.transform, arrayClimbOnPos[(int)_animState], new Vector3(0f, 0f, 0f), _animTime, _isLoalPos: false);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.175f, 0f), new Vector3(340.5f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, -0.015f), new Vector3(43.77412f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.04087448f, 0.01260412f), new Vector3(1.943285f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.122f, 0.034f), new Vector3(336f, 73f, 235f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.127f, 0.04f), new Vector3(350f, 295f, 95f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(315f, 5f, 6.86f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, -0.029f, 0f), new Vector3(315f, 30f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.112f, -0.021f, 0.054f), new Vector3(8.567815f, 318.7467f, 291.7385f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.047f, 0.015f), new Vector3(332.2637f, 0f, 0f), _animTime);
			break;
		case AnimState.Anim3:
			Animation(base.transform, arrayClimbOnPos[(int)_animState], new Vector3(0f, _isGoal ? 180f : 0f, 0f), _animTime, _isLoalPos: false);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		}
	}
	public void SetCollectRopeAnimation(Action _callBack_1 = null, Action _callBack_2 = null)
	{
		SetCollectRopeAnimation(SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCollectRopeTime(), AnimState.Anim1);
		LeanTween.delayedCall(base.gameObject, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCollectRopeTime(), (Action)delegate
		{
			if (_callBack_1 != null)
			{
				_callBack_1();
			}
			SetCollectRopeAnimation(SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCollectRopeTime(), AnimState.Anim2);
			LeanTween.delayedCall(base.gameObject, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCollectRopeTime(), (Action)delegate
			{
				animationTime = 0f;
				if (_callBack_2 != null)
				{
					_callBack_2();
				}
			});
		});
	}
	private void SetCollectRopeAnimation(float _animTime, AnimState _animState)
	{
		switch (_animState)
		{
		case AnimState.Anim1:
			Animation(base.transform, player.transform.localPosition, new Vector3(0f, 180f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(11.60249f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(38.4684f, 0.9345477f, 0.6659224f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.0836f, 0.071f, 0.09f), new Vector3(270f, 25f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.105f, 0.127f, 0.117f), new Vector3(270f, 335f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.1f, -0.0483f, 0.063f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.1f, -0.0483f, 0.063f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case AnimState.Anim2:
		{
			animationTime = 0f;
			float shoulderLPosZ = parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).transform.localPosition.z;
			float shoulderRPosZ = parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localPosition.z;
			LeanTween.value(base.gameObject, 0f, 1f, _animTime).setOnUpdate((Action<float>)delegate
			{
				parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalPositionZ(shoulderLPosZ + Mathf.Sin(animationTime * (float)Math.PI * 2f) * 0.05f);
				parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalPositionZ(shoulderRPosZ + Mathf.Sin(animationTime * (float)Math.PI * 2f) * -0.05f);
				animationTime += collectRopeAnimationSpeed * Time.deltaTime;
				if (animationTime >= 1f)
				{
					animationTime = 0f;
				}
			});
			break;
		}
		}
	}
	public void GoalRankAnimation(float _animTime, int _rank)
	{
		switch (_rank)
		{
		case 1:
			Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.135f, 0.15f, 0f), new Vector3(0f, 0f, 195f), _animTime);
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
		LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y + 15f, 1f);
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y - 30f, 2f).setLoopPingPong();
		});
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
	public void SetNinjaMaterial(Material _mat)
	{
		parts.Parts.SetMaterial(_mat);
	}
	public void NinjaReadyThrowAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 340f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 20f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.163f, 0.126f, 0.053f), new Vector3(298.0606f, 144.05f, 193.5049f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.157f, 0.188f, 0.064f), new Vector3(21.20433f, 191.3867f, 197.1786f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 29.81172f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void NinjaThrowAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.163f, 0.126f, 0.053f), new Vector3(298.0606f, 144.05f, 193.5049f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.153f, 0.098f, 0.064f), new Vector3(307.2052f, 15.86499f, 334.3918f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 29.81172f), _animTime);
		Animation(parts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
	}
}
