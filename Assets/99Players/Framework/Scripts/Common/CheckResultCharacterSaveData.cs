using System;
using UnityEngine;
[Serializable]
public class CheckResultCharacterSaveData {
    public enum LayoutType {
        Single,
        Two,
        Four
    }
    public string[][] singleLayoutData;
    public string[][] twoLayoutData;
    public string[][] fourLayoutData;
    public CheckResultCharacterSaveData() {
        singleLayoutData = new string[8][];
        for (int i = 0; i < singleLayoutData.Length; i++) {
            singleLayoutData[i] = new string[5];
            for (int j = 0; j < singleLayoutData[i].Length; j++) {
                singleLayoutData[i][j] = "";
            }
        }
        twoLayoutData = new string[8][];
        for (int k = 0; k < twoLayoutData.Length; k++) {
            twoLayoutData[k] = new string[5];
            for (int l = 0; l < twoLayoutData[k].Length; l++) {
                twoLayoutData[k][l] = "";
            }
        }
        fourLayoutData = new string[8][];
        for (int m = 0; m < fourLayoutData.Length; m++) {
            fourLayoutData[m] = new string[5];
            for (int n = 0; n < fourLayoutData[m].Length; n++) {
                fourLayoutData[m][n] = "";
            }
        }
    }
    public void SaveData(LayoutType _layOutType, int _charaNo, int _faceNo, Vector3 _pos) {
        switch (_layOutType) {
            case LayoutType.Single:
                singleLayoutData[_charaNo][_faceNo] = _pos.x.ToString() + "_" + _pos.y.ToString() + "_" + _pos.z.ToString();
                break;
            case LayoutType.Two:
                twoLayoutData[_charaNo][_faceNo] = _pos.x.ToString() + "_" + _pos.y.ToString() + "_" + _pos.z.ToString();
                break;
            case LayoutType.Four:
                fourLayoutData[_charaNo][_faceNo] = _pos.x.ToString() + "_" + _pos.y.ToString() + "_" + _pos.z.ToString();
                break;
        }
    }
    public Vector3 GetData(LayoutType _layOutType, int _charaNo, int _faceNo) {
        string[] array;
        switch (_layOutType) {
            case LayoutType.Single:
                array = singleLayoutData[_charaNo][_faceNo].Split('_');
                break;
            case LayoutType.Two:
                array = twoLayoutData[_charaNo][_faceNo].Split('_');
                break;
            default:
                array = fourLayoutData[_charaNo][_faceNo].Split('_');
                break;
        }
        if (array.Length == 1) {
            return Vector3.one;
        }
        UnityEngine.Debug.Log("Pos(" + array[0] + ", " + array[1] + ", " + array[2] + ")");
        return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
    }
}
