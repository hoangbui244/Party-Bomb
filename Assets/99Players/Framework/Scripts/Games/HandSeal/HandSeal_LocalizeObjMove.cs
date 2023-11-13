using UnityEngine;
public class HandSeal_LocalizeObjMove : MonoBehaviour
{
	[SerializeField]
	[Header("移動するPosition")]
	private Vector3 pos;
	[SerializeField]
	[Header("画面4分割時のみ移動させるフラグ")]
	private bool isfourDiv;
	private void Awake()
	{
		if (Localize_Define.Language != Localize_Define.LanguageType.English)
		{
			return;
		}
		if (isfourDiv)
		{
			if (HandSeal_Define.PLAYER_NUM >= 3)
			{
				base.gameObject.transform.localPosition = pos;
			}
		}
		else
		{
			base.gameObject.transform.localPosition = pos;
		}
	}
}
