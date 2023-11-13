using UnityEngine;
public class OverlayMainCamera : MonoBehaviour {
    [SerializeField]
    [Header("Baseになるカメラ")]
    private Camera baseCamera;
    public Camera BaseCamera => baseCamera;
}
