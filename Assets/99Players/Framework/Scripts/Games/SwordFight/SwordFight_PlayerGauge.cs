using UnityEngine;
public class SwordFight_PlayerGauge : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private SpriteRenderer spPlayerNo;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer spPlayerIcon;
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer spFrame;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private Sprite[] arrayFrame;
	[SerializeField]
	[Header("ゲ\u30fcジ黄色")]
	private SpriteRenderer gaugeYellow;
	[SerializeField]
	[Header("ゲ\u30fcジ赤色")]
	private SpriteRenderer gaugeRed;
	[SerializeField]
	[Header("プレイヤ\u30fcフレ\u30fcム")]
	private SpriteRenderer[] arrayPlayerFrame;
	[SerializeField]
	[Header("フレ\u30fcムカラ\u30fc")]
	private Color[] arrayPlayerFrameColor;
	private float currentScale;
	public static readonly string[] WORLD_PLAYER_SPRITE_NAMES = new string[9]
	{
		"_common_c_1P",
		"_common_c_2P",
		"_common_c_3P",
		"_common_c_4P",
		"_common_c_cp1",
		"_common_c_cp2",
		"_common_c_cp3",
		"_common_c_cp4",
		"_common_c_cp5"
	};
	private static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
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
	private SwordFight_CharacterScript currentPlayer;
	public void Init(SwordFight_CharacterScript _player)
	{
		currentPlayer = _player;
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_player.PlayerNo];
		spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, WORLD_PLAYER_SPRITE_NAMES[_player.PlayerNo]);
		spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
		arrayPlayerFrame[0].color = arrayPlayerFrameColor[num];
		currentScale = 1f;
		gaugeYellow.transform.SetLocalScaleX(1f);
		LeanTween.cancel(gaugeYellow.gameObject);
		gaugeRed.transform.SetLocalScaleX(1f);
	}
	public void UpdateMethod()
	{
		if (currentScale != currentPlayer.HpScale)
		{
			gaugeYellow.transform.SetLocalScaleX(currentPlayer.HpScale);
			LeanTween.cancel(gaugeYellow.gameObject);
			LeanTween.scaleX(gaugeRed.gameObject, currentPlayer.HpScale, 0.25f).setDelay(0.25f);
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(gaugeYellow.gameObject);
	}
}
