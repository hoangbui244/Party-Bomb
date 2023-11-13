using TMPro;
using UnityEngine;
[ExecuteAlways]
public class OperationUIAddjust_Single : MonoBehaviour {
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
    private const float DEF_OPERATION_POS_X = 56f;
    private const float OPERATION_ADDJUST_POS_X = 12.5f;
    private int textNum;
    private const float DEF_BUTTON_SCALE = 0.6f;
    private const float BUTTON_ADDJUST_POS_X = -51f;
    private void Update() {
        if (operationText != null && operationUnderlay != null) {
            if (operationUnderlay.drawMode != SpriteDrawMode.Sliced) {
                operationUnderlay.drawMode = SpriteDrawMode.Sliced;
            }
            operationUnderlay.transform.localScale = Vector3.one;
            operationText.transform.localScale = Vector3.one;
            textNum = operationText.text.Length;
            if (textNum == 0) {
                operationUnderlay.size = new Vector2(63.5f, 37f);
                operationUnderlay.transform.localPosition = new Vector3(56f * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f, 37f);
                operationText.transform.localPosition = new Vector3(56f * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
            } else {
                operationUnderlay.size = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 37f);
                operationUnderlay.transform.localPosition = new Vector3((56f + 12.5f * (float)(textNum - 1)) * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 37f);
                operationText.transform.localPosition = new Vector3((56f + 12.5f * (float)(textNum - 1)) * (float)((!flipX) ? 1 : (-1)), 0f, 0f);
            }
        }
        for (int i = 0; i < operationButton.Length; i++) {
            if (operationButton[i] != null) {
                operationButton[i].transform.localScale = 0.6f * Vector3.one;
                operationButton[i].transform.localPosition = new Vector3(-51f * (float)i, 0f, 0f);
            }
        }
    }
}
