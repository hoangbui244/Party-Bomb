using System;
using UnityEngine;
public class Skijump_StageManager : SingletonCustom<Skijump_StageManager>
{
	[Serializable]
	public struct FieldData
	{
		[SerializeField]
		[Header("センタ\u30fcサ\u30fcクル")]
		private Transform centerCircle;
		public Skijump_Define.StageType stageType;
		public Transform CenterCircle => centerCircle;
		public int StageTypeNo => (int)stageType;
		public Vector3 GetCenterPos()
		{
			return centerCircle.transform.position;
		}
		public void SetStageTypeNo(Skijump_Define.StageType _type)
		{
			stageType = _type;
		}
	}
	[Serializable]
	public struct AnchorList
	{
		[Header("チ\u30fcムアンカ\u30fc")]
		public Transform teamAnchor;
		[Header("オブジェクトアンカ\u30fc")]
		public Transform objAnchor;
	}
	[SerializeField]
	[Header("フィ\u30fcルド情報")]
	private FieldData fieldData;
	[SerializeField]
	[Header("アンカ\u30fcリスト")]
	private AnchorList anchorList;
	[SerializeField]
	[Header("ヒル")]
	private Skijump_Hill hill;
	[SerializeField]
	[Header("ヒルアンカ\u30fc")]
	private Transform hillAnchor;
	private RaycastHit rayHit;
	public AnchorList AncList => anchorList;
	public void Init()
	{
		SingletonCustom<Skijump_CameraWorkManager>.Instance.Init();
	}
	public void LateUpdateMethod()
	{
		SingletonCustom<Skijump_CameraWorkManager>.Instance.LateUpdateMethod();
	}
	public float CalcJumpDistance(Vector3 _landingPos, bool _jumping = true)
	{
		if (_jumping)
		{
			return CalcManager.Length(_landingPos, hill.slopeStartAnchor.position);
		}
		float num = 0f;
		float num2 = CalcManager.Length(_landingPos, hill.kPointAnchor.position);
		float num3 = 0f;
		if (_landingPos.z >= hill.kPointAnchor.position.z)
		{
			num3 = CalcManager.Length(hill.hsAnchor.position, hill.kPointAnchor.position);
			return hill.GetKPointDistanceData() + hill.GetJumpDistanceRange() * (num2 / num3);
		}
		num3 = CalcManager.Length(hill.lowestPosAnchor.position, hill.kPointAnchor.position);
		return hill.GetKPointDistanceData() - hill.GetJumpDistanceRange() * (num2 / num3);
	}
	public Vector3 CalcLandingPos(float _jumpDistance)
	{
		if (Physics.Raycast((hill.kPointAnchor.position - hill.slopeStartAnchor.position).normalized * _jumpDistance + Vector3.up * (Skijump_Define.RAY_DISTANCE_MAX * 0.5f), Vector3.down, out rayHit, Skijump_Define.RAY_DISTANCE_MAX, Skijump_Define.GetLayerMask("Collision_Obj_1") | Skijump_Define.GetLayerMask("Field")))
		{
			return rayHit.point;
		}
		return hill.kPointAnchor.position;
	}
	public void NextBrakePos()
	{
		hill.NextBrakePos();
	}
	public bool IsBrakeLastPos()
	{
		return hill.IsBrakeLastPos();
	}
	public void SetCameraWorkType(Skijump_Define.CameraWorkType _type)
	{
	}
	public bool CheckTakeOffDistancePer(Vector3 _pos, float _offset = 0f)
	{
		return _pos.z >= hill.takeOffAnchor.position.z - _offset;
	}
	public bool CheckOverApproachEnd(Vector3 _pos)
	{
		return _pos.z >= hill.approachEndAnchor.position.z;
	}
	public bool CheckStandingAnchor(Vector3 _pos)
	{
		return _pos.z >= hill.standingAnchor.position.z;
	}
	public bool CheckOverBrakeAnchorPos(Vector3 _pos, int _no = -1)
	{
		return hill.CheckOverBrakeAnchorPos(_pos, _no);
	}
	public void SettingBrakeAnchorData(int _no = -1)
	{
		hill.SettingBrakeAnchorData(_no);
	}
	public Transform GetCharaCreateAnchor()
	{
		return hill.startAnchor;
	}
	public Transform GetApproachEndAnchor()
	{
		return hill.approachEndAnchor;
	}
	public Transform GetTakeOffAnchor()
	{
		return hill.takeOffAnchor;
	}
	public Transform GetStandingAnchor()
	{
		return hill.standingAnchor;
	}
	public Transform GetKPointAnchor()
	{
		return hill.kPointAnchor;
	}
	public Transform GetHSAnchor()
	{
		return hill.hsAnchor;
	}
	public Transform GetLowestPosAnchor()
	{
		return hill.lowestPosAnchor;
	}
	public Transform GetSlopeEndAnchor()
	{
		return hill.slopeEndAnchor;
	}
	public Transform GetHillAnchor()
	{
		return hillAnchor;
	}
	public FieldData GetFieldData()
	{
		return fieldData;
	}
	public int GetStageTypeNo()
	{
		return hill.TypeNo;
	}
	public float GetApproachStartPosZ(float _speed)
	{
		return _speed * 0.7f;
	}
	public float GetTakeOffDistance(Vector3 _pos)
	{
		return hill.takeOffAnchor.position.z - _pos.z;
	}
	public float GetKPointDistance()
	{
		return hill.GetKPointDistance();
	}
	public float GetJumpDistanceRange()
	{
		return hill.GetJumpDistanceRange();
	}
	public float GetKPointDistanceData()
	{
		return hill.GetKPointDistanceData();
	}
	public Vector3 GetBrakeAnchorPos(int _no = -1)
	{
		return hill.GetBrakeAnchorPos(_no);
	}
	public Vector3 GetBrakeAnchorForward(int _no = -1)
	{
		return hill.GetBrakeAnchorForward(_no);
	}
}
