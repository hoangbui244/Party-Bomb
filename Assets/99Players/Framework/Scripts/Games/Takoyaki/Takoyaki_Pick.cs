using System;
using UnityEngine;
public class Takoyaki_Pick : MonoBehaviour
{
	[SerializeField]
	[Header("ピックのオブジェクト")]
	private GameObject pickObj;
	[SerializeField]
	[Header("ピックを置く場所のアンカ\u30fc")]
	private Transform pickPlacementAnchor;
	[SerializeField]
	[Header("ピックのアニメ\u30fcションの開始地点")]
	private Transform pickAnimationStartPoint;
	[SerializeField]
	[Header("ピックのアニメ\u30fcションの中間地点")]
	private Transform pickAnimationHalfwayPoint;
	[SerializeField]
	[Header("ピックのアニメ\u30fcションの終了地点")]
	private Transform pickAnimationEndPoint;
	private Vector3 pickAnimationEuler = new Vector3(300f, 90f, 0f);
	public void Init()
	{
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			pickPlacementAnchor.SetLocalPosition(0.195f, 0.01f, -0.12f);
		}
		else
		{
			pickPlacementAnchor.SetLocalPosition(0.236f, 0.01f, -0.07f);
		}
		base.transform.position = pickPlacementAnchor.position;
		base.transform.eulerAngles = pickPlacementAnchor.eulerAngles;
		pickObj.transform.localPosition = Vector3.zero;
		pickObj.transform.localEulerAngles = Vector3.zero;
	}
	public void PlayPickAnimation(Vector3 _holePosition)
	{
		LeanTween.cancel(pickObj);
		base.transform.position = _holePosition;
		base.transform.localEulerAngles = Vector3.zero;
		pickObj.transform.localPosition = pickAnimationStartPoint.localPosition;
		pickObj.transform.localEulerAngles = pickAnimationEuler;
		LeanTween.moveLocal(pickObj, pickAnimationHalfwayPoint.localPosition, 0.1f);
		LeanTween.moveLocal(pickObj, pickAnimationEndPoint.localPosition, 0.1f).setDelay(0.1f).setOnComplete((Action)delegate
		{
			base.transform.position = pickPlacementAnchor.position;
			base.transform.eulerAngles = pickPlacementAnchor.eulerAngles;
			pickObj.transform.localPosition = Vector3.zero;
			pickObj.transform.localEulerAngles = Vector3.zero;
		});
	}
}
