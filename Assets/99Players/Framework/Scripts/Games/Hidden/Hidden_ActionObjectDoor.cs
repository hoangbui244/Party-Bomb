using System;
using UnityEngine;
public class Hidden_ActionObjectDoor : Hidden_ActionObjectBase
{
	[SerializeField]
	private Transform[] actionAnchors;
	[SerializeField]
	private Transform[] doorAnchors;
	[SerializeField]
	private Vector3[] doorMoveLocalPoses;
	private Vector3[] doorStartLocalPoses;
	private void Start()
	{
		doorStartLocalPoses = new Vector3[doorAnchors.Length];
		for (int i = 0; i < doorStartLocalPoses.Length; i++)
		{
			doorStartLocalPoses[i] = doorAnchors[i].localPosition;
		}
	}
	public override void PlayAction(Hidden_CharacterScript _chara)
	{
		if (isAction)
		{
			return;
		}
		base.PlayAction(_chara);
		if (isSwitchOn)
		{
			for (int i = 0; i < doorAnchors.Length; i++)
			{
				if (i == 0)
				{
					LeanTween.moveLocal(doorAnchors[i].gameObject, doorStartLocalPoses[i], actionTime).setOnComplete((Action)delegate
					{
						base.EndAction();
					});
				}
				else
				{
					LeanTween.moveLocal(doorAnchors[i].gameObject, doorStartLocalPoses[i], actionTime);
				}
			}
			return;
		}
		for (int j = 0; j < doorAnchors.Length; j++)
		{
			if (j == 0)
			{
				LeanTween.moveLocal(doorAnchors[j].gameObject, doorMoveLocalPoses[j], actionTime).setOnComplete((Action)delegate
				{
					base.EndAction();
				});
			}
			else
			{
				LeanTween.moveLocal(doorAnchors[j].gameObject, doorMoveLocalPoses[j], actionTime);
			}
		}
	}
	public override Transform GetActionTrans(Hidden_CharacterScript _chara)
	{
		return Hidden_ActionObjectBase.SearchNearestAnchor(actionAnchors, _chara.GetPos());
	}
}
