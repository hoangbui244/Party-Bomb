using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Timer : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("タイム表示")]
	private CommonGameTimeUI_Font_Time timeUI;
	private int user;
	public void Initialize(int controlUser)
	{
		user = controlUser;
		UpdateTime(0f);
	}
	public void UpdateTime(float time)
	{
		timeUI.SetTime(time, user);
	}
}
