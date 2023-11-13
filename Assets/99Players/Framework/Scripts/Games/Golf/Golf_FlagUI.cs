using System;
using UnityEngine;
public class Golf_FlagUI : MonoBehaviour
{
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	private Vector3 originRootPos;
	[SerializeField]
	[Header("旗のアイコン")]
	private SpriteRenderer flagIcon;
	private Vector3 flagIconOriginPos;
	[SerializeField]
	[Header("旗のアイコンのY座標を動かす値")]
	private float flagIconMoveValue;
	private float flagIconTargetMovePosY;
	[SerializeField]
	[Header("旗のアイコンを動かす時間")]
	private float FLAG_ICON_MOVE_TIME;
	[SerializeField]
	[Header("旗のアイコンを動かすアニメ\u30fcションの回数")]
	private int FLAG_ICON_PING_PONG_CNT;
	[SerializeField]
	[Header("旗のアイコンを動かせるまでのインタ\u30fcバル")]
	private float FLAG_ICON_INTERVAL;
	private bool isFlagIconAnimation;
	public void Init()
	{
		Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
		CalcManager.mCalcVector3 = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().WorldToScreenPoint(cupPos);
		CalcManager.mCalcVector3 = SingletonCustom<Golf_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(CalcManager.mCalcVector3);
		root.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, root.transform.position.z);
		originRootPos = root.transform.localPosition;
		flagIconOriginPos = flagIcon.transform.localPosition;
		flagIconTargetMovePosY = flagIcon.transform.localPosition.y + flagIconMoveValue;
		root.SetActive(value: false);
	}
	public void InitPlay()
	{
		root.transform.localPosition = originRootPos;
		flagIcon.transform.localPosition = flagIconOriginPos;
	}
	public void UpdateMethod()
	{
		Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
		CalcManager.mCalcVector3 = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().WorldToScreenPoint(cupPos);
		CalcManager.mCalcVector3 = SingletonCustom<Golf_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(CalcManager.mCalcVector3);
		root.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, root.transform.position.z);
		if (!isFlagIconAnimation)
		{
			LeanTween.moveLocalY(flagIcon.gameObject, flagIconTargetMovePosY, FLAG_ICON_MOVE_TIME).setLoopPingPong(FLAG_ICON_PING_PONG_CNT).setEaseInQuad();
			isFlagIconAnimation = true;
			LeanTween.delayedCall(flagIcon.gameObject, FLAG_ICON_MOVE_TIME * (float)FLAG_ICON_PING_PONG_CNT + FLAG_ICON_INTERVAL, (Action)delegate
			{
				isFlagIconAnimation = false;
			});
		}
	}
	public void Show()
	{
		root.SetActive(value: true);
	}
	public void Hide()
	{
		root.SetActive(value: false);
	}
}
