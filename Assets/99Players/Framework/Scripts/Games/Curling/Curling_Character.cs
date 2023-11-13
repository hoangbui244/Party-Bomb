using System;
using UnityEngine;
public class Curling_Character : MonoBehaviour
{
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("体の各部位")]
	private Curling_CharacterParts parts;
	[SerializeField]
	[Header("リフレクション用のメッシュ")]
	private MeshRenderer reflectionMesh;
	[SerializeField]
	[Header("リフレクション用のテクスチャ")]
	private Texture[] arrayReflectionTexture;
	private Vector3 originBodyPos;
	private Vector3 sweepBodyPos;
	[SerializeField]
	[Header("こするときのBodyを動かすアンカ\u30fc")]
	private Transform sweepBodyAnchor;
	[SerializeField]
	[Header("こするときのBodyを動かす範囲")]
	private float sweepBodyRange;
	[SerializeField]
	[Header("こすった時に描画する床のトレイル")]
	private TrailRenderer sweepLine;
	[SerializeField]
	[Header("トレイルを動かす距離")]
	private float sweepLineRange;
	private bool isBrushAnimation;
	public void SetStyle(int _teamNo, int _charaIdx)
	{
		style.SetGameStyle(GS_Define.GameType.RECEIVE_PON, _charaIdx, _teamNo);
	}
	public void SetBibsStyle(int _teamNo, int _bipsIdx)
	{
	}
	public void SetReflection(int _charaIdx)
	{
		int num = 0;
		num = ((_charaIdx >= SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx.Length) ? GS_Define.CHARACTER_MAX : SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_charaIdx]);
		reflectionMesh.material.mainTexture = arrayReflectionTexture[num];
	}
	public Curling_CharacterParts GetParts()
	{
		return parts;
	}
	private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime, float _delay = 0f)
	{
		LeanTween.cancel(_parts.gameObject);
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime).setDelay(_delay);
		LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime).setDelay(_delay);
	}
	private void AnimationRotateAround(Transform _parts, Vector3 _pos, Vector3 _dir, float _angle, float _animTime, float _delay = 0f)
	{
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime).setDelay(_delay);
		LeanTween.rotateAround(_parts.gameObject, _dir, _angle, _animTime).setDelay(_delay);
	}
	public void ResetAnimation(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void SetThrowMotion(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.181f, 0.023f), new Vector3(340.1761f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(350f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.049f, 0.04f), new Vector3(50f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.155f, 0.123f, 0.007f), new Vector3(292.7624f, 0f, 335.8868f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1407182f, 0.07882839f), new Vector3(270.1297f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, 0.049f, 0.095f), new Vector3(310f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.059f, 0.002f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void SetStandMotion(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.089f, 0.127f, 0.094f), new Vector3(313.692f, 25.96011f, 358.608f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.089f, 0.127f, 0.09400012f), new Vector3(313.692f, 334.0399f, 1.392009f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(341.1992f, 7.606334f, 352.2504f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(341.1992f, 7.606334f, 352.2504f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void SetSweepMotion0(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(1.444499f, 22.1456f, 352.6286f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(18.79625f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.107f, 0.118f, 0.093f), new Vector3(277.1172f, 315.0975f, 59.66295f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.092f, 0.117f, 0.122f), new Vector3(277.1168f, 44.90251f, 302.2121f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 37.48354f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 322.5164f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.088f, -0.048f, 0.024f), new Vector3(2.022501f, 344.715f, 357.6376f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.058f, -0.048f, -0.014f), new Vector3(25.3376f, 323.8131f, 0f), _animTime);
		LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
		{
			originBodyPos = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition;
			Vector3 forward = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).forward;
			forward.y = 0f;
			sweepBodyPos = originBodyPos - forward * 0.05f;
		});
	}
	public void SetSweepMotion1(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(1.444499f, 337.8544f, 7.371396f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(18.79625f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.107f, 0.118f, 0.093f), new Vector3(277.1172f, 315.0975f, 59.66295f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.092f, 0.117f, 0.122f), new Vector3(277.1168f, 44.90251f, 302.2121f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 37.48354f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 322.5164f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0.069f), new Vector3(350.1335f, 34.1409f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.058f, -0.048f, -0.057f), new Vector3(25.3376f, 34.1409f, 0f), _animTime);
		LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
		{
			originBodyPos = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition;
			Vector3 forward = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).forward;
			forward.y = 0f;
			sweepBodyPos = originBodyPos - forward * 0.05f;
		});
	}
	public void SetHouseSweepMotion(float _animTime)
	{
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(18.79625f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.107f, 0.118f, 0.093f), new Vector3(277.1172f, 315.0975f, 59.66295f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.092f, 0.117f, 0.122f), new Vector3(277.1168f, 44.90251f, 302.2121f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 37.48354f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 322.5164f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0.069f), new Vector3(350.1335f, 0f, 0f), _animTime);
		Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, -0.057f), new Vector3(25.33761f, 34.1409f, 0f), _animTime);
		LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
		{
			originBodyPos = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition;
			Vector3 forward = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).forward;
			forward.y = 0f;
			sweepBodyPos = originBodyPos - forward * 0.05f;
		});
	}
	public void SetBodyOriginAndSweepPos()
	{
		originBodyPos = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).localPosition;
		Vector3 forward = parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).forward;
		forward.y = 0f;
		sweepBodyPos = originBodyPos - forward * sweepBodyRange;
	}
	public void SetSweepAnimation(float _animTime, Curling_Player.ActionState _actionState)
	{
		if (!isBrushAnimation)
		{
			isBrushAnimation = true;
			sweepLine.enabled = true;
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), sweepBodyAnchor.localPosition, new Vector3(18.79625f, 0f, 0f), _animTime);
			sweepLine.transform.SetLocalPositionZ(0f);
			LeanTween.moveLocalZ(sweepLine.gameObject, sweepLineRange, _animTime);
			LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
			{
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(18.79625f, 0f, 0f), _animTime);
				LeanTween.moveLocalZ(sweepLine.gameObject, 0f, _animTime);
				switch (_actionState)
				{
				case Curling_Player.ActionState.SWEEP_0:
				case Curling_Player.ActionState.SWEEP_1:
					if (!SingletonCustom<Curling_GameManager>.Instance.GetIsSkip())
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_curling_brush");
					}
					break;
				case Curling_Player.ActionState.HOUSE_SWEEP:
					if (!SingletonCustom<Curling_GameManager>.Instance.GetIsSkip())
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_curling_brush");
						if (!SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().GetIsCpu())
						{
							SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().GetUserType());
						}
					}
					break;
				}
				LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
				{
					isBrushAnimation = false;
					sweepLine.enabled = false;
				});
			});
		}
	}
	public void SetSweepAnimation(float _animTime, bool _isOdd, Curling_Player.ActionState _actionState)
	{
		if (!_isOdd)
		{
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(18.79625f, 0f, 0f), _animTime);
		}
		else
		{
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY), sweepBodyAnchor.localPosition, new Vector3(18.79625f, 0f, 0f), _animTime);
		}
		LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate
		{
			switch (_actionState)
			{
			case Curling_Player.ActionState.SWEEP_0:
			case Curling_Player.ActionState.SWEEP_1:
				SingletonCustom<AudioManager>.Instance.SePlay("se_curling_brush");
				break;
			case Curling_Player.ActionState.HOUSE_SWEEP:
				SingletonCustom<AudioManager>.Instance.SePlay("se_curling_brush");
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().GetUserType());
				break;
			}
		});
	}
	public void SetSweepRunAnimation(float _animTime, bool _isOdd)
	{
		if (!_isOdd)
		{
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.088f, -0.048f, 0.024f), new Vector3(2.022501f, 344.715f, 357.6376f), _animTime);
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.058f, -0.048f, -0.014f), new Vector3(25.3376f, 323.8131f, 0f), _animTime);
		}
		else
		{
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.063f, -0.044f, -0.066f), new Vector3(25.3376f, 344.715f, 357.6376f), _animTime);
			Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.033f, -0.048f, 0.076f), new Vector3(2.022501f, 323.8131f, 0f), _animTime);
		}
	}
	public void SetSweepRunAnimation(float _animTime, bool _isOdd, Curling_Player.ActionState _actionState)
	{
		if (!_isOdd)
		{
			switch (_actionState)
			{
			case Curling_Player.ActionState.SWEEP_0:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.088f, -0.048f, 0.024f), new Vector3(2.022501f, 344.715f, 357.6376f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.058f, -0.048f, -0.014f), new Vector3(25.3376f, 323.8131f, 0f), _animTime);
				break;
			case Curling_Player.ActionState.SWEEP_1:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0.069f), new Vector3(350.1335f, 34.1409f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.058f, -0.048f, -0.057f), new Vector3(25.3376f, 34.1409f, 0f), _animTime);
				break;
			case Curling_Player.ActionState.HOUSE_SWEEP:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0.069f), new Vector3(350.1335f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, -0.057f), new Vector3(25.33761f, 0f, 0f), _animTime);
				break;
			}
		}
		else
		{
			switch (_actionState)
			{
			case Curling_Player.ActionState.SWEEP_0:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.063f, -0.044f, -0.066f), new Vector3(25.3376f, 344.715f, 357.6376f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.033f, -0.048f, 0.076f), new Vector3(2.022501f, 323.8131f, 0f), _animTime);
				break;
			case Curling_Player.ActionState.SWEEP_1:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.06f, -0.057f, -0.045f), new Vector3(25.33762f, 34.14089f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.07f, -0.048f, 0.052f), new Vector3(350.1335f, 34.1409f, 0f), _animTime);
				break;
			case Curling_Player.ActionState.HOUSE_SWEEP:
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, -0.057f), new Vector3(25.33761f, 0f, 0f), _animTime);
				Animation(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0.069f), new Vector3(350.1335f, 0f, 0f), _animTime);
				break;
			}
		}
	}
	public void LeanTweenCancel()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HEAD).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.BODY).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.HIP).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_L).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.SHOULDER_R).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_L).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.ARM_R).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_L).gameObject);
		LeanTween.cancel(parts.Parts.RendererParts(Curling_CharacterParts.BodyPartsList.LEG_R).gameObject);
	}
}
