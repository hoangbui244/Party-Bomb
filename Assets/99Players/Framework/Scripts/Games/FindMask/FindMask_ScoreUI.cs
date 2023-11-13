using UnityEngine;
public class FindMask_ScoreUI : MonoBehaviour
{
	[SerializeField]
	private SpriteNumbers spriteNumbers;
	public SpriteNumbers SpriteNumbers => spriteNumbers;
	public void SetScoreText(int _score)
	{
		spriteNumbers.Set(_score);
	}
}
