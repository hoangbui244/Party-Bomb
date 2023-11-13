using System;
using UnityEngine;
public class FlyingSquirrelRace_ObstacleObject_CallAction : MonoBehaviour
{
	private Action callBack;
	public void Initialize(Action _callBack)
	{
		callBack = _callBack;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (callBack != null)
		{
			callBack();
		}
	}
}
