using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Scuba_ShutterAnimation : MonoBehaviour
{
	[SerializeField]
	private int playerNo;
	[SerializeField]
	private GameObject source;
	[SerializeField]
	private float radius = 512f;
	[SerializeField]
	private int count = 8;
	[SerializeField]
	private float offsetAngleFactor = 1f;
	[SerializeField]
	private float pivot = 0.5f;
	[SerializeField]
	[Range(0f, 1f)]
	private float amount;
	private List<RectTransform> sheets = new List<RectTransform>();
	private void Start()
	{
		Rebuild();
		Update();
	}
	private void Update()
	{
		if (!(source == null))
		{
			float num = 360f / (float)count;
			for (int i = 0; i < count; i++)
			{
				float z = num * (float)i + num * offsetAngleFactor * amount;
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * Vector3.right * (1f - amount) * radius;
				sheets[i].anchoredPosition = new Vector2(vector.x, vector.y);
				sheets[i].localEulerAngles = new Vector3(0f, 0f, z);
			}
			Vector2 vector2 = new Vector2(0f, Mathf.Lerp(pivot, 1f, amount));
			sheets[count - 1].pivot = vector2;
			if (count >= 2)
			{
				sheets[count - 2].pivot = vector2;
			}
		}
	}
	private void Rebuild()
	{
		sheets.RemoveAll((RectTransform x) => x == null);
		while (sheets.Count > count)
		{
			RectTransform rectTransform = sheets[sheets.Count - 1];
			sheets.RemoveAt(sheets.Count - 1);
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(rectTransform.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(rectTransform.gameObject);
			}
		}
		while (sheets.Count < count)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(source, source.transform.parent);
			sheets.Add(gameObject.GetComponent<RectTransform>());
			gameObject.SetActive(value: true);
		}
		source.SetActive(value: false);
	}
	public void SetAmount(float _amount)
	{
		amount = _amount;
	}
	public void CloseShutter()
	{
		if (SingletonCustom<Scuba_CharacterManager>.Instance != null)
		{
			SingletonCustom<Scuba_CharacterManager>.Instance.RenderTextureDirection(playerNo);
		}
	}
	public void AnimationEnd()
	{
		if (SingletonCustom<Scuba_UiManager>.Instance != null)
		{
			SingletonCustom<Scuba_UiManager>.Instance.ShutterAnimationEnd(playerNo);
		}
	}
}
