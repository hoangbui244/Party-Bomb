using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandSeal_SecretStyleEffect : MonoBehaviour
{
	[Header("Fire")]
	[SerializeField]
	private ParticleSystem fire0;
	[SerializeField]
	private ParticleSystem fire1;
	[SerializeField]
	private float angle = 180f;
	[SerializeField]
	private float radius = 1f;
	[SerializeField]
	private float emitDuration = 1.5f;
	[SerializeField]
	private float fireDuration = 1f;
	[Header("Swirl")]
	[SerializeField]
	private ParticleSystem swirl;
	[Header("Spark")]
	[SerializeField]
	private ParticleSystem spark;
	[Header("Rock")]
	[SerializeField]
	private ParticleSystem rock;
	private List<ParticleSystem> fires0 = new List<ParticleSystem>();
	private List<ParticleSystem> fires1 = new List<ParticleSystem>();
	public bool IsPlaying
	{
		get;
		private set;
	}
	[ContextMenu("Play")]
	public void Play()
	{
		if (!IsPlaying)
		{
			IsPlaying = true;
			StartCoroutine(PlayInternal());
			swirl.Play();
			spark.Play();
			rock.Play();
		}
	}
	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.P))
		{
			Play();
		}
	}
	private IEnumerator PlayInternal()
	{
		float elapsed2 = 0f;
		Transform transform = base.transform;
		int count = 0;
		while (elapsed2 < emitDuration)
		{
			ParticleSystem fire = GetFire(fire0, fires0, count);
			ParticleSystem fire2 = GetFire(fire1, fires1, count);
			float num = angle * Mathf.Clamp01(elapsed2 / emitDuration);
			Vector3 localPosition = Quaternion.Euler(0f, num, 0f) * Vector3.forward * radius;
			fire.transform.localPosition = localPosition;
			localPosition = Quaternion.Euler(0f, 0f - num, 0f) * Vector3.forward * radius;
			fire2.transform.localPosition = localPosition;
			fire.Play();
			fire2.Play();
			count++;
			elapsed2 += Time.deltaTime;
			yield return null;
		}
		elapsed2 = 0f;
		while (elapsed2 < emitDuration)
		{
			elapsed2 += Time.deltaTime;
			yield return null;
		}
		fire0.Stop(withChildren: true);
		fire1.Stop(withChildren: true);
		foreach (ParticleSystem item in fires0)
		{
			item.Stop(withChildren: true);
		}
		foreach (ParticleSystem item2 in fires1)
		{
			item2.Stop(withChildren: true);
		}
		IsPlaying = false;
	}
	private ParticleSystem GetFire(ParticleSystem source, List<ParticleSystem> list, int index)
	{
		if (list.Count <= index)
		{
			ParticleSystem item = UnityEngine.Object.Instantiate(source, base.transform);
			list.Add(item);
		}
		return list[index];
	}
}
