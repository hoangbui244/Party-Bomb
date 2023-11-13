using UnityEngine;
public class AudienceFaceAddOnSettings : MonoBehaviour {
    [SerializeField]
    [Header("顔のMeshRenderer")]
    private MeshRenderer mrFace;
    [SerializeField]
    [Header("表情変更用テクスチャ(男性)")]
    private Texture[] texBoyFaceNormal;
    [SerializeField]
    [Header("表情変更用テクスチャ(女性)")]
    private Texture[] texGirlFaceNormal;
    [SerializeField]
    [Header("性別フラグ(True:男性、False:女性)")]
    private bool isMan;
    private void Start() {
        if (isMan) {
            if (texBoyFaceNormal != null) {
                int num = UnityEngine.Random.Range(0, texBoyFaceNormal.Length);
                mrFace.material.SetTexture("_FaceTex", texBoyFaceNormal[num]);
            }
        } else if (texGirlFaceNormal != null) {
            int num2 = UnityEngine.Random.Range(0, texGirlFaceNormal.Length);
            mrFace.material.SetTexture("_FaceTex", texGirlFaceNormal[num2]);
        }
    }
}
