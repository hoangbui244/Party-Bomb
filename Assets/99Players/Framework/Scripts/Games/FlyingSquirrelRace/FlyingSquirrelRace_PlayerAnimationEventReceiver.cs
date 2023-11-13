using UnityEngine;
public class FlyingSquirrelRace_PlayerAnimationEventReceiver : MonoBehaviour
{
	[SerializeField]
	private FlyingSquirrelRace_PlayerAnimation playerAnimation;
	public void PlaySmokeEffect()
	{
		playerAnimation.PlaySmokeEffect();
	}
	public void ChangeCharacterObject()
	{
		playerAnimation.ChangeCharacterObject();
	}
	public void PlayLandingSmokeEffect()
	{
		playerAnimation.PlayLandingSmokeEffect();
	}
}
