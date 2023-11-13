using System;
using UnityEngine;
public class ArenaBattleFieldManager : SingletonCustom<ArenaBattleFieldManager>
{
	[SerializeField]
	[Header("中央位置アンカ\u30fc")]
	private Transform anchorCenter;
	[SerializeField]
	[Header("吊り橋アニメ\u30fcタ\u30fc")]
	private Animator[] arrayDrawBridgeAnimator;
	[SerializeField]
	[Header("リングコライダ\u30fc")]
	private GameObject objRingCol;
	[SerializeField]
	[Header("壁モデル")]
	private GameObject objWall;
	[SerializeField]
	[Header("壁アニメ\u30fcタ\u30fc")]
	private Animator animatorWall;
	[SerializeField]
	[Header("煙エフェクト")]
	private ParticleSystem psSmoke;
	public Transform AnchorCenter => anchorCenter;
	public bool IsWallLimit
	{
		get;
		set;
	}
	public void PlayDrawBridge()
	{
		for (int i = 0; i < arrayDrawBridgeAnimator.Length; i++)
		{
			arrayDrawBridgeAnimator[i].SetTrigger("Up");
		}
		objRingCol.SetActive(value: true);
	}
	public void StartWallLimit()
	{
		LeanTween.scaleY(objWall, 1f, 3f).setOnComplete((Action)delegate
		{
			psSmoke.Stop();
		});
		LeanTween.value(base.gameObject, 1f, 0.65f, 1.5f).setOnUpdate(delegate(float _value)
		{
			objRingCol.transform.SetLocalScale(_value, 1f, _value);
		});
		animatorWall.enabled = true;
		SingletonCustom<AudioManager>.Instance.SePlay("se_wall");
		psSmoke.Play();
		IsWallLimit = true;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(objWall);
		LeanTween.cancel(base.gameObject);
	}
}
