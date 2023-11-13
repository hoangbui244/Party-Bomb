using System;
using UnityEngine;
public class Golf_AnnounceUI : MonoBehaviour
{
	private readonly float ANNOUNCE_SCALE_TIME = 0.5f;
	private readonly float ANNOUNCE_VIEW_TIME = 1.5f;
	private readonly float ANNOUNCE_FADE_TIME = 0.5f;
	private readonly float ANNOUNCE_CALL_BACK_WAIT_TIME = 1f;
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[Header("ゲ\u30fcム開始前のアナウンスUI配列")]
	private SpriteRenderer[] arrayAnnounceUI;
	private Vector3 originRootScale;
	public void Init()
	{
		originRootScale = root.transform.localScale;
		root.transform.localScale = Vector3.zero;
		root.gameObject.SetActive(value: false);
	}
	public void Show(Action _callBack)
	{
		root.SetActive(value: true);
		LeanTween.scale(root, originRootScale, ANNOUNCE_SCALE_TIME).setEaseOutBack();
		LeanTween.delayedCall(base.gameObject, ANNOUNCE_SCALE_TIME + ANNOUNCE_VIEW_TIME, (Action)delegate
		{
			LeanTween.scale(root, Vector3.zero, ANNOUNCE_SCALE_TIME).setEaseInBack().setOnComplete((Action)delegate
			{
				root.SetActive(value: false);
				LeanTween.delayedCall(base.gameObject, ANNOUNCE_CALL_BACK_WAIT_TIME, (Action)delegate
				{
					_callBack();
				});
			});
		});
	}
}
