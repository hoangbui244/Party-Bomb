using UnityEngine;
public class GS_CaptionCollection : MonoBehaviour
{
	[SerializeField]
	[Header("コイン表示画像")]
	private SpriteRenderer[] arrayCoin;
	[SerializeField]
	[Header("ゲ\u30fcム種別")]
	private GS_Define.GameType type;
	private int coinType;
	public void Refresh()
	{
		for (int i = 0; i < arrayCoin.Length; i++)
		{
			arrayCoin[i].gameObject.SetActive(value: false);
		}
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			return;
		}
		coinType = 0;
		for (int num = arrayCoin.Length - 1; num >= 0; num--)
		{
			if (SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.GetOpen((TrophyData.Type)num, TrophyData.ConversionGameTypeNo((int)type)))
			{
				arrayCoin[coinType].gameObject.SetActive(value: true);
				switch (num)
				{
				default:
					coinType++;
					break;
				}
			}
		}
	}
}
