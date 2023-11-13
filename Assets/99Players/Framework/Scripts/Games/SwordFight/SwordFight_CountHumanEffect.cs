using System.Collections;
using UnityEngine;
public class SwordFight_CountHumanEffect : MonoBehaviour
{
	private float EFFECT_TIME = 3f;
	private float effectTime;
	[SerializeField]
	[Header("文字アンカ\u30fc")]
	private Transform textAnchor;
	[SerializeField]
	[Header("～人目の数値")]
	private SpriteNumbers countHumanNumbers;
	private void Start()
	{
		StartCoroutine(_MoveText());
	}
	public void SetCountHuman(int _count)
	{
		countHumanNumbers.Set(_count);
		if (_count >= 10)
		{
			countHumanNumbers.transform.SetLocalPositionX(-206f);
		}
		else
		{
			countHumanNumbers.transform.SetLocalPositionX(-301f);
		}
	}
	private IEnumerator _MoveText()
	{
		float defLocal = textAnchor.localPosition.x;
		LeanTween.moveLocalX(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
		yield return new WaitForSeconds(2f);
		LeanTween.moveLocalX(textAnchor.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad();
	}
	private void Update()
	{
		effectTime += Time.deltaTime;
		if (effectTime >= EFFECT_TIME)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
