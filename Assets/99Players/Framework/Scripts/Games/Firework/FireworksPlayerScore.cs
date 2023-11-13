using UnityEngine;
public class FireworksPlayerScore : MonoBehaviour
{
	[SerializeField]
	[Header("スコア")]
	private SpriteNumbers score;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private SpriteRenderer spPlayerNo;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer spPlayerIcon;
	[SerializeField]
	[Header("チ\u30fcム表記")]
	private SpriteRenderer spTeamIcon;
	private FireworksDefine.TeamType teamType;
	private FireworksPlayer currentPlayer;
	private int currentScore;
	public int CurrentScore => currentScore;
	public void Init(FireworksPlayer _player, FireworksDefine.TeamType _teamType)
	{
		teamType = _teamType;
		currentPlayer = _player;
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)currentPlayer.UserType];
		SetPlayerIcon();
		SetCharacterIcon();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			spTeamIcon.gameObject.SetActive(value: false);
		}
		else
		{
			spTeamIcon.gameObject.SetActive(value: true);
			if (teamType == FireworksDefine.TeamType.TEAM_A)
			{
				spTeamIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_team_A");
			}
			else if (teamType == FireworksDefine.TeamType.TEAM_B)
			{
				spTeamIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_team_B");
			}
		}
		score.Set(0);
	}
	private void SetPlayerIcon()
	{
		switch (currentPlayer.UserType)
		{
		case FireworksDefine.UserType.PLAYER_1:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
			break;
		case FireworksDefine.UserType.PLAYER_2:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
			break;
		case FireworksDefine.UserType.PLAYER_3:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
			break;
		case FireworksDefine.UserType.PLAYER_4:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
			break;
		case FireworksDefine.UserType.CPU_1:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
			break;
		case FireworksDefine.UserType.CPU_2:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
			break;
		case FireworksDefine.UserType.CPU_3:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
			break;
		case FireworksDefine.UserType.CPU_4:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
			break;
		case FireworksDefine.UserType.CPU_5:
			spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
			break;
		}
	}
	private void SetCharacterIcon()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)currentPlayer.UserType])
		{
		case 0:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_0" + 2.ToString());
			break;
		case 1:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_0" + 2.ToString());
			break;
		case 2:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_0" + 2.ToString());
			break;
		case 3:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_0" + 2.ToString());
			break;
		case 4:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_0" + 2.ToString());
			break;
		case 5:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_0" + 2.ToString());
			break;
		case 6:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_0" + 2.ToString());
			break;
		case 7:
			spPlayerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_0" + 2.ToString());
			break;
		}
	}
	public void UpdateMethod()
	{
		if (currentPlayer.Score != currentScore)
		{
			LeanTween.value(currentScore, currentPlayer.Score, 0.5f).setOnUpdate(delegate(float _value)
			{
				score.Set((int)_value);
			});
			currentScore = currentPlayer.Score;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
