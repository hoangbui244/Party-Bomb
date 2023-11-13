using UnityEngine;
[RequireComponent(typeof(Camera))]
public class GhostFreeRoamCamera : MonoBehaviour {
    public float initialSpeed = 10f;
    public float increaseSpeed = 1.25f;
    public bool allowMovement = true;
    public bool allowRotation = true;
    public KeyCode forwardButton = KeyCode.W;
    public KeyCode backwardButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    public KeyCode leftButton = KeyCode.A;
    public float cursorSensitivity = 0.025f;
    public bool cursorToggleAllowed = true;
    public KeyCode cursorToggleButton = KeyCode.Escape;
    private float currentSpeed;
    private bool moving;
    private bool togglePressed;
    private void OnEnable() {
        if (cursorToggleAllowed) {
            Screen.lockCursor = true;
            Cursor.visible = false;
        }
    }
    private void Update() {
        if (allowMovement) {
            bool flag = moving;
            Vector3 deltaPosition = Vector3.zero;
            if (moving) {
                currentSpeed += increaseSpeed * Time.deltaTime;
            }
            moving = false;
            CheckMove(forwardButton, ref deltaPosition, base.transform.forward);
            CheckMove(backwardButton, ref deltaPosition, -base.transform.forward);
            CheckMove(rightButton, ref deltaPosition, base.transform.right);
            CheckMove(leftButton, ref deltaPosition, -base.transform.right);
            if (moving) {
                if (moving != flag) {
                    currentSpeed = initialSpeed;
                }
                base.transform.position += deltaPosition * currentSpeed * Time.deltaTime;
            } else {
                currentSpeed = 0f;
            }
        }
        if (allowRotation) {
            Vector3 eulerAngles = base.transform.eulerAngles;
            eulerAngles.x += (0f - UnityEngine.Input.GetAxis("Mouse Y")) * 359f * cursorSensitivity;
            eulerAngles.y += UnityEngine.Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
            base.transform.eulerAngles = eulerAngles;
        }
        if (cursorToggleAllowed) {
            if (UnityEngine.Input.GetKey(cursorToggleButton)) {
                if (!togglePressed) {
                    togglePressed = true;
                    Screen.lockCursor = !Screen.lockCursor;
                    Cursor.visible = !Cursor.visible;
                }
            } else {
                togglePressed = false;
            }
        } else {
            togglePressed = false;
            Cursor.visible = false;
        }
    }
    private void CheckMove(KeyCode keyCode, ref Vector3 deltaPosition, Vector3 directionVector) {
        if (UnityEngine.Input.GetKey(keyCode)) {
            moving = true;
            deltaPosition += directionVector;
        }
    }
}
