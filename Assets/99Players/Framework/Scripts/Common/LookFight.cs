using UnityEngine;
public class LookFight : MonoBehaviour {
    private Vector3 cameraPos;
    private void Update() {
        cameraPos = SingletonCustom<SwordFight_CameraMover>.Instance.TargetPos;
        cameraPos.y = base.transform.position.y;
        base.transform.LookAt(cameraPos);
    }
}
