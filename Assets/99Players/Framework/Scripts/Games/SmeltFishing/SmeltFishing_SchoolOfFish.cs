using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_SchoolOfFish : SingletonCustom<SmeltFishing_SchoolOfFish>
{
	[SerializeField]
	[DisplayName("魚群の位置")]
	private Transform point;
	[SerializeField]
	[DisplayName("魚群の大きさ")]
	private float size = 4f;
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed = 2f;
	[SerializeField]
	[DisplayName("滞在時間")]
	private float stayTime = 10f;
	[SerializeField]
	[DisplayName("魚群の移動範囲オフセット")]
	private Vector3 moveAreaOffset;
	[SerializeField]
	[DisplayName("魚群の移動範囲のスケ\u30fcル")]
	private Vector3 moveAreaScale;
	[SerializeField]
	[DisplayName("魚群の移動範囲")]
	private float moveAreaLength;
	private float nextMoveTime;
	private Vector3 nextPosition;
	private bool isMoving;
	public void Init()
	{
	}
	public void UpdateMethod()
	{
	}
	public float GetDistance(Vector3 position)
	{
		return 0f;
	}
	private Vector3 GetRandomPoint()
	{
		Vector2 vector = Random.insideUnitCircle * moveAreaLength;
		Vector3 a = new Vector3(vector.x, 0f, vector.y);
		a.Scale(moveAreaScale);
		return a + moveAreaOffset;
	}
}
