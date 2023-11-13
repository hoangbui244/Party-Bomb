using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Shooting_UIManager : SingletonCustom<Shooting_UIManager>
{
	[Serializable]
	private struct CursorPosition
	{
		public Vector3[] pos;
	}
	[Serializable]
	public struct bulletsSprite
	{
		public SpriteRenderer[] sprite;
	}
	[Serializable]
	private class ControlInfomationUI
	{
		[Header("アンカ\u30fc")]
		public GameObject anchor;
		[Header("操作情報を示す画像デ\u30fcタ")]
		public SpriteRenderer[] infomationSpriteUI;
		[Header("操作情報を示す文字デ\u30fcタ")]
		public TextMeshPro[] infomationTextUI;
		public float NowAlpha
		{
			get;
			set;
		}
		public void SetActive(bool _active)
		{
			anchor.SetActive(_active);
		}
		public void SetAlpha(float _alpha)
		{
			for (int i = 0; i < infomationSpriteUI.Length; i++)
			{
				infomationSpriteUI[i].SetAlpha(_alpha);
			}
			for (int j = 0; j < infomationTextUI.Length; j++)
			{
				infomationTextUI[j].SetAlpha(_alpha);
			}
			NowAlpha = _alpha;
		}
	}
	[SerializeField]
	[Header("カ\u30fcソル")]
	private Shooting_AimingCursor[] arrayCursor;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した各カ\u30fcソルUI座標")]
	private CursorPosition[] arrayCursorPosition;
	[SerializeField]
	[Header("プレイヤ\u30fc人数に対応した各カ\u30fcソルUI座標")]
	private Vector3[] cursorPosition;
	[SerializeField]
	private Transform cursorLimitLeftAnchor;
	[SerializeField]
	private Transform cursorLimitRightAnchor;
	[SerializeField]
	private Transform cursorLimitBottomAnchor;
	[SerializeField]
	private Transform cursorLimitTopAnchor;
	[SerializeField]
	[Header("残弾の色を変更するため")]
	private bulletsSprite[] BulletsSprite;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("共通レイアウト")]
	private CommonWaterPistolBattleUILayout commonUILayout;
	[SerializeField]
	private GameObject singleOnlyObj;
	[SerializeField]
	private GameObject multiOnlyObj;
	[SerializeField]
	private Transform multiOperationAnchor;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI firstCtrlUI;
	[SerializeField]
	[Header("スキップ表示UI")]
	private ControlInfomationUI skipCtrlUI;
	[SerializeField]
	[Header("ポイントゲット表示")]
	private Shooting_GetPointUi[] getPointUis;
	[SerializeField]
	[Header("ポイントゲット最大スケ\u30fcルアンカ\u30fc")]
	private Transform getPointMaxScaleWorldAnchor;
	[SerializeField]
	[Header("ポイントゲット最小スケ\u30fcルアンカ\u30fc")]
	private Transform getPointMinScaleWorldAnchor;
	private int[] gunNoTemp;
	private int[] scoreNoTemp;
	private readonly float MAX_TEXT_POSX = -17.5f;
	[SerializeField]
	[Header("弾数表示アンカ\u30fc")]
	private GameObject[] arrayRemainingBulletObjAnchor;
	[SerializeField]
	[Header("弾数のテキスト")]
	private TextMeshPro[] arrayRemainingBulletNumText;
	[SerializeField]
	[Header("弾数のSpriteNumbers")]
	private SpriteNumbers[] arrayRemainingBulletSpriteNum;
	[SerializeField]
	[Header("弾数近くのプレイヤ\u30fc表示スプライト")]
	private SpriteRenderer[] arrayRemainingBulletPlayerSprite;
	[SerializeField]
	[Header("プレイヤ\u30fcの色")]
	private Shooting_Controller[] controller;
	public float CursorMoveSpeed => 1f;
	public float CursorMoveValue => 500f;
	public float CursorInputTimeValue => 1f;
	public void Init()
	{
		gunNoTemp = SingletonCustom<Shooting_ControllerManager>.Instance.GetArrayGunNo();
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleOnlyObj.SetActive(value: true);
			multiOnlyObj.SetActive(value: false);
		}
		else
		{
			if (!Shooting_Define.IS_BATTLE_MODE && Shooting_Define.IS_TEAM_MODE)
			{
				multiOperationAnchor.SetLocalPositionY(434f);
			}
			singleOnlyObj.SetActive(value: false);
			multiOnlyObj.SetActive(value: true);
		}
		SetCursorUI();
		SetRemainingBulletUI();
		SetControlUI();
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; i++)
		{
			UpdateRemainingBulletNumUI(i, SingletonCustom<Shooting_ControllerManager>.Instance.RemainingBulletNum);
		}
		bool isSinglePlay = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
		commonUILayout.Init(60f, SingletonCustom<Shooting_GameManager>.Instance.PlayerGroupList, _isEightBattle: false, SingletonCustom<Shooting_GameManager>.Instance.HasSecondGroup);
	}
	public void SecondGroupInit()
	{
		SetCursorUI();
		SetRemainingBulletUI();
		SetControlUI();
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; i++)
		{
			UpdateRemainingBulletNumUI(i, SingletonCustom<Shooting_ControllerManager>.Instance.RemainingBulletNum);
		}
		for (int j = 0; j < getPointUis.Length; j++)
		{
		}
		commonUILayout.SecondGroupInit();
	}
	private void SetCursorUI()
	{
		int num = 0;
		for (int i = 0; i < gunNoTemp.Length; i++)
		{
			if (SingletonCustom<Shooting_GameManager>.Instance.GetIsPlayer(gunNoTemp[i]))
			{
				num++;
			}
		}
		for (int j = 0; j < gunNoTemp.Length; j++)
		{
			arrayCursor[gunNoTemp[j]].transform.localPosition = cursorPosition[j];
			if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
			{
				arrayCursor[gunNoTemp[j]].transform.localPosition = new Vector3(0f, 0f, -3f);
			}
			else
			{
				switch (num)
				{
				case 1:
					arrayCursor[gunNoTemp[j]].transform.SetLocalPositionX(0f);
					break;
				case 2:
					if (gunNoTemp[j] < 2)
					{
						arrayCursor[gunNoTemp[j]].transform.AddLocalPositionX(100f);
					}
					else
					{
						arrayCursor[gunNoTemp[j]].transform.AddLocalPositionX(-100f);
					}
					break;
				case 3:
					arrayCursor[gunNoTemp[j]].transform.AddLocalPositionX(50f);
					break;
				}
			}
			arrayCursor[gunNoTemp[j]].Init(gunNoTemp[j]);
		}
		SingletonCustom<Shooting_ControllerManager>.Instance.UpdateLookCursor();
	}
	private void SetBattleModeCursorUIPosition()
	{
		gunNoTemp.Shuffle();
	}
	private void SetTeamModeCursorUIPosition()
	{
		int num = 0;
		num = ((SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM != 3) ? UnityEngine.Random.Range(0, 2) : ((SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count <= SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count) ? 1 : 0));
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM - 1; i++)
		{
			int num2 = (i == SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM - 2) ? 1 : (SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM - 2 - i);
			bool flag = false;
			for (int j = i; j < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; j++)
			{
				if (SingletonCustom<Shooting_GameManager>.Instance.ArrayTeamNo[gunNoTemp[j]] == num)
				{
					if (i == j)
					{
						break;
					}
					int num3 = j;
					while (num3 > 0 && SingletonCustom<Shooting_GameManager>.Instance.ArrayTeamNo[gunNoTemp[num3 - 1]] != num)
					{
						int num4 = gunNoTemp[num3];
						gunNoTemp[num3] = gunNoTemp[num3 - 1];
						gunNoTemp[num3 - 1] = num4;
						num2--;
						if (num2 == 0)
						{
							break;
						}
						num3--;
					}
					flag = true;
				}
				if (flag)
				{
					break;
				}
			}
			num = ((num == 0) ? 1 : 0);
		}
	}
	private void SetRemainingBulletUI()
	{
		if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
		{
			for (int i = 1; i < arrayRemainingBulletObjAnchor.Length; i++)
			{
				arrayRemainingBulletObjAnchor[i].SetActive(value: false);
			}
			arrayRemainingBulletPlayerSprite[0].gameObject.SetActive(value: false);
			arrayRemainingBulletObjAnchor[0].transform.SetLocalPositionX(-200f);
		}
		for (int j = 0; j < arrayRemainingBulletPlayerSprite.Length; j++)
		{
			int playerNo = SingletonCustom<Shooting_GameManager>.Instance.GetPlayerNo(j);
			if (playerNo < 4)
			{
				arrayRemainingBulletPlayerSprite[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, Shooting_Define.PLAYER_SPRITE_NAMES[playerNo]);
			}
			else
			{
				arrayRemainingBulletPlayerSprite[j].gameObject.SetActive(value: false);
			}
		}
	}
	private void SetControlUI()
	{
		firstCtrlUI.SetActive(_active: false);
		firstCtrlUI.SetAlpha(0f);
		skipCtrlUI.SetActive(_active: false);
		skipCtrlUI.SetAlpha(0f);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < SingletonCustom<Shooting_GameManager>.Instance.PLAY_NUM; i++)
		{
			arrayCursor[i].UpdateMethod();
		}
		if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
		{
			arrayCursor[0].UpdateMethod();
			float x = SingletonCustom<Shooting_ControllerManager>.Instance.GetSinglePosXLerp() * -900f + 250f;
			arrayRemainingBulletObjAnchor[0].transform.SetLocalPositionX(x);
		}
	}
	public Vector3 GetCursorPosition(int _gunNo)
	{
		return arrayCursor[_gunNo].GetPos();
	}
	public Vector3 GetCursorMoveVec(int _gunNo)
	{
		return arrayCursor[_gunNo].GetMoveVec();
	}
	public float GetClampMoveTargetPosX(float _posX)
	{
		return Mathf.Clamp(_posX, cursorLimitLeftAnchor.position.x, cursorLimitRightAnchor.position.x);
	}
	public float GetClampMoveTargetPosY(float _posY)
	{
		return Mathf.Clamp(_posY, cursorLimitBottomAnchor.position.y, cursorLimitTopAnchor.position.y);
	}
	public void SetCursorMoveDir(int _gunNo, Vector2 _dir, GameObject _player, bool _isShot)
	{
		if (_gunNo == 0 && SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
		{
			SingletonCustom<Shooting_ControllerManager>.Instance.SingleMove(_dir, _player, _isShot);
		}
		else
		{
			arrayCursor[_gunNo].Move(_dir, _isSpeedUp: false);
		}
	}
	public void StopCursor(int _gunNo)
	{
		if (_gunNo == 0 && SingletonCustom<Shooting_GameManager>.Instance.IsSingle)
		{
			SingletonCustom<Shooting_ControllerManager>.Instance.SingleStop();
		}
		else
		{
			arrayCursor[_gunNo].Stop();
		}
	}
	public void SetSingleCursorZoomScaleLerp(float _lerp)
	{
		arrayCursor[0].SetSingleScaleLerp(_lerp);
	}
	public void UpdateScoreUI(int _gunNo, int _score)
	{
		commonUILayout.SetScore(_gunNo, _score);
	}
	public void UpdateTimeUI(float _time)
	{
		commonUILayout.SetTime(_time);
	}
	public void PlayGetPointAnimation(int _gunNo, Sprite _point, Vector3 _pos, Vector3 _scale)
	{
		_scale = Vector3.one * Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(Mathf.InverseLerp(getPointMinScaleWorldAnchor.position.z, getPointMaxScaleWorldAnchor.position.z, _pos.z)));
		getPointUis[_gunNo].Show(_gunNo, _point, _pos, _scale);
	}
	public void UpdateRemainingBulletNumUI(int _gunNo, int _bulletNum)
	{
		if (_bulletNum < 0)
		{
			_bulletNum = 0;
		}
		if (_bulletNum == 0)
		{
			for (int i = 0; i < BulletsSprite[i].sprite.Length; i++)
			{
				BulletsSprite[_gunNo].sprite[i].color = Color.red;
			}
		}
		arrayRemainingBulletSpriteNum[_gunNo].Set(_bulletNum);
	}
	public void ViewBulletEnd(int _gunNo)
	{
		commonUILayout.SetEndChara(_gunNo);
	}
	public void ViewFirstControlInfo()
	{
		firstCtrlUI.SetActive(_active: true);
		ControlInfomationFade(firstCtrlUI, 1f, 0.5f);
		ControlInfomationFade(firstCtrlUI, 0f, 0.5f, 3f, delegate
		{
			firstCtrlUI.SetActive(_active: false);
		});
	}
	public void ViewSkipControlInfo()
	{
		skipCtrlUI.SetActive(_active: true);
		ControlInfomationFade(skipCtrlUI, 1f, 0.5f, 1f);
	}
	public void Fade(float _time, float _delay, Action _act)
	{
		Color color = fade.color;
		color.a = 1f;
		fade.SetAlpha(0f);
		fade.gameObject.SetActive(value: true);
		LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				_act();
			});
		color.a = 0f;
		LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				fade.gameObject.SetActive(fade);
			});
	}
	private void ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		StartCoroutine(_ControlInfomationFade(_infoUI, _setAlpha, _fadeTime, _delayTime, _callback));
	}
	private IEnumerator _ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		yield return new WaitForSeconds(_delayTime);
		float startAlpha = _infoUI.NowAlpha;
		while (time < _fadeTime)
		{
			_infoUI.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			time += Time.deltaTime;
			yield return null;
		}
		_infoUI.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
}
