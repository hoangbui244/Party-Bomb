using UnityEngine;
public class LegendarySword_PlayerIcon : MonoBehaviour
{
	[SerializeField]
	[Header("対象となるプレイヤ\u30fc")]
	private LegendarySword_Player[] player;
	[SerializeField]
	[Header("使用するアイコンのリスト")]
	private Sprite[] icons;
	[SerializeField]
	[Header("使用する「あなた」アイコン(JP)")]
	private Sprite icon_you;
	[SerializeField]
	[Header("使用する「あなた」アイコン(EN)")]
	private Sprite icon_you_en;
	[SerializeField]
	[Header("使用するアイコンのレンダラ\u30fc")]
	private SpriteRenderer[] iconRenderer;
	[SerializeField]
	[Header("アイコンの元\u3005の位置")]
	private Transform[] originPos;
	public void SetIcon()
	{
		for (int i = 0; i < iconRenderer.Length; i++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && player[i].UserType == LegendarySword_Define.UserType.PLAYER_1)
			{
				iconRenderer[i].sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? icon_you : icon_you_en);
			}
			else
			{
				iconRenderer[i].sprite = icons[(int)player[i].UserType];
			}
			Color color = iconRenderer[i].color;
			color.a = 0f;
			iconRenderer[i].color = color;
		}
	}
	public void ShowIcon()
	{
		for (int i = 0; i < iconRenderer.Length; i++)
		{
			Fade(iconRenderer[i], 1f, 0.5f);
			iconRenderer[i].drawMode = SpriteDrawMode.Simple;
		}
	}
	public void InvisibleIcon()
	{
		for (int i = 0; i < iconRenderer.Length; i++)
		{
			Fade(iconRenderer[i], 0f, 0.5f);
		}
	}
	public void IconPoschange(bool change)
	{
		if (change)
		{
			iconRenderer[0].transform.localPosition = originPos[1].localPosition;
			iconRenderer[1].transform.localPosition = originPos[0].localPosition;
		}
		else
		{
			iconRenderer[0].transform.localPosition = originPos[0].localPosition;
			iconRenderer[1].transform.localPosition = originPos[1].localPosition;
		}
	}
	private void Fade(SpriteRenderer _sprite, float _alpha, float _time = -1f)
	{
		if (_time < 0f)
		{
			_time = 0.5f;
		}
		LeanTween.cancel(_sprite.gameObject);
		LeanTween.value(_sprite.gameObject, _sprite.color.a, _alpha, _time).setOnUpdate(delegate(float _value)
		{
			_sprite.SetAlpha(_value);
		});
	}
}
