using UnityEngine;
public class CreateSkinedMeshCollider : MonoBehaviour {
    [SerializeField]
    [Header("更新インタ\u30fcバル")]
    private float interval;
    private MeshCollider meshCollider;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private float nextTime;
    private void Start() {
        meshCollider = GetComponent<MeshCollider>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        nextTime = 0f;
    }
    private void Update() {
        nextTime -= Time.deltaTime;
        if (nextTime < 0f) {
            nextTime = interval;
            Mesh mesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(mesh);
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }
}
