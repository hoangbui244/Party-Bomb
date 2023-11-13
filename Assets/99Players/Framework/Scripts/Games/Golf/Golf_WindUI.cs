using TMPro;
using UnityEngine;
public class Golf_WindUI : MonoBehaviour
{
	[SerializeField]
	[Header("風速テキスト")]
	private TextMeshPro windSpeedText;
	[SerializeField]
	[Header("風向きオブジェクト")]
	private GameObject windDirObj;
	[SerializeField]
	[Header("風向き矢印メッシュ")]
	private MeshRenderer windArrow;
	public void InitPlay()
	{
		windSpeedText.text = SingletonCustom<Golf_WindManager>.Instance.GetWindSpeed().ToString();
		windArrow.material = SingletonCustom<Golf_WindManager>.Instance.GetWindArrowMaterial();
		windDirObj.transform.SetLocalEulerAnglesZ(SingletonCustom<Golf_WindManager>.Instance.GetWindDir());
	}
}
