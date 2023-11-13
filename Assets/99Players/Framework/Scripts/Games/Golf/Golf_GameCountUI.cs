using TMPro;
using UnityEngine;
public class Golf_GameCountUI : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcム回数")]
	private TextMeshPro gameCnt;
	[SerializeField]
	[Header("ゲ\u30fcムの最大回数")]
	private TextMeshPro totalGameCnt;
	private readonly int GAME_CNT_IDX = 5;
	private readonly int TOTAL_GAME_CNT_IDX = 6;
	public void Init()
	{
		totalGameCnt.text = SingletonCustom<TextDataBaseManager>.Instance.GetReplaceTag(TextDataBaseItemEntity.DATABASE_NAME.MAKING_POTION, TOTAL_GAME_CNT_IDX, TextDataBaseItemEntity.TAG_NAME.NUMBER, Golf_Define.TOTAL_GAME_CNT.ToString());
	}
	public void SetGameCnt(int _gameCnt)
	{
		gameCnt.text = SingletonCustom<TextDataBaseManager>.Instance.GetReplaceTag(TextDataBaseItemEntity.DATABASE_NAME.MAKING_POTION, GAME_CNT_IDX, TextDataBaseItemEntity.TAG_NAME.NUMBER, (_gameCnt + 1).ToString());
	}
}
