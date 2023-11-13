using UnityEngine;
public class GS_SelectCursor : MonoBehaviour
{
	[SerializeField]
	[Header("カ\u30fcソルフレ\u30fcム")]
	private SpriteRenderer cursorFrame;
	[SerializeField]
	[Header("カ\u30fcソルキャラ")]
	private SpriteRenderer cursorCharacter;
	[SerializeField]
	[Header("カ\u30fcソルフレ\u30fcム差分")]
	private string[] cursorFrameName;
	[SerializeField]
	[Header("カ\u30fcソルキャラ差分")]
	private string[] cursorCharacterName;
	private int currentIdx;
	public void Init()
	{
	}
	public void SetAngle(float _angle)
	{
	}
}
