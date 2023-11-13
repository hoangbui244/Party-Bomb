using UnityEngine;
namespace BeachSoccer
{
	public class FormationData : MonoBehaviour
	{
		public new string name;
		public string info;
		public GameDataParams.Rarity rarity;
		public PlayerData[] playerData = new PlayerData[11];
	}
}
