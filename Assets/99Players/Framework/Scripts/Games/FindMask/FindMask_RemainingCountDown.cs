using UnityEngine;
public class FindMask_RemainingCountDown : MonoBehaviour
{
	private int currentSecond;
	[SerializeField]
	[Header("カウントダウンの画像")]
	private SpriteRenderer countSp;
	[SerializeField]
	[Header("カウントダウンの画像名")]
	private Sprite[] arrayCountSpName;
	public void Init()
	{
		currentSecond = -1;
		countSp.transform.localScale = Vector3.zero;
	}
	public void SetAcitveCountSp(bool _isActive)
	{
		countSp.gameObject.SetActive(_isActive);
	}
	public void SetTime(float _time)
	{
		if (currentSecond != (int)_time)
		{
			currentSecond = (int)_time;
			countSp.sprite = arrayCountSpName[currentSecond];
			countSp.transform.localScale = Vector3.zero;
			LeanTween.scale(countSp.gameObject, Vector3.one, 0.5f).setEaseOutBack();
		}
	}
}
