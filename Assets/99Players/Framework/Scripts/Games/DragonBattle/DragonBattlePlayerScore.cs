using TMPro;
using UnityEngine;
public class DragonBattlePlayerScore : MonoBehaviour
{
	[SerializeField]
	[Header("スコアテキスト")]
	private TextMeshPro textScore;
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
	private DragonBattlePlayer currentPlayer;
	private int currentScore;
	public int CurrentScore => currentScore;
	public void Init(int _idx)
	{
		currentPlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(_idx);
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[currentPlayer.IsCpu ? (4 + (_idx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : _idx];
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && _idx == 0)
		{
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_you");
		}
		else
		{
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, WORLD_PLAYER_SPRITE_NAMES[currentPlayer.IsCpu ? (4 + (_idx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : _idx]);
		}
		spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
		spFrame.sprite = arrayFrame[num];
		textScore.SetText("0");
	}
	public void UpdateMethod()
	{
		if (currentPlayer.Score != currentScore)
		{
			LeanTween.value(currentScore, currentPlayer.Score, 0.5f).setOnUpdate(delegate(float _value)
			{
				textScore.SetText(((int)_value).ToString());
			});
			currentScore = currentPlayer.Score;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
