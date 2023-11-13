using UnityEngine;
public class Canoe_CloudManager : SingletonCustom<Canoe_CloudManager>
{
	[SerializeField]
	[Header("雲配列")]
	private GameObject[] arrayCloud;
	private Vector3[] arrayDiffPos;
	public void Init()
	{
		arrayDiffPos = new Vector3[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayCloud[i].SetActive(value: false);
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(j))
			{
				arrayCloud[j].gameObject.layer = LayerMask.NameToLayer("Collision_Obj_" + (j + 1).ToString());
				arrayDiffPos[j] = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(j).transform.position - arrayCloud[j].transform.position;
				arrayCloud[j].SetActive(value: true);
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(i))
			{
				Canoe_Player player = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i);
				Vector3 position = arrayCloud[i].transform.position;
				position.x = player.transform.position.x - arrayDiffPos[i].x;
				position.z = player.transform.position.z - arrayDiffPos[i].z;
				arrayCloud[i].transform.position = position;
			}
		}
	}
}
