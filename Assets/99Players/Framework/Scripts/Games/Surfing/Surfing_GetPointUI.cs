using UnityEngine;
public class Surfing_GetPointUI : MonoBehaviour
{
	[SerializeField]
	[Header("点数表示に使用する画像")]
	public SpriteRenderer[] sp;
	[SerializeField]
	[Header("100の位の画像")]
	public SpriteRenderer spHundredsPlace;
	[SerializeField]
	[Header("100の位の画像(画像入れ替え用リスト)")]
	public Sprite[] spHundredsPlaceList;
	[SerializeField]
	[Header("Nice")]
	public SpriteRenderer spNice;
	[SerializeField]
	[Header("Good")]
	public SpriteRenderer spGood;
	[SerializeField]
	[Header("perfect")]
	public SpriteRenderer spPerfect;
	private Vector3 pos;
	private SpriteRenderer setSp;
	private void Start()
	{
		for (int i = 0; i < sp.Length; i++)
		{
			sp[i].SetAlpha(0f);
		}
		spNice.SetAlpha(0f);
		spGood.SetAlpha(0f);
		spPerfect.SetAlpha(0f);
		pos = base.gameObject.transform.position;
	}
	public void GetPoint(int _point)
	{
		if (_point != 0)
		{
			LeanTween.cancel(base.gameObject);
			base.gameObject.transform.localPosition = pos;
			for (int i = 0; i < sp.Length; i++)
			{
				sp[i].SetAlpha(0f);
			}
			spNice.SetAlpha(0f);
			spGood.SetAlpha(0f);
			spPerfect.SetAlpha(0f);
			switch (_point)
			{
			case 100:
				spHundredsPlace.sprite = spHundredsPlaceList[1];
				setSp = spNice;
				break;
			case 200:
				spHundredsPlace.sprite = spHundredsPlaceList[2];
				setSp = spGood;
				break;
			case 300:
				spHundredsPlace.sprite = spHundredsPlaceList[3];
				break;
			case 400:
				spHundredsPlace.sprite = spHundredsPlaceList[4];
				break;
			case 500:
				spHundredsPlace.sprite = spHundredsPlaceList[5];
				setSp = spPerfect;
				break;
			case 600:
				spHundredsPlace.sprite = spHundredsPlaceList[6];
				break;
			case 700:
				spHundredsPlace.sprite = spHundredsPlaceList[7];
				break;
			case 800:
				spHundredsPlace.sprite = spHundredsPlaceList[8];
				break;
			case 900:
				spHundredsPlace.sprite = spHundredsPlaceList[9];
				break;
			}
			LeanTween.value(base.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float val)
			{
				for (int j = 0; j < sp.Length; j++)
				{
					sp[j].SetAlpha(val);
				}
				setSp.SetAlpha(val);
			});
			LeanTween.moveY(base.gameObject, pos.y + 200f * base.gameObject.transform.localScale.y, 1f);
		}
	}
}
