using System;
using UnityEngine;
public class FindMask_MaskData : MonoBehaviour
{
	[Serializable]
	public struct MaskData
	{
		[SerializeField]
		[Header("お面のメッシュ")]
		public Mesh maskMesh;
		[SerializeField]
		[Header("お面のマテリアル")]
		public Material maskMat;
	}
	[SerializeField]
	[Header("お面デ\u30fcタ")]
	private MaskData[] maskData;
	[SerializeField]
	[Header("お面のスプライトレンダラ\u30fc")]
	private MeshRenderer maskMeshRenderer;
	[SerializeField]
	[Header("お面のメッシュフィルタ\u30fc")]
	private MeshFilter maskMeshFilter;
	private int objNo = -1;
	private int typeNo = -1;
	private int point;
	private bool isSelect;
	private bool isFindPair;
	private bool isRare;
	public int ObjNo => objNo;
	public int TypeNo => typeNo;
	public int Point => point;
	public bool IsSelect => isSelect;
	public bool IsFindPair => isFindPair;
	public bool IsRare => isRare;
	public void SetMaskTypeNo(int _typeNo)
	{
		typeNo = _typeNo;
	}
	public void SetMaskObjNo(int _objNo)
	{
		objNo = _objNo;
	}
	public void SetMaskPoint(int _point)
	{
		point = _point;
	}
	public void SetMaskSilhouette()
	{
		Mesh mesh = maskMeshFilter.mesh;
		Material material = maskMeshRenderer.material;
		mesh = maskData[maskData.Length - 1].maskMesh;
		material = maskData[maskData.Length - 1].maskMat;
		maskMeshFilter.mesh = mesh;
		maskMeshRenderer.material = material;
	}
	public void MaskChangeSilhouette()
	{
		Mesh mesh = maskMeshFilter.mesh;
		Material material = maskMeshRenderer.material;
		mesh = maskData[typeNo].maskMesh;
		material = maskData[typeNo].maskMat;
		maskMeshFilter.mesh = mesh;
		maskMeshRenderer.material = material;
	}
	public void SelectMask()
	{
		isSelect = true;
	}
	public void SelectMaskFindPair()
	{
		isFindPair = true;
	}
	public void IsRareMask()
	{
		isRare = true;
	}
	public void ResetMaskFlag()
	{
		isSelect = false;
		isFindPair = false;
	}
}
