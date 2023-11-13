using System;
using UnityEngine;
public class LegendarySword_Player : MonoBehaviour
{
	[SerializeField]
	[Header("LegendarySword_Chara")]
	private LegendarySword_Chara chara;
	[SerializeField]
	[Header("各キャラクタ\u30fcの表示オブジェクト")]
	private GameObject[] charaObj;
	[SerializeField]
	[Header("AI処理")]
	private LegendarySword_AI ai;
	[SerializeField]
	[Header("動かすキャラ")]
	private GameObject moveCharacter;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	private bool visible;
	private LegendarySword_Define.UserType userType;
	public bool isPlay;
	public bool goal;
	public bool isover = true;
	private Vector3 originPos;
	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}
	public LegendarySword_Chara Chara => chara;
	public LegendarySword_Define.UserType UserType => userType;
	public void Init(LegendarySword_Define.UserType _userType, bool _isPlay)
	{
		userType = _userType;
		visible = true;
		isover = false;
		HideCharacterModeSet();
		characterStyle.DisableAddModel();
		characterStyle.SetGameStyle(GS_Define.GameType.BLOCK_WIPER, (int)userType);
		characterStyle.SetMainCharacterFaceDiff((int)UserType, StyleTextureManager.MainCharacterFaceType.NORMAL);
		isPlay = _isPlay;
		chara.PlayerInit(this);
		ai.Init();
	}
	public void SetPos(Vector3 _pos)
	{
		base.transform.localPosition = _pos;
		originPos = base.transform.localPosition;
	}
	private void HideCharacterModeSet()
	{
		for (int i = 0; i < charaObj.Length; i++)
		{
			charaObj[i].SetActive(value: false);
		}
	}
	private static GameObject[] GetChildren(Transform parent)
	{
		GameObject[] array = new GameObject[parent.childCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = parent.GetChild(i).gameObject;
		}
		return array;
	}
	public void StartMethod()
	{
		chara.StartMethod();
	}
	public void UpdateMethod()
	{
		bool flag = LegendarySword_Define.GM.GetGameState() >= LegendarySword_GameManager.GameState.GAME_START_WAIT && userType <= LegendarySword_Define.UserType.PLAYER_4;
		if (!((LegendarySword_Define.GM.GetGameState() >= LegendarySword_GameManager.GameState.DURING_GAME) | flag) || !isPlay || chara.processType == LegendarySword_Chara.ProcessType.GOAL)
		{
			return;
		}
		if (userType <= LegendarySword_Define.UserType.PLAYER_4)
		{
			if (!LegendarySword_Define.CM.IsStickTiltDirection(userType, LegendarySword_ControllerManager.StickType.L, LegendarySword_ControllerManager.StickDirType.UP) && !LegendarySword_Define.CM.IsStickTiltDirection(userType, LegendarySword_ControllerManager.StickType.L, LegendarySword_ControllerManager.StickDirType.RIGHT) && !LegendarySword_Define.CM.IsStickTiltDirection(userType, LegendarySword_ControllerManager.StickType.L, LegendarySword_ControllerManager.StickDirType.LEFT))
			{
				LegendarySword_Define.CM.IsStickTiltDirection(userType, LegendarySword_ControllerManager.StickType.L, LegendarySword_ControllerManager.StickDirType.DOWN);
			}
			if (!LegendarySword_Define.CM.IsStickTilt(userType, LegendarySword_ControllerManager.StickType.L) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.UP, LegendarySword_ControllerManager.ButtonPushType.DOWN) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.UP, LegendarySword_ControllerManager.ButtonPushType.HOLD) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.RIGHT, LegendarySword_ControllerManager.ButtonPushType.DOWN) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.RIGHT, LegendarySword_ControllerManager.ButtonPushType.HOLD) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.LEFT, LegendarySword_ControllerManager.ButtonPushType.DOWN) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.LEFT, LegendarySword_ControllerManager.ButtonPushType.HOLD) && !LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.DOWN, LegendarySword_ControllerManager.ButtonPushType.DOWN))
			{
				LegendarySword_Define.CM.IsPushCrossKey(userType, LegendarySword_ControllerManager.CrossKeyType.DOWN, LegendarySword_ControllerManager.ButtonPushType.HOLD);
			}
			if (LegendarySword_Define.CM.IsPushButton_A(userType, LegendarySword_ControllerManager.ButtonPushType.DOWN))
			{
				UnityEngine.Debug.Log("Aボタンを押した");
				if (chara.AccelInput())
				{
					CharacterShake();
				}
			}
		}
		else
		{
			ai.UpdateMethod();
		}
	}
	public void CharacterShake(int _shakeCnt = 2)
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.value(base.gameObject, 0f, 1f, 0.02f).setOnUpdate(delegate(float _value)
		{
			base.transform.SetLocalPositionX(originPos.x + (LeanTween.shake.Evaluate(_value) - 0.5f) * 0.003f);
		}).setLoopPingPong(_shakeCnt)
			.setOnComplete((Action)delegate
			{
				base.transform.SetLocalPositionX(originPos.x);
			});
	}
	public GameObject GetMoveChara()
	{
		return moveCharacter;
	}
	public void PlayerFaceJoy()
	{
		characterStyle.SetMainCharacterFaceDiff((int)UserType, StyleTextureManager.MainCharacterFaceType.HAPPY);
	}
	public void PlayerFaceSorrow()
	{
		characterStyle.SetMainCharacterFaceDiff((int)UserType, StyleTextureManager.MainCharacterFaceType.SAD);
	}
}
