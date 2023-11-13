using UnityEngine;
public class GS_Cloud : MonoBehaviour
{
	[SerializeField]
	[Header("雲画像")]
	private SpriteRenderer[] arrayCloud;
	private void SetCloudMove()
	{
		for (int i = 0; i < arrayCloud.Length; i++)
		{
			LeanTween.cancel(arrayCloud[i].gameObject);
		}
		for (int j = 0; j < arrayCloud.Length; j++)
		{
			LeanTween.scaleY(arrayCloud[j].gameObject, UnityEngine.Random.Range(0.009f, 0.0095f), UnityEngine.Random.Range(3f, 5f)).setEaseInOutQuad().setDelay(UnityEngine.Random.Range(0f, 1f))
				.setLoopPingPong();
		}
	}
	private void Update()
	{
		for (int i = 0; i < arrayCloud.Length; i++)
		{
			arrayCloud[i].transform.AddLocalPositionX(Time.deltaTime * UnityEngine.Random.Range(0.045f, 0.075f));
			if (arrayCloud[i].transform.localPosition.x > 11.35f)
			{
				arrayCloud[i].transform.SetLocalPositionX(-11.35f);
			}
		}
	}
	private void OnEnable()
	{
		SetCloudMove();
	}
	private void OnDisable()
	{
		for (int i = 0; i < arrayCloud.Length; i++)
		{
			LeanTween.cancel(arrayCloud[i].gameObject);
		}
	}
}
