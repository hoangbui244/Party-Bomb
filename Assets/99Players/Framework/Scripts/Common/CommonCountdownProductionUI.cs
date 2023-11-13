using UnityEngine;
public class CommonCountdownProductionUI : SingletonCustom<CommonCountdownProductionUI> {
    [SerializeField]
    [Header("数字画像")]
    private GameObject[] numberSprite;
    [SerializeField]
    [Header("画像差分")]
    private Sprite[] arrayDiff;
    [SerializeField]
    [Header("画像差分（EN）")]
    private Sprite[] arrayDiff_EN;
    private int timeNow = -1;
    private void Start() {
        Init();
    }
    private void Init() {
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(value: false);
        }
    }
    public void UpdateCountdown(float _time) {
        if (_time > (float)numberSprite.Length) {
            return;
        }
        if (_time <= 0f) {
            for (int i = 0; i < numberSprite.Length; i++) {
                numberSprite[i].SetActive(value: false);
            }
        }
        int num = (int)(_time + 1f) - 1;
        if (timeNow != num) {
            timeNow = num;
            for (int j = 0; j < numberSprite.Length; j++) {
                numberSprite[j].SetActive(j == num);
            }
            switch (num) {
                case 0:
                    SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
                    break;
                case 1:
                    SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
                    break;
                case 2:
                    SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
                    break;
            }
        }
    }
    public void SetNumActive(bool _isActive) {
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(_isActive);
        }
    }
}
