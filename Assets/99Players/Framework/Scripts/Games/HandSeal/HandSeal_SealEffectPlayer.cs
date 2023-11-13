using UnityEngine;
public class HandSeal_SealEffectPlayer : MonoBehaviour
{
	[SerializeField]
	[Header("印エフェクト")]
	private SpriteRenderer[] spSeal = new SpriteRenderer[9];
	private int id_1;
	private int id_2;
	private void Start()
	{
		for (int i = 0; i < spSeal.Length; i++)
		{
			spSeal[i].SetAlpha(0f);
		}
	}
	public void PlaySealEffect(HandSeal_Hand.Kuji _set)
	{
		switch (_set)
		{
		case HandSeal_Hand.Kuji.Rin:
			id_1 = LeanTween.alpha(spSeal[0].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[0].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Hyou:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[0].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[1].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[1].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Tou:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[1].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[2].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[2].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Sya:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[2].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[3].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[3].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Kai:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[3].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[4].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[4].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Jin:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[4].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[5].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[5].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Retsu:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[5].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[6].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[6].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Zai:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[6].SetAlpha(0f);
			id_1 = LeanTween.alpha(spSeal[7].gameObject, 1f, 0.2f).id;
			id_2 = LeanTween.alpha(spSeal[7].gameObject, 0f, 0.2f).setDelay(0.6f).id;
			break;
		case HandSeal_Hand.Kuji.Zen:
			LeanTween.cancel(id_1);
			LeanTween.cancel(id_2);
			spSeal[7].SetAlpha(0f);
			LeanTween.alpha(spSeal[8].gameObject, 1f, 0.2f);
			LeanTween.alpha(spSeal[8].gameObject, 0f, 0.2f).setDelay(0.6f);
			break;
		}
	}
}
