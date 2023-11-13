using UnityEngine;
public class Biathlon_OperationUI : MonoBehaviour
{
	public void Init(int no)
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[no][0] >= 4)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
