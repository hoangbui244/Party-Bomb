using UnityEngine;
public class SmeltFishing_OperationUI : MonoBehaviour
{
	private GameObject[] arrayControl;
	[SerializeField]
	private GameObject[] arrayControl_JP;
	[SerializeField]
	private GameObject[] arrayControl_EN;
	public void Init()
	{
		for (int i = 0; i < arrayControl_JP.Length; i++)
		{
			arrayControl_JP[i].SetActive(value: false);
		}
		for (int j = 0; j < arrayControl_EN.Length; j++)
		{
			arrayControl_EN[j].SetActive(value: false);
		}
		arrayControl = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayControl_JP : arrayControl_EN);
		ShowMoveControl();
		SetActionControlAsSitDown();
		HideCancelControl();
	}
	public void ShowMoveControl()
	{
	}
	public void HideMoveControl()
	{
	}
	public void SetActionControlAsSitDown()
	{
		for (int i = 0; i < arrayControl.Length; i++)
		{
			arrayControl[i].SetActive(i == 0);
		}
	}
	public void SetActionControlAsCastLine()
	{
		for (int i = 0; i < arrayControl.Length; i++)
		{
			arrayControl[i].SetActive(i == 1);
		}
	}
	public void SetActionControlAsRollUp()
	{
		for (int i = 0; i < arrayControl.Length; i++)
		{
			arrayControl[i].SetActive(i == 2);
		}
	}
	public void ShowCancelControl()
	{
	}
	public void HideCancelControl()
	{
	}
	public void ShowPullUpControl()
	{
	}
	public void HidePullUpControl()
	{
	}
}
