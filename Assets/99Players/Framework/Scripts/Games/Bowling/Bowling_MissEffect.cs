using UnityEngine;
public class Bowling_MissEffect : ThrowResultEffect
{
	[SerializeField]
	[Header("文字画像")]
	private SpriteRenderer[] sprites;
	private Vector3[] defSpriteLocalPos;
	public override void Init()
	{
		base.Init();
		defSpriteLocalPos = new Vector3[sprites.Length];
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].SetAlpha(1f);
			defSpriteLocalPos[i] = sprites[i].transform.localPosition;
			sprites[i].transform.AddLocalPositionY(15f);
		}
		for (int j = 0; j < sprites.Length; j++)
		{
			LeanTween.moveLocalY(sprites[j].gameObject, defSpriteLocalPos[j].y, 1.5f).setEaseOutBounce().setDelay((float)j * 0.25f);
			LeanTween.color(sprites[j].gameObject, new Color(1f, 1f, 1f, 0f), 0.5f).setDelay((float)j * 0.25f + 3f);
		}
	}
}
