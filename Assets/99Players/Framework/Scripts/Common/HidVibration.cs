using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class HidVibration : SingletonCustom<HidVibration> {
    public enum VibrationType {
        None,
        Common,
        Weak,
        Normal,
        Strong
    }
    private struct VibrationDevice {
        public int vibrationDeviceCount;
        public VibrationType vibrationType;
        public float vibrationTime;
        public float lowFrequency;
        public float highFrequency;
    }
    private const int vibrationDeviceCountMax = 2;
    private const int MAX_PLAYER_COUNT = 6;
    private VibrationDevice[] arrayVibrationDevice = new VibrationDevice[6];
    private float[] arrayVibrationTime = new float[6];
    public bool IsEnable {
        get;
        set;
    }
    public void RefreshVibrationDevice(int playerIndex) {
        IsEnable = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration;
        InitValue(playerIndex);
    }
    private void InitValue(int playerIndex) {
        arrayVibrationDevice[playerIndex].vibrationDeviceCount = 0;
        arrayVibrationDevice[playerIndex].vibrationType = VibrationType.None;
        arrayVibrationDevice[playerIndex].lowFrequency = 0f;
        arrayVibrationDevice[playerIndex].highFrequency = 0f;
    }
    private void Start() {
        for (int i = 0; i < 6; i++) {
            InitValue(i);
        }
        IsEnable = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isVibration;
    }
    private void Update() {
        if (!IsEnable) {
            return;
        }
        Gamepad[] array = Gamepad.all.ToArray();
        for (int i = 0; i < arrayVibrationDevice.Length; i++) {
            if (arrayVibrationDevice[i].vibrationType != 0) {
                Vibration(i);
                if (array != null && i < array.Length) {
                    array[i].SetMotorSpeeds(arrayVibrationDevice[i].lowFrequency, arrayVibrationDevice[i].highFrequency);
                }
            }
        }
    }
    private void Vibration(int playerIndex) {
        arrayVibrationDevice[playerIndex].vibrationTime += Time.unscaledDeltaTime;
        if (arrayVibrationDevice[playerIndex].vibrationTime < arrayVibrationTime[playerIndex]) {
            switch (arrayVibrationDevice[playerIndex].vibrationType) {
                case VibrationType.Common:
                    arrayVibrationDevice[playerIndex].lowFrequency = UnityEngine.Random.Range(0f, 0.05f);
                    arrayVibrationDevice[playerIndex].highFrequency = UnityEngine.Random.Range(0f, 0.025f);
                    break;
                case VibrationType.Weak:
                    arrayVibrationDevice[playerIndex].lowFrequency = UnityEngine.Random.Range(0f, 0.025f);
                    arrayVibrationDevice[playerIndex].highFrequency = UnityEngine.Random.Range(0f, 0.05f);
                    break;
                case VibrationType.Normal:
                    arrayVibrationDevice[playerIndex].lowFrequency = UnityEngine.Random.Range(0.05f, 0.1f);
                    arrayVibrationDevice[playerIndex].highFrequency = UnityEngine.Random.Range(0.1f, 0.15f);
                    break;
                case VibrationType.Strong:
                    arrayVibrationDevice[playerIndex].lowFrequency = UnityEngine.Random.Range(0f, 0.2f);
                    arrayVibrationDevice[playerIndex].highFrequency = UnityEngine.Random.Range(0f, 0.25f);
                    break;
            }
        } else {
            arrayVibrationDevice[playerIndex].vibrationType = VibrationType.None;
            arrayVibrationDevice[playerIndex].lowFrequency = 0f;
            arrayVibrationDevice[playerIndex].highFrequency = 0f;
        }
    }
    private void SetPlayerIndex(ref int _playerIdx) {
    }
    public void SetCommonVibration(int _playerIdx) {
        SetPlayerIndex(ref _playerIdx);
        SetVibration(_playerIdx, VibrationType.Common);
        arrayVibrationTime[_playerIdx] = 0.25f;
    }
    public void SetCustomVibration(int _playerIdx, VibrationType _vibrationType = VibrationType.Common, float _vibrationTime = 0.3f) {
        UnityEngine.Debug.Log("--- [Vib]CustomSet ---" + Environment.NewLine + "[PlayerId] -> " + _playerIdx.ToString() + Environment.NewLine + "[VibType] -> " + _vibrationType.ToString() + Environment.NewLine + "[VibTime] -> " + _vibrationTime.ToString());
        SetPlayerIndex(ref _playerIdx);
        SetVibration(_playerIdx, _vibrationType);
        arrayVibrationTime[_playerIdx] = _vibrationTime;
    }
    private void SetVibration(int _playerIdx, VibrationType _vibrationType) {
        arrayVibrationDevice[_playerIdx].vibrationTime = 0f;
        arrayVibrationDevice[_playerIdx].vibrationType = _vibrationType;
    }
    public void StopVibration(int _playerIdx) {
        arrayVibrationDevice[_playerIdx].vibrationTime = 0f;
        arrayVibrationDevice[_playerIdx].vibrationType = VibrationType.None;
    }
}
