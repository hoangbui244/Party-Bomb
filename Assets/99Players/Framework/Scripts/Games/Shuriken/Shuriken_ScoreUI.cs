using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ScoreUI : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("スコア数値表示")]
	private SpriteNumbers spriteNumberses;
	public void Initialize()
	{
		SetPoint(0);
	}
	public void SetPoint(int point)
	{
		spriteNumberses.Set(point);
	}
}
