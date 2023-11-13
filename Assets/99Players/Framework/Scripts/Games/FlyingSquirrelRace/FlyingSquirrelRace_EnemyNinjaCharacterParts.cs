using System;
using UnityEngine;
using UnityEngine.Rendering;
public class FlyingSquirrelRace_EnemyNinjaCharacterParts : CharacterParts
{
	public enum NinjaPartsList
	{
		ShoulderArmor_L,
		ShoulderArmor_R,
		Long_Knot,
		NinjaSowrd_Double
	}
	[Serializable]
	public struct EnemyNinjaParts
	{
		public Renderer[] rendererList;
		public Transform RendererParts(NinjaPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform RendererParts(int _parts)
		{
			return rendererList[_parts].transform;
		}
		public int GetRendererListLength()
		{
			return rendererList.Length;
		}
		public void SetMaterial(Material _mat)
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				Material[] materials = rendererList[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j] = _mat;
				}
				rendererList[i].materials = materials;
			}
		}
		public void SetShadowCasting(bool _isShadowOn)
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				rendererList[i].shadowCastingMode = (_isShadowOn ? ShadowCastingMode.On : ShadowCastingMode.Off);
			}
		}
		public void SetLayer(int _layer)
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				rendererList[i].gameObject.layer = _layer;
			}
		}
		public void SetLayer(int _layer, BodyPartsList _parts, bool _isIncludeActive = false)
		{
			MeshRenderer[] componentsInChildren = rendererList[(int)_parts].GetComponentsInChildren<MeshRenderer>(_isIncludeActive);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = _layer;
			}
		}
	}
	[SerializeField]
	[Header("忍者パ\u30fcツ")]
	private EnemyNinjaParts enemyNinjaParts;
	public EnemyNinjaParts NinjaParts => enemyNinjaParts;
}
