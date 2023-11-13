using UnityEngine;
public class JoyConButton : MonoBehaviour {
    public enum PlayerType {
        PLAYER_1,
        PLAYER_2,
        PLAYER_3,
        PLAYER_4
    }
    public enum ButtonType {
        X,
        Y,
        B,
        A,
        OTHER
    }
    [SerializeField]
    [Header("操作説明に対応するプレイヤ\u30fcの種類")]
    private PlayerType playerType;
    [SerializeField]
    [Header("操作説明に表示するボタン（X Y B A の時は指定、それ以外は OTHER）")]
    private ButtonType buttonType = ButtonType.OTHER;
    [SerializeField]
    [Header("操作説明の親オブジェクト")]
    private GameObject controlRootObj;
    [Header("--------SpriteRendererで描画する場合--------------------------")]
    [SerializeField]
    [Header("ボタン描画_SpriteRenderer")]
    private SpriteRenderer btnRenderer_Sprite;
    [SerializeField]
    [Header("シングルモ\u30fcド画像")]
    private Sprite singleSp_Sprite;
    [SerializeField]
    [Header("マルチモ\u30fcド画像")]
    private Sprite multiSp_Sprite;
    [Header("--------オブジェクトで描画する場合------------------------------")]
    [SerializeField]
    [Header("シングルモ\u30fcドオブジェクト")]
    private GameObject singleButtonObj;
    [SerializeField]
    [Header("マルチモ\u30fcドオブジェクト")]
    private GameObject multiButtonObj;
    private int npadId;
    private void Awake() {
        if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[(int)playerType][0] != (int)playerType && controlRootObj != null) {
            controlRootObj.SetActive(value: false);
        }
        CheckJoyconButton();
        SingletonCustom<JoyConManager>.Instance.OnChangeConnect.AddListener(delegate {
            CheckJoyconButton();
        });
    }
    public void CheckJoyconButton() {
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            npadId = 0;
        } else if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[(int)playerType][0] == (int)playerType) {
            if ((PlayerType)6 <= playerType) {
                return;
            }
            npadId = (int)playerType;
        }
        if (btnRenderer_Sprite != null) {
            if (SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId)) {
                if (singleSp_Sprite != null) {
                    btnRenderer_Sprite.sprite = singleSp_Sprite;
                    btnRenderer_Sprite.transform.SetLocalEulerAnglesZ(0f);
                }
            } else if (multiSp_Sprite != null) {
                btnRenderer_Sprite.sprite = multiSp_Sprite;
                ButtonType buttonType = this.buttonType;
                if ((uint)buttonType <= 3u) {
                    btnRenderer_Sprite.transform.SetLocalEulerAnglesZ(90f * (float)this.buttonType);
                } else {
                    btnRenderer_Sprite.transform.SetLocalEulerAnglesZ(0f);
                }
            }
            return;
        }
        if (singleButtonObj != null) {
            singleButtonObj.SetActive(value: false);
        }
        if (multiButtonObj != null) {
            multiButtonObj.SetActive(value: false);
        }
        if (SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId)) {
            if (singleButtonObj != null) {
                singleButtonObj.SetActive(value: true);
            }
        } else if (multiButtonObj != null) {
            multiButtonObj.SetActive(value: true);
        }
    }
    public void SetPlayerType(PlayerType _playerType) {
        playerType = _playerType;
    }
}
