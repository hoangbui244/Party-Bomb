using System;
using System.Collections.Generic;
using UnityEngine;
public class MakingPotion_Thread : MonoBehaviour
{
	private enum State
	{
		Standy,
		FadeIn,
		Playing,
		FadeOut
	}
	[Serializable]
	private class Thread
	{
		[SerializeField]
		private MeshRenderer renderer;
		[SerializeField]
		private float rotationSpeed;
		[SerializeField]
		private float scrollSpeed;
		[SerializeField]
		[Range(0f, 1f)]
		private float baseAlpha;
		[SerializeField]
		private Vector4 scaleOffset;
		private Material mat;
		private static int _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
		private static int _Color = Shader.PropertyToID("_Color");
		private static int _Alpha = Shader.PropertyToID("_Alpha");
		public void Initialize()
		{
			mat = UnityEngine.Object.Instantiate(renderer.sharedMaterial);
			renderer.sharedMaterial = mat;
			SetAlpha(0f);
		}
		public void Update(float delta)
		{
			scaleOffset.z = Mathf.Repeat(scaleOffset.z + rotationSpeed * delta, 1f);
			scaleOffset.w = Mathf.Repeat(scaleOffset.w + scrollSpeed * delta, 1f);
			mat.SetVector(_MainTex_ST, scaleOffset);
		}
		public void SetColor(Color color)
		{
			mat.SetColor(_Color, color);
		}
		public void SetAlpha(float alpha)
		{
			mat.SetFloat(_Alpha, baseAlpha * alpha);
		}
	}
	[SerializeField]
	private List<Thread> threads = new List<Thread>();
	[SerializeField]
	private float speed = 1f;
	[SerializeField]
	private float fadeDuration = 1f;
	private State state;
	private float elapsed;
	private Color nowColor;
	private void Start()
	{
		foreach (Thread thread in threads)
		{
			thread.Initialize();
		}
	}
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		switch (state)
		{
		case State.FadeIn:
			elapsed += deltaTime;
			SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
			if (elapsed >= fadeDuration)
			{
				state = State.Playing;
			}
			break;
		case State.FadeOut:
			elapsed -= deltaTime;
			SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
			if (elapsed <= 0f)
			{
				state = State.Standy;
			}
			break;
		}
		if (state != 0)
		{
			foreach (Thread thread in threads)
			{
				thread.Update(deltaTime * speed);
			}
		}
	}
	private void SetAlpha(float alpha)
	{
		foreach (Thread thread in threads)
		{
			thread.SetAlpha(alpha);
		}
	}
	[ContextMenu("Play")]
	public void Play()
	{
		state = State.FadeIn;
	}
	[ContextMenu("Stop")]
	public void Stop()
	{
		state = State.FadeOut;
	}
	public void SetColor(Color color)
	{
		nowColor = color;
		foreach (Thread thread in threads)
		{
			thread.SetColor(color);
		}
	}
	public Color GetColor()
	{
		return nowColor;
	}
}
