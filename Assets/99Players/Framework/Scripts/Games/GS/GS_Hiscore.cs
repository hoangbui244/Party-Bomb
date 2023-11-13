using TMPro;
using UnityEngine;
public class GS_Hiscore : MonoBehaviour
{
	[SerializeField]
	[Header("【最高記録】キャプション")]
	private TextMeshPro[] textCaptionHighestRecord;
	[SerializeField]
	[Header("【最高到達ステ\u30fcジ】キャプション")]
	private TextMeshPro textcaptionHighestReach;
	[SerializeField]
	[Header("【人抜き】キャプション")]
	private TextMeshPro textCaptionSwordFight;
	[SerializeField]
	[Header("【ハイスコア】テキスト")]
	private TextMeshPro textHiscore;
	[SerializeField]
	[Header("【最高タイム】テキスト")]
	private TextMeshPro textHighestTime;
	[SerializeField]
	[Header("【チャンバラ】テキスト")]
	private TextMeshPro textSwordFight;
	[SerializeField]
	[Header("【ブックタワ\u30fc】テキスト")]
	private TextMeshPro textBookTower;
	[SerializeField]
	[Header("【ハイスコア】")]
	private GameObject captionHiscore;
	[SerializeField]
	[Header("【最高到達ステ\u30fcジ】")]
	private GameObject captionHighestReach;
	[SerializeField]
	[Header("【最高記録】")]
	private GameObject captionHighestRecord;
	[SerializeField]
	[Header("【最高タイム】")]
	private GameObject captionHighestTime;
	[SerializeField]
	[Header("【チャンバラ】")]
	private GameObject captionSwordFight;
	[SerializeField]
	[Header("【ブックタワ\u30fc】")]
	private GameObject captionBookTower;
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer frame;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private SpriteRenderer backFrame;
	private string timeStr = "";
	public void Set(GS_Define.GameType _type)
	{
		base.gameObject.SetActive(value: true);
		for (int i = 0; i < textCaptionHighestRecord.Length; i++)
		{
			textCaptionHighestRecord[i].SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 173));
		}
		textcaptionHighestReach.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 174));
		textCaptionSwordFight.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 175));
		if (_type == GS_Define.GameType.BLOCK_WIPER)
		{
			textHighestTime.gameObject.SetActive(value: true);
			timeStr = SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get(GS_Define.GameType.BLOCK_WIPER, RecordData.InitType.Score);
			textHighestTime.SetText(timeStr.Equals("59:59.99") ? "00:00.00" : timeStr);
			captionHighestTime.SetActive(value: true);
		}
		SetBackFrame(_type);
	}
	public void Close()
	{
		textHiscore.gameObject.SetActive(value: false);
		textHighestTime.gameObject.SetActive(value: false);
		textSwordFight.gameObject.SetActive(value: false);
		textBookTower.gameObject.SetActive(value: false);
		captionHiscore.SetActive(value: false);
		captionHighestReach.SetActive(value: false);
		captionHighestRecord.SetActive(value: false);
		captionHighestTime.SetActive(value: false);
		captionSwordFight.SetActive(value: false);
		captionBookTower.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}
	private void SetBackFrame(GS_Define.GameType _type)
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
