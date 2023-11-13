using System.Collections;
using UnityEngine;
public class SmeltFishing_SmeltShadow : MonoBehaviour
{
	[SerializeField]
	private SmeltFishing_SmeltShadowConfig config;
	[SerializeField]
	private SpriteRenderer renderer;
	[SerializeField]
	private Animator animator;
	[SerializeField]
	[Header("バケツ内の魚影かどうかのフラグ")]
	private bool isBucket;
	private Vector3 moveTarget;
	private Vector3 targetDir;
	public bool IsShowing
	{
		get;
		private set;
	}
	public void Init()
	{
		base.transform.localPosition = GetRandomPosition();
		Next();
		if (!isBucket)
		{
			Deactivate();
		}
	}
	public void UpdateMethod()
	{
		if (Vector3.Distance(base.transform.localPosition, moveTarget) < 0.1f)
		{
			base.transform.localPosition = moveTarget;
			Next();
		}
		else
		{
			base.transform.localPosition += targetDir * config.MoveSpeed * Time.deltaTime;
		}
	}
	public void Show()
	{
		StartCoroutine(ShowAnimation());
	}
	public void Hide()
	{
		StopAllCoroutines();
		renderer.SetAlpha(0f);
	}
	public void Activate()
	{
		renderer.enabled = true;
	}
	public void Deactivate()
	{
		Hide();
		renderer.enabled = false;
	}
	private void Next()
	{
		moveTarget = GetRandomPosition();
		targetDir = moveTarget - base.transform.localPosition;
		float z = Mathf.Atan2(targetDir.y, targetDir.x) * 57.29578f - 90f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}
	private IEnumerator ShowAnimation()
	{
		IsShowing = true;
		float elapsed = 0f;
		float livingTime = Mathf.Lerp(config.MinLivingTime, config.MaxLivingTime, UnityEngine.Random.value);
		while (elapsed < livingTime)
		{
			elapsed += Time.deltaTime;
			float time = Mathf.Clamp01(elapsed / livingTime);
			renderer.SetAlpha(config.AlphaCurve.Evaluate(time));
			yield return null;
		}
		renderer.SetAlpha(0f);
		IsShowing = false;
	}
	private Vector3 GetRandomPosition()
	{
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		return new Vector3(normalized.x, normalized.y, 0f) * config.MoveRadius;
	}
}
