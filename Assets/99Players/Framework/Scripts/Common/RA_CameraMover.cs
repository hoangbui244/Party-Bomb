using UnityEngine;
public class RA_CameraMover : MonoBehaviour {
    public enum State {
        STANBDY,
        START_ZOOM,
        CROWN_ZOOM,
        WINNER_ZOOM,
        THREE_OR_MORE_ZOOM
    }
    private Vector3 targetPos = new Vector3(0f, 1.765f, -3.287f);
    private Vector3 targetPosThreeOrMore = new Vector3(0f, 1.589f, -1.287f);
    private Vector3 crownPos = new Vector3(0f, 1.665f, -1.587f);
    private Vector3 winnerPos = new Vector3(0f, 1.7f, -1.537f);
    private State currentState;
    private float moveCompSpeed = 0.015f;
    public bool IsMove {
        get;
        set;
    }
    public void SetCameraState(State _state) {
        currentState = _state;
        switch (currentState) {
            case State.CROWN_ZOOM:
                moveCompSpeed = 0.015f;
                targetPos = crownPos;
                break;
            case State.THREE_OR_MORE_ZOOM:
                moveCompSpeed = 0.015f;
                targetPos = targetPosThreeOrMore;
                break;
            case State.WINNER_ZOOM:
                targetPos = winnerPos;
                break;
        }
    }
    private void FixedUpdate() {
        switch (currentState) {
            case State.STANBDY:
                break;
            case State.START_ZOOM:
                base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, targetPos, moveCompSpeed);
                base.transform.SetLocalEulerAnglesX(Mathf.SmoothStep(base.transform.localEulerAngles.x, 40f, Time.deltaTime * 7f));
                break;
            case State.CROWN_ZOOM:
                base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, targetPos, moveCompSpeed);
                break;
            case State.THREE_OR_MORE_ZOOM:
                base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, targetPos, moveCompSpeed);
                break;
            case State.WINNER_ZOOM:
                base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, targetPos, moveCompSpeed);
                break;
        }
    }
}
