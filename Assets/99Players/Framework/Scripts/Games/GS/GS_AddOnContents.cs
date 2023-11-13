using System;
using UnityEngine;
public class GS_AddOnContents : MonoBehaviour {
    [SerializeField]
    [Header("表示ル\u30fcト")]
    private GameObject frame;
    [SerializeField]
    [Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursor;
    [SerializeField]
    [Header("DLC1購入済マ\u30fcク")]
    private GameObject objPurchasedDLC1;
    [SerializeField]
    [Header("DLC2購入済マ\u30fcク")]
    private GameObject objPurchasedDLC2;
    [SerializeField]
    [Header("DLC3購入済マ\u30fcク")]
    private GameObject objPurchasedDLC3;
    [SerializeField]
    [Header("DLC1詳細ダイアログ")]
    private GS_DLC_Detail dlc1DetailDialog;
    [SerializeField]
    [Header("DLC2詳細ダイアログ")]
    private GS_DLC_Detail dlc2DetailDialog;
    [SerializeField]
    [Header("DLC3詳細ダイアログ")]
    private GS_DLC_Detail dlc3DetailDialog;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示フレ\u30fcム")]
    private GameObject objControllerFrame;
    private Action callBack;
    public bool IsOpen => frame.activeSelf;
    public void Show(Action _callBack = null) {
        callBack = _callBack;
        frame.SetActive(value: true);
        objPurchasedDLC1.SetActive(SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX));
        objPurchasedDLC2.SetActive(SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX));
        objPurchasedDLC3.SetActive(SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX));
        cursor.IsStop = false;
        cursor.SetCursorPos(0, 0);
        LeanTween.cancel(objControllerFrame);
        objControllerFrame.transform.SetLocalPositionX(1145f);
        LeanTween.moveLocalX(objControllerFrame, 618f, 0.55f).setEaseOutQuint();
    }
    public void Hide() {
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
            if (callBack != null) {
                callBack();
                callBack = null;
            }
            frame.SetActive(value: false);
        });
    }
    public void DirectClose() {
        if (callBack != null) {
            callBack();
            callBack = null;
        }
        if (dlc1DetailDialog.IsOpen) {
            dlc1DetailDialog.DirectClose();
        }
        if (dlc2DetailDialog.IsOpen) {
            dlc2DetailDialog.DirectClose();
        }
        if (dlc3DetailDialog.IsOpen) {
            dlc3DetailDialog.DirectClose();
        }
        frame.SetActive(value: false);
    }
    private void Update() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursor.IsOkButton()) {
            OnSelectButtonDown();
        }
        else if (cursor.IsReturnButton()) {
            OnReturnButtonDown();
        }
    }
    private bool IsButtonInteractable() {
        if (dlc1DetailDialog.IsOpen) {
            return false;
        }
        if (dlc2DetailDialog.IsOpen) {
            return false;
        }
        if (dlc3DetailDialog.IsOpen) {
            return false;
        }
        if (SingletonCustom<SceneManager>.Instance.IsFade) {
            return false;
        }
        if (!frame.activeSelf) {
            return false;
        }
        return true;
    }
    public void OnSelectButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        OnSelectButtonDownVariant(cursor.GetSelectNo());
    }
    public void OnSelectButtonDownVariant(int selectId) {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        switch (selectId) {
            case 0:
                dlc1DetailDialog.Open(false, delegate { cursor.IsStop = false; });
                cursor.IsStop = true;
                break;
            case 1:
                dlc2DetailDialog.Open(false, delegate { cursor.IsStop = false; });
                cursor.IsStop = true;
                break;
            case 2:
                dlc3DetailDialog.Open(false, delegate { cursor.IsStop = false; });
                cursor.IsStop = true;
                break;
        }
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        Hide();
    }
    private void OnDestroy() {
        LeanTween.cancel(objControllerFrame);
    }
}