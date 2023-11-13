using System;
using System.Collections.Generic;
using UnityEngine;
public class DragonBattleUIManager : SingletonCustom<DragonBattleUIManager>
{
	[Serializable]
	public struct HpData
	{
		[SerializeField]
		[Header("オブジェクト")]
		private Transform obj;
		[SerializeField]
		[Header("ゲ\u30fcジ")]
		private SpriteRenderer gauge;
		private float gaugeValue;
		public Transform Obj => obj;
		public void Init()
		{
			gaugeValue = gauge.size.x;
		}
		public void SetHp(float _value)
		{
			gauge.size = new Vector2(gaugeValue * _value, gauge.size.y);
		}
	}
	[SerializeField]
	[Header("プレイヤ\u30fc番号表示")]
	private SpriteRenderer[] objPlayerNo;
	[SerializeField]
	[Header("プレイヤ\u30fcスコア表示")]
	private DragonBattlePlayerScore[] arrayPlayerScore;
	[SerializeField]
	[Header("残り距離表示")]
	private SpriteNumbers remainingDistance;
	[SerializeField]
	[Header("ポイントプレハブ")]
	private DragonBattleDispPoint[] pointPrefs;
	[SerializeField]
	[Header("一人用ボタンレイアウト")]
	private GameObject objSingleButtonLayout;
	[SerializeField]
	[Header("複数人用ボタンレイアウト")]
	private GameObject objMultiButtonLayout;
	private float OFFSET_PLAYER_NO_Y = 125f;
	private float[] arrayScorePositionY;
	private List<DragonBattlePlayerScore> listPlayerScore = new List<DragonBattlePlayerScore>();
	private bool isHideScore;
	[SerializeField]
	[Header("ボスHP")]
	private Transform bossHpObj;
	[SerializeField]
	[Header("ボスHPゲ\u30fcジ")]
	private SpriteRenderer bossHpGauge;
	private float bossHpValue;
	[SerializeField]
	[Header("プレイヤ\u30fcのHP情報リスト")]
	private HpData[] hpDataPlayers;
	public bool IsHideScore => isHideScore;
	public void Init()
	{
		for (int i = 0; i < arrayPlayerScore.Length; i++)
		{
			arrayPlayerScore[i].Init(i);
			listPlayerScore.Add(arrayPlayerScore[i]);
		}
		arrayScorePositionY = new float[arrayPlayerScore.Length];
		for (int j = 0; j < arrayScorePositionY.Length; j++)
		{
			arrayScorePositionY[j] = arrayPlayerScore[j].transform.localPosition.y;
		}
		for (int k = 0; k < objPlayerNo.Length; k++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && k == 0)
			{
				objPlayerNo[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_you");
			}
			else
			{
				objPlayerNo[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, DragonBattlePlayerScore.WORLD_PLAYER_SPRITE_NAMES[SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer()[k].IsCpu ? (4 + (k - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : k]);
			}
		}
		LeanTween.value(1f, 0f, 0.25f).setDelay(4f).setOnUpdate(delegate(float _value)
		{
			for (int m = 0; m < objPlayerNo.Length; m++)
			{
				if ((bool)objPlayerNo[m])
				{
					objPlayerNo[m].SetAlpha(_value);
				}
			}
		});
		bossHpObj.gameObject.SetActive(value: false);
		SetBossHp(0f);
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			objSingleButtonLayout.SetActive(value: true);
			objMultiButtonLayout.SetActive(value: false);
		}
		else
		{
			objSingleButtonLayout.SetActive(value: false);
			objMultiButtonLayout.SetActive(value: true);
		}
		for (int l = 0; l < hpDataPlayers.Length; l++)
		{
			hpDataPlayers[l].Init();
		}
	}
	public void HideScore()
	{
		LeanTween.cancel(arrayPlayerScore[0].gameObject);
		LeanTween.cancel(arrayPlayerScore[1].gameObject);
		LeanTween.cancel(arrayPlayerScore[2].gameObject);
		LeanTween.cancel(arrayPlayerScore[3].gameObject);
		LeanTween.moveLocalX(arrayPlayerScore[0].gameObject, -1500f, 1.25f).setEaseInQuint();
		LeanTween.moveLocalX(arrayPlayerScore[1].gameObject, -1500f, 1.25f).setDelay(0.15f).setEaseInQuint();
		LeanTween.moveLocalX(arrayPlayerScore[2].gameObject, -1500f, 1.25f).setDelay(0.3f).setEaseInQuint();
		LeanTween.moveLocalX(arrayPlayerScore[3].gameObject, -1500f, 1.25f).setDelay(0.45f).setEaseInQuint();
		isHideScore = true;
	}
	public void ShowScore(int _playerIdx, int _score)
	{
		DragonBattleDispPoint dragonBattleDispPoint = null;
		for (int i = 0; i < DragonBattleDefine.POINT_TYPE_LIST.Length; i++)
		{
			if (DragonBattleDefine.POINT_TYPE_LIST[i] == _score)
			{
				dragonBattleDispPoint = UnityEngine.Object.Instantiate(pointPrefs[i], base.transform);
				break;
			}
		}
		dragonBattleDispPoint.gameObject.SetActive(value: true);
		dragonBattleDispPoint.transform.SetLocalPositionZ(-3f);
		DragonBattlePlayer playerAtIdx = SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(_playerIdx);
		CalcManager.mCalcVector3 = SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera().WorldToScreenPoint(playerAtIdx.transform.position);
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
		dragonBattleDispPoint.transform.SetPosition(Mathf.Clamp(CalcManager.mCalcVector3.x, -870f, 870f), Mathf.Clamp(CalcManager.mCalcVector3.y + OFFSET_PLAYER_NO_Y, -458f, 458f), dragonBattleDispPoint.transform.position.z);
		dragonBattleDispPoint.Init(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[playerAtIdx.IsCpu ? (4 + (_playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : _playerIdx]);
	}
	public void ShowPlayerNo(int _playerNo)
	{
		objPlayerNo[_playerNo].SetAlpha(1f);
		LeanTween.value(1f, 0f, 0.25f).setDelay(2f).setOnUpdate(delegate(float _value)
		{
			if ((bool)objPlayerNo[_playerNo])
			{
				objPlayerNo[_playerNo].SetAlpha(_value);
			}
		});
	}
	public void ShowBossHp(Action _callBack = null)
	{
		if (!bossHpObj.gameObject.activeSelf)
		{
			UnityEngine.Debug.Log("ボスHP表示");
			DragonBattleDefine.EnemyType enemyType = SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.Enemys[0].EnemyType;
			SingletonCustom<DragonBattlePlayerManager>.Instance.IsProhibitAttack = true;
			SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.Enemys[0].BossStandby();
			SetBossHp(0f);
			bossHpGauge.transform.SetLocalScaleX(0f);
			bossHpObj.gameObject.SetActive(value: true);
			UpdateBossHp();
			LeanTween.value(0f, 1f, 1f).setDelay(0.5f).setOnUpdate(delegate(float _value)
			{
				if ((bool)bossHpGauge)
				{
					SetBossHp(_value);
					bossHpGauge.transform.SetLocalScaleX(_value);
				}
			})
				.setOnComplete((Action)delegate
				{
					if (_callBack != null)
					{
						_callBack();
					}
				});
		}
	}
	public void HideBossHp()
	{
		if ((bool)bossHpObj)
		{
			bossHpObj.gameObject.SetActive(value: false);
		}
	}
	public void SetBossHp(float _value)
	{
		bossHpValue = _value;
	}
	public bool IsShowBossHp()
	{
		return bossHpObj.gameObject.activeSelf;
	}
	public void UpdateMethod()
	{
		UpdatePlayerNoPos();
		for (int i = 0; i < arrayPlayerScore.Length; i++)
		{
			arrayPlayerScore[i].UpdateMethod();
		}
		listPlayerScore.Sort((DragonBattlePlayerScore a, DragonBattlePlayerScore b) => b.CurrentScore - a.CurrentScore);
		for (int j = 0; j < arrayPlayerScore.Length; j++)
		{
			listPlayerScore[j].transform.SetLocalPositionY(Mathf.SmoothStep(listPlayerScore[j].transform.localPosition.y, arrayScorePositionY[j], 0.25f));
		}
		remainingDistance.Set(Mathf.Clamp((int)(1000f * SingletonCustom<DragonBattleCameraMover>.Instance.DistanceScale), 0, 1000));
		UpdateBossHp();
		UpdatePlayerHp();
	}
	private void UpdatePlayerNoPos()
	{
		for (int i = 0; i < objPlayerNo.Length; i++)
		{
			if (i < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				CalcManager.mCalcVector3 = SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera().WorldToScreenPoint(SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(i).transform.position);
				CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
				objPlayerNo[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + OFFSET_PLAYER_NO_Y, objPlayerNo[i].transform.position.z);
			}
			else
			{
				objPlayerNo[i].transform.SetLocalPositionX(-2000f);
			}
		}
	}
	private void UpdateBossHp()
	{
		if (bossHpObj.gameObject.activeSelf)
		{
			CalcManager.mCalcVector3 = SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera().WorldToScreenPoint(SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.Enemys[0].HpGaugeAnchor.position);
			CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
			bossHpObj.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, bossHpObj.transform.position.z);
			if (SingletonCustom<DragonBattleFieldManager>.Instance.NowParts.IsEnemyActive)
			{
				bossHpGauge.transform.SetLocalScaleX(Mathf.Lerp(bossHpGauge.transform.localScale.x, bossHpValue, 5f * Time.deltaTime));
			}
		}
	}
	private void UpdatePlayerHp()
	{
		for (int i = 0; i < hpDataPlayers.Length; i++)
		{
			CalcManager.mCalcVector3 = SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera().WorldToScreenPoint(SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(i).transform.position + new Vector3(0f, 0f, SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(i).HpAnchor.localPosition.x - (SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(i).transform.position.z - SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera().transform.position.z - 12f) * 0.1f));
			CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
			hpDataPlayers[i].Obj.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, hpDataPlayers[i].Obj.position.z);
			hpDataPlayers[i].SetHp(SingletonCustom<DragonBattlePlayerManager>.Instance.GetPlayerAtIdx(i).HpData.Per);
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		for (int i = 0; i < arrayPlayerScore.Length; i++)
		{
			LeanTween.cancel(arrayPlayerScore[i].gameObject);
		}
		LeanTween.cancel(arrayPlayerScore[0].gameObject);
		LeanTween.cancel(arrayPlayerScore[1].gameObject);
		LeanTween.cancel(arrayPlayerScore[2].gameObject);
		LeanTween.cancel(arrayPlayerScore[3].gameObject);
	}
}
