using System;
using UnityEngine;
public class Takoyaki_SauceBrush : MonoBehaviour
{
	[SerializeField]
	[Header("ソ\u30fcスブラシを置く場所のアンカ\u30fc")]
	private Transform sauceBrushPlacementAnchor;
	[SerializeField]
	[Header("座標オフセットアンカ\u30fc")]
	private Transform positionOffsetAnchor;
	[SerializeField]
	[Header("回転オフセットアンカ\u30fc")]
	private Transform rotateOffsetAnchor;
	private const float SAUCE_BRUSH_ROT_Y = 90f;
	private const float SAUCE_BRUSH_ROT_X_START = 10f;
	private const float SAUCE_BRUSH_ROT_X_END = -10f;
	private const float SAUCE_BRUSH_POS_Y = 0.245f;
	private const float SAUCE_BRUSH_POS_X_START = -0.05f;
	private const float SAUCE_BRUSH_POS_X_END = 0.05f;
	private const float SAUCE_BRUSH_MOVE_ANIMATION_TIME = 1f;
	private const float SAUCE_BRUSH_SWING_ANIMATION_TIME = 0.25f;
	public void Init()
	{
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			sauceBrushPlacementAnchor.SetLocalPosition(-0.185f, 0.14f, 0.1f);
		}
		else
		{
			sauceBrushPlacementAnchor.SetLocalPosition(-0.37f, 0.153f, 0.09f);
		}
		base.transform.position = sauceBrushPlacementAnchor.position;
		base.transform.eulerAngles = sauceBrushPlacementAnchor.eulerAngles;
		positionOffsetAnchor.localPosition = Vector3.zero;
		rotateOffsetAnchor.localEulerAngles = Vector3.zero;
	}
	public void PlaySauceBrushAnimation(Transform _takoBoxTransform)
	{
		LeanTween.cancel(positionOffsetAnchor.gameObject);
		LeanTween.cancel(rotateOffsetAnchor.gameObject);
		base.transform.position = _takoBoxTransform.position;
		base.transform.localEulerAngles = _takoBoxTransform.localEulerAngles;
		positionOffsetAnchor.SetLocalPosition(-0.05f, 0.245f, 0f);
		rotateOffsetAnchor.SetLocalEulerAngles(10f, 90f, 0f);
		SauceBrushAnimationProcess();
	}
	private void SauceBrushAnimationProcess()
	{
		LeanTween.rotateX(rotateOffsetAnchor.gameObject, -10f, 0.25f).setLoopPingPong();
		LeanTween.moveLocalX(positionOffsetAnchor.gameObject, 0.05f, 1f).setOnComplete((Action)delegate
		{
			LeanTween.cancel(rotateOffsetAnchor.gameObject);
			base.transform.position = sauceBrushPlacementAnchor.position;
			base.transform.eulerAngles = sauceBrushPlacementAnchor.eulerAngles;
			positionOffsetAnchor.localPosition = Vector3.zero;
			rotateOffsetAnchor.localEulerAngles = Vector3.zero;
		});
	}
}
