using System;
using UnityEngine;
public class Takoyaki_OilBrush : MonoBehaviour
{
	[SerializeField]
	[Header("油引きを置く場所のアンカ\u30fc")]
	private Transform oilBrushPlacementAnchor;
	[SerializeField]
	[Header("座標オフセットアンカ\u30fc")]
	private Transform positionOffsetAnchor;
	[SerializeField]
	[Header("回転オフセットアンカ\u30fc")]
	private Transform rotateOffsetAnchor;
	private const float OIL_BRUSH_POS_Y = 0.004f;
	private const float OIL_BRUSH_POS_X_START = -0.015f;
	private const float OIL_BRUSH_POS_X_END = 0.015f;
	private const float OIL_BRUSH_ROT_Z_START = -5f;
	private const float OIL_BRUSH_ROT_Z_END = 5f;
	private const float OIL_BRUSH_ANIMATION_TIME = 0.15f;
	public void Init()
	{
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			oilBrushPlacementAnchor.SetLocalPosition(0.2f, 0.08f, 0.2f);
		}
		else
		{
			oilBrushPlacementAnchor.SetLocalPosition(0.271f, 0.08f, 0.117f);
		}
		base.transform.position = oilBrushPlacementAnchor.position;
		base.transform.eulerAngles = oilBrushPlacementAnchor.eulerAngles;
		positionOffsetAnchor.localPosition = Vector3.zero;
		rotateOffsetAnchor.localEulerAngles = Vector3.zero;
	}
	public void PlayOilBrushAnimation(Vector3 _holePoint)
	{
		LeanTween.cancel(positionOffsetAnchor.gameObject);
		LeanTween.cancel(base.gameObject);
		LeanTween.delayedCall(0.15f, (Action)delegate
		{
			base.transform.position = _holePoint;
			base.transform.localEulerAngles = Vector3.zero;
			positionOffsetAnchor.SetLocalPositionY(0.004f);
			positionOffsetAnchor.SetLocalPositionX(-0.015f);
			rotateOffsetAnchor.SetLocalEulerAnglesZ(-5f);
			OilBrushAnimationProcess();
		});
	}
	private void OilBrushAnimationProcess()
	{
		LeanTween.moveLocalX(positionOffsetAnchor.gameObject, 0.015f, 0.15f).setOnComplete((Action)delegate
		{
			base.transform.position = oilBrushPlacementAnchor.position;
			base.transform.eulerAngles = oilBrushPlacementAnchor.eulerAngles;
			positionOffsetAnchor.localPosition = Vector3.zero;
			rotateOffsetAnchor.localEulerAngles = Vector3.zero;
		});
		LeanTween.rotateZ(rotateOffsetAnchor.gameObject, 5f, 0.15f);
	}
}
