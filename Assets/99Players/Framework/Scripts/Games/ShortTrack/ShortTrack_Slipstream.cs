using UnityEngine;
public class ShortTrack_Slipstream : MonoBehaviour
{
	private enum State
	{
		Standby,
		FadeIn,
		Playing,
		FadeOut
	}
	[SerializeField]
	private float speed = 1f;
	[SerializeField]
	private float fadeDuration = 0.1f;
	private Material mat;
	private Vector4 uv;
	private float scroll;
	private float elapsed;
	private State state;
	private Renderer renderer;
	private static int _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
	private static int _Alpha = Shader.PropertyToID("_Alpha");
	private void Start()
	{
		renderer = GetComponent<Renderer>();
		mat = UnityEngine.Object.Instantiate(renderer.sharedMaterial);
		mat.SetFloat(_Alpha, 0f);
		renderer.enabled = false;
		uv = mat.GetVector(_MainTex_ST);
		renderer.sharedMaterial = mat;
	}
	private void Update()
	{
		switch (state)
		{
		case State.FadeIn:
			elapsed += Time.deltaTime;
			mat.SetFloat(_Alpha, Mathf.Clamp01(elapsed / fadeDuration));
			if (elapsed >= fadeDuration)
			{
				state = State.Playing;
			}
			break;
		case State.FadeOut:
			elapsed += Time.deltaTime;
			mat.SetFloat(_Alpha, 1f - Mathf.Clamp01(elapsed / fadeDuration));
			if (elapsed >= fadeDuration)
			{
				state = State.Standby;
				renderer.enabled = false;
			}
			break;
		}
		if (state != 0)
		{
			scroll = (scroll + Time.deltaTime * speed) % 1f;
			uv.w = 0f - scroll;
			mat.SetVector(_MainTex_ST, uv);
		}
	}
	public void Play()
	{
		switch (state)
		{
		case State.Standby:
			uv.w = 0f;
			elapsed = 0f;
			state = State.FadeIn;
			renderer.enabled = true;
			break;
		case State.FadeOut:
			state = State.FadeIn;
			break;
		}
	}
	public void Stop()
	{
		switch (state)
		{
		case State.Playing:
			elapsed = 0f;
			state = State.FadeOut;
			break;
		case State.FadeIn:
			state = State.FadeOut;
			break;
		}
	}
}
