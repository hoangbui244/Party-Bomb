using UnityEngine;
using BeachSoccer;
public class BeachVolley_FormationData : MonoBehaviour {
    public new string name;
    public string info;
    public BeachVolley_Define.Rarity rarity;
    public PlayerData[] playerData = new PlayerData[BeachVolley_Define.TEAM_MEMBER_NUM_MAX];
}
