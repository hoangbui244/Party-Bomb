using System.Collections;
using UnityEngine;
public class BeachVolley_TextEffect : MonoBehaviour
{
	private float EFFECT_TIME = 2.75f;
	private float EFFECT_REAL_TIME = 1f;
	private float FADE_IN_TIME = 0.3f;
	private float FADE_OUT_TIME = 0.3f;
	private float effectTime;
	[SerializeField]
	[Header("文字アンカ\u30fc")]
	private Transform textAnchor;
	[SerializeField]
	[Header("文字アンカ\u30fc英語")]
	private Transform textAnchor_EN;
	private void Start()
	{
		StartCoroutine(_MoveText());
	}
	private IEnumerator _MoveText()
	{
		Transform anchor = textAnchor_EN;
		if (Localize_Define.Language == Localize_Define.LanguageType.Japanese)
		{
			anchor = textAnchor;
		}
		float defLocal = anchor.localPosition.x;
		LeanTween.moveLocalX(anchor.gameObject, 0f, FADE_IN_TIME).setEaseOutBack();
		yield return new WaitForSeconds(FADE_IN_TIME + EFFECT_REAL_TIME);
		LeanTween.moveLocalX(anchor.gameObject, 0f - defLocal, FADE_OUT_TIME).setEaseOutQuad();
	}
	private void Update()
	{
		effectTime += Time.deltaTime;
		if (effectTime >= EFFECT_TIME)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	public bool IsViewDisplay()
	{
		return effectTime < EFFECT_REAL_TIME;
	}
	public float RemainViewDisplayTime()
	{
		return Mathf.Max(EFFECT_REAL_TIME - effectTime, 0f);
	}
}
