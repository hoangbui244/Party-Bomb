using UnityEngine;
public class HandSeal_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private HandSeal_UserUIData[] userUIDatas;
	[SerializeField]
	[Header("～組目の画像")]
	private SpriteRenderer groupNumberSp;
	private int goalRankingCnt;
	public void Init()
	{
		if (groupNumberSp != null)
		{
			groupNumberSp.gameObject.SetActive(value: false);
		}
		commonGameTime.SetTime(HandSeal_Define.GAME_TIME);
	}
	public void SetUserUIData(HandSeal_Define.UserType[] _userTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i]);
		}
	}
	public void SetUserScore(HandSeal_Define.UserType _userType, int _score)
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
	}
	public void HideControlInfoBalloon()
	{
	}
	public void SetGroupNumberIcon()
	{
		if (!(groupNumberSp == null))
		{
			groupNumberSp.gameObject.SetActive(value: true);
			if (HandSeal_Define.PM.CheckNowGroup1Playing())
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
