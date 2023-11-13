using UnityEngine;
public class FireworksWayAnchor : MonoBehaviour
{
	[SerializeField]
	[Header("番号")]
	public int no;
	[SerializeField]
	[Header("移動可能先")]
	private FireworksWayAnchor[] arrayConnectWay;
	public FireworksWayAnchor[] ConnectWay => arrayConnectWay;
}
