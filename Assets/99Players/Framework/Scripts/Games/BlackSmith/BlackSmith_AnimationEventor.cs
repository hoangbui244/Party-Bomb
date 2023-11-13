using UnityEngine;
public class BlackSmith_AnimationEventor : MonoBehaviour
{
	private BlackSmith_Field field;
	public void Init(BlackSmith_Field _field)
	{
		field = _field;
	}
	public void StartGaugeFadeIn()
	{
		field.StartGaugeFadeIn();
	}
	public void CompleteFadeIn()
	{
		field.CompleteFadeIn();
	}
	public void CompleteFadeOut()
	{
		field.CompleteFadeOut();
	}
}
