using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class Biathlon_Reticle : MonoBehaviour
{
	private static readonly Color[] Colors = new Color[4]
	{
		new Color(0.24f, 0.56f, 0.06f),
		new Color(0.95f, 0.13f, 0.25f),
		new Color(0.27f, 0.47f, 0.98f),
		new Color(0.96f, 0.71f, 0.02f)
	};
	[SerializeField]
	private float fillWidth = 250f;
	[SerializeField]
	private float fillHeight = 24f;
	[SerializeField]
	private float xScaleFactor = 1f;
	[SerializeField]
	private float yScaleFactor = 1f;
	[SerializeField]
	[DisplayName("表示までにかかる時間")]
	private float showDuration = 0.2f;
	[SerializeField]
	[DisplayName("表示状態を保つ時間")]
	private float keepDuration = 1f;
	[SerializeField]
	[DisplayName("非表示までにかかる時間")]
	private float hideDuration = 0.2f;
	[SerializeField]
	private GameObject reloadRoot;
	[SerializeField]
	private SpriteRenderer reticle;
	[SerializeField]
	private SpriteRenderer fill;
	[SerializeField]
	private SpriteRenderer hit;
	[SerializeField]
	private SpriteRenderer miss;
	private Coroutine coroutine;
	private SpriteRenderer currentRenderer;
	public void Init(int no)
	{
		reloadRoot.SetActive(value: false);
		hit.enabled = false;
		miss.enabled = false;
		reticle.color = Colors[no];
	}
	public void UpdatePosition(Vector3 position)
	{
		position = new Vector3(position.x * xScaleFactor, position.y * yScaleFactor, position.z);
		base.transform.localPosition = position;
	}
	public void ShowReloadUI(float reloadTime)
	{
		StartCoroutine(Reload(reloadTime));
	}
	public void ShowHit()
	{
		HideCurrentIfNeeded();
		coroutine = StartCoroutine(ShowAndHideAnimation(hit));
	}
	public void ShowMiss()
	{
		HideCurrentIfNeeded();
		coroutine = StartCoroutine(ShowAndHideAnimation(miss));
	}
	private IEnumerator Reload(float reloadTime)
	{
		reloadRoot.SetActive(value: true);
		fill.size = new Vector2(0f, fillHeight);
		float elapsed = 0f;
		float duration = reloadTime - Time.time;
		while (Time.time < reloadTime)
		{
			elapsed += Time.deltaTime;
			float num = elapsed / duration;
			fill.size = new Vector2(fillWidth * num, fillHeight);
			yield return null;
		}
		fill.size = new Vector2(fillWidth, fillHeight);
		yield return null;
		reloadRoot.SetActive(value: false);
	}
	private void HideCurrentIfNeeded()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
		if (currentRenderer != null)
		{
			currentRenderer.SetAlpha(0f);
			currentRenderer = null;
		}
	}
	private IEnumerator ShowAndHideAnimation(SpriteRenderer sp)
	{
		currentRenderer = sp;
		sp.SetAlpha(0f);
		sp.enabled = true;
		float elapsed3 = 0f;
		while (elapsed3 < showDuration)
		{
			elapsed3 += Time.deltaTime;
			float a = elapsed3 / showDuration;
			sp.SetAlpha(a);
			yield return null;
		}
		sp.SetAlpha(1f);
		elapsed3 = 0f;
		while (elapsed3 < keepDuration)
		{
			elapsed3 += Time.deltaTime;
			yield return null;
		}
		elapsed3 = 0f;
		while (elapsed3 < hideDuration)
		{
			elapsed3 += Time.deltaTime;
			float num = elapsed3 / hideDuration;
			sp.SetAlpha(1f - num);
			yield return null;
		}
		sp.SetAlpha(0f);
		sp.enabled = false;
		coroutine = null;
		currentRenderer = null;
	}
	private void OnDisable()
	{
		reloadRoot.SetActive(value: false);
		fill.size = new Vector2(0f, fillHeight);
		if (currentRenderer != null)
		{
			currentRenderer.SetAlpha(0f);
			currentRenderer.enabled = false;
			currentRenderer = null;
		}
		coroutine = null;
	}
}
