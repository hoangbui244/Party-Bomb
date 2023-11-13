using UnityEngine;
public class CommonGameTimeUI_Font_Time : MonoBehaviour {
    [SerializeField]
    [Header("TIMEの文字表示のUI")]
    private GameObject showTextUI;
    [SerializeField]
    [Header("TIMEの文字非表示のUI")]
    private GameObject hideTextUI;
    [SerializeField]
    [Header("TIMEの残り時間の文字表示のUI")]
    private GameObject remainingTimeShowTextUI;
    [SerializeField]
    [Header("TIMEの残り時間の文字非表示のUI")]
    private GameObject remainingTimeHideTextUI;
    [SerializeField]
    [Header("TIMEの残り時間(ミリ秒ver)の文字表示のUI")]
    private GameObject remainingDecimalTimeShowTextUI;
    [SerializeField]
    [Header("TIMEの残り時間(ミリ秒ver)の文字非表示のUI")]
    private GameObject remainingDecimalTimeHideTextUI;
    [SerializeField]
    [Header("TIME表示UIの[分]の数値")]
    private SpriteNumbers minutes_showTextUI;
    [SerializeField]
    [Header("TIME表示UIの[秒]の数値")]
    private SpriteNumbers second_showTextUI;
    [SerializeField]
    [Header("TIME表示UIの[ミリ秒]の数値")]
    private SpriteNumbers millSecond_showTextUI;
    [SerializeField]
    [Header("TIME非表示UIの[分]の数値")]
    private SpriteNumbers minutes_hideTextUI;
    [SerializeField]
    [Header("TIME非表示UIの[秒]の数値")]
    private SpriteNumbers second_hideTextUI;
    [SerializeField]
    [Header("TIME非表示UIの[ミリ秒]の数値")]
    private SpriteNumbers millSecond_hideTextUI;
    [SerializeField]
    [Header("残り時間文字表示UIの[分]の数値")]
    private SpriteNumbers minutes_remainingTime_ShowTextUI;
    [SerializeField]
    [Header("残り時間文字表示UIの[秒]の数値")]
    private SpriteNumbers second_remainingTime_ShowTextUI;
    [SerializeField]
    [Header("残り時間文字非表示UIの[分]の数値")]
    private SpriteNumbers minutes_remainingTime_HideTextUI;
    [SerializeField]
    [Header("残り時間文字非表示UIの[秒]の数値")]
    private SpriteNumbers second_remainingTime_HideTextUI;
    [SerializeField]
    [Header("残り時間文字表示UIの[秒]の数値")]
    private SpriteNumbers second_remainingDecimalTime_ShowTextUI;
    [SerializeField]
    [Header("残り時間文字表示UIの[ミリ秒]の数値")]
    private SpriteNumbers millSecond_remainingDecimalTime_ShowTextUI;
    [SerializeField]
    [Header("残り時間文字非表示UIの[秒]の数値")]
    private SpriteNumbers second_remainingDecimalTime_HideTextUI;
    [SerializeField]
    [Header("残り時間文字非表示UIの[ミリ秒]の数値")]
    private SpriteNumbers millSecond_remainingDecimalTime_HideTextUI;
    private float currentGameTime;
    private void Awake() {
        minutes_showTextUI.Set(0);
        second_showTextUI.Set(0);
        millSecond_showTextUI.Set(0);
        minutes_hideTextUI.Set(0);
        second_hideTextUI.Set(0);
        millSecond_hideTextUI.Set(0);
    }
    public void SetTime(float _time, int _no = 0) {
        currentGameTime = _time;
        string text = "";
        text = ((!(_time > CalcManager.ConvertRecordStringToTime("9:59.99"))) ? CalcManager.ConvertTimeToRecordString(_time, _no) : "9:59.99");
        string[] array = text.Split(':');
        string[] array2 = array[1].Split('.');
        string numbers = array[0];
        string numbers2 = array2[0];
        string numbers3 = array2[1];
        if (showTextUI.activeSelf) {
            minutes_showTextUI.SetNumbers(numbers);
            second_showTextUI.SetNumbers(numbers2);
            millSecond_showTextUI.SetNumbers(numbers3);
        }
        if (hideTextUI.activeSelf) {
            minutes_hideTextUI.SetNumbers(numbers);
            second_hideTextUI.SetNumbers(numbers2);
            millSecond_hideTextUI.SetNumbers(numbers3);
        }
        if (remainingTimeShowTextUI.activeSelf) {
            int num = Mathf.CeilToInt(_time) / 60;
            int num2 = Mathf.CeilToInt(_time) % 60;
            minutes_remainingTime_ShowTextUI.Set(num);
            second_remainingTime_ShowTextUI.Set(num2);
        }
        if (remainingTimeHideTextUI.activeSelf) {
            int num3 = Mathf.CeilToInt(_time) / 60;
            int num4 = Mathf.CeilToInt(_time) % 60;
            minutes_remainingTime_HideTextUI.Set(num3);
            second_remainingTime_HideTextUI.Set(num4);
        }
        if (remainingDecimalTimeShowTextUI.activeSelf) {
            second_remainingDecimalTime_ShowTextUI.SetNumbers(numbers2);
            millSecond_remainingDecimalTime_ShowTextUI.SetNumbers(numbers3);
        }
        if (remainingDecimalTimeHideTextUI.activeSelf) {
            second_remainingDecimalTime_HideTextUI.SetNumbers(numbers2);
            millSecond_remainingDecimalTime_HideTextUI.SetNumbers(numbers3);
        }
    }
    public void SetTimeAtString(string _time) {
        string[] array = _time.Split(':');
        string[] array2 = array[1].Split('.');
        string numbers = array[0];
        string numbers2 = array2[0];
        string numbers3 = array2[1];
        minutes_showTextUI.SetNumbers(numbers);
        second_showTextUI.SetNumbers(numbers2);
        millSecond_showTextUI.SetNumbers(numbers3);
    }
    public void ShowTimeRecord_ShowTextUI() {
        showTextUI.SetActive(value: true);
        hideTextUI.SetActive(value: false);
        remainingTimeShowTextUI.SetActive(value: false);
        remainingTimeHideTextUI.SetActive(value: false);
    }
    public void ShowTimeRecord_HideTextUI() {
        showTextUI.SetActive(value: false);
        hideTextUI.SetActive(value: true);
        remainingTimeShowTextUI.SetActive(value: false);
        remainingTimeHideTextUI.SetActive(value: false);
    }
    public void ShowRemainingTime_ShowTextUI() {
        showTextUI.SetActive(value: false);
        hideTextUI.SetActive(value: false);
        remainingTimeShowTextUI.SetActive(value: true);
        remainingTimeHideTextUI.SetActive(value: false);
    }
    public void ShowRemainingTime_HideTextUI() {
        showTextUI.SetActive(value: false);
        hideTextUI.SetActive(value: false);
        remainingTimeShowTextUI.SetActive(value: false);
        remainingTimeHideTextUI.SetActive(value: true);
    }
    public float GetCurrentGameTime() {
        return currentGameTime;
    }
}
