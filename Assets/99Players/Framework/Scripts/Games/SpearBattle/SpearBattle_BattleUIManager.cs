using TMPro;
using UnityEngine;
public class SpearBattle_BattleUIManager : SingletonCustom<SpearBattle_BattleUIManager>
{
	[SerializeField]
	private GameObject anchorObj;
	[SerializeField]
	private GameObject[] duelDisableObjs;
	[SerializeField]
	private GameObject[] leftHideObjs;
	[SerializeField]
	private GameObject[] leftOrderObjs;
	[SerializeField]
	private SpriteRenderer[] leftSkillSprite;
	[SerializeField]
	private SpriteRenderer[] leftResultSprite;
	[SerializeField]
	private SpearBattle_PlayerGauge leftPlayerGauge;
	[SerializeField]
	private Transform leftHpScaleAnchor;
	[SerializeField]
	private TextMeshPro leftScoreText;
	[SerializeField]
	private SpriteRenderer leftScoreSprite;
	[SerializeField]
	private GameObject[] rightHideObjs;
	[SerializeField]
	private GameObject[] rightOrderObjs;
	[SerializeField]
	private SpriteRenderer[] rightSkillSprite;
	[SerializeField]
	private SpriteRenderer[] rightResultSprite;
	[SerializeField]
	private SpearBattle_PlayerGauge rightPlayerGauge;
	[SerializeField]
	private Transform rightHpScaleAnchor;
	[SerializeField]
	private TextMeshPro rightScoreText;
	[SerializeField]
	private SpriteRenderer rightScoreSprite;
	public bool IsView => anchorObj.activeSelf;
	public void Init()
	{
		leftPlayerGauge.Init();
		rightPlayerGauge.Init();
		SetHp(_isLeft: true, 5);
		SetHp(_isLeft: false, 5);
		SetScore(_isLeft: true, 0);
		SetScore(_isLeft: false, 0);
	}
	public void UpdateMethod()
	{
		leftPlayerGauge.UpdateMethod();
		rightPlayerGauge.UpdateMethod();
	}
	public void SetPartitionActive(bool _active)
	{
		for (int i = 0; i < duelDisableObjs.Length; i++)
		{
			duelDisableObjs[i].SetActive(_active);
		}
	}
	public void SetHp(bool _isLeft, int _hp)
	{
		float num = Mathf.Max(0f, (float)_hp / 5f);
		if (_isLeft)
		{
			leftPlayerGauge.SetNowHpScale(num);
			leftHpScaleAnchor.SetLocalScaleX(num);
		}
		else
		{
			rightPlayerGauge.SetNowHpScale(num);
			rightHpScaleAnchor.SetLocalScaleX(num);
		}
	}
	public void SetScore(bool _isLeft, int _score)
	{
		if (_isLeft)
		{
			leftScoreText.text = _score.ToString();
			leftScoreSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "point_" + _score.ToString());
		}
		else
		{
			rightScoreText.text = _score.ToString();
			rightScoreSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "point_" + _score.ToString());
		}
	}
	public void SetHideActive(bool _isLeft, bool _active)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftHideObjs.Length; i++)
			{
				leftHideObjs[i].SetActive(_active);
			}
		}
		else
		{
			for (int j = 0; j < rightHideObjs.Length; j++)
			{
				rightHideObjs[j].SetActive(_active);
			}
		}
	}
	public void SetHideActive(bool _isLeft, int _no, bool _active)
	{
		if (_isLeft)
		{
			leftHideObjs[_no].SetActive(_active);
		}
		else
		{
			rightHideObjs[_no].SetActive(_active);
		}
	}
	public void SetHideActive(bool _isLeft, SpearBattle_Define.SkillType[] _skillArray)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftHideObjs.Length; i++)
			{
				leftHideObjs[i].SetActive(_skillArray[i] != SpearBattle_Define.SkillType.Empty);
			}
		}
		else
		{
			for (int j = 0; j < rightHideObjs.Length; j++)
			{
				rightHideObjs[j].SetActive(_skillArray[j] != SpearBattle_Define.SkillType.Empty);
			}
		}
	}
	public void SetOrderActive(bool _isLeft, bool _active)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftOrderObjs.Length; i++)
			{
				leftOrderObjs[i].SetActive(_active);
			}
		}
		else
		{
			for (int j = 0; j < rightOrderObjs.Length; j++)
			{
				rightOrderObjs[j].SetActive(_active);
			}
		}
	}
	public void SetOrderActive(bool _isLeft, int _no, bool _active)
	{
		if (_isLeft)
		{
			leftOrderObjs[_no].SetActive(_active);
		}
		else
		{
			rightOrderObjs[_no].SetActive(_active);
		}
	}
	public void SetSkillActive(bool _isLeft, bool _active)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftSkillSprite.Length; i++)
			{
				leftSkillSprite[i].gameObject.SetActive(_active);
			}
		}
		else
		{
			for (int j = 0; j < rightSkillSprite.Length; j++)
			{
				rightSkillSprite[j].gameObject.SetActive(_active);
			}
		}
	}
	public void SetSkillActive(bool _isLeft, int _no, SpearBattle_Define.SkillType _skill)
	{
		if (_isLeft)
		{
			leftSkillSprite[_no].gameObject.SetActive(_skill != SpearBattle_Define.SkillType.Empty);
			leftSkillSprite[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.SkillSpriteNameArray[(int)_skill]);
		}
		else
		{
			rightSkillSprite[_no].gameObject.SetActive(_skill != SpearBattle_Define.SkillType.Empty);
			rightSkillSprite[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.SkillSpriteNameArray[(int)_skill]);
		}
	}
	public void SetSkillActive(bool _isLeft, SpearBattle_Define.SkillType[] _skillArray)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftSkillSprite.Length; i++)
			{
				leftSkillSprite[i].gameObject.SetActive(_skillArray[i] != SpearBattle_Define.SkillType.Empty);
				leftSkillSprite[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.SkillSpriteNameArray[(int)_skillArray[i]]);
			}
		}
		else
		{
			for (int j = 0; j < rightSkillSprite.Length; j++)
			{
				rightSkillSprite[j].gameObject.SetActive(_skillArray[j] != SpearBattle_Define.SkillType.Empty);
				rightSkillSprite[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.SkillSpriteNameArray[(int)_skillArray[j]]);
			}
		}
	}
	public void SetResultActive(bool _isLeft, int _no, bool _active, int _resultNo)
	{
		if (_isLeft)
		{
			leftResultSprite[_no].gameObject.SetActive(_active);
			leftResultSprite[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.PhaseResultSpriteNameArray[_resultNo]);
		}
		else
		{
			rightResultSprite[_no].gameObject.SetActive(_active);
			rightResultSprite[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, SpearBattle_Define.PhaseResultSpriteNameArray[_resultNo]);
		}
	}
	public void SetViewActive(bool _active)
	{
		anchorObj.SetActive(_active);
	}
}
