using System.Collections;
using UnityEngine;
public class AlpineSkiing_Camera : MonoBehaviour {
    [SerializeField]
    [Header("追尾タ\u30fcゲット")]
    private GameObject objTarget;
    [SerializeField]
    [Header("対象のAlpineSkiing_SkiBoard")]
    private AlpineSkiing_SkiBoard skiBoard;
    [SerializeField]
    [Header("RadialBlurEffect")]
    private RadialBlurEffect radialBlurEffect;
    [SerializeField]
    [Header("ゴ\u30fcル時カメラアンカ\u30fc")]
    private Transform goalCameraAnchor;
    private Camera _camera;
    private float cameraFoV;
    private Vector3 targetPos;
    private float moveCompSpeed = 0.275f;
    private float rotCompSpeed = 0.3f;
    private Vector3 relativePos;
    private Quaternion rotation;
    private bool isAddSpeed;
    private bool isAccel;
    private float addSpeed;
    private bool isGoalCameraUp;
    private bool isStartCoroutine;
    private Vector3 tempPos;
    private void Start() {
        if (radialBlurEffect != null) {
            radialBlurEffect.blurDegree = 0f;
        }
        _camera = GetComponent<Camera>();
        cameraFoV = _camera.fieldOfView;
    }
    public void RadialBlur(bool active, float _set = 0.02f, bool autoOFF = true) {
        if (active) {
            LeanTween.value(base.gameObject, 0f, _set, 0.15f).setOnUpdate(RadialBlurSet);
            skiBoard.radioControl.SpeedLineStart();
            if (autoOFF) {
                LeanTween.value(base.gameObject, _set, 0f, 0.5f).setOnUpdate(RadialBlurSet).setDelay(0.3f);
                StartCoroutine(SpeedLineEndWait());
            }
        } else {
            LeanTween.value(base.gameObject, radialBlurEffect.blurDegree, 0f, 0.6f).setOnUpdate(RadialBlurSet);
            skiBoard.radioControl.SpeedLineEnd();
        }
    }
    private void RadialBlurSet(float _set) {
        radialBlurEffect.blurDegree = _set;
    }
    public void MotionBlur(bool active, bool autoOFF = false) {
        if (active) {
            LeanTween.value(base.gameObject, cameraFoV, cameraFoV - 20f, 0.6f).setOnUpdate(FovSet);
            if (autoOFF) {
                LeanTween.value(base.gameObject, cameraFoV - 20f, cameraFoV, 0.6f).setOnUpdate(FovSet).setDelay(1f);
            }
        } else {
            LeanTween.value(base.gameObject, _camera.fieldOfView, cameraFoV, 0.6f).setOnUpdate(FovSet);
        }
    }
    private void FovSet(float _set) {
        _camera.fieldOfView = _set;
    }
    public void Init() {
        targetPos = objTarget.transform.position + objTarget.transform.forward * -2.1f;
        targetPos.y += 1.2f;
        base.transform.position = targetPos;
        relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
        base.transform.rotation = Quaternion.LookRotation(relativePos);
        base.transform.SetLocalEulerAnglesX(25f);
        addSpeed = 0f;
    }
    private void FixedUpdate() {
        if (skiBoard.processType == AlpineSkiing_SkiBoard.SkiBoardProcessType.GOAL) {
            moveCompSpeed = 0.065f;
            if (isGoalCameraUp) {
                targetPos = goalCameraAnchor.position;
            } else {
                if (!isStartCoroutine) {
                    StartCoroutine(IsGoalCameraUpWait());
                }
                switch (skiBoard.CameraPos) {
                    case AlpineSkiing_SkiBoard.CameraPosType.NEAR:
                        tempPos = objTarget.transform.position + objTarget.transform.forward * -1.1f;
                        tempPos.y += 1.5f;
                        break;
                    case AlpineSkiing_SkiBoard.CameraPosType.NORMAL:
                        tempPos = objTarget.transform.position + objTarget.transform.forward * -2.1f;
                        tempPos.y += 1.5f;
                        break;
                    case AlpineSkiing_SkiBoard.CameraPosType.DISTANT:
                        tempPos = objTarget.transform.position + objTarget.transform.forward * -3.1f;
                        tempPos.y += 2f;
                        break;
                }
                targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
            }
        } else if (skiBoard.processType != 0) {
            switch (skiBoard.CameraPos) {
                case AlpineSkiing_SkiBoard.CameraPosType.NEAR:
                    tempPos = objTarget.transform.position + objTarget.transform.forward * -1.1f;
                    tempPos.y += 0.6f;
                    break;
                case AlpineSkiing_SkiBoard.CameraPosType.NORMAL:
                    tempPos = objTarget.transform.position + objTarget.transform.forward * -2.1f;
                    tempPos.y += 1.2f;
                    break;
                case AlpineSkiing_SkiBoard.CameraPosType.DISTANT:
                    tempPos = objTarget.transform.position + objTarget.transform.forward * -3.1f;
                    tempPos.y += 2f;
                    break;
            }
            targetPos = Vector3.Slerp(targetPos, tempPos, 0.175f);
        }
        base.transform.position = Vector3.Slerp(base.transform.position, targetPos, moveCompSpeed);
        relativePos = objTarget.transform.position + Vector3.up * 0.35f - base.transform.position;
        rotation = Quaternion.LookRotation(relativePos);
        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, rotCompSpeed);
        if (base.transform.localEulerAngles.x <= 25f) {
            base.transform.SetLocalEulerAnglesX(25f);
        }
    }
    public void AddSpeed() {
        isAddSpeed = true;
        isAccel = false;
        addSpeed = 0f;
    }
    private IEnumerator SpeedLineEndWait() {
        yield return new WaitForSeconds(0.5f);
        skiBoard.radioControl.SpeedLineEnd();
    }
    private IEnumerator IsGoalCameraUpWait() {
        isStartCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        isGoalCameraUp = true;
    }
}
