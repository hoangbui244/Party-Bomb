using UnityEngine;
public class GS_CharacterSelect : MonoBehaviour
{
	[SerializeField]
	[Header("レイアウト_1人")]
	private GS_CharacterSelectLayoutOne layoutOne;
	[SerializeField]
	[Header("レイアウト_2人")]
	private GS_CharacterSelectLayoutTwo layoutTwo;
	[SerializeField]
	[Header("レイアウト_3人")]
	private GS_CharacterSelectLayoutThree layoutThree;
	[SerializeField]
	[Header("レイアウト_4人")]
	private GS_CharacterSelectLayoutFour layoutFour;
	public void Show()
	{
		base.gameObject.SetActive(value: true);
		SingletonCustom<GameSettingManager>.Instance.IsCpuFixSelect = false;
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			layoutOne.Show();
			break;
		case 2:
			layoutTwo.Show();
			break;
		case 3:
			layoutThree.Show();
			break;
		case 4:
			layoutFour.Show();
			break;
		}
	}
	private void OnEnable()
	{
		Show();
	}
	public void Hide()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			layoutOne.Hide();
			break;
		case 2:
			layoutTwo.Hide();
			break;
		case 3:
			layoutThree.Hide();
			break;
		case 4:
			layoutFour.Hide();
			break;
		}
		base.gameObject.SetActive(value: false);
	}
	private void Update()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			layoutOne.UpdateMethod();
			break;
		case 2:
			layoutTwo.UpdateMethod();
			break;
		case 3:
			layoutThree.UpdateMethod();
			break;
		case 4:
			layoutFour.UpdateMethod();
			break;
		}
	}
}
