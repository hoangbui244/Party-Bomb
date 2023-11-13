using UnityEngine;
public class SimpleEffect : MonoBehaviourExtension {
    [SerializeField]
    [Header("削除までの時間")]
    private float lifeTime;
    private void Start() {
        WaitAfterExec(lifeTime, delegate {
            UnityEngine.Object.Destroy(base.gameObject);
        });
    }
}
