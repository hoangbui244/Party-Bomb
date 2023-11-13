using UnityEngine;
public class MakingPotion_Heater : MonoBehaviour
{
	private enum State
	{
		Standy,
		FadeIn,
		Playing,
		FadeOut
	}
	[SerializeField]
	private int rpm = 5400;
	[SerializeField]
	private float shakeAngle = 2f;
	[SerializeField]
	private float fadeDuration = 1f;
	private State state;
	private float elapsed;
	private float angle;
	private void Update()
	{
		float num = Time.deltaTime;
		switch (state)
		{
		case State.FadeIn:
			elapsed += num;
			num *= Mathf.Clamp01(elapsed / fadeDuration);
			if (elapsed >= fadeDuration)
			{
				state = State.Playing;
			}
			break;
		case State.FadeOut:
			elapsed -= num;
			num *= Mathf.Clamp01(elapsed / fadeDuration);
			if (elapsed <= 0f)
			{
				state = State.Standy;
			}
			break;
		}
		if (state != 0)
		{
			angle = Mathf.Repeat(angle + (float)rpm / 60f * num, 360f);
			base.transform.localRotation = Quaternion.Euler(UnityEngine.Random.Range(0f - shakeAngle, shakeAngle), 0f, UnityEngine.Random.Range(0f - shakeAngle, shakeAngle)) * Quaternion.Euler(0f, angle, 0f);
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
}
