using System;
using UnityEngine;
public class MakingPotion_Rod : MonoBehaviour
{
	public enum CreateState
	{
		StartWait,
		Create,
		End,
		Interval
	}
	private int charaNo;
	private MaterialPropertyBlock pb;
	private CreateState createState;
	private Vector3 startLocalPos;
	private Quaternion startLocalRot;
	private Vector3 createViewPos;
	private Vector3 createViewEuler;
	public void Init(int _charaNo)
	{
		charaNo = _charaNo;
		startLocalPos = base.transform.localPosition;
		startLocalRot = base.transform.localRotation;
		Transform transform = SingletonCustom<MakingPotion_GameManager>.Instance.GetCamera(_charaNo).transform;
		createViewPos = transform.position + transform.forward * 0.5f;
		createViewEuler = transform.eulerAngles;
		DataInit();
	}
	public void SecondGroupInit()
	{
		base.transform.localPosition = startLocalPos;
		base.transform.localRotation = startLocalRot;
		DataInit();
	}
	private void DataInit()
	{
		if (pb == null)
		{
			pb = new MaterialPropertyBlock();
		}
		createState = CreateState.StartWait;
	}
	public void InitTrans()
	{
		base.transform.localPosition = startLocalPos;
		base.transform.localRotation = startLocalRot;
	}
	public CreateState GetCreateState()
	{
		return createState;
	}
	private Color GetCottonColor(MakingPotion_PlayerScript.SugarColorType _colorType)
	{
		return SingletonCustom<MakingPotion_PlayerManager>.Instance.GetCottonColor(_colorType);
	}
	public void SetPos(Vector3 _pos, bool _isChangeY = false)
	{
		if (!_isChangeY)
		{
			_pos.y = base.transform.localPosition.y;
		}
		base.transform.localPosition = _pos;
	}
	public void SetCreateState(CreateState _state)
	{
		createState = _state;
	}
	public bool CheckCreateState(CreateState _state)
	{
		return createState == _state;
	}
	public void CreateEnd()
	{
		LeanTween.rotate(base.gameObject, createViewEuler, 0.5f);
		LeanTween.move(base.gameObject, createViewPos, 0.5f).setOnComplete((Action)delegate
		{
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				if (SingletonCustom<MakingPotion_PlayerManager>.Instance.GetIsPlayer(charaNo))
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
				}
			});
			LeanTween.delayedCall(1.5f, (Action)delegate
			{
				SingletonCustom<MakingPotion_PlayerManager>.Instance.TargetUpdate(charaNo);
				LeanTween.moveLocalY(base.gameObject, base.transform.localPosition.y + 2f, 0.5f).setOnComplete((Action)delegate
				{
					createState = CreateState.StartWait;
				});
			});
		});
	}
}
