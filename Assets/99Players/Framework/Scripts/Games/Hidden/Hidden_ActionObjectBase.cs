using System.Collections;
using UnityEngine;
public class Hidden_ActionObjectBase : MonoBehaviour
{
	public enum ActionObjectType
	{
		RotY,
		Door,
		RotFloor,
		Scroll
	}
	[SerializeField]
	protected ActionObjectType actionObjectType;
	protected bool isAction;
	[SerializeField]
	protected bool isSwitchOn;
	[SerializeField]
	protected bool isAutoReturn;
	[SerializeField]
	protected float autoReturnTime = 5f;
	[SerializeField]
	protected int[] fieldNoArray;
	[SerializeField]
	protected float actionTime = 0.5f;
	protected Hidden_CharacterScript actionChara;
	public bool IsAction => isAction;
	public bool IsSwitchOn => isSwitchOn;
	public bool IsAutoReturn => isAutoReturn;
	public int[] FieldNoArray => fieldNoArray;
	public float ActionTime => actionTime;
	public virtual void PlayAction(Hidden_CharacterScript _chara)
	{
		if (!isAction)
		{
			isAction = true;
			actionChara = _chara;
		}
	}
	protected virtual void EndAction()
	{
		isAction = false;
		isSwitchOn = !isSwitchOn;
		if (actionChara != null)
		{
			actionChara.EndAction();
		}
		if (isAutoReturn && isSwitchOn)
		{
			StartCoroutine(_AutoReturnDirection());
		}
	}
	protected virtual IEnumerator _AutoReturnDirection()
	{
		float timer = 0f;
		while (timer < autoReturnTime)
		{
			timer += Time.deltaTime;
			if (isAction || !isSwitchOn)
			{
				yield break;
			}
			yield return null;
		}
		PlayAction(null);
	}
	public virtual Hidden_CharacterScript.ActionType GetCharaActionType()
	{
		switch (actionObjectType)
		{
		case ActionObjectType.RotY:
			return Hidden_CharacterScript.ActionType.RotY;
		case ActionObjectType.Door:
			return Hidden_CharacterScript.ActionType.Door;
		case ActionObjectType.RotFloor:
			return Hidden_CharacterScript.ActionType.RotFloor;
		case ActionObjectType.Scroll:
			return Hidden_CharacterScript.ActionType.Scroll;
		default:
			return Hidden_CharacterScript.ActionType.RotY;
		}
	}
	public virtual Transform GetActionTrans(Hidden_CharacterScript _chara)
	{
		return base.transform;
	}
	public virtual bool ContainsFieldNo(int _fieldNo)
	{
		for (int i = 0; i < fieldNoArray.Length; i++)
		{
			if (fieldNoArray[i] == _fieldNo)
			{
				return true;
			}
		}
		return false;
	}
	public virtual bool CheckActionObjectType(ActionObjectType _type)
	{
		return actionObjectType == _type;
	}
	protected static Transform SearchNearestAnchor(Transform[] _anchors, Vector3 _pos)
	{
		int num = 0;
		float num2 = (_anchors[0].transform.position - _pos).sqrMagnitude;
		for (int i = 0; i < _anchors.Length; i++)
		{
			float sqrMagnitude = (_anchors[i].transform.position - _pos).sqrMagnitude;
			if (num2 > sqrMagnitude)
			{
				num = i;
				num2 = sqrMagnitude;
			}
		}
		return _anchors[num];
	}
}
