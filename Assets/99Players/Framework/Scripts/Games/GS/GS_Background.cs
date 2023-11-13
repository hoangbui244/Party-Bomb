using System;
using UnityEngine;
public class GS_Background : MonoBehaviour
{
	[SerializeField]
	[Header("鳥アニメ\u30fcション")]
	private Animator[] arrayBirdAnimator;
	[SerializeField]
	[Header("雲ル\u30fcト")]
	private GameObject cloud;
	[SerializeField]
	[Header("海")]
	private SpriteRenderer sea;
	private Vector3[] arrayBirdInitLocalPos;
	private float[] arrayBirdScale;
	private float calcScale;
	private void Awake()
	{
		arrayBirdInitLocalPos = new Vector3[arrayBirdAnimator.Length];
		arrayBirdScale = new float[arrayBirdAnimator.Length];
		for (int i = 0; i < arrayBirdInitLocalPos.Length; i++)
		{
			arrayBirdInitLocalPos[i] = arrayBirdAnimator[i].transform.localPosition;
			arrayBirdScale[i] = UnityEngine.Random.Range(0.0125f, 0.025f);
		}
	}
	private void OnEnable()
	{
		for (int i = 0; i < arrayBirdAnimator.Length; i++)
		{
			arrayBirdAnimator[i].Play(arrayBirdAnimator[i].GetCurrentAnimatorStateInfo(0).shortNameHash, 0, UnityEngine.Random.Range(0f, 1f));
			arrayBirdAnimator[i].transform.localPosition = arrayBirdInitLocalPos[i];
			LeanTween.moveLocalY(arrayBirdAnimator[i].gameObject, arrayBirdInitLocalPos[i].y - 14f, 3f).setEaseInOutQuad().setLoopPingPong()
				.setDelay(UnityEngine.Random.Range(0f, 0.25f));
		}
		cloud.transform.SetLocalPositionX(0f);
		LeanTween.moveLocalX(cloud, -2500f, 240f).setOnComplete((Action)delegate
		{
			OnCloudRepeat();
		});
		sea.transform.SetLocalScaleY(1f);
		LeanTween.scaleY(sea.gameObject, 0.9f, 3.3f).setEaseInOutQuad().setLoopPingPong();
	}
	private void Update()
	{
		for (int i = 0; i < arrayBirdAnimator.Length; i++)
		{
			arrayBirdAnimator[i].transform.AddLocalPositionX(Time.deltaTime * -20f);
			calcScale = Mathf.Clamp(arrayBirdAnimator[i].transform.localScale.x - Time.deltaTime * arrayBirdScale[i], 0f, 1f);
			arrayBirdAnimator[i].transform.SetLocalScale(calcScale, calcScale, calcScale);
			if (calcScale <= 0f)
			{
				arrayBirdAnimator[i].transform.SetLocalPositionX(1050f);
				arrayBirdAnimator[i].transform.SetLocalScale(0.9f, 0.9f, 0.9f);
				arrayBirdScale[i] = UnityEngine.Random.Range(0.0125f, 0.025f);
			}
		}
	}
	private void OnCloudRepeat()
	{
		cloud.transform.SetLocalPositionX(0f);
		LeanTween.moveLocalX(cloud, -2500f, 240f).setOnComplete((Action)delegate
		{
			OnCloudRepeat();
		});
	}
	private void OnDestroy()
	{
		for (int i = 0; i < arrayBirdAnimator.Length; i++)
		{
			LeanTween.cancel(arrayBirdAnimator[i].gameObject);
		}
		LeanTween.cancel(cloud);
		LeanTween.cancel(sea.gameObject);
	}
}
