using UnityEngine;
public class HandSeal_LocalizeObjHide : MonoBehaviour
{
	private void Awake()
	{
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
