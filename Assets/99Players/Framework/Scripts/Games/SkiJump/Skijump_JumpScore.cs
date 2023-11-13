using System;
using System.Collections;
using UnityEngine;
public class Skijump_JumpScore : MonoBehaviour
{
	[SerializeField]
	[Header("飛距離スコア")]
	private SpriteNumbers textDistanceScore;
	[SerializeField]
	[Header("飛型点スコア")]
	private SpriteNumbers textBalanceScore;
	[SerializeField]
	[Header("合計スコア")]
	private SpriteNumbers textTotalScore;
	public void Show(int _distanceScore, int _balanceScore)
	{
		base.gameObject.SetActive(value: true);
		base.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.75f).setEaseOutBack().setDelay(0.25f)
			.setOnComplete((Action)delegate
			{
				StartCoroutine(_SetScore(_distanceScore, _balanceScore));
			});
		textDistanceScore.Set(0);
		textBalanceScore.Set(0);
		textTotalScore.Set(0);
	}
	private IEnumerator _SetScore(int _distanceScore, int _balanceScore)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
		LeanTween.value(0f, _distanceScore, 0.5f).setOnUpdate(delegate(float _value)
		{
			textDistanceScore.Set((int)_value);
			textTotalScore.Set((int)_value);
		});
		yield return new WaitForSeconds(1.05f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
		LeanTween.value(0f, _balanceScore, 0.5f).setOnUpdate(delegate(float _value)
		{
			textBalanceScore.Set((int)_value);
			textTotalScore.Set(_distanceScore + (int)_value);
		});
		yield return new WaitForSeconds(0.5f);
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
