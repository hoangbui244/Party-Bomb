using System;
using UnityEngine;
public class Hidden_ActionObjectRotFloor : Hidden_ActionObjectBase
{
	[SerializeField]
	private Transform rotAnchor;
	[SerializeField]
	private Transform[] actionAnchors;
	[SerializeField]
	private AnimationCurve rotCurve;
	[SerializeField]
	private Collider damageCollider;
	private bool isRotDorPositive;
	public override void PlayAction(Hidden_CharacterScript _chara)
	{
		if (!isAction)
		{
			base.PlayAction(_chara);
			if (_chara != null)
			{
				isRotDorPositive = (Vector3.Dot(_chara.GetPos() - rotAnchor.position, base.transform.right) > 0f);
			}
			else
			{
				isRotDorPositive = !isSwitchOn;
			}
			if (isSwitchOn)
			{
				LeanTween.value(rotAnchor.gameObject, 0f, 1f, actionTime).setOnUpdate(delegate(float _value)
				{
					rotAnchor.SetLocalPositionY(Mathf.Sin(_value * (float)Math.PI) * 0.5f);
					if (isRotDorPositive)
					{
						rotAnchor.SetLocalEulerAnglesZ(rotCurve.Evaluate(_value) * 180f + 180f);
					}
					else
					{
						rotAnchor.SetLocalEulerAnglesZ(rotCurve.Evaluate(1f - _value) * 180f);
					}
				}).setOnComplete((Action)delegate
				{
					EndAction();
				});
			}
			else
			{
				LeanTween.value(rotAnchor.gameObject, 0f, 1f, actionTime).setOnUpdate(delegate(float _value)
				{
					rotAnchor.SetLocalPositionY(Mathf.Sin(_value * (float)Math.PI) * 0.5f);
					if (isRotDorPositive)
					{
						rotAnchor.SetLocalEulerAnglesZ(rotCurve.Evaluate(_value) * 180f);
					}
					else
					{
						rotAnchor.SetLocalEulerAnglesZ(rotCurve.Evaluate(1f - _value) * 180f + 180f);
					}
				}).setOnComplete((Action)delegate
				{
					EndAction();
				});
			}
		}
	}
	protected override void EndAction()
	{
		base.EndAction();
		damageCollider.enabled = isSwitchOn;
	}
	public override Transform GetActionTrans(Hidden_CharacterScript _chara)
	{
		return Hidden_ActionObjectBase.SearchNearestAnchor(actionAnchors, _chara.GetPos());
	}
}
