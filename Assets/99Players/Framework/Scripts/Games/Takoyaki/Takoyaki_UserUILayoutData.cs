using UnityEngine;
public class Takoyaki_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private Takoyaki_UserUIData[] userUIDatas;
	[SerializeField]
	[Header("～組目の画像")]
	private SpriteRenderer groupNumberSp;
	public void Init()
	{
		if (groupNumberSp != null)
		{
			groupNumberSp.gameObject.SetActive(value: false);
		}
		commonGameTime.SetTime(Takoyaki_Define.GAME_TIME);
	}
	public void SetUserUIData(Takoyaki_Define.UserType[] _userTypeArray, Takoyaki_Define.TeamType[] _teamTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i], _teamTypeArray[i]);
		}
	}
	public void SetUserScore(Takoyaki_Define.UserType _userType, int _score)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			if (userUIDatas[i].UserType == _userType)
			{
				userUIDatas[i].SetScore(_score);
			}
		}
	}
	public void ShowControlInfoBalloon()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].FadeProcess_ControlInfomationUI(_fadeIn: true);
		}
	}
	public void HideControlInfoBalloon()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].FadeProcess_ControlInfomationUI(_fadeIn: false);
		}
	}
	public void SetGroupNumberIcon()
	{
		if (!(groupNumberSp == null))
		{
			groupNumberSp.gameObject.SetActive(value: true);
			if (Takoyaki_Define.PM.CheckNowGroup1Playing())
			{
				groupNumberSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_1group");
			}
			else
			{
				groupNumberSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
			}
		}
	}
	public void SetGameTime(float _time)
	{
		commonGameTime.SetTime(_time);
	}
}
