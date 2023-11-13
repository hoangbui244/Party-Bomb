using System;
using UnityEngine;
using UnityStandardAssets.Utility;
public class MikoshiRaceCourse : MonoBehaviour
{
	[Serializable]
	public class Points
	{
		public Transform[] points;
	}
	[Serializable]
	public class GuideArrows
	{
		public MikoshiRaceGuideArrow[] guideArrows;
	}
	public Transform[] createAnchor;
	[SerializeField]
	private WaypointCircuit posCircuit;
	[SerializeField]
	private WaypointCircuit[] aiCircuit;
	public Transform carAnchor;
	public Transform oneCameraAnchor;
	public Transform keshigomuAnchor;
	public Transform ichigoAnchor;
	public Collider[] mikoshiPointColliders;
	public Collider[] mikoshiPointRoadColliders;
	public Points[] mikoshiPointCameraTranses;
	public MikoshiRacePointSpaceData[] mikoshiPointSpaceData;
	[SerializeField]
	private float[] mikoshiPointCircuitDistances;
	public GuideArrows[] carsGuideArrows;
	public Color[] carsGuideArrowColors;
	public void DataInit()
	{
		for (int i = 0; i < mikoshiPointSpaceData.Length; i++)
		{
			mikoshiPointSpaceData[i].DataInit();
		}
	}
	public WaypointCircuit GetPosCircuit()
	{
		return posCircuit;
	}
	public WaypointCircuit GetAiCircuit()
	{
		return aiCircuit[UnityEngine.Random.Range(0, aiCircuit.Length)];
	}
	public WaypointCircuit GetAiCircuit(int _no)
	{
		return aiCircuit[_no % aiCircuit.Length];
	}
	public int SearchMikoshiPointNo(Collider _col)
	{
		int result = -1;
		for (int i = 0; i < mikoshiPointColliders.Length; i++)
		{
			if (_col == mikoshiPointColliders[i])
			{
				result = i;
			}
		}
		return result;
	}
	public int SearchMikoshiPointRoadNo(Collider _col)
	{
		int result = -1;
		for (int i = 0; i < mikoshiPointRoadColliders.Length; i++)
		{
			if (_col == mikoshiPointRoadColliders[i])
			{
				result = i;
			}
		}
		return result;
	}
	public Transform GetMikoshiPointCameraTrans(int _mikoshiPointNo)
	{
		return mikoshiPointCameraTranses[_mikoshiPointNo].points[UnityEngine.Random.Range(0, mikoshiPointCameraTranses[_mikoshiPointNo].points.Length)];
	}
	public float GetMikoshiPointCircuitDistance(int _mikoshiPointNo)
	{
		if (_mikoshiPointNo >= mikoshiPointCircuitDistances.Length)
		{
			return -1f;
		}
		return mikoshiPointCircuitDistances[_mikoshiPointNo];
	}
	public MikoshiRaceGuideArrow GetGuideArrow(int _carNo, int _mikoshiPointNo)
	{
		if (_mikoshiPointNo >= carsGuideArrows[_carNo].guideArrows.Length)
		{
			return null;
		}
		return carsGuideArrows[_carNo].guideArrows[_mikoshiPointNo];
	}
	public MikoshiRaceGuideArrow[] GetGuideArrows(int _carNo)
	{
		return carsGuideArrows[_carNo].guideArrows;
	}
	public void ChangeGudeArrowColor(int _carNo, int _playerNo)
	{
		for (int i = 0; i < carsGuideArrows[_carNo].guideArrows.Length; i++)
		{
			carsGuideArrows[_carNo].guideArrows[i].SetParticleColor(carsGuideArrowColors[_playerNo]);
		}
	}
}
