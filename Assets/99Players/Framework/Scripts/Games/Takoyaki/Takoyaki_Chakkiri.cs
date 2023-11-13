using UnityEngine;
public class Takoyaki_Chakkiri : MonoBehaviour
{
	[SerializeField]
	[Header("チャッキリを置く場所のアンカ\u30fc")]
	private Transform chakkiriPlacementAnchor;
	[SerializeField]
	[Header("座標オフセットアンカ\u30fc")]
	private Transform positionOffsetAnchor;
	[SerializeField]
	[Header("回転オフセットアンカ\u30fc")]
	private Transform rotateOffsetAnchor;
	[SerializeField]
	[Header("生地の液体モデル")]
	private GameObject butterLiquidModel;
	private readonly Vector3 CHAKKIRI_DEF_POS = Vector3.zero;
	private readonly Vector3 CHAKKIRI_POUR_POS = new Vector3(0f, 0.075f, 0.1115f);
	private readonly float CHAKKIRI_DEF_ROT_X;
	private readonly float CHAKKIRI_POUR_ROT_Y = 180f;
	private readonly float CHAKKIRI_POUR_ROT_X_END = 45f;
	private bool duringChakkiriAnimation;
	public readonly float CHAKKIRI_POUR_ROTATE_TIME = 0.2f;
	public readonly float BUTTER_TRAIL_POUR_TIME = 0.25f;
	public bool DuringChakkiriAnimation => duringChakkiriAnimation;
	public void Init()
	{
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			chakkiriPlacementAnchor.SetLocalPosition(0.25f, 0f, 0.35f);
		}
		else
		{
			chakkiriPlacementAnchor.SetLocalPosition(0.25f, 0f, 0.35f);
		}
		base.transform.position = chakkiriPlacementAnchor.position;
		base.transform.eulerAngles = chakkiriPlacementAnchor.eulerAngles;
		positionOffsetAnchor.localPosition = CHAKKIRI_DEF_POS;
		rotateOffsetAnchor.SetLocalEulerAnglesX(CHAKKIRI_DEF_ROT_X);
		butterLiquidModel.SetActive(value: false);
	}
	public void PlayChakkiriAnimation(Vector3 _butterBurriedPoint)
	{
		if (!duringChakkiriAnimation)
		{
			duringChakkiriAnimation = true;
			base.transform.position = _butterBurriedPoint;
			base.transform.localEulerAngles = Vector3.zero;
			positionOffsetAnchor.localPosition = CHAKKIRI_POUR_POS;
			rotateOffsetAnchor.SetLocalEulerAnglesY(CHAKKIRI_POUR_ROT_Y);
			rotateOffsetAnchor.SetLocalEulerAnglesX(CHAKKIRI_POUR_ROT_X_END);
			butterLiquidModel.SetActive(value: true);
		}
	}
	public void StopChakkiriAnimation()
	{
		duringChakkiriAnimation = false;
		base.transform.position = chakkiriPlacementAnchor.position;
		base.transform.eulerAngles = chakkiriPlacementAnchor.eulerAngles;
		positionOffsetAnchor.localPosition = CHAKKIRI_DEF_POS;
		rotateOffsetAnchor.localEulerAngles = Vector3.zero;
		butterLiquidModel.SetActive(value: false);
	}
}
