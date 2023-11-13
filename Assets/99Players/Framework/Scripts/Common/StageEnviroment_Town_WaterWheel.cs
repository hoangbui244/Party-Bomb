using UnityEngine;
public class StageEnviroment_Town_WaterWheel : MonoBehaviour {
    [SerializeField]
    [Header("回転速度")]
    private float ROTATION_SPEED = -40f;
    private Vector3 axis;
    private void Start() {
        axis = Vector3.forward;
        UnityEngine.Debug.Log("Windmill blade axis : " + axis.ToString());
    }
    private void Update() {
        Quaternion quaternion = Quaternion.AngleAxis(ROTATION_SPEED * Time.deltaTime, axis);
        base.transform.rotation *= quaternion;
        UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + axis * 6f, Color.cyan);
        UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.right * 2f, Color.red);
        UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.up * 2f, Color.green);
        UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.back * 2f, Color.blue);
    }
}
