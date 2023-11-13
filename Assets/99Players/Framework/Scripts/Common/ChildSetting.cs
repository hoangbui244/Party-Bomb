using UnityEngine;
public class ChildSetting : MonoBehaviour {
    [SerializeField]
    [Header("子設定の対象")]
    private Transform[] arrayChildTarget;
    private void Awake() {
        for (int i = 0; i < arrayChildTarget.Length; i++) {
            arrayChildTarget[i].parent = base.transform;
            arrayChildTarget[i].transform.SetLocalPosition(0f, 0f, 0f);
        }
    }
}
