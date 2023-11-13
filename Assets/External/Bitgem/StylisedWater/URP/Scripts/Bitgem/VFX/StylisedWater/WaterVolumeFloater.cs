using UnityEngine;

namespace Bitgem.VFX.StylisedWater
{
	public class WaterVolumeFloater : MonoBehaviour
	{
		public WaterVolumeHelper WaterVolumeHelper;

		private void Update()
		{
			WaterVolumeHelper waterVolumeHelper = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
			if ((bool)waterVolumeHelper)
			{
				base.transform.position = new Vector3(base.transform.position.x, waterVolumeHelper.GetHeight(base.transform.position) ?? base.transform.position.y, base.transform.position.z);
			}
		}
	}
}
