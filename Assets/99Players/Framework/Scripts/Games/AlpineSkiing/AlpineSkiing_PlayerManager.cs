using System;
using UnityEngine;
public class AlpineSkiing_PlayerManager : SingletonCustom<AlpineSkiing_PlayerManager> {
    [Serializable]
    public struct UserData {
        public AlpineSkiing_Player player;
        public AlpineSkiing_Define.UserType userType;
        public Camera camera;
        public float goalTime;
        public bool isPlayer;
    }
    [SerializeField]
    [Header("プレイヤ\u30fc")]
    private AlpineSkiing_Player[] players;
    [SerializeField]
    [Header("カメラ")]
    private Camera[] cameras;
    [SerializeField]
    [Header("ライト")]
    private Light[] DirectionalLight = new Light[3];
    private UserData[] userData_Group1;
    private UserData[] userData_Group2;
    private bool isGroup1Playing = true;
    private static readonly Rect[] SINGLE_CAMERA_RECT = new Rect[4]
    {
        new Rect(0f, 0f, 0.666f, 1f),
        new Rect(0.667f, 0.667f, 0.333f, 0.333f),
        new Rect(0.667f, 0.3335f, 0.333f, 0.333f),
        new Rect(0.667f, 0f, 0.333f, 0.333f)
    };
    private static readonly Rect[] SINGLE_PLAYERONLY_CAMERA_RECT = new Rect[4]
    {
        new Rect(0f, 0f, 1f, 1f),
        new Rect(1f, 1f, 1f, 1f),
        new Rect(1f, 1f, 1f, 1f),
        new Rect(1f, 1f, 1f, 1f)
    };
    private static readonly Rect[] TWO_PLAYER_CAMERA_RECT = new Rect[4]
    {
        new Rect(-0.5f, 0f, 1f, 1f),
        new Rect(0.5f, 0f, 0.5f, 1f),
        new Rect(1f, 1f, 1f, 1f),
        new Rect(1f, 1f, 1f, 1f)
    };
    private static readonly Rect[] FOUR_PLAYER_CAMERA_RECT = new Rect[4]
    {
        new Rect(0f, 0.5f, 0.5f, 0.5f),
        new Rect(0.5f, 0.5f, 0.5f, 0.5f),
        new Rect(0f, 0f, 0.5f, 0.5f),
        new Rect(0.5f, 0f, 0.5f, 0.5f)
    };
    public AlpineSkiing_Player[] Players => players;
    public UserData[] UserData_Group1 => userData_Group1;
    public void Init() {
        Init_UserData();
        switch (AlpineSkiing_Define.PLAYER_NUM) {
            case 1:
                for (int k = 0; k < cameras.Length; k++) {
                    cameras[k].rect = SINGLE_PLAYERONLY_CAMERA_RECT[k];
                }
                DirectionalLight[0].shadows = LightShadows.Hard;
                DirectionalLight[1].shadows = LightShadows.Hard;
                DirectionalLight[2].shadows = LightShadows.Hard;
                break;
            case 2:
                for (int j = 0; j < cameras.Length; j++) {
                    cameras[j].rect = TWO_PLAYER_CAMERA_RECT[j];
                }
                DirectionalLight[0].shadows = LightShadows.None;
                DirectionalLight[1].shadows = LightShadows.None;
                DirectionalLight[2].shadows = LightShadows.None;
                break;
            case 3:
                for (int l = 0; l < cameras.Length; l++) {
                    cameras[l].rect = FOUR_PLAYER_CAMERA_RECT[l];
                }
                DirectionalLight[0].shadows = LightShadows.None;
                DirectionalLight[1].shadows = LightShadows.None;
                DirectionalLight[2].shadows = LightShadows.None;
                break;
            case 4:
                for (int i = 0; i < cameras.Length; i++) {
                    cameras[i].rect = FOUR_PLAYER_CAMERA_RECT[i];
                }
                DirectionalLight[0].shadows = LightShadows.None;
                DirectionalLight[1].shadows = LightShadows.None;
                DirectionalLight[2].shadows = LightShadows.None;
                break;
        }
    }
    public void PlayerGameStart() {
        players[0].SkiBoard.StartMethod();
        players[1].SkiBoard.StartMethod();
        players[2].SkiBoard.StartMethod();
        players[3].SkiBoard.StartMethod();
    }
    public void CpuViewUpdate() {
        int pLAYER_NUM = AlpineSkiing_Define.PLAYER_NUM;
    }
    private void Init_UserData() {
        userData_Group1 = new UserData[AlpineSkiing_Define.MEMBER_NUM];
        for (int i = 0; i < userData_Group1.Length; i++) {
            userData_Group1[i].player = players[i];
            if (AlpineSkiing_Define.PLAYER_NUM == 3 && i == userData_Group1.Length - 1) {
                userData_Group1[i].userType = AlpineSkiing_Define.UserType.CPU_1;
            } else {
                userData_Group1[i].userType = (AlpineSkiing_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
            }
            userData_Group1[i].goalTime = 180f;
            userData_Group1[i].camera = cameras[i];
            userData_Group1[i].isPlayer = (userData_Group1[i].userType <= AlpineSkiing_Define.UserType.PLAYER_4);
            userData_Group1[i].player.Init(userData_Group1[i].userType);
        }
        AlpineSkiing_Define.UIM.SetUserUIData(userData_Group1);
    }
    public void UpdateMethod() {
        if (AlpineSkiing_Define.GM.IsDuringGame()) {
            for (int i = 0; i < userData_Group1.Length; i++) {
                userData_Group1[i].player.UpdateMethod();
            }
        }
    }
    private void Awake() {
    }
    public UserData GetUserData(AlpineSkiing_Define.UserType _userType) {
        for (int i = 0; i < userData_Group1.Length; i++) {
            if (userData_Group1[i].userType == _userType) {
                return userData_Group1[i];
            }
        }
        return default(UserData);
    }
    public int GetUserDataLength() {
        if (CheckNowGroup1Playing()) {
            return userData_Group1.Length;
        }
        return userData_Group2.Length;
    }
    public float[] GetAllUserRecordArray(bool _isGroup1 = true) {
        float[] array = new float[userData_Group1.Length];
        for (int i = 0; i < userData_Group1.Length; i++) {
            array[i] = userData_Group1[i].goalTime;
        }
        return array;
    }
    public int[] GetAllUserNoArray(bool _isGroup1 = true) {
        int[] array = new int[userData_Group1.Length];
        for (int i = 0; i < userData_Group1.Length; i++) {
            array[i] = (int)userData_Group1[i].userType;
        }
        return array;
    }
    public Camera GetUserCamera(AlpineSkiing_Define.UserType _userType) {
        if (CheckNowGroup1Playing()) {
            for (int i = 0; i < userData_Group1.Length; i++) {
                if (userData_Group1[i].userType == _userType) {
                    return userData_Group1[i].camera;
                }
            }
        } else {
            for (int j = 0; j < userData_Group2.Length; j++) {
                if (userData_Group2[j].userType == _userType) {
                    return userData_Group2[j].camera;
                }
            }
        }
        return null;
    }
    public bool CheckNowGroup1Playing() {
        return isGroup1Playing;
    }
    public void SetGoalTime(AlpineSkiing_Define.UserType _userType, float _setTime) {
        int num = 0;
        while (true) {
            if (num < userData_Group1.Length) {
                if (userData_Group1[num].userType == _userType) {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        userData_Group1[num].goalTime = _setTime;
        userData_Group1[num].goalTime = CalcManager.ConvertDecimalSecond(userData_Group1[num].goalTime);
    }
    public bool PlayerGoalCheck() {
        int num = 0;
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM; i++) {
            if (userData_Group1[i].player.SkiBoard.processType == AlpineSkiing_SkiBoard.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                num++;
            }
        }
        if (num == AlpineSkiing_Define.PLAYER_NUM) {
            return true;
        }
        return false;
    }
    public bool PlayerOnlyOneCheck() {
        int num = 0;
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM; i++) {
            if (userData_Group1[i].player.SkiBoard.processType == AlpineSkiing_SkiBoard.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                num++;
            }
        }
        if (num + 1 == AlpineSkiing_Define.PLAYER_NUM) {
            return true;
        }
        return false;
    }
    public void SetGroupVibration() {
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2 || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP) {
            return;
        }
        if (isGroup1Playing) {
            for (int i = 0; i < userData_Group1.Length; i++) {
                if (userData_Group1[i].userType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                    SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
                }
            }
            return;
        }
        for (int j = 0; j < userData_Group2.Length; j++) {
            if (userData_Group2[j].userType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
            }
        }
    }
    public void SetDebugRecord() {
        for (int i = 0; i < userData_Group1.Length; i++) {
            userData_Group1[i].goalTime = UnityEngine.Random.Range(60f, 100f);
            userData_Group1[i].goalTime = CalcManager.ConvertDecimalSecond(userData_Group1[i].goalTime);
        }
    }
}
