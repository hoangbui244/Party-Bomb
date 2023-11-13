using UnityEngine;
public class ShortTrack_CurvePoint : MonoBehaviour
{
	public enum CurvePointType
	{
		IN,
		OUT
	}
	[SerializeField]
	[Header("カ\u30fcブの種類（入るか、出るか）")]
	private CurvePointType curvePointType;
	public CurvePointType GetCurvePointType()
	{
		return curvePointType;
	}
}
