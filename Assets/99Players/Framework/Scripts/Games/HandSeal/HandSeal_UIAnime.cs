using System;
using UnityEngine;
public class HandSeal_UIAnime : MonoBehaviour
{
	[SerializeField]
	[Header("対象のオブジェクト")]
	private Transform[] obj;
	[SerializeField]
	[Header("半径(縦)")]
	public float radius_ver;
	[SerializeField]
	[Header("半径(横)")]
	public float radius_hor;
	[SerializeField]
	[Header("始点角度")]
	public float startDeg;
	[SerializeField]
	[Header("WorldScale(Y軸)の影響を受けるか")]
	public bool isScaleOffset = true;
	private float angleDiff;
	private float revolverDeg;
	private float targetDeg;
	private int revolverID;
	private void Awake()
	{
		angleDiff = 360f / (float)obj.Length;
		revolverDeg = 0f;
		if (isScaleOffset)
		{
			radius_ver *= base.gameObject.transform.lossyScale.y;
			radius_hor *= base.gameObject.transform.lossyScale.y;
		}
		Deploy();
	}
	public void Init()
	{
		LeanTween.cancel(revolverID);
		revolverDeg = 0f;
		targetDeg = 0f;
		Deploy();
	}
	private void OnValidate()
	{
		Deploy();
	}
	[ContextMenu("Deploy")]
	private void Deploy()
	{
		angleDiff = 360f / (float)obj.Length;
		for (int i = 0; i < obj.Length; i++)
		{
			Vector3 position = base.gameObject.transform.position;
			float f = (90f + startDeg + revolverDeg - angleDiff * (float)i) * ((float)Math.PI / 180f);
			position.x += radius_hor * Mathf.Cos(f);
			position.y += radius_ver * Mathf.Sin(f);
			obj[i].position = position;
		}
	}
	[ContextMenu("Revolver")]
	public void Revolver()
	{
		targetDeg += angleDiff;
		revolverID = LeanTween.value(base.gameObject, revolverDeg, targetDeg, 0.3f).setOnUpdate(delegate(float val)
		{
			revolverDeg = val;
			Deploy();
		}).setOnComplete(SetRevolverDeg)
			.id;
		}
		private void SetRevolverDeg()
		{
			revolverDeg = Mathf.Repeat(revolverDeg, 360f);
		}
	}
