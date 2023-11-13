using System;
using System.Collections;
using UnityEngine;
public class BeachVolley_AddScoreProduction : MonoBehaviour
{
	[Serializable]
	public struct ScoreObj
	{
		[Header("スコアアンカ\u30fc")]
		public Transform scoreAnchor;
		[Header("スコアリスト")]
		public SpriteRenderer[] score;
		[Header("加点前のスコアアンカ\u30fc")]
		public Transform beforeScoreAnchor;
		[Header("加点前のプレイヤ\u30fcスコア")]
		public SpriteRenderer[] beforeScore;
	}
	[SerializeField]
	[Header("加点時の学校名テキスト")]
	private ScoreObj[] scoreObj = new ScoreObj[2];
	[SerializeField]
	[Header("加点時のスコアオブジェクト")]
	private GameObject addScoreObj;
	[SerializeField]
	[Header("ハイフン")]
	private SpriteRenderer hyphen;
	private bool isChangeScoreColor;
	[SerializeField]
	[Header("文字間")]
	private float ScoreSpace = 73.5f;
	[SerializeField]
	[Header("２桁時の文字サイズ")]
	private float SmallCharSize = 0.775f;
	[SerializeField]
	[Header("数字スプライト")]
	private Sprite[] numberSprites;
	private float showTime = 1.65f;
	public float SHOW_TIME => showTime;
	public void ShowAddScoreObj(int _teamNo, string[] _schoolName, int[] _score)
	{
		bool flag = _score[0] >= 10 || _score[1] >= 10;
		if (flag)
		{
			hyphen.transform.SetLocalScaleX(SmallCharSize);
			hyphen.transform.SetLocalScaleY(SmallCharSize);
		}
		else
		{
			hyphen.transform.SetLocalScaleX(1f);
			hyphen.transform.SetLocalScaleY(1f);
		}
		for (int i = 0; i < _score.Length; i++)
		{
			SettingScore(i, _score[i], flag);
		}
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			addScoreObj.transform.SetLocalEulerAnglesZ(0f);
		}
		addScoreObj.SetActive(value: true);
		RotAddScore(_teamNo);
	}
	private void SettingScore(int _teamNo, int _score, bool _showSmall)
	{
		for (int i = 0; i < scoreObj[_teamNo].score.Length; i++)
		{
			scoreObj[_teamNo].score[i].gameObject.SetActive((float)_score >= Mathf.Pow(10f, i) - (float)((i == 0) ? 1 : 0));
			if (scoreObj[_teamNo].score[i].gameObject.activeSelf)
			{
				scoreObj[_teamNo].score[i].sprite = numberSprites[(int)((float)_score % Mathf.Pow(10f, i + 1) / Mathf.Pow(10f, i))];
				scoreObj[_teamNo].score[i].color = new Color(1f, 1f, 1f);
			}
			if (_score >= 10)
			{
				scoreObj[_teamNo].score[i].transform.SetLocalPositionX((i == 0) ? ScoreSpace : (0f - ScoreSpace));
			}
			else
			{
				scoreObj[_teamNo].score[i].transform.SetLocalPositionX(0f);
			}
			if (_showSmall)
			{
				scoreObj[_teamNo].score[i].transform.SetLocalScaleX(SmallCharSize);
				scoreObj[_teamNo].score[i].transform.SetLocalScaleY(SmallCharSize);
			}
			else
			{
				scoreObj[_teamNo].score[i].transform.SetLocalScaleX(1f);
				scoreObj[_teamNo].score[i].transform.SetLocalScaleY(1f);
			}
			int num = _score - 1;
			if (num < 0)
			{
				num = 0;
			}
			scoreObj[_teamNo].beforeScore[i].gameObject.SetActive((float)num >= Mathf.Pow(10f, i) - (float)((i == 0) ? 1 : 0));
			if (scoreObj[_teamNo].beforeScore[i].gameObject.activeSelf)
			{
				scoreObj[_teamNo].beforeScore[i].sprite = numberSprites[(int)((float)num % Mathf.Pow(10f, i + 1) / Mathf.Pow(10f, i))];
				scoreObj[_teamNo].score[i].color = new Color(1f, 1f, 1f);
			}
			if (num >= 10)
			{
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalPositionX((i == 0) ? ScoreSpace : (0f - ScoreSpace));
			}
			else
			{
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalPositionX(0f);
			}
			if (_showSmall)
			{
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalScaleX(SmallCharSize);
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalScaleY(SmallCharSize);
			}
			else
			{
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalScaleX(1f);
				scoreObj[_teamNo].beforeScore[i].transform.SetLocalScaleY(1f);
			}
		}
	}
	private void RotAddScore(int _addTeamNo)
	{
		for (int i = 0; i < scoreObj.Length; i++)
		{
			if (i == _addTeamNo)
			{
				scoreObj[i].scoreAnchor.SetLocalEulerAnglesY(90f);
			}
			else
			{
				scoreObj[i].scoreAnchor.SetLocalEulerAnglesY(0f);
			}
			scoreObj[i].beforeScoreAnchor.gameObject.SetActive(i == _addTeamNo);
			if (scoreObj[i].beforeScoreAnchor.gameObject.activeSelf)
			{
				scoreObj[i].beforeScoreAnchor.SetLocalEulerAnglesY(0f);
			}
		}
		for (int j = 0; j < scoreObj[_addTeamNo].score.Length; j++)
		{
			if (isChangeScoreColor && scoreObj[_addTeamNo].beforeScore[j].color == new Color(1f, 1f, 1f))
			{
				scoreObj[_addTeamNo].beforeScore[j].color = new Color(1f, 1f, 0f);
			}
			if (scoreObj[_addTeamNo].score[j].color == new Color(1f, 1f, 1f))
			{
				scoreObj[_addTeamNo].score[j].color = new Color(1f, 1f, 0f);
			}
		}
		LeanTween.cancel(scoreObj[_addTeamNo].beforeScoreAnchor.gameObject);
		LeanTween.rotateAroundLocal(scoreObj[_addTeamNo].beforeScoreAnchor.gameObject, Vector3.up, 90f, 0.1f).setDelay(0f).setEaseLinear();
		LeanTween.cancel(scoreObj[_addTeamNo].scoreAnchor.gameObject);
		LeanTween.rotateAroundLocal(scoreObj[_addTeamNo].scoreAnchor.gameObject, Vector3.up, 270f, 0.2f).setDelay(0.03f).setEaseLinear();
		isChangeScoreColor = true;
		StartCoroutine(_HideAddScoreObj());
	}
	public IEnumerator _HideAddScoreObj()
	{
		yield return new WaitForSeconds(SHOW_TIME);
		addScoreObj.SetActive(value: false);
	}
}
