using UnityEngine;
public class AlpineSkiing_Character : MonoBehaviour {
    [SerializeField]
    [Header("キャラの親アンカ\u30fc")]
    public GameObject characterAnchor;
    [SerializeField]
    [Header("足元の地面座標取得用アンカ\u30fc")]
    private Transform downAnchor;
    [SerializeField]
    [Header("前方の地面座標取得用アンカ\u30fc")]
    private Transform forwardAnchor;
    private RaycastHit Hit;
    private Vector3 angle;
    private Quaternion targetAngle;
    private float angleY;
    private int layerMask = 1048576;
    public Vector3 angleAcc;
    public bool isForward;
    private void Update() {
        if (Physics.Raycast(forwardAnchor.position, Vector3.down, out Hit, 10f, layerMask)) {
            angle = Hit.point - downAnchor.position;
        }
        UnityEngine.Debug.DrawRay(forwardAnchor.position, Vector3.down * 10f, Color.red);
        targetAngle = Quaternion.LookRotation(angle, Vector3.up);
        angleY = targetAngle.eulerAngles.x;
        if (angleY <= 180f) {
            angleY = Mathf.Clamp(angleY, 0f, 30f);
        } else {
            angleY = Mathf.Clamp(angleY, 330f, 360f);
        }
        characterAnchor.transform.SetLocalEulerAnglesX(angleY);
        if (Hit.point.y <= downAnchor.position.y + 0.02f) {
            isForward = true;
            angleAcc = (Hit.point - downAnchor.position).normalized;
        } else {
            isForward = false;
            angleAcc = -(Hit.point - downAnchor.position).normalized;
        }
    }
}
