using System;
using System.Collections;
using UnityEngine;
public class SmartBall_GameOverEffect : MonoBehaviour
{
	[SerializeField]
	[Header("文字アンカ\u30fc")]
	private Transform textAnchor;
	private float EFFECT_TIME = 3f;
	private float effectTime;
	private const float DEF_POS_Y = 2000f;
	private void Awake()
	{
		textAnchor.transform.SetLocalPositionY(2000f);
	}
	public void PlayGameOverEffect(Action _callBack = null)
	{
		StartCoroutine(SlideInOutEffect_UpToDown(_callBack));
	}
	private IEnumerator SlideInOutEffect_UpToDown(Action _callBack)
	{
		textAnchor.transform.SetLocalPositionY(2000f);
		LeanTween.moveLocalY(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
		yield return new WaitForSeconds(2f);
		LeanTween.moveLocalY(textAnchor.gameObject, -2000f, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate
		{
			if (_callBack != null)
			{
				_callBack();
			}
		});
	}
}
