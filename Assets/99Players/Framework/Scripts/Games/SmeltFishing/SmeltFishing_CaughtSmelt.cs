using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CaughtSmelt : MonoBehaviour
{
	[SerializeField]
	[DisplayName("水しぶきエフェクト")]
	private ParticleSystem splash;
	private float time;
	public void Init()
	{
		time = 0.2f * UnityEngine.Random.value;
		splash.Stop();
		Deactivate();
	}
	public void Activate()
	{
		base.gameObject.SetActive(value: true);
		splash.Play();
	}
	public void Deactivate()
	{
		base.gameObject.SetActive(value: false);
		splash.Stop();
	}
	public void UpdateMethod()
	{
		float b = 15f;
		float num = 0.15f;
		time += Time.deltaTime;
		if (time >= num)
		{
			time -= num;
		}
		float t = Mathf.Clamp01(time / num);
		float x = Mathf.Lerp(-15f, b, t);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.x = x;
		base.transform.localEulerAngles = localEulerAngles;
	}
}
