using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.L10;
using UnityEngine.U2D;
public class Shuriken_PlayerIconUI : DecoratedMonoBehaviour
{
	private static readonly string SinglePlayerControlUserSpriteName = "_screen_you";
	private static readonly string[] ControlUserSpriteNames = new string[11]
	{
		"_screen_1p",
		"_screen_2p",
		"_screen_3p",
		"_screen_4p",
		"_screen_cp1",
		"_screen_cp2",
		"_screen_cp3",
		"_screen_cp4",
		"_screen_cp5",
		"_screen_cp6",
		"_screen_cp7"
	};
	private static readonly string[] PlayerCharacterSpriteNames = new string[8]
	{
		"character_yuto_02",
		"character_hina_02",
		"character_ituki_02",
		"character_souta_02",
		"character_takumi_02",
		"character_rin_02",
		"character_akira_02",
		"character_rui_02"
	};
	[SerializeField]
	[DisplayName("操作ユ\u30fcザ\u30fcアイコン")]
	private SpriteRenderer controlUserIcon;
	[SerializeField]
	[DisplayName("手裏剣用操作ユ\u30fcザ\u30fcアイコン")]
	private SpriteRenderer shurikenControlUserIcon;
	[SerializeField]
	[DisplayName("キャラクタ\u30fcアイコン")]
	private SpriteRenderer playerCharacterIcon;
	[SerializeField]
	[DisplayName("スプライトアトラス")]
	private SpriteAtlas spriteAtlas;
	private bool isMoveOut;
	private int idx;
	public void Initialize(Shuriken_Definition.ControlUser controlUserId)
	{
		string text = ControlUserSpriteNames[(int)controlUserId];
		if (controlUserId == Shuriken_Definition.ControlUser.Player1 && SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			text = SinglePlayerControlUserSpriteName;
			if (Localization.Language == Localization.SupportedLanguage.English)
			{
				text = "en" + text;
			}
			controlUserIcon.transform.SetLocalScale(55f, 55f, 1f);
		}
		controlUserIcon.sprite = spriteAtlas.GetSprite(text, Localization.Language);
		shurikenControlUserIcon.sprite = controlUserIcon.sprite;
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)controlUserId];
		string name = PlayerCharacterSpriteNames[num];
		playerCharacterIcon.sprite = spriteAtlas.GetSprite(name);
		LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
		{
			shurikenControlUserIcon.SetAlpha(_value);
		}).setDelay(4f);
		idx = (int)((controlUserId >= Shuriken_Definition.ControlUser.Cpu1) ? (controlUserId - (4 - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : controlUserId);
	}
	private void Update()
	{
		if (SingletonMonoBehaviour<Shuriken_GameMain>.Instance.RemainingTime <= 10f && !isMoveOut)
		{
			LeanTween.moveLocalY(base.gameObject, base.transform.localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay((float)idx * 0.15f);
			isMoveOut = true;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
