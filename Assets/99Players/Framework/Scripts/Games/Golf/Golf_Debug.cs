using TMPro;
using UnityEngine;
public class Golf_Debug : SingletonCustom<Golf_Debug>
{
	public TextMeshPro distanceCarryText;
	public GameObject swingAnimation;
	private void Awake()
	{
		SetSwingAnimationActive(_isActive: false);
	}
	private void Update()
	{
		if (distanceCarryText.gameObject.activeInHierarchy)
		{
			DebugDistanceCarry();
		}
	}
	public void DebugDistanceCarry()
	{
		Golf_Ball turnPlayerBall = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		float distanceCarry = SingletonCustom<Golf_BallManager>.Instance.GetDistanceCarry(turnPlayerBall.transform.position);
		distanceCarryText.text = ((int)distanceCarry).ToString();
	}
	public void SetSwingAnimationActive(bool _isActive)
	{
		swingAnimation.SetActive(_isActive);
	}
}
