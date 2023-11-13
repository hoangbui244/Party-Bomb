using UnityEngine;
public class Biathlon_PlayerIcon : MonoBehaviour
{
	private static readonly string[] IconSpriteNames = new string[7]
	{
		"_screen_1p",
		"_screen_2p",
		"_screen_3p",
		"_screen_4p",
		"_screen_cp1",
		"_screen_cp2",
		"_screen_cp3"
	};
	[SerializeField]
	private SpriteRenderer icon;
	public void Init(int no)
	{
		int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[no][0];
		icon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, IconSpriteNames[num]);
	}
}
