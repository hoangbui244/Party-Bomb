using UnityEngine;
namespace BeachSoccer
{
	public class StageData : MonoBehaviour
	{
		public Material fieldMaterial;
		public Material floorMaterial;
		public Material[] ballMaterial;
		public Material deskMaterial;
		public Material chairMaterial;
		public TranslucentObject[] arrayTranslucent;
		public Transform[] arrayFixBenchAnchor;
		public Material GetFieldMaterial()
		{
			return fieldMaterial;
		}
		public Material GetFloorMaterial()
		{
			return floorMaterial;
		}
		public Material GetRandomBallMaterial()
		{
			return ballMaterial[Random.Range(0, ballMaterial.Length)];
		}
		public Material GetBallMaterial(int _no)
		{
			return ballMaterial[_no];
		}
		public Material GetDeskMaterial()
		{
			return deskMaterial;
		}
		public Material GetChairMaterial()
		{
			return chairMaterial;
		}
		public TranslucentObject[] GetArrayTranslucentObject()
		{
			return arrayTranslucent;
		}
		public Transform[] GetArrayFixBenchAnchor()
		{
			return arrayFixBenchAnchor;
		}
	}
}
