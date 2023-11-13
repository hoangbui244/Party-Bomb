using UnityEngine;
public class Fishing_FishData : MonoBehaviour
{
	[SerializeField]
	[Header("魚の名前")]
	public string fishName;
	[SerializeField]
	[Header("魚の種類")]
	public FishingDefinition.FishType fishType;
	[SerializeField]
	[Header("魚のサイズ")]
	public FishingDefinition.FishSizeType fishSizeType;
	[SerializeField]
	[Header("魚のポイント")]
	public int fishPoint;
}
