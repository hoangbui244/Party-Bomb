using UnityEngine;
public class BeachFlag_UserUILayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time commonGameTime;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIのデ\u30fcタ")]
	private BeachFlag_UserUIData[] userUIDatas;
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
		if (commonGameTime != null)
		{
			commonGameTime.SetTime(0f);
		}
	}
	public void SetUserUIData(BeachFlag_Define.UserType[] _userTypeArray)
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].Init(_userTypeArray[i]);
		}
	}
	public void SetUserScore(BeachFlag_Define.UserType _userType, int _score)
	{
	}
	public void ShowControlInfoBalloon()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].FadeProcess_ControlInfomationUI(_fadeIn: true);
			userUIDatas[i].FadeOutPlayerIcon();
		}
	}
	public void HideControlInfoBalloon()
	{
	}
	public void CourseOutWarning(int _player)
	{
		if (_player <= userUIDatas.Length)
		{
			userUIDatas[_player].CourseOutWarning();
		}
	}
	public void SetGameTime(float _time)
	{
		commonGameTime.SetTime(_time);
	}
}
