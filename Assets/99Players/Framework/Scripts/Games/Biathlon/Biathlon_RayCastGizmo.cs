using UnityEngine;
public class Biathlon_RayCastGizmo : MonoBehaviour
{
	[SerializeField]
	private bool onlySelected = true;
	private void OnDrawGizmos()
	{
		if (!onlySelected)
		{
			DrawRaycast();
		}
	}
	private void OnDrawGizmosSelected()
	{
		if (onlySelected)
		{
			DrawRaycast();
		}
	}
	private void DrawRaycast()
	{
		Vector3 forward = base.transform.forward;
		Vector3 position = base.transform.position;
		Vector3 to = position + forward * 60f;
		Gizmos.DrawLine(position, to);
	}
}
