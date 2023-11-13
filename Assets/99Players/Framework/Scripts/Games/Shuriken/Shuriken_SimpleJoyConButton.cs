using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.U2D;
public class Shuriken_SimpleJoyConButton : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("レンダラ\u30fc")]
	private SpriteRenderer buttonIcon;
	[SerializeField]
	[DisplayName("シングル用スプライト名")]
	private string fullKeySpriteName;
	[SerializeField]
	[DisplayName("マルチ用スプライト名")]
	private string joyLeftOrRightSpriteName;
	[SerializeField]
	[DisplayName("スプライトアトラス")]
	private SpriteAtlas spriteAtlas;
	private void OnEnable()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			buttonIcon.sprite = spriteAtlas.GetSprite(fullKeySpriteName);
		}
		else
		{
			buttonIcon.sprite = spriteAtlas.GetSprite(joyLeftOrRightSpriteName);
		}
	}
}
