using System;
using UnityEngine;
[Serializable]
public class TrophyData {
    public enum Type {
        BRONZE,
        SILVER,
        GOLD,
        MAX
    }
    public bool[] openBronze;
    public bool[] openSilver;
    public bool[] openGold;
    public static readonly int BASE_ACHIEVE_NUM = 14;
    public bool openDLC1Complete;
    private bool tempCheck;
    public TrophyData() {
        openDLC1Complete = false;
        openBronze = new bool[BASE_ACHIEVE_NUM];
        for (int i = 0; i < openBronze.Length; i++) {
            openBronze[i] = false;
        }
        openSilver = new bool[BASE_ACHIEVE_NUM];
        for (int j = 0; j < openBronze.Length; j++) {
            openSilver[j] = false;
        }
        openGold = new bool[BASE_ACHIEVE_NUM];
        for (int k = 0; k < openBronze.Length; k++) {
            openGold[k] = false;
        }
    }
    public bool SetOpen(Type _type, GS_Define.GameType _gameType) {
        UnityEngine.Debug.Log("type:" + _type.ToString() + " gType:" + _gameType.ToString());
        return false;
    }
    public bool GetOpen(Type _type, int _gameTypeIdx) {
        switch (_type) {
            case Type.BRONZE:
                return openBronze[_gameTypeIdx];
            case Type.SILVER:
                return openSilver[_gameTypeIdx];
            case Type.GOLD:
                return openGold[_gameTypeIdx];
            default:
                return false;
        }
    }
    public bool GetOpenDLC1Complete() {
        return openDLC1Complete;
    }
    public static int ConversionGameTypeNo(int _currentNo) {
        switch (_currentNo) {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 3;
            case 4:
                return 4;
            case 5:
                return 5;
            case 6:
                return 6;
            case 7:
                return 7;
            case 8:
                return 8;
            case 9:
                return 9;
            default:
                return 0;
        }
    }
    public void IsTotalOpen_DLC1() {
        UnityEngine.Debug.Log("IsTotalOpen_DLC1 ");
        tempCheck = true;
        if (!openBronze[2] || !openSilver[2] || !openGold[2]) {
            tempCheck = false;
        }
        UnityEngine.Debug.Log("openBronze[i] " + openBronze[2].ToString());
        UnityEngine.Debug.Log("openSilver[i] " + openSilver[2].ToString());
        UnityEngine.Debug.Log("openGold[i] " + openGold[2].ToString());
        if (!openBronze[2] || !openSilver[2] || !openGold[2]) {
            tempCheck = false;
        }
        UnityEngine.Debug.Log("openBronze[i] " + openBronze[2].ToString());
        UnityEngine.Debug.Log("openSilver[i] " + openSilver[2].ToString());
        UnityEngine.Debug.Log("openGold[i] " + openGold[2].ToString());
        UnityEngine.Debug.Log("tempCheck " + tempCheck.ToString());
        if (tempCheck) {
            openDLC1Complete = true;
        }
    }
}
