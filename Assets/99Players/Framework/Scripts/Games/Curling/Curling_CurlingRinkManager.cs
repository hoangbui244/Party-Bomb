using UnityEngine;
public class Curling_CurlingRinkManager : SingletonCustom<Curling_CurlingRinkManager>
{
	private enum HouseCircleType
	{
		Circle_0,
		Circle_1,
		Circle_2,
		Circle_3,
		None
	}
	private const float START_LINE = 0f;
	private const float HACK_LINE_F = 1.83f;
	private const float BACK_LINE_F = 1.83f;
	private const float TEA_LINE_F = 1.83f;
	private const float HOG_LINE_F = 6.4f;
	private const float HOG_LINE_B = 21.94f;
	private const float TEA_LINE_B = 6.4f;
	private const float BACK_LINE_B = 1.83f;
	private const float HACK_LINE_B = 1.83f;
	private const float END_LINE = 1.83f;
	private float toHogLine_Front;
	private float toHogLine_Back;
	private float toTeaLine_Back;
	private float toBackLine_Back;
	[SerializeField]
	[Header("距離を計りはじめるアンカ\u30fc（距離を測る時はＸＹ座標：スト\u30fcンの座標）")]
	private Transform lineStartAnchor;
	private readonly float TEA_RADIUS = 0.15f;
	private readonly float HOUSE_CIRCLE_0_RADIUS = 0.15f;
	private readonly float HOUSE_CIRCLE_1_RADIUS = 0.61f;
	private readonly float HOUSE_CIRCLE_2_RADIUS = 1.22f;
	private readonly float HOUSE_CIRCLE_3_RADIUS = 1.83f;
	[SerializeField]
	[Header("ハウスのポイントの配列")]
	private int[] arrayPoint;
	[SerializeField]
	[Header("ポイントが入る時にハウスを点滅させる画像")]
	private SpriteRenderer[] arrayHouseSprite;
	[NonReorderable]
	private bool[] arrayIsHouseBlink;
	[Header("---------------以下、デバッグ用------------------")]
	[SerializeField]
	[Header("デバッグ用：手前のホッグラインのライン描画")]
	private LineRenderer toHogLine_Front_LineRenderer;
	[SerializeField]
	[Header("デバッグ用：奥のホッグラインのライン描画")]
	private LineRenderer toHogLine_Back_LineRenderer;
	[SerializeField]
	[Header("デバッグ用：奥のバックラインのライン描画")]
	private LineRenderer toBackLine_Back_LineRenderer;
	[SerializeField]
	[Header("Gizmo用のサイドボ\u30fcドアンカ\u30fc")]
	public Transform[] gizmoSideBoardAnchor;
	public void Init()
	{
		toHogLine_Front = lineStartAnchor.transform.position.z + 1.83f + 1.83f + 1.83f + 6.4f;
		toHogLine_Back = toHogLine_Front + 21.94f;
		toTeaLine_Back = toHogLine_Back + 6.4f;
		toBackLine_Back = toTeaLine_Back + 1.83f;
		ViewLine();
		arrayIsHouseBlink = new bool[arrayHouseSprite.Length];
		InitPlay();
	}
	public void InitPlay()
	{
		for (int i = 0; i < arrayHouseSprite.Length; i++)
		{
			arrayIsHouseBlink[i] = false;
			arrayHouseSprite[i].SetAlpha(0f);
		}
	}
	public float GetToHogLine_Front()
	{
		return toHogLine_Front;
	}
	public float GetToHogLine_Back()
	{
		return toHogLine_Back;
	}
	public Vector3 GetBackTeaPos()
	{
		return new Vector3(lineStartAnchor.position.x, 0f, toTeaLine_Back);
	}
	public float GetToBackLine_Back()
	{
		return toBackLine_Back;
	}
	public float GetTeaCircleRadius_Second()
	{
		return HOUSE_CIRCLE_1_RADIUS;
	}
	public int[] GetArrayPoint()
	{
		return arrayPoint;
	}
	public int GetPointIdx(Curling_Stone _stone)
	{
		float num = CalcManager.Length(new Vector3(_stone.transform.position.x, 0f, _stone.transform.position.z), GetBackTeaPos()) - _stone.GetStoneObjectHalfSize();
		UnityEngine.Debug.Log("distance " + num.ToString());
		HouseCircleType houseCircleType = HouseCircleType.None;
		houseCircleType = ((!(num <= HOUSE_CIRCLE_0_RADIUS)) ? ((num <= HOUSE_CIRCLE_1_RADIUS) ? HouseCircleType.Circle_1 : ((num <= HOUSE_CIRCLE_2_RADIUS) ? HouseCircleType.Circle_2 : ((!(num <= HOUSE_CIRCLE_3_RADIUS)) ? HouseCircleType.None : HouseCircleType.Circle_3))) : HouseCircleType.Circle_0);
		int num2 = arrayPoint[(int)houseCircleType];
		if (SingletonCustom<Curling_GameManager>.Instance.GetThrowStone() == _stone && houseCircleType != HouseCircleType.None && !arrayIsHouseBlink[(int)houseCircleType])
		{
			arrayIsHouseBlink[(int)houseCircleType] = true;
		}
		return (int)houseCircleType;
	}
	public bool GetIsHouseBlink()
	{
		for (int i = 0; i < arrayIsHouseBlink.Length; i++)
		{
			if (arrayIsHouseBlink[i])
			{
				return true;
			}
		}
		return false;
	}
	public void BlinkHouseCircle()
	{
		LeanTween.value(base.gameObject, 0f, 0.7f, 0.5f).setOnUpdate(delegate(float _value)
		{
			for (int i = 0; i < arrayHouseSprite.Length; i++)
			{
				if (arrayIsHouseBlink[i])
				{
					arrayHouseSprite[i].SetAlpha(_value);
				}
			}
		}).setLoopPingPong();
	}
	public void LeanTweenCancel()
	{
		LeanTween.cancel(base.gameObject);
	}
	private void ViewLine()
	{
		toHogLine_Front_LineRenderer.positionCount = 2;
		toHogLine_Front_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, toHogLine_Front));
		toHogLine_Front_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, toHogLine_Front));
		toHogLine_Back_LineRenderer.positionCount = 2;
		toHogLine_Back_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, toHogLine_Back));
		toHogLine_Back_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, toHogLine_Back));
		toBackLine_Back_LineRenderer.positionCount = 2;
		toBackLine_Back_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, toBackLine_Back));
		toBackLine_Back_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, toBackLine_Back));
	}
	private void OnDrawGizmos()
	{
		float num = lineStartAnchor.transform.position.z + 1.83f + 1.83f + 1.83f + 6.4f;
		float num2 = num + 21.94f;
		float num3 = num2 + 6.4f;
		float z = num3 + 1.83f;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(new Vector3(lineStartAnchor.position.x, lineStartAnchor.position.y, num3), HOUSE_CIRCLE_0_RADIUS);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(new Vector3(lineStartAnchor.position.x, lineStartAnchor.position.y, num3), HOUSE_CIRCLE_1_RADIUS);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(new Vector3(lineStartAnchor.position.x, lineStartAnchor.position.y, num3), HOUSE_CIRCLE_2_RADIUS);
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(new Vector3(lineStartAnchor.position.x, lineStartAnchor.position.y, num3), HOUSE_CIRCLE_3_RADIUS);
		toHogLine_Front_LineRenderer.positionCount = 2;
		toHogLine_Front_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, num));
		toHogLine_Front_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, num));
		toHogLine_Back_LineRenderer.positionCount = 2;
		toHogLine_Back_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, num2));
		toHogLine_Back_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, num2));
		toBackLine_Back_LineRenderer.positionCount = 2;
		toBackLine_Back_LineRenderer.SetPosition(0, new Vector3(gizmoSideBoardAnchor[0].position.x, gizmoSideBoardAnchor[0].position.y, z));
		toBackLine_Back_LineRenderer.SetPosition(1, new Vector3(gizmoSideBoardAnchor[1].position.x, gizmoSideBoardAnchor[1].position.y, z));
	}
}
