using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
public class Fishing_UserUI : MonoBehaviour
{
	[Serializable]
	public struct UserUI
	{
		[Header("ユ\u30fcザ\u30fcUIアンカ\u30fc")]
		public GameObject userUIAnchor;
		[Header("下敷き")]
		public SpriteRenderer underlay;
		[Header("キャラクタ\u30fcアイコン")]
		public SpriteRenderer characterIcon;
		[Header("ポイント数")]
		public SpriteNumbers pointNumber;
		[Header("ポイント文字")]
		public SpriteRenderer pointText;
		[Header("魚アイコン")]
		public SpriteRenderer fishIcon;
		[Header("×")]
		public SpriteRenderer cross;
		[Header("釣った数")]
		public SpriteNumbers countNumber;
	}
	private static readonly string[] CharacterSpriteNames = new string[8]
	{
		"character_yuto_0",
		"character_hina_0",
		"character_ituki_0",
		"character_souta_0",
		"character_takumi_0",
		"character_rin_0",
		"character_akira_0",
		"character_rui_0"
	};
	[FormerlySerializedAs("userUIData")]
	[FormerlySerializedAs("userUIData_Single")]
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIデ\u30fcタ(チ\u30fcム所属なし)")]
	private UserUI userUI;
	private SpriteAtlas commonSpriteAtlas;
	private int userDataNo = -1;
	private FishingDefinition.User uiUser;
	public void Init(int userNo, FishingDefinition.User user)
	{
		commonSpriteAtlas = SingletonCustom<Fishing_SpriteAtlasCache>.Instance.GetCommonAtlas();
		userDataNo = userNo;
		uiUser = user;
		userUI.userUIAnchor.SetActive(value: true);
		SetUserIcon();
		SetFishCount(0);
		SetPoint(0);
	}
	public void SetUserIcon()
	{
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)uiUser];
		userUI.characterIcon.sprite = commonSpriteAtlas.GetSprite(CharacterSpriteNames[num] + 2.ToString());
	}
	public void SetPoint(int point)
	{
		userUI.pointNumber.Set(point);
	}
	public void SetFishCount(int count)
	{
		userUI.countNumber.Set(count);
	}
}
