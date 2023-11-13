using UnityEngine;
public class GS_Teacher : MonoBehaviour
{
	[SerializeField]
	[Header("キャラレンダラ")]
	private SpriteRenderer[] arrayCharacter;
	[SerializeField]
	[Header("キャラ画像")]
	private Sprite[] arrayCharacterSp;
	[SerializeField]
	[Header("キャラ座標")]
	private Vector3[] arrayCharacterPos;
	[SerializeField]
	[Header("総合位置オフセット")]
	private Vector3 offset;
	[SerializeField]
	[Header("名前表示ル\u30fcト")]
	private GameObject rootPlate;
	[SerializeField]
	[Header("名前スプライト")]
	private SpriteRenderer nameSp;
	[SerializeField]
	[Header("背景スプライト")]
	private SpriteRenderer frameSp;
	[SerializeField]
	[Header("科目スプライト")]
	private SpriteRenderer captionSp;
	private int teacherIdx;
	public void Set(GS_Define.GameType _type)
	{
		UnityEngine.Debug.Log("【キャラ】:" + _type.ToString());
		_type = GS_Define.GameType.GET_BALL;
		if (_type == GS_Define.GameType.MAX)
		{
			arrayCharacter[0].gameObject.SetActive(value: true);
			LeanTween.cancel(arrayCharacter[0].gameObject);
			arrayCharacter[0].transform.SetLocalPositionX(1700f);
			arrayCharacter[0].transform.SetLocalPositionY(arrayCharacterPos[0].y + offset.y);
			LeanTween.moveLocalX(arrayCharacter[0].gameObject, arrayCharacterPos[0].x + offset.x, 0.35f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
			return;
		}
		teacherIdx = (int)_type;
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			arrayCharacter[i].gameObject.SetActive(value: false);
		}
		arrayCharacter[(int)_type].gameObject.SetActive(value: true);
		LeanTween.cancel(arrayCharacter[(int)_type].gameObject);
		arrayCharacter[(int)_type].transform.SetLocalPositionX(1700f);
		arrayCharacter[(int)_type].transform.SetLocalPositionY(arrayCharacterPos[(int)_type].y + offset.y);
		LeanTween.moveLocalX(arrayCharacter[(int)_type].gameObject, arrayCharacterPos[(int)_type].x + offset.x, 0.35f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
	}
	public void Close()
	{
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			arrayCharacter[i].gameObject.SetActive(value: false);
		}
		if (rootPlate != null)
		{
			rootPlate.SetActive(value: false);
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(arrayCharacter[teacherIdx].gameObject);
		if (rootPlate != null)
		{
			LeanTween.cancel(rootPlate);
		}
	}
}
