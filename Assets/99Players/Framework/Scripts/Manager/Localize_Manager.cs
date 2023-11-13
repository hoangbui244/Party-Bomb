using UnityEngine;
public class Localize_Manager : MonoBehaviour {
    [SerializeField]
    [Header("ル\u30fcト")]
    private Transform root;
    [SerializeField]
    [Header("Awakeで設定するかどうか")]
    private bool isAwake = true;
    private void Awake() {
        if (isAwake) {
            Set();
        }
    }
    public void Set() {
        UnityEngine.Debug.Log("<color=red>Localize Language : " + Localize_Define.Language.ToString() + "</color>");
        if (Localize_Define.Language != 0 && !(root == null)) {
            Localize_Target[] componentsInChildren = root.GetComponentsInChildren<Localize_Target>(includeInactive: true);
            for (int i = 0; i < componentsInChildren.Length; i++) {
                UnityEngine.Debug.Log("LocalizeTarget " + componentsInChildren[i]?.ToString());
                componentsInChildren[i].Set();
            }
        }
    }
}
