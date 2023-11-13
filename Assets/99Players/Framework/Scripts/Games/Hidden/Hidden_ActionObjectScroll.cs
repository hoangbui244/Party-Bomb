using System;
using UnityEngine;
public class Hidden_ActionObjectScroll : Hidden_ActionObjectBase
{
	private static readonly Vector3 SCROLL_DEFAULT_LOCAL_EULER = Vector3.zero;
	private static readonly Vector3 SCROLL_ACTION_LOCAL_EULER = new Vector3(20f, 70f, 10f);
	[SerializeField]
	private Transform rotAnchor;
	[SerializeField]
	private Transform[] actionAnchors;
	[SerializeField]
	private Collider scrollCollider;
	public override void PlayAction(Hidden_CharacterScript _chara)
	{
		if (!isAction)
		{
			base.PlayAction(_chara);
			if (isSwitchOn)
			{
				LeanTween.rotateLocal(rotAnchor.gameObject, SCROLL_DEFAULT_LOCAL_EULER, actionTime).setOnComplete((Action)delegate
				{
					scrollCollider.enabled = true;
					base.EndAction();
				});
				return;
			}
			scrollCollider.enabled = false;
			LeanTween.rotateLocal(rotAnchor.gameObject, SCROLL_ACTION_LOCAL_EULER, actionTime).setOnComplete((Action)delegate
			{
				base.EndAction();
			});
		}
	}
	public override Transform GetActionTrans(Hidden_CharacterScript _chara)
	{
		return Hidden_ActionObjectBase.SearchNearestAnchor(actionAnchors, _chara.GetPos());
	}
}
