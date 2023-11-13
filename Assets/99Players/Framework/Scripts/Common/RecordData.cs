using System;
[Serializable]
public class RecordData {
    public enum InitType {
        Score,
        Time
    }
    private readonly int RECORD_NUM = (int)GS_Define.GameType.MAX;
    public string[] recordData;
    public string[] recordDataSix;
    public RecordData() {
        recordData = new string[RECORD_NUM];
        recordDataSix = new string[RECORD_NUM];
    }
    public void Set(GS_Define.GameType _type, string _data, int _idx = 0) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        recordData[(int)_type] = _data;
    }
    public void SetSix(GS_Define.GameType _type, string _data) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        recordDataSix[(int)_type] = _data;
    }
    public bool IsDataEmpty(GS_Define.GameType _type) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        return string.IsNullOrEmpty(recordData[(int)_type]);
    }
    public bool IsDataEmptySix(GS_Define.GameType _type) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        return string.IsNullOrEmpty(recordDataSix[(int)_type]);
    }
    public string Get(GS_Define.GameType _type, InitType _initType, int _idx = 0) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        if (string.IsNullOrEmpty(recordData[(int)_type])) {
            switch (_initType) {
                case InitType.Score:
                    recordData[(int)_type] = "0";
                    break;
                case InitType.Time:
                    recordData[(int)_type] = "9:59.99";
                    break;
            }
        }
        return recordData[(int)_type];
    }
    public string GetSix(GS_Define.GameType _type, InitType _initType, int _idx = 0) {
        if (recordData.Length <= (int)_type) {
            FixArraySize();
        }
        if (string.IsNullOrEmpty(recordDataSix[(int)_type])) {
            switch (_initType) {
                case InitType.Score:
                    recordDataSix[(int)_type] = "0";
                    break;
                case InitType.Time:
                    recordDataSix[(int)_type] = "9:59.99";
                    break;
            }
        }
        return recordDataSix[(int)_type];
    }
    private void FixArraySize() {
        string[] array = recordData;
        string[] array2 = recordDataSix;
        recordData = new string[RECORD_NUM];
        recordDataSix = new string[RECORD_NUM];
        for (int i = 0; i < array.Length; i++) {
            recordData[i] = array[i];
            recordDataSix[i] = array2[i];
        }
    }
}
