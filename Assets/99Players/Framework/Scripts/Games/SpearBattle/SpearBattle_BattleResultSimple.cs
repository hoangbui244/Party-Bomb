using System;
using UnityEngine;
public class SpearBattle_BattleResultSimple : SingletonCustom<SpearBattle_BattleResultSimple>
{
	private float EFFECT_TIME = 3f;
	private float defStartObjXPos;
	[SerializeField]
	[Header("アンカ\u30fc")]
	private GameObject objRoot;
	[SerializeField]
	private GameObject objWin;
	[SerializeField]
	private GameObject objLose;
	[SerializeField]
	private GameObject objDraw;
	[SerializeField]
	private SpriteRenderer winPlayerSprite;
	[SerializeField]
	private SpriteRenderer winSprite;
	[SerializeField]
	private SpriteRenderer losePlayerSprite;
	[SerializeField]
	private SpriteRenderer loseSprite;
	private void Start()
	{
		defStartObjXPos = objRoot.transform.localPosition.x;
	}
	public void Show(Action _callBack = null)
	{
		objRoot.SetActive(value: true);
		if (Localize_Define.Language == Localize_Define.LanguageType.Japanese)
		{
			GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
		}
		else
		{
			GS_Define.GameType lastSelectGameType2 = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
		}
		objRoot.transform.SetLocalPositionX(defStartObjXPos);
		LeanTween.moveLocalX(objRoot.transform.gameObject, 0f, 0.75f).setEaseOutBack();
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		WaitAfterExec(2f, delegate
		{
			LeanTween.moveLocalX(objRoot.transform.gameObject, 0f - defStartObjXPos, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate
			{
				if (_callBack != null)
				{
					_callBack();
				}
				objRoot.SetActive(value: false);
			});
		});
	}
	public void WinSetting(int _playerNo)
	{
		objWin.SetActive(value: true);
		objLose.SetActive(value: false);
		objDraw.SetActive(value: false);
		if (_playerNo < 4)
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.PlayerNum > 1)
			{
				winSprite.transform.localPosition = new Vector3(277f, -54f, 0f);
				winPlayerSprite.transform.localPosition = new Vector3(-234f, -54f, 0f);
				winPlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "h_" + (_playerNo + 1).ToString() + "p");
			}
			else
			{
				winSprite.transform.localPosition = new Vector3(446f, -54f, 0f);
				winPlayerSprite.transform.localPosition = new Vector3(-229f, -54f, 0f);
				winPlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "_h_you");
			}
		}
		else
		{
			winSprite.transform.localPosition = new Vector3(446f, -54f, 0f);
			winPlayerSprite.transform.localPosition = new Vector3(-229f, -54f, 0f);
			winPlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "h_cpu" + (_playerNo - 3).ToString());
		}
	}
	public void LoseSetting(int _playerNo)
	{
		objWin.SetActive(value: false);
		objLose.SetActive(value: true);
		objDraw.SetActive(value: false);
		if (_playerNo < 4)
		{
			if (SingletonCustom<SpearBattle_GameManager>.Instance.PlayerNum > 1)
			{
				loseSprite.transform.localPosition = new Vector3(275f, -54f, 0f);
				losePlayerSprite.transform.localPosition = new Vector3(-236f, -54f, 0f);
				losePlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "h_" + (_playerNo + 1).ToString() + "p");
			}
			else
			{
				loseSprite.transform.localPosition = new Vector3(444f, -54f, 0f);
				losePlayerSprite.transform.localPosition = new Vector3(-231f, -54f, 0f);
				losePlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "_h_you");
			}
		}
		else
		{
			loseSprite.transform.localPosition = new Vector3(444f, -54f, 0f);
			losePlayerSprite.transform.localPosition = new Vector3(-231f, -54f, 0f);
			losePlayerSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SpearBattle, "h_cpu" + (_playerNo - 3).ToString());
		}
	}
	public void DrawSetting()
	{
		objWin.SetActive(value: false);
		objLose.SetActive(value: false);
		objDraw.SetActive(value: true);
	}
}
