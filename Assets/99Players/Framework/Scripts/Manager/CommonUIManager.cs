using System;
using io.ninenine.players.party3d.games.common;
using UnityEngine;
public class CommonUIManager : SingletonCustom<CommonUIManager> {
    public enum UIType {
        UpperUI_Point,
        UpperUI_Rank,
        UpperUI_Time,
        UpperUI_Empty,
        SideUI_Point,
        SideUI_Rank,
        SideUI_Time,
        SideUI_Empty,
        BottomUI_Point,
        BottomUI_Rank,
        BottomUI_Time,
        BottomUI_Empty,
        BottomUI_TimerOnly,
        TimerOnly,
        None
    }
    [Serializable]
    public struct LayoutData {
        [Header("レイアウトのオブジェクト")]
        public GameObject layoutObject;
        [Header("プレイヤ\u30fcのUIデ\u30fcタ")]
        public CommonPlayerUIData[] arrayPlayerUIData;
    }
    [SerializeField]
    [Header("画面右のボタン")]
    private CommonButtonExplanationUI buttonExplanationUI;
    [SerializeField]
    [Header("画面右のボタンのY座標（デフォルト時）")]
    private float buttonExplanationUI_PosY;
    [SerializeField]
    [Header("画面右のボタンのY座標（6人時の画面上にプレイヤ\u30fcUIがある場合）")]
    private float buttonExplanationUI_PosY_Upper_Six;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面４つ（上配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Upper_4;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面６つ（上配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Upper_6;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面４つ（左配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Side_4;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面６つ（左配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Side_6;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面４つ（下配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Bottom_4;
    [SerializeField]
    [Header("プレイヤ\u30fcUIデ\u30fcタ：１画面６つ（下配置パタ\u30fcン）")]
    private LayoutData layoutData_Single_Bottom_6;
    private LayoutData ActiveLayoutData;
    [SerializeField]
    [Header("時間クラス")]
    private CommonTimeUI timeUI;
    [SerializeField]
    [Header("現在／合計ポイントUIクラス")]
    private CommonCurrentTotalPointUI currentTotalPointUI;
    [SerializeField]
    [Header("現在／合計ゲ\u30fcム回数UIクラス")]
    private CommonCurrentTotalGameCntUI currentTotalGameCntUI;
    [SerializeField]
    [Header("ゲ\u30fcム開始時のル\u30fcル説明UIクラス")]
    private CommonStartGameRuleProductionUI startGameRuleProductionUI;
    [SerializeField]
    [Header("プレイヤ\u30fcアイコンの背景色用の色")]
    private Color[] arrayPlayerColor;
    [SerializeField]
    [Header("プレイヤ\u30fc番号プレハブ")]
    private CommonPlayerNo playerNoPrefab;
    [SerializeField]
    [Header("スコアプレハブ")]
    private CommonScorePoint scorePointPrefab;
    private CommonPlayerNo[] playerNoObjects = new CommonPlayerNo[GS_Define.PLAYER_MAX];
    private float offsetScorePointPosZ;
    [SerializeField]
    [Header("ポ\u30fcズUIオブジェクト")]
    private GameObject pauseObject;
    [SerializeField]
    [Header("スキップUI")]
    private CommonSkipUI skipUI;
    public void Init(UIType _uiType) {
        buttonExplanationUI.Init();
        bool flag = false;
        layoutData_Single_Upper_4.layoutObject.SetActive(value: false);
        layoutData_Single_Upper_6.layoutObject.SetActive(value: false);
        layoutData_Single_Side_4.layoutObject.SetActive(value: false);
        layoutData_Single_Side_6.layoutObject.SetActive(value: false);
        layoutData_Single_Bottom_4.layoutObject.SetActive(value: false);
        layoutData_Single_Bottom_6.layoutObject.SetActive(value: false);
        switch (_uiType) {
            case UIType.UpperUI_Point:
            case UIType.UpperUI_Rank:
            case UIType.UpperUI_Time:
            case UIType.UpperUI_Empty:
                flag = true;
                //buttonExplanationUI.transform.SetLocalPositionY((SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length == GS_Define.PLAYER_SMALL_MAX) ? buttonExplanationUI_PosY : buttonExplanationUI_PosY_Upper_Six);
                ActiveLayoutData = ((SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length == GS_Define.PLAYER_SMALL_MAX) ? layoutData_Single_Upper_4 : layoutData_Single_Upper_6);
                timeUI.Init(Anchor.TopMiddle);
                break;
            case UIType.SideUI_Point:
            case UIType.SideUI_Rank:
            case UIType.SideUI_Time:
            case UIType.SideUI_Empty:
                flag = true;
                //buttonExplanationUI.transform.SetLocalPositionY(buttonExplanationUI_PosY);
                ActiveLayoutData = ((SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length == GS_Define.PLAYER_SMALL_MAX) ? layoutData_Single_Side_4 : layoutData_Single_Side_6);
                timeUI.Init(Anchor.TopMiddle);
                break;
            case UIType.BottomUI_Point:
            case UIType.BottomUI_Rank:
            case UIType.BottomUI_Time:
            case UIType.BottomUI_Empty:
                flag = true;
                ActiveLayoutData = ((SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length == GS_Define.PLAYER_SMALL_MAX) ? layoutData_Single_Bottom_4 : layoutData_Single_Bottom_6);
                //pauseObject.transform.SetLocalPositionY(0f - pauseObject.transform.localPosition.y);
                timeUI.Init(Anchor.BottomMiddle);
                break;
            case UIType.BottomUI_TimerOnly:
                //buttonExplanationUI.transform.SetLocalPositionY(buttonExplanationUI_PosY);
                timeUI.Init(Anchor.BottomMiddle);
                break;
            case UIType.TimerOnly:
                //buttonExplanationUI.transform.SetLocalPositionY(buttonExplanationUI_PosY);
                timeUI.Init(Anchor.TopMiddle);
                break;
            case UIType.None:
            default:
                timeUI.Init(Anchor.TopMiddle);
                timeUI.gameObject.SetActive(value: false);
                break;
        }
        if (flag) {
            ActiveLayoutData.layoutObject.SetActive(value: true);
            UnityEngine.Debug.Log("arrayPlayerUIData:" + ActiveLayoutData.arrayPlayerUIData.Length.ToString());
            UnityEngine.Debug.Log("Len:" + SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length.ToString());
            for (int i = 0; i < ActiveLayoutData.arrayPlayerUIData.Length; i++) {
                UnityEngine.Debug.Log("i:" + i.ToString());
                int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
                ActiveLayoutData.arrayPlayerUIData[i].Init(_uiType);
                ActiveLayoutData.arrayPlayerUIData[i].SetPlayerIcon(num);
                ActiveLayoutData.arrayPlayerUIData[i].SetPlayerIconBack(arrayPlayerColor[i]);
            }
        }
        currentTotalPointUI.gameObject.SetActive(value: false);
        currentTotalGameCntUI.gameObject.SetActive(value: false);
        startGameRuleProductionUI.Init();
        skipUI.gameObject.SetActive(value: false);
    }
    public void AddButtonExplanation(int _idx, string _text, CommonButtonExplanationUI.ButtonType _leftButtonType = CommonButtonExplanationUI.ButtonType.None, CommonButtonExplanationUI.ButtonType _rightButtonType = CommonButtonExplanationUI.ButtonType.None, float _diffPosY = 0f) {
        buttonExplanationUI.AddButtonExplanation(_idx, _text, _leftButtonType, _rightButtonType);
    }
    public void SetButtonExplanation() {
        buttonExplanationUI.SetButtonExplanation();
    }
    public void SetButtonExplanationPos(Vector3 _pos) {
        //buttonExplanationUI.transform.localPosition = _pos;
    }
    public void SortPlayerUIData(int[] _sort) {
        Vector3[] array = new Vector3[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++) {
            array[i] = ActiveLayoutData.arrayPlayerUIData[i].transform.localPosition;
        }
        for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++) {
            ActiveLayoutData.arrayPlayerUIData[j].transform.localPosition = array[_sort[j]];
        }
    }
    public void SetPoint(int _playerNo, int _point, int _prevPoint = -1) {
        ActiveLayoutData.arrayPlayerUIData[_playerNo].SetPoint(_point, _prevPoint);
    }
    public void HidePoint() {
        for (int i = 0; i < ActiveLayoutData.arrayPlayerUIData.Length; i++) {
            ActiveLayoutData.arrayPlayerUIData[i].HidePoint((float)i * 0.15f);
        }
    }
    public void SetRank(int _playerNo, int _rank) {
        ActiveLayoutData.arrayPlayerUIData[_playerNo].SetRank(_rank);
    }
    public void SetTime_PlayerUI(int _playerNo, float _time) {
        ActiveLayoutData.arrayPlayerUIData[_playerNo].SetTime(_time);
    }
    public void SetTime(float _time, bool _isCountdown = false) {
        timeUI.SetTime(_time);
        if (_isCountdown) {
            SingletonCustom<CommonCountdownProductionUI>.Instance.UpdateCountdown(_time);
        }
    }
    public void SetTimePos(Vector3 _pos) {
        //timeUI.transform.localPosition = _pos;
    }
    public void SetTimeUIEnable(bool isEnable) {
        timeUI.gameObject.SetActive(isEnable);
    }
    public void ShowStartGameRule(Action _callBack = null) {
        UnityEngine.Debug.Log("production:" + startGameRuleProductionUI?.ToString());
        startGameRuleProductionUI.ShowStartGameRule(_callBack);
    }
    public CommonPlayerUIData GetPlayerUIData(int _playerNo) {
        return ActiveLayoutData.arrayPlayerUIData[_playerNo];
    }
    public void InitPointUIData(int _targetTotalPoint) {
        currentTotalPointUI.SetCurrentPoint(0);
        currentTotalPointUI.SetTargetTotalPoint(_targetTotalPoint);
        currentTotalPointUI.gameObject.SetActive(value: true);
        // TODO: Temporary fix for UI overlapping
        timeUI.Init(Anchor.BottomMiddle);
    }
    public void SetCurrentPoint(int _point, int _prevPoint = -1) {
        if (_point != _prevPoint) {
            currentTotalPointUI.SetCurrentPoint(_point, _prevPoint);
        }
    }
    public void AddCurrentPoint(int _point, int _playerNo) {
        currentTotalPointUI.AddCurrentPoint(_point, _playerNo);
    }
    public void SetCurrentPointPos(Vector3 _pos) {
        //currentTotalPointUI.transform.localPosition = _pos;
    }
    public void InitGameCntUIData(int _targetTotalGameCnt) {
        currentTotalGameCntUI.SetCurrentGameCnt(1);
        currentTotalGameCntUI.SetTotalGameCnt(_targetTotalGameCnt);
        currentTotalGameCntUI.gameObject.SetActive(value: true);
    }
    public void SetCurrentGameCnt(int _cnt) {
        currentTotalGameCntUI.SetCurrentGameCnt(_cnt);
    }
    public void SetGameCntPos(Vector3 _pos) {
        //currentTotalGameCntUI.transform.localPosition = _pos;
    }
    public void SetPauseButtonPos(Vector3 _pos) {
        //pauseObject.transform.parent.localPosition = _pos;
    }
    public void ShowPlayerNo(int _playerNo, Vector3 _worldPos, Camera _camera, float _offsetY = 100f, float _time = 10f) {
        playerNoObjects[_playerNo] = UnityEngine.Object.Instantiate(playerNoPrefab, base.transform);
        playerNoObjects[_playerNo].gameObject.SetActive(value: true);
        playerNoObjects[_playerNo].transform.SetLocalPositionZ(-10f);
        if (_playerNo < SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
            CalcManager.mCalcVector3 = _camera.WorldToScreenPoint(_worldPos);
            CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
            playerNoObjects[_playerNo].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + _offsetY, playerNoObjects[_playerNo].transform.position.z);
        } else {
            playerNoObjects[_playerNo].gameObject.SetActive(value: false);
        }
        playerNoObjects[_playerNo].Init(_playerNo, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && _playerNo == 0);
        LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setDelay(_time).setOnUpdate(delegate (float _value) {
            playerNoObjects[_playerNo].spPlayerNo.SetAlpha(_value);
        })
            .setOnComplete((Action)delegate {
                UnityEngine.Object.Destroy(playerNoObjects[_playerNo].gameObject);
            });
    }
    public void UpdatePlayerNoPos(int _playerNo, Vector3 _worldPos, Camera _camera, float _offsetY = 100f) {
        if (!(playerNoObjects[_playerNo] == null) && playerNoObjects[_playerNo].gameObject.activeSelf && _playerNo < SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
            CalcManager.mCalcVector3 = _camera.WorldToScreenPoint(_worldPos);
            CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
            playerNoObjects[_playerNo].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + _offsetY, playerNoObjects[_playerNo].transform.position.z);
        }
    }
    public void SetPlayerNoLocalScale(float _localScale) {
        for (int i = 0; i < playerNoObjects.Length; i++) {
            if (playerNoObjects[i] != null) {
                playerNoObjects[i].transform.localScale = Vector3.one * _localScale;
            }
        }
    }
    public CommonScorePoint ShowScorePoint(int _playerNo, Vector3 _worldPos, CommonScorePoint.Type _score, Camera _camera, float _offsetY = 0f, float _size = 1f, float _offsetX = 0f, float _offsetZ = 0f, float _time = 1f) {
        CommonScorePoint commonScorePoint = UnityEngine.Object.Instantiate(scorePointPrefab, base.transform);
        commonScorePoint.gameObject.SetActive(value: true);
        commonScorePoint.transform.SetLocalPositionZ(-10f);
        CalcManager.mCalcVector3 = _camera.WorldToScreenPoint(_worldPos);
        CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
        commonScorePoint.transform.SetPosition(Mathf.Clamp(CalcManager.mCalcVector3.x + _offsetX, -870f, 870f), Mathf.Clamp(CalcManager.mCalcVector3.y + _offsetY, -458f, 458f), commonScorePoint.transform.position.z + _offsetZ + offsetScorePointPosZ);
        commonScorePoint.Init(_playerNo, _score, _size, _time);
        offsetScorePointPosZ -= 0.02f;
        return commonScorePoint;
    }
    public void ShowSkipUI() {
        // TODO: Temporary disable SkipUI since it's game dependant method
        skipUI.ShowSkipUI();
    }
    public void SetSkipPos(Vector3 _pos) {
        //skipUI.transform.localPosition = _pos;
    }
    public void HidePause() {
        LeanTween.moveLocal(pauseObject, pauseObject.transform.localPosition + new Vector3(-300f, 0f, 0f), 0.5f).setEaseInQuint();
    }
    private new void OnDestroy() {
        LeanTween.cancel(base.gameObject);
        LeanTween.cancel(pauseObject);
    }
}
