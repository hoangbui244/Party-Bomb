using UnityEngine;
public class MonsterRace_CameraTriggerCheck : MonoBehaviour
{
	private const float MARGIN_Y = 0.5f;
	[SerializeField]
	private int cameraNo;
	private void OnTriggerStay(Collider _col)
	{
		if (_col.gameObject.tag == "HorizontalWall")
		{
			Vector3 position = base.transform.position;
			position.y += 10f;
			if (Physics.SphereCast(position, 0.5f, Vector3.down, out RaycastHit hitInfo, 20f, 8388608))
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.SetCameraOnTriggerData(cameraNo, hitInfo.point.y + 0.5f);
			}
		}
	}
}
