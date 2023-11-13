using UnityEngine;
public class Golf_Character : MonoBehaviour
{
	private Golf_Player player;
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("クラブ")]
	private MeshRenderer club;
	public void Init(Golf_Player _player)
	{
		player = _player;
	}
	public void SetStyle(int _userType)
	{
		style.SetGameStyle(GS_Define.GameType.BOMB_ROULETTE, _userType);
	}
	public void SetClubMaterial(Material _mat)
	{
		club.material = _mat;
	}
}
