using UnityEngine;
public class AddRotation : MonoBehaviour {
    [SerializeField]
    [Header("回転速度")]
    private Vector3 rotation;
    private void Update() {
        base.transform.AddLocalEulerAnglesX(rotation.x);
        base.transform.AddLocalEulerAnglesY(rotation.y);
        base.transform.AddLocalEulerAnglesZ(rotation.z);
    }
}
