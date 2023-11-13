using System;
using System.Collections.Generic;
using UnityEngine;
public class ShavedIce_IceObjectManager : MonoBehaviour
{
	[Serializable]
	public struct IceCreatePointData
	{
		public int stepNo;
		public int widthIntervalNum;
		public CreateVecData[] createPointArray;
		public Transform rootAnchor;
		public void CreateVecList(int _stepNo, Transform _rootAnchor)
		{
			stepNo = _stepNo;
			rootAnchor = _rootAnchor;
			widthIntervalNum = (6 + stepNo * 2 - 1) / 2;
			if (widthIntervalNum >= 10)
			{
				widthIntervalNum = 10;
			}
			createPointArray = new CreateVecData[widthIntervalNum * 2 + 1];
			float num = 0.025f * Mathf.Floor(createPointArray.Length / 2) * -1f;
			int num2 = -widthIntervalNum;
			for (int i = 0; i < createPointArray.Length; i++)
			{
				Vector3 createPoint = new Vector3(num, 0f, 0f);
				createPointArray[i].Init(createPoint, num2);
				num += 0.025f;
				num2++;
			}
		}
	}
	[Serializable]
	public struct CreateVecData
	{
		public Vector3 createVec;
		public ShavedIce_IceObject createIce;
		public int pointNo;
		public bool isUsed;
		public void Init(Vector3 _createPoint, int _pointNo)
		{
			createVec = _createPoint;
			createIce = null;
			pointNo = _pointNo;
			isUsed = false;
		}
	}
	[Serializable]
	public struct SyrupMatData
	{
		[Header("シロップの各段階のマテリアル")]
		[NonReorderable]
		public Material[] syrupuMat;
	}
	[SerializeField]
	[Header("氷のプレハブ")]
	private ShavedIce_IceObject[] iceObjPrefabs;
	[SerializeField]
	[Header("氷のメッシュデ\u30fcタ")]
	private Mesh[] iceBallMeshs;
	[SerializeField]
	[Header("各プレイヤ\u30fcごとのシロップのマテリアル")]
	private SyrupMatData[] syrupMaterials;
	[SerializeField]
	[Header("各キャラごとのシロップのカラ\u30fc")]
	private Gradient[] syrupColor;
	[SerializeField]
	[Header("手前の氷")]
	private GameObject frontIceBall;
	private const int FIRST_STEP_ICE_CREATE_POINT_NUM = 6;
	private const int MAX_WIDTH_INTERVAL_SIZE = 10;
	private const float ICE_CREATE_POINT_XPOS_OFFSET = 0.025f;
	private List<IceCreatePointData> iceCreatePointList = new List<IceCreatePointData>();
	private List<ShavedIce_IceObject> createIceObjectList = new List<ShavedIce_IceObject>();
	private ShavedIce_Player player;
	private int[] checkPointNo = new int[3];
	public void Init(ShavedIce_Player _player)
	{
		player = _player;
		AddIceCreatePointStepList(2);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < createIceObjectList.Count; i++)
		{
			if (!createIceObjectList[i].IsRoundIceCreate())
			{
				createIceObjectList[i].UpdateMethod();
			}
		}
		for (int j = 0; j < iceCreatePointList.Count; j++)
		{
			for (int k = 0; k < iceCreatePointList[j].createPointArray.Length; k++)
			{
				if (iceCreatePointList[j].createPointArray[k].createIce == null && IsRightAndLeftIceForm(iceCreatePointList[j].stepNo, iceCreatePointList[j].createPointArray[k].pointNo) && IsUpperIceForm(iceCreatePointList[j].stepNo, iceCreatePointList[j].createPointArray[k].pointNo) && IsUnderIceForm(iceCreatePointList[j].stepNo, iceCreatePointList[j].createPointArray[k].pointNo))
				{
					CreateIceObject_FormUpped(iceCreatePointList[j].stepNo, iceCreatePointList[j].createPointArray[k].pointNo);
				}
			}
		}
		if (frontIceBall.activeSelf)
		{
			return;
		}
		for (int l = 0; l < iceCreatePointList[0].createPointArray.Length; l++)
		{
			if ((iceCreatePointList[0].createPointArray[l].pointNo == -1 && (iceCreatePointList[0].createPointArray[l].createIce == null || !iceCreatePointList[0].createPointArray[l].createIce.IsFormIce)) || (iceCreatePointList[0].createPointArray[l].pointNo == 0 && (iceCreatePointList[0].createPointArray[l].createIce == null || !iceCreatePointList[0].createPointArray[l].createIce.IsFormIce)) || (iceCreatePointList[0].createPointArray[l].pointNo == 1 && (iceCreatePointList[0].createPointArray[l].createIce == null || !iceCreatePointList[0].createPointArray[l].createIce.IsFormIce)))
			{
				return;
			}
		}
		for (int m = 0; m < iceCreatePointList[1].createPointArray.Length; m++)
		{
			if ((iceCreatePointList[1].createPointArray[m].pointNo == -1 && (iceCreatePointList[1].createPointArray[m].createIce == null || !iceCreatePointList[1].createPointArray[m].createIce.IsFormIce)) || (iceCreatePointList[1].createPointArray[m].pointNo == 0 && (iceCreatePointList[1].createPointArray[m].createIce == null || !iceCreatePointList[1].createPointArray[m].createIce.IsFormIce)) || (iceCreatePointList[1].createPointArray[m].pointNo == 1 && (iceCreatePointList[1].createPointArray[m].createIce == null || !iceCreatePointList[1].createPointArray[m].createIce.IsFormIce)))
			{
				return;
			}
		}
		frontIceBall.SetActive(value: true);
	}
	public void GameEndProcess()
	{
		for (int i = 0; i < createIceObjectList.Count; i++)
		{
			createIceObjectList[i].GameEndProcess();
		}
	}
	public void AddIceCreatePointStepList(int _addNum)
	{
		for (int i = 0; i < _addNum; i++)
		{
			IceCreatePointData item = default(IceCreatePointData);
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.name = "Step_" + iceCreatePointList.Count.ToString();
			item.CreateVecList(iceCreatePointList.Count, gameObject.transform);
			iceCreatePointList.Add(item);
		}
	}
	public CreateVecData[] GetStepIceCreatePointData(int _stepNo)
	{
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo == _stepNo)
			{
				return iceCreatePointList[i].createPointArray;
			}
		}
		return null;
	}
	public void CreateIceObject(int _stepNo, int _pointNo)
	{
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo)
			{
				continue;
			}
			for (int j = 0; j < iceCreatePointList[i].createPointArray.Length; j++)
			{
				if (iceCreatePointList[i].createPointArray[j].pointNo == _pointNo && !iceCreatePointList[i].createPointArray[j].isUsed)
				{
					if (IsRightAndLeftIceForm(_stepNo, _pointNo))
					{
						iceCreatePointList[i].createPointArray[j].createIce = UnityEngine.Object.Instantiate(iceObjPrefabs[UnityEngine.Random.Range(0, iceObjPrefabs.Length)], Vector3.zero, Quaternion.identity, iceCreatePointList[i].rootAnchor);
					}
					else
					{
						iceCreatePointList[i].createPointArray[j].createIce = UnityEngine.Object.Instantiate(iceObjPrefabs[3], Vector3.zero, Quaternion.identity, iceCreatePointList[i].rootAnchor);
					}
					iceCreatePointList[i].createPointArray[j].createIce.transform.localPosition = iceCreatePointList[i].createPointArray[j].createVec;
					iceCreatePointList[i].createPointArray[j].createIce.transform.SetLocalPositionZ(UnityEngine.Random.Range(-0.01f, 0.01f));
					if (_stepNo != 0)
					{
						iceCreatePointList[i].createPointArray[j].createIce.transform.SetPositionY(GetUnderIceTop(_stepNo, _pointNo));
						iceCreatePointList[i].createPointArray[j].createVec = iceCreatePointList[i].createPointArray[j].createIce.transform.localPosition;
					}
					iceCreatePointList[i].createPointArray[j].createIce.Init(player.UserType, this, _stepNo, iceCreatePointList[i].createPointArray[j].pointNo);
					iceCreatePointList[i].createPointArray[j].isUsed = true;
					createIceObjectList.Add(iceCreatePointList[i].createPointArray[j].createIce);
					if (_stepNo == iceCreatePointList.Count - 1)
					{
						AddIceCreatePointStepList(1);
					}
					return;
				}
			}
		}
	}
	public void CreateIceObject_FormUpped(int _stepNo, int _pointNo)
	{
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo)
			{
				continue;
			}
			for (int j = 0; j < iceCreatePointList[i].createPointArray.Length; j++)
			{
				if (iceCreatePointList[i].createPointArray[j].pointNo == _pointNo && !iceCreatePointList[i].createPointArray[j].isUsed)
				{
					iceCreatePointList[i].createPointArray[j].createIce = UnityEngine.Object.Instantiate(iceObjPrefabs[UnityEngine.Random.Range(0, iceObjPrefabs.Length)], Vector3.zero, Quaternion.identity, iceCreatePointList[i].rootAnchor);
					iceCreatePointList[i].createPointArray[j].createIce.transform.localPosition = iceCreatePointList[i].createPointArray[j].createVec;
					if (_stepNo != 0)
					{
						iceCreatePointList[i].createPointArray[j].createIce.transform.SetPositionY(GetUnderIceTop(_stepNo, _pointNo));
						iceCreatePointList[i].createPointArray[j].createVec = iceCreatePointList[i].createPointArray[j].createIce.transform.localPosition;
					}
					iceCreatePointList[i].createPointArray[j].createIce.InitPerfect(player.UserType, this, _stepNo, iceCreatePointList[i].createPointArray[j].pointNo);
					iceCreatePointList[i].createPointArray[j].isUsed = true;
					createIceObjectList.Add(iceCreatePointList[i].createPointArray[j].createIce);
					if (_stepNo == iceCreatePointList.Count - 1)
					{
						AddIceCreatePointStepList(1);
					}
					return;
				}
			}
		}
	}
	public CreateVecData GetNearFirstIceObject(int _stepNo, Vector3 _originPos)
	{
		Vector3 b = base.transform.InverseTransformPoint(_originPos);
		int num = 0;
		float num2 = 9999f;
		for (int i = 0; i < GetStepIceCreatePointData(_stepNo).Length; i++)
		{
			if (Vector3.Distance(GetStepIceCreatePointData(_stepNo)[i].createVec, b) < num2)
			{
				num2 = Vector3.Distance(GetStepIceCreatePointData(_stepNo)[i].createVec, b);
				num = i;
			}
		}
		return GetStepIceCreatePointData(_stepNo)[num];
	}
	public void NearUnderIceProcess(int _stepNo, int _pointNo)
	{
		if (_stepNo == 0)
		{
			return;
		}
		checkPointNo[0] = _pointNo;
		checkPointNo[1] = _pointNo - 1;
		checkPointNo[2] = _pointNo + 1;
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo - 1)
			{
				continue;
			}
			for (int j = 0; j < checkPointNo.Length; j++)
			{
				if (checkPointNo[j] < -iceCreatePointList[i].widthIntervalNum || checkPointNo[j] > iceCreatePointList[i].widthIntervalNum)
				{
					continue;
				}
				for (int k = 0; k < iceCreatePointList[i].createPointArray.Length; k++)
				{
					if (iceCreatePointList[i].createPointArray[k].pointNo != checkPointNo[j])
					{
						continue;
					}
					if (iceCreatePointList[i].createPointArray[k].createIce == null)
					{
						CreateIceObject(_stepNo - 1, iceCreatePointList[i].createPointArray[k].pointNo);
						return;
					}
					if (iceCreatePointList[i].createPointArray[k].createIce.IsFormIce)
					{
						break;
					}
					iceCreatePointList[i].createPointArray[k].createIce.PileIce();
					return;
				}
			}
		}
	}
	public void NearUpperIceProcess(int _stepNo, int _pointNo)
	{
		checkPointNo[0] = _pointNo;
		checkPointNo[1] = _pointNo - 1;
		checkPointNo[2] = _pointNo + 1;
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo + 1)
			{
				continue;
			}
			for (int j = 0; j < checkPointNo.Length; j++)
			{
				if (checkPointNo[j] < -iceCreatePointList[i].widthIntervalNum || checkPointNo[j] > iceCreatePointList[i].widthIntervalNum)
				{
					continue;
				}
				for (int k = 0; k < iceCreatePointList[i].createPointArray.Length; k++)
				{
					if (iceCreatePointList[i].createPointArray[k].pointNo != checkPointNo[j])
					{
						continue;
					}
					if (iceCreatePointList[i].createPointArray[k].createIce == null)
					{
						CreateIceObject(_stepNo + 1, iceCreatePointList[i].createPointArray[k].pointNo);
						return;
					}
					if (iceCreatePointList[i].createPointArray[k].createIce.IsFormIce)
					{
						break;
					}
					iceCreatePointList[i].createPointArray[k].createIce.PileIce();
					return;
				}
			}
		}
	}
	public bool IsNearUnderIceAllForm(int _stepNo, int _pointNo)
	{
		if (_stepNo == 0)
		{
			return true;
		}
		checkPointNo[0] = _pointNo;
		checkPointNo[1] = _pointNo - 1;
		checkPointNo[2] = _pointNo + 1;
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo - 1)
			{
				continue;
			}
			for (int j = 0; j < checkPointNo.Length; j++)
			{
				if (checkPointNo[j] < -iceCreatePointList[i].widthIntervalNum || checkPointNo[j] > iceCreatePointList[i].widthIntervalNum)
				{
					continue;
				}
				for (int k = 0; k < iceCreatePointList[i].createPointArray.Length; k++)
				{
					if (iceCreatePointList[i].createPointArray[k].pointNo == checkPointNo[j])
					{
						if (!(iceCreatePointList[i].createPointArray[k].createIce == null) && iceCreatePointList[i].createPointArray[k].createIce.IsFormIce)
						{
							break;
						}
						return false;
					}
				}
			}
		}
		return true;
	}
	public bool IsNearUpperIceAllForm(int _stepNo, int _pointNo)
	{
		checkPointNo[0] = _pointNo;
		checkPointNo[1] = _pointNo - 1;
		checkPointNo[2] = _pointNo + 1;
		for (int i = 0; i < iceCreatePointList.Count; i++)
		{
			if (iceCreatePointList[i].stepNo != _stepNo + 1)
			{
				continue;
			}
			for (int j = 0; j < checkPointNo.Length; j++)
			{
				if (checkPointNo[j] < -iceCreatePointList[i].widthIntervalNum || checkPointNo[j] > iceCreatePointList[i].widthIntervalNum)
				{
					continue;
				}
				for (int k = 0; k < iceCreatePointList[i].createPointArray.Length; k++)
				{
					if (iceCreatePointList[i].createPointArray[k].pointNo == checkPointNo[j])
					{
						if (!(iceCreatePointList[i].createPointArray[k].createIce == null) && iceCreatePointList[i].createPointArray[k].createIce.IsFormIce)
						{
							break;
						}
						return false;
					}
				}
			}
		}
		return true;
	}
	public float GetUnderIceTop(int _stepNo, int _pointNo)
	{
		if ((float)_pointNo * Mathf.Sign(_pointNo) - (float)iceCreatePointList[_stepNo - 1].widthIntervalNum <= 0f)
		{
			for (int i = 0; i < iceCreatePointList[_stepNo - 1].createPointArray.Length; i++)
			{
				if (iceCreatePointList[_stepNo - 1].createPointArray[i].pointNo == _pointNo && iceCreatePointList[_stepNo - 1].createPointArray[i].createIce != null)
				{
					return iceCreatePointList[_stepNo - 1].createPointArray[i].createIce.IceBallTopAnchor.position.y;
				}
			}
		}
		float num = 0f;
		float num2 = 0f;
		for (int j = 0; j < iceCreatePointList[_stepNo - 1].createPointArray.Length; j++)
		{
			if (iceCreatePointList[_stepNo - 1].createPointArray[j].createIce != null)
			{
				num += 1f;
				num2 += iceCreatePointList[_stepNo - 1].createPointArray[j].createIce.IceBallTopAnchor.position.y;
			}
		}
		return num2 / num;
	}
	public bool IsUnderIceForm(int _stepNo, int _pointNo)
	{
		if (_stepNo == 0)
		{
			return false;
		}
		if ((float)_pointNo * Mathf.Sign(_pointNo) - (float)iceCreatePointList[_stepNo - 1].widthIntervalNum <= 0f)
		{
			for (int i = 0; i < iceCreatePointList[_stepNo - 1].createPointArray.Length; i++)
			{
				if (iceCreatePointList[_stepNo - 1].createPointArray[i].pointNo == _pointNo && iceCreatePointList[_stepNo - 1].createPointArray[i].createIce != null)
				{
					return iceCreatePointList[_stepNo - 1].createPointArray[i].createIce.IsFormIce;
				}
			}
		}
		return false;
	}
	public bool IsUpperIceForm(int _stepNo, int _pointNo)
	{
		if (_stepNo == iceCreatePointList.Count - 1)
		{
			return false;
		}
		if ((float)_pointNo * Mathf.Sign(_pointNo) - (float)iceCreatePointList[_stepNo + 1].widthIntervalNum <= 0f)
		{
			for (int i = _stepNo + 1; i < iceCreatePointList.Count; i++)
			{
				for (int j = 0; j < iceCreatePointList[i].createPointArray.Length; j++)
				{
					if (iceCreatePointList[i].createPointArray[j].pointNo == _pointNo && iceCreatePointList[i].createPointArray[j].createIce != null && iceCreatePointList[i].createPointArray[j].createIce.IsFormIce)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	public bool IsRightAndLeftIceForm(int _stepNo, int _pointNo, bool _isDebug = false)
	{
		if (_pointNo + 1 >= -iceCreatePointList[_stepNo].widthIntervalNum && _pointNo + 1 > iceCreatePointList[_stepNo].widthIntervalNum && _pointNo - 1 >= -iceCreatePointList[_stepNo].widthIntervalNum && _pointNo - 1 > iceCreatePointList[_stepNo].widthIntervalNum)
		{
			if (_isDebug)
			{
				UnityEngine.Debug.Log("<color=red>座標番号が範囲外です</color>");
			}
			return false;
		}
		for (int i = iceCreatePointList[_stepNo].widthIntervalNum + (_pointNo + 1); i < iceCreatePointList[_stepNo].createPointArray.Length; i++)
		{
			if (iceCreatePointList[_stepNo].createPointArray[i].createIce != null && iceCreatePointList[_stepNo].createPointArray[i].createIce.IsFormIce)
			{
				if (_isDebug)
				{
					UnityEngine.Debug.Log("右側の氷は形成されています");
				}
				break;
			}
			if (i == iceCreatePointList[_stepNo].createPointArray.Length - 1)
			{
				if (_isDebug)
				{
					UnityEngine.Debug.Log("<color=red>右方向の氷は一つも形成されていません</color>");
				}
				return false;
			}
		}
		for (int num = iceCreatePointList[_stepNo].widthIntervalNum + (_pointNo - 1); num >= 0; num--)
		{
			if (iceCreatePointList[_stepNo].createPointArray[num].createIce != null && iceCreatePointList[_stepNo].createPointArray[num].createIce.IsFormIce)
			{
				if (_isDebug)
				{
					UnityEngine.Debug.Log("左側の氷は形成されています");
				}
				break;
			}
			if (num == 0)
			{
				if (_isDebug)
				{
					UnityEngine.Debug.Log("<color=red>左方向の氷は一つも形成されていません</color>");
				}
				return false;
			}
		}
		if (_isDebug)
		{
			UnityEngine.Debug.Log("<color=cyan>左右の氷は形成されています</color>");
		}
		return true;
	}
	public float GetIceTowerWidth()
	{
		int index = iceCreatePointList.Count - 1;
		float f = 0f;
		float num = 0f;
		for (int num2 = 0; num2 > -iceCreatePointList[index].widthIntervalNum; num2--)
		{
			for (int i = 0; i < iceCreatePointList[index].createPointArray.Length; i++)
			{
				if (iceCreatePointList[index].createPointArray[i].pointNo == num2 && iceCreatePointList[index].createPointArray[i].createIce != null)
				{
					f = iceCreatePointList[index].createPointArray[i].createVec.x;
				}
			}
		}
		for (int j = 0; j < iceCreatePointList[index].widthIntervalNum; j++)
		{
			for (int k = 0; k < iceCreatePointList[index].createPointArray.Length; k++)
			{
				if (iceCreatePointList[index].createPointArray[k].pointNo == j && iceCreatePointList[index].createPointArray[k].createIce != null)
				{
					num = iceCreatePointList[index].createPointArray[k].createVec.x;
				}
			}
		}
		if (!(Mathf.Abs(f) > num))
		{
			return num;
		}
		return Mathf.Abs(f);
	}
	public void SetRandomIceBallMesh(int _stepNo, int _pointNo)
	{
		for (int i = 0; i < iceCreatePointList[_stepNo].createPointArray.Length; i++)
		{
			if (iceCreatePointList[_stepNo].createPointArray[i].pointNo == _pointNo)
			{
				iceCreatePointList[_stepNo].createPointArray[i].createIce.SetMesh(iceBallMeshs[UnityEngine.Random.Range(0, iceBallMeshs.Length)]);
			}
		}
	}
	public void SetShavedIcePourSyrup()
	{
		int num = 20;
		int num2 = 5;
		int num3 = iceCreatePointList.Count - 1;
		float num4 = 1f / (float)num2;
		for (int i = 0; i < num && i < iceCreatePointList.Count; i++)
		{
			for (int j = 0; j < iceCreatePointList[num3].createPointArray.Length; j++)
			{
				if (iceCreatePointList[num3].createPointArray[j].createIce != null)
				{
					iceCreatePointList[num3].createPointArray[j].createIce.SetMaterial(syrupMaterials[ShavedIce_Define.GetConvertCPUNo(player.UserType)].syrupuMat[i / (num / num2)]);
				}
			}
			num3--;
		}
	}
	public Vector3 GetCenterTopIceVec()
	{
		for (int i = 0; i < iceCreatePointList[iceCreatePointList.Count - 1].createPointArray.Length; i++)
		{
			if (iceCreatePointList[iceCreatePointList.Count - 1].createPointArray[i].pointNo == 0)
			{
				return iceCreatePointList[iceCreatePointList.Count - 1].createPointArray[i].createVec;
			}
		}
		return Vector3.zero;
	}
	public Vector3 GetNowTopIceVec()
	{
		for (int num = iceCreatePointList.Count - 1; num >= 0; num--)
		{
			for (int i = 0; i < iceCreatePointList[num].createPointArray.Length; i++)
			{
				if (iceCreatePointList[num].createPointArray[i].createIce != null)
				{
					return iceCreatePointList[num].createPointArray[i].createVec;
				}
			}
		}
		return Vector3.zero;
	}
}
