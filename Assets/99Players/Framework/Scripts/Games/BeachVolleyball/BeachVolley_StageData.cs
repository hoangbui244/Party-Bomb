using UnityEngine;
public class BeachVolley_StageData : MonoBehaviour
{
	public Material fieldMaterial;
	public Material floorMaterial;
	public Material[] ballMaterial;
	[SerializeField]
	private Color[] ballOutLineColor;
	public Material deskMaterial;
	public Color stageLightColor = Color.white;
	[SerializeField]
	[Header("ベンチの位置")]
	private Vector3[] benchPosition = new Vector3[2];
	[SerializeField]
	[Header("【縦視点】常時非表示オブジェクト")]
	public GameObject[] arrayHiddenVerticalObj;
	[SerializeField]
	[Header("【横視点】常時非表示オブジェクト")]
	public GameObject[] arrayHiddenHorizontalObj;
	[SerializeField]
	[Header("縦視点非表示オブジェクト（前半）")]
	public GameObject[] arrayHiddenVerticalObjFirstHalf;
	[SerializeField]
	[Header("縦視点非表示オブジェクト（後半）")]
	public GameObject[] arrayHiddenVerticalObjSecondHalf;
	[SerializeField]
	[Header("横視点非表示オブジェクト（前半）")]
	public GameObject[] arrayHiddenHorizontalFirstHalfObj;
	[SerializeField]
	[Header("横視点非表示オブジェクト（後半）")]
	public GameObject[] arrayHiddenHorizontalSecondHalfObj;
	[SerializeField]
	[Header("固有フィ\u30fcルド")]
	public GameObject fixField;
	public Material GetFieldMaterial()
	{
		return fieldMaterial;
	}
	public Material GetFloorMaterial()
	{
		return floorMaterial;
	}
	public Material GetRandomBallMaterial()
	{
		return ballMaterial[Random.Range(0, ballMaterial.Length)];
	}
	public Material GetBallMaterial(int _no)
	{
		return ballMaterial[_no];
	}
	public Color GetBallOutLineColor(int outlineNo)
	{
		if (outlineNo > ballOutLineColor.Length || outlineNo < 0)
		{
			UnityEngine.Debug.Log(outlineNo.ToString() + "番のアウトラインカラ\u30fcが定義されていません");
			return ColorPalet.red;
		}
		return ballOutLineColor[outlineNo];
	}
	public Color[] GetBallOutLineColor()
	{
		return ballOutLineColor;
	}
	public Vector3 GetBenchPosition(int _playerNo)
	{
		return benchPosition[_playerNo];
	}
	public Material GetDeskMaterial()
	{
		return deskMaterial;
	}
}
