using UnityEngine;
public class Biathlon_LapUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer currentLapRenderer;
	[SerializeField]
	private SpriteRenderer totalLapRenderer;
	[SerializeField]
	private Sprite[] numberSprites;
	public void Init()
	{
		SetLap(1);
		totalLapRenderer.sprite = numberSprites[SingletonCustom<Biathlon_Courses>.Instance.Current.RaceLap];
	}
	public void SetLap(int lap)
	{
		currentLapRenderer.sprite = numberSprites[lap];
	}
}
