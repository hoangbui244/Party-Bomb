using UnityEngine;
public class CommonAudienceCharacter : MonoBehaviour {
    [SerializeField]
    [Header("マテリアル変更対象のメッシュ（体）")]
    private MeshRenderer[] arrayBodyMesh;
    [SerializeField]
    [Header("マテリアル変更対象のメッシュ（顔）")]
    private MeshRenderer faceMesh;
    public void SetBodyMaterial(Material _bodyMat) {
        for (int i = 0; i < arrayBodyMesh.Length; i++) {
            arrayBodyMesh[i].material = _bodyMat;
        }
    }
    public void SetFaceMaterial(Material _faceMat) {
        faceMesh.material = _faceMat;
    }
}
