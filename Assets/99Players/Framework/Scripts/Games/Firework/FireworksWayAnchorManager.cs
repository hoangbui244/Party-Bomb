using UnityEngine;
public class FireworksWayAnchorManager : SingletonCustom<FireworksWayAnchorManager>
{
	[SerializeField]
	[Header("移動判定用アンカ\u30fc配列")]
	private FireworksWayAnchor[] arrayWay;
	public FireworksWayAnchor[] ArrayWay => arrayWay;
}
