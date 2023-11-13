using System;
using UnityEngine;
public class BlackSmith_Field : MonoBehaviour
{
	[SerializeField]
	[Header("武器を格納するアンカ\u30fc")]
	private Transform weaponAnchor;
	[SerializeField]
	[Header("FadeIn Animator")]
	private Animator fadeInAnimator;
	private Action gaugeFadeInCallBack;
	private Action fadeInCompleteCallBack;
	[SerializeField]
	[Header("フェ\u30fcドイン時に武器を格納するアンカ\u30fc")]
	private Transform fadeInWeaponAnchor;
	[SerializeField]
	[Header("FadeOut Animator")]
	private Animator fadeOutAnimator;
	private Action fadeOutCompleteCallBack;
	[SerializeField]
	[Header("フェ\u30fcドアウト時に武器を格納するアンカ\u30fc")]
	private Transform fadeOutWeaponAnchor;
	public void Init()
	{
		fadeInAnimator.GetComponent<BlackSmith_AnimationEventor>().Init(this);
		fadeOutAnimator.GetComponent<BlackSmith_AnimationEventor>().Init(this);
	}
	public Transform GetCreateWeaponAnchor()
	{
		return weaponAnchor;
	}
	public void PlayFadeInAnimation(Action _gaugeFadeInCallBack = null, Action _callBack = null)
	{
		fadeInAnimator.SetTrigger("ToFade");
		gaugeFadeInCallBack = _gaugeFadeInCallBack;
		fadeInCompleteCallBack = _callBack;
	}
	public void PlayFadeOutAnimation(Action _callBack = null)
	{
		fadeOutAnimator.SetTrigger("ToFade");
		fadeOutCompleteCallBack = _callBack;
	}
	public void StartGaugeFadeIn()
	{
		if (gaugeFadeInCallBack != null)
		{
			gaugeFadeInCallBack();
		}
	}
	public void CompleteFadeIn()
	{
		if (fadeInCompleteCallBack != null)
		{
			fadeInCompleteCallBack();
		}
	}
	public void CompleteFadeOut()
	{
		if (fadeOutCompleteCallBack != null)
		{
			fadeOutCompleteCallBack();
		}
	}
	public Transform GetFadeInAnchor()
	{
		return fadeInWeaponAnchor;
	}
	public Transform GetFadeOutAnchor()
	{
		return fadeOutWeaponAnchor;
	}
}
