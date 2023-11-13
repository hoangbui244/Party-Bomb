using UnityEngine;
public class SmeltFishing_OperationInformationSwticher : MonoBehaviour
{
	private SmeltFishing_OperationInformation[] information;
	[SerializeField]
	private SmeltFishing_OperationInformation[] information_JP;
	[SerializeField]
	private SmeltFishing_OperationInformation[] information_EN;
	private bool[] arrayIsOnceShow;
	private SmeltFishing_OperationInformation latest;
	public void Init()
	{
		for (int i = 0; i < information_JP.Length; i++)
		{
			information_JP[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < information_EN.Length; j++)
		{
			information_EN[j].gameObject.SetActive(value: false);
		}
		information = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? information_JP : information_EN);
		for (int k = 0; k < information.Length; k++)
		{
			information[k].gameObject.SetActive(value: true);
		}
		SmeltFishing_OperationInformation[] array = information;
		for (int l = 0; l < array.Length; l++)
		{
			array[l].Init();
		}
		arrayIsOnceShow = new bool[information.Length];
	}
	public void ShowOperationInformation(int infoIndex)
	{
		if (!arrayIsOnceShow[infoIndex])
		{
			arrayIsOnceShow[infoIndex] = true;
			if ((bool)latest)
			{
				latest.ForceHide();
			}
			latest = information[infoIndex];
			latest.Show();
		}
	}
	public bool IsOnceOperationInformation(int infoIndex)
	{
		return arrayIsOnceShow[infoIndex];
	}
	public void HideOperationInformation()
	{
		if (!(latest == null))
		{
			latest.Hide();
		}
	}
}
