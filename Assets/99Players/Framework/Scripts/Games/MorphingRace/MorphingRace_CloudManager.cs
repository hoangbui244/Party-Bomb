using UnityEngine;
public class MorphingRace_CloudManager : SingletonCustom<MorphingRace_CloudManager>
{
	[SerializeField]
	[Header("雲配列")]
	private GameObject[] arrayCloud;
	[SerializeField]
	[Header("雲を表示する高さ")]
	private float cloudHeight;
	public void Init()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			arrayCloud[i].SetActive(value: false);
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(j))
			{
				arrayCloud[j].gameObject.layer = LayerMask.NameToLayer("Collision_Obj_" + (j + 1).ToString());
				arrayCloud[j].transform.position = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetPlayer(j).transform.position;
				arrayCloud[j].transform.SetLocalPositionY(cloudHeight);
				arrayCloud[j].SetActive(value: true);
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(i))
			{
				arrayCloud[i].transform.SetPositionZ(SingletonCustom<MorphingRace_PlayerManager>.Instance.GetPlayer(i).transform.position.z);
			}
		}
	}
}
