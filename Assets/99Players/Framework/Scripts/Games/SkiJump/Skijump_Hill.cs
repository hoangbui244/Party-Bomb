using System;
using UnityEngine;
public class Skijump_Hill : MonoBehaviour
{
	[Serializable]
	public struct BrakeAnchorData
	{
		public Transform anchor;
		public Transform[] pos;
	}
	[Header("スタ\u30fcトアンカ\u30fc")]
	public Transform startAnchor;
	[Header("アプロ\u30fcチ終了")]
	public Transform approachEndAnchor;
	[Header("踏み切りアンカ\u30fc")]
	public Transform takeOffAnchor;
	[Header("K点（建築基準点）アンカ\u30fc")]
	public Transform kPointAnchor;
	[Header("HS（ヒルサイズ）アンカ\u30fc")]
	public Transform hsAnchor;
	[Header("最低地点アンカ\u30fc")]
	public Transform lowestPosAnchor;
	[Header("着地アンカ\u30fc")]
	public Transform standingAnchor;
	[Header("斜面開始アンカ\u30fc")]
	public Transform slopeStartAnchor;
	[Header("斜面終了アンカ\u30fc")]
	public Transform slopeEndAnchor;
	[SerializeField]
	[Header("ブレ\u30fcキアンカ\u30fc情報")]
	private BrakeAnchorData[] brakeAnchorData;
	private int typeNo;
	private int brakeTypeNo;
	private int brakeAnchorNo;
	public static float[] FRONT_POINT_DISTANCE = new float[2]
	{
		70f,
		90f
	};
	public static float[] K_POINT_DISTANCE = new float[2]
	{
		90f,
		120f
	};
	public static float[] HS_POINT_DISTANCE = new float[2]
	{
		100f,
		130f
	};
	public int TypeNo => typeNo;
	public void NextBrakePos()
	{
		brakeAnchorNo++;
	}
	public bool IsBrakeLastPos()
	{
		return brakeAnchorNo >= brakeAnchorData[brakeTypeNo].pos.Length - 1;
	}
	public bool CheckOverBrakeAnchorPos(Vector3 _pos, int _no = -1)
	{
		if (_no < 0)
		{
			_no = brakeAnchorNo;
		}
		return CalcManager.Length(_pos, GetBrakeAnchorPos(_no - 1)) >= CalcManager.Length(GetBrakeAnchorPos(_no), GetBrakeAnchorPos(_no - 1));
	}
	public void SettingBrakeAnchorData(int _no = -1)
	{
		if (_no < 0)
		{
			brakeTypeNo = UnityEngine.Random.Range(1, brakeAnchorData.Length);
		}
		else
		{
			brakeTypeNo = _no;
		}
		brakeAnchorNo = 1;
	}
	public float GetKPointDistance()
	{
		return CalcManager.Length(kPointAnchor.position, slopeStartAnchor.position);
	}
	public float GetKPointDistanceData(int _no = -1)
	{
		if (_no == -1)
		{
			_no = typeNo;
		}
		return K_POINT_DISTANCE[_no];
	}
	public float GetJumpDistanceRange(int _no = -1, bool _add = true)
	{
		if (_no == -1)
		{
			_no = typeNo;
		}
		if (_add)
		{
			return HS_POINT_DISTANCE[_no] - K_POINT_DISTANCE[_no];
		}
		return K_POINT_DISTANCE[_no] - FRONT_POINT_DISTANCE[_no];
	}
	public Vector3 GetBrakeAnchorPos(int _no = -1)
	{
		if (_no < 0)
		{
			_no = brakeAnchorNo;
		}
		return brakeAnchorData[brakeTypeNo].pos[_no].position;
	}
	public Vector3 GetBrakeAnchorForward(int _no = -1)
	{
		if (_no < 0)
		{
			_no = brakeAnchorNo;
		}
		return brakeAnchorData[brakeTypeNo].pos[_no].forward;
	}
}
