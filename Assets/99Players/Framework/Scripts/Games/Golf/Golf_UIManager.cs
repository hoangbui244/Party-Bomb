using System;
using System.Collections;
using UnityEngine;
public class Golf_UIManager : SingletonCustom<Golf_UIManager>
{
	public enum ContorollerType
	{
		Shot_Ready_Dir,
		Shot_Power_Imact,
		Skip
	}
	private Camera globalCamera;
	[SerializeField]
	[Header("ゲ\u30fcム回数クラス")]
	private Golf_GameCountUI gameCntUI;
	[SerializeField]
	[Header("プレイヤ\u30fcUIクラス")]
	private Golf_PlayerUI[] arrayPlayerUI;
	[SerializeField]
	[Header("タ\u30fcンカットインクラス")]
	private TurnCutIn turnCutIn;
	[SerializeField]
	[Header("打つ場所のUIクラス")]
	private Golf_HitPointUI hitPointUI;
	[SerializeField]
	[Header("風のUIクラス")]
	private Golf_WindUI windUI;
	[SerializeField]
	[Header("ゲ\u30fcジUIクラス")]
	private Golf_GaugeUI gaugeUI;
	[SerializeField]
	[Header("カップ付近のパワ\u30fc割合")]
	private float CUP_POWER_LERP;
	[SerializeField]
	[Header("カップ付近のインパクトのズレの割合")]
	private float CUP_IMPACT_LERP;
	[SerializeField]
	[Header("カップまでの距離UIクラス")]
	private Golf_ToCupDistanceUI distanceToCupUI;
	[SerializeField]
	[Header("ポイント結果UIクラス")]
	private Golf_PointResultUI pointResultUI;
	[SerializeField]
	[Header("旗の位置UIクラス")]
	private Golf_FlagUI flagUI;
	[SerializeField]
	[Header("ゲ\u30fcム開始前のアナウンスUIクラス")]
	private Golf_AnnounceUI announceUI;
	[SerializeField]
	[Header("残りヤ\u30fcドの表示UIクラス")]
	private Golf_ViewCupYardUI viewCupYardUI;
	[SerializeField]
	[Header("操作吹き出しUIクラス")]
	private ControllerOperationBalloonUI operationBalloonUI;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUIクラス")]
	private ControllerOperationExplanationUI operationExplanationUI;
	private readonly float UI_MOVE_TIME = 0.25f;
	private readonly float SCREEN_FADE_TIME = 1f;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer screenFade;
	private readonly float SKIP_FADE_OUT_DISTANCE = 30f;
	[SerializeField]
	[Header("UIのル\u30fcト")]
	private Transform uiRoot;
	private JoyConButton[] arrayJoyConButton;
	public void Init()
	{
		globalCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		arrayJoyConButton = uiRoot.GetComponentsInChildren<JoyConButton>(includeInactive: true);
		CUP_POWER_LERP = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetRequiredPower();
		gameCntUI.Init();
		Vector3[] array = new Vector3[arrayPlayerUI.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = arrayPlayerUI[i].transform.position;
		}
		for (int j = 0; j < arrayPlayerUI.Length; j++)
		{
			int indexOfOrderOfPlay = SingletonCustom<Golf_GameManager>.Instance.GetIndexOfOrderOfPlay(j);
			arrayPlayerUI[j].transform.position = array[indexOfOrderOfPlay];
			arrayPlayerUI[j].Init(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0]);
		}
		hitPointUI.Init();
		gaugeUI.Init();
		distanceToCupUI.Init();
		pointResultUI.Init();
		flagUI.Init();
		announceUI.Init();
		viewCupYardUI.Init();
		operationBalloonUI.Init(_isPlayer: true);
		operationExplanationUI.Init(_isPlayer: true);
		InitPlay();
	}
	public void InitPlay()
	{
		SetJoyConButton();
		hitPointUI.InitPlay();
		windUI.InitPlay();
		gaugeUI.InitPlay();
		distanceToCupUI.InitPlay();
		pointResultUI.InitPlay();
		flagUI.InitPlay();
		viewCupYardUI.InitPlay();
	}
	public void ResetShotReadyInitPlay()
	{
		hitPointUI.InitPlay();
		gaugeUI.InitPlay();
	}
	public void UpdateMethod()
	{
		SetJoyConButton();
		if (!SingletonCustom<Golf_GameManager>.Instance.GetIsWaitUpdate())
		{
			switch (SingletonCustom<Golf_GameManager>.Instance.GetState())
			{
			case Golf_GameManager.State.SHOT_READY:
				flagUI.UpdateMethod();
				break;
			case Golf_GameManager.State.SHOT_POWER:
			case Golf_GameManager.State.SHOT_IMPACT:
				gaugeUI.UpdateMethod();
				flagUI.UpdateMethod();
				break;
			}
		}
	}
	private void SetJoyConButton()
	{
		Golf_Player turnPlayer = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
		for (int i = 0; i < arrayJoyConButton.Length; i++)
		{
			if (!turnPlayer.GetIsCpu())
			{
				arrayJoyConButton[i].SetPlayerType((JoyConButton.PlayerType)turnPlayer.GetUserType());
			}
			else
			{
				arrayJoyConButton[i].SetPlayerType((JoyConButton.PlayerType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][0]);
			}
		}
	}
	public void SetGameCnt(int _gameCnt)
	{
		gameCntUI.SetGameCnt(_gameCnt);
	}
	public void ShowTurnCutIn(int _userType, float _delay, Action _callBack = null)
	{
		turnCutIn.ShowTurnCutIn(_userType, _delay, _callBack);
	}
	public void SetPoint(int _playerNo, int _point)
	{
		arrayPlayerUI[_playerNo].SetPoint(_point);
	}
	public void ShowHitPoint()
	{
		hitPointUI.Move(_inside: true, UI_MOVE_TIME);
	}
	public void HideHitPoint()
	{
		hitPointUI.Move(_inside: false, UI_MOVE_TIME);
	}
	public void MoveHitPoint(Vector3 _vec)
	{
		hitPointUI.MoveHitPoint(_vec);
	}
	public void ShowGauge()
	{
		gaugeUI.Move(_inside: true, UI_MOVE_TIME);
	}
	public void HideGauge()
	{
		gaugeUI.Move(_inside: false, UI_MOVE_TIME);
	}
	public bool GetIsGaugeBarReturnOriginPos()
	{
		return gaugeUI.GetIsReturnOriginPos();
	}
	public float GetShotPowerLerp(bool _isInput = false)
	{
		return gaugeUI.GetShotPowerLerp(_isInput);
	}
	public float GetCupPowerLerp()
	{
		return CUP_POWER_LERP;
	}
	public float GetImpactDiff(bool _isInput = false)
	{
		return gaugeUI.GetImpactDiff(_isInput);
	}
	public float GetCupImpactLerp()
	{
		return CUP_IMPACT_LERP;
	}
	public void HideDistanceToCupUI()
	{
		distanceToCupUI.Move(UI_MOVE_TIME);
	}
	public void ShowPointResultUI(float _remainingDistanceToCup, int _addPoint)
	{
		pointResultUI.Show(_remainingDistanceToCup, _addPoint);
	}
	public void HidePointResultUI()
	{
		pointResultUI.Hide();
	}
	public float GetPointResultViewTime()
	{
		return pointResultUI.GetResultViewTime();
	}
	public void ShowFlagUI()
	{
		flagUI.Show();
	}
	public void HideFlagUI()
	{
		flagUI.Hide();
	}
	public void ShowAnnounceUI(Action _callBack)
	{
		announceUI.Show(_callBack);
	}
	public void ShowViewCupYard()
	{
		viewCupYardUI.Show();
	}
	public void HideViewCupYard()
	{
		viewCupYardUI.Hide();
	}
	public float GetUIMoveTime()
	{
		return UI_MOVE_TIME;
	}
	public Camera GetGlobalCamera()
	{
		return globalCamera;
	}
	public void SetControllerBalloonActive(ContorollerType _controllerType, bool _isFade, bool _isActive)
	{
		operationBalloonUI.SetControllerBalloonActive((int)_controllerType, _isFade, _isActive);
	}
	public void SetControllerExplanationActive(ContorollerType _controllerType)
	{
		operationExplanationUI.ChangeControllerUIType((int)_controllerType);
	}
	public void StartScreenFade(Action _fadeInCallBack = null, Action _fadeOutCallBack = null)
	{
		Fade(isView: true, SCREEN_FADE_TIME);
		LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
		{
			if (_fadeInCallBack != null)
			{
				_fadeInCallBack();
			}
			if (SingletonCustom<Golf_GameManager>.Instance.GetIsSkip())
			{
				SingletonCustom<SceneManager>.Instance.IsFade = true;
				Time.timeScale = 3f;
				UnityEngine.Debug.Log("timeScale : " + Time.timeScale.ToString());
				StartCoroutine(WaitFadeOut(_fadeOutCallBack));
			}
			else
			{
				FadeOut(_fadeOutCallBack);
			}
		});
	}
	private void FadeOut(Action _fadeOutCallBack = null)
	{
		SingletonCustom<SceneManager>.Instance.IsFade = false;
		Time.timeScale = 1f;
		Fade(isView: false, SCREEN_FADE_TIME);
		LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
		{
			if (_fadeOutCallBack != null)
			{
				_fadeOutCallBack();
			}
		});
	}
	private IEnumerator WaitFadeOut(Action _fadeOutCallBack = null)
	{
		Golf_Ball ball = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		yield return new WaitWhile(() => SingletonCustom<Golf_BallManager>.Instance.GetDistanceCarry(ball.transform.position) < SKIP_FADE_OUT_DISTANCE && !ball.GetIsAnyCollision());
		FadeOut(_fadeOutCallBack);
	}
	private void Fade(bool isView, float _fadeTime)
	{
		Color alpha;
		LeanTween.value(screenFade.gameObject, isView ? 0f : 1f, isView ? 1f : 0f, _fadeTime).setOnUpdate(delegate(float val)
		{
			alpha = screenFade.color;
			alpha.a = val;
			screenFade.color = alpha;
		});
	}
	public void HideUI()
	{
		HidePointResultUI();
	}
}
