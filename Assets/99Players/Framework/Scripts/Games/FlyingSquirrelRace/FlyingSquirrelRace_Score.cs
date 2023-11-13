using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Score : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("スコア表示")]
	private SpriteNumbers numbers;
	public void Initialize()
	{
		UpdateScore(0);
	}
	public void UpdateScore(int score)
	{
		numbers.Set(score);
	}
}
