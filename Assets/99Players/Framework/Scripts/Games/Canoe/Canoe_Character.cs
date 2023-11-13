using UnityEngine;
public class Canoe_Character : MonoBehaviour
{
	private Canoe_Player player;
	private CharacterStyle style;
	public void Init(Canoe_Player _player)
	{
		player = _player;
		style = GetComponent<CharacterStyle>();
	}
	public void SetStyle(int _userType)
	{
		style.SetGameStyle(GS_Define.GameType.BLOW_AWAY_TANK, _userType);
	}
	public void SetGoalFace(int _rank)
	{
		switch (_rank)
		{
		case 1:
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.HAPPY);
			break;
		case 2:
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.SMILE);
			break;
		case 3:
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.NORMAL);
			break;
		case 4:
			style.SetMainCharacterFaceDiff((int)player.GetUserType(), StyleTextureManager.MainCharacterFaceType.SAD);
			break;
		}
	}
}
