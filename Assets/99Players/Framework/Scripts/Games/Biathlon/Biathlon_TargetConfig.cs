using UnityEngine;
public class Biathlon_TargetConfig : ScriptableObject
{
	[SerializeField]
	private Vector3 guardOpenPosition = new Vector3(0f, 0f, 90f);
	[SerializeField]
	private Vector3 guardClosePosition = new Vector3(0f, 0f, 0f);
	[SerializeField]
	private float closeTime = 0.05f;
	public Vector3 GuardOpenPosition => guardOpenPosition;
	public Vector3 GuardClosePosition => guardClosePosition;
	public float CloseTime => closeTime;
}
