using UnityEngine;
public class MenuBackScroll : MonoBehaviour {
    private MeshRenderer meshRenderer;
    [SerializeField]
    [Header("テクスチャ")]
    private Texture[] arrayTex;
    [SerializeField]
    [Header("スクロ\u30fcルx")]
    private float scrollX;
    [SerializeField]
    [Header("スクロ\u30fcルy")]
    private float scrollY;
    [SerializeField]
    [Header("自動設定のオフ")]
    private bool isAutoSettingDisable;
    private Vector2 calcUV;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        calcUV.x = SingletonCustom<GameSettingManager>.Instance.GlobalScrollX;
        calcUV.y = SingletonCustom<GameSettingManager>.Instance.GlobalScrollY;
        meshRenderer.material.SetTextureOffset("_MainTex", calcUV);
    }
    private void OnEnable() {
        if (!isAutoSettingDisable) {
            SetQuad();
        }
    }
    public void DirectSetQuad(int _idx) {
        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        UnityEngine.Debug.Log("setId:" + _idx.ToString());
        meshRenderer.material.SetTexture("_MainTex", arrayTex[_idx]);
        calcUV.x = SingletonCustom<GameSettingManager>.Instance.GlobalScrollX;
        calcUV.y = SingletonCustom<GameSettingManager>.Instance.GlobalScrollY;
        meshRenderer.material.SetTextureOffset("_MainTex", calcUV);
    }
    public void SetQuad() {
        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        UnityEngine.Debug.Log("setId:" + SingletonCustom<GameSettingManager>.Instance.BackFrameTexIdx.ToString());
        if (arrayTex.Length <= SingletonCustom<GameSettingManager>.Instance.BackFrameTexIdx) {
            meshRenderer.material.SetTexture("_MainTex", arrayTex[0]);
        } else {
            meshRenderer.material.SetTexture("_MainTex", arrayTex[SingletonCustom<GameSettingManager>.Instance.BackFrameTexIdx]);
        }
        calcUV.x = SingletonCustom<GameSettingManager>.Instance.GlobalScrollX;
        calcUV.y = SingletonCustom<GameSettingManager>.Instance.GlobalScrollY;
        meshRenderer.material.SetTextureOffset("_MainTex", calcUV);
    }
    private void Update() {
        calcUV.x = SingletonCustom<GameSettingManager>.Instance.GlobalScrollX;
        calcUV.y = SingletonCustom<GameSettingManager>.Instance.GlobalScrollY;
        meshRenderer.material.SetTextureOffset("_MainTex", calcUV);
    }
}
