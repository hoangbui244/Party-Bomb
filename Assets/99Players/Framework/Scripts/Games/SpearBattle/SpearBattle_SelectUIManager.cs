using TMPro;
using UnityEngine;
public class SpearBattle_SelectUIManager : SingletonCustom<SpearBattle_SelectUIManager>
{
	[SerializeField]
	private GameObject anchorObj;
	[SerializeField]
	private GameObject singlePlayerIconObj;
	[SerializeField]
	private GameObject[] leftDecideObjs;
	[SerializeField]
	private SpriteRenderer[] leftSkillSprite;
	[SerializeField]
	private SpriteRenderer leftPlayerIcon;
	[SerializeField]
	private GameObject leftCtrlObj;
	[SerializeField]
	private JoyConButton[] leftCtrlButtons;
	[SerializeField]
	private TextMeshPro[] leftRemainTexts;
	[SerializeField]
	private SpriteRenderer[] leftRemainSprites;
	[SerializeField]
	private GameObject[] rightDecideObjs;
	[SerializeField]
	private SpriteRenderer[] rightSkillSprite;
	[SerializeField]
	private SpriteRenderer rightPlayerIcon;
	[SerializeField]
	private GameObject rightCtrlObj;
	[SerializeField]
	private JoyConButton[] rightCtrlButtons;
	[SerializeField]
	private TextMeshPro[] rightRemainTexts;
	[SerializeField]
	private SpriteRenderer[] rightRemainSprites;
	[SerializeField]
	private JoyConButton[] centerCtrlButtons;
	public bool IsView => anchorObj.activeSelf;
	public void Init()
	{
		if (SingletonCustom<SpearBattle_GameManager>.Instance.PlayerNum == 1 && SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.isPlayer)
		{
			singlePlayerIconObj.SetActive(value: true);
			leftPlayerIcon.gameObject.SetActive(value: false);
		}
		else
		{
			singlePlayerIconObj.SetActive(value: false);
			leftPlayerIcon.gameObject.SetActive(value: true);
		}
		leftPlayerIcon.sprite = SingletonCustom<SpearBattle_UIManager>.Instance.GetNowUiPlayerIconSprite(_isLeft: true);
		rightPlayerIcon.sprite = SingletonCustom<SpearBattle_UIManager>.Instance.GetNowUiPlayerIconSprite(_isLeft: false);
		SpearBattle_GameManager.BattleData nowBattleData = SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData();
		leftCtrlObj.SetActive(nowBattleData.leftCharaData.isPlayer);
		rightCtrlObj.SetActive(nowBattleData.rightCharaData.isPlayer);
		if (leftCtrlObj.activeSelf)
		{
			for (int i = 0; i < leftCtrlButtons.Length; i++)
			{
				leftCtrlButtons[i].SetPlayerType((JoyConButton.PlayerType)nowBattleData.leftCharaData.playerNo);
				leftCtrlButtons[i].CheckJoyconButton();
			}
		}
		if (rightCtrlObj.activeSelf)
		{
			for (int j = 0; j < rightCtrlButtons.Length; j++)
			{
				rightCtrlButtons[j].SetPlayerType((JoyConButton.PlayerType)nowBattleData.rightCharaData.playerNo);
				rightCtrlButtons[j].CheckJoyconButton();
			}
		}
		if (SingletonCustom<SpearBattle_GameManager>.Instance.PlayerNum > 1)
		{
			if (nowBattleData.leftCharaData.isPlayer)
			{
				for (int k = 0; k < centerCtrlButtons.Length; k++)
				{
					centerCtrlButtons[k].SetPlayerType((JoyConButton.PlayerType)nowBattleData.leftCharaData.playerNo);
					centerCtrlButtons[k].CheckJoyconButton();
				}
			}
			else if (nowBattleData.rightCharaData.isPlayer)
			{
				for (int l = 0; l < centerCtrlButtons.Length; l++)
				{
					centerCtrlButtons[l].SetPlayerType((JoyConButton.PlayerType)nowBattleData.rightCharaData.playerNo);
					centerCtrlButtons[l].CheckJoyconButton();
				}
			}
			else
			{
				for (int m = 0; m < centerCtrlButtons.Length; m++)
				{
					centerCtrlButtons[m].SetPlayerType(JoyConButton.PlayerType.PLAYER_1);
					centerCtrlButtons[m].CheckJoyconButton();
				}
			}
		}
		ResetSkillAndDecide();
	}
	public void UpdateMethod()
	{
	}
	public void ResetSkillAndDecide()
	{
		for (int i = 0; i < leftDecideObjs.Length; i++)
		{
			leftDecideObjs[i].SetActive(value: false);
		}
		for (int j = 0; j < rightDecideObjs.Length; j++)
		{
			rightDecideObjs[j].SetActive(value: false);
		}
		for (int k = 0; k < leftSkillSprite.Length; k++)
		{
			leftSkillSprite[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < rightSkillSprite.Length; l++)
		{
			rightSkillSprite[l].gameObject.SetActive(value: false);
		}
	}
	public void SetDecideActive(bool _isLeft, SpearBattle_Define.SkillType[] _skillArray)
	{
		if (_isLeft)
		{
			for (int i = 0; i < leftDecideObjs.Length; i++)
			{
				leftDecideObjs[i].SetActive(_skillArray[i] != SpearBattle_Define.SkillType.Empty);
			}
		}
		else
		{
			for (int j = 0; j < rightDecideObjs.Length; j++)
			{
				rightDecideObjs[j].SetActive(_skillArray[j] != SpearBattle_Define.SkillType.Empty);
			}
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
	public void SetRemainText(bool _isLeft, int _num, SpearBattle_Define.SkillType _skillType)
	{
		switch (_skillType)
		{
		case SpearBattle_Define.SkillType.Rock:
			if (_isLeft)
			{
				leftRemainTexts[0].text = _num.ToString();
				leftRemainSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			else
			{
				rightRemainTexts[0].text = _num.ToString();
				rightRemainSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			break;
		case SpearBattle_Define.SkillType.Scissors:
			if (_isLeft)
			{
				leftRemainTexts[1].text = _num.ToString();
				leftRemainSprites[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			else
			{
				rightRemainTexts[1].text = _num.ToString();
				rightRemainSprites[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			break;
		case SpearBattle_Define.SkillType.Paper:
			if (_isLeft)
			{
				leftRemainTexts[2].text = _num.ToString();
				leftRemainSprites[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			else
			{
				rightRemainTexts[2].text = _num.ToString();
				rightRemainSprites[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "remaining_number_" + _num.ToString());
			}
			break;
		}
	}
	public void SetViewActive(bool _active)
	{
		anchorObj.SetActive(_active);
	}
}
