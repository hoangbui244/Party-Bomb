using TMPro;
using UnityEngine;
[ExecuteAlways]
public class OperationUIAddjust_Cross : MonoBehaviour {
    [SerializeField]
    [Header("操作説明の下敷き画像")]
    private SpriteRenderer operationUnderlay;
    [SerializeField]
    [Header("操作説明の文字")]
    private TextMeshPro operationText;
    [SerializeField]
    [Header("左右反転")]
    private bool flipX;
    [SerializeField]
    [Header("上下反転")]
    private bool flipY;
    [SerializeField]
    [Header("中心に表示される操作説明かどうか")]
    private bool isCenter;
    private const float DEF_UNDERLAY_WIDTH = 63.5f;
    private const float UNDERLAY_ADDJUST_OFFSET_W = 24.5f;
    private const float DEF_TEXT_WIDTH = 63.5f;
    private const float TEXT_ADDJUST_OFFSET_W = 24.5f;
    private const float DEF_OPERATION_POS_X = 85.5f;
    private const float OPERATION_ADDJUST_POS_X = 12.5f;
    private const float DEF_OPERATION_POS_Y = 71.5f;
    private int textNum;
    private void Update() {
        if (operationText != null && operationUnderlay != null) {
            if (operationUnderlay.drawMode != SpriteDrawMode.Sliced) {
                operationUnderlay.drawMode = SpriteDrawMode.Sliced;
            }
            operationUnderlay.transform.localScale = Vector2.one;
            textNum = operationText.text.Length;
            if (textNum == 0) {
                operationUnderlay.size = new Vector2(63.5f, 37f);
                operationUnderlay.transform.localPosition = new Vector3((!isCenter) ? (85.5f * (float)((!flipX) ? 1 : (-1))) : 0f, (!isCenter) ? 0f : (71.5f * (float)((!flipY) ? 1 : (-1))), 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f, 37f);
                operationText.transform.localPosition = new Vector3((!isCenter) ? (85.5f * (float)((!flipX) ? 1 : (-1))) : 0f, (!isCenter) ? 0f : (71.5f * (float)((!flipY) ? 1 : (-1))), 0f);
            } else {
                operationUnderlay.size = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 37f);
                operationUnderlay.transform.localPosition = new Vector3((!isCenter) ? ((85.5f + 12.5f * (float)(textNum - 1)) * (float)((!flipX) ? 1 : (-1))) : 0f, (!isCenter) ? 0f : (71.5f * (float)((!flipY) ? 1 : (-1))), 0f);
                operationText.rectTransform.sizeDelta = new Vector2(63.5f + 24.5f * (float)(textNum - 1), 37f);
                operationText.transform.localPosition = new Vector3((!isCenter) ? ((85.5f + 12.5f * (float)(textNum - 1)) * (float)((!flipX) ? 1 : (-1))) : 0f, (!isCenter) ? 0f : (71.5f * (float)((!flipY) ? 1 : (-1))), 0f);
            }
        }
    }
}
