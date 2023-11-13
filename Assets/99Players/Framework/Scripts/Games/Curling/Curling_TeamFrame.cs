using UnityEngine;
public class Curling_TeamFrame : MonoBehaviour
{
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer frame;
	[SerializeField]
	[Header("フレ\u30fcム用の画像")]
	private Sprite[] arrayFrameSprite;
	[SerializeField]
	[Header("チ\u30fcムアイコン")]
	private SpriteRenderer teamIcon;
	[SerializeField]
	[Header("チ\u30fcムアイコン用の画像")]
	private Sprite[] arrayTeamIconSprite;
	[SerializeField]
	[Header("チ\u30fcムアイコン用の画像（English）")]
	private Sprite[] arrayTeamIconSprite_EN;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer[] arrayPlayerIcon;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の画像")]
	private Sprite[] arrayPlayerIconSprite;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像")]
	private Sprite playerIconSprite_Single;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像（English）")]
	private Sprite playerIconSprite_Single_EN;
	private const float MULTI_PLAYER_ICON_SCALE = 0.6f;
	private const float SINGLE_PLAYER_ICON_SCALE = 0.7f;
	[SerializeField]
	[Header("石アイコン")]
	private SpriteRenderer[] arrayStoneIcon;
	[SerializeField]
	[Header("石アイコンの画像")]
	private Sprite[] arrayStoneIconSprite;
	[SerializeField]
	[Header("得点用のSpriteNumber")]
	private SpriteNumbers pointSp;
	public void Init(int _teamNo, Curling_Define.UserType _firstThrowUserType, Curling_Define.UserType _secondThrowUserType)
	{
		frame.sprite = arrayFrameSprite[_teamNo];
		teamIcon.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayTeamIconSprite[_teamNo] : arrayTeamIconSprite_EN[_teamNo]);
		if (_firstThrowUserType >= Curling_Define.UserType.CPU_1)
		{
			arrayPlayerIcon[0].sprite = arrayPlayerIconSprite[arrayPlayerIconSprite.Length - 1];
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			arrayPlayerIcon[0].sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerIconSprite_Single : playerIconSprite_Single_EN);
		}
		else
		{
			arrayPlayerIcon[0].sprite = arrayPlayerIconSprite[(int)_firstThrowUserType];
		}
		if (_firstThrowUserType != _secondThrowUserType)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				arrayPlayerIcon[1].sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerIconSprite_Single : playerIconSprite_Single_EN);
			}
			else
			{
				arrayPlayerIcon[1].sprite = arrayPlayerIconSprite[(int)_secondThrowUserType];
			}
		}
		else
		{
			float x = (arrayPlayerIcon[0].transform.localPosition.x + arrayPlayerIcon[1].transform.localPosition.x) / 2f;
			arrayPlayerIcon[0].transform.SetLocalPositionX(x);
			arrayPlayerIcon[1].gameObject.SetActive(value: false);
		}
		for (int i = 0; i < arrayPlayerIcon.Length; i++)
		{
			arrayPlayerIcon[i].transform.localScale = Vector3.one * ((SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2) ? 0.6f : 0.7f);
		}
		for (int j = 0; j < arrayStoneIcon.Length; j++)
		{
			arrayStoneIcon[j].sprite = arrayStoneIconSprite[_teamNo];
			arrayStoneIcon[j].gameObject.SetActive(value: true);
		}
	}
	public void InitPlay(int _teamNo)
	{
		for (int i = 0; i < arrayStoneIcon.Length; i++)
		{
			arrayStoneIcon[i].sprite = arrayStoneIconSprite[_teamNo];
			arrayStoneIcon[i].gameObject.SetActive(value: true);
		}
	}
	public void SetThrowStoneIcon(int _throwCnt)
	{
		arrayStoneIcon[Curling_Define.THROW_CNT - _throwCnt].gameObject.SetActive(value: false);
	}
	public void SetPoint(int _point)
	{
		pointSp.Set(_point);
	}
}
