using UnityEngine;
namespace BeachSoccer
{
	[ExecuteInEditMode]
	public class CharaUniform : MonoBehaviour
	{
		[SerializeField]
		private Transform rootBone;
		[SerializeField]
		private Material material;
		[SerializeField]
		private Material defHairMaterial;
		public void SetUniform(int _uniformNo)
		{
			ChangeMaterial(_uniformNo);
		}
		private void ChangeMaterial(int _uniformNo)
		{
			Material material = SingletonCustom<UniformListManager>.Instance.GetMaterial(_uniformNo);
			MeshRenderer[] componentsInChildren = rootBone.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].materials.Length == 1)
				{
					componentsInChildren[i].material = material;
				}
				else if (componentsInChildren[i].materials.Length == 2)
				{
					Material[] materials = componentsInChildren[i].materials;
					if (SingletonCustom<UniformListManager>.Instance.CheckNoneHair(_uniformNo))
					{
						materials[0] = material;
					}
					else
					{
						materials[0] = defHairMaterial;
					}
					materials[1] = material;
					componentsInChildren[i].materials = materials;
				}
			}
		}
		public void SetUniform(int _uniformNo, Material _material)
		{
			ChangeMaterial(_uniformNo, _material);
		}
		private void ChangeMaterial(int _uniformNo, Material _material)
		{
			MeshRenderer[] componentsInChildren = rootBone.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].materials.Length == 1)
				{
					componentsInChildren[i].material = _material;
				}
				else if (componentsInChildren[i].materials.Length == 2)
				{
					Material[] materials = componentsInChildren[i].materials;
					if (SingletonCustom<UniformListManager>.Instance.CheckNoneHair(_uniformNo))
					{
						materials[0] = _material;
					}
					else
					{
						materials[0] = defHairMaterial;
					}
					materials[1] = _material;
					componentsInChildren[i].materials = materials;
				}
			}
		}
		public void SetUniformAndHair(int _uniformNo, int _hairNo)
		{
			defHairMaterial = SingletonCustom<UniformListManager>.Instance.GetHairMaterial(_hairNo);
			SetUniform(_uniformNo);
		}
	}
}
