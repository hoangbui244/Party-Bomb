using UnityEngine;
public class Golf_PlayerUI : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("キャラクタ\u30fcアイコン")]
	private SpriteRenderer characterIcon;
	private readonly string[] CHARACTER_SPRITE_NAME = new string[8]
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
	[Header("ポイント")]
	private SpriteNumbers point;
	public void Init(int _userType)
	{
		SetPlayerIcon(_userType);
		SetCharacterIcon(_userType);
		point.Set(0);
	}
	private void SetPlayerIcon(int _userType)
	{
		if (_userType < 4)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_userType + 1).ToString() + "p");
			}
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_userType - 4 + 1).ToString());
		}
	}
	private void SetCharacterIcon(int _userType)
	{
		characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAME[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_userType]]);
	}
	public void SetPoint(int _point)
	{
		point.Set(_point);
	}
}
