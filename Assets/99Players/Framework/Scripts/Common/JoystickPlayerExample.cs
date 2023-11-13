using UnityEngine;
public class JoystickPlayerExample : MonoBehaviour {
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    public void FixedUpdate() {
        Vector3 a = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        rb.AddForce(a * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
