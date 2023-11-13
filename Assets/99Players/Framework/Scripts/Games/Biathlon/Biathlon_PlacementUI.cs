using UnityEngine;
public class Biathlon_PlacementUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private Vector3 scale = Vector3.one;
	[SerializeField]
	private string useSpriteNameBase = "_common_rank_s_";
	private bool isPrepare;
	private int preparePlacement;
	private LTDescr tween;
	public void Init(int placement)
	{
		SetPlacementImmediate(placement);
		SetAlpha(0f);
	}
	public void SetPlacementImmediate(int placement)
	{
		spriteRenderer.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, GetSpriteNameByPlacement(placement));
		SetAlpha(1f);
		base.transform.localScale = scale;
	}
	public void SetPlacement(int placement)
	{
		if (tween != null)
		{
			isPrepare = true;
			preparePlacement = placement;
			return;
		}
		Sprite sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, GetSpriteNameByPlacement(placement));
		if (!(sprite == spriteRenderer.sprite))
		{
			spriteRenderer.sprite = sprite;
			spriteRenderer.transform.SetLocalScale(0f, 0f, 0f);
			tween = LeanTween.scale(spriteRenderer.gameObject, scale, 0.3f).setEaseInCubic().setIgnoreTimeScale(useUnScaledTime: true)
				.setOnComplete(PreparePlacement);
		}
	}
	public void SetAlpha(float alpha)
	{
		spriteRenderer.SetAlpha(alpha);
	}
	public void Hide()
	{
		spriteRenderer.enabled = false;
	}
	private void PreparePlacement()
	{
		if (isPrepare)
		{
			LeanTween.delayedCall(0.3f, SetPreparePlacement);
		}
	}
	private void SetPreparePlacement()
	{
		isPrepare = false;
		tween = null;
		Sprite sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, GetSpriteNameByPlacement(preparePlacement));
		if (!(sprite == spriteRenderer.sprite))
		{
			spriteRenderer.sprite = sprite;
			spriteRenderer.transform.SetLocalScale(0f, 0f, 0f);
			tween = LeanTween.scale(spriteRenderer.gameObject, scale, 0.3f).setEaseInCubic().setIgnoreTimeScale(useUnScaledTime: true)
				.setOnComplete(PreparePlacement);
		}
	}
	private string GetSpriteNameByPlacement(int placement)
	{
		return $"{useSpriteNameBase}{placement}";
	}
}
