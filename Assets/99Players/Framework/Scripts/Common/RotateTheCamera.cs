using UnityEngine;
public class RotateTheCamera : MonoBehaviour {
    [SerializeField]
    private Vector3 axis;
    [SerializeField]
    private float speed;
    private void Update() {
        base.transform.Rotate(axis * speed);
    }
}
