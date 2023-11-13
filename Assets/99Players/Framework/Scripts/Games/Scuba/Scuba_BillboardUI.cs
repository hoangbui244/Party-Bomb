using UnityEngine;
public class Scuba_BillboardUI : MonoBehaviour
{
	[SerializeField]
	private Transform lookTarget;
	[SerializeField]
	private bool isAdjustScale;
	private const float add = 3f;
	private const float div = 5f;
	private const float min = 1f;
	private void Update()
	{
		base.transform.rotation = lookTarget.rotation;
		if (isAdjustScale)
		{
			Vector3 vector = base.transform.position - lookTarget.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			magnitude *= Vector3.Dot(Quaternion.Euler(0f, lookTarget.eulerAngles.y, 0f) * Vector3.forward, vector.normalized);
			base.transform.localScale = Vector3.one * Mathf.Max((magnitude + 3f) / 5f, 1f);
		}
	}
	public void SetLookTarget(Transform _target)
	{
		lookTarget = _target;
	}
}
