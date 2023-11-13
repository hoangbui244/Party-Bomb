using TMPro;
using UnityEngine;
public class GS_GameInfo : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム名画像")]
	private GameObject[] arrayTextGameTitle;
	[SerializeField]
	[Header("通常背景フレ\u30fcム")]
	private GameObject frameDefault;
	[SerializeField]
	[Header("短縮背景フレ\u30fcム")]
	private GameObject frameSmall;
	[SerializeField]
	[Header("ショ\u30fcト表示オブジェクト")]
	private GameObject objShort;
	[SerializeField]
	[Header("ロング表示オブジェクト")]
	private GameObject objLong;
	[SerializeField]
	[Header("ショ\u30fcトテキスト")]
	private TextMeshPro textShort;
	[SerializeField]
	[Header("ロングテキスト")]
	private TextMeshPro textLong;
	[SerializeField]
	[Header("ボ\u30fcド背景（ショ\u30fcト）")]
	private SpriteRenderer boardShort;
	[SerializeField]
	[Header("ボ\u30fcド背景（ロング）")]
	private SpriteRenderer boardLong;
	private readonly float TITLE_POS_SHORT = -4f;
	private readonly float TITLE_POS_LONG = -16f;
	public void SetShort(GS_Define.GameType _type, bool _messageFrameShort)
	{
		frameDefault.SetActive(value: false);
		frameSmall.SetActive(value: true);
		objShort.SetActive(_messageFrameShort);
		objLong.SetActive(!_messageFrameShort);
		if (objShort.activeSelf)
		{
			textShort.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(40 + _type)));
		}
		if (objLong.activeSelf)
		{
			textLong.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(40 + _type)));
		}
		SetTitle(_type, _isShort: true);
		SetBoard(_type);
	}
	public void SetLong(GS_Define.GameType _type, bool _messageFrameShort)
	{
		frameDefault.SetActive(value: true);
		frameSmall.SetActive(value: false);
		objShort.SetActive(_messageFrameShort);
		objLong.SetActive(!_messageFrameShort);
		if (objShort.activeSelf)
		{
			textShort.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(40 + _type)));
		}
		if (objLong.activeSelf)
		{
			textLong.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(40 + _type)));
		}
		SetTitle(_type);
		SetBoard(_type);
	}
	private void SetTitle(GS_Define.GameType _type, bool _isShort = false)
	{
		for (int i = 0; i < arrayTextGameTitle.Length; i++)
		{
			arrayTextGameTitle[i].SetActive(value: false);
		}
		arrayTextGameTitle[(int)_type].SetActive(value: true);
		arrayTextGameTitle[(int)_type].transform.SetLocalPositionY(_isShort ? TITLE_POS_SHORT : TITLE_POS_LONG);
	}
	private void SetBoard(GS_Define.GameType _type)
	{
		switch (_type)
		{
		case GS_Define.GameType.GET_BALL:
		case GS_Define.GameType.CANNON_SHOT:
		case GS_Define.GameType.BLOCK_WIPER:
		case GS_Define.GameType.MOLE_HAMMER:
		case GS_Define.GameType.BOMB_ROULETTE:
		case GS_Define.GameType.RECEIVE_PON:
			return;
		}
	}
}
