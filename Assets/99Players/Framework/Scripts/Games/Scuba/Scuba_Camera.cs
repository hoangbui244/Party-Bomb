using UnityEngine;
public class Scuba_Camera : MonoBehaviour
{
	private const float POS_MAG = -0.18f;
	[SerializeField]
	private int charaNo;
	private void OnPreCull()
	{
		if (SingletonCustom<Scuba_CharacterManager>.Instance.GetIsCameraFps(charaNo))
		{
			SingletonCustom<Scuba_ItemManager>.Instance.AddLODSize(0f);
		}
		else
		{
			SingletonCustom<Scuba_ItemManager>.Instance.AddLODSize(Mathf.Max(0f, base.transform.localPosition.z * -0.18f));
		}
	}
}
