using System;
using UnityEngine;
using UnityEngine.Rendering;
public class Curling_CharacterParts : MonoBehaviour
{
	public enum BodyPartsList
	{
		HEAD,
		BODY,
		HIP,
		SHOULDER_L,
		SHOULDER_R,
		ARM_L,
		ARM_R,
		LEG_L,
		LEG_R
	}
	[Serializable]
	public struct BodyParts
	{
		[SerializeField]
		[Header("体アンカ\u30fc")]
		public GameObject bodyAnchor;
		public MeshRenderer[] rendererList;
		public MeshRenderer cap;
		public Transform RendererParts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform RendererParts(int _parts)
		{
			return rendererList[_parts].transform;
		}
		public void SetShadowCastingOn()
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				rendererList[i].shadowCastingMode = ShadowCastingMode.On;
			}
			cap.shadowCastingMode = ShadowCastingMode.On;
		}
		public void SetShadowCastingOff()
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				rendererList[i].shadowCastingMode = ShadowCastingMode.Off;
			}
			cap.shadowCastingMode = ShadowCastingMode.Off;
		}
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	private bool isReverse;
	[SerializeField]
	[Header("ブラシ")]
	private GameObject brushObj;
	[SerializeField]
	[Header("ブラシのMeshRenderer")]
	private MeshRenderer[] brushMeshRenderer;
	[SerializeField]
	[Header("ゼッケン")]
	private GameObject bibsObj;
	public BodyParts Parts => bodyParts;
	public bool GetIsReverse()
	{
		return isReverse;
	}
	public void SetIsReverse(bool _isReverse)
	{
		isReverse = _isReverse;
	}
	public void SetBrushMaterial(Material _mat)
	{
		for (int i = 0; i < brushMeshRenderer.Length; i++)
		{
			brushMeshRenderer[i].material = _mat;
		}
	}
	public GameObject GetBrushObj()
	{
		return brushObj;
	}
	public GameObject GetBibsObj()
	{
		return bibsObj;
	}
}
