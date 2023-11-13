using System;
using UnityEngine;
public class Hidden_ActionObjectRotY : Hidden_ActionObjectBase
{
	[SerializeField]
	private Transform rotAnchor;
	[SerializeField]
	private Transform[] actionAnchors;
	public override void PlayAction(Hidden_CharacterScript _chara)
	{
		if (!isAction)
		{
			base.PlayAction(_chara);
			LeanTween.rotateAroundLocal(rotAnchor.gameObject, Vector3.up, -180f, actionTime).setOnComplete((Action)delegate
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
