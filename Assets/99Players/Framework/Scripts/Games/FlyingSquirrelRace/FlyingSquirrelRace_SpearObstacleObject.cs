using System;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_SpearObstacleObject : FlyingSquirrelRace_ObstacleObject
{
	private enum Type
	{
		Type_0,
		Type_1
	}
	[SerializeField]
	[Header("種類")]
	private Type type;
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed = 2f;
	[SerializeField]
	[DisplayName("移動させる座標")]
	private Vector3 movePos;
	private Vector3 origin;
	private Vector3 targetPos;
	[SerializeField]
	[Header("動作可能判定にするクラス")]
	private FlyingSquirrelRace_ObstacleObject_CallAction callAcition;
	private bool isAction;
	[SerializeField]
	[DisplayName("レンダラ\u30fc")]
	private Renderer renderer;
	[SerializeField]
	[DisplayName("マテリアル")]
	private Material[] materials;
	private float time;
	private bool isPushUp;
	private bool isNextActionWait;
	[SerializeField]
	[Header("次に突き上げるまでの待機時間")]
	private float nextActionWaitTime = 0.5f;
	protected override void OnInitialize()
	{
		origin = base.transform.localPosition;
		targetPos = origin + movePos;
		switch (type)
		{
		case Type.Type_0:
			callAcition.Initialize(SetIsAction);
			break;
		case Type.Type_1:
			SetIsAction();
			break;
		}
		isPushUp = true;
	}
	protected override void OnFixedUpdateMethod(float speed)
	{
		if (!isAction || isNextActionWait)
		{
			return;
		}
		if (!isPushUp && base.transform.localPosition == origin)
		{
			isPushUp = true;
			isNextActionWait = true;
			LeanTween.delayedCall(base.gameObject, nextActionWaitTime, (Action)delegate
			{
				isNextActionWait = false;
			});
			return;
		}
		if (isPushUp && base.transform.localPosition == targetPos)
		{
			switch (type)
			{
			case Type.Type_0:
				isAction = false;
				return;
			case Type.Type_1:
				isPushUp = false;
				break;
			}
		}
		if (isPushUp)
		{
			time += moveSpeed * Time.fixedDeltaTime;
		}
		else
		{
			time -= moveSpeed * 0.5f * Time.fixedDeltaTime;
		}
		base.transform.localPosition = Vector3.Lerp(origin, targetPos, time);
	}
	public void SetIsAction()
	{
		isAction = true;
		callAcition.gameObject.SetActive(value: false);
	}
}
