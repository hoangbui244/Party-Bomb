using UnityEngine;
public class BeachSoccerFootprint : MonoBehaviour
{
	private const float FADE_START_TIME = 0.5f;
	private const float FADE_TIME = 1f;
	[SerializeField]
	[Header("描画対象")]
	private MeshRenderer meshRenderer;
	private bool isStartFade;
	private float timer;
	private void OnEnable()
	{
		meshRenderer.SetAlpha(1f);
		isStartFade = false;
		timer = 0f;
	}
	private void Update()
	{
		timer += Time.deltaTime;
		if (isStartFade)
		{
			meshRenderer.SetAlpha(1f - timer / 1f);
			if (timer > 1f)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (timer > 0.5f)
		{
			isStartFade = true;
			timer -= 0.5f;
		}
	}
}
