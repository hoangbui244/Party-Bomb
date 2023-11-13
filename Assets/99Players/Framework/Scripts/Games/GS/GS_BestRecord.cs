using TMPro;
using UnityEngine;
public class GS_BestRecord : MonoBehaviour
{
	[SerializeField]
	[Header("キャプションテキスト")]
	private TextMeshPro captionText;
	[SerializeField]
	[Header("最高タイムテキスト")]
	private TextMeshPro recordText;
	private string timeStr = "";
	public void Set(GS_Define.GameType _type)
	{
		base.gameObject.SetActive(value: true);
		captionText.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 172));
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
}
