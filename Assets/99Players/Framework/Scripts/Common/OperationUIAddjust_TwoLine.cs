using System;
using TMPro;
using UnityEngine;
[ExecuteInEditMode]
public class OperationUIAddjust_TwoLine : MonoBehaviour {
    [SerializeField]
    [Header("操作説明の下敷き画像")]
    private SpriteRenderer operationUnderlay;
    [SerializeField]
    [Header("操作説明の文字")]
    private TextMeshPro operationText;
    [SerializeField]
    [Header("操作説明のボタン")]
    private SpriteRenderer[] operationButton;
    [SerializeField]
    [Header("左右反転")]
    private bool flipX;
    private const float DEF_UNDERLAY_WIDTH = 63.5f;
    private const float UNDERLAY_ADDJUST_OFFSET_W = 24.5f;
    private const float DEF_TEXT_WIDTH = 63.5f;
    private const float TEXT_ADDJUST_OFFSET_W = 24.5f;
    private const float DEF_TEXT_POS_X = 43.5f;
    private const float DEF_OPERATION_POS_X = 56f;
    private const float OPERATION_ADDJUST_POS_X = 12.5f;
    private int textNum;
    private const float DEF_BUTTON_SCALE = 0.6f;
    private const float BUTTON_ADDJUST_POS_X = -51f;
    private string[] charcters;
    private void Update() {
        if (operationText != null && operationUnderlay != null) {
            if (operationUnderlay.drawMode != SpriteDrawMode.Sliced) {
                operationUnderlay.drawMode = SpriteDrawMode.Sliced;
            }
            operationText.rectTransform.pivot = (flipX ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f));
            operationText.alignment = (flipX ? TextAlignmentOptions.Right : TextAlignmentOptions.Left);
            operationUnderlay.transform.localScale = Vector3.one;
            operationText.transform.localScale = Vector3.one;
            charcters = operationText.text.Split(Environment.NewLine.ToCharArray());
            textNum = 0;
            for (int i = 0; i < charcters.Length; i++) {
                textNum = Mathf.Max(textNum, charcters[i].Length);
            }
            if (textNum == 0) {
                operationUnderlay.size = new Vector2(63.5f, 67f);
                operationUnderlay.transform.localPosition = new Vector3(56f * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f, 67f);
                operationText.transform.localPosition = new Vector3(43.5f * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
            } else {
                operationUnderlay.size = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 67f);
                operationUnderlay.transform.localPosition = new Vector3((56f + 12.5f * (float)(textNum - 1)) * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 67f);
                operationText.transform.localPosition = new Vector3(43.5f * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
            }
        }
        for (int j = 0; j < operationButton.Length; j++) {
            if (operationButton[j] != null) {
                operationButton[j].transform.localScale = 0.6f * Vector3.one;
                operationButton[j].transform.localPosition = new Vector3(-51f * (float)j, 0f, 0f);
            }
        }
    }
}
