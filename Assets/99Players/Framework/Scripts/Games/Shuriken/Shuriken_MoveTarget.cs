using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.CoffeeBreakTime;
public class Shuriken_MoveTarget : Shuriken_BasicTarget
{
	[Header("移動に関する設定")]
	[SerializeField]
	[DisplayName("ポイント1")]
	private Vector3 point1;
	[SerializeField]
	[DisplayName("ポイント2")]
	private Vector3 point2;
	[SerializeField]
	[DisplayName("移動時間")]
	private float duration = 1f;
	private float elapsed;
	private CoffeeBreak cb;
	private Vector3 basePosition;
	public Vector3 GetPredictionPosition(float seconds)
	{
		float t = Mathf.PingPong(elapsed + seconds, duration) / duration;
		Vector3 result = basePosition + Vector3.Lerp(Vector3.zero, point2 - point1, t);
		result.y += base.Collider.bounds.size.y * 0.75f;
		return result;
	}
	protected override void OnInitialize()
	{
		base.transform.localPosition = point1;
		basePosition = base.transform.position;
	}
	protected override void ShowMethod()
	{
		base.ShowMethod();
		cb = this.CoffeeBreak().Keep(-1f, Move).Start();
	}
	protected override void HideMethod()
	{
		base.HideMethod();
		cb?.Stop();
		cb = null;
	}
	private void Move()
	{
		if (base.IsActive)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.PingPong(elapsed, duration) / duration;
			base.transform.localPosition = Vector3.Lerp(point1, point2, t);
		}
	}
}
