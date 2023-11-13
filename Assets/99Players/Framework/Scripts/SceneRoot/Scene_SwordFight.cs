using System.Collections.Generic;
using UnityEngine;
public class Scene_SwordFight : MonoBehaviour {
    private bool isInit;
    private List<int> team1Data = new List<int>();
    private List<int> team2Data = new List<int>();
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void QuitGame() {
    }
    private void Awake() {
        SwordFight_Define.IS_TEAM_MODE = false;
        LightingSettings.ChangeSceneLighting();
        if (!SwordFight_Define.IS_TEAM_MODE) {
            return;
        }
        team1Data = new List<int>(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0]);
        team2Data = new List<int>(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1]);
        SwordFight_Define.PlayerGroupList[0] = team1Data;
        SwordFight_Define.PlayerGroupList[1] = team2Data;
        for (int i = 0; i < SwordFight_Define.MAX_GAME_PLAYER_NUM - SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            if (SwordFight_Define.PlayerGroupList[0].Count != SwordFight_Define.MAX_GAME_TEAM_PLAYER_NUM) {
                SwordFight_Define.PlayerGroupList[0].Add((i == 0) ? (-1) : (-2));
            } else if (SwordFight_Define.PlayerGroupList[1].Count != SwordFight_Define.MAX_GAME_TEAM_PLAYER_NUM) {
                SwordFight_Define.PlayerGroupList[1].Add((i == 0) ? (-1) : (-2));
            }
        }
        UnityEngine.Debug.Log("チ\u30fcム１：参加者番号" + SwordFight_Define.PlayerGroupList[0][0].ToString());
        UnityEngine.Debug.Log("チ\u30fcム１：参加者番号" + SwordFight_Define.PlayerGroupList[0][1].ToString());
        UnityEngine.Debug.Log("チ\u30fcム２：参加者番号" + SwordFight_Define.PlayerGroupList[1][0].ToString());
        UnityEngine.Debug.Log("チ\u30fcム２：参加者番号" + SwordFight_Define.PlayerGroupList[1][1].ToString());
    }
    private void OnDisable() {
        SwordFight_Define.SetNowWinningNum(0);
    }
    private void OnEnable() {
        OpenLayer();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<SwordFight_MainCharacterManager>.Instance.Init();
        SingletonCustom<SwordFight_CharacterUIManager>.Instance.Init();
        SingletonCustom<SwordFight_GameUiManager>.Instance.Init();
        SingletonCustom<SwordFight_MainGameManager>.Instance.Init(delegate {
            GameEnd();
        });
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                });
                isInit = true;
            }
            SingletonCustom<SwordFight_MainCharacterManager>.Instance.UpdateMethod();
            SingletonCustom<SwordFight_MainGameManager>.Instance.UpdateMethod();
            SingletonCustom<SwordFight_FieldManager>.Instance.UpdateMethod();
        }
    }
    private void GameEnd() {
    }
}
