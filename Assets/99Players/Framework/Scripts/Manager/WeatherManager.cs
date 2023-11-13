using System;
using System.Collections.Generic;
using UnityEngine;
public class WeatherManager : MonoBehaviour {
    [Serializable]
    public class WeatherData {
        [Header("設定名")]
        public string name;
        [Header("スカイボックス")]
        [Space(10f)]
        public Material skybox;
        [Header("Fogを使うかどうか")]
        [Space(10f)]
        public bool useFog;
        [Header("Fogの色")]
        public Color fogColor = Color.white;
        [Header("Fogモ\u30fcド")]
        public FogMode fogMode;
        [Header("Fogモ\u30fcド Linear の設定")]
        public float fogStartDistance;
        public float fogEndDistance;
        [Header("Fogモ\u30fcド Exponential 系の設定")]
        public float fogDensity;
        [Header("アクティブにするオブジェクト")]
        [Space(10f)]
        public GameObject[] activeObjects;
        public WeatherData() {
            fogColor = Color.red;
        }
    }
    public enum ViewType {
        Random,
        Order,
        Fix
    }
    private static int PrevAwakeWeatherIndex = -1;
    [SerializeField]
    [Header("通しモ\u30fcドでは一番上の設定が反映されます")]
    private List<WeatherData> weatherDataList;
    [HideInInspector]
    public ViewType viewType;
    [HideInInspector]
    public int fixNo;
    [HideInInspector]
    public int viewSettingNo;
    private int nowWeatherIndex = -1;
    private void Awake() {
        if (weatherDataList == null || weatherDataList.Count == 0) {
            return;
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
            SetWeather(0);
            return;
        }
        switch (viewType) {
            case ViewType.Random:
                SetWeather(UnityEngine.Random.Range(0, weatherDataList.Count));
                break;
            case ViewType.Order:
                if (PrevAwakeWeatherIndex == -1) {
                    PrevAwakeWeatherIndex = UnityEngine.Random.Range(0, weatherDataList.Count);
                } else {
                    PrevAwakeWeatherIndex++;
                    if (PrevAwakeWeatherIndex >= weatherDataList.Count) {
                        PrevAwakeWeatherIndex = 0;
                    }
                }
                SetWeather(PrevAwakeWeatherIndex);
                break;
            case ViewType.Fix:
                SetWeather(fixNo);
                break;
        }
    }
    private void Start() {
        if (nowWeatherIndex != -1) {
            LightingSettings.SetSkybox(GetWeatherData(nowWeatherIndex).skybox);
        }
    }
    public void SetWeather(int _idx) {
        if (nowWeatherIndex >= 0) {
            AllDisableWeatherObjects();
        }
        if (nowWeatherIndex == _idx) {
            return;
        }
        nowWeatherIndex = _idx;
        WeatherData weatherData = GetWeatherData(_idx);
        LightingSettings.SetSkybox(weatherData.skybox);
        RenderSettings.fog = weatherData.useFog;
        if (weatherData.useFog) {
            RenderSettings.fogColor = weatherData.fogColor;
            RenderSettings.fogMode = weatherData.fogMode;
            switch (weatherData.fogMode) {
                case FogMode.Linear:
                    RenderSettings.fogStartDistance = weatherData.fogStartDistance;
                    RenderSettings.fogStartDistance = weatherData.fogEndDistance;
                    break;
                case FogMode.Exponential:
                case FogMode.ExponentialSquared:
                    RenderSettings.fogDensity = weatherData.fogDensity;
                    break;
            }
        }
        for (int i = 0; i < weatherData.activeObjects.Length; i++) {
            weatherData.activeObjects[i].SetActive(value: true);
        }
    }
    public WeatherData GetWeatherData(int _idx) {
        return weatherDataList[_idx];
    }
    public bool CheckWeather(int _idx) {
        return nowWeatherIndex == _idx;
    }
    private void AllDisableWeatherObjects() {
        for (int i = 0; i < weatherDataList.Count; i++) {
            for (int j = 0; j < weatherDataList[i].activeObjects.Length; j++) {
                weatherDataList[i].activeObjects[j].SetActive(value: false);
            }
        }
    }
    public string[] GetDataNames() {
        if (weatherDataList == null) {
            return new string[0];
        }
        string[] array = new string[weatherDataList.Count];
        for (int i = 0; i < array.Length; i++) {
            array[i] = weatherDataList[i].name;
        }
        return array;
    }
}
