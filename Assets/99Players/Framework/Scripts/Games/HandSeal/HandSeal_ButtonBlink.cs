using UnityEngine;
public class HandSeal_ButtonBlink : MonoBehaviour
{
	[SerializeField]
	[Header("点滅させる画像")]
	private GameObject sp;
	private void Start()
	{
		sp.GetComponent<SpriteRenderer>().SetAlpha(0f);
	}
	public void BlinkStart()
	{
		LeanTween.cancel(sp);
		sp.GetComponent<SpriteRenderer>().SetAlpha(0f);
		LeanTween.alpha(sp, 1f, 0.8f).setLoopPingPong();
	}
	public void BlinkStop()
	{
		LeanTween.cancel(sp);
		sp.GetComponent<SpriteRenderer>().SetAlpha(0f);
	}
}
