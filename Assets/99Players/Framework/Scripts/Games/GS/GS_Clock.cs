using System;
using UnityEngine;
public class GS_Clock : MonoBehaviour
{
	[SerializeField]
	[Header("長針")]
	private GameObject longNeedle;
	[SerializeField]
	[Header("短針")]
	private GameObject shortNeedle;
	private DateTime dt;
	private void SetClockNeedle()
	{
		dt = SingletonCustom<GS_GameSelectManager>.Instance.GetDateTime();
		longNeedle.transform.SetLocalEulerAnglesZ((float)dt.Minute / 60f * -360f);
		shortNeedle.transform.SetLocalEulerAnglesZ((float)dt.Hour / 12f * -360f + (float)dt.Minute / 60f * -30f);
	}
	private void OnEnable()
	{
		SetClockNeedle();
	}
	private void LateUpdate()
	{
		SetClockNeedle();
	}
}
