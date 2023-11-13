using System;
using TMPro;
using UnityEngine;
public class Bowling_BallThrowUI : MonoBehaviour
{
	[Serializable]
	public struct BalloonTextData
	{
		[Header("吹き出しの下敷き画像")]
		public SpriteRenderer balloonUnderlay;
		[Header("吹き出しのボタン画像")]
		public SpriteRenderer balloonButton;
		[Header("吹き出しの文字")]
		public TextMeshPro balloonText;
		public void Init()
		{
			balloonUnderlay.SetAlpha(0f);
			balloonButton.SetAlpha(0f);
			balloonText.SetAlpha(0f);
		}
	}
	[Serializable]
	public struct BalloonTextData_Another
	{
		[Header("吹き出しの下敷き画像")]
		public SpriteRenderer balloonUnderlay;
		[Header("吹き出しのボタン画像_X")]
		public SpriteRenderer balloonButton_X;
		[Header("吹き出しのボタン画像_Y")]
		public SpriteRenderer balloonButton_Y;
		[Header("吹き出しのボタン画像_A")]
		public SpriteRenderer balloonButton_A;
		[Header("吹き出しの文字")]
		public TextMeshPro balloonText;
		public void Init()
		{
			balloonUnderlay.SetAlpha(0f);
			balloonButton_X.SetAlpha(0f);
			balloonButton_Y.SetAlpha(0f);
			balloonButton_A.SetAlpha(0f);
			balloonText.SetAlpha(0f);
		}
	}
	[SerializeField]
	[Header("投球基準")]
	private GameObject throwBase;
	[SerializeField]
	[Header("投球矢印")]
	private SpriteRenderer throwArrow;
	[SerializeField]
	[Header("プレイヤ\u30fc番号アンカ\u30fc")]
	private GameObject playerNoAnchor;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン画像")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("プレイヤ\u30fc番号矢印")]
	private SpriteRenderer playerNoArrow;
	[SerializeField]
	[Header("キャラアイコン画像")]
	private SpriteRenderer charaIconSprite;
	[SerializeField]
	[Header("キャラアイコンフレ\u30fcム画像")]
	private SpriteRenderer charaIconFrame;
	[SerializeField]
	[Header("キャラアイコンフレ\u30fcム下敷き画像")]
	private SpriteRenderer charaIconFrameUnderlay;
	[SerializeField]
	[Header("投球準備時の吹き出しデ\u30fcタ")]
	private BalloonTextData throwReadyBalloonTextData;
	[SerializeField]
	[Header("投球方向決定時の吹き出しデ\u30fcタ")]
	private BalloonTextData throwDirectionBalloonTextData;
	[SerializeField]
	[Header("投球時の吹き出しデ\u30fcタ")]
	private BalloonTextData_Another throwBalloonTextData;
	[SerializeField]
	[Header("スキップの吹き出しデ\u30fcタ")]
	private BalloonTextData skipBalloonTextData;
	private const float PLAYER_NO_MOVE_DISTANCE = 15f;
	private const float PLAYER_NO_MOVE_TIME = 1f;
	private const float PLAYER_NO_FADE_TIME = 6f;
	private const float BALL_ARROW_SCALE_MIN = 0.5f;
	private const float BALL_ARROW_SCALE_MAX = 0.75f;
	private const float BALL_ARROW_POS_OFFSET = 0f;
	private float playerNoDefLocalPos;
	private bool isThrowInfomationEnd;
	private const float DEF_CHARACTER_ICON_POS_X = -85f;
	public void Init()
	{
		throwBase.gameObject.SetActive(value: false);
		throwArrow.gameObject.SetActive(value: false);
		playerNoDefLocalPos = playerNoAnchor.transform.localPosition.y;
		throwArrow.color = new Color(1f, 1f, 1f, 1f);
		LeanTween.moveLocalY(playerNoAnchor, 15f, 1f).setEaseOutExpo().setLoopPingPong();
		throwDirectionBalloonTextData.Init();
		throwReadyBalloonTextData.Init();
		throwBalloonTextData.Init();
		skipBalloonTextData.Init();
	}
	private void UpdatePlayerNoAnimation()
	{
		if (Bowling_Define.MPM.GetNowThrowUser().OperationState == Bowling_Define.OperationState.VECTOR_SELECT)
		{
			playerNoArrow.SetAlpha(Mathf.Clamp(playerNoArrow.color.a - Time.deltaTime * 6f, 0f, 1f));
			playerIcon.SetAlpha(Mathf.Clamp(playerIcon.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconSprite.SetAlpha(Mathf.Clamp(charaIconSprite.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconFrame.SetAlpha(Mathf.Clamp(charaIconFrame.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconFrameUnderlay.SetAlpha(Mathf.Clamp(charaIconFrameUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
			if (Bowling_Define.MPM.NowThrowUserType >= Bowling_Define.UserType.CPU_1)
			{
				skipBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
			}
			else
			{
				if (isThrowInfomationEnd)
				{
					return;
				}
				throwReadyBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwReadyBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwReadyBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
				if (throwArrow.gameObject.activeSelf)
				{
					throwDirectionBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwDirectionBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwDirectionBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonUnderlay.color.a + Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonText.color.a + Time.deltaTime * 6f, 0f, 1f));
					if (Bowling_Define.MPM.GetNowThrowUser().gsThrowData.throwType == Bowling_Define.ThrowType.STRAIGHT)
					{
						throwBalloonTextData.balloonButton_X.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_X.color.a + Time.deltaTime * 6f, 0f, 1f));
					}
					else if (Bowling_Define.MPM.GetNowThrowUser().gsThrowData.throwType == Bowling_Define.ThrowType.RIGHT_S)
					{
						throwBalloonTextData.balloonButton_Y.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_Y.color.a + Time.deltaTime * 6f, 0f, 1f));
					}
					else if (Bowling_Define.MPM.GetNowThrowUser().gsThrowData.throwType == Bowling_Define.ThrowType.LEFT_S)
					{
						throwBalloonTextData.balloonButton_A.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_A.color.a + Time.deltaTime * 6f, 0f, 1f));
					}
				}
				else
				{
					throwDirectionBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonUnderlay.color.a + Time.deltaTime * 6f, 0f, 1f));
					throwDirectionBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonButton.color.a + Time.deltaTime * 6f, 0f, 1f));
					throwDirectionBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonText.color.a + Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonButton_X.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_X.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonButton_Y.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_Y.color.a - Time.deltaTime * 6f, 0f, 1f));
					throwBalloonTextData.balloonButton_A.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_A.color.a - Time.deltaTime * 6f, 0f, 1f));
				}
			}
		}
		else if (Bowling_Define.MPM.GetNowThrowUser().OperationState == Bowling_Define.OperationState.BALL_MOVE)
		{
			playerNoArrow.SetAlpha(Mathf.Clamp(playerNoArrow.color.a + Time.deltaTime * 6f, 0f, 1f));
			playerIcon.SetAlpha(Mathf.Clamp(playerIcon.color.a + Time.deltaTime * 6f, 0f, 1f));
			charaIconSprite.SetAlpha(Mathf.Clamp(charaIconSprite.color.a + Time.deltaTime * 6f, 0f, 1f));
			charaIconFrame.SetAlpha(Mathf.Clamp(charaIconFrame.color.a + Time.deltaTime * 6f, 0f, 1f));
			charaIconFrameUnderlay.SetAlpha(Mathf.Clamp(charaIconFrameUnderlay.color.a + Time.deltaTime * 6f, 0f, 1f));
			if (Bowling_Define.MPM.NowThrowUserType >= Bowling_Define.UserType.CPU_1 && Bowling_Define.MPM.GetNowThrowUser().OperationState == Bowling_Define.OperationState.BALL_MOVE)
			{
				skipBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonUnderlay.color.a + Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonButton.color.a + Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonText.color.a + Time.deltaTime * 6f, 0f, 1f));
			}
			else if (!isThrowInfomationEnd)
			{
				throwReadyBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonUnderlay.color.a + Time.deltaTime * 6f, 0f, 1f));
				throwReadyBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonButton.color.a + Time.deltaTime * 6f, 0f, 1f));
				throwReadyBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonText.color.a + Time.deltaTime * 6f, 0f, 1f));
				throwDirectionBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwDirectionBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwDirectionBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwBalloonTextData.balloonButton_X.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_X.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwBalloonTextData.balloonButton_Y.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_Y.color.a - Time.deltaTime * 6f, 0f, 1f));
				throwBalloonTextData.balloonButton_A.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_A.color.a - Time.deltaTime * 6f, 0f, 1f));
			}
		}
		else
		{
			if (Bowling_Define.MPM.GetNowThrowUser().OperationState != Bowling_Define.OperationState.THROW)
			{
				return;
			}
			playerNoArrow.SetAlpha(Mathf.Clamp(playerNoArrow.color.a - Time.deltaTime * 6f, 0f, 1f));
			playerIcon.SetAlpha(Mathf.Clamp(playerIcon.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconSprite.SetAlpha(Mathf.Clamp(charaIconSprite.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconFrame.SetAlpha(Mathf.Clamp(charaIconFrame.color.a - Time.deltaTime * 6f, 0f, 1f));
			charaIconFrameUnderlay.SetAlpha(Mathf.Clamp(charaIconFrameUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
			if (Bowling_Define.MPM.NowThrowUserType >= Bowling_Define.UserType.CPU_1)
			{
				skipBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
				skipBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(skipBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
				return;
			}
			throwReadyBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwReadyBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwReadyBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwReadyBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwDirectionBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwDirectionBalloonTextData.balloonButton.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonButton.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwDirectionBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwDirectionBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwBalloonTextData.balloonUnderlay.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonUnderlay.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwBalloonTextData.balloonText.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonText.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwBalloonTextData.balloonButton_X.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_X.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwBalloonTextData.balloonButton_Y.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_Y.color.a - Time.deltaTime * 6f, 0f, 1f));
			throwBalloonTextData.balloonButton_A.SetAlpha(Mathf.Clamp(throwBalloonTextData.balloonButton_A.color.a - Time.deltaTime * 6f, 0f, 1f));
			if (!isThrowInfomationEnd)
			{
				isThrowInfomationEnd = true;
			}
		}
	}
	private void ResetPlayerNoAnimation()
	{
		UnityEngine.Debug.Log(base.gameObject.name + "：プレイヤ\u30fc番号：" + Bowling_Define.MPM.NowThrowUserType.ToString());
		SetArrowColor(Bowling_Define.MPM.NowThrowUserType);
		SetCharacterIcon(Bowling_Define.MPM.NowThrowUserType);
		SetPlayerIcon(Bowling_Define.MPM.NowThrowUserType);
		playerNoAnchor.transform.SetLocalPositionY(playerNoDefLocalPos);
	}
	public void UpdateMethod()
	{
		UpdatePlayerNoAnimation();
	}
	public void SetActiveThrowArrow(bool _active)
	{
		throwBase.gameObject.SetActive(_active);
		if (_active)
		{
			ResetPlayerNoAnimation();
		}
	}
	public void SetActiveBallArrow(bool _value)
	{
		throwArrow.gameObject.SetActive(_value);
	}
	public void BallArrowSetting(float _nowLength, Vector3 _touchDir, float _MaxLength, float _minLength)
	{
		throwArrow.transform.localScale = new Vector3(0.5f, 0.25f / (_MaxLength - _minLength) * _nowLength + 0.25f, 0.5f);
		throwArrow.transform.parent.rotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(_touchDir.y, _touchDir.x) + 90f);
	}
	public void SetThrowStartUI()
	{
		throwArrow.transform.SetLocalPositionY(0f);
	}
	public void SetThrowArrow(Bowling_Define.ThrowType _throwType)
	{
		switch (_throwType)
		{
		case Bowling_Define.ThrowType.STRAIGHT:
			throwArrow.transform.localScale = new Vector3(0.5f, 0.75f, 0.5f);
			throwArrow.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "throwArrow_0");
			throwArrow.flipX = false;
			break;
		case Bowling_Define.ThrowType.LEFT_S:
			throwArrow.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			throwArrow.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "throwArrow_2");
			throwArrow.flipX = false;
			break;
		case Bowling_Define.ThrowType.RIGHT_S:
			throwArrow.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			throwArrow.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "throwArrow_2");
			throwArrow.flipX = true;
			break;
		}
	}
	public float GetLaneLengthToScreen()
	{
		Vector3 vector = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().WorldToScreenPoint(throwArrow.transform.position);
		return Mathf.Abs(Bowling_Define.MSM.StageCamera.WorldToScreenPoint(Bowling_Define.MSM.LaneEndPos).y - vector.y);
	}
	private void SetArrowColor(Bowling_Define.UserType _userType)
	{
		playerNoArrow.color = Bowling_Define.GetUserColor(_userType);
		playerNoArrow.SetAlpha(0f);
	}
	private void SetCharacterIcon(Bowling_Define.UserType _userType)
	{
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType])
		{
		case 0:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_0" + 2.ToString());
			break;
		case 1:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_0" + 2.ToString());
			break;
		case 2:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_0" + 2.ToString());
			break;
		case 3:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_0" + 2.ToString());
			break;
		case 4:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_0" + 2.ToString());
			break;
		case 5:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_0" + 2.ToString());
			break;
		case 6:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_0" + 2.ToString());
			break;
		case 7:
			charaIconSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_0" + 2.ToString());
			break;
		}
		charaIconFrameUnderlay.color = Bowling_Define.GetUserColor(_userType);
	}
	private void SetPlayerIcon(Bowling_Define.UserType _userType)
	{
		if (_userType <= Bowling_Define.UserType.PLAYER_4)
		{
			playerIcon.gameObject.SetActive(value: true);
			charaIconFrame.transform.SetLocalPositionX(-85f);
			if (Bowling_Define.PLAYER_NUM == 1)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
				return;
			}
			switch (_userType)
			{
			case Bowling_Define.UserType.PLAYER_1:
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_1p");
				break;
			case Bowling_Define.UserType.PLAYER_2:
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
				break;
			case Bowling_Define.UserType.PLAYER_3:
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
				break;
			case Bowling_Define.UserType.PLAYER_4:
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
				break;
			}
			return;
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			playerIcon.gameObject.SetActive(value: false);
			charaIconFrame.transform.SetLocalPositionX(0f);
		}
		switch (_userType)
		{
		case Bowling_Define.UserType.CPU_1:
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
			break;
		case Bowling_Define.UserType.CPU_2:
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
			break;
		case Bowling_Define.UserType.CPU_3:
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
			break;
		case Bowling_Define.UserType.CPU_4:
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
			break;
		case Bowling_Define.UserType.CPU_5:
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
			break;
		}
	}
}
