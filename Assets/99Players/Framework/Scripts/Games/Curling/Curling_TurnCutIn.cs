using System;
using UnityEngine;
public class Curling_TurnCutIn : MonoBehaviour
{
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer frame;
	private Vector3 originFrameScale;
	[SerializeField]
	[Header("チ\u30fcム")]
	private SpriteRenderer teamSpriteRenderer;
	[SerializeField]
	[Header("チ\u30fcム用の画像")]
	private Sprite[] arrayTeamSprite;
	[SerializeField]
	[Header("チ\u30fcム用の画像（English）")]
	private Sprite[] arrayTeamSprite_EN;
	[SerializeField]
	[Header("投げるプレイヤ\u30fcアイコン")]
	private SpriteRenderer throwPlayerIcon;
	[SerializeField]
	[Header("ハウス付近でこするプレイヤ\u30fcアイコン")]
	private SpriteRenderer houseSweepPlayerIcon;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の画像")]
	private Sprite[] arrayPlayerSprite;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像")]
	private Sprite playerSprite_Single;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像（English）")]
	private Sprite playerSprite_Single_EN;
	[SerializeField]
	[Header("タ\u30fcンカットインの開閉時間")]
	private float TURN_CUT_IN_FADE_TIME;
	[SerializeField]
	[Header("タ\u30fcンカットインの表示時間")]
	private float TURN_CUT_IN_VIEW_TIME;
	public void Init()
	{
		originFrameScale = frame.transform.localScale;
		frame.transform.localScale = Vector3.zero;
	}
	public void ShowTurnCutIn(Action _callBack)
	{
		Curling_GameManager.Team turnTeam = SingletonCustom<Curling_GameManager>.Instance.GetTurnTeam();
		Curling_Define.UserType userType = SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().GetUserType();
		Curling_Define.UserType userType2 = SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer().GetUserType();
		frame.transform.localScale = Vector3.zero;
		teamSpriteRenderer.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayTeamSprite[(int)turnTeam] : arrayTeamSprite_EN[(int)turnTeam]);
		if (userType >= Curling_Define.UserType.CPU_1)
		{
			throwPlayerIcon.sprite = arrayPlayerSprite[arrayPlayerSprite.Length - 1];
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			throwPlayerIcon.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerSprite_Single : playerSprite_Single_EN);
		}
		else
		{
			throwPlayerIcon.sprite = arrayPlayerSprite[(int)userType];
		}
		if (userType >= Curling_Define.UserType.CPU_1)
		{
			houseSweepPlayerIcon.sprite = arrayPlayerSprite[arrayPlayerSprite.Length - 1];
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			houseSweepPlayerIcon.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerSprite_Single : playerSprite_Single_EN);
		}
		else
		{
			houseSweepPlayerIcon.sprite = arrayPlayerSprite[(int)userType2];
		}
		LeanTween.scale(frame.gameObject, originFrameScale, TURN_CUT_IN_FADE_TIME).setEaseOutBack();
		LeanTween.delayedCall(base.gameObject, TURN_CUT_IN_FADE_TIME, (Action)delegate
		{
			LeanTween.delayedCall(base.gameObject, TURN_CUT_IN_VIEW_TIME, (Action)delegate
			{
				LeanTween.scale(frame.gameObject, Vector3.zero, TURN_CUT_IN_FADE_TIME).setEaseInBack();
				LeanTween.delayedCall(base.gameObject, TURN_CUT_IN_FADE_TIME, (Action)delegate
				{
					_callBack();
				});
			});
		});
	}
	public void SkipTurnCutIn()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(frame.gameObject);
		frame.gameObject.transform.localScale = Vector3.zero;
	}
}
