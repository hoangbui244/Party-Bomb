using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_GetScoreUIConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("表示時間")]
	private float showTime = 1f;
	[SerializeField]
	[DisplayName("移動時間")]
	private float moveTime = 0.5f;
	[SerializeField]
	[DisplayName("移動量(Y)")]
	private float moveY = 50f;
	[SerializeField]
	[DisplayName("フェ\u30fcド時間")]
	private float fadeOutTime = 0.5f;
	[SerializeField]
	[DisplayName("キャラクタ\u30fcカラ\u30fcを使用する")]
	private bool useCharacterColor;
	[SerializeField]
	[Hide("useCharacterColor", true)]
	[DisplayName("プレイヤ\u30fcごとの表示カラ\u30fc")]
	private Color[] colors;
	[SerializeField]
	[Hide("useCharacterColor", false)]
	[DisplayName("キャラクタ\u30fcごとの表示カラ\u30fc")]
	private Color[] characterColors;
	public float ShowTime => showTime;
	public float MoveTime => moveTime;
	public float MoveY => moveY;
	public float FadeOutTime => fadeOutTime;
	public bool UseCharacterColor => useCharacterColor;
	public Color[] Colors => colors;
	public Color[] CharacterColors => colors;
}
