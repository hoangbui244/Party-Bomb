using UnityEngine;
public class ShavedIce_IceObject : MonoBehaviour
{
	[SerializeField]
	[Header("BoxCollider")]
	private BoxCollider col;
	[SerializeField]
	[Header("氷のモデル")]
	private MeshRenderer iceBall;
	[SerializeField]
	[Header("氷のモデルのメッシュ")]
	private MeshFilter iceBallMesh;
	[SerializeField]
	[Header("氷のモデルの頂点アンカ\u30fc")]
	private Transform iceBallTopAnchor;
	[SerializeField]
	[Header("氷のモデルの回転アンカ\u30fc")]
	private Transform iceBallRotateAnchor;
	[SerializeField]
	[Header("球形の氷かどうか")]
	private bool isSphereBall;
	private const int CREATE_ICE_PILE_CNT = 5;
	private int nowPileCnt;
	private bool isFormIce;
	private ShavedIce_Define.UserType userType;
	private ShavedIce_IceObjectManager IOM;
	private int stepNo;
	private int pointNo;
	private bool isUpperIceAllCreate;
	private bool isUnderIceAllCreate;
	private bool isRightAndLeftIceAllCreate;
	private bool isChangeIceBallMesh;
	public Transform IceBallTopAnchor => iceBallTopAnchor;
	public bool IsFormIce => isFormIce;
	public void Init(ShavedIce_Define.UserType _userType, ShavedIce_IceObjectManager _IOM, int _stepNo, int _pointNo)
	{
		userType = _userType;
		IOM = _IOM;
		stepNo = _stepNo;
		pointNo = _pointNo;
		base.gameObject.name = "IceObject_" + _pointNo.ToString();
		ShavedIce_Define.PM.AddCreateIceCount(userType);
		col.enabled = false;
		iceBall.gameObject.SetActive(value: false);
		iceBallRotateAnchor.SetLocalEulerAnglesZ(UnityEngine.Random.Range(0, 360));
	}
	public void InitPerfect(ShavedIce_Define.UserType _userType, ShavedIce_IceObjectManager _IOM, int _stepNo, int _pointNo)
	{
		userType = _userType;
		IOM = _IOM;
		stepNo = _stepNo;
		pointNo = _pointNo;
		base.gameObject.name = "IceObject_" + ShavedIce_Define.PM.GetCreateIceCount(userType).ToString();
		ShavedIce_Define.PM.AddCreateIceCount(userType);
		col.enabled = false;
		iceBall.gameObject.SetActive(value: true);
		iceBallRotateAnchor.SetLocalEulerAnglesZ(UnityEngine.Random.Range(0, 360));
		isUnderIceAllCreate = true;
		isUpperIceAllCreate = true;
		isFormIce = true;
		nowPileCnt = 5;
	}
	public void UpdateMethod()
	{
		if (!isFormIce)
		{
			if (IOM.IsRightAndLeftIceForm(stepNo, pointNo) && IOM.IsUpperIceForm(stepNo, pointNo) && IOM.IsUnderIceForm(stepNo, pointNo))
			{
				nowPileCnt = 5;
				isFormIce = true;
				iceBall.gameObject.SetActive(value: true);
				col.enabled = true;
			}
		}
		else if (!isUnderIceAllCreate)
		{
			if (IOM.IsNearUnderIceAllForm(stepNo, pointNo))
			{
				isUnderIceAllCreate = true;
			}
		}
		else if (!isUpperIceAllCreate)
		{
			if (IOM.IsNearUpperIceAllForm(stepNo, pointNo))
			{
				isUpperIceAllCreate = true;
				col.enabled = false;
			}
		}
		else if (!isRightAndLeftIceAllCreate && IOM.IsRightAndLeftIceForm(stepNo, pointNo))
		{
			if (isSphereBall && !isChangeIceBallMesh)
			{
				IOM.SetRandomIceBallMesh(stepNo, pointNo);
				isChangeIceBallMesh = true;
			}
			isRightAndLeftIceAllCreate = true;
		}
	}
	public void PileIce()
	{
		if (!isFormIce)
		{
			nowPileCnt++;
			if (nowPileCnt == 5)
			{
				isFormIce = true;
				iceBall.gameObject.SetActive(value: true);
				col.enabled = true;
			}
		}
		else if (!isUnderIceAllCreate)
		{
			IOM.NearUnderIceProcess(stepNo, pointNo);
		}
		else if (!isUpperIceAllCreate)
		{
			IOM.NearUpperIceProcess(stepNo, pointNo);
		}
	}
	public void GameEndProcess()
	{
		if (!isFormIce)
		{
			base.gameObject.SetActive(value: false);
		}
	}
	public void SetMaterial(Material _material)
	{
		iceBall.sharedMaterial = _material;
	}
	public void SetColor(Color _color)
	{
		iceBall.sharedMaterial.SetColor("_Color", _color);
	}
	public void SetMaterialPropartyBlock(MaterialPropertyBlock _block)
	{
		iceBall.SetPropertyBlock(_block);
	}
	public void SetMesh(Mesh _mesh)
	{
		iceBallMesh.sharedMesh = _mesh;
	}
	public bool IsRoundIceCreate()
	{
		if (isUnderIceAllCreate && isUpperIceAllCreate)
		{
			return isRightAndLeftIceAllCreate;
		}
		return false;
	}
	private void OnParticleCollision()
	{
		PileIce();
	}
}
