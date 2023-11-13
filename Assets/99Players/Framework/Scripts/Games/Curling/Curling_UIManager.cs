using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
public class Curling_UIManager : SingletonCustom<Curling_UIManager>
{
	public enum ArrowDir
	{
		STRAIGHT,
		LEFT_CURVE,
		RIGHT_CURVE
	}
	public enum ContorollerUIType
	{
		THROW_POWER,
		THROW,
		SWEEP,
		SKIP,
		MAX
	}
	public enum OperationUIType
	{
		THROW,
		SWEEP
	}
	public enum PlayerIconUIType
	{
		THROW,
		SWEEP
	}
	private Camera globalCamera;
	[SerializeField]
	[Header("ハウス付近を映す用のカメラ")]
	private Camera rendererTextureCamera;
	[SerializeField]
	private Material renderTextureMaterial;
	[SerializeField]
	[Header("タ\u30fcンカットイン")]
	private Curling_TurnCutIn turnCutIn;
	[SerializeField]
	[Header("タ\u30fcンフレ\u30fcム")]
	private GameObject turnFrame;
	private Vector3 originTurnFramePos;
	[SerializeField]
	[Header("チ\u30fcム別フレ\u30fcム")]
	private Curling_TeamFrame[] arrayTeamFrame;
	private Vector3[] arrayTeamFrameOriginPos;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer screenFade;
	private readonly float SCREEN_FADE_TIME = 1f;
	[SerializeField]
	[Header("投げる時の矢印")]
	private SpriteRenderer throwArrow;
	private Vector3 originArrowScale;
	[SerializeField]
	[Header("投げる時の矢印の画像")]
	private Sprite[] arrayArrowSprite;
	[SerializeField]
	[Header("予測の矢印")]
	private SpriteRenderer predictArrow;
	[SerializeField]
	[Header("予測の矢印用の画像")]
	private Sprite[] arrayPredictArrowSprite;
	private float defPredictArrowSizeY;
	[SerializeField]
	[Header("ゲ\u30fcム回数")]
	private TextMeshPro gameCnt;
	[SerializeField]
	[Header("ゲ\u30fcムの最大回数")]
	private TextMeshPro totalGameCnt;
	private float MOVE_ROOT_POS_X = -1500f;
	private float MOVE_ROOT_TIME = 0.5f;
	[SerializeField]
	[Header("投げる時に画面外へ移動させるオブジェクト")]
	private GameObject[] arrayMoveOutsideObject;
	private Vector3[] arrayOriginMoveOutsideObjectPos;
	private ControllerBalloonUI[] controllerUI;
	[SerializeField]
	[Header("操作吹き出しコントロ\u30fcラ\u30fcUI")]
	private ControllerBalloonUI[] arrayControllerUI;
	[SerializeField]
	[Header("操作吹き出しコントロ\u30fcラ\u30fcUI（English）")]
	private ControllerBalloonUI[] arrayControllerUI_EN;
	[SerializeField]
	[Header("操作吹き出しコントロ\u30fcラ\u30fcUIの補正座標")]
	private Vector3[] DIFF_CONTOROLLER_UI;
	private ContorollerUIType controllerUIType;
	private ContorollerUIType before_ControllerUIType;
	private GameObject[] operationUI;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI")]
	private GameObject[] arrayOperationUI;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI（English）")]
	private GameObject[] arrayOperationUI_EN;
	private OperationUIType before_OperationType;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer[] arrayPlayerIcon;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の画像")]
	private Sprite[] arrayPlayerIconSprite;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像")]
	private Sprite playerIconSprite_Single;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン用の「あなた」画像（English）")]
	private Sprite playerIconSprite_Single_EN;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコンの補正座標")]
	private Vector3[] DIFF_PLAYER_ICON;
	[SerializeField]
	[Header("JoyConButton")]
	private JoyConButton[] arrayJoyConButton;
	[SerializeField]
	[Header("獲得ポイントを格納するル\u30fcト")]
	private Transform getPointRoot;
	[SerializeField]
	[Header("獲得ポイントのプレハブ")]
	private Curling_GetPoint getPointPref;
	private List<Curling_GetPoint> getPointObjList = new List<Curling_GetPoint>();
	public void Init()
	{
		RenderTexture renderTexture = new RenderTexture(256, 256, 24, GraphicsFormat.R8G8B8A8_UNorm);
		renderTexture.Create();
		rendererTextureCamera.targetTexture = renderTexture;
		renderTextureMaterial.mainTexture = renderTexture;
		globalCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		originTurnFramePos = turnFrame.transform.localPosition;
		arrayTeamFrameOriginPos = new Vector3[arrayTeamFrame.Length];
		for (int i = 0; i < arrayTeamFrameOriginPos.Length; i++)
		{
			arrayTeamFrameOriginPos[i] = arrayTeamFrame[i].transform.localPosition;
		}
		arrayOriginMoveOutsideObjectPos = new Vector3[arrayMoveOutsideObject.Length];
		for (int j = 0; j < arrayMoveOutsideObject.Length; j++)
		{
			arrayOriginMoveOutsideObjectPos[j] = arrayMoveOutsideObject[j].transform.localPosition;
		}
		LeanTween.delayedCall(base.gameObject, 0.01f, (Action)delegate
		{
			SetGameCnt();
			totalGameCnt.text = SingletonCustom<TextDataBaseManager>.Instance.GetReplaceTag(TextDataBaseItemEntity.DATABASE_NAME.RECEIVE_PON, 7, TextDataBaseItemEntity.TAG_NAME.NUMBER, Curling_Define.GAME_CNT.ToString());
		});
		originArrowScale = throwArrow.transform.localScale;
		turnCutIn.Init();
		for (int k = 0; k < arrayControllerUI.Length; k++)
		{
			arrayControllerUI[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < arrayControllerUI_EN.Length; l++)
		{
			arrayControllerUI_EN[l].gameObject.SetActive(value: false);
		}
		for (int m = 0; m < arrayOperationUI.Length; m++)
		{
			arrayOperationUI[m].gameObject.SetActive(value: false);
		}
		for (int n = 0; n < arrayOperationUI_EN.Length; n++)
		{
			arrayOperationUI_EN[n].gameObject.SetActive(value: false);
		}
		controllerUI = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayControllerUI : arrayControllerUI_EN);
		operationUI = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayOperationUI : arrayOperationUI_EN);
		defPredictArrowSizeY = predictArrow.size.y;
	}
	public void InitPlay()
	{
		for (int i = 0; i < controllerUI.Length; i++)
		{
			controllerUI[i].Init();
		}
		controllerUIType = ContorollerUIType.THROW_POWER;
		before_ControllerUIType = ContorollerUIType.MAX;
		for (int j = 0; j < operationUI.Length; j++)
		{
			operationUI[j].SetActive(j == 0);
		}
		before_OperationType = OperationUIType.THROW;
		for (int k = 0; k < arrayPlayerIcon.Length; k++)
		{
			SetPlayerIconActive(k, _isActive: false);
		}
		SetThrowArrowActive(_isActive: false);
		SetThrowArrowRot(ArrowDir.STRAIGHT);
		SetThrowArrowAngle(0f);
		SetThrowArrowPower(0f);
		for (int l = 0; l < arrayMoveOutsideObject.Length; l++)
		{
			arrayMoveOutsideObject[l].transform.localPosition = arrayOriginMoveOutsideObjectPos[l];
		}
		ClearGetPointList();
	}
	public void UpdateMethod()
	{
		SetContollerType();
	}
	public void SetContollerType()
	{
		for (int i = 0; i < arrayJoyConButton.Length; i++)
		{
			if (!SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().GetIsCpu())
			{
				arrayJoyConButton[i].SetPlayerType((JoyConButton.PlayerType)SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer().GetUserType());
			}
			else
			{
				arrayJoyConButton[i].SetPlayerType(JoyConButton.PlayerType.PLAYER_1);
			}
			arrayJoyConButton[i].CheckJoyconButton();
		}
	}
	public void LateUpdateMethod()
	{
		switch (SingletonCustom<Curling_GameManager>.Instance.GetState())
		{
		case Curling_GameManager.State.PREP_THROW:
		case Curling_GameManager.State.THROW:
		{
			Curling_Player houseSweepPlayer = SingletonCustom<Curling_GameManager>.Instance.GetThrowPlayer();
			if (houseSweepPlayer.GetUserType() < Curling_Define.UserType.CPU_1)
			{
				CalcManager.mCalcVector3 = SingletonCustom<Curling_GameManager>.Instance.GetCamera().WorldToScreenPoint(houseSweepPlayer.transform.position);
				CalcManager.mCalcVector3 = globalCamera.ScreenToWorldPoint(CalcManager.mCalcVector3);
				if (controllerUIType != ContorollerUIType.MAX)
				{
					controllerUI[(int)controllerUIType].transform.SetPosition(CalcManager.mCalcVector3.x + DIFF_CONTOROLLER_UI[(int)controllerUIType].x, CalcManager.mCalcVector3.y + DIFF_CONTOROLLER_UI[(int)controllerUIType].y, controllerUI[(int)controllerUIType].transform.position.z);
				}
				arrayPlayerIcon[0].transform.SetPosition(CalcManager.mCalcVector3.x + DIFF_PLAYER_ICON[0].x, CalcManager.mCalcVector3.y + DIFF_PLAYER_ICON[0].y, arrayPlayerIcon[0].transform.position.z);
			}
			break;
		}
		case Curling_GameManager.State.HOUSE_SWEEP:
		case Curling_GameManager.State.PLAY_END:
		{
			Curling_Player houseSweepPlayer = SingletonCustom<Curling_GameManager>.Instance.GetHouseSweepPlayer();
			if (houseSweepPlayer.GetUserType() < Curling_Define.UserType.CPU_1)
			{
				CalcManager.mCalcVector3 = SingletonCustom<Curling_GameManager>.Instance.GetCamera().WorldToScreenPoint(houseSweepPlayer.transform.position);
				CalcManager.mCalcVector3 = globalCamera.ScreenToWorldPoint(CalcManager.mCalcVector3);
				if (controllerUIType != ContorollerUIType.MAX)
				{
					controllerUI[(int)controllerUIType].transform.SetPosition(CalcManager.mCalcVector3.x + DIFF_CONTOROLLER_UI[(int)controllerUIType].x, CalcManager.mCalcVector3.y + DIFF_CONTOROLLER_UI[(int)controllerUIType].y, controllerUI[(int)controllerUIType].transform.position.z);
				}
				arrayPlayerIcon[1].transform.SetPosition(CalcManager.mCalcVector3.x + DIFF_PLAYER_ICON[1].x, CalcManager.mCalcVector3.y + DIFF_PLAYER_ICON[1].y, arrayPlayerIcon[1].transform.position.z);
			}
			break;
		}
		}
	}
	public void TeamFrameInit(int _teamNo, Curling_Define.UserType _firstThrowUserType, Curling_Define.UserType _secondThrowUserType)
	{
		arrayTeamFrame[_teamNo].Init(_teamNo, _firstThrowUserType, _secondThrowUserType);
	}
	public void TeamFrameInitPlay(int _teamNo)
	{
		arrayTeamFrame[_teamNo].InitPlay(_teamNo);
		int num = (_teamNo != (int)SingletonCustom<Curling_GameManager>.Instance.GetTurnTeam()) ? 1 : 0;
		arrayTeamFrame[_teamNo].transform.localPosition = arrayTeamFrameOriginPos[num];
	}
	public void ShowTurnCutIn(Action _callBack)
	{
		turnCutIn.ShowTurnCutIn(_callBack);
	}
	public void SkipTurnCutIn()
	{
		turnCutIn.SkipTurnCutIn();
	}
	public void SetTrunFrame(int _teamNo)
	{
		turnFrame.transform.parent = arrayTeamFrame[_teamNo].transform;
		turnFrame.transform.localPosition = originTurnFramePos;
	}
	public void SetPlayerIconActive(int _idx, bool _isActive)
	{
		arrayPlayerIcon[_idx].gameObject.SetActive(_isActive);
	}
	public void SetPlayerIcon(Curling_Define.UserType _throwUserType, Curling_Define.UserType _houseSweepUserType)
	{
		if (_throwUserType < Curling_Define.UserType.CPU_1)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				arrayPlayerIcon[0].sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerIconSprite_Single : playerIconSprite_Single_EN);
			}
			else
			{
				arrayPlayerIcon[0].sprite = arrayPlayerIconSprite[(int)_throwUserType];
			}
		}
		if (_houseSweepUserType < Curling_Define.UserType.CPU_1)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				arrayPlayerIcon[1].sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? playerIconSprite_Single : playerIconSprite_Single_EN);
			}
			else
			{
				arrayPlayerIcon[1].sprite = arrayPlayerIconSprite[(int)_houseSweepUserType];
			}
		}
	}
	public void MoveUIOutside()
	{
		for (int i = 0; i < arrayMoveOutsideObject.Length; i++)
		{
			LeanTween.moveLocalX(arrayMoveOutsideObject[i], MOVE_ROOT_POS_X, MOVE_ROOT_TIME);
		}
	}
	public void SetThrowStoneIcon(int _teamNo, int _throwCnt)
	{
		arrayTeamFrame[_teamNo].SetThrowStoneIcon(_throwCnt);
	}
	public void SetPoint(int _teamNo, int _point)
	{
		arrayTeamFrame[_teamNo].SetPoint(_point);
	}
	public void SetThrowArrowActive(bool _isActive)
	{
		throwArrow.gameObject.SetActive(_isActive);
	}
	public void SetThrowArrowRot(ArrowDir _arrowDir)
	{
		throwArrow.transform.SetLocalScaleX((_arrowDir == ArrowDir.RIGHT_CURVE) ? (0f - originArrowScale.x) : originArrowScale.x);
		throwArrow.sprite = arrayArrowSprite[(int)_arrowDir];
		predictArrow.sprite = arrayPredictArrowSprite[(int)_arrowDir];
	}
	public void SetThrowArrowAngle(float _angle)
	{
		throwArrow.transform.SetLocalEulerAnglesZ(_angle);
	}
	public void SetThrowArrowPower(float _power)
	{
		throwArrow.transform.SetLocalScaleY(originArrowScale.y * _power);
	}
	public void SetPredictArrowActive(bool _isActive)
	{
		predictArrow.gameObject.SetActive(_isActive);
		if (!_isActive)
		{
			Vector2 size = predictArrow.size;
			size.y = defPredictArrowSizeY;
			predictArrow.size = size;
			predictArrow.transform.SetLocalEulerAnglesY(0f);
		}
	}
	public void SetPredictArrow(Vector3 _originPos, Vector3 _minPos, Vector3 _maxPos)
	{
		predictArrow.transform.position = _originPos;
		float num = CalcManager.Length(_minPos, _maxPos);
		Vector2 size = predictArrow.size;
		size.y = defPredictArrowSizeY + num / predictArrow.transform.localScale.y;
		predictArrow.size = size;
		Vector3 vector = _maxPos - _originPos;
		vector.y = 0f;
		float num2 = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		UnityEngine.Debug.Log("angle " + num2.ToString());
		predictArrow.transform.SetEulerAnglesY((num2 - 90f) * -1f);
	}
	public void SetGameCnt()
	{
		gameCnt.text = SingletonCustom<TextDataBaseManager>.Instance.GetReplaceTag(TextDataBaseItemEntity.DATABASE_NAME.RECEIVE_PON, 6, TextDataBaseItemEntity.TAG_NAME.NUMBER, (SingletonCustom<Curling_GameManager>.Instance.GetGameCnt() + 1).ToString());
	}
	public void ShowGetPoint(int _teamNo, Vector3 _pos, int _pointIdx, int _pointCnt)
	{
		Curling_GetPoint curling_GetPoint = UnityEngine.Object.Instantiate(getPointPref, getPointRoot);
		curling_GetPoint.SetPoint(_teamNo, _pos, _pointIdx, _pointCnt);
		curling_GetPoint.Show();
		getPointObjList.Add(curling_GetPoint);
	}
	public void ClearGetPointList()
	{
		for (int i = 0; i < getPointObjList.Count; i++)
		{
			UnityEngine.Object.Destroy(getPointObjList[i].gameObject);
		}
		getPointObjList.Clear();
	}
	public void SetOperationUI(OperationUIType _operationUIType)
	{
		operationUI[(int)before_OperationType].SetActive(value: false);
		operationUI[(int)_operationUIType].SetActive(value: true);
		before_OperationType = _operationUIType;
	}
	public void SetControlInfoBalloonActive(ContorollerUIType _controllerType)
	{
		if (before_ControllerUIType == _controllerType)
		{
			return;
		}
		controllerUIType = _controllerType;
		if (_controllerType == ContorollerUIType.MAX)
		{
			for (int i = 0; i < controllerUI.Length; i++)
			{
				controllerUI[i].gameObject.SetActive(value: false);
			}
			before_ControllerUIType = _controllerType;
		}
		else
		{
			if (before_ControllerUIType != ContorollerUIType.MAX && before_ControllerUIType != _controllerType)
			{
				controllerUI[(int)before_ControllerUIType].gameObject.SetActive(value: false);
			}
			controllerUI[(int)_controllerType].gameObject.SetActive(value: true);
			before_ControllerUIType = _controllerType;
		}
	}
	public void HideControlInfoBalloon(ContorollerUIType _controllerType)
	{
		controllerUI[(int)_controllerType].FadeProcess_ControlInfomationUI(_fadeIn: false);
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
			if (SingletonCustom<Curling_GameManager>.Instance.GetIsSkip())
			{
				SingletonCustom<SceneManager>.Instance.IsFade = true;
				Time.timeScale = (SingletonCustom<Curling_GameManager>.Instance.GetIsSkip() ? 3f : 1f);
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
		yield return new WaitWhile(() => SingletonCustom<Curling_GameManager>.Instance.GetThrowStone().transform.position.z < SingletonCustom<Curling_GameManager>.Instance.GetSkipFadeOutAnchor().position.z);
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
	public Camera GetGlobalCamera()
	{
		return globalCamera;
	}
	private void OnDisable()
	{
		RenderTexture targetTexture = rendererTextureCamera.targetTexture;
		rendererTextureCamera.targetTexture = null;
		renderTextureMaterial.mainTexture = null;
		targetTexture.Release();
	}
}
