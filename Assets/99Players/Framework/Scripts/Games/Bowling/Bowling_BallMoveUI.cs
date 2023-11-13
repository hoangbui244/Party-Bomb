using UnityEngine;
public class Bowling_BallMoveUI : MonoBehaviour
{
	[SerializeField]
	[Header("ボ\u30fcル移動矢印親アンカ\u30fc")]
	private Transform ballMoveArrowRootAnchor;
	[SerializeField]
	[Header("ボ\u30fcル移動矢印アンカ\u30fc")]
	private Transform[] ballMoveArrowAnchor;
	[SerializeField]
	[Header("ボ\u30fcル移動矢印")]
	private SpriteRenderer[] ballMoveArrow;
	public void Init()
	{
		SetActiveThrowArrow(_value: false);
		for (int i = 0; i < ballMoveArrowAnchor.Length; i++)
		{
			LeanTween.moveLocalX(ballMoveArrowAnchor[i].gameObject, ballMoveArrowAnchor[i].transform.localPosition.x + 0.025f * ((i == 0) ? (-1f) : 1f), 0.75f).setEaseInOutQuad().setLoopPingPong();
		}
	}
	public void FadeInBallMoveArrow(float _delay)
	{
		for (int i = 0; i < ballMoveArrow.Length; i++)
		{
			LeanTween.cancel(ballMoveArrow[i].gameObject);
			LeanTween.color(ballMoveArrow[i].gameObject, new Color(1f, 1f, 1f, 1f), 0.25f).setDelay(_delay);
		}
	}
	public void FadeOutBallMoveArrow(float _delay)
	{
		for (int i = 0; i < ballMoveArrow.Length; i++)
		{
			LeanTween.cancel(ballMoveArrow[i].gameObject);
			LeanTween.color(ballMoveArrow[i].gameObject, new Color(1f, 1f, 1f, 0f), 0.25f).setDelay(_delay);
		}
	}
	public void SetBallArrowPos(Vector3 _ballPos)
	{
		ballMoveArrowRootAnchor.SetPositionX(_ballPos.x);
	}
	public void SetActiveThrowArrow(bool _value, bool _standby = false)
	{
		if (_standby)
		{
			ballMoveArrowRootAnchor.SetLocalPositionX(0f);
		}
		if (_value)
		{
			for (int i = 0; i < ballMoveArrow.Length; i++)
			{
				ballMoveArrow[i].color = new Color(1f, 1f, 1f, 0f);
				ballMoveArrow[i].gameObject.SetActive(value: true);
			}
			FadeInBallMoveArrow(0f);
		}
		else
		{
			for (int j = 0; j < ballMoveArrow.Length; j++)
			{
				ballMoveArrow[j].gameObject.SetActive(value: false);
			}
		}
	}
}
