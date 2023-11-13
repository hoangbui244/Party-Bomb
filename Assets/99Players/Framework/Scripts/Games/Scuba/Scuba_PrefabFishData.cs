using UnityEngine;
public class Scuba_PrefabFishData : MonoBehaviour
{
	[Header("スコア")]
	public int score = 100;
	[Header("スキンメッシュレンダラ\u30fc")]
	public SkinnedMeshRenderer skinnedMeshRenderer;
	[Header("撮影マ\u30fcクを上に出すときのY座標オフセット")]
	public float foundUiUpperOffset;
	[Header("撮影マ\u30fcクを横に出すときの横方向座標オフセット")]
	public float foundUiRightOffset;
	[Header("撮影範囲の倍率")]
	public float foundRangeMag = 1f;
}
